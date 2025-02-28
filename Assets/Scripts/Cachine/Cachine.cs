using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Rendering.FilterWindow;

namespace Cachine
{
    /// <summary>
    /// 物体缓存
    /// 
    /// 主要解决的问题是，反复创建和销毁同一种东西
    /// 通过缓存，将其暂时存放在Cachine之中，
    /// 在一定时间内，可以再次取出相应物体。重复使用。
    /// 
    /// </summary>
    public class Cachine<T> where T : class, ISelfDestroyAble
    {
        /// <summary>
        /// 检查并清除的一个间隔时间
        /// </summary>
        private float _clearDeltaTime;

        public Cachine(float checkDeltaTime)
        {
            this._clearDeltaTime = checkDeltaTime;
            _cachine = new();
            SetAutoDele();
        }

        private Dictionary<string,int> _nameToKey = new ();

        #region 从name -》 key的映射

        /// <summary>
        /// 查看是否已经存在，
        /// 如果存在，返回-1
        /// 如果不存在，返回可用空间的index
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int CreateNewKey(string name)
        {
            ///证明存在，不需要进行添加
            if (_nameToKey.ContainsKey(name))
            {
                ///如果这个区域没有被占用 ，直接使用即可
                if (object.ReferenceEquals(_cachine[_nameToKey[name]].Obj, null))
                {
                    return _nameToKey[name];
                }
                ///已经存在，不再使用
                else
                {
                    return -1;
                }
            }

            int free = this.FindFreePackage();

            //如果没有空闲值，就进行扩展并给出扩展之后的index
            if(free<0)
            {
                free = ExtendCachineElem();
            }

            return free;
        }

        public int GetKey(string name)
        {
            return _nameToKey.GetValueOrDefault(name,-1);
        }

        #endregion


        /// <summary>
        /// 实际上的存储
        /// </summary>
        private List<CachineObjPackage<T>> _cachine;


        #region 自动销毁

        CachineAutoDelMachine _autoDeleMachine;


        /// <summary>
        /// TODO:给autoMachine一个委托
        /// 1.删除第i个物体的操作。
        /// 2.判断 第i个物体的存在与否
        /// 
        /// 当自己所处时间小于零之后就直接销毁
        /// </summary>
        private List<float> _autoDeleTimes = new List<float>();


        private void SetAutoDele()
        {
            _autoDeleTimes.Add(-1);
            _autoDeleMachine = new(AutoDele, 5f);
            _autoDeleMachine.Start();
        }


        protected virtual void AutoDele()
        {
            try
            {
                for (int i = 0; i < _cachine.Count; ++i)
                {
                    Debug.Log(i + "   :" + _cachine[i].State);
                    if (_cachine[i].State != ECachineState.Have)
                        continue;

                    _autoDeleTimes[i] -= this._clearDeltaTime;

                    if (_autoDeleTimes[i] <= 0)
                    {
                        Remove(i);
                    }
                }
            }
            catch (OutOfMemoryException e)
            {
            }

        }

        /// <summary>
        /// 自动删除缓存中的东西
        /// </summary>
        /// <param name="name"></param>
        private void Remove(int key)
        {
            _cachine[key].UnpackingAndDele();
        }


        private void Remove(string name)
        {
            _cachine[_nameToKey[name]].UnpackingAndDele();
            _nameToKey.Remove(name);
        }



        #endregion


        #region 管理cachine

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns> 添加进Cachine的位置</returns>
        private int ExtendCachineElem(T element)
        {
            _cachine.Add(new CachineObjPackage<T>(element));
            _cachine[_cachine.Count - 1].Packing(element);
            return _cachine.Count-1;
        }

        private int ExtendCachineElem()
        {
            return ExtendCachineElem(null);
        }

        public void UseCachineElem(int key,T element)
        {
            try
            {
                _cachine[key].Packing(element);
            }catch (ArgumentOutOfRangeException e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void DeleCachinElem(int index)
        {
            if (index < 0)
                return;

            _cachine[index].UnpackingAndDele();
        }



        private int _freePackageIndex = 0;

        /// <summary>
        /// 寻找一个没有被使用的包
        /// </summary>
        private int FindFreePackage()
        {
            for (int i = 0; i < _cachine.Count; ++i)
            {
                if (_cachine[i].State != ECachineState.None)
                    continue;

                return i;
            }

            ///没有找到空闲的地方
            return -1;
        }

        private void ToFreePackage(int key)
        {
            _freePackageIndex = key;
        }


        #endregion



        #region 外部接口

        /// <summary>
        /// 添加一个缓存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cachinedObj"></param>
        public void Add(string name, T cachinedObj)
        {
            int key = CreateNewKey(name);
            
            //TODO:bug
            if(key<0)
            {
                try
                {
                    _autoDeleTimes[_nameToKey[name]] = _clearDeltaTime;
                    Debug.LogWarning("这个元素在Cachine中已经存在，不需要继续添加");
                }catch(ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e);
                    return;
                }
            }
            else
            {
                try
                {
                    _nameToKey.Add(name, key);
                    UseCachineElem(key, cachinedObj);
                }
                ///多添加了name和key
                catch(ArgumentException e)
                {
                    Debug.LogWarning(e);
                    UseCachineElem(key, cachinedObj);
                }
            }

        }


        /// <summary>
        /// 将缓存拿出使用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get(string name)
        {
            try
            {
                int key = GetKey(name);
                return (_cachine[key].GetOut());
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }



        public void StopAllCachine()
        {
            this.Clear();
            this._autoDeleMachine.Stop();
        }
        
        
        /// <summary>
        /// 清空所有缓存
        /// TODO:可能有问题
        /// </summary>
        public void Clear()
        {
            this._nameToKey.Clear();
            this._cachine.Clear();
        }


        #endregion

    }
}
