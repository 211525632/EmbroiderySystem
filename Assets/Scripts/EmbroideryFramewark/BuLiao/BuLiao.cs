using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 布料
    /// </summary>
    public abstract class BuLiao : MonoBehaviour, IGetBuLiaoDate
    {
        public abstract BuLiaoData GetBuLiaoData();
    }
}
