using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Cuemon.Diagnostics;
using Cuemon.Reflection;

namespace Cuemon.Runtime.Serialization
{
    /// <summary>
    /// Implements a serialization surrogate selector that allows one object to perform serialization of another.
    /// Most objects should be able to be serialized. Please note; experimental version!
    /// </summary>
    public sealed class SimpleSerializationSurrogate : ISerializationSurrogate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSerializationSurrogate"/> class.
        /// </summary>
        public SimpleSerializationSurrogate()
        {
        }

        /// <summary>
        /// Populates the provided <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (info == null) throw new ArgumentNullException("info");
            Dictionary<string, object> members = new Dictionary<string, object>();
            Type currentType = obj.GetType();
            while (currentType != typeof(object))
            {
                foreach (FieldInfo field in currentType.GetFields(ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited))
                {
                    if (SerializationUtility.IsTypeValidForSerializing(field.FieldType))
                    {
                        if (field.IsLiteral || field.IsInitOnly) { continue; }
                        object fieldValue = field.GetValue(obj);
                        if (fieldValue != null)
                        {
                            if (!members.ContainsKey(field.Name)) { members.Add(field.Name, fieldValue); }
                        }
                    }
                }
                currentType = currentType.BaseType;
            }
            foreach (KeyValuePair<string, object> member in members) { info.AddValue(member.Key, member.Value); }
        }

        /// <summary>
        /// Populates the object using the information in the <see cref="T:System.Runtime.Serialization.SerializationInfo"/>.
        /// </summary>
        /// <param name="obj">The object to populate.</param>
        /// <param name="info">The information to populate the object.</param>
        /// <param name="context">The source from which the object is deserialized.</param>
        /// <param name="selector">The surrogate selector where the search for a compatible surrogate begins.</param>
        /// <returns>The populated deserialized object.</returns>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (info == null) throw new ArgumentNullException("info");
            Type currentType = obj.GetType();
            while (currentType != typeof(object))
            {
                foreach (FieldInfo field in currentType.GetFields(ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited))
                {
                    if (SerializationUtility.IsTypeValidForSerializing(field.FieldType))
                    {
						if (field.IsLiteral || field.IsInitOnly) { continue; }
                        field.SetValue(obj, info.GetValue(field.Name, field.FieldType));
                    }
                }
                currentType = currentType.BaseType;
            }
            return obj;
        }
    }
}