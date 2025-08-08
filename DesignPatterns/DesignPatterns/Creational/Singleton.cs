namespace DesignPatterns.Creational
{
    public class Singleton
    {
        private static readonly Lazy<Singleton> _instance = new(() => new Singleton());
        private Singleton()
        {
            Console.WriteLine("Singleton initialized.");
        }

        public static Singleton Instance => _instance.Value;

        public void Log(string message)
        {
            Console.WriteLine($"[Log]: {message}");
        }


        public static void Run()
        {
            var singleton1 = Singleton.Instance;
            var singleton2 = Singleton.Instance;

            Console.WriteLine(singleton1 == singleton2);

            singleton1.Log("First message");
            singleton2.Log("Second message");
        }
    }
}