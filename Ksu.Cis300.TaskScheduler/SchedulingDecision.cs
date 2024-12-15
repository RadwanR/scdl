using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler
{
    public class SchedulingDecision
    {
        /// <summary>
        /// Gets the TaskInstance for which this scheduling decision was made.
        /// </summary>
        public TaskInstance TaskInstance { get; }

        /// <summary>
        /// Gets the time at which this task instance starts executing.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Gets the processor on which this task instance is scheduled.
        /// </summary>
        public int Processor { get; }
      public int ExecutionTimeCompleted { get; }

        /// <summary>
        /// Gets the next scheduling decision for the task instance, if there is one.
        /// Otherwise, returns null.
        /// </summary>
        public SchedulingDecision? NextDecision
        {
            get
            {
                // Get the index of the current scheduling decision in the task instance's list.
                int nextIndex = ExecutionTimeCompleted + 1;

                // Check if the next index is within bounds of the SchedulingDecisions list.
                if (nextIndex < TaskInstance.SchedulingDecisions.Count)
                {
                    // Return the next decision if within bounds.
                    return TaskInstance.SchedulingDecisions[nextIndex];
                }

                // Return null if there is no next decision.
                return null;
            }
        }





        //nextdesition {get; }

        

        /// <summary>
        /// Constructs a new SchedulingDecision.
        /// </summary>
        /// <param name="taskInstance">The TaskInstance for which this decision is made.</param>
        /// <param name="time">The time at which the TaskInstance is scheduled.</param>
        /// <param name="executionTimeCompleted">The execution time completed before this decision.</param>
        public SchedulingDecision(TaskInstance taskInstance, int time, int executionTimeCompleted)
        {
            TaskInstance = taskInstance ?? throw new ArgumentNullException(nameof(taskInstance));
            Time = time;
            ExecutionTimeCompleted = executionTimeCompleted;
        }


    }

}
