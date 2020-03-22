using GraphWalker.Test.Fixtures.GraphWalker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GraphWalker.Test.Fixtures
{
    public static class GraphWalkerData
    {
        public static IEnumerable<object[]> GetPerson()
        {
            string data = File.ReadAllText("Fixtures/GraphWalker/graphwalker.json");
            yield return new object[]
            {

                JsonConvert.DeserializeObject<Person>(data)
            };
        }
    }
}
