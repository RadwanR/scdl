using System;
using System.Collections.Generic;
using System.Drawing;
///
namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// A static class for generating task schedules and colors.
    /// </summary>
    public static class ScheduleGenerator
    {
        private const int _backgroundRgb = 0x1aa02a;
        private const int _incrementRgb = 0x33c056;
        private const uint _maskRgb = 0xff000000;
        private const int _minSum = 383;

        /// <summary>
        /// Generates a schedule of tasks based on the given task list, processor count, and super-period.
        /// </summary>
        /// <param name="tasks">The list of tasks to schedule.</param>
        /// <param name="processorCount">The number of available processors.</param>
        /// <param name="superPeriod">The super-period for the tasks.</param>
        /// <returns>A 2D array of SchedulingDecision objects representing the schedule.</returns>
        public static SchedulingDecision?[,] GetSchedule(List<Task> tasks, int processorCount, int superPeriod)
        {
            if (processorCount <= 0 || superPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException("Processor count and super-period must be greater than 0.");
            }

            SchedulingDecision?[,] schedule = new SchedulingDecision?[superPeriod, processorCount];
            DirectedGraph<object, int> flowNetwork = BuildFlowNetwork(tasks, processorCount, superPeriod);
            FindMaximumFlow(flowNetwork);
            HashSet<SchedulingDecision>[] rawSchedule = ExtractRawSchedule(flowNetwork, superPeriod);
            BuildFinalSchedule(rawSchedule, schedule);
            return schedule;
        }

        /// <summary>
        /// Builds the flow network based on tasks, processors, and the super-period.
        /// </summary>
        private static DirectedGraph<object, int> BuildFlowNetwork(List<Task> tasks, int processorCount, int superPeriod)
        {
            DirectedGraph<object, int> graph = new();
            object source = new();
            object sink = new();
            graph.AddNode(source);
            graph.AddNode(sink);

            HashSet<object> existingNodes = new(); // Keep track of added nodes

            foreach (Task task in tasks)
            {
                foreach (TaskInstance instance in task.CreateInstances(superPeriod))
                {
                    if (!existingNodes.Contains(instance))
                    {
                        graph.AddNode(instance);
                        existingNodes.Add(instance);
                    }
                    graph.AddEdge(source, instance, task.ExecutionTime);

                    for (int time = instance.Available; time < instance.Deadline; time++)
                    {
                        if (!existingNodes.Contains(time))
                        {
                            graph.AddNode(time);
                            existingNodes.Add(time);
                        }
                        graph.AddEdge(instance, time, 1);
                    }
                }
            }

            for (int time = 0; time < superPeriod; time++)
            {
                if (!existingNodes.Contains(time))
                {
                    graph.AddNode(time);
                    existingNodes.Add(time);
                }
                graph.AddEdge(time, sink, processorCount);
            }

            return graph;
        }


        /// <summary>
        /// Finds the maximum flow using the Ford-Fulkerson algorithm.
        /// </summary>
        private static void FindMaximumFlow(DirectedGraph<object, int> residualGraph)
        {
            while (FindAugmentingPath(residualGraph, out List<object> path))
            {
                int bottleneck = FindBottleneck(residualGraph, path);
                AugmentFlow(residualGraph, path, bottleneck);
            }
        }

        /// <summary>
        /// Finds an augmenting path in the residual graph.
        /// </summary>
        private static bool FindAugmentingPath(DirectedGraph<object, int> graph, out List<object> path)
        {
            path = new List<object>();
            Dictionary<object, object> parents = new();
            Queue<object> queue = new();
            object source = graph.Nodes.First();
            queue.Enqueue(source);
            parents[source] = null;

            while (queue.Count > 0)
            {
                object current = queue.Dequeue();
                foreach (var edge in graph.OutgoingEdges(current))
                {
                    if (!parents.ContainsKey(edge.Destination) && graph[edge.Source, edge.Destination] > 0)
                    {
                        parents[edge.Destination] = current;
                        if (edge.Destination == graph.Nodes.Last())
                        {
                            path = BuildPath(parents, edge.Destination);
                            return true;
                        }
                        queue.Enqueue(edge.Destination);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Builds a path from the parent dictionary.
        /// </summary>
        private static List<object> BuildPath(Dictionary<object, object> parents, object sink)
        {
            List<object> path = new();
            for (object? current = sink; current != null; current = parents[current])
            {
                path.Insert(0, current);
            }
            return path;
        }

        /// <summary>
        /// Finds the bottleneck capacity along a path.
        /// </summary>
        private static int FindBottleneck(DirectedGraph<object, int> graph, List<object> path)
        {
            int bottleneck = int.MaxValue;
            for (int i = 0; i < path.Count - 1; i++)
            {
                bottleneck = Math.Min(bottleneck, graph[path[i], path[i + 1]]);
            }
            return bottleneck;
        }

        /// <summary>
        /// Augments the flow along a path.
        /// </summary>
        private static void AugmentFlow(DirectedGraph<object, int> graph, List<object> path, int bottleneck)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                object source = path[i];
                object dest = path[i + 1];
                graph[source, dest] -= bottleneck;

                if (graph.ContainsEdge(dest, source))
                {
                    graph[dest, source] += bottleneck;
                }
                else
                {
                    graph.AddEdge(dest, source, bottleneck);
                }
            }
        }

        /// <summary>
        /// Extracts the raw schedule from the residual graph.
        /// </summary>
        private static HashSet<SchedulingDecision>[] ExtractRawSchedule(DirectedGraph<object, int> graph, int superPeriod)
        {
            HashSet<SchedulingDecision>[] rawSchedule = new HashSet<SchedulingDecision>[superPeriod];
            for (int i = 0; i < superPeriod; i++)
            {
                rawSchedule[i] = new HashSet<SchedulingDecision>();
            }

            foreach (object node in graph.Nodes)
            {
                if (node is TaskInstance instance)
                {
                    foreach (var edge in graph.OutgoingEdges(node))
                    {
                        if (edge.Destination is int time && graph[instance, time] == 0)
                        {
                            rawSchedule[time].Add(new SchedulingDecision(instance, time, -1));
                        }
                    }
                }
            }

            return rawSchedule;
        }

        /// <summary>
        /// Builds the final schedule from the raw schedule.
        /// </summary>
        private static void BuildFinalSchedule(HashSet<SchedulingDecision>[] rawSchedule, SchedulingDecision?[,] schedule)
        {
            for (int time = 0; time < schedule.GetLength(0); time++)
            {
                int processor = 0;
                foreach (SchedulingDecision decision in rawSchedule[time])
                {
                    if (processor < schedule.GetLength(1))
                    {
                        schedule[time, processor] = decision;
                        processor++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an array of background colors for tasks.
        /// </summary>
        public static Color[] GetBackgroundColors(int numColors)
        {
            Color[] colors = new Color[numColors];
            int rgb = _backgroundRgb;

            for (int i = 0; i < numColors; i++)
            {
                uint argb = _maskRgb | (uint)rgb;
                colors[i] = Color.FromArgb((int)argb);
                rgb += _incrementRgb;
            }

            return colors;
        }

        /// <summary>
        /// Gets the foreground color for a given background color.
        /// </summary>
        public static Color GetForegroundColor(Color backgroundColor)
        {
            int sum = backgroundColor.R + backgroundColor.G + backgroundColor.B;
            return sum >= _minSum ? Color.Black : Color.White;
        }
    }
}
