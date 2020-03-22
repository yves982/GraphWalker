using System.Collections.Generic;

namespace GraphWalker.Test.Fixtures.GraphWalker
{
    public class Dog
    {
        public string Name { get; set; }

        public bool CanBark { get; set; }

        public int Height { get; set; }

        public ICollection<Location> Places { get; set; } = new List<Location>();
    }
}
