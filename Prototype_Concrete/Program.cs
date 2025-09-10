namespace Prototype_Concrete
{


	public class Car: ICloneable
	{
		private List<string> _parts = new();
		public string Name { get; set; }
		public string VIN { get; set; }

		public void AddPart(string part)
		{
			_parts.Add(part);
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		public void ShowParts()
		{
			Console.WriteLine("Части автомобиля: " + string.Join(", ", _parts));
		}
	}


	internal class Program
	{
		static void Main(string[] args)
		{

			var oldCar = new Car();
			oldCar.Name = "Вася";
			oldCar.VIN = "VINIKJGOIU#*U*#YU%";
			oldCar.AddPart("Четыре колеса");
			oldCar.AddPart("Кузов");
			oldCar.AddPart("Мотор");
			oldCar.AddPart("Тормоза");


			var newCar = new Car();
		    newCar.Name = oldCar.Name;
			newCar.VIN = oldCar.VIN;



		}
	}
}
