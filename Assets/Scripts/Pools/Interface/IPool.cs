using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pools
{
    /// <summary>
    /// 子对象池
    /// 默认存放方式
    /// 
    /// 功能实现：
    /// 1. 实现
    /// 
    /// </summary>
    public interface IPool
    {


        /// <summary>
        /// 设置当前pool的初始化模板
        /// </summary>
        void SetModel(IMPoolAble model);

        /// <summary>
        /// 从池中借出一个物体
        /// </summary>
        IMPoolAble BorrowObj();


        /// <summary>
        /// 返回当前Pool的模型，
        /// 不会对Pool存储的物体造成影响
        /// </summary>
        /// <returns></returns>
        IMPoolAble GetModel();


        /// <summary>
        /// 从外部回收一个物体到池中
        /// </summary>
        void RecycleObj(IMPoolAble recycledObj);

        /// <summary>
        /// 删除自己
        /// </summary>
        void DelePool();

    }

}
