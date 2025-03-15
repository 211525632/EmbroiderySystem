using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    public class DefaultBuLiao : BuLiao
    {
        private void Start()
        {
            this.SetAllChildLayer(LayerMask.NameToLayer(BuLiaoManager.Instance.LayerName));
        }

        private void SetAllChildLayer(int layerNum)
        {
            this.gameObject.layer = layerNum;

            int childCount          = this.transform.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                this.transform.GetChild(i).gameObject.layer = layerNum;
            }
        }


        public override BuLiaoData GetBuLiaoData()
        {
            BuLiaoData data = new BuLiaoData();

            data.isValid = true;
            data.position   = this.transform.position;
            data.rotation   = this.transform.rotation;
            data.scale      = this.transform.localScale;

            return data;
        }
    }
}
