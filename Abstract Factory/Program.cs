/*
 Абстрактная фабрика - 
 */



using System.Globalization;

namespace AbstractFactory.Concept
{

    // Интерфейс Астрактной Фабрики объявляет набор методов, которые возвращают различные абстрактные продукт.
    public interface IAbstractFactory
    {
        IAbstractProductA CreateProductA();
        IAbstractProductB CreateProductB();
    }



    // Конкретная Фабрика производит семейство продуктов одной вариации.
    // Фабрика гарантирует совместимость полученных продуктов. 
    class ConcreteFactory1 : IAbstractFactory
    {
        public IAbstractProductA CreateProductA()
        {
            return new ConcreteProductA1();
        }

        public IAbstractProductB CreateProductB()
        {
            return new ConcreteProductB1();
        }
    }

    class ConcreteFactory2 : IAbstractFactory
    {
        public IAbstractProductA CreateProductA()
        {
            return new ConcreteProductA2();
        }

        public IAbstractProductB CreateProductB()
        {
            return new ConcreteProductB2();
        }
    }

    // Каждый отдельный продукт семейства продуктов должен иметь базовый интерфейс и все вариации продукта должны реализовывать этот интерфейс.
    public interface IAbstractProductA
    {
        string UsefulFunctionA();
    }


    // Конкретные продукты создаются соответствующими Конкретными фабриками.
    class ConcreteProductA1 : IAbstractProductA
    {
        public string UsefulFunctionA()
        {
            return "Результат продукта A1.";

        }
    }

    class ConcreteProductA2 : IAbstractProductA
    {
        public string UsefulFunctionA()
        {
            return "Результат продукта A2.";
        }
    }

    // Базовый интерфейс другого продуктаа.
    public interface IAbstractProductB
    {

        // Продукт способен работать самостоятельно
        string UsefulFunctionB();

        // а также взаимодействовать с Продуктами А тойже вариации.
        // Абстрактная Фабрика гарантирует, что все продукты, которые она создает, имеют одинаковую вариацию, и совместимы.
        string AnotherUsefulFunctionB(IAbstractProductA collobarator);
    }


    class ConcreteProductB1 : IAbstractProductB
    {
        public string AnotherUsefulFunctionB(IAbstractProductA collobarator)
        {

            var result  = collobarator.UsefulFunctionA();
            return $"Результат B1 сотрудничества с {result}";
        }

        public string UsefulFunctionB()
        {
            return "Результат продукта B1.";
        }
    }


    class ConcreteProductB2 : IAbstractProductB
    {
        public string AnotherUsefulFunctionB(IAbstractProductA collobarator)
        {

            var result = collobarator.UsefulFunctionA();
            return $"Результат B2 сотрудничества с {result}";
        }

        public string UsefulFunctionB()
        {
            return "Результат продукта B2.";
        }
    }

    class Client
    {
        public void Main()
        {
            Console.WriteLine("Клиент: Тестирует клиентский код с фабрикой первого типа..");
            ClientMethod(new ConcreteFactory1());
            Console.WriteLine();


            Console.WriteLine("Клиент: Тестирует клиентский код с фабрикой второго типа..");
            ClientMethod(new ConcreteFactory2());

        }

        public void ClientMethod(IAbstractFactory factory)
        {
            var productA = factory.CreateProductA();
            var productB = factory.CreateProductB();

            Console.WriteLine(productB.UsefulFunctionB());
            Console.WriteLine(productB.AnotherUsefulFunctionB(productA));

        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }


            new Client().Main();
        }
    }



}
