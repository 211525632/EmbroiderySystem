using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace EmbroideryFramewark
{
    public class GameManager : MonoBehaviour
    {
        [Header("布料的相关信息：(在AB包中实现)")]
        private GameObject BuLiao;

        [Header("布料的宽度")]
        [SerializeField] float _buLiaoWidth = 0.01f;


        [Header("测试用绳子初始化长度：")]
        [SerializeField] float testLength = 1;



        private Vector3 collidPoistion;

        private void Start()
        {
            ///初始化布料
            BuLiao = GameObject.Find("BuLiao");
            if (object.ReferenceEquals(BuLiao, null)) Debug.LogError("没有找到BuLiao!");

            ///创建初始绳子
            RopeManager.Instance.CreateRope( Vector3.up ,Vector3.zero, _buLiaoWidth);
            RopeManager.Instance.CurrentRopeHelper.SetBeginBoundingObject(PinManager.Instance.CurrentPinHelper._endTransform);
            RopeManager.Instance.CurrentRopeHelper.SetRopeLengthTo(testLength);

            ///设置针
            PinManager.Instance.SetPinTransform(Vector3.up);

            //设置可视化范围（设置布料）
            PinPointVisualization.Instance.SetBuLiao(this.BuLiao.transform.position);

            SetEvent();
        }

        public float shrinkLength = 0.1f;

        private void FixedUpdate()
        {
            ///收缩完成判断

            //Debug.Log("distance:" + RopeManager.Instance.PreferRopeHelper.RopePointDistance + "  RopeLength:"+RopeManager.Instance.PreferRopeHelper.RopeLength);

            if (PinManager.Instance.CurrentPinHelper._isShrink
                && RopeManager.Instance.PreferRopeHelper?.RopeLength <= 0.1f)
            {
                this._preLength = 0f;

                StartCoroutine(WaitToNextFrame());

                EventCenter<Action>.Instance.GetAction(EventConst.OnRopeShrinkComplete)?.Invoke();


                return;
            }

            ///收缩中,第二个作用是使其脱离BeginEnter的吸附效果
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
                    //    //将其移出Flag的检测启动范围
                    //    PinManager.Instance.CurrentPinOperator.transform.position -= Vector3.up * 
                    //    break;
                    //}

            }

        }


        IEnumerator WaitToNextFrame()
        {
            yield return null;

            RopeManager.Instance.PreferRopeHelper.SetRopeLengthTo(-1f);
            ///此时已经重构了rope的顺序
            RopeManager.Instance.CurrentRopeHelper.SetRopeLengthTo(1.2f);
        }


        #region 事件设置

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
        /// TODO:将初始点与终止点由Flag的坐标来决定
        /// </summary>
        /// <param name="target">   针脚坐标</param>
        private void SetNewRope(Vector3 target)
        {
            if (object.ReferenceEquals(this.BuLiao, null))
            {
                Debug.LogError("布料没有被设置！");
                return;
            }

            float x = PinPointVisualization.Instance.CalculateAdsorbedNumber(target.x);
            float z = PinPointVisualization.Instance.CalculateAdsorbedNumber(target.z);

            Vector3 ordinaryPosition = new Vector3(x,0,z);

            ///设置新生成的绳子的初始位置
            Vector3 tempBegin = Vector3.zero;
            Vector3 tempEnd = Vector3.zero;
            RopeManager.Instance.CreateRopePointPositionWithSide(
                ordinaryPosition, PinManager.Instance.PinSide
                ,out tempBegin,out tempEnd);

            //创建新绳子
            RopeManager.Instance.CreateRope(tempBegin, tempEnd, _buLiaoWidth);
            RopeManager.Instance.CurrentRopeHelper.SetBeginBoundingObject(PinManager.Instance.CurrentPinHelper._endTransform);
            
            //规格化新绳子
            RopeManager.Instance.PreferRopeHelper.SetBeginBoundingObject(null);
                //调整到同样高度
            tempEnd.y = RopeManager.Instance.PreferRopeHelper._endTransform.position.y;
            RopeManager.Instance.PreferRopeHelper.SetBeginPosition(tempEnd);
        }


        /// <summary>
        /// TODO:还可以添加”刺绣操作的撤销操作“所需要的对象池与快照相关函数
        /// </summary>
        private void ChangeToModelAndSaveOperation()
        {
            GameObject PreRopeModel = RopeManager.Instance.RopeChangeToModel();
            EmbroideryOpSaverCtl.Instance.SaveNewOp(RopeManager.Instance.PreferRopeHelper, PreRopeModel);
        }

        /// <summary>
        /// 记录之前的length
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