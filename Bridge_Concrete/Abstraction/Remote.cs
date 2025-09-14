using Bridge_Concrete.Realization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge_Concrete.Abstraction
{
    public class Remote
    {
        protected IDevice _device;

        public Remote(IDevice device)
        {
            this._device = device;
        }

        public void TogglePower()
        {
            if (_device.IsEnabled)
            {
                _device.Disable();
                Console.WriteLine("Устройство выключено");
            }
            else
            {
                _device.Enable();
                Console.WriteLine("Устройство включено");

            }
        }

        public void VolumeDown()
        {
            if (_device.IsEnabled)
            {
                if (_device.Volume >= 10)
                {
                    _device.Volume -= 10;
                }
            }

        }

        public void VolumeUp()
        {
            if (_device.IsEnabled)
            {
                if (_device.Volume <= 90)
                {
                    _device.Volume += 10;
                }
            }
        }


        public void ChannelDown()
        {
            if (_device.IsEnabled)
            {

                _device.Channel -= 1;
            }

        }

        public void ChannelUp()
        {
            if (_device.IsEnabled)
            {
                _device.Channel += 1;
            }
        }

        public virtual void DisplayStatus()
        {
            Console.WriteLine($"Статус: {(_device.IsEnabled ? "ВКЛ" : "ВЫКЛ")}");
            Console.WriteLine($"Громкость: {_device.Volume}%");
            Console.WriteLine($"Канал: {_device.Channel}");

        }

    }
}
