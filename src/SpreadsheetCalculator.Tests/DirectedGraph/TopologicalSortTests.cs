using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.DirectedGraph;
using Xunit;

namespace SpreadsheetCalculator.Tests.DirectedGraph
{
    public class TopologicalSortTests
    {
        [Fact]
        public void Sort_EmptyGraph_GraphRemainsEmpty()
        {
            Assert.Equal(0, TopologicalSort.Sort(new string[0], item => new string[0]).Count);
        }

        [Fact]
        public void Sort_DirectedGraphWithoutEdges_NothingToSort()
        {
            Dictionary<string, string[]> mock = new Dictionary<string, string[]>();
            mock.Add("A", new string[0]);
            mock.Add("B", new string[0]);
            mock.Add("C", new string[0]);

            var graphWithoutEdges = new[] { "A", "B", "C" };

            Assert.Equal(graphWithoutEdges, TopologicalSort.Sort(graphWithoutEdges, item => mock[item]));
        }

        [Fact]
        public void Sort_TrivialDirectedGraph_GraphSorted()
        {
            Dictionary<string, string[]> mock = new Dictionary<string, string[]>();
            mock.Add("A", new string[0]);
            mock.Add("B", new[] { "A" });
            mock.Add("C", new[] { "B" });
            mock.Add("D", new[] { "C" });

            var unsorted = new[] { "D", "C", "B", "A" };
            var expected = new[] { "A", "B", "C", "D" };

            Assert.Equal(expected, TopologicalSort.Sort(unsorted, item => mock[item]));
        }

        [Fact]
        public void Sort_DirectedGraph_GraphSorted()
        {
            Dictionary<string, string[]> mock = new Dictionary<string, string[]>();
            mock.Add("A", new string[0]);
            mock.Add("B", new[] { "C", "E" });
            mock.Add("C", new string[0]);
            mock.Add("D", new[] { "A" });
            mock.Add("E", new[] { "D", "G" });
            mock.Add("F", new string[0]);
            mock.Add("G", new[] { "F", "H" });
            mock.Add("H", new string[0]);
            

            var unsorted = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };
            var expected = new[] { "A", "C", "D", "F", "H", "G", "E", "B" };

            Assert.Equal(expected, TopologicalSort.Sort(unsorted, item => mock[item]));
        }

        [Fact]
        public void Sort_DirectedGraphWithCyclicDependency_ThrowCyclicDependencyException()
        {
            Dictionary<string, string[]> mock = new Dictionary<string, string[]>();
            mock.Add("A", new string[0]);
            mock.Add("B", new[] { "C", "E" });
            mock.Add("C", new string[0]);
            mock.Add("D", new[] { "A" });
            mock.Add("E", new[] { "D", "G" }); //Cyclic Dependency: E -> G -> F -> E
            mock.Add("F", new[] { "E" });
            mock.Add("G", new[] { "F", "H" });
            mock.Add("H", new string[0]);
            
            var unsorted = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };

            Assert.Throws<CyclicDependencyException>(() => TopologicalSort.Sort(unsorted, item => mock[item]));
        }
    }
}
