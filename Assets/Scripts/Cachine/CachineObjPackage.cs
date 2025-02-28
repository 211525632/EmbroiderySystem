using Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cachine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachineObjPackage<T>:ICachinePackage<T> where T : class, ISelfDestroyAble
    {
        /// <summary>
        /// 包中的物体
        /// </summary>
        public T Obj;

        /// <summary>
        /// 这个包的状态
        /// </summary>
        public ECachineState State { get; private set; } = ECachineState.None;


        public CachineObjPackage(T packedObj)
        {
            this.Obj = packedObj;
        }

        #region 对包的操作

        /// <summary>
        /// 将外部物体打包
        /// </summary>
        public void Packing(T targetObj)
        {
            if (object.ReferenceEquals(null, targetObj))
                return;

            Debug.Log("packing!");

            this.Obj = targetObj;
            this.State = ECachineState.Have;
        }



        /// <summary>
        /// 将打包的物体释放
        /// 并删除
        /// </summary>
        public void UnpackingAndDele()
        {

            if (object.ReferenceEquals(null, Obj))
                return;
            State = ECachineState.None;
            Obj.SelfDestory();
            Obj = null;
        }


        public T GetOut()
        {
            if (object.ReferenceEquals(null, Obj))
                return null;
            T obj = Obj;
            State = ECachineState.None;
            Obj = null;
            return obj;
        }

        #endregion
    }
}
