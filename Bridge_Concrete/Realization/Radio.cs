using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge_Concrete.Realization
{
    public class Radio : IDevice
    {
        private bool _isEnabled;
        private int _volume = 30;
        private int _channel = 101;
        private string _station = "FM";

        public bool IsEnabled => _isEnabled;

        public int Volume
        {
            get => _volume;
            set => _volume = Math.Clamp(value, 0, 100);
        }
        public int Channel
        {
            get => _channel;
            set => _channel = Math.Max(1, value);
        }

        public string Station => _station;

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
            return $"Радио [ВКЛ: {IsEnabled}, Громкость: {Volume}, Канал:{Channel}{Station}]";
        }
    }
}
