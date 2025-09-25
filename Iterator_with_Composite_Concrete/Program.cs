
namespace Iterator_Conceptual_With_Composite
{
	// Интерфейс итератора
	public interface IOrderIterator
	{
		OrderComponent Current { get; }
		bool MoveNext();
		void Reset();
	}

	// Базовый компонент с поддержкой итерации
	public abstract class OrderComponent
	{
		public abstract decimal CalculateTotal();
		public virtual void Add(OrderComponent component) =>
			throw new NotImplementedException();
		public virtual void Remove(OrderComponent component) =>
			throw new NotImplementedException();

		// Метод для создания итератора
		public virtual IOrderIterator CreateIterator() =>
			new NullIterator();

		// Метод для получения всех элементов (для простоты реализации)
		public virtual IEnumerable<OrderComponent> GetChildren() =>
			Enumerable.Empty<OrderComponent>();
	}

	// Итератор для пустых элементов (листьев)
	public class NullIterator : IOrderIterator
	{
		public OrderComponent Current => null;
		public bool MoveNext() => false;
		public void Reset() { }
	}

	// Итератор для композитных объектов
	public class CompositeIterator : IOrderIterator
	{
		private readonly Stack<IOrderIterator> _stack = new();

		public CompositeIterator(IOrderIterator iterator)
		{
			_stack.Push(iterator);
		}

		public OrderComponent Current => _stack.Count > 0 ? _stack.Peek().Current : null;

		public bool MoveNext()
		{
			if (_stack.Count == 0) return false;

			var iterator = _stack.Peek();

			if (!iterator.MoveNext())
			{
				_stack.Pop();
				return MoveNext();
			}

			var current = iterator.Current;
			if (current is ProductBundle)
			{
				_stack.Push(current.CreateIterator());
			}

			return true;
		}

		public void Reset()
		{
			_stack.Clear();
		}
	}

	// Итератор для обхода в глубину (DFS)
	public class DepthFirstIterator : IOrderIterator
	{
		private readonly List<OrderComponent> _components;
		private int _position = -1;

		public DepthFirstIterator(OrderComponent component)
		{
			_components = FlattenComponents(component).ToList();
		}

		public OrderComponent Current =>
			_position >= 0 && _position < _components.Count ? _components[_position] : null;

		public bool MoveNext()
		{
			_position++;
			return _position < _components.Count;
		}

		public void Reset() => _position = -1;

		private IEnumerable<OrderComponent> FlattenComponents(OrderComponent component)
		{
			yield return component;

			foreach (var child in component.GetChildren())
			{
				foreach (var grandChild in FlattenComponents(child))
				{
					yield return grandChild;
				}
			}
		}
	}

	// Итератор для обхода в ширину (BFS)
	public class BreadthFirstIterator : IOrderIterator
	{
		private readonly Queue<OrderComponent> _queue = new();
		private OrderComponent _current;

		public BreadthFirstIterator(OrderComponent component)
		{
			_queue.Enqueue(component);
		}

		public OrderComponent Current => _current;

		public bool MoveNext()
		{
			if (_queue.Count == 0) return false;

			_current = _queue.Dequeue();

			foreach (var child in _current.GetChildren())
			{
				_queue.Enqueue(child);
			}

			return true;
		}

		public void Reset()
		{
			_queue.Clear();
			_current = null;
		}
	}

	// Обновленные классы Product и ProductBundle
	public class Product : OrderComponent
	{
		public string Name { get; }
		public decimal Price { get; }

		public Product(string name, decimal price)
		{
			Name = name;
			Price = price;
		}

		public override decimal CalculateTotal() => Price;

		public override string ToString() => $"{Name} (${Price})";
	}

	public class ProductBundle : OrderComponent
	{
		private readonly List<OrderComponent> _children = new();
		public string Name { get; }

		public ProductBundle(string name) => Name = name;

		public override void Add(OrderComponent component) =>
			_children.Add(component);

		public override void Remove(OrderComponent component) =>
			_children.Remove(component);

