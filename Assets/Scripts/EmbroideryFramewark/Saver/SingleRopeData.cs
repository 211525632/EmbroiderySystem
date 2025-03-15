using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 单个Rope初始化所需的所有数据
    /// </summary>
    public struct SingleRopeData
    {
        public Vector3 begin;
        public Vector3 end;

        /// <summary>
        /// 如果有光泽度等需求，再进行添加即可
        /// </summary>
        public RopeMaterialData materalData;


        public SingleRopeData(Vector3 begin, Vector3 end, Material material)
        {
            this.begin = begin;
            this.end = end;

            this.materalData = new RopeMaterialData(material);
        }

        public SingleRopeData(SingleRopeHelper ropeHelper)
        {
            this = new SingleRopeData(ropeHelper._beginTransform.position, ropeHelper._endTransform.position,
                ropeHelper.GetMeshRender().GetComponent<Renderer>().sharedMaterial);
        }
    }
}
