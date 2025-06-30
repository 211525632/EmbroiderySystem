using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 初始化材质所需的数据
    /// （暂时没有完成）
    /// </summary>
    public struct RopeMaterialData
    {
        //颜色
        public Vector4 color;

        ///其他

        public RopeMaterialData(Material material)
        {
            this.color = material.color;
        }
    }


}
