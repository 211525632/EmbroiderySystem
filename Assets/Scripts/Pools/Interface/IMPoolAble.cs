using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pools
{

    /// <summary>
    /// 这个里面规定pool物体的基本函数
    /// </summary>
    public interface IMPoolAble
    {
        /// <summary>
        /// 在回收的时候需要做的事情
        /// </summary>
        void OnRecycle();


        /// <summary>
        /// 在借出的时候需要完成的事情
        /// </summary>
        void OnBorrow();


        /// <summary>
        /// 在销毁物件的时候要做的事情
        /// </summary>
        void OnDestroy();

        
        /// <summary>
        /// 复制一个新的自己
        /// 需要在这里限制以下
        /// 
        /// 一定要与自己设定的类型一致！
        /// 
        /// </summary>
        object SelfCopy();
    }
}
