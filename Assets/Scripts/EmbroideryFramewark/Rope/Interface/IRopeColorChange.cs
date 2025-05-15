using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 获取到SingleRopeHelper -》 直接操控
    /// 
    /// 功能开启与否      ——》新模块
    /// </summary>

    public interface IRopeColorChange
    {
        void SetNewTargetMaterial(Material targetMaterial);

        void SetColor(Color color);
    }
}
