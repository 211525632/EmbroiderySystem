using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UiFramewark;
using UnityEngine;
using XLua;
using MyLua;
using Cachine;

namespace UiFramewark
{

    /// <summary>
    /// TODO:需要写，对于刚刚初始化的UI
    /// 我们需要一个Ui事件监听器。
    /// 当某个UiCollection被show的时候
    /// 需要发送广播
    /// 对于需要的模块进行事件 绑定
    /// </summary>
    public class UiManager : MonoSingleton<UiManager>
    {

        public float cachineTime = 2f;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _uilayerManager = new UiLayerManager();

            _uiCachine = new(cachineTime);

            SetLuaRequire();
        }

        private void OnDestroy()
        {
            _uiCachine.StopAllCachine();
            _currentActiveUiCollections.Clear();
        }

        private void SetLuaRequire()
        {
            LuaManager.LuaEnv.DoString("require 'LuaUiSetting'");
            LuaManager.LuaEnv.Global.GetInPath<LuaFunction>("LuaUi.SetAllRequire").Call();
        }

        /// <summary>
        /// Ui层级控制器
        /// </summary>
        private UiLayerManager _uilayerManager;

        #region -------------------------在当前UI界面中“添加”一个新的UI界面-------------------------------



        /// <summary>
        /// 生成的UiCollection位于Main层
        /// </summary>
        /// <param name="collectionName">UiCollection的名称</param>
        public void AddUiColliction(string collectionName)
        {
            AddUiColliction(collectionName, UiLayer.GetLayer("Main"));
        }


        /// <summary>
        /// 生成的UiCollection位于指定层
        /// </summary>
        /// <param name="collectionName">   UiCollection的名称</param>
        /// <param name="uiLayerNum">       Layer的标号</param>
        public void AddUiColliction(string collectionName, int uiLayerNum)
        {
            this.AddUiColliction(collectionName, _uilayerManager.GetLayerRootTrans(uiLayerNum));
        }


        private GameObject tempUiCollection = null;

        /// <summary>
        /// 生成的UiCollection位于指定的物体之下
        /// </summary>
        /// <param name="collectionName">   UiCollection的名称</param>
        /// <param name="uilayerTrans"      被设置成ui集群的父物体</param>
        private void AddUiColliction(string collectionName, Transform uilayerTrans)
        {
            ///查看是否完全显示
            if(!object.ReferenceEquals(
                null,this.GetUiCollection(collectionName)))
            {
                Debug.LogWarning("这个UI已经被显示了，无需再次显示");
                return;
            }

            ///在缓存中拿这样的一个Ui
            UiCollection targetCollection = _uiCachine.Get(collectionName);
            
            ///不存在
            if(object.ReferenceEquals(targetCollection,null))
            {
                CreateOneNewUiCollection(name, uilayerTrans);
            }
            ///存在
            /// 将UiCollection从Ui中拿出，并将其重新显示
            else
            {
                targetCollection.gameObject.SetActive(true);

                this._uilayerManager.SetUiLayer(
                    targetCollection,
                    uilayerTrans
                    );

                ///添加到Ui字典
                _currentActiveUiCollections.Add(collectionName, targetCollection);
            }

        }






        /// <summary>
        /// 根据层级已经Ui的名称，重新创建一个新实例
        /// 并放入到Ui字典中
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="uiLayerTrans"></param>
        private void CreateOneNewUiCollection(string collectionName,Transform uiLayerTrans)
        {
            ///新资源加载
            tempUiCollection = UiResoucesManager.Instance.LoadUi(collectionName);
            //实例化
            tempUiCollection = Instantiate<GameObject>(tempUiCollection);


            UiCollection collection = tempUiCollection.GetComponent<UiCollection>();

            //绑定lua脚本
            SetBinding(collection);


            if (object.ReferenceEquals(null, tempUiCollection))
            {
                Debug.LogError("在ui资源中找不到合适的资源：name:" + collectionName);
                return;
            }

            this._uilayerManager.SetUiLayer(
                            collection,
                            uiLayerTrans
                            );

            ///添加到Ui字典
            _currentActiveUiCollections.Add(collectionName, collection);
        }




        /// <summary>
        /// 对Ui进行Lua侧绑定
        /// </summary>
        /// <param name="collection"></param>
        private void SetBinding(UiCollection collection)
        {
            if (collection.ActionClassName.Equals("None"))
                return;

            collection.LuaBinding(collection.ActionClassName);
        }

        #endregion

        #region ——————————————关闭当前UI界面下的一个Ui界面
        
        /// <summary>
        /// 把要关闭的UI隐藏，并传入缓存中，定时保存
        /// </summary>
        /// <param name="name"></param>
        public void CloseUiCollection(string name)
        {
            UiCollection closedUi = this.GetUiCollection(name);
            if (object.ReferenceEquals(closedUi, null))
            {
                Debug.LogWarning("不需要关闭一个不存在的Ui");
                return ;
            }
            
            closedUi.gameObject.SetActive(false);

            _currentActiveUiCollections.Remove(name);

            ///将这个Ui放入缓存，设定时间
            _uiCachine.Add(name, closedUi);
        }

        #endregion

        #region ---------------------ui资源管理------------------------ 

        /// <summary>
        /// 直接存储
        /// </summary>
        private Dictionary<string, UiCollection> _currentActiveUiCollections = new();

        /// <summary>
        /// 缓存，只有在UI被关闭之后才会添加进去
        /// </summary>
        public UiCachine _uiCachine = null;


        /// <summary>
        /// 得到当前
        /// </summary>
        /// <param name="name"></param>
        public UiCollection GetUiCollection(string name)
        {
            return _currentActiveUiCollections.GetValueOrDefault(name, null);
        }

        #endregion


    }
}