using SpreadsheetCalculator.Exceptions;
using SpreadsheetCalculator.Utils;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpreadsheetCalculator.Tests
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
        public void Sort_DirectedGraph_SortedNodes()
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
