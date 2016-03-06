using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;
using Cuemon.Runtime.Caching;

namespace Cuemon.Web.Compilation
{
    /// <summary>
    /// This utility class provides a set of methods to help discover referenced assemblies of an ASP.NET application.
    /// </summary>
    public static class CompilationUtility
    {
        /// <summary>
        /// Gets all referenced types matching the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The <see cref="Type"/> that must be present in the inheritance chain.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> containing all <paramref name="filter"/> implemented types of this ASP.NET application.</returns>
        /// <remarks><see cref="Type"/> from assemblies starting with <b>Cuemon</b>, <b>System</b> or <b>Microsoft</b> is excluded from the result.</remarks>
        public static IReadOnlyCollection<Type> GetReferencedTypes(Type filter)
        {
            return GetReferencedTypes(filter, "Cuemon", "System", "Microsoft");
        }

        /// <summary>
        /// Gets all referenced types matching the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The <see cref="Type"/> that must be present in the inheritance chain.</param>
        /// <param name="excludeAssembliesStartingWith">A sequence of assemblies to exclude from the result by matching the beginning of each string in the sequence.</param>
        /// <returns>An <see cref="IReadOnlyCollection{Type}"/> containing all <paramref name="filter"/> implemented types of this ASP.NET application.</returns>
        public static IReadOnlyCollection<Type> GetReferencedTypes(Type filter, params string[] excludeAssembliesStartingWith)
        {
            List<Type> types = new List<Type>();
            try
            {
                try
                {
                    ICollection assemblies = BuildManager.GetReferencedAssemblies();
                    foreach (Assembly assembly in assemblies)
                    {
                        if (AddReferenceType(assembly, excludeAssembliesStartingWith))
                        {
                            types.AddRange(ReflectionUtility.GetAssemblyTypes(assembly, null, filter));
                        }
                    }
                }
                catch (MissingMethodException)
                {
                    // we might end here if .NET Framework 2.0 is without SP1.
                }

                IEnumerable<Assembly> temporaryAssemblies = AppDomain.CurrentDomain.GetAssemblies(); // we need this for Temporary ASP.NET files
                foreach (Assembly assembly in temporaryAssemblies)
                {
                    if (AddReferenceType(assembly, excludeAssembliesStartingWith))
                    {
                        types.AddRange(ReflectionUtility.GetAssemblyTypes(assembly, null, filter));
                    }
                }

                types = new List<Type>(EnumerableUtility.Distinct(types));
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }
            return new ReadOnlyCollection<Type>(types);
        }

        private static bool AddReferenceType(Assembly assembly, string[] excludeAssembliesStartingWith)
        {
            if (excludeAssembliesStartingWith == null) { return true; }
            for (int i = 0; i < excludeAssembliesStartingWith.Length; i++)
            {
                if (assembly.FullName.StartsWith(excludeAssembliesStartingWith[i], StringComparison.Ordinal))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets all referenced <see cref="IHttpHandler"/> types of this ASP.NET application.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{Type}"/> containing all <see cref="IHttpHandler"/> implemented types of this ASP.NET application.</returns>
        /// <remarks><see cref="IHttpHandler"/> implementations from assemblies starting with <b>Cuemon</b>, <b>System</b> or <b>Microsoft</b> is excluded from the result.</remarks>
	    public static IReadOnlyCollection<Type> GetReferencedHandlerTypes()
        {
            Doer<Type, IReadOnlyCollection<Type>> getReferenceTypes = CachingManager.Cache.Memoize<Type, IReadOnlyCollection<Type>>(GetReferencedTypes);
            return getReferenceTypes(typeof(IHttpHandler));
        }

        /// <summary>
        /// Gets all referenced <see cref="IHttpModule"/> types of this ASP.NET application.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{Type}"/> containing all <see cref="IHttpModule"/> implemented types of this ASP.NET application.</returns>
        /// <remarks><see cref="IHttpHandler"/> implementations from assemblies starting with <b>Cuemon</b>, <b>System</b> or <b>Microsoft</b> is excluded from the result.</remarks>
        public static IReadOnlyCollection<Type> GetReferencedModuleTypes()
        {
            Doer<Type, IReadOnlyCollection<Type>> getReferenceTypes = CachingManager.Cache.Memoize<Type, IReadOnlyCollection<Type>>(GetReferencedTypes);
            return getReferenceTypes(typeof(IHttpModule));
        }
    }
}