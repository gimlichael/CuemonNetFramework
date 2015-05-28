using System;
using System.Collections.Generic;

namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Specifies the XML fragment value for the <see cref="XmlSerialization"/> class.
    /// </summary>
    public class XmlSerializationFragment
    {
        private Attribute _memberAttribute;
        private readonly Type _memberAttributeType;
        private readonly object _instance;
        private readonly object _memberValue;
        private readonly string _memberName;
        private readonly bool _hasAttributeOverriden;
        private bool _skipRootName;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationFragment"/> class.
        /// </summary>
        /// <param name="instance">The originating object instance.</param>
        /// <param name="memberName">Name of the member to be serialized.</param>
        /// <param name="memberValue">The member value to be serialized.</param>
        /// <param name="memberAttributeType">The type of the memberAttribute to support the serialization.</param>
        internal XmlSerializationFragment(object instance, string memberName, object memberValue, Type memberAttributeType)
        {
            _instance = instance;
            _memberName = memberName;
            _memberValue = memberValue ?? "null";
            _memberAttributeType = memberAttributeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationFragment"/> class.
        /// </summary>
        /// <param name="instance">The originating object instance.</param>
        /// <param name="memberName">Name of the member to be serialized.</param>
        /// <param name="memberValue">The member value to be serialized.</param>
        /// <param name="memberAttributeType">The type of the memberAttribute to support the serialization.</param>
        /// <param name="memberAttribute">The member memberAttribute to support the serialization.</param>
        internal XmlSerializationFragment(object instance, string memberName, object memberValue, Type memberAttributeType, Attribute memberAttribute)
        {
            _instance = instance;
            _memberName = memberName;
			_memberValue = memberValue ?? "null";
            _memberAttribute = memberAttribute;
            _memberAttributeType = memberAttributeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationFragment"/> class.
        /// </summary>
        /// <param name="instance">The originating object instance.</param>
        /// <param name="memberName">Name of the member to be serialized.</param>
        /// <param name="memberValue">The member value to be serialized.</param>
        /// <param name="memberAttributeType">The type of the memberAttribute to support the serialization.</param>
        /// <param name="memberAttribute">The member memberAttribute to support the serialization.</param>
        /// <param name="hasAttributeOverridden">if set to <c>true</c> this object has been marked with an attribute overridden value.</param>
        internal XmlSerializationFragment(object instance, string memberName, object memberValue, Type memberAttributeType, Attribute memberAttribute, bool hasAttributeOverridden)
        {
            _instance = instance;
            _memberName = memberName;
			_memberValue = memberValue ?? "null";
            _memberAttribute = memberAttribute;
            _memberAttributeType = memberAttributeType;
            _hasAttributeOverriden = hasAttributeOverridden;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the member memberAttribute to support the serialization.
        /// </summary>
        /// <value>The member memberAttribute for to support the serialization.</value>
        internal Attribute MemberAttribute
        {
            get { return _memberAttribute; }
            set { _memberAttribute = value; }
        }

        /// <summary>
        /// Gets the member memberAttribute type to support for the serialization.
        /// </summary>
        /// <value>The member memberAttribute type to support for the serialization.</value>
        internal Type MemberAttributeType
        {
            get { return _memberAttributeType; }
        }

        /// <summary>
        /// Gets the originating object instance.
        /// </summary>
        /// <value>The type of the object instance.</value>
        internal object Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        internal string MemberName
        {
            get { return _memberName; }
        }

        /// <summary>
        /// Gets the member value to be serialized.
        /// </summary>
        /// <value>The member value to be serialized.</value>
        internal object MemberValue
        {
            get { return _memberValue; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has its member attribute overridden.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has its member attribute overridden; otherwise, <c>false</c>.
        /// </value>
        internal bool HasAttributeOverriden
        {
            get { return _hasAttributeOverriden; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether writing of the XML root name should be bypassed.
        /// </summary>
        /// <value><c>true</c> if writing of the XML root name should be bypassed; otherwise, <c>false</c>.</value>
        internal bool SkipRootName
        {
            get { return _skipRootName; }
            set { _skipRootName = value; }
        }
        #endregion
    }
}