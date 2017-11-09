using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("TradeApp.Tests")]
namespace TradeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static void Run(TradeAppConfig config)
        {
            throw new NotImplementedException();
        }
    }
}
