namespace Observer_Concrete
{
    using System;
    using System.Collections.Generic;

    // Общий интерфейс подписчиков
    public interface IEventListener
    {
        void Update(string filename);
    }

    // Базовый класс-издатель. Содержит код управления подписчиками
    // и их оповещения.
    public class EventManager
    {
        private Dictionary<string, List<IEventListener>> listeners = new Dictionary<string, List<IEventListener>>();

        public void Subscribe(string eventType, IEventListener listener)
        {
            if (!listeners.ContainsKey(eventType))
            {
                listeners[eventType] = new List<IEventListener>();
            }

            listeners[eventType].Add(listener);
        }

        public void Unsubscribe(string eventType, IEventListener listener)
        {
            if (listeners.ContainsKey(eventType))
            {
                listeners[eventType].Remove(listener);
            }
        }

        public void Notify(string eventType, string data)
        {
            if (listeners.ContainsKey(eventType))
            {
                foreach (var listener in listeners[eventType])
                {
                    listener.Update(data);
                }
            }
        }
    }

    // Вспомогательный класс File для демонстрации
    public class File
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public File(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public void Write()
        {
            // Реализация записи файла
            Console.WriteLine($"Запись файла: {Name}");
        }
    }

    // Конкретный класс-издатель
    public class Editor
    {
        public EventManager Events { get; private set; }
        private File file;

        public Editor()
        {
            Events = new EventManager();
        }

        // Методы бизнес-логики, которые оповещают подписчиков об изменениях
        public void OpenFile(string path)
        {
            this.file = new File(path);
            Events.Notify("open", file.Name);
        }

        public void SaveFile()
        {
            if (file != null)
            {
                file.Write();
                Events.Notify("save", file.Name);
            }
        }
    }

    // Конкретный подписчик - логирование
    public class LoggingListener : IEventListener
    {
        private string logPath;
        private string message;

        public LoggingListener(string logPath, string message)
        {
            this.logPath = logPath;
            this.message = message;
        }

        public void Update(string filename)
        {
            string logMessage = message.Replace("%s", filename);
            Console.WriteLine($"[LOG {logPath}] {logMessage}");
            // Здесь была бы реальная запись в файл
            // System.IO.File.AppendAllText(logPath, logMessage + Environment.NewLine);
        }
    }

    // Конкретный подписчик - email уведомления
    public class EmailAlertsListener : IEventListener
    {
        private string email;
        private string message;

        public EmailAlertsListener(string email, string message)
        {
            this.email = email;
            this.message = message;
        }

        public void Update(string filename)
        {
            string emailMessage = message.Replace("%s", filename);
            Console.WriteLine($"[EMAIL to {email}] {emailMessage}");
            // Здесь была бы реальная отправка email
            // system.email(email, emailMessage);
        }
    }

    // Приложение для демонстрации работы
    public class Application
    {
        public void Config()
        {
            Editor editor = new Editor();

            var logger = new LoggingListener(
                "/path/to/log.txt",
                "Someone has opened file: %s");
            editor.Events.Subscribe("open", logger);

            var emailAlerts = new EmailAlertsListener(
                "admin@example.com",
                "Someone has changed the file: %s");
            editor.Events.Subscribe("save", emailAlerts);

            // Демонстрация работы
            editor.OpenFile("document.txt");
            editor.SaveFile();
        }
    }

    // Пример использования
    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application();
            app.Config();

            // Дополнительный пример
            Editor editor = new Editor();

            editor.Events.Subscribe("open", new LoggingListener("log.txt", "Файл открыт: %s"));
            editor.Events.Subscribe("save", new EmailAlertsListener("user@test.com", "Файл сохранен: %s"));

            editor.OpenFile("test.docx");
            editor.SaveFile();
        }
    }
}
