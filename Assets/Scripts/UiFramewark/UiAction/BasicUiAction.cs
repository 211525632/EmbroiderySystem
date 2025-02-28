using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLua;

namespace UiFramewark
{

    public abstract class BasicUiAction : IUiAction
    {
        public BasicUiAction()
        {

        }

        public abstract void OnCreate();
        public abstract void OnDestroy();
        public abstract void OnDisable();
        public abstract void OnEnable();
    }
}
