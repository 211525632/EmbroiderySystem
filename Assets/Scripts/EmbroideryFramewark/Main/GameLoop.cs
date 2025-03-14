using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbroideryFramewark
{

    /// <summary>
    /// 只能由GameManager来控制其中的状态
    /// </summary>

    public class GameLoop : MonoBehaviour,IGameStateManager
    {
        /// <summary>
        /// 保证线程安全
        /// </summary>
        private object m_lock = new object();   


        public readonly Dictionary<int, IEbdGameState> allGameStates = new();

        private int currentGameState = (int)EEbdGameState.EditMode;

        private void Start()
        {

            InitGameState();

            ///初始化第一个状态
            allGameStates
                .GetValueOrDefault(currentGameState, null)
                .OnStateEnter();

        }

        private void InitGameState()
        {
            allGameStates.Add((int)EEbdGameState.EditMode, new EbdEditState());
            allGameStates.Add((int)EEbdGameState.Test, new EbdTestState());
        }


        private void Update()
        {
            lock (m_lock)
            {
                allGameStates
                .GetValueOrDefault(currentGameState, null)
                ?.OnStateUpdate();
            }
        }

        private void FixedUpdate()
        {
            lock (m_lock)
            {
                allGameStates
                    .GetValueOrDefault(currentGameState, null)
                    ?.OnStateFixedUpdate();
            }
        }

        private void LateUpdate()
        {
            lock (m_lock)
            {
                allGameStates
                    .GetValueOrDefault(currentGameState, null)
                    ?.OnStateLateUpdate();
            }
        }


        /// ---------------
        /// <summary>
        /// 设置当前的状态
        /// </summary>
        
        /// <summary>
        /// OnStateEnter
        /// OnStateExit
        /// 都在set中实现
        /// </summary>
        public void SetGameState(EEbdGameState state)
        {
            try
            {
                this.SetGameState((int)state);
            }catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void SetGameState(int state)
        {
            lock (m_lock)
            {
                Debug.Log("set state:" + state);

                ///转换前
                allGameStates
                    ?.GetValueOrDefault(currentGameState, null)
                    ?.OnStateExit();

                currentGameState = state;

                ///转换后
                allGameStates
                    ?.GetValueOrDefault(currentGameState, null)
                    ?.OnStateEnter();
            }
        }

        public int GetGameState()
        {
            return this.currentGameState;
        }
    }

}