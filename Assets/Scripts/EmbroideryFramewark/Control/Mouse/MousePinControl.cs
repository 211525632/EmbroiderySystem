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
    /// 在外部进行添加
    /// </summary>
    public class MousePinControl : IEmbroideryOp
    {
        private EEmbroideryOperation _currentState = EEmbroideryOperation.MOUSE;

        public EEmbroideryOperation OpState { get { return _currentState; } }


        private EMouseOperationState _mouseOpState = EMouseOperationState.NONE_OP;

        /// <summary>
        /// 必要
        /// 针控制器的位置组件
        /// </summary>
        private Transform _pinOperatorTrans = null;


        public MousePinControl()
        {
            _pinOperatorTrans = PinManager.Instance.CurrentPinOperator.transform;
        }

        //暂时没用
        public void JoinInManager()
        {
            EmbroideryOpMethedManager.Instance.AddOperation(_currentState, this);
        }

        /// <summary>
        /// 使用的函数
        /// </summary>
        public void Run()
        {
            if (object.ReferenceEquals(_pinOperatorTrans, null))
            {
                Debug.LogError("MousePinControl：：不能得到PinOperator！");
                return;
            }

            JudgeState();
            GetInput();
        }


        #region MouseInput（主循环使用的函数）
        /// <summary>
        /// 长按右键移动针操作器
        /// 点击左键就直接开始刺绣
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

            //自跟新为None
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
                        //Debug.Log("没有操作");
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

                        ///针操作器跟随
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
        /// 相对布料的距离
        /// </summary>
        private float _pinAboveDistance = 0.4f;

        /// <summary>
        /// 缓存
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


        #region 确定刺绣点（刺绣动画播放等）

        StringBuilder _animName = new StringBuilder();


        ///动画的状态：1.刺下、2.拉扯、3.恢复位置（反面位置）
        /// <summary>
        /// 确认刺绣点，播放动画
        /// </summary>
        private void ConformPinPoint()
        {
            MonoHelper.Instance.StartCoroutine(StartEmbroideryOp(PinManager.Instance.PinSide));
        }

        private string _downToUp = "DownToUp";
        private string _upToDown = "UpToDown";

        StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// 携程控制刺绣的每一个步骤的进行
        /// </summary>
        /// <param name="pinSide"></param>
        /// <returns></returns>
        IEnumerator StartEmbroideryOp(float pinSide)
        {
            //同步刺绣点  
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

            ///-----------------切换root的position----------------------
            
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
        //没有操作
        NONE_OP= 0,

        //正在移动针
        MOVE_PIN = 1,

        //确认刺绣点
        CONFORM_POINT = 2
    }

}