using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Cachine
{
    /// <summary>
    /// 以时间为例
    /// </summary>
    public class CachineAutoDelMachine
    {

        private Action _dele;

        /// <summary>
        /// 以时间驱动
        /// </summary>
        /// <param name="judge"></param>
        /// <param name="dele"></param>
        /// <param name="judegTime"></param>
        public CachineAutoDelMachine(Action dele,float judegTime)
        {
            this._dele = dele;

            ///设置等待时间
            wait = new(judegTime);
        }

        #region 自动删除

        private bool _isStart=false;

        private void AutoDel()
        {
            ///执行删除
            this._dele?.Invoke();
        }

        #endregion

        #region 自动运行

        public void Start()
        {
            this._isStart = true;
            AutoRun();
        }

        public void Stop()
        {
            this._isStart = false;
        }

        private void AutoRun()
        {
            AutoDel();

            MonoHelper.Instance.StartCoroutine(WaitForTime());
        }

        private WaitForSeconds wait;
        private IEnumerator WaitForTime()
        {
            Debug.Log("进入自动Run");
            yield return wait;
            if (_isStart)
                AutoRun();

        }

        #endregion

    }
}
