using System;

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
        /// <summary>
        /// this is the extension method for Adviser 
        /// </summary>
        /// <param name="adv"></param>
        public static void GetBalance(this Adviser adv)
        {

            Console.WriteLine("Logging in Ext method{0}",adv.Name);
        }
    }

}
