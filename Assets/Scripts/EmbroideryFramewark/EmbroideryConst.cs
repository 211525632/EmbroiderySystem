using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace EmbroideryFramewark
{
    public class EmbroideryConst
    {
        /// <summary>
        /// 当撤销操作的时候
        /// </summary>
        public static string OnEmbroideryOpRevoke = "OnEmbroideryOpRevoke";
    }


    public class EventConst
    {
        public static string OnPinBeginExit = "OnPinBeginExit";

        public static string OnPinBeginEnter = "OnPinBeginEnter";

        public static string OnPinEndEnter = "OnPinEndEnter";



        public static string OnRopeShrink = "OnRopeShrink";

        public static string OnRopeShrinkComplete = "OnRopeShrinkComplete";

        public static string OnCuteRope = "OnCuteRope";
    }



}

