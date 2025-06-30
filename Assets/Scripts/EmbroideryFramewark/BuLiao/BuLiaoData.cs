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
    public struct BuLiaoData
    {

        public static BuLiaoData None = new BuLiaoData(false, Vector3.zero, null);



        //首先需要检查这个是否有效
        public bool         isValid;

        //法线向量
        public Vector3      normal;
        
        //transform：
        public Vector3      position;
        public Quaternion   rotation;
        public Vector3      scale;


        public BuLiaoData(bool isValid,Vector3 normal,Transform targetTrans)
        {
            if (!isValid)
            {
                this.isValid = false;

                this.normal = normal.normalized;

                this.position = Vector3.zero;
                this.rotation = Quaternion.identity;
                this.scale = Vector3.zero;
                return;
            }

            this.isValid    = true;

            this.normal     = normal.normalized;

            this.position   = targetTrans.position;
            this.rotation   = targetTrans.rotation;
            this.scale      = targetTrans.localScale;
        }

        public BuLiaoData(bool isValid,Vector3 normal,Vector3 position,Quaternion rotation,Vector3 scale)
        {
            if (!isValid)
            {
                this.isValid    = false;

                this.normal     = normal.normalized;

                this.position   = Vector3.zero;
                this.rotation   = Quaternion.identity;
                this.scale      = Vector3.zero;

                return;
            }

            this.isValid = isValid;

            this.normal     = normal.normalized;

            this.position   = position;
            this.rotation   = rotation;
            this.scale      = scale;
        }



    }
}
