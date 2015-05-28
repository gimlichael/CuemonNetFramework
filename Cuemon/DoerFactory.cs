using System.Reflection;
using Cuemon.Reflection;

namespace Cuemon
{
    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> function delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}"/> delegate to invoke.</param>
        public DoerFactory(Doer<TResult> method)
        {
            this.Method = method;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}"/> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}"/> delegate to invoke.</value>
        protected Doer<TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public virtual bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public virtual TResult ExecuteMethod()
        {
            return this.Method();
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method"/>.
        /// </summary>
        /// <returns>A <see cref="MethodInfo"/> describing the <see cref="Method"/> represented by the function delegate.</returns>
        public virtual MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public virtual object[] GetGenericArguments()
        {
            return new object[0];
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public virtual DoerFactory<TResult> Clone()
        {
            return new DoerFactory<TResult>(this.Method);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}"/> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T, TResult> : DoerFactory<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}"/> delegate to invoke.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T, TResult> method, T arg) : base(null)
        {
            this.Method = method;
            this.Arg1 = arg;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}"/> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}"/> delegate to invoke.</value>
        protected new Doer<T, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the first parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The first parameter of the function delegate <see cref="Method"/>.</value>
        public T Arg1 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T, TResult>(this.Method, this.Arg1);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}"/> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, TResult> : DoerFactory<T1, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}"/> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, TResult> method, T1 arg1, T2 arg2) : base(null, arg1)
        {
            this.Method = method;
            this.Arg2 = arg2;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}"/> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}"/> delegate to invoke.</value>
        protected new Doer<T1, T2, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the second parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The second parameter of the function delegate <see cref="Method"/>.</value>
        public T2 Arg2 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, TResult>(this.Method, this.Arg1, this.Arg2);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}"/> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, TResult> : DoerFactory<T1, T2, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}"/> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3) : base(null, arg1, arg2)
        {
            this.Method = method;
            this.Arg3 = arg3;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}"/> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}"/> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the third parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The third parameter of the function delegate <see cref="Method"/>.</value>
        public T3 Arg3 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}"/> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, TResult> : DoerFactory<T1, T2, T3, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}"/> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(null, arg1, arg2, arg3)
        {
            this.Method = method;
            this.Arg4 = arg4;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}"/> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}"/> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the fourth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The fourth parameter of the function delegate <see cref="Method"/>.</value>
        public T4 Arg4 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}"/> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, TResult> : DoerFactory<T1, T2, T3, T4, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}"/> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(null, arg1, arg2, arg3, arg4)
        {
            this.Method = method;
            this.Arg5 = arg5;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}"/> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}"/> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the fifth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The fifth parameter of the function delegate <see cref="Method"/>.</value>
        public T5 Arg5 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, TResult> : DoerFactory<T1, T2, T3, T4, T5, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            : base(null, arg1, arg2, arg3, arg4, arg5)
        {
            this.Method = method;
            this.Arg6 = arg6;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the sixth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The sixth parameter of the function delegate <see cref="Method"/>.</value>
        public T6 Arg6 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            : base(null, arg1, arg2, arg3, arg4, arg5, arg6)
        {
            this.Method = method;
            this.Arg7 = arg7;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the seventh parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The seventh parameter of the function delegate <see cref="Method"/>.</value>
        public T7 Arg7 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7)
        {
            this.Method = method;
            this.Arg8 = arg8;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the eighth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The eighth parameter of the function delegate <see cref="Method"/>.</value>
        public T8 Arg8 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8)
        {
            this.Method = method;
            this.Arg9 = arg9;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the ninth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The ninth parameter of the function delegate <see cref="Method"/>.</value>
        public T9 Arg9 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9)
        {
            this.Method = method;
            this.Arg10 = arg10;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the tenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The tenth parameter of the function delegate <see cref="Method"/>.</value>
        public T10 Arg10 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10)
        {
            this.Method = method;
            this.Arg11 = arg11;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the eleventh parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The eleventh parameter of the function delegate <see cref="Method"/>.</value>
        public T11 Arg11 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11)
        {
            this.Method = method;
            this.Arg12 = arg12;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the twelfth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The twelfth parameter of the function delegate <see cref="Method"/>.</value>
        public T12 Arg12 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12)
        {
            this.Method = method;
            this.Arg13 = arg13;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the thirteenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The thirteenth parameter of the function delegate <see cref="Method"/>.</value>
        public T13 Arg13 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13)
        {
            this.Method = method;
            this.Arg14 = arg14;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the fourteenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The fourteenth parameter of the function delegate <see cref="Method"/>.</value>
        public T14 Arg14 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14)
        {
            this.Method = method;
            this.Arg15 = arg15;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the fifteenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The fifteenth parameter of the function delegate <see cref="Method"/>.</value>
        public T15 Arg15 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15)
        {
            this.Method = method;
            this.Arg16 = arg16;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the sixteenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The sixteenth parameter of the function delegate <see cref="Method"/>.</value>
        public T16 Arg16 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,T17,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16)
        {
            this.Method = method;
            this.Arg17 = arg17;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the seventeenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The seventeenth parameter of the function delegate <see cref="Method"/>.</value>
        public T17 Arg17 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T18">The type of the eighteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,T17,T18,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
            : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17)
        {
            this.Method = method;
            this.Arg18 = arg18;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the eighteenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The eighteenth parameter of the function delegate <see cref="Method"/>.</value>
        public T18 Arg18 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T18">The type of the eighteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T19">The type of the nineteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,T17,T18,T19,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg19">The nineteenth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18)
        {
            this.Method = method;
            this.Arg19 = arg19;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the nineteenth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The nineteenth parameter of the function delegate <see cref="Method"/>.</value>
        public T19 Arg19 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18, this.Arg19);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18, this.Arg19 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18, this.Arg19);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T9">The type of the ninth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T18">The type of the eighteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T19">The type of the nineteenth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="T20">The type of the twentieth parameter of the function delegate <see cref="Method"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> : DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,T17,T18,T19,T20,TResult}"/> class.
        /// </summary>
        /// <param name="method">The <see cref="Doer{TResult}" /> delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg19">The nineteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg20">The twentieth parameter of the function delegate <paramref name="method"/>.</param>
        public DoerFactory(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20) : base(null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19)
        {
            this.Method = method;
            this.Arg20 = arg20;
        }

        /// <summary>
        /// Gets the <see cref="Doer{TResult}" /> delegate to invoke.
        /// </summary>
        /// <value>The <see cref="Doer{TResult}" /> delegate to invoke.</value>
        protected new Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> Method { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate to <see cref="Method"/>.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate to <see cref="Method"/>; otherwise, <c>false</c>.</value>
        public override bool HasDelegate
        {
            get { return (this.Method != null); }
        }

        /// <summary>
        /// Gets or sets the twentieth parameter of the function delegate <see cref="Method"/>.
        /// </summary>
        /// <value>The twentieth parameter of the function delegate <see cref="Method"/>.</value>
        public T20 Arg20 { get; set; }

        /// <summary>
        /// Executes the <see cref="Doer{TResult}" /> delegate associated with <see cref="Method" />.
        /// </summary>
        /// <returns>The result of the the function delegate <see cref="Method"/>.</returns>
        public override TResult ExecuteMethod()
        {
            return this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18, this.Arg19, this.Arg20);
        }

        /// <summary>
        /// Gets the method represented by the function delegate <see cref="Method" />.
        /// </summary>
        /// <returns>A <see cref="MethodInfo" /> describing the <see cref="Method" /> represented by the function delegate.</returns>
        public override MethodInfo GetMethodInfo()
        {
            return this.Method.Method;
        }

        /// <summary>
        /// Returns an array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance.
        /// </summary>
        /// <returns>An array of objects that represent the arguments passed to this <see cref="DoerFactory{TResult}"/> instance. Returns an empty array if the current instance was constructed with no generic arguments.</returns>
        public override object[] GetGenericArguments()
        {
            return new object[] { this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18, this.Arg19, this.Arg20 };
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TResult}"/> that is a copy of this instance.</returns>
        public override DoerFactory<TResult> Clone()
        {
            return new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(this.Method, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7, this.Arg8, this.Arg9, this.Arg10, this.Arg11, this.Arg12, this.Arg13, this.Arg14, this.Arg15, this.Arg16, this.Arg17, this.Arg18, this.Arg19, this.Arg20);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (this.HasDelegate)
            {
                MethodSignature signature = new MethodSignature(this.Method.Method);
                return signature.ToString();
            }
            return base.ToString();
        }
    }
}