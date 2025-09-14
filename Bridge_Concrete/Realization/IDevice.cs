using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge_Concrete.Realization
{
    public interface IDevice
    {
        bool IsEnabled { get; }
        int Volume { get; set; }
        int Channel { get; set; }
        void Enable();
        void Disable();
    }
}
