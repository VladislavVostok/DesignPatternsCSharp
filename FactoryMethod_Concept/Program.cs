namespace FactoryMethod_Concept
{
	public interface IProduct
	{
		string Operation();
	}

	abstract class Creator
	{
		public abstract Task<IProduct> FactoryMethodAsync();

		public async Task<string> SomeOperation()
		{
			var product = await FactoryMethodAsync();
			var res = "Создатель: Некоторый код создателя просто работал с " + product.Operation();
			return res;
		}
	}

	class ConcreteCreator1 : Creator
	{
		public async override Task<IProduct> FactoryMethodAsync()
		{
			await Task.Delay(10);
			return new ConcreteProduct1();
		}
	}

	class ConcreteCreator2 : Creator
	{
		public async override Task<IProduct> FactoryMethodAsync()
		{
            await Task.Delay(10);
            return new ConcreteProduct2();
		}
	}

	class ConcreteProduct1 : IProduct
	{
		public string Operation()
		{
			return "{Результат Конкретного продукта 1}";
		}
	}

	class ConcreteProduct2 : IProduct
	{
		public string Operation()
		{
			return "{Результат Конкретного продукта 2}";
		}
	}

	class Client
	{

		public void Main()
		{
			ClientCode(new ConcreteCreator1());
			ClientCode(new ConcreteCreator2());
		}


		public void ClientCode(Creator creator)
		{
			Console.WriteLine("Клиен:" + creator.SomeOperation());
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			new Client().Main();
		}

	}
}
