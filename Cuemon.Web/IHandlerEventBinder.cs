using System.Web;

namespace Cuemon.Web
{
    /// <summary>
    /// Specifies a way to attach events to methods from an <see cref="IHttpHandler"/> implemented class.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IHttpHandler"/> that this instance represents.</typeparam>
    public interface IHandlerEventBinder<out T> where T : IHttpHandler
    {
        /// <summary>
        /// Provides a way to initialize and manage the various handler specific events.
        /// </summary>
        void ManageHandlerEvents();

        /// <summary>
        /// Gets the handler that this instance represents.
        /// </summary>
        /// <value>The handler that this instance represents.</value>
        T Handler { get; }
    }
}