using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 当前布料的基本信息
    /// 
    /// 但是修改此信息不会对本来的BuLiao产生相关的改变
    /// </summary>
    public struct BuLiaoData: ISetOrGetBuLiaoTrans, ISetOrGetBuLiaoType
    {
        //法线向量
        public Vector3      normal;
        
        //transform：
        public Vector3      position;
        public Quaternion   rotation;
        public Vector3      scale;



        #region Set 、Get Trans

        public Vector3 GetBuLiaoPosition()
        {
            return position;
        }

        public Quaternion GetBuLiaoRotation()
        {
            return rotation;
        }

        public Vector3 GetBuLiaoScale()
        {
            return scale;
        }



        public void SetBuLiaoTrans(Transform targetTrans)
        {
            this.position   = targetTrans.position;
            this.rotation   = targetTrans.rotation;
            this.scale      = targetTrans.localScale;  
        }

        public void SetBuLiaoTrans(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position   = position;
            this.rotation   = rotation;
            this.scale      = scale;
        }




        #endregion



        #region Set、Get BuLiaoType

        public GameObject GetBuLiaoType()
        {
            throw new NotImplementedException();  
        }

        public void SetBuLiaoType(GameObject prefabs)
        {
            throw new NotImplementedException();
        }

        #endregion

        ///材质相关：暂无


    }
}
