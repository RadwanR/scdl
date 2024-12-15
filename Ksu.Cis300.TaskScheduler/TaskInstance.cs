using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler
{
    public class TaskInstance
    {
        /// <summary>
        /// Gets the Task for which this TaskInstance is an instance.
        /// </summary>
        public Task Task { get; }

        /// <summary>
        /// Gets the time at which this instance becomes available.
        /// </summary>
        public int Available { get; }

        /// <summary>
        /// Gets the deadline of this instance; i.e., the time it becomes available plus the task's period.
        /// </summary>
        public int Deadline => Available + Task.Period;

        /// <summary>
        /// Gets the decisions for each unit of execution time for which this task has been scheduled.
        /// </summary>
        public List<SchedulingDecision> SchedulingDecisions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskInstance"/> class.
        /// </summary>
        /// <param name="task">The Task for which this TaskInstance is an instance.</param>
        /// <param name="available">The time at which this instance becomes available.</param>
        public TaskInstance(Task task, int available)
        {
            Task = task;
            Available = available;
            SchedulingDecisions = new List<SchedulingDecision>();
        }






    }
}
