using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum EEbdGameState
{
    Test = -2,

    GameExit = -1,

    GameStart = 0,

    /// <summary>
    /// 选择存档
    /// </summary>
    SelectArchive = 1,

    /// <summary>
    /// 预览模式
    /// </summary>
    ViewMode = 2,

    /// <summary>
    /// 编辑模式
    /// </summary>
    EditMode = 3,

}

