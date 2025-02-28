using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace EmbroideryFramewark
{
    public class GameManager : MonoBehaviour
    {
        [Header("���ϵ������Ϣ��(��AB����ʵ��)")]
        private GameObject BuLiao;

        [Header("���ϵĿ��")]
        [SerializeField] float _buLiaoWidth = 0.01f;


        [Header("���������ӳ�ʼ�����ȣ�")]
        [SerializeField] float testLength = 1;



        private Vector3 collidPoistion;

        private void Start()
        {
            ///��ʼ������
            BuLiao = GameObject.Find("BuLiao");
            if (object.ReferenceEquals(BuLiao, null)) Debug.LogError("û���ҵ�BuLiao!");

            ///������ʼ����
            RopeManager.Instance.CreateRope( Vector3.up ,Vector3.zero, _buLiaoWidth);
            RopeManager.Instance.CurrentRopeHelper.SetBeginBoundingObject(PinManager.Instance.CurrentPinHelper._endTransform);
            RopeManager.Instance.CurrentRopeHelper.SetRopeLengthTo(testLength);

            ///������
            PinManager.Instance.SetPinTransform(Vector3.up);

            //���ÿ��ӻ���Χ�����ò��ϣ�
            PinPointVisualization.Instance.SetBuLiao(this.BuLiao.transform.position);

            SetEvent();
        }

        public float shrinkLength = 0.1f;

        private void FixedUpdate()
        {
            ///��������ж�

            //Debug.Log("distance:" + RopeManager.Instance.PreferRopeHelper.RopePointDistance + "  RopeLength:"+RopeManager.Instance.PreferRopeHelper.RopeLength);

            if (PinManager.Instance.CurrentPinHelper._isShrink
                && RopeManager.Instance.PreferRopeHelper?.RopeLength <= 0.1f)
            {
                this._preLength = 0f;

                StartCoroutine(WaitToNextFrame());

                EventCenter<Action>.Instance.GetAction(EventConst.OnRopeShrinkComplete)?.Invoke();


                return;
            }

            ///������,�ڶ���������ʹ������BeginEnter������Ч��
            if (PinManager.Instance.CurrentPinHelper._isShrink)
            {
                EventCenter<Action>.Instance.GetAction(EventConst.OnRopeShrink)?.Invoke();
                return;
            }

            switch (PinManager.Instance.CurrentPinState)
            {
                    case PinState.BeginExit:
                    {
                        EventCenter<Action>.Instance.GetAction(EventConst.OnPinBeginExit)?.Invoke();
                        break;
                    }
                    case PinState.BeginEnter:
                    {
                        EventCenter<Action>.Instance.GetAction(EventConst.OnPinBeginEnter)?.Invoke();
                        break;
                    }
                    case PinState.EndEnter:
                    {
                        collidPoistion = PinManager.Instance.CurrentPinHelper._endTransform.position;
                        EventCenter<Action<Vector3>>.Instance.GetAction(EventConst.OnPinEndEnter)?.Invoke(collidPoistion);
                        PinManager.Instance.CurrentPinHelper._isShrink = true;
                        EventCenter<Action>.Instance.GetAction(EventConst.OnPinEndEnter)?.Invoke();
                        break;
                    }
                    //case PinState.EndExit:
                    //{
                    //    //�����Ƴ�Flag�ļ��������Χ
                    //    PinManager.Instance.CurrentPinOperator.transform.position -= Vector3.up * 
                    //    break;
                    //}

            }

        }


        IEnumerator WaitToNextFrame()
        {
            yield return null;

            RopeManager.Instance.PreferRopeHelper.SetRopeLengthTo(-1f);
            ///��ʱ�Ѿ��ع���rope��˳��
            RopeManager.Instance.CurrentRopeHelper.SetRopeLengthTo(1.2f);
        }


        #region �¼�����

        private void SetEvent()
        {

            ///EndEnter
            EventCenter<Action<Vector3>>.Instance.AddEvent(EventConst.OnPinEndEnter, SetNewRope);


            ///OnShrink
            EventCenter<Action>.Instance.AddEvent(EventConst.OnRopeShrink, OnRopeShrink);

            //OnShrinkComplete
            EventCenter<Action>.Instance.AddEvent(EventConst.OnRopeShrinkComplete, ChangeToModelAndSaveOperation);

        }

        /// <summary>
        /// TODO:����ʼ������ֹ����Flag������������
        /// </summary>
        /// <param name="target">   �������</param>
        private void SetNewRope(Vector3 target)
        {
            if (object.ReferenceEquals(this.BuLiao, null))
            {
                Debug.LogError("����û�б����ã�");
                return;
            }

            float x = PinPointVisualization.Instance.CalculateAdsorbedNumber(target.x);
            float z = PinPointVisualization.Instance.CalculateAdsorbedNumber(target.z);

            Vector3 ordinaryPosition = new Vector3(x,0,z);

            ///���������ɵ����ӵĳ�ʼλ��
            Vector3 tempBegin = Vector3.zero;
            Vector3 tempEnd = Vector3.zero;
            RopeManager.Instance.CreateRopePointPositionWithSide(
                ordinaryPosition, PinManager.Instance.PinSide
                ,out tempBegin,out tempEnd);

            //����������
            RopeManager.Instance.CreateRope(tempBegin, tempEnd, _buLiaoWidth);
            RopeManager.Instance.CurrentRopeHelper.SetBeginBoundingObject(PinManager.Instance.CurrentPinHelper._endTransform);
            
            //���������
            RopeManager.Instance.PreferRopeHelper.SetBeginBoundingObject(null);
                //������ͬ���߶�
            tempEnd.y = RopeManager.Instance.PreferRopeHelper._endTransform.position.y;
            RopeManager.Instance.PreferRopeHelper.SetBeginPosition(tempEnd);
        }


        /// <summary>
        /// TODO:��������ӡ���������ĳ�������������Ҫ�Ķ�����������غ���
        /// </summary>
        private void ChangeToModelAndSaveOperation()
        {
            GameObject PreRopeModel = RopeManager.Instance.RopeChangeToModel();
            EmbroideryOpSaverCtl.Instance.SaveNewOp(RopeManager.Instance.PreferRopeHelper, PreRopeModel);
        }

        /// <summary>
        /// ��¼֮ǰ��length
        /// </summary>
        float _preLength = 0f;

        private void OnRopeShrink()
        {
            float moveLength = RopeManager.Instance.CurrentRopeHelper.RopePointDistance;
            if (_preLength > moveLength)
            {
                return;
            }

            float delta = moveLength - _preLength;

            _preLength = moveLength;


            RopeManager.Instance.PreferRopeHelper.SetRopeLength(-delta);
            RopeManager.Instance.CurrentRopeHelper.SetRopeLength(delta);
        }

        #endregion
    }

}