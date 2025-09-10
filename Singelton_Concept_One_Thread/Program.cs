using System.Text;

namespace Singelton_Concept_One_Thread
{
	public sealed class Singelton
	{
		private static Singelton _instance;
		private Guid g ;
		private Singelton() {
			g = Guid.NewGuid();
		}

		public static Singelton GetInstance() {

			if (_instance == null)
			{
				_instance = new Singelton();
			}
			
			return _instance;
		}

		public void someBussinesLogic()
		{
			Console.WriteLine($"Я бизнес-логика! {g}");
		}

	}


	public sealed class SingeltonMultyThreading
	{
		private static SingeltonMultyThreading _instance;
		private static readonly object _lock = new object();
		public string Value { get; set; }
		private Guid g;
		private SingeltonMultyThreading()
		{
			g = Guid.NewGuid();
		}

		public static SingeltonMultyThreading GetInstance(string value)
		{

				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = new SingeltonMultyThreading();
						_instance.Value = value;
					}
				}


			return _instance;
		}

		public void someBussinesLogic()
		{
			Console.WriteLine($"Я бизнес-логика! {g}");
		}

	}


	internal class Program
	{
		public static void TestSingleton(string value)
		{
			SingeltonMultyThreading sing = SingeltonMultyThreading.GetInstance(value);
			Console.WriteLine(sing.Value);
		}

		static void Main(string[] args)
		{
			//Singelton s1 = Singelton.GetInstance();
			//s1.someBussinesLogic();

			//Singelton s2 = Singelton.GetInstance();
			//s2.someBussinesLogic();

			//if (s1 == s2)
			//{
			//	Console.WriteLine("Мы близнецы!");
			//}
			//else
			//{
			//	Console.WriteLine("Мы чужие друг другу!");
			//}

			Thread process1 = new Thread(() => {
				TestSingleton("FOO");
			});

			Thread process2 = new Thread(() => {
				TestSingleton("BAR");
			});

			process1.Start();
			process2.Start();

			process1.Join();
			process2.Join();


		}
	}
}
