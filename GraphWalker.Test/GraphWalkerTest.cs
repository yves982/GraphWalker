using GraphWalker.Test.Fixtures;
using GraphWalker.Test.Fixtures.GraphWalker;
using Xunit;
using Moq;
using System;

namespace GraphWalker.Test
{
    
    public class GraphWalkerTest
    {
        [Fact]
        public void WalkConsumerExceptionPropagated()
        {
            GraphWalker walker = new GraphWalker();
            Person jack = new Person();
            Mock<Action<string, object>> consumerMock = new Mock<Action<string, object>>();
            consumerMock.Setup(cons => cons.Invoke(It.IsAny<string>(), It.IsAny<object>())).Throws(new Exception("test"));

            Assert.Throws<Exception>(() => walker.Walk(jack, null, consumerMock.Object));

        }

        [Fact]
        public void WalkNullArgsNoExceptionTest()
        {
            GraphWalker walker = new GraphWalker();
            walker.Walk<Person>(null, null, null);
        }

        [Fact]
        public void WalkNullArgWithConsumerNoInvokeTest()
        {
            GraphWalker walker = new GraphWalker();
            Mock<Action<string, object>> consumerMock = new Mock<Action<string, object>>();
            consumerMock.Setup(cons => cons.Invoke(It.IsAny<string>(), It.IsAny<object>()));

            walker.Walk<Person>(null, null, consumerMock.Object);
            consumerMock.Verify(cons => cons.Invoke(It.IsAny<string>(), It.IsAny<object>()), Times.Never());
            consumerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(GraphWalkerData.GetPerson), MemberType = typeof(GraphWalkerData))]
        public void WalkOKTest(Person person)
        {
            GraphWalker walker = new GraphWalker();
            Mock<Action<string, object>> consumerMock = new Mock<Action<string, object>>();
            consumerMock.Setup(cons => cons.Invoke(It.IsAny<string>(), It.IsAny<object>()));

            walker.Walk(person, null, consumerMock.Object);

            consumerMock.Verify(cons => cons.Invoke("Name", person.Name), Times.Once());
            consumerMock.Verify(cons => cons.Invoke("Age", person.Age), Times.Once());
            int i = 0;
            foreach(var dog in person.Dogs)
            {
                consumerMock.Verify(cons => cons.Invoke($"Dogs[{i}].CanBark", dog.CanBark), Times.Once());
                consumerMock.Verify(cons => cons.Invoke($"Dogs[{i}].Name", dog.Name), Times.Once());
                consumerMock.Verify(cons => cons.Invoke($"Dogs[{i}].Height", dog.Height), Times.Once());

                int j = 0;
                foreach (var place in dog.Places)
                {
                    consumerMock.Verify(cons => cons.Invoke($"Dogs[{i}].Places[{j}].Lat", place.Lat), Times.Once());
                    consumerMock.Verify(cons => cons.Invoke($"Dogs[{i}].Places[{j}].Long", place.Long), Times.Once());
                    j++;
                }
                i++;
            }
            consumerMock.VerifyNoOtherCalls();

        }
    }
}
