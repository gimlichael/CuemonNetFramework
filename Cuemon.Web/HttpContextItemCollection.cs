using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;
using System.Xml.Serialization;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents a collection of one or more <see cref="HttpContext"/> collection members, such as Application, Cookies, Form, QueryString, Session and/or ServerVariables.
    /// </summary>
    [XmlRoot("Items")]
    [XmlSerialization(EnableAutomatedXmlSerialization = true)]
    public sealed class HttpContextItemCollection : XmlSerialization, IEnumerable<HttpContextItem>
    {
        private IList<HttpContextItem> ContextItemsValue;
        private readonly HttpContextType[] ContextTypesValue;
        private readonly HttpContext ContextValue;
        private bool HasInitializedValue;
        private bool IsInitializingValue;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextItemCollection"/> class.
        /// </summary>
        public HttpContextItemCollection()
        {
            ContextValue = HttpContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextItemCollection"/> class.
        /// </summary>
        /// <param name="context">An instance of an <see cref="HttpContext"/> object.</param>
        public HttpContextItemCollection(HttpContext context)
        {
            ContextValue = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextItemCollection"/> class.
        /// </summary>
        /// <param name="context">An instance of an <see cref="HttpContext"/> object.</param>
        /// <param name="contextTypes">The <see cref="HttpContextType"/> values you want to include in your collection of <see cref="HttpContextItem"/> objects.</param>
        public HttpContextItemCollection(HttpContext context, params HttpContextType[] contextTypes)
        {
            ContextValue = context;
            ContextTypesValue = contextTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextItemCollection"/> class with option for filtered <see cref="HttpRequest"/> collection members.
        /// </summary>
        /// <param name="contextTypes">The <see cref="HttpContextType"/> values you want to include in your collection of <see cref="HttpContextItem"/> objects.</param>
        public HttpContextItemCollection(params HttpContextType[] contextTypes)
        {
            ContextValue = HttpContext.Current;
            ContextTypesValue = contextTypes;
        }
        #endregion

        #region Properties
        private HttpContext Context
        {
            get { return ContextValue; }
        }

        private bool IsInitializing
        {
            get { return IsInitializingValue; }
            set { IsInitializingValue = value; }
        }

        private bool HasInitialized
        {
            get { return HasInitializedValue; }
            set { HasInitializedValue = value; }
        }

        private HttpContextType[] ContextTypes
        {
            get { return ContextTypesValue; }
        }

        private IList<HttpContextItem> InnerCollection
        {
            get
            {
                if (ContextItemsValue == null)
                {
                    ContextItemsValue = new List<HttpContextItem>();
                }
                return ContextItemsValue;
            }
        }

        private void Initialize()
        {
            if (!this.HasInitialized)
            {
                if (!this.IsInitializing)
                {
                    this.IsInitializing = true;
                    lock (this.InnerCollection)
                    {
                        if (this.ContextTypes == null) // fetch all values
                        {
                            foreach (int i in Enum.GetValues(typeof (HttpContextType)))
                            {
                                this.ContextByType((HttpContextType) i);
                            }
                        }
                        else
                        {
                            foreach (HttpContextType contextType in this.ContextTypes)
                            {
                                this.ContextByType(contextType);
                            }
                        }
                    }
                    this.HasInitialized = true;
                    this.IsInitializing = false;
                }
            }
        }

        private void ContextByType(HttpContextType contextType)
        {
            HttpCookieCollection cookies = null;
            NameValueCollection items = null;
            HttpApplicationState application = null;
            HttpSessionState session = null;
            HttpFileCollection files = null;

            switch (contextType)
            {
                case HttpContextType.Application:
                    application = this.Context.Application;
                    break;
                case HttpContextType.Cookies:
                    cookies = this.Context.Request.Cookies;
                    break;
                case HttpContextType.Files:
                    files = this.Context.Request.Files;
                    break;
                case HttpContextType.Form:
                    items = this.Context.Request.Form;
                    break;
                case HttpContextType.QueryString:
                    items = this.Context.Request.QueryString;
                    break;
                case HttpContextType.ServerVariables:
                    items = this.Context.Request.ServerVariables;
                    break;
                case HttpContextType.Session:
                    session = this.Context.Session;
                    break;
            }

            if (application != null)
            {
                foreach (string name in application)
                {
                    this.InnerCollection.Add(new HttpContextItem(name, application[name], contextType));
                }
            }

            if (cookies != null)
            {
                for (int i = 0; i < cookies.Count; i++)
                {
                    this.InnerCollection.Add(new HttpContextItem(cookies[i].Name, cookies[i].Value, contextType, cookies[i].Expires));
                }
            }

            if (items != null)
            {
                foreach (string name in items)
                {
                    this.InnerCollection.Add(new HttpContextItem(name, items[name], contextType));
                }
            }

            if (files != null)
            {
                foreach (string name in files.AllKeys)
                {
                    this.InnerCollection.Add(new HttpContextItem(name, contextType, files[name].FileName, files[name].ContentType, files[name].ContentLength));
                }
            }

            if (session != null)
            {
                foreach (string name in session)
                {
                    this.InnerCollection.Add(new HttpContextItem(name, session[name], contextType));
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this object.
        /// </summary>
        /// <value>The number of elements contained in this object.</value>
        [XmlIgnore]
        public int Count
        {
            get
            {
                this.Initialize(); 
                return this.InnerCollection.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="HttpContextItem"/> at the specified index.
        /// </summary>
        /// <value>An instance of the <see cref="HttpContextItem"/> object.</value>
        public HttpContextItem this[int index]
        {
            get
            {
                this.Initialize();
                return this.InnerCollection[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="HttpContextItem"/> with the specified name.
        /// </summary>
        /// <value>An instance of the <see cref="HttpContextItem"/> object.</value>
        [XmlIgnore]
        public HttpContextItem this[string name]
        {
            get
            {
                foreach (HttpContextItem item in this)
                {
                    if (item.Name == name) { return item; }
                }
                return null;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a filtered count of the items stored in this collection.
        /// </summary>
        /// <param name="contextTypes">The <see cref="HttpContextType"/> values you want to filter your count by.</param>
        /// <returns>The filtered number of items contained in this object.</returns>
        public int GetFilteredCount(params HttpContextType[] contextTypes)
        {
            if (contextTypes == null) throw new ArgumentNullException("contextTypes");
            int count = 0;
            foreach (HttpContextItem item in this)
            {
                foreach (HttpContextType contextType in contextTypes)
                {
                    if (item.Type == contextType) { count++; }
                }
            }
            return count;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<HttpContextItem> GetEnumerator()
        {
            this.Initialize();
            return this.InnerCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}