using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    public class EbdTestState : IEbdGameState
    {
        private readonly EEbdGameState _state = EEbdGameState.Test;
        public EEbdGameState GameState => _state;

        public void OnStateEnter()
        {
            Debug.Log("Enter Test");
        }

        public void OnStateExit()
        {
            Debug.Log("Exit Test");
        }

        public void OnStateFixedUpdate()
        {
        }

        public void OnStateLateUpdate()
        {
        }

        public void OnStateUpdate()
        {
        }
    }
}
