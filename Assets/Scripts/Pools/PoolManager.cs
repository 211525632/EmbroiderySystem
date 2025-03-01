using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pools
{
    public class PoolManager : MonoSingleton<PoolManager>,IPoolManager
    {
        /// <summary>
        /// 池存放地
        /// </summary>
        private Dictionary<string, IPool> _poolDict;

        public Dictionary<string, IPool> PoolsDict => _poolDict;



        /// <summary>
        /// ————————————初始化函数——————————
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            this._poolDict = new();
        }



        #region 外部接口


        public IPool GetPool<T>(string key) where T : class,IMPoolAble
        {
            try
            {
                if (object.ReferenceEquals(_poolDict[key].GetModel() as T, null))
                {
                    Debug.LogWarning($"没有找到合适类型的池：您所期望的{typeof(T).FullName} " +
                        $"实际上的:{_poolDict[key].GetModel()?.GetType().FullName}");
                    return null;
                }
                return _poolDict[key];
            }
            catch (KeyNotFoundException )
            {
                Debug.LogWarning($"PoolManager::找不到注册key为:{key}的池对象");
                return null;
            }
        }

        public T GetPooledObj<T>(string key) where T : class, IMPoolAble
        {
            try
            {
                if (object.ReferenceEquals(_poolDict[key].GetModel() as T, null))
                {
                    Debug.LogWarning($"没有找到合适类型的池：您所期望的{typeof(T).FullName} " +
                        $"实际上的: {_poolDict[key].GetModel()?.GetType().FullName}  ，由此得不到您想要的值");
                    return null;
                }

                return _poolDict[key]?.BorrowObj() as T;
            }
            catch (KeyNotFoundException )
            {
                Debug.LogWarning($"PoolManager::找不到注册key为:{key}的池对象,也就找不到相应的值");
                return null;
            }
        }



        public void InitNewPool<T>(string key, T model) where T : class, IMPoolAble
        {
            try
            {
                _poolDict.Add(key, new DefaultPool(model));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return ;
            }
        }

        public void InitNewPool<T>(string key, T model, IPool poolModel) where T : class, IMPoolAble
        {
            try
            {
                _poolDict.Add(key, poolModel);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
        }



        public bool RemovePool(string key)
        {
            try
            {
                _poolDict[key].DelePool();
                _poolDict.Remove(key);
                return true;
            }catch(KeyNotFoundException)
            {
                return false;
            }
        }

        #endregion
    }
}
