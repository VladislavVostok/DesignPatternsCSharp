namespace Composite_Concrete
{

    public abstract class OrderComponent
    {
        public abstract decimal CalculateTotal();
        public virtual void Add(OrderComponent component) => 
            throw new NotImplementedException();

        public virtual void Remove(OrderComponent component) => 
            throw new NotImplementedException();
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
