using System;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetCalculator.DirectedGraph
{
    /// <summary>
    /// Use depth-first search algorithm in order to sort items in directed graph.
    /// </summary>
    internal static class TopologicalSort
    {
        /// <summary>
        /// Sorts the specified source.
        /// </summary>
        /// <typeparam name="T">Type of sorted elements.</typeparam>
        /// <param name="source">Directed graph.</param>
        /// <param name="getDependencies">Get dependencies for each node.</param>
        /// <exception cref="CyclicDependencyException">Cyclic dependency found.</exception>
        /// <returns>Topologically sorted list.</returns>
        public static IList<string> Sort(IEnumerable<string> source, Func<string, IEnumerable<string>> getDependencies)
        {
            var sorted = new List<string>();
            var visited = new Dictionary<string, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private static void Visit(string item, Func<string, IEnumerable<string>> getDependencies, ICollection<string> sorted, IDictionary<string, bool> visited)
        {
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    var cellReffsTrackList = new List<string>();
                    cellReffsTrackList.Add(item);

                    var current = item;
                    do
                    {
                        current = getDependencies(current).First(x => visited.ContainsKey(x) && visited[x]);
                        cellReffsTrackList.Add(current);
                    }
                    while (current != item);                   

                    throw new CyclicDependencyException("Cyclic dependency found: "+ string.Join("-> ", cellReffsTrackList.ToArray()));
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
