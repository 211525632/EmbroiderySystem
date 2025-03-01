using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;

namespace EmbroideryFramewark.Interface
{
    /// <summary>
    /// 刺绣操作方式的接口。
    /// 与control模块密切相关
    /// </summary>
    public interface IEmbroideryOp
    {
        /// <summary>
        /// 刺绣类的运行接口
        /// </summary>
        public void Run();

        /// <summary>
        /// 添加到Manager中
        /// </summary>
        public void JoinInManager();  
    }
}
