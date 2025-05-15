using Obi;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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


    public class RopeManager : MonoSingleton<RopeManager>
    {
        [Header("后续使用AB包实现：")]

        ///如果需要调整线条的材质，则需要调整这个模板的材质
        [SerializeField] public GameObject _ropeModel;

        [SerializeField] public GameObject _obiSolverModel;

        [SerializeField] public GameObject _empty;


        [Header("功能设置：")]
        [Header("穿过针线时，生成的绳子与布料之间的偏移量：")]

        ///TODO:增加一个 根据布料的位置 确定当前绳子的位置
        public float RopePointY = 0.01f;

        public GameObject _objModelRoot { get; private set; }

        public float RopeRidus
        {
            get => _ropePool.RopeRidus;
        }




        protected override void Awake()
        {
            base.Awake();

            _objModelRoot = Instantiate<GameObject>(_empty);
            _objModelRoot.name = "Root_RopePool";


            _ropePool = new();

            _colorChange = this.GetOrAddComponent<RopeColorChange>();

            SetEvent();
        }

        private void Start()
        {
            InitColorChangeMachine();
        }


        #region 绳子颜色控制器

        [Header("设置绳子材质与颜色：(暂时拖拽)")]
        public GameObject ColorChangeWithCanvus;

        private RopeColorChange _colorChange;

        public void InitColorChangeMachine()
        {
            _colorChange.SetNewTargetMaterial(this.CurrentRopeHelper.RopeMaterial);

            _colorChange.Enable();
        }

        #endregion


        #region RopeHelper池
        /// <summary>
        /// 只能从池中获取RopeHelper
        /// </summary>
        private RopePools _ropePool;

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

        /// <summary>
        /// 重新初始化所有的绳子
        /// 并将其隐藏
        /// </summary>
        public void ResetAllRope()
        {
            _ropePool.ResetAllRope();
        }


        #endregion


        /// <summary>
        /// 创建新的绳子
        /// 将之前不用的Rope退回，之后重新借出一个没有被使用的绳子
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

        public GameObject RopeChangeToModel(int ropeindex = 1)
        {
            GameObject model=null;
            
            switch (ropeindex)
            {
                case 0:
                    model = this.CurrentRopeHelper.GetRopeModelObj();
                    break;
                case 1:
                    model = this.PreferRopeHelper.GetRopeModelObj();
                    break;
                case 2:
                    model = this._ropePool.AfterRopeHelper.GetRopeModelObj();
                    break;
            }
            
            //静态批处理
            model.isStatic = true;

            return model;
        }



        /// <summary>
        /// 强制刷新Rope的模型形状
        /// 在同一帧既改变了绳子的begin和end的值，如果不使用这个函数，将会出现错误
        /// </summary>
        public void RopeForcedUpdate()
        {
            _ropePool.ObiUpdater.FrocedUpdate();
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
        /// <param name="flagxz">   初定的刺绣点</param>
        /// <param name="side">     这个绳子的方位</param>
        /// <param name="begin">    生成绳子的beign</param>
        /// <param name="end">      生成绳子的End</param>
        public void CreateRopePointPositionWithSide(Vector3 flagxz,float side,out Vector3 begin,out Vector3 end)
        {
            float buliaoY = BuLiaoManager.Instance.GetBuLiaoData().position.y;

            //1正面朝下，-1负面朝上
            float ropePointY = PinManager.Instance.PinSide < 0 ? buliaoY + RopePointY : buliaoY - RopePointY;

            ///设置新生成的绳子的初始位置
            begin = new Vector3(flagxz.x, ropePointY, flagxz.z);
             end = new Vector3(flagxz.x, ropePointY, flagxz.z);
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