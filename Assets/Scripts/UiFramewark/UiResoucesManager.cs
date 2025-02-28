using UnityEngine;

namespace UiFramewark
{
    /// <summary>
    /// 利用AB包实现
    /// 应该是属于全局的
    /// </summary>
    class UiResoucesManager:MonoSingleton<UiResoucesManager>
    {
        [Header("Ui预制体：（后期使用AB包加载）")]
        [SerializeField] private GameObject OneUiPrefab;

        [SerializeField] public GameObject testObj;

        [Header("空物体（后期使用AB包加载）")]
        public GameObject _empty;

        public GameObject _canvas;

        /// <summary>
        /// 暂时的函数
        /// 当前无论如何他都只能加载一个固定的Ui
        /// 
        /// 返回值始终是GameObject——》用于实例化的 Ui Prefabe
        /// </summary>
        public GameObject LoadUi(string name)
        {
            return OneUiPrefab;
        }
    }
}
