using System.Collections.Generic;

namespace GraphWalker.Test.Fixtures.GraphWalker
{
    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public ICollection<Dog> Dogs { get; set; } = new List<Dog>();
    }
}
