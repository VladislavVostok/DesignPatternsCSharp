namespace Adapter_Concept
{
    public class Program
    {

        // Целевой класс объявляет интерфейс, с которым может работать клиентский код.
        public interface ITarget
        {
            string GetRequest();
        }

        // Адаптируемый класс, который будет содержать какое-то полезное поведение,
        // но его интерфейс не стовместим с существующим кодом. Нуждается в некоторой доработке.
        class Adaptee
        {
            public string GetSpecificRequest() {
                return "Specific request.";
            }
        }

        // Адаптер делает интерфейс Адаптируемого класса совместимым с целевым итерфейсом
        class UpperAdapter : ITarget
        {
            private readonly Adaptee _adaptee;

            public UpperAdapter(Adaptee adaptee) { 
                _adaptee = adaptee;
            }

            public string GetRequest()
            {
                return $"This is '{this._adaptee.GetSpecificRequest()}'".ToUpper();
            }
        }

        class LowerAdapter : ITarget
        {
            private readonly Adaptee _adaptee;

            public LowerAdapter(Adaptee adaptee)
            {
                _adaptee = adaptee;
            }

            public string GetRequest()
            {
                return $"This is '{this._adaptee.GetSpecificRequest()}'".ToLower();
            }
        }



        static void Main(string[] args)
        {
            Adaptee adaptee = new Adaptee();
            ITarget targetUpper = new UpperAdapter(adaptee);
            Console.WriteLine(targetUpper.GetRequest());


            ITarget targetLower = new LowerAdapter(adaptee);
            Console.WriteLine(targetLower.GetRequest());
        }
    }
}
