using System;

namespace Cuemon
{
    /// <summary>
    /// Provides generic data for an event.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the event data.</typeparam>
    public class EventArgs<T1> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1}"/> class.
        /// </summary>
        /// <param name="arg1">The value of the first parameter of the event data.</param>
        public EventArgs(T1 arg1)
        {
            this.Arg1 = arg1;
        }

        /// <summary>
        /// Gets the first parameter of the event data.
        /// </summary>
        /// <value>The first parameter of the event data.</value>
        public T1 Arg1 { get; private set; }
    }

    /// <summary>
    /// Provides generic data for an event.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the event data.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the event data.</typeparam>
    public class EventArgs<T1, T2> : EventArgs<T1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1, T2}"/> class.
        /// </summary>
        /// <param name="arg1">The value of the first parameter of the event data.</param>
        /// <param name="arg2">The value of the second parameter of the event data.</param>
        public EventArgs(T1 arg1, T2 arg2) : base(arg1)
        {
            this.Arg2 = arg2;
        }

        /// <summary>
        /// Gets the second parameter of the event data.
        /// </summary>
        /// <value>The second parameter of the event data.</value>
        public T2 Arg2 { get; private set; }
    }

    /// <summary>
    /// Provides generic data for an event.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the event data.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the event data.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the event data.</typeparam>
    public class EventArgs<T1, T2, T3> : EventArgs<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="arg1">The value of the first parameter of the event data.</param>
        /// <param name="arg2">The value of the second parameter of the event data.</param>
        /// <param name="arg3">The value of the third parameter of the event data.</param>
        public EventArgs(T1 arg1, T2 arg2, T3 arg3) : base(arg1, arg2)
        {
            this.Arg3 = arg3;
        }

        /// <summary>
        /// Gets the third parameter of the event data.
        /// </summary>
        /// <value>The third parameter of the event data.</value>
        public T3 Arg3 { get; private set; }
    }

    /// <summary>
    /// Provides generic data for an event.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the event data.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the event data.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the event data.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the event data.</typeparam>
    public class EventArgs<T1, T2, T3, T4> : EventArgs<T1, T2, T3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1, T2, T3, T4}"/> class.
        /// </summary>
        /// <param name="arg1">The value of the first parameter of the event data.</param>
        /// <param name="arg2">The value of the second parameter of the event data.</param>
        /// <param name="arg3">The value of the third parameter of the event data.</param>
        /// <param name="arg4">The value of the fourth parameter of the event data.</param>
        public EventArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(arg1, arg2, arg3)
        {
            this.Arg4 = arg4;
        }

        /// <summary>
        /// Gets the fourth parameter of the event data.
        /// </summary>
        /// <value>The fourth parameter of the event data.</value>
        public T4 Arg4 { get; private set; }
    }

    /// <summary>
    /// Provides generic data for an event.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the event data.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the event data.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the event data.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the event data.</typeparam>
    /// <typeparam name="T5">The type of the fith parameter of the event data.</typeparam>
    public class EventArgs<T1, T2, T3, T4, T5> : EventArgs<T1, T2, T3, T4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1, T2, T3, T4, T5}"/> class.
        /// </summary>
        /// <param name="arg1">The value of the first parameter of the event data.</param>
        /// <param name="arg2">The value of the second parameter of the event data.</param>
        /// <param name="arg3">The value of the third parameter of the event data.</param>
        /// <param name="arg4">The value of the fourth parameter of the event data.</param>
        /// <param name="arg5">The value of the fith parameter of the event data.</param>
        public EventArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(arg1, arg2, arg3, arg4)
        {
            this.Arg5 = arg5;
        }

        /// <summary>
        /// Gets the fith parameter of the event data.
        /// </summary>
        /// <value>The fith parameter of the event data.</value>
        public T5 Arg5 { get; private set; }
    }
}
