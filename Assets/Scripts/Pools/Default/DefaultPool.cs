using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pools
{
    /// <summary>
    /// TODO:实现数量上的控制，
    /// 以及出现扩容之后会在一定时间之后
    /// 将不常用的元素删除，直到初始化时设置的容量
    /// </summary>

    public class DefaultPool : IPool
    {
        private IMPoolAble _model = null;

        /// <summary>
        /// 存储容器
        /// </summary>
        private Queue<IMPoolAble> pools;


        readonly int ordinaryCount = 20;


        public DefaultPool(IMPoolAble pooledObj,int ordinaryCount = 20)
        {
            pools = new();
            this._model = pooledObj;
            AddObjsToPool(ordinaryCount);
        }

        private bool IsSetModel
        {
            get
            {
                if(object.ReferenceEquals(null, this._model))
                {
                    Debug.LogWarning("DefaultPool::没有设置Model！");
                    return false;
                }

                return true;
            }
        }


        private int PoolCount => pools.Count;


        /// <summary>
        /// 添加指定数量的物体到池中
        /// </summary>
        /// <param name="num">  要添加的物体的量</param>
        private void AddObjsToPool(int num)
        {
            try
            {
                if (IsSetModel)
                {
                    for (int i = 0; i < num; ++i)
                    {
                        IMPoolAble temp = this._model.SelfCopy() as IMPoolAble;
                        temp.OnRecycle();
                        this.pools.Enqueue(temp);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #region 外部接口

        public IMPoolAble GetModel()
        {
            return this._model;
        }


        public IMPoolAble BorrowObj()
        {
            if (!IsSetModel)
                return null;

            //TODO:添加新的物体
            if (PoolCount <= 0)
            {
                Debug.Log("brrow new obj");
                return this._model.SelfCopy() as IMPoolAble;
            }

            //运行当前借出逻辑
            pools.Peek().OnBorrow();

            return pools.Dequeue();
        }

        public void RecycleObj(IMPoolAble recycledObj)
        {
            if(object.ReferenceEquals(null,recycledObj))
                return;

            //执行回收时的逻辑
            recycledObj.OnRecycle();

            this.pools.Enqueue(recycledObj);
        }

        public void SetModel(IMPoolAble model)
        {
            this._model = model;
        }


        /// <summary>
        /// 后面如果有物体归还，则直接销毁
        /// 
        /// TODO：应该至少保证未被归还的物体
        /// 在pool被销毁的时候同步销毁
        /// </summary>
        public void DelePool()
        {
            ///清空整个队列
            while (this.pools.Count > 0)
            {
                this.pools.Dequeue().OnDestroy();
            }
            this.pools = null;
            _model = null;
        }

        #endregion
    }
}
