using System;

namespace Cuemon
{
    /// <summary>
    /// Provide ways to create an instance of <see cref="DefaultOr{T}"/> that represents an intermediate object that to some extent implements a generic way to support the Null Object Pattern.
    /// </summary>
    public static class DefaultOr
    {
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOr{T}"/> class. 
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="instance"/> to return.</typeparam>
        /// <param name="instance">The instance to intermediate.</param>
        /// <returns>The specified <paramref name="instance"/> if <see cref="DefaultOr{T}.IsDefault"/> evaluates to <c>false</c>; otherwise a new instance of the specified <typeparamref name="T"/>, using the parameterless constructor.</returns>
        public static DefaultOr<T> Create<T>(T instance) where T : new()
        {
            return new DefaultOr<T>(instance);
        }
        #endregion
    }

    /// <summary>
    /// Represents an intermediate object that to some extent implements a generic way to support the Null Object Pattern.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Instance"/> to return.</typeparam>
    public sealed class DefaultOr<T> where T : new()
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOr{T}"/> class.
        /// </summary>
        /// <param name="instance">The instance to intermediate.</param>
        internal DefaultOr(T instance)
        {
            IsDefault = Condition.IsDefault(instance);
            Instance = IsDefault ? Activator.CreateInstance<T>() : instance;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <value>An instance of <typeparamref name="T"/>.</value>
        public T Instance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the originally supplied instance of <typeparamref name="T"/> equaled default(T).
        /// </summary>
        /// <value><c>true</c> if the originally supplied instance of <typeparamref name="T"/> equaled default(T); otherwise, <c>false</c>.</value>
        public bool IsDefault { get; private set; }
        #endregion
    }
}