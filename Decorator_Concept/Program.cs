namespace Decorator_Concept
{
    // Базовый интерфейс Компонента определяет поведение,
    // которое изменяется декораторами.
    public abstract class Component
    {
        public abstract string Operation();
    }

    // Конкретные Компоненты предоставляют реализации поведения по умолчанию.
    // Может быть несколько вариации этих класов.
    class ConcreteComponent : Component {
        public override string Operation() {
            return "ConcreteComponent";
        }
    }

    // Базовый класс Декоратора следует тому же интерфейсу, что и другие
    // компоненты. Основная цель этого класса - определить интерфейс обёртки для 
    // всех конкретных декораторов.
    abstract class  Decorator : Component
    {
        protected Component _component;

        public Decorator(Component component) { 
            _component = component;
        }

        public void SetComponent(Component component) {
            _component = component;
        }


        // Декоратор делегирует всю работу обёрнутому компоненту.
        public override string Operation() {
            if (_component != null)
            {

                return this._component.Operation();
            }
            else {

                return string.Empty;
            }
        }
    }

    // Конкретный Декоратор вызывают обёрнутый объект и изменяют
    // его результат неторым образом.
    class ConcreteDecoratorA : Decorator
    {
        public ConcreteDecoratorA(Component comp) : base(comp) { }

        public void SetComponent(Component component)
        {
            _component = component;
        }


        // Декораторы могут вызывать родительскую реализацию операции,
        // вместо того, чтобы вызывать обёрнутый объект на прямую
        public override string Operation()
        {
            return $"ConcreteDecoratorA({base.Operation()})";      
        }
    }


    class ConcreteDecoratorB : Decorator
    {
        public ConcreteDecoratorB(Component comp) : base(comp) { }

        public void SetComponent(Component component)
        {
            _component = component;
        }


        // Декораторы могут вызывать родительскую реализацию операции,
        // вместо того, чтобы вызывать обёрнутый объект на прямую
        public override string Operation()
        {
            return $"ConcreteDecoratorB({base.Operation()})";
        }
    }

    public class Client
    {
        public void ClientCode(Component comp)
        {
            Console.WriteLine("Result: " + comp.Operation());
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            bool slack = false;
            bool face = false;
            Component stack = new ConcreteComponent();   // Типо емаил
            string slack_str = string.Empty;
            string face_str = string.Empty;

            while (true) {
                
                if (slack)
                    stack = new ConcreteDecoratorA(stack);

                if (face)
                    stack = new ConcreteDecoratorB(stack);

                slack_str = Console.ReadLine();
                face_str = Console.ReadLine();

                if (slack_str.Trim().ToLower() == "t")
                    slack = true;
                else
                    slack = false;

                if (face_str.Trim().ToLower() == "t")
                    face = true;
                else
                    face = false;

                client.ClientCode(stack);
            }

        }
    }
}
