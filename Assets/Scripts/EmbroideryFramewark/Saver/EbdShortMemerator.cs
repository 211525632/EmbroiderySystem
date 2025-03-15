using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinwheel.MeshToFile;
using System;

namespace EmbroideryFramewark
{


    [RequireComponent(typeof(RopeManager))]
    public class EbdShortMemerator
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public EbdShortMemerator()
        {
            RopeDatas = new();
            RopeModels = new();

            _opDateCachine = new();
            _opModelCachine = new();
        }

        /// <summary>
        /// 存放每一次操作（撤销或回忆）的绳子信息
        /// </summary>
        public readonly Stack<SingleRopeData> RopeDatas;


        /// <summary>
        /// 存放所有被实例化的模型实例
        /// 通过销毁或者隐藏这个里面的元素来实现撤销操作
        /// </summary>
        public readonly Stack<GameObject> RopeModels;



        #region 功能实现

        /// <summary>
        /// 保存一次刺绣操作
        /// </summary>
        /// <param name="currentRopeHelper">    要保存的Rope的RopeHelper</param>
        public void SaveOp(SingleRopeHelper currentRopeHelper, GameObject ropeModel)
        {
            this.ClearCachine();

            RopeDatas.Push(new SingleRopeData(currentRopeHelper));
            RopeModels.Push(ropeModel);
        }


        /// <summary>
        /// 回忆上次操作
        /// </summary>
        public void RecollectOp()
        {
            //检查是否存在可以回忆的操作
            if (_opDateCachine.Count <= 0f)
                return;

            ///释放缓存，放回之前的区域
            RopeDatas.Push(_opDateCachine.Pop());
            RopeModels.Push(_opModelCachine.Pop());

            ///显示被撤销的模型
            RopeModels.Peek().SetActive(true);

            Vector3 reversePosition = RopeDatas.Peek().begin;
            reversePosition.y = -reversePosition.y;

            ///将剩下的绳子的尾部 与 被隐藏的尾部相连接
            RopeManager.Instance.CurrentRopeHelper.SetEndPosition(reversePosition);

        }


        /// <summary>
        /// --------------------------撤销操作时的缓存-----------------------------------
        /// </summary>
        private Stack<SingleRopeData> _opDateCachine;

        private Stack<GameObject> _opModelCachine;

        /// <summary>
        /// 撤销上一次的操作
        /// </summary>
        public void RevokeOp()
        {
            //检测是否存在可以撤销的操作
            if (RopeModels.Count <= 0f)
                return;

            ///缓存，直到下一次新操作
            _opDateCachine.Push(RopeDatas.Pop());
            _opModelCachine.Push(RopeModels.Pop());

            ///隐藏被撤销的模型，可以使用池来缓冲
            _opModelCachine.Peek().SetActive(false);

            RopeManager.Instance.HideOrActivePreRope(false);
            RopeManager.Instance.HideOrActiveAfterRope(false);

            ///将剩下的绳子的首部 与 现在的尾部相连接
            RopeManager.Instance.CurrentRopeHelper.SetEndPosition(_opDateCachine.Peek().end);

        }


        /// <summary>
        /// 撤销缓存
        /// </summary>
        private void ClearCachine()
        {
            _opDateCachine.Clear();

            int cachineLength = _opModelCachine.Count;

            for (int i = 0; i < cachineLength; ++i)
            {
                MonoHelper.Destroy(_opModelCachine.Pop());
            }
        }

        #endregion


    }

}