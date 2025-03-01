using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiFramewark;

namespace Pools
{
    public class DefaultUiPool : DefaultPool
    {
        public DefaultUiPool(IMPoolAble pooledObj, int ordinaryCount = 20) : base(pooledObj, ordinaryCount)
        {

        }

    }
}
