using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbroideryFramewark
{
    public interface IGameStateManager
    {
        void SetGameState(EEbdGameState state);

        void SetGameState(int state);

        int GetGameState();
    }
}
