using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 一个游戏状态
    /// </summary>
    public interface IEbdGameState
    {
        /// <summary>
        /// 只读的
        /// </summary>
        EEbdGameState GameState { get; }

        void OnStateEnter();

        void OnStateExit();

        void OnStateUpdate();

        void OnStateFixedUpdate();

        void OnStateLateUpdate();


    }
}
