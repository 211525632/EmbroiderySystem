using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
namespace EmbroideryFramewark
{

    /// <summary>
    /// 用于全局的游戏控制
    /// </summary>

    public class GameManager : MonoSingleton<GameManager>
    {
        /// <summary>
        /// 控制游戏流程的状态
        /// </summary>
        public IGameStateManager GameStateManager { get; private set; }

        private void Start()
        {
            ///初始化游戏循环
            GameObject gameLoop= GameObject.Find("GameLoop");
            if(object.ReferenceEquals(null,gameLoop))
                gameLoop = new GameObject("GameLoop");
            GameStateManager = gameLoop.GetOrAddComponent<GameLoop>();
        }

        public void ChangeGameState(int state)
        {
            this.GameStateManager.SetGameState(state);
        }

        //[Header("调节音量")]

        //[Header("调节设置")]

        //[Header("导出导入")]

        [SerializeField] List<SingleRopeData> singleRopeDatas;
        [SerializeField] List<GameObject> models;

        public void GraphicImport()
        {
            if(this.GameStateManager.GetGameState() == (int)EEbdGameState.EditMode)
            {
                Debug.Log("显示不能在编辑模式下进行导入");
                return ;
            }

            RopeManager.Instance.ResetAllRope();

            singleRopeDatas = new();
            models = new();
            ///测试
            string dirPath = Application.dataPath + "/---------TextTestFile";
            string fileName = "/TestText.text";

            EbdGraphIO.GetRopeGraphFromFile(dirPath + fileName, SetDatas);
        }

        public void GraphicExport()
        {
   

            ///测试
            string dirPath = Application.dataPath + "/---------TextTestFile";
            string fileName = "/TestText.text";

            Debug.Log("EbdShortMemeryCtrl.Instance.CurrentRopeDatas;"+EbdShortMemeryCtrl.Instance.CurrentRopeDatas);

            EbdGraphIO.SaveRopeGraphToFile(dirPath + fileName, EbdShortMemeryCtrl.Instance.CurrentRopeDatas);
        }

        void SetDatas(List<SingleRopeData> datas, List<GameObject> models)
        {
            this.singleRopeDatas = datas;
            this.models = models;
        }

    }

}