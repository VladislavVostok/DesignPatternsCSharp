namespace Composite_Concrete
{

    public interface IOrderIterator
    {
        OrderComponent Current { get; }
        bool MoveNext();
        void Reset();
    }


    public abstract class OrderComponent
    {
        public abstract decimal CalculateTotal();
        public virtual void Add(OrderComponent component) => 
            throw new NotImplementedException();

        public virtual void Remove(OrderComponent component) => 
            throw new NotImplementedException();

        public virtual IOrderIterator CreateIterator() => "fasdfasdf"
            
        public virtual IEnumerable<OrderComponent> GetChildren() => 
            Enumerable.Empty<OrderComponent>(); 
    }

    public class NullIterator : IOrderIterator
    {
        public OrderComponent Current => null;

        public bool MoveNext() => false;

        public void Reset() { }

	}

	public class CompositeIterator : IOrderIterator
	{

        private readonly Stack<IOrderIterator> _stack = new();
        public CompositeIterator(IOrderIterator iterator) { 
            _stack.Push(iterator);
        }

		public OrderComponent Current => _stack.Count > 0 ? _stack.Peek().Current: null;

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

            if(current is ProductBundle)
            {
                _stack.Push(current.CreateIterator());
            }
            return true;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}

    public class DepthFirstIterator: IOrderIterator
    {
        private readonly List<OrderComponent> _components;
        private int _position = -1;

        public DepthFirstIterator(OrderComponent component)
        {
            
        }

		public OrderComponent Current => throw new NotImplementedException();

		public bool MoveNext()
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}


	public class Product : OrderComponent { 
    
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public override decimal CalculateTotal() => Price;
        public override string ToString() => $"{Name} (${Price})";

    }

    public class ProductBundle : OrderComponent {
        private readonly List<OrderComponent> _products = new();

        public string Name { get; private set; }

        public ProductBundle(string name) => Name = name;

        public override void Add(OrderComponent component)
        {
            _products.Add(component);
        }

        public override void Remove(OrderComponent component) {
            _products.Remove(component);
        }

        public override decimal CalculateTotal() =>
           _products.Sum(child => child.CalculateTotal());

        public override string ToString()
        {
            return $"{Name} [${CalculateTotal()}] \n{string.Join("\n", _products)}";
        }

    }



    class Program
    {
        static void Main(string[] args)
        {
            var laptop = new Product("MacBook Pro", 2500m);
            var mouse = new Product("Magic Mouse", 99m);
            var keyboard = new Product("Magic Keyboard", 149m);

            var computerSet = new ProductBundle("Computer Set");
            var peripheralSet = new ProductBundle("Peripheral Set");

            peripheralSet.Add(mouse);
            peripheralSet.Add(keyboard);

            computerSet.Add(laptop);
            computerSet.Add(peripheralSet);

            Console.WriteLine(computerSet.ToString());
            Console.WriteLine($"Total: ${computerSet.CalculateTotal()}");
        }
    }
}
