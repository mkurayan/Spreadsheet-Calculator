using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.DirectedGraph;
using Xunit;

namespace SpreadsheetCalculator.Tests.DirectedGraph
{
    public class TopologicalSortTests
    {
        class Node
        {
            public string Name { get; private set; }
            public List<Node> Dependencies { get; private set; }

            public Node(string name, params Node[] dependencies)
            {
                Name = name;
                Dependencies = dependencies.ToList();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Node))
                    return false;

                return Equals((Node)obj);
            }

            public bool Equals(Node other)
            {
                return Name == other.Name;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }

        [Fact]
        public void Sort_EmptyGrapgh_GrapghRemainsEmpty()
        {
            var emptyGrapgh = new Node[0];

            Assert.Equal(0, TopologicalSort.Sort(emptyGrapgh, item => item.Dependencies).Count);
        }

        [Fact]
        public void Sort_DirectedGraphWithoutEdges_NothingToSort()
        {
            var a = new Node("A");
            var b = new Node("B");
            var c = new Node("C");

            var graphWithoutEdges = new[] { a, b, c };

            Assert.Equal(graphWithoutEdges, TopologicalSort.Sort(graphWithoutEdges, item => item.Dependencies));
        }

        [Fact]
        public void Sort_TrivialDirectedGraph_GrapthSorted()
        {
            var a = new Node("A");
            var b = new Node("B", a);
            var c = new Node("C", b);
            var d = new Node("D", c);

            var unsorted = new[] { d, c, b, a };
            var expected = new[] { a, b, c, d };

            Assert.Equal(expected, TopologicalSort.Sort(unsorted, item => item.Dependencies));
        }

        [Fact]
        public void Sort_DirectedGraph_GrapthSorted()
        {
            var a = new Node("A");
            var c = new Node("C");
            var d = new Node("D", a);
            var f = new Node("F");
            var h = new Node("H");
            var g = new Node("G", f, h);
            var e = new Node("E", d, g);
            var b = new Node("B", c, e);

            var unsorted = new[] { a, b, c, d, e, f, g, h };
            var expected = new[] { a, c, d, f, h, g, e, b };


            Assert.Equal(expected, TopologicalSort.Sort(unsorted, item => item.Dependencies));
        }

        [Fact]
        public void Sort_DirectedGraphWithCyclicDependency_ThrowCyclicDependencyException()
        {
            var a = new Node("A");
            var c = new Node("C");
            var h = new Node("H");
            var d = new Node("D", a);
            var e = new Node("E", d);
            var b = new Node("B", c, e);
            var f = new Node("F", e);
            var g = new Node("G", f, h);

            // Add Cyclic Dependency
            e.Dependencies.Add(g);

            var unsorted = new[] { a, b, c, d, e, f, g, h };

            Assert.Throws<CyclicDependencyException>(() => TopologicalSort.Sort(unsorted, item => item.Dependencies));
        }
    }
}
