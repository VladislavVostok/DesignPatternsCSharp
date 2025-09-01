using System.Xml.Serialization;

namespace Builder
{

	public class Car
	{
		private List<string> _parts = new();

		public void AddPart(string part)
		{
			_parts.Add(part);
		}

		public void ShowParts()
		{
			Console.WriteLine("Части автомобиля: "  + string.Join(", ", _parts));
		}
	}

	public interface ICarBuilder
	{
		void BuildEngine();
		void BuildSeats();
		void BuildGPS();
		Car GetCar();

	}


	public class SportsCarBuilder : ICarBuilder
	{
		private Car _car = new();

		public void BuildEngine()
		{
			_car.AddPart("Спортивный двигатель");
		}

		public void BuildGPS()
		{
			_car.AddPart("Навигационная система премиум-класса");
		}

		public void BuildSeats()
		{
			_car.AddPart("Два понтовейших кожаных сидения");
		}

		public Car GetCar()
		{
			return _car;
		}
	}


	public class MiniCarBuilder : ICarBuilder
	{
		private Car _car = new();

		public void BuildEngine()
		{
			_car.AddPart("Микроскопический двигатель");
		}

		public void BuildGPS()
		{
			_car.AddPart("Навигационная система ГЛОНАСС");
		}

		public void BuildSeats()
		{
			_car.AddPart("Два понтовейших очень маленьких кожаных сидения");
		}

		public Car GetCar()
		{
			return _car;
		}
	}

	public class Director
	{
		private ICarBuilder _builder;

		public Director(ICarBuilder builder)
		{
			_builder = builder;
		}

		public void ConstructCar()
		{
			_builder.BuildEngine();
			_builder.BuildSeats();
			_builder.BuildGPS();
		}


	}

	internal class Program
	{
		static void Main(string[] args)
		{
			var builder = new SportsCarBuilder();
			var director = new Director(builder);

			director.ConstructCar();
			Car sportCar = builder.GetCar();
			sportCar.ShowParts();

			var builder2 = new MiniCarBuilder();
			var director2 = new Director(builder2);

			director2.ConstructCar();
			Car miniCar = builder2.GetCar();
			miniCar.ShowParts();
		}
	}
}
