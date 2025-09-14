using Bridge_Concrete.Abstraction;
using Bridge_Concrete.Realization;

namespace Bridge_Concrete
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1. Телевизор с базовым пультом");

            var tv = new TV();
            var remote = new Remote(tv);

            remote.TogglePower();
            remote.ChannelUp();
            remote.VolumeUp();
            remote.DisplayStatus();

            Console.WriteLine();


            Console.WriteLine("2. Радио с расширенным пультом");

            var radio = new Radio();
            var advancedRemote = new AdvancedRemote(radio);

            advancedRemote.TogglePower();
            advancedRemote.SetChannel(105);
            advancedRemote.VolumeUp();
            advancedRemote.Mute();
            advancedRemote.DisplayStatus();


            Console.WriteLine();
        }
    }
}
