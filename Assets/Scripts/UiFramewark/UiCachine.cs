using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cachine;

namespace UiFramewark
{
    /// <summary>
    /// 对Hide的Ui进行缓存
    /// </summary>
    public class UiCachine : Cachine<UiCollection>
    {
        public UiCachine(float checkDeltaTime) : base(checkDeltaTime)
        {

        }
    }
}
