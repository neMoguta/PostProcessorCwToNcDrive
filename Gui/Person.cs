using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui
{
    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static Person[] GetPersons()
        {
            return new []
            {
                new Person{LastName = "Sidorov", FirstName = "Sidor"},
                new Person{LastName = "Petrov", FirstName = "Peter"},
                new Person{LastName = "Victorov",FirstName = "Victor"}
            };
        }
    }
}