		public override decimal CalculateTotal() =>
			_children.Sum(child => child.CalculateTotal());

		public override IOrderIterator CreateIterator() =>
			new CompositeIterator(new ListIterator(_children));

		public override IEnumerable<OrderComponent> GetChildren() => _children;

		public override string ToString() =>
			$"{Name} [${CalculateTotal()}]";
	}

	// Итератор для списка компонентов
	public class ListIterator : IOrderIterator
	{
		private readonly List<OrderComponent> _components;
		private int _position = -1;

		public ListIterator(List<OrderComponent> components)
		{
			_components = components;
		}

		public OrderComponent Current =>
			_position >= 0 && _position < _components.Count ? _components[_position] : null;

		public bool MoveNext()
		{
			_position++;
			return _position < _components.Count;
		}

		public void Reset() => _position = -1;
	}

	// Утилитарный класс для расширенных операций с итераторами
	public static class OrderIteratorExtensions
	{
		public static IEnumerable<OrderComponent> ToEnumerable(this IOrderIterator iterator)
		{
			while (iterator.MoveNext())
			{
				yield return iterator.Current;
			}
		}

		public static List<Product> GetAllProducts(this OrderComponent component)
		{
			return component.CreateIterator()
				.ToEnumerable()
				.OfType<Product>()
				.ToList();
		}

		public static decimal GetTotalPrice(this OrderComponent component)
		{
			return component.CreateIterator()
				.ToEnumerable()
				.OfType<Product>()
				.Sum(product => product.Price);
		}
	}

	// Клиентский код
	class Program
	{
		static void Main()
		{
			// Создаем товары
			var laptop = new Product("MacBook Pro", 2500m);
			var mouse = new Product("Magic Mouse", 99m);
			var keyboard = new Product("Magic Keyboard", 149m);
			var headphones = new Product("AirPods Pro", 249m);

			// Создаем комплекты
			var computerSet = new ProductBundle("Computer Set");
			var peripheralSet = new ProductBundle("Peripheral Set");
			var premiumSet = new ProductBundle("Premium Set");

			// Строим иерархию
			peripheralSet.Add(mouse);
			peripheralSet.Add(keyboard);

			computerSet.Add(laptop);
			computerSet.Add(peripheralSet);

			premiumSet.Add(computerSet);
			premiumSet.Add(headphones);

			// Демонстрация разных итераторов

			Console.WriteLine("=== Обход в глубину (DFS) ===");
			var dfsIterator = new DepthFirstIterator(premiumSet);
			while (dfsIterator.MoveNext())
			{
				var component = dfsIterator.Current;
				Console.WriteLine(component);
			}

			Console.WriteLine("\n=== Обход в ширину (BFS) ===");
			var bfsIterator = new BreadthFirstIterator(premiumSet);
			while (bfsIterator.MoveNext())
			{
				var component = bfsIterator.Current;
				Console.WriteLine(component);
			}

			Console.WriteLine("\n=== Композитный итератор (только листья) ===");
			var compositeIterator = premiumSet.CreateIterator();
			while (compositeIterator.MoveNext())
			{
				var product = compositeIterator.Current as Product;
				if (product != null)
				{
					Console.WriteLine($"Товар: {product.Name} - ${product.Price}");
				}
			}

			Console.WriteLine("\n=== Использование extension методов ===");
			var allProducts = premiumSet.GetAllProducts();
			Console.WriteLine($"Все товары: {string.Join(", ", allProducts.Select(p => p.Name))}");
			Console.WriteLine($"Общая стоимость: ${premiumSet.GetTotalPrice()}");

			// Поиск конкретного товара
			Console.WriteLine("\n=== Поиск товара ===");
			var searchIterator = premiumSet.CreateIterator();
			while (searchIterator.MoveNext())
			{
				if (searchIterator.Current is Product product && product.Name.Contains("AirPods"))
				{
					Console.WriteLine($"Найден товар: {product}");
					break;
				}
			}
		}
	}
}