using System;
using System.Collections.Generic;
using System.Text;
namespace Cuemon.Scheduling
{
    public class SchedulerActivity
    {
        #region Constructors
        public SchedulerActivity() : this(TimeSpan.FromMinutes(2))
        {
        }

        public SchedulerActivity(TimeSpan timeToLive)
        {
            this.TimeToLive = timeToLive;
            this.Created = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the maximum amount of time this <see cref="SchedulerActivity"/> remains available.
        /// </summary>
        /// <value>The maximum amount of time this <see cref="SchedulerActivity"/> remains available.</value>
        public TimeSpan TimeToLive { get; private set; }

        /// <summary>
        /// Gets the UTC date time value from when this <see cref="SchedulerActivity"/> was created.
        /// </summary>
        /// <value>The UTC date time value from when this <see cref="SchedulerActivity"/> was created.</value>
        public DateTime Created { get; private set; }
        #endregion
    }
}