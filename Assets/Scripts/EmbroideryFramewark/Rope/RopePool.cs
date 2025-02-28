using Obi;
using System;
using Unity.VisualScripting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 如果需要增加Rope请另写其他函数
    /// </summary>
    public class RopePool
    { 

        //新创建的rope的父物体
        public GameObject _ropePoolRoot { get; private set; }


        private SingleRopeData _ordinaryData;

        public RopePool()
        {
            Init();

            ///记录
            _ordinaryData = new SingleRopeData(CurrentRopeHelper);
        }

        private readonly SingleRopeHelper[] _ropeHelpers = new SingleRopeHelper[3];

        /// <summary>
        /// 记录Rope的使用情况
        /// 防止操作冲突
        /// </summary>
        readonly bool[] ropeState = new bool[3];

        readonly int size = 3;

        int current = 2;
        int pre = 1;
        int after = 0;


        /// <summary>
        /// 创建Root和RopeHelper实例
        /// </summary>

        private void Init()
        {
            try
            {
                _ropePoolRoot = GameObject.Instantiate<GameObject>(RopeManager.Instance._obiSolverModel);
                _ropePoolRoot.name = "RopePool_Root";

                for (int i = 0; i < 3; ++i)
                {
                    _ropeHelpers[i] = GameObject.Instantiate<GameObject>(RopeManager.Instance._ropeModel, _ropePoolRoot.transform).GetComponent<SingleRopeHelper>();
                    _ropeHelpers[i].name = i.ToString();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        #region 访问器
        public SingleRopeHelper CurrentRopeHelper { get => _ropeHelpers[current]; }

        public SingleRopeHelper PreRopeHelper {  get => _ropeHelpers[pre]; }

        public SingleRopeHelper AfterRopeHelper { get =>_ropeHelpers[after]; }

        /// <summary>
        /// 绳子的半径，用于处理吸附
        /// </summary>
        private float _ropeRidus = -1;

        public float RopeRidus
        {
            get
            {
                if (_ropeRidus < 0)
                {
                    _ropeRidus = RopeManager.Instance._ropeModel.GetComponent<ObiRopeBlueprint>().thickness;
                }

                return _ropeRidus;
            }
        }

        #endregion


        public bool BorrowRopeAndInit(Vector3 begin,Vector3 end,float length)
        {

            Debug.Log("借出！");

            if (IsBorrowing)
            {
                Debug.LogError("等待Rope归还，请先归还后再借出新的Rope！");
                return false;
            }

            IsBorrowing = true;

            RenewOrder();

            ///显示当前的Rope
            this.HideOrActiveCurrentRope(true);

            this.CurrentRopeHelper.SetBeginPosition(begin);
            this.CurrentRopeHelper.SetEndPosition(end);
            this.CurrentRopeHelper.SetRopeLengthTo(length);

            return true;
        }


        public bool IsBorrowing = false;
        /// <summary>
        /// 归还借出的绳子
        /// </summary>
        public void ReturnPreRope()
        {
            if (!IsBorrowing)
                return;

            //检查是否是空闲状态
            if (ropeState[after])
            {
                Debug.LogError("还存在Rope没用被使用完，不能切换！");
                return;
            }
            ///TODO：改变当前的RopeHelper初始位置之类的
            ropeState[pre] = false;
            ropeState[after] = true;

            //ReSetRope();

            //PreRopeHelper.SetRopeDisActive();

            IsBorrowing = false;
        }

        private void RenewOrder()
        {
            current = RenewOneOrder(current);
            pre = RenewOneOrder(pre);
            after = RenewOneOrder(after);
        }

        private int RenewOneOrder(int order)
        {
            return (order + 1) % size;
        }


        #region 绳子隐藏与显示

        private void HideRope(int index)
        {
            _ropeHelpers[index].GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        private void ActiveRope(int index)
        {
            _ropeHelpers[index].GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        private void HideOrActiveRope(int index,bool isActive)
        {
            if(isActive)
                ActiveRope(index);
            else
                HideRope(index);
        }


        public void HideOrActiveCurrentRope(bool isActive)
        {
            HideOrActiveRope(current, isActive);
        }

        public void HideOrActivePreRope(bool isActive)
        {
            HideOrActiveRope(pre, isActive);
        }

        public void HideOrActiveAfterRope(bool isActive)
        {
            HideOrActiveRope(after, isActive);
        }

        #endregion

    }
}