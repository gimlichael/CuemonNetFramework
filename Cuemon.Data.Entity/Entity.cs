using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Cuemon.Annotations;
using Cuemon.Collections.Generic;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Reflection;
using Cuemon.Xml.Serialization;
using Cuemon.Xml.XPath;

namespace Cuemon.Data.Entity
{
    /// <summary>
    /// An abstract class representing an base Entity to be used in your business logic code.
    /// </summary>
    public abstract class Entity : XmlSerialization, IFormatProvider, IXPathNavigable, INotifyPropertyChanged
    {
        private IXPathNavigable _document;
        private EntityDataAdapter _dataAdapter;
        private XPathNavigator _navigator;
        private bool _hasValue;
        private readonly object _padLock = new object();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        internal Entity()
        {
            this.IsNew = true;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when this instance has initialized.
        /// </summary>
        public event EventHandler<BusinessEntityEventArgs> Initialized;

        /// <summary>
        /// Occurs when this instance is initializing.
        /// </summary>
        public event EventHandler<BusinessEntityEventArgs> Initializing;

        /// <summary>
        /// Occurs when this instance is loading data from a data source.
        /// </summary>
        public event EventHandler<BusinessEntityEventArgs> DataLoading;

        /// <summary>
        /// Occurs when this instance has loaded data from a data source.
        /// </summary>
        public event EventHandler<BusinessEntityEventArgs> DataLoaded;

        /// <summary>
        /// Occurs when this instance is binding the loaded data into storage fields.
        /// </summary>
        public event EventHandler<BusinessEntityEventArgs> DataBinding;

        /// <summary>
        /// Occurs when this instance has bound the loaded data into storage fields.
        /// </summary>
        public event EventHandler<BusinessEntityEventArgs> DataBindingComplete;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this object has changed.
        /// </summary>
        /// <value><c>true</c> if the content has changed; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool IsDirty
        {
            get; protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool IsNew
        {
            get; protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has loaded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has loaded; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        protected bool HasLoaded
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has initialized; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        protected bool HasInitialized
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initializing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initializing; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        protected bool IsInitializing
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only (no changes can be made to the repository).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only (no changes can be made to the repository); otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has values from the underlying repository.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has values from the underlying repository; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool HasValue
        {
            get
            {
                if (!this.HasLoaded) { this.Load(); }
                return _hasValue;
            }
            internal set { _hasValue = value; }
        }

        /// <summary>
        /// Gets the <see cref="EntityDataAdapter"/> reference for this <see cref="BusinessEntity"/> instance.
        /// </summary>
        /// <value>The <see cref="EntityDataAdapter"/> reference for this <see cref="BusinessEntity"/> instance.</value>
        [XmlIgnore]
        public EntityDataAdapter DataAdapter
        {
            get { return _dataAdapter ?? (_dataAdapter = this.InitializeDataAdapter()); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the <see cref="EntityDataAdapter"/> associated with this <see cref="BusinessEntity"/>
        /// </summary>
        /// <returns>An implementation of the <see cref="EntityDataAdapter"/> object.</returns>
        protected abstract EntityDataAdapter InitializeDataAdapter();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected void Initialize()
        {
            if (this.DataAdapter == null) { throw new InvalidOperationException("No DataAdapter has been specified - please make sure that you have initialized a valid DataAdapter in the InitializeDataAdapter() method."); }
            if (this.IsInitializing) { return; }
            if (this.HasInitialized) { return; }

            if (!this.IsInitializing)
            {
                this.OnInitializing();
                if (!this.HasInitialized)
                {
                    lock (_padLock)
                    {
                        if (!this.HasInitialized)
                        {
                            try
                            {
                                this.InitializeCore();
                            }
                            finally
                            {
                                this.OnInitialized();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void InitializeCore()
        {
            DataSourceAttribute dataSourceAttribute = ReflectionUtility.GetAttribute<DataSourceAttribute>(this.GetType(), true);
            if (dataSourceAttribute != null && dataSourceAttribute.EnableRowVerification)
            {
                this.IsNew = !this.DataAdapter.Exists(this.GetType());
            }
        }

        /// <summary>
        /// Gets an object that provides formatting services for the specified type. 
        /// Default format is taken from the <see cref="CultureInfo.CurrentUICulture"/> static method.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to get.</param>
        /// <returns>
        /// The current instance, if formatType is the same type as the current instance; otherwise, null.
        /// </returns>
        public virtual object GetFormat(Type formatType)
        {
            return CultureInfo.CurrentUICulture.GetFormat(formatType);
        }

        /// <summary>
        /// Returns a new <see cref="T:System.Xml.XPath.XPathNavigator"></see> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.XPath.XPathNavigator"></see> object.
        /// </returns>
        public XPathNavigator CreateNavigator()
        {
            if (_navigator == null)
            {
                _navigator = this.Document.CreateNavigator();
            }
            return _navigator;
        }

        /// <summary>
        /// Gets or sets the IXPathNavigable object.
        /// </summary>
        /// <value>The IXPathNavigable object generated from the data adapter.</value>
        private IXPathNavigable Document
        {
            get
            {
                if (!this.HasLoaded) { this.Load(); }
                if (_document == null)
                {
                    _document = XPathUtility.CreateXPathNavigableDocument(this.ToXml());
                }
                return _document;
            }
        }

        /// <summary>
        /// Loads this entity from the data source.
        /// </summary>
        public void Load()
        {
            if (!this.HasInitialized) { this.Initialize(); }
            if (this.IsNew) { return; }
            if (this.HasLoaded) { return; }
            lock (_padLock)
            {
                this.DataAdapter.Selecting += new EventHandler<DataAdapterEventArgs>(DataAdapterSelecting);
                this.DataAdapter.Selected += new EventHandler<DataAdapterEventArgs>(DataAdapterSelected);
                this.DataAdapter.DataBinding += new EventHandler<EntityDataAdapterEventArgs>(DataAdapterPreLoading);
                this.DataAdapter.DataBindingCompleted += new EventHandler<EntityDataAdapterEventArgs>(DataAdapterPostLoaded);
                this.Load(this.GetType());
            }
        }

        /// <summary>
        /// Loads the specified <paramref name="entityType"/> from the data source.
        /// </summary>
        /// <param name="entityType">The entity type to load from the data source.</param>
        public virtual void Load(Type entityType)
        {
            this.DataAdapter.Open(entityType);
        }


        /// <summary>
        /// Deletes this entity from the data source.
        /// </summary>
        public void Delete()
        {
            if (this.IsReadOnly) { throw new NotSupportedException("This instance is in a read-only state - changes to the repository has been disabled."); }
            if (!this.HasInitialized) { this.Initialize(); }
            if (this.IsNew) { return; }
            if (!this.HasLoaded) { this.Load(); }
            lock (_padLock)
            {
                this.Delete(this.GetType());
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entityType"/> from the data source.
        /// </summary>
        /// <param name="entityType">The entity type to delete from the data source.</param>
        public virtual void Delete(Type entityType) 
        {
            this.DataAdapter.Delete(entityType);
        }

        /// <summary>
        /// Gets the reflected columns of this instance.
        /// </summary>
        /// <returns>A sequence of reflected <see cref="ColumnAttribute" /> objects for this instance.</returns>
        public IEnumerable<ColumnAttribute> GetDataMappedColumns()
        {
            return this.GetDataMappedColumns(this);
        }

        /// <summary>
        /// Gets the reflected columns of the specified <paramref name="entity"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The entity to resolve an array of reflected <see cref="ColumnAttribute" /> objects.</param>
        /// <returns>A sequence of reflected <see cref="ColumnAttribute" /> objects for the specified <paramref name="entity"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public IEnumerable<ColumnAttribute> GetDataMappedColumns<T>(T entity) where T : Entity
        {
            Validator.ThrowIfNull(entity, "entity");
            return this.GetDataMappedColumns(typeof(T));
        }

        /// <summary>
        /// Gets the reflected columns of the specified <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">The entity type to resolve an array of reflected <see cref="ColumnAttribute" /> objects.</param>
        /// <returns>A sequence of reflected <see cref="ColumnAttribute" /> objects for the specified <paramref name="entityType"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityType"/> is null.
        /// </exception>
        public virtual IEnumerable<ColumnAttribute> GetDataMappedColumns(Type entityType)
        {
            Validator.ThrowIfNull(entityType, "entityType");
            return DataMapperUtility.ParseColumns(entityType);
        }

        /// <summary>
        /// Saves this object to a data source and reloads the instance afterwards.
        /// </summary>
        public void Save()
        {
            this.SaveOnly();
            this.IsNew = false;
            this.HasLoaded = false;
        }

        /// <summary>
        /// Saves this object to a data source. 
        /// Does not reload the instance from the data source.
        /// </summary>
        public void SaveOnly()
        {
            if (this.IsReadOnly) { throw new NotSupportedException("This instance is in a read-only state - changes to the repository has been disabled."); }
            if (!this.HasInitialized) { this.Initialize(); }
            if (this.IsNew)
            {
                lock (_padLock)
                {
                    if (this.IsNew)
                    {
                        this.SaveOnly(this.GetType(), QueryType.Insert);
                        this.IsNew = false;
                    }
                }
            }
            else
            {
                if (!this.HasLoaded) { this.Load(); }
                if (this.IsDirty)
                {
                    lock (_padLock)
                    {
                        if (this.IsDirty)
                        {
                            this.SaveOnly(this.GetType(), QueryType.Update);    
                            this.IsDirty = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves this object to a data source.
        /// Does not reload the instance from the data source.
        /// </summary>
        /// <param name="entityType">The entity type to save to the data source.</param>
        /// <param name="action">The query action to perform. Can be either <see cref="QueryType.Insert"/> or <see cref="QueryType.Update"/>.</param>
        public virtual void SaveOnly(Type entityType, QueryType action)
        {
            switch (action)
            {
                case QueryType.Insert:
                    this.DataAdapter.Create(entityType);
                    this.IsNew = false;
                    break;
                case QueryType.Update:
                    this.DataAdapter.Modify(entityType);
                    this.IsDirty = false;
                    break;
            }
        }

        private void BindAutoProperties(Type entityType)
        {
            IEnumerable<ColumnAttribute> columns = this.GetDataMappedColumns(entityType);
            foreach (ColumnAttribute column in columns)
            {
                if (column.AssumeAutoProperty)
                {
                    // if AssumeAutoProperty:
                    // find a way to store initial property value from either instance or repository
                    // later compare this value with the snapshot and mark IsDirty if necessary
                }
            }
        }

        private void DataAdapterSelected(object sender, DataAdapterEventArgs e)
        {
            this.DataAdapter.Selected -= new EventHandler<DataAdapterEventArgs>(DataAdapterSelected);
            this.HasLoaded = true;
            this.OnDataLoaded();
        }

        private void DataAdapterSelecting(object sender, DataAdapterEventArgs e)
        {
            this.DataAdapter.Selecting -= new EventHandler<DataAdapterEventArgs>(DataAdapterSelecting);
            this.OnDataLoading();
        }

        private void DataAdapterPreLoading(object sender, DataAdapterEventArgs e)
        {
            this.DataAdapter.DataBinding -= new EventHandler<EntityDataAdapterEventArgs>(DataAdapterPreLoading);
            this.OnDataBinding();
        }

        private void DataAdapterPostLoaded(object sender, DataAdapterEventArgs e)
        {
            this.DataAdapter.DataBindingCompleted -= new EventHandler<EntityDataAdapterEventArgs>(DataAdapterPostLoaded);
            this.OnDataBindingComplete();
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">The name of the property being changed.</param>
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(this, new PropertyChangedEventArgs(name)); }
        }

        /// <summary>
        /// Raises the <see cref="Initializing"/> event.
        /// </summary>
        protected virtual void OnInitializing()
        {
            this.IsInitializing = true;
            EventHandler<BusinessEntityEventArgs> handler = this.Initializing;
            EventUtility.Raise(handler, this, BusinessEntityEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="Initialized"/> event.
        /// </summary>
        protected virtual void OnInitialized()
        {
            this.HasInitialized = true;
            EventHandler<BusinessEntityEventArgs> handler = this.Initialized;
            EventUtility.Raise(handler, this, BusinessEntityEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="DataLoading"/> event.
        /// </summary>
        protected virtual void OnDataLoading()
        {
            EventHandler<BusinessEntityEventArgs> handler = this.DataLoading;
            EventUtility.Raise(handler, this, BusinessEntityEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="DataLoaded"/> event.
        /// </summary>
        protected virtual void OnDataLoaded()
        {
            EventHandler<BusinessEntityEventArgs> handler = this.DataLoaded;
            EventUtility.Raise(handler, this, BusinessEntityEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="DataBinding"/> event.
        /// </summary>
        protected virtual void OnDataBinding()
        {
            EventHandler<BusinessEntityEventArgs> handler = this.DataBinding;
            EventUtility.Raise(handler, this, BusinessEntityEventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="DataBindingComplete"/> event.
        /// </summary>
        protected virtual void OnDataBindingComplete()
        {
            EventHandler<BusinessEntityEventArgs> handler = this.DataBindingComplete;
            EventUtility.Raise(handler, this, BusinessEntityEventArgs.Empty);
        }

        /// <summary>
        /// An infrastructure helper method to be used by properties, from which trivial calls will be handled.
        /// </summary>
        /// <typeparam name="T">The type of the return value of this method.</typeparam>
        /// <param name="propertyName">The property from which information is gathered from.</param>
        /// <returns>The value associated with the class property <paramref name="propertyName"/>, or <c>default(T)</c>.</returns>
        protected T GetCore<T>(string propertyName)
        {
            return this.GetCore(propertyName, default(T));
        }

        /// <summary>
        /// An infrastructure helper method to be used by properties, from which trivial calls will be handled.
        /// </summary>
        /// <typeparam name="T">The type of the return value of this method.</typeparam>
        /// <param name="propertyName">The property from which information is gathered from.</param>
        /// <param name="defaultValue">The default value of the <typeparamref name="T" />.</param>
        /// <returns>The value associated with the class property <paramref name="propertyName"/>, or <paramref name="defaultValue"/>.</returns>
        protected virtual T GetCore<T>(string propertyName, T defaultValue)
        {
            ColumnAttribute column = CheckForAttribute(this.GetProperty(propertyName));
            if (!this.HasLoaded) { this.Load(); }
            if (this.IsNew && column.IsDBGenerated) { throw new InvalidOperationException("This value is unavailable for the current state of this object."); }
            IReadOnlyCollection<PropertyInfo> properties;
            string fieldName = column.ResolveStorage();
            FieldInfo field = MappingUtility.ParseStorageField(ref fieldName, this.GetType(), out properties);
            object value = MappingUtility.ParseStorageFieldValue(this, field, properties);
            return value == null ? defaultValue : (T)value;
        }

        /// <summary>
        /// An infrastructure helper method to be used by properties, from which trivial calls will be handled.
        /// </summary>
        /// <param name="propertyName">The property from which information is gathered from.</param>
        /// <param name="newValue">The new value to be assigned to the property.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="propertyName"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="propertyName"/> is empty.
        /// </exception>
        protected virtual void SetCore(string propertyName, object newValue)
        {
            if (propertyName == null) { throw new ArgumentNullException("propertyName"); }
            if (propertyName.Length == 0) { throw new ArgumentException("This argument cannot be empty.", "propertyName"); }
            PropertyInfo property = this.GetProperty(propertyName);
            ColumnAttribute column = CheckForAttribute(property);
            if (!this.HasLoaded) { this.Load(); }

            string fieldName = column.ResolveStorage();
            IReadOnlyCollection<PropertyInfo> properties;
            FieldInfo field = MappingUtility.ParseStorageField(ref fieldName, this.GetType(), out properties);
            object oldValue = MappingUtility.ParseStorageFieldValue(this, field, properties);

            if (!this.IsNew)
            {
                newValue = this.SetCorePropertyChanged(property, oldValue, newValue);
            }

            field.SetValue(this, newValue);
        }

        private object SetCorePropertyChanged(PropertyInfo property, object oldValue, object newValue)
        {
            bool isAnyValueNull = (oldValue == null || newValue == null);
            bool isEqual = isAnyValueNull && (oldValue == newValue);
            if (!isAnyValueNull) { isEqual = newValue.Equals(oldValue); }

            if (isAnyValueNull || !isEqual) // check if we need to change the property value
            {
                string newValueAsString = newValue as string;
                if (newValueAsString != null)
                {
                    if (!string.IsNullOrEmpty(newValueAsString))
                    {
                        newValue = newValueAsString.Trim();
                    }
                }
                this.OnPropertyChanged(property.Name);
                this.IsDirty = true;
            }
            return newValue;
        }

        /// <summary>
        /// Infrastructure. Verifies and returns the specified <paramref name="property"/> for the presence of a <see cref="ColumnAttribute"/> attribute.
        /// </summary>
        /// <param name="property">The property to look for <see cref="ColumnAttribute"/>.</param>
        /// <returns>A <see cref="ColumnAttribute"/> if present; otherwise an <see cref="ArgumentException"/> is thrown.</returns>
        protected static ColumnAttribute CheckForAttribute(PropertyInfo property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }
            object[] attributes = property.GetCustomAttributes(false);
            if (attributes.Length == 0) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No attributes has been specified on this property; \"{0}\".", property.Name), "property"); }
            for (int i = 0; i < attributes.Length; i++)
            {
                Type attributeType = attributes[i].GetType();
                while (attributeType != typeof(Attribute))
                {
                    if (attributeType == typeof(ColumnAttribute))
                    {
                        return (ColumnAttribute)attributes[i];
                    }
                    attributeType = attributeType.BaseType; // get inheriting type
                }
            }
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No attribute with base in ColumnAttribute has been specified on this property; \"{0}\".", property.Name), "property");
        }

        /// <summary>
        /// Validates this instance towards any <see cref="ValidationAttribute"/> decorated properties.
        /// </summary>
        public void Validate()
        {
            this.Validate(this.GetType());
        }

        /// <summary>
        /// Validates this instance towards any <see cref="ValidationAttribute"/> decorated properties.
        /// </summary>
        /// <param name="entityType">The <see cref="BusinessEntity"/> to validate towards.</param>
        public virtual void Validate(Type entityType)
        {
            IDictionary<PropertyInfo, ValidationAttribute[]> propertiesAndAttributes = ReflectionUtility.GetPropertyAttributeDecorations<ValidationAttribute>(entityType);
            foreach (KeyValuePair<PropertyInfo, ValidationAttribute[]> propertyAndAttribute in propertiesAndAttributes)
            {
                foreach (ValidationAttribute validationAttribute in propertyAndAttribute.Value)
                {
                    ColumnAttribute[] columns = propertyAndAttribute.Key.GetCustomAttributes(typeof(ColumnAttribute), false) as ColumnAttribute[];
                    ColumnAttribute column = columns != null && columns.Length > 0 ? columns[0] : null;
                    if (column == null) { continue; }
                    PropertyInfo property;
                    switch (validationAttribute.GetType().Name)
                    {
                        case "UniqueValidationAttribute":
                            if (this.DataAdapter.ValidateUniqueIndex(entityType, column, this.GetCore<object>(propertyAndAttribute.Key.Name)))
                            {
                                throw new ValidationException(string.IsNullOrEmpty(validationAttribute.Message) ? string.Format(CultureInfo.InvariantCulture, "{0} must be unique.", propertyAndAttribute.Key.Name) : validationAttribute.Message);
                            }
                            break;
                        case "RequiredValidationAttribute":
                            property = this.GetProperty(propertyAndAttribute.Key.Name);
                            object requiredValue = property.GetValue(this, null);
                            if (requiredValue == null)
                            {
                                throw new ValidationException(string.IsNullOrEmpty(validationAttribute.Message) ? string.Format(CultureInfo.InvariantCulture, "{0} specification is required.", property.Name) : validationAttribute.Message);
                            }

                            if (TypeUtility.ContainsInterface(requiredValue, typeof(IEnumerable)))
                            {
                                if (EnumerableUtility.Count(requiredValue as IEnumerable) == 0) { throw new ValidationException(string.IsNullOrEmpty(validationAttribute.Message) ? string.Format(CultureInfo.InvariantCulture, "{0} specification is required.", propertyAndAttribute.Key.Name) : validationAttribute.Message); }
                            }
                            break;
                        case "RangeValidationAttribute":
                            RangeValidationAttribute range = validationAttribute as RangeValidationAttribute;
                            property = this.GetProperty(propertyAndAttribute.Key.Name);
                            range.Validate(this, entityType, property);
                            break;
                        case "MinLengthValidationAttribute":
                            MinLengthValidationAttribute minLength = validationAttribute as MinLengthValidationAttribute;
                            property = this.GetProperty(propertyAndAttribute.Key.Name);
                            minLength.Validate(this, entityType, property);
                            break;
                        case "MaxLengthValidationAttribute":
                            MaxLengthValidationAttribute maxLength = validationAttribute as MaxLengthValidationAttribute;
                            property = this.GetProperty(propertyAndAttribute.Key.Name);
                            maxLength.Validate(this, entityType, property);
                            break;
                        case "EmailAddressValidationAttribute":
                            EmailAddressValidationAttribute emailAddress = validationAttribute as EmailAddressValidationAttribute;
                            property = this.GetProperty(propertyAndAttribute.Key.Name);
                            emailAddress.Validate(this, entityType, property);
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// Infrastructure. Helper method for retrieving a property associated with a <see cref="BusinessEntity"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property to get.</param>
        /// <returns>A <see cref="PropertyInfo"/> object.</returns>
        protected PropertyInfo GetProperty(string propertyName)
        {
            Type currentType = this.GetType();
            while (currentType != typeof(BusinessEntity))
            {
                PropertyInfo property = currentType.GetProperty(propertyName, ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited);
                if (property != null) { return property; }
                currentType = currentType.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public override Stream ToXml(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter> writer)
        {
            if (!this.HasLoaded) { this.Load(); }
            return base.ToXml(encoding, order, omitXmlDeclaration, qualifiedRootEntity, writer);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public override Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Encoding encoding)
        {
            if (!this.HasLoaded) { this.Load(); }
            return base.ToXml(omitXmlDeclaration, qualifiedRootEntity, encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public override Stream ToXml<T1, T2, T3, T4, T5>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2, T3, T4, T5> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (!this.HasLoaded) { this.Load(); }
            return base.ToXml(encoding, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public override Stream ToXml<T1, T2, T3, T4>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2, T3, T4> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (!this.HasLoaded) { this.Load(); }
            return base.ToXml(encoding, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public override Stream ToXml<T1, T2, T3>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2, T3> writer, T1 arg1, T2 arg2, T3 arg3)
        {
            if (!this.HasLoaded) { this.Load(); }
            return base.ToXml(encoding, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public override Stream ToXml<T1, T2>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2> writer, T1 arg1, T2 arg2)
        {
            if (!this.HasLoaded) { this.Load(); }
            return base.ToXml(encoding, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public override Stream ToXml<T>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T> writer, T arg)
        {
            if (!this.HasLoaded) { this.Load(); }
            return base.ToXml(encoding, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg);
        }

        #endregion
    }
}