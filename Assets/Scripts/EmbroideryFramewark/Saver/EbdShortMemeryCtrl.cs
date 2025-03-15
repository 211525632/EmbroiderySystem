using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 短时间保存控制器
    /// 用于检测输入操作
    /// 执行 保存/撤销 操作
    /// </summary>
    public class EbdShortMemeryCtrl : MonoSingleton<EbdShortMemeryCtrl>
    {

        private EbdShortMemerator _saver;

        public List<SingleRopeData> CurrentRopeDatas { get => _saver.RopeDatas.ToList(); }

        protected override void Awake()
        {
            base.Awake();

            _saver = new();

        }




        void Update()
        {
            Run();
        }


        /// <summary>
        ///  
        /// (false,false) -> (false | true) -> (false,false)
        /// </summary>
        private void Run()
        {
            if(!_active)
            {
                if(!(m_MouseInput.IsRevokeOp || m_MouseInput.IsSaveOp))
                {
                    _active = true;    
                }
                return;
            }

            if (m_MouseInput.IsSaveOp)
            {
                Debug.Log("向前回忆");
                RecollectOp();
                _active = false;
            }
            else if(m_MouseInput.IsRevokeOp)
            {
                Debug.Log("向后撤销");
                RevokeOp();
                _active = false;
            }
        }


        #region 撤销与回忆功能实现

        /// <summary>
        /// 为了保证针的位置始终正确，我决定一次撤销操作将会连续撤销两次
        /// </summary>
        public void RecollectOp()
        {
            _saver?.RecollectOp();
            _saver?.RecollectOp();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void RevokeOp()
        {
            _saver?.RevokeOp();
            _saver?.RevokeOp();
        }

        /// <summary>
        /// 缓存一次操作
        /// </summary>
        /// <param name="currentRopeHelper"></param>
        /// <param name="ropeModel"></param>
        public void SaveNewOp(SingleRopeHelper currentRopeHelper, GameObject ropeModel)
        {
            _saver?.SaveOp(currentRopeHelper, ropeModel);
        }

        #endregion


        #region 功能开启关闭


        private bool _active = true;


        private void Enable()
        {
            this.gameObject.SetActive(true);
        }

        private void Disable()
        {
            this.gameObject.SetActive(false);
        }

        #endregion


    }
}