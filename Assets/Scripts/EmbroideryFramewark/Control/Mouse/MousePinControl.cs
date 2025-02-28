using EmbroideryFramewark.Interface;
using Obi;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// ���ⲿ�������
    /// </summary>
    public class MousePinControl : IEmbroideryOp
    {
        private EEmbroideryOperation _currentState = EEmbroideryOperation.MOUSE;

        public EEmbroideryOperation OpState { get { return _currentState; } }


        private EMouseOperationState _mouseOpState = EMouseOperationState.NONE_OP;

        /// <summary>
        /// ��Ҫ
        /// ���������λ�����
        /// </summary>
        private Transform _pinOperatorTrans = null;


        public MousePinControl()
        {
            _pinOperatorTrans = PinManager.Instance.CurrentPinOperator.transform;
        }

        //��ʱû��
        public void JoinInManager()
        {
            EmbroideryOpMethedManager.Instance.AddOperation(_currentState, this);
        }

        /// <summary>
        /// ʹ�õĺ���
        /// </summary>
        public void Run()
        {
            if (object.ReferenceEquals(_pinOperatorTrans, null))
            {
                Debug.LogError("MousePinControl�������ܵõ�PinOperator��");
                return;
            }

            JudgeState();
            GetInput();
        }


        #region MouseInput����ѭ��ʹ�õĺ�����
        /// <summary>
        /// �����Ҽ��ƶ��������
        /// ��������ֱ�ӿ�ʼ����
        /// </summary>
        private void JudgeState()
        {
            //Debug.Log("isMovePin:" + m_MouseInput.IsMovePin);
            //Debug.Log("NoCtrl:" + m_MouseInput.IsNotCtrl);


            if (m_MouseInput.IsMovePin && !_isConformPin)
            {
                _mouseOpState = EMouseOperationState.MOVE_PIN;
                return;
            }

            //�Ը���ΪNone
            if (m_MouseInput.IsConformPinPoint || _isConformPin)
            {
                _mouseOpState = EMouseOperationState.CONFORM_POINT;
                return;
            }

            _mouseOpState = EMouseOperationState.NONE_OP;

        }

        private bool _isConformPin = false;

        private void GetInput()
        {
            switch (this._mouseOpState)
            {
                case EMouseOperationState.NONE_OP:
                    {
                        //Debug.Log("û�в���");
                        break;
                    }
                case EMouseOperationState.MOVE_PIN:
                    {
                        MovePin();
                        break;
                    }
                case EMouseOperationState.CONFORM_POINT:
                    {
                        if (!_isConformPin)
                        {
                            _isConformPin = true;
                            ConformPinPoint();
                        }

                        ///�����������
                        PinManager.Instance.CurrentPinOperator.transform.position =
                            EmbroideryOpMethedManager.Instance._follow.transform.position;

                        break;
                    }
            }
        }



        #endregion


        #region MovePin

        private RaycastHit[] _raycastHits = new RaycastHit[5];

        /// <summary>
        /// ��Բ��ϵľ���
        /// </summary>
        private float _pinAboveDistance = 0.4f;

        /// <summary>
        /// ����
        /// </summary>
        private Quaternion up = Quaternion.Euler(90,0,0);
        private Quaternion down = Quaternion.Euler(-90, 0, 0);


        private void MovePin()
        {
            int checkNum = Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(Input.mousePosition), _raycastHits,100f,LayerMask.GetMask("BuLiao"));
            if (checkNum > 0)
            {
                _pinOperatorTrans.position = _raycastHits[0].point + Vector3.up * _pinAboveDistance * PinManager.Instance.PinSide;
                _pinOperatorTrans.rotation = (PinManager.Instance.PinSide > 0 ? this.up : this.down);
            }
        }

        #endregion


        #region ȷ������㣨���嶯�����ŵȣ�

        StringBuilder _animName = new StringBuilder();


        ///������״̬��1.���¡�2.������3.�ָ�λ�ã�����λ�ã�
        /// <summary>
        /// ȷ�ϴ���㣬���Ŷ���
        /// </summary>
        private void ConformPinPoint()
        {
            MonoHelper.Instance.StartCoroutine(StartEmbroideryOp(PinManager.Instance.PinSide));
        }

        private string _downToUp = "DownToUp";
        private string _upToDown = "UpToDown";

        StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// Я�̿��ƴ����ÿһ������Ľ���
        /// </summary>
        /// <param name="pinSide"></param>
        /// <returns></returns>
        IEnumerator StartEmbroideryOp(float pinSide)
        {
            //ͬ�������  
            EmbroideryOpMethedManager.Instance._root.transform.position 
                = PinPointVisualization.Instance.FlagTransform.position;

            stringBuilder.Clear();
            stringBuilder.Append("Enter_");
            stringBuilder.Append((pinSide > 0 ? _upToDown : _downToUp));

            Debug.Log(stringBuilder.ToString());
            EmbroideryOpMethedManager.Instance.pinMoveAnim.Play(stringBuilder.ToString());

            yield return new WaitForSeconds(GetAnimationRunTime(stringBuilder.ToString()));

            stringBuilder.Clear();
            stringBuilder.Append("Shrink_");
            stringBuilder.Append((pinSide > 0 ? _upToDown : _downToUp));

            Debug.Log(stringBuilder.ToString());
            EmbroideryOpMethedManager.Instance.pinMoveAnim.Play(stringBuilder.ToString());

            yield return new WaitForSeconds(GetAnimationRunTime(stringBuilder.ToString()));

            stringBuilder.Clear();
            stringBuilder.Append("Back_");
            stringBuilder.Append((pinSide > 0 ? _upToDown : _downToUp));

            Debug.Log(stringBuilder.ToString());
            EmbroideryOpMethedManager.Instance.pinMoveAnim.Play(stringBuilder.ToString());

            yield return new WaitForSeconds(GetAnimationRunTime(stringBuilder.ToString()));

            ///-----------------�л�root��position----------------------
            
            EmbroideryOpMethedManager.Instance.pinMoveAnim.Stop();

            EmbroideryOpMethedManager.Instance._root.transform.position = 
                EmbroideryOpMethedManager.Instance._follow.transform.position;

            _isConformPin = false;
            this._mouseOpState = EMouseOperationState.NONE_OP;
        }

        private float GetAnimationRunTime(string name)
        {
            return EmbroideryOpMethedManager.Instance.animTimeDict[name];
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(EmbroideryOpMethedManager.Instance._follow.transform.position,1f);
        }

        #endregion
    }


    enum EMouseOperationState
    {
        //û�в���
        NONE_OP= 0,

        //�����ƶ���
        MOVE_PIN = 1,

        //ȷ�ϴ����
        CONFORM_POINT = 2
    }

}