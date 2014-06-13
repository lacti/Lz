using System;

namespace LzServer
{
    internal class Program
    {
        private readonly ServerContext _context;
        private readonly Coroutine _coroutine;
        private readonly DataManager _data;
        private readonly Listener _listener;

        public Program()
        {
            _data = new DataManager();
            _listener = new Listener();
            _coroutine = new Coroutine();
            _context = new ServerContext(_listener, _data, _coroutine);

            _listener.Port = _data.World.Config.port;
            _listener.RegisterHandler(new PacketHandler(_context));

            _coroutine.AddEntry(_context.SaveContextEntry);
        }

        public void Start()
        {
            _coroutine.Start();
            _listener.Start();
        }

        public static void Main(string[] args)
        {
            var program = new Program();
            program.Start();

            Console.WriteLine("running");
            Console.ReadLine();
        }
    }
}