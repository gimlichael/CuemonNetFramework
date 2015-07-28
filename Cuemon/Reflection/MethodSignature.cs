using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cuemon.Collections.Generic;

namespace Cuemon.Reflection
{
    /// <summary>
    /// Represent the signature of a method.
    /// </summary>
    public sealed class MethodSignature
    {
        private readonly bool _isProperty;
        private readonly string _className;
        private readonly string _methodName;
        private readonly IEnumerable<ParameterSignature> _parameters;
        private readonly bool _hasParameters;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSignature" /> class.
        /// </summary>
        /// <param name="method">The method to extract a signature for.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public MethodSignature(MethodBase method)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            string methodName = string.IsNullOrEmpty(method.Name) ? "NotAvailable" : method.Name;
            bool isPresumedProperty = methodName.StartsWith("get_", StringComparison.OrdinalIgnoreCase) | methodName.StartsWith("set_", StringComparison.OrdinalIgnoreCase);

            _isProperty = isPresumedProperty;
            _methodName = isPresumedProperty ? methodName.Remove(0, 4) : methodName;
            _className = method.ReflectedType == null ? "NotAvailable" : method.ReflectedType.Name;
            _parameters = ParameterSignature.Parse(method);
            _hasParameters = EnumerableUtility.Any(_parameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSignature" /> class.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <remarks>This represents a method with no parameters.</remarks>
        public MethodSignature(string className, string methodName) : this(className, methodName, new ParameterSignature[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSignature" /> class.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="isProperty">A value indicating whether the method is a property. Default is <c>false</c>.</param>
        /// <remarks>This represents a method with no parameters or a normal property.</remarks>
        public MethodSignature(string className, string methodName, bool isProperty) : this(className, methodName, isProperty, new ParameterSignature[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSignature" /> class.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">A sequence of <see cref="ParameterSignature"/> that represent the parameter signature of the method.</param>
        /// <remarks>This represents a method with one or more parameters.</remarks>
        public MethodSignature(string className, string methodName, params ParameterSignature[] parameters) : this(className, methodName, false, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSignature" /> class.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="isProperty">A value indicating whether the method is a property. Default is <c>false</c>.</param>
        /// <param name="parameters">A sequence of <see cref="ParameterSignature" /> that represent the parameter signature of the method.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="className"/> is null or <br/>
        /// <paramref name="methodName"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="className"/> is empty or <br/>
        /// <paramref name="methodName"/> is empty.
        /// </exception>
        /// <remarks>This represents a method with one or more parameters or a property indexer.</remarks>
        public MethodSignature(string className, string methodName, bool isProperty, params ParameterSignature[] parameters)
        {
            if (className == null) { throw new ArgumentNullException("className"); }
            if (className.Length == 0) { throw new ArgumentEmptyException("className"); }
            if (methodName == null) { throw new ArgumentNullException("methodName"); }
            if (methodName.Length == 0) { throw new ArgumentEmptyException("methodName"); }

            _className = className;
            _methodName = methodName;
            _isProperty = isProperty;
            _parameters = parameters ?? new ParameterSignature[0];
            _hasParameters = (parameters != null && parameters.Length > 0);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the class where the method is located.
        /// </summary>
        /// <value>The name of the class where the method is located.</value>
        public string ClassName
        {
            get { return _className; }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName
        {
            get { return _methodName; }
        }

        /// <summary>
        /// Gets the parameter of the method.
        /// </summary>
        /// <value>A sequence of type <see cref="ParameterSignature"/> containing information that matches the signature of the method.</value>
        public IEnumerable<ParameterSignature> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Gets a value indicating whether the method has parameters.
        /// </summary>
        /// <value><c>true</c> if the method has parameters; otherwise, <c>false</c>.</value>
        public bool HasParameters
        {
            get { return _hasParameters; }
        }

        /// <summary>
        /// Gets a value indicating whether the method is a property.
        /// </summary>
        /// <value><c>true</c> if the method is a property; otherwise, <c>false</c>.</value>
        public bool IsProperty
        {
            get { return _isProperty; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates and returns a default <see cref="MethodSignature"/> object ideal for debugging scenarios.
        /// </summary>
        /// <returns>A <see cref="MethodSignature"/> object ideal for debugging scenarios.</returns>
        /// <remarks>
        /// This method will use <see cref="StackFrame"/> to resolve the originating method you invoked this method from. This should only be used for debugging purposes due to the JIT.<br/>
        /// Read these articles for more information:<br/>
        /// <b>Release IS NOT Debug: 64bit Optimizations and C# Method Inlining in Release Build Call Stacks:</b> http://www.hanselman.com/blog/ReleaseISNOTDebug64bitOptimizationsAndCMethodInliningInReleaseBuildCallStacks.aspx <br/>
        /// <b>Tail call JIT conditions:</b> http://blogs.msdn.com/b/davbr/archive/2007/06/20/tail-call-jit-conditions.aspx <br/>
        /// <b>Compiler optimization:</b> http://en.wikipedia.org/wiki/Compiler_optimization
        /// </remarks>
        public static MethodSignature CreateDefault()
        {
            MethodBase method = new StackFrame(2, false).GetMethod();
            return new MethodSignature(method);
        }

        /// <summary>
        /// Creates and returns a <see cref="MethodSignature"/> object and automatically determines the type of the signature (be that method or property).
        /// </summary>
        /// <param name="method">The method to extract a signature for.</param>
        /// <returns>A <see cref="MethodSignature"/> object.</returns>
        /// <remarks>Although confusing a property is to be thought of as a method with either one or two methods (Get, Set) contained inside the property declaration.</remarks>
        public static MethodSignature Create(MethodBase method)
        {
            return new MethodSignature(method);
        }

        /// <summary>
        /// Creates and returns a <see cref="MethodSignature"/> object.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>A <see cref="MethodSignature"/> object.</returns>
        public static MethodSignature CreateMethod(string className, string methodName)
        {
            return new MethodSignature(className, methodName);
        }

        /// <summary>
        /// Creates and returns a <see cref="MethodSignature"/> object.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">A sequence of <see cref="ParameterSignature" /> that represent the parameter signature of the method.</param>
        /// <returns>A <see cref="MethodSignature"/> object.</returns>
        public static MethodSignature CreateMethod(string className, string methodName, params ParameterSignature[] parameters)
        {
            return new MethodSignature(className, methodName, parameters);
        }

        /// <summary>
        /// Creates and returns a <see cref="MethodSignature"/> object.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>A <see cref="MethodSignature"/> object with <see cref="IsProperty"/> initialized to <c>true</c>.</returns>
        /// <remarks>Although confusing a property is to be thought of as a method with either one or two methods (Get, Set) contained inside the property declaration.</remarks>
        public static MethodSignature CreateProperty(string className, string propertyName)
        {
            return new MethodSignature(className, propertyName, true);
        }

        /// <summary>
        /// Creates and returns a <see cref="MethodSignature"/> object.
        /// </summary>
        /// <param name="className">The name of the class where the method is located.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="parameters">A sequence of <see cref="ParameterSignature" /> that represent the parameter signature of the method.</param>
        /// <returns>A <see cref="MethodSignature"/> object with <see cref="IsProperty"/> initialized to <c>true</c>.</returns>
        /// <remarks>Although confusing a property is to be thought of as a method with either one or two methods (Get, Set) contained inside the property declaration.</remarks>
        public static MethodSignature CreateProperty(string className, string propertyName, params ParameterSignature[] parameters)
        {
            return new MethodSignature(className, propertyName, true, parameters);
        }

        /// <summary>
        /// Merges the <see cref="Parameters"/> signature of this instance with the specified <paramref name="runtimeParameterValues"/>.
        /// </summary>
        /// <param name="runtimeParameterValues">The runtime parameter values.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> containing the merged result of the <see cref="Parameters"/> signature of this instance and <paramref name="runtimeParameterValues"/>.</returns>
        public IDictionary<string, object> MergeParameters(params object[] runtimeParameterValues)
        {
            return MergeParameters(this.Parameters, runtimeParameterValues);
        }

        /// <summary>
        /// Merges the <paramref name="method"/> parameter signature with the specified <paramref name="runtimeParameterValues"/>.
        /// </summary>
        /// <param name="method">The method holding the parameter signature to merge with the runtime parameter values.</param>
        /// <param name="runtimeParameterValues">The runtime parameter values.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> containing the merged result of the <paramref name="method"/> parameter signature and <paramref name="runtimeParameterValues"/>.</returns>
        public static IDictionary<string, object> MergeParameters(MethodSignature method, params object[] runtimeParameterValues)
        {
            Validator.ThrowIfNull(method, "method");
            return MergeParameters(method.Parameters, runtimeParameterValues);
        }

        /// <summary>
        /// Merges the <paramref name="parameters"/> signature with the specified <paramref name="runtimeParameterValues"/>.
        /// </summary>
        /// <param name="parameters">The parameter signature to merge with the runtime parameter values.</param>
        /// <param name="runtimeParameterValues">The runtime parameter values.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> containing the merged result of the <paramref name="parameters"/> signature and <paramref name="runtimeParameterValues"/>.</returns>
        public static IDictionary<string, object> MergeParameters(IEnumerable<ParameterSignature> parameters, params object[] runtimeParameterValues)
        {
            Dictionary<string, object> wrapper = new Dictionary<string, object>();
            if (runtimeParameterValues != null)
            {
                ParameterSignature[] methodParameters = EnumerableUtility.ToArray(parameters);
                bool hasEqualNumberOfParameters = methodParameters.Length == runtimeParameterValues.Length;
                for (int i = 0; i < runtimeParameterValues.Length; i++)
                {
                    wrapper.Add(string.Format(CultureInfo.InvariantCulture, "{0}", hasEqualNumberOfParameters ? methodParameters[i].ParameterName : string.Format(CultureInfo.InvariantCulture, "arg{0}", i + 1)), runtimeParameterValues[i]);
                }
            }
            return wrapper;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the method signature.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the method signature.</returns>
        /// <remarks>
        /// The returned string has the following format: <br/>
        /// Method without parameters: [<see cref="ClassName"/>].[<see cref="MethodName"/>]()<br/>
        /// Method with at least one or more parameter: [<see cref="ClassName"/>].[<see cref="MethodName"/>]([<see cref="ParameterSignature.ParameterType"/>] [<see cref="ParameterSignature.ParameterName"/>])<br/><br/>
        /// Property: [<see cref="ClassName"/>].[<see cref="MethodName"/>]<br/>
        /// Property with at least one indexer: [<see cref="ClassName"/>].[<see cref="MethodName"/>][[<see cref="ParameterSignature.ParameterType"/>] [<see cref="ParameterSignature.ParameterName"/>]]
        /// </remarks>
        public override string ToString()
        {
            StringBuilder signature = new StringBuilder(string.Concat(this.ClassName, ".", this.MethodName));
            if (!this.IsProperty) { signature.Append("("); }
            if (EnumerableUtility.Any(this.Parameters))
            {
                if (this.IsProperty) { signature.Append("["); }
                int parameterCount = EnumerableUtility.Count(this.Parameters);
                int i = 1;
                foreach (ParameterSignature parameter in Parameters)
                {
                    signature.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}", parameter.ParameterType.Name, parameter.ParameterName);
                    if (i < parameterCount) { signature.Append(", "); }
                    i++;
                }
                if (this.IsProperty) { signature.Append("]"); }
            }
            if (!this.IsProperty) { signature.Append(")"); }
            return signature.ToString();
        }
        #endregion
    }
}
