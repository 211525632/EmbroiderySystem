using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Pools
{
    /// <summary>
    /// 1.致力于实现 string/type？ 映射到对象池的
    /// 2.致力于完成不常用对象池的自动释放实现
    /// 3.致力于通过对象池管理器来获取各种对象池
    /// 并从中获取到提前注册和存储的物品
    /// 4.使用过程中，传入 key 与 要设置的Prefab
    /// 5.不会设立pool内物体的最大数量
    /// 
    /// 对象池解析：
    /// 1.初始化时就已经生成了一系列目标物体
    /// 
    /// 2.借出的时候仅仅是交出了一份对象的引用。
    /// 实际上还是池持有本身的引用。如果对象池不清空。
    /// 在不使用弱引用的情况下是不会因为外部的清空而清空的（外部不能自主Destroy）
    /// 
    /// 3.回收的时候返回null来表示外部已经结束这个池物体的操作权力
    /// 且，后续的销毁应该由池来完成
    /// 
    /// 4。外部可以指定自定义的回收方式与借出方式（在初始化池的时候就应该完成），即指定active的情况
    /// （但是应该只使用注册在池中的接口方法，而不是其他非指定的方法）
    /// （确保整个编程不会出现未知的函数出现）
    /// 
    /// 5.
    /// </summary>


    public interface IPoolManager
    {
        /// <summary>
        /// key与Pool之间的映射关系
        /// </summary>
        public Dictionary<string, IPool> PoolsDict { get; }


        #region 初始化相关

        /// <summary>
        /// 初始化一个对象池
        ///     
        ///     如果出现hashCode存在的情况
        ///     需要核实生成对象Model之间之间的相等关系
        ///     
        /// 通过type + 名称 + GetHashCode来区分不同 pool 
        /// </summary>
        /// <typeparam name="T">    要存储的物体的类型</typeparam>
        /// <param name="key">      该物体在该类型下的key，可以是string</param>
        /// <param name="model">    生成该pool的指定模板物体</param>
        void InitNewPool<T>(string key, T model) where T : class,IMPoolAble;



        /// <summary>
        /// 初始化一个对象池
        /// 在初始化一个pool的时候允许设置该pool自己的借出回收逻辑
        /// </summary>
        /// <typeparam name="T">        要存储的物体的类型</typeparam>
        /// <param name="key">          该物体在该类型下的key，可以是string</param>
        /// <param name="model">        生成该pool的指定模板物体</param>
        /// <param name="poolModel">    生成的子池的存储模式</param>
        void InitNewPool<T>(string key, T model, IPool poolModel) where T : class, IMPoolAble;


        #endregion


        #region 获取相关


        /// <summary>
        /// 直接给出指定的对象池
        /// </summary>
        /// <typeparam name="T">        该池存储物体的类型</typeparam>
        /// <param name="key">          该池的key</param>
        /// <returns>                   子池接口 </returns>
        IPool GetPool<T>(string key) where T : class, IMPoolAble;


        /// <summary>
        /// 直接返回存放在pool中的物体
        /// 不存在该pool则返回null
        /// </summary>
        /// <typeparam name="T">        要获取的物体的类型</typeparam>
        /// <param name="key">          该物体在该类型下的key，可以是string</param>
        /// <returns>                   池中对象 </returns>
        T GetPooledObj<T>(string key) where T : class, IMPoolAble;

        #endregion


        #region 删除相关

        /// <summary>
        /// 可以选择手动删除pool，但是需要检查pool内物体的使用情况
        /// 只有当物体被完全归还之后才可释放，否则提出警告，返回false
        /// </summary>
        /// <typeparam name="T">        要删除的池的类型</typeparam>
        /// <param name="key">          该池在该类型下的key</param>
        /// <returns>                   销毁池的最终结果 </returns>
        bool RemovePool(string key);

        #endregion
    }

}
