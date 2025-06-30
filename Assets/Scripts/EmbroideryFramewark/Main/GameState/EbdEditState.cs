using System;
using System.Collections;
using UnityEngine;

namespace EmbroideryFramewark
{
    public class EbdEditState : IEbdGameState
    {
        [Header("测试用绳子初始化长度：")]
        [SerializeField] float testLength = 1;

        /// <summary>
        /// 针穿过布料的位置
        /// </summary>
        private Vector3 collidPoistion;

        /// <summary>
        /// 收缩到多长就会认定为收缩完成
        /// </summary>
        public float shrinkLength = 0.1f;



        #region GameState相关

        private readonly EEbdGameState _state = EEbdGameState.EditMode;

        public EEbdGameState GameState => _state;

        public void OnStateEnter()
        {
            BuLiaoData buLiaoData   = BuLiaoManager.Instance.GetBuLiaoData();
            float buLiaoWidth       = BuLiaoManager.Instance.GetBuLiaoWidth();

            /// 创建初始绳子
            RopeManager.Instance.CreateRope(Vector3.up + buLiaoData.position, buLiaoData.position, buLiaoWidth);
            RopeManager.Instance.CurrentRopeHelper.SetBeginBoundingObject(PinManager.Instance.CurrentPinHelper._endTransform);
            RopeManager.Instance.CurrentRopeHelper.SetRopeLengthTo(testLength);

            ///设置针
            PinManager.Instance.SetPinTransform(Vector3.up);

            //设置可视化范围（设置布料）
            PinPointVisualization.Instance.SetBuLiao(buLiaoData.position);

            SetEvent();
        }



        public void OnStateExit()
        {
            Debug.Log("Exit Editor");
            ///注销事件
            RopeManager.Instance.ResetAllRope();
            
            ///终止所有的刺绣行为
        }



        public void OnStateUpdate() { }


        public void OnStateFixedUpdate()
        {
            ///收缩完成判断

            if (PinManager.Instance.CurrentPinHelper._isShrink
                && RopeManager.Instance.PreferRopeHelper?.RopeLength <= 0.1f)
            {
                this._preLength = 0f;

                MonoHelper.Instance.StartCoroutine(WaitToNextFrame());

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



        public void OnStateLateUpdate() { }

        #endregion



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
            float buliaoWidth = BuLiaoManager.Instance.GetBuLiaoWidth();

            //将标记的位置作为创建绳子的标记
            float x = PinPointVisualization.Instance.CalculateAdsorbedNumber(target.x);
            float z = PinPointVisualization.Instance.CalculateAdsorbedNumber(target.z);

            Vector3 flagPosition = new Vector3(x, 0, z);

            ///设置新生成的绳子的初始位置
            Vector3 tempBegin = Vector3.zero;
            Vector3 tempEnd = Vector3.zero;
            RopeManager.Instance.CreateRopePointPositionWithSide(
                flagPosition, PinManager.Instance.PinSide
                , out tempBegin, out tempEnd);

            //创建新绳子
            RopeManager.Instance.CreateRope(tempBegin, tempEnd, buliaoWidth);
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
            EbdShortMemeryCtrl.Instance.SaveNewOp(RopeManager.Instance.PreferRopeHelper, PreRopeModel);
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
