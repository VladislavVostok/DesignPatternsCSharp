using Bridge_Concrete.Realization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge_Concrete.Abstraction
{
    public class AdvancedRemote : Remote
    {
        public AdvancedRemote(IDevice device) : base(device) { }
        public void Mute()
        {
            _device.Volume = 0;
            Console.WriteLine("Режим без звука активирован");
        }
        public void SetChannel(int channel)
        {
            _device.Channel = channel;
            Console.WriteLine($"Переключенно на канал: {_device.Channel}");
        }

        public override void DisplayStatus() {
            base.DisplayStatus();
            Console.WriteLine($"=== Расширенный пульт ===");
        }
    }
}
