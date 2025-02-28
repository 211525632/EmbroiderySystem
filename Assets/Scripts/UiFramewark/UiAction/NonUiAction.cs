using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiFramewark
{
    /// <summary>
    /// 什么都不做
    /// </summary>
    public sealed class NonUiAction : BasicUiAction
    {
        private static NonUiAction _nonAction = new NonUiAction();

        public static NonUiAction Instance { get { return _nonAction; } }

        public override void OnCreate()
        {
            
        }

        public override void OnDestroy()
        {
            
        }

        public override void OnDisable()
        {
            
        }

        public override void OnEnable()
        {
            
        }
    }
}
