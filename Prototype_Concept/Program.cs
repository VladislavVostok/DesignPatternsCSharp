using System.Data;
using System.Net;

namespace Prototype_Concept
{
	//interface IDeepCloneable
	//{
	//	IDeepCloneable DeepClone();
	//}

	public class Person : ICloneable
	{
		public int Age;
		public DateTime BirthDate;
		public string Name;
		public IdInfo idInfo;

		public object Clone() => DeepCopy();

		private Person DeepCopy()
		{
			var clone = (Person)MemberwiseClone();
			clone.idInfo = (IdInfo)idInfo?.Clone();
			clone.Name = Name;
			return clone;
		}



	}

	public class IdInfo : ICloneable
	{
		public int IdNumber;
		public IdInfo(int idNumber) => this.IdNumber = idNumber;


		public object Clone() => DeepCopy();

		private IdInfo DeepCopy() => new IdInfo(IdNumber);
	}

	internal class Program
	{
		static void Main(string[] args)
		{
			Person p1 = new Person
			{
				Age = 23,
				BirthDate = DateTime.Parse("1991-01-01"),
				Name = "Jack Daniels",
				idInfo = new IdInfo(1)
			};
		
			Person p2 = (Person) p1.Clone();
			Person p3 = (Person) p1.Clone();


			p1.Age = 33;
			p1.BirthDate = DateTime.Parse("1900-01-01");
			p1.Name = "Shalom";
			p1.idInfo.IdNumber = 7878;

			Person p4 = (Person) p1.Clone();


		}

	}
}
