using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    public interface ISetOrGetBuLiaoTrans
    {

        void SetBuLiaoTrans(Transform targetTrans);
        void SetBuLiaoTrans(Vector3 position,Quaternion rotation,Vector3 scale);


    }
}
