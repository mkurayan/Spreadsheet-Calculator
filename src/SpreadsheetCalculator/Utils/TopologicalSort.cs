using System;
using System.Collections.Generic;
using SpreadsheetCalculator.Exceptions;
using System.Runtime.CompilerServices;

namespace SpreadsheetCalculator.Utils
{
    /// <summary>
    /// Use depth-first search algorithm in order to sort items.
    /// </summary>
    static class TopologicalSort
    {
        /// <summary>
        /// Sorts the specified source.
        /// </summary>
        /// <typeparam name="T">Type of sorted elements.</typeparam>
        /// <param name="source">Directed graph.</param>
        /// <param name="getDependencies">Get dependencies for each node.</param>
        /// <exception cref="CyclicDependencyException">Cyclic dependency found.</exception>
        /// <returns>Topologically sorted list.</returns>
        public static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new CyclicDependencyException("Cyclic dependency found.");
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
