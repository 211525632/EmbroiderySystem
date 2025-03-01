using Obi;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 1.反复使用两个至三个 RopeHelper来完成绳子的模拟
    /// 2.完成刺绣的RopeHelper在结束之后，会先保存当前的Mesh
    /// 3.之后继续担任下一次绳子模拟的RopeHelper
    /// </summary>

    [RequireComponent(typeof(SingleRopeHelper))]
    [RequireComponent(typeof(RopePool))]

    public class RopeManager : MonoSingleton<RopeManager>
    {
        [Header("后续使用AB包实现：")]

        ///如果需要调整线条的材质，则需要调整这个模板的材质
        [SerializeField] public GameObject _ropeModel;

        [SerializeField] public GameObject _obiSolverModel;

        [SerializeField] public GameObject _empty;


        public GameObject _objModelRoot { get; private set; }



        #region RopeHelper池
        /// <summary>
        /// 只能从池中获取RopeHelper
        /// </summary>
        private RopePool _ropePool;

        /// <summary>
        /// TODO：改成反复使用
        /// 当前绳子的操作
        /// </summary>
        public SingleRopeHelper CurrentRopeHelper
        {
            get => this._ropePool.CurrentRopeHelper;
        }

        /// <summary>
        /// 上一个绳子的操作
        /// </summary>
        public SingleRopeHelper PreferRopeHelper
        {
            get => this._ropePool.PreRopeHelper;
        }


        public void ResetRope()
        {

        }


        #endregion


        public float RopeRidus
        {
            get => _ropePool.RopeRidus;
        }


        protected override void Awake()
        {
            base.Awake();
            //_objRoot = Instantiate<GameObject>(_obiSolverModel);
            //_objRoot.name = "Root_"+_obiSolverModel.name;


            _objModelRoot = Instantiate<GameObject>(_empty);
            _objModelRoot.name = "Root_RopePool";


            _ropePool = new();

            SetEvent();
        }

        private void Start()
        {
            
        }


        /// <summary>
        /// 创建新的绳子
        /// 会更新CurrentRope和PreferRope的值
        /// </summary>
        /// <param name="ropeBeginPosition"></param>
        /// <param name="ropeEnd"></param>
        /// <param name="ropeLength"></param>
        public void CreateRope(Vector3 ropeBeginPosition, Vector3 ropeEnd, float ropeLength)
        {
            CreateRope(ropeBeginPosition, ropeEnd, ropeLength,null);
        }

        public void CreateRope(Vector3 ropeBeginPosition, Vector3 ropeEnd, float ropeLength,Material ropeMaterial)
        {

            _ropePool.ReturnPreRope();

            _ropePool.BorrowRopeAndInit(ropeBeginPosition, ropeEnd, ropeLength);

            CurrentRopeHelper.SetEndLoackState(true);

            if (!object.ReferenceEquals(null, ropeMaterial))
            {
                CurrentRopeHelper.GetComponentInChildren<Renderer>().sharedMaterial = ropeMaterial;
            }
        }


        #region RopeHelper的模型化

        /// <summary>
        /// 将指定序号的Rope替换成Model
        /// 
        /// 就是需要RopeHelper
        /// </summary>
        public GameObject RopeChangeToModel()
        {
            GameObject model = this.PreferRopeHelper.
                GetRopeModelObj();
            //静态批处理
            model.isStatic = true;

            return model;
        }

        #endregion

        #region 绳子的隐藏

        private Vector3 _hidePosition = Vector3.down * 1000f;
        public void HideOrActiveCurrentRope(bool isActive)
        {
            _ropePool.HideOrActiveCurrentRope(isActive);
        }

        public void HideOrActivePreRope(bool isActive)
        {
            _ropePool.HideOrActivePreRope(isActive);
        }

        public void HideOrActiveAfterRope(bool isActive)
        {
            _ropePool.HideOrActiveAfterRope(isActive);
        }

        #endregion


        #region 根据要生成Rope处于布料的位置生成绳子的Begin与End

        /// <summary>
        /// TODO：将0.01f与布料控制器关联起来
        /// 根据要生成Rope处于布料的位置生成绳子的Begin与End
        /// </summary>
        /// <param name="origin">   初定的刺绣点</param>
        /// <param name="side">     这个绳子的方位</param>
        /// <param name="begin">    生成绳子的beign</param>
        /// <param name="end">      生成绳子的End</param>
        public void CreateRopePointPositionWithSide(Vector3 origin,float side,out Vector3 begin,out Vector3 end)
        {
            ///设置新生成的绳子的初始位置
             begin = new Vector3(origin.x, 0.01f * -side, origin.z);
             end = new Vector3(origin.x, 0.01f * -side, origin.z);
        }


        #endregion

        #region 事件设置

        public void SetEvent()
        {
            //EventCenter<Action>.Instance.AddEvent(EventConst.OnPinEndEnter, _ropePool.ReturnPreRope);
        }

        #endregion

    }
}