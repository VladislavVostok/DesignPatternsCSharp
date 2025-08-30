using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DBSysteAbstractFactory_Concrete
{

    public interface ICache
    {
        void Put(string key, string value);
        string Get(string key);
    }

    public class RedisCache : ICache
    {
        public string Get(string key)
        {
            Console.WriteLine($"Читаем из Redis: {key}");
            return "данные из Redis";
        }

        public void Put(string key, string value)
        {
            Console.WriteLine($"Сохранение в Redis: {key} = {value}");
        }
    }

    public class MemcachedCache : ICache
    {
        public string Get(string key)
        {
            Console.WriteLine($"Читаем из Memcach: {key}");
            return "данные из Memcach";
        }

        public void Put(string key, string value)
        {
            Console.WriteLine($"Сохранение в Memcach: {key} = {value}");
        }
    }




    public interface IDatabase
    {
        void Save(string data);
        string Load();
    }

    public class PostgresDatabase : IDatabase
    {
        public string Load()
        {
            Console.WriteLine($"Читаем из Postgres");
            return "Данные из Postgres";
        }

        public void Save(string data)
        {
            Console.WriteLine($"Сохранение в Postgres: {data}");
        }
    }

    public class MongoDatabase : IDatabase
    {
        public string Load()
        {
            Console.WriteLine($"Читаем из Mongo");
            return "Данные из Mongo";
        }

        public void Save(string data)
        {
            Console.WriteLine($"Сохранение в Mongo: {data}");
        }
    }


    public interface IDataAccessFactory
    {
        ICache CreateCache();
        IDatabase CreateDatabase();
    }

    public class RedisPostgresFactory : IDataAccessFactory
    {
        public ICache CreateCache()
        {
            return new RedisCache();
        }

        public IDatabase CreateDatabase()
        {
            return new PostgresDatabase();
        }
    }

    public class MemcachedMongoFactory : IDataAccessFactory
    {
        public ICache CreateCache()
        {
            return new MemcachedCache();
        }

        public IDatabase CreateDatabase()
        {
            return new MongoDatabase();
        }
    }


    public class Application
    {
        private readonly ICache _cache;
        private readonly IDatabase _database;

        public Application(IDataAccessFactory factory) {
            _cache = factory.CreateCache(); 
            _database = factory.CreateDatabase();
        }

        public void Run()
        {
            string data = _database.Load();
            _cache.Put("key", data);
            Console.WriteLine("Полученно: " + _cache.Get("key"));
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            IDataAccessFactory factory;

            string env = "Production";

            if(env == "Production")
            {
                factory = new MemcachedMongoFactory();
            }
            else
            {
                factory = new RedisPostgresFactory();
            }

            var app = new Application(factory);
            app.Run();


            Console.ReadKey();
        }
    }
}
