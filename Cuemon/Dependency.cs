using System;
using Cuemon.Diagnostics;
namespace Cuemon
{
    /// <summary>
    /// An abstract class for establishing various methods of a dependency relationship to an object. 
    /// The implementing class of the <see cref="Dependency"/> class must monitor the dependency relationships so that when any of them changes, action will automatically be taken.
    /// </summary>
    public abstract class Dependency : IDependency
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Dependency"/> class.
        /// </summary>
        protected Dependency()
        {
            this.UtcLastModified = DateTime.UtcNow;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a <see cref="Dependency"/> has changed.
        /// </summary>
        public event EventHandler<DependencyEventArgs> DependencyChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets time when the dependency was last changed.
        /// </summary>
        /// <value>The time when the dependency was last changed.</value>
        /// <remarks>This property is measured in Coordinated Universal Time (UTC) (also known as Greenwich Mean Time).</remarks>
        public DateTime UtcLastModified { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Dependency"/> object has changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="Dependency"/> object has changed; otherwise, <c>false</c>.
        /// </value>
        public abstract bool HasChanged { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Marks the time when a dependency last changed.
        /// </summary>
        /// <param name="utcLastModified">The time when the dependency last changed.</param>
        protected void SetUtcLastModified(DateTime utcLastModified)
        {
            if (utcLastModified.Kind != DateTimeKind.Utc) { throw new ArgumentException("The time from when the dependency was last changed, must be specified in  Coordinated Universal Time (UTC).", "utcLastModified"); }
            this.UtcLastModified = utcLastModified;
        }

        /// <summary>
        /// Raises the <see cref="DependencyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDependencyChangedRaised(DependencyEventArgs e)
        {
            EventHandler<DependencyEventArgs> handler = this.DependencyChanged;
            EventUtility.Raise(handler, this, e);
        }

        /// <summary>
        /// Starts and performs the necessary dependency tasks of this instance.
        /// </summary>
        public abstract void Start();
        #endregion
    }
}