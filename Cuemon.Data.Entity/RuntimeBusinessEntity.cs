using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cuemon.Reflection;

namespace Cuemon.Data.Entity
{
    /// <summary>
    /// Infrastructure. Will resolve a runtime constructor for a <see cref="BusinessEntity"/>.
    /// </summary>
    public class RuntimeBusinessEntity
    {
        private readonly Type _entityType;
        private readonly Type[] _constructorTypes;
        private readonly object[] _constructorArguments;
        private readonly string _constructorParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBusinessEntity"/> class.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="primaryKeyValues">A sequence of primary key values.</param>
        public RuntimeBusinessEntity(Type entityType, IEnumerable<object> primaryKeyValues)
        {
            if (primaryKeyValues == null) { throw new ArgumentNullException("primaryKeyValues"); }
            IList<object> entityArgs = new List<object>(primaryKeyValues);
            string actualCtorParams = "";
            Type[] ctorTypes = new Type[entityArgs.Count];
            object[] ctorArgs = new object[ctorTypes.Length];
            for (byte i = 0; i < entityArgs.Count; i++) // traverse the primaryKeys and prepare constructor of the BusinessEntity object.
            {
                ctorTypes[i] = entityArgs[i].GetType();
                ctorArgs[i] = entityArgs[i];
                actualCtorParams += ctorTypes[i].FullName + ", ";
            }

            _entityType = entityType;
            _constructorTypes = ctorTypes;
            _constructorArguments = ctorArgs;
            _constructorParameters = actualCtorParams;
        }

        #region Properties
        private Type[] ConstructorTypes
        {
            get { return _constructorTypes; }
        }

        /// <summary>
        /// Gets an array of constructor arguments.
        /// </summary>
        public object[] ConstructorArguments
        {
            get { return _constructorArguments; }
        }

        private string ConstructorParameters
        {
            get { return _constructorParameters; }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the <see cref="BusinessEntity"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> of the <see cref="BusinessEntity"/>.
        /// </value>
        public Type EntityType
        {
            get { return _entityType; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the associated constructor for <see cref="EntityType"/>.
        /// </summary>
        /// <returns>A <see cref="ConstructorInfo"/> that can be invoked to create a new instance of the <see cref="EntityType"/>.</returns>
        public virtual ConstructorInfo GetConstructor()
        {
            ConstructorInfo[] constructors = this.EntityType.GetConstructors(ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited);
            bool hasMatchingParameters = true;
            string resolvedConstructorParams = "";
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] constructorParams = constructor.GetParameters();
                if (constructorParams.Length == this.ConstructorTypes.Length)
                {
                    for (byte i = 0; i < constructorParams.Length; i++)
                    {
                        Type paramType = constructorParams[i].ParameterType;
                        hasMatchingParameters = (paramType == this.ConstructorTypes[i]);
                        resolvedConstructorParams += paramType.FullName + ", ";
                        if (!hasMatchingParameters) { break; } // resume with next ctor
                    }

                    if (hasMatchingParameters) // we have a match - process it
                    {
                        return constructor;
                    }
                }
            }

            string actualConstructorParameters = this.ConstructorParameters;
            if (actualConstructorParameters.Length > 0) { actualConstructorParameters = actualConstructorParameters.Remove(actualConstructorParameters.LastIndexOf(", ", StringComparison.OrdinalIgnoreCase)); }
            if (resolvedConstructorParams.Length > 0) { resolvedConstructorParams = resolvedConstructorParams.Remove(resolvedConstructorParams.LastIndexOf(", ", StringComparison.OrdinalIgnoreCase)); }

            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The actual constructor parameter(s); \"{0}\", does not match the resolved constructor parameter(s); \"{1}\".",
                                                actualConstructorParameters.Length == 0 ? "<empty>" : actualConstructorParameters,
                                                resolvedConstructorParams.Length == 0 ? "<empty>" : resolvedConstructorParams));
        }
        #endregion

    }
}