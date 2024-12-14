using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// A static class for reading tasks from a file and writing schedules to a file.
    /// </summary>
    public static class ScheduleIO
    {
        /// <summary>
        /// The maximum allowable length for a task name.
        /// </summary>
        public const int MaxNameLength = 15;

        /// <summary>
        /// The maximum allowable task period or super-period.
        /// </summary>
        private const int _maxPeriod = 200;

        /// <summary>
        /// The minimum allowable number of processors.
        /// </summary>
        private const int _minProcessors = 1;

        /// <summary>
        /// The maximum allowable number of processors.
        /// </summary>
        private const int _maxProcessors = 5;

        /// <summary>
        /// The maximum allowable number of tasks.
        /// </summary>
        private const int _maxTasks = 60;

        /// <summary>
        /// The number of fields in all input lines after the first.
        /// </summary>
        private const int _fieldsPerLine = 3;

        /// <summary>
        /// The index of the task name field.
        /// </summary>
        private const int _nameField = 0;

        /// <summary>
        /// The index of the execution time field.
        /// </summary>
        private const int _executionTimeField = 1;

        /// <summary>
        /// The index of the period field.
        /// </summary>
        private const int _periodField = 2;

        /// <summary>
        /// The separator character for fields in files.
        /// </summary>
        private const char _separator = ',';

        /// <summary>
        /// Reads the tasks from the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the file to read.</param>
        /// <param name="processorCount">The number of processors (output parameter).</param>
        /// <param name="superPeriod">The super-period (output parameter).</param>
        /// <returns>A list of tasks read from the file.</returns>
        /// <exception cref="IOException">Thrown if the file is invalid or violates the input constraints.</exception>
        public static List<Task> ReadTasks(string filePath, out int processorCount, out int superPeriod)
        {
            List<Task> tasks = new List<Task>();
            superPeriod = 1;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string? firstLine = reader.ReadLine();
                if (firstLine == null)
                {
                    throw new IOException("The number of processors is not given.");
                }

                if (!int.TryParse(firstLine, out processorCount) || processorCount < _minProcessors || processorCount > _maxProcessors)
                {
                    throw new IOException("The number of processors must be at least 1 and at most 5.");
                }

                int lineNumber = 1;
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    string? line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    string[] fields = line.Split(_separator);
                    if (fields.Length != _fieldsPerLine)
                    {
                        throw new IOException($"Line {lineNumber} has {fields.Length} fields.");
                    }

                    string name = fields[_nameField];
                    if (name.Length > MaxNameLength)
                    {
                        throw new IOException($"The task name in line {lineNumber} is too long.");
                    }

                    if (!int.TryParse(fields[_executionTimeField], out int executionTime) || executionTime < 1)
                    {
                        throw new IOException($"Line {lineNumber} has an execution time of {executionTime}.");
                    }

                    if (!int.TryParse(fields[_periodField], out int period) || period > _maxPeriod)
                    {
                        throw new IOException($"Line {lineNumber} has a period of {period}.");
                    }

                    if (executionTime > period)
                    {
                        throw new IOException($"Line {lineNumber} has an execution time larger than its period.");
                    }

                    tasks.Add(new Task(name, executionTime, period, tasks.Count));

                    int updatedSuperPeriod = Lcm(superPeriod, period);
                    if (updatedSuperPeriod > _maxPeriod)
                    {
                        throw new IOException("The super-period is greater than 200.");
                    }

                    superPeriod = updatedSuperPeriod;
                }
            }

            if (tasks.Count > _maxTasks)
            {
                throw new IOException("The number of tasks is greater than 60.");
            }

            if (!IsFeasible(tasks, processorCount, superPeriod))
            {
                throw new IOException("The task set is infeasible.");
            }

            foreach (Task task in tasks)
            {
                task.CreateInstances(superPeriod);
            }

            return tasks;
        }

        /// <summary>
        /// Writes the schedule to the specified file.
        /// </summary>
        /// <param name="schedule">The schedule to write.</param>
        /// <param name="filePath">The full path to the file to write.</param>
        public static void WriteSchedule(SchedulingDecision?[,] schedule, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                int rows = schedule.GetLength(0);
                int columns = schedule.GetLength(1);

                writer.Write("Time");
                for (int i = 1; i <= columns; i++)
                {
                    writer.Write($",Processor {i}");
                }
                writer.WriteLine();

                for (int i = 0; i < rows; i++)
                {
                    writer.Write(i);
                    for (int j = 0; j < columns; j++)
                    {
                        SchedulingDecision? decision = schedule[i, j];
                        writer.Write(",");
                        if (decision != null)
                        {
                            writer.Write(decision.TaskInstance.Task.Name);
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Computes the least common multiple (LCM) of two integers.
        /// </summary>
        /// <param name="a">The first integer.</param>
        /// <param name="b">The second integer.</param>
        /// <returns>The LCM of the two integers.</returns>
        private static int Lcm(int a, int b)
        {
            return (a / Gcd(a, b)) * b;
        }

        /// <summary>
        /// Computes the greatest common divisor (GCD) of two integers.
        /// </summary>
        /// <param name="a">The first integer.</param>
        /// <param name="b">The second integer.</param>
        /// <returns>The GCD of the two integers.</returns>
        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        /// <summary>
        /// Determines whether the task set is feasible.
        /// </summary>
        /// <param name="tasks">The list of tasks.</param>
        /// <param name="processorCount">The number of processors.</param>
        /// <param name="superPeriod">The super-period.</param>
        /// <returns>True if the task set is feasible; otherwise, false.</returns>
        private static bool IsFeasible(List<Task> tasks, int processorCount, int superPeriod)
        {
            int totalRequiredTime = 0;
            foreach (Task task in tasks)
            {
                totalRequiredTime += (superPeriod / task.Period) * task.ExecutionTime;
            }
            return totalRequiredTime <= processorCount * superPeriod;
        }
    }
}
