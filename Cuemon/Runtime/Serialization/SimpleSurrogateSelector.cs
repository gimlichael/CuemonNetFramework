using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Cuemon.Runtime.Serialization
{
    /// <summary>
    /// Provides a simple implementation of a serialization surrogate to delegate the serialization or deserialization process to.
    /// </summary>
    public class SimpleSurrogateSelector : ISurrogateSelector
    {
        private ISurrogateSelector _nextSurrogateSelector;
        private readonly IDictionary<Type, ISerializationSurrogate> _handles = new Dictionary<Type, ISerializationSurrogate>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSurrogateSelector"/> class.
        /// </summary>
        public SimpleSurrogateSelector()
        {
        }

        /// <summary>
        /// Specifies the next <see cref="T:System.Runtime.Serialization.ISurrogateSelector"/> for surrogates to examine if the current instance does not have a surrogate for the specified type and assembly in the specified context.
        /// </summary>
        /// <param name="selector">The next surrogate selector to examine.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void ChainSelector(ISurrogateSelector selector)
        {
            _nextSurrogateSelector = selector;
        }

        /// <summary>
        /// Returns the next surrogate selector in the chain.
        /// </summary>
        /// <returns>
        /// The next surrogate selector in the chain or null.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual ISurrogateSelector GetNextSelector()
        {
            return _nextSurrogateSelector;
        }

        /// <summary>
        /// Finds the surrogate that represents the specified object's type, starting with the specified surrogate selector for the specified serialization context.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type"/> of object (class) that needs a surrogate.</param>
        /// <param name="context">The source or destination context for the current serialization.</param>
        /// <param name="selector">When this method returns, contains a <see cref="T:System.Runtime.Serialization.ISurrogateSelector"/> that holds a reference to the surrogate selector where the appropriate surrogate was found. This parameter is passed uninitialized.</param>
        /// <returns>
        /// The appropriate surrogate for the given type in the given context.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (type == null) throw new ArgumentNullException("type");
            selector = null;
            if (type.IsSerializable) { return null; }
            ISerializationSurrogate surrogate;
            lock (_handles) 
            {
                if (!_handles.TryGetValue(type, out surrogate))
                {
                    surrogate = FormatterServices.GetSurrogateForCyclicalReference(new SimpleSerializationSurrogate());
                    _handles.Add(type, surrogate);
                }
            }
            selector = this;
            return surrogate;
        }
    }
}