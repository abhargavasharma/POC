using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericsIndepth
{
    class Program
    {
        static void Main(string[] args)
        {
            IMyTesting obj = new Square();
            Console.WriteLine("Square Area:" + obj.Area(10, 10));

            obj = new Cube();
            obj.length = 2;
            Console.WriteLine("Cube Area:" + obj.Area(10, 10));

            Cube obj1 = new Cube();
            Console.WriteLine("Cube Area from generics:" + getArea(obj1));

            obj = new Square();
            Console.WriteLine("Square Area from generics:" + getArea(obj, 10));

            Console.ReadKey();
        }

        //Creating a generic method of type Class and using those members internally in generic method
        public static int getArea<T>(T obj) where T : Cube
        {
            obj.length = 5;
            return obj.Area(1, 10);
        }

        //Creating a generic method of type Interface and using those members internally in generic method
        public static int getArea<T>(T obj, int l) where T : IMyTesting
        {
            obj.length = l;
            return obj.Area(2, 100);
        }

    }

    interface IMyTesting
    {
        int length { get; set; }
        int Area(int length, int breadth);
    }

    class Square : IMyTesting
    {
        private int length1;
        public int length { get { return length1; } set { length1 = 1; } }
        public virtual int Area(int length, int beadth)
        {
            return length * beadth;
        }
    }

    class Cube : IMyTesting
    {
        private int length1;
        public int length { get { return length1; } set { length1 = value; } }
        public virtual int Area(int length, int beadth)
        {
            return length * beadth * length1;
        }
    }


    //// PersonalIdentification Class 
    //public class PersonalIdentification
    //{
    //    private string _name;
    //    private DateTime _dob;

    //    public PersonalIdentification(string name)
    //    {
    //        _name = name;
    //        _dob = new DateTime(0);
    //    }

    //    public string FullName
    //    {
    //        get { return _name; }
    //        set { _name = value; }
    //    }

    //    public DateTime DateofBirth
    //    {
    //        get { return _dob; }
    //        set
    //        {
    //            _dob = value;
    //        }
    //    }
    //}

    ////Calling Constraint of type class (Class name: PersonalIdentification)
    //public class Employee<T>
    //where T : PersonalIdentification
    //{
    //    private T info;

    //    public Employee()
    //    {
    //    }

    //    public Employee(T record)
    //    {
    //        info = record;
    //    }

    //    public T Identification
    //    {
    //        get
    //        {
    //            return info;
    //        }

    //        set
    //        {
    //            info = value;
    //        }
    //    }
    //}


    ////Usage:
    //public class Program
    //{
    //    static int Main()
    //    {
    //        var std = new PersonalIdentification("bhargav Sharma");
    //        std.DateofBirth = new DateTime(1980, 12, 8);

    //        Employee<PersonalIdentification> empl = new Employee<PersonalIdentification>();
    //        empl.Identification = std;

    //        Console.WriteLine("Full Name:     {0}", empl.Identification.FullName);
    //        Console.WriteLine("Date Of birth: {0}", empl.Identification.DateofBirth.ToShortDateString());

    //        Console.WriteLine();
    //        return 0;
    //    }
    //}

}
