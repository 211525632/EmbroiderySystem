using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachine
{
    /// <summary>
    /// 这个物品可以被“打包”
    /// </summary>
    public interface ICachinePackage<T>where T : class,ISelfDestroyAble
    {
        public void UnpackingAndDele();

        public void Packing(T targetObj);
    }
}
