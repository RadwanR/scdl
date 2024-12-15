using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler
{
    public class Task
    {
        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the execution time of the task.
        /// </summary>
        public int ExecutionTime { get; }

        /// <summary>
        /// Gets the period of the task.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Gets the priority of the task.
        /// </summary>
        public int Priority { get; } //not required



        public int Index { get; }


        /// <summary>
        /// Gets the list of task instances for this task.
        /// </summary>
        public List<TaskInstance> Instances { get; } = new List<TaskInstance>();






        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="executionTime">The execution time of the task.</param>
        /// <param name="period">The period of the task.</param>
        /// <param name="priority">The priority of the task.</param>
        public Task(string name, int executionTime, int period, int index)
        {
            Name = name;
            ExecutionTime = executionTime;
            Period = period;
            Index = index;
            Instances = new List<TaskInstance>();
        }


        /// <summary>
        /// Creates a list of task instances for this task up to the given hyper-period.
        /// </summary>
        /// <param name="hyperPeriod">The hyper-period used to determine the number of task instances.</param>
        /// <returns>A list of <see cref="TaskInstance"/> objects for this task.</returns>
        /// <summary>
        /// Creates a list of task instances for this task up to the given super-period.
        /// </summary>
        /// <param name="superPeriod">The super-period used to determine the number of task instances.</param>
        /// <returns>A list of <see cref="TaskInstance"/> objects for this task.</returns>
        public List<TaskInstance> CreateInstances(int superPeriod)
        {
            if (superPeriod <= 0 || superPeriod % Period != 0)
            {
                throw new ArgumentException("Super-period must be a positive multiple of the task period.", nameof(superPeriod));
            }

            Instances.Clear();

            for (int time = 0; time < superPeriod; time += Period)
            {
                Instances.Add(new TaskInstance(this, time));
            }

            return Instances;
        }

    }


}
