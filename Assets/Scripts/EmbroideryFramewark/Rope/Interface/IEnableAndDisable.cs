using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbroideryFramewark
{
    public interface IEnableAndDisable
    {
        public bool IsEnable { get; }

        void Enable();

        void Disable(); 
    }
}
