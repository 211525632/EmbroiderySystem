
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UiFramewark
{

    /// <summary>
    /// Ui层级
    /// </summary>
    public static class UiLayer
    {
        public static Dictionary<string, int> _layerDict = null;

        private static void SetLayer()
        {
            string[] _tempList = { "Top", "Background", "Main", "Common" };
            
            _layerDict = new();  

            for (int i=0;i<_tempList.Length;++i)
            {
                _layerDict.Add(_tempList[i],i);

            }
        }

        public static int GetLayer(string layerName)
        {
            if (object.ReferenceEquals(null, _layerDict))
            {
                SetLayer();
            }
 
            return _layerDict[layerName];
        }
    }


    /// <summary>
    /// 层级管理器
    /// TODO：把IUiComponent删除
    /// </summary>
    public class UiLayerManager 
    {

        private bool _isInit = false;

        private Transform _layerRootTrans;

        private Dictionary<int, Transform> _layerRootDict = new();


        public UiLayerManager()
        {
            Init();
        }

        #region 初始化
        public void Init()
        {
            if (_isInit)
                return;

            
            _layerRootTrans = MonoHelper.Instantiate(UiResoucesManager.Instance._empty).transform;
            _layerRootTrans.gameObject.name = "ui_layer_root";
            
            ///底层
            CreateLayerRoot("Background",0);
            ///主要Ui窗口
            CreateLayerRoot("Main",1);
            ///常驻层
            CreateLayerRoot("Common",2);
            ///最上层的Ui
            CreateLayerRoot("Top",3);

        }


        private GameObject _tempLayerRoot;
        private void CreateLayerRoot(string rootName,int sortOrder = 1)
        {
            ///实例化canvas
            _tempLayerRoot = MonoHelper.Instantiate<GameObject>(UiResoucesManager.Instance._canvas, _layerRootTrans);
            _tempLayerRoot.name = rootName;

            //设置渲染层级
            _tempLayerRoot.GetComponent<Canvas>().sortingOrder = sortOrder;


            _layerRootDict.Add(
                UiLayer.GetLayer(rootName),
                _tempLayerRoot.transform
                );

            _layerRootDict[UiLayer.GetLayer(rootName)].name = rootName;
        }


        #endregion

        #region GetLayer

        public Transform AllLayerRoot
        {
            get => _layerRootTrans;
        }


        public Transform GetLayerRootTrans(int uiLayerNum)
        {
            return _layerRootDict.GetValueOrDefault(uiLayerNum,null);
        }

        #endregion


                ///TODO：将其只管理层级相关的方面
        #region -------------------------Ui层级管理-------------------------------



        /// <summary>
        /// 生成的UiCollection位于Main层
        /// </summary>
        /// <param name="collectionName">UiCollection的名称</param>
        public void SetUiLayer(UiCollection uiCollection)
        {
            SetUiLayer(uiCollection, UiLayer.GetLayer("Main"));
        }





        /// <summary>
        /// 生成的UiCollection位于指定层
        /// </summary>
        /// <param name="collectionName">   UiCollection的名称</param>
        /// <param name="uiLayerNum">       Layer的标号</param>
        public void SetUiLayer(UiCollection uiCollection, int uiLayerNum)
        {
            this.SetUiLayer(uiCollection, this.GetLayerRootTrans(uiLayerNum));
        }






        private GameObject tempUiCollection = null;

        /// <summary>
        /// 生成的UiCollection位于指定 Root之下
        /// </summary>
        /// <param name="collectionName">   UiCollection的名称</param>
        /// <param name="uilayerTrans"      被设置成ui集群的父物体</param>
        public void SetUiLayer(UiCollection uiCollection, Transform uilayerTrans)
        {
            if(object.ReferenceEquals(null,uiCollection))
            {
                Debug.LogError("设置layer时传入uiCollectio为空！");
                return;
            }

            if (object.ReferenceEquals(null, uilayerTrans))
            {
                Debug.LogError("设置layer时传入LayerRootTrans为空！");
                return;
            }


            uiCollection.gameObject.transform.SetParent(uilayerTrans, true);

        }

        #endregion



    }
}
