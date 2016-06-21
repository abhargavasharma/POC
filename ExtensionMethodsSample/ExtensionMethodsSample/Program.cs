using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethodsSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var adv = new Adviser { ID = "1" , Name = "bhargav" };
            adv.GetBalance();
            Console.ReadKey();
        }
    }

    class Adviser
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }


    static class ExtensionMethods
    {
        public static void GetBalance(this Adviser adv)
        {

            Console.WriteLine("Logging in Ext method{0}",adv.Name);
        }
    }
}
