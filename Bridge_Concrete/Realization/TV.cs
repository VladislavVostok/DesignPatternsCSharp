using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge_Concrete.Realization
{
    public class TV : IDevice
    {

        private bool _isEnabled;
        private int _volume = 50;
        private int _channel = 1;


        public bool IsEnabled => _isEnabled;

        public int Volume { 
            get => _volume;
            set => _volume = Math.Clamp(value, 0, 100); 
        }
        public int Channel
        {
            get => _channel;
            set => _channel = Math.Max(1, value);
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void Enable()
        {
           _isEnabled = true;
        }

        public override string ToString()
        {
            return $"Телевизор [ВКЛ: {IsEnabled}, Громкость: {Volume}, Канал:{Channel}]";
        }

    }
}
