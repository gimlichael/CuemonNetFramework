using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using Cuemon.Caching;
using Cuemon.Collections.Generic;

namespace Cuemon.Web.SessionState
{
    /// <summary>
    /// Provides access to session-state values.
    /// </summary>
    public sealed class HttpFastSessionState : IHttpSessionState
    {
        private TimeSpan _timeout = TimeSpan.FromMinutes(20);
        private HttpRequest _request;
        private HttpResponse _response;
        private static readonly object PadLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpFastSessionState"/> class.
        /// </summary>
        /// <remarks>Experimental session "replacement" that does not suffer from ASP.NET lifecycle blocking states.</remarks>
        internal HttpFastSessionState() : this(HttpContext.Current.Request, HttpContext.Current.Response)
        {
            GlobalModule.CheckForHttpContextAvailability();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpFastSessionState"/> class.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        internal HttpFastSessionState(HttpRequest request, HttpResponse response)
        {
            if (!IsCurrentRequestNotificationValid()) { return; }
            this.Request = request;
            this.Response = response;
            HttpCookie sessionCookie = request.Cookies[WebsiteUtility.CuemonSessionName];
            if (sessionCookie == null)
            {
                string sessionId = StringUtility.CreateRandomString(24, StringUtility.EnglishAlphabetCharactersMinuscule, "0123456789");
                sessionCookie = this.CreateSession(sessionId);
                this.IsNewSession = true;
            }
            this.SessionID = sessionCookie.Value;
        }

        private HttpRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }

        private HttpResponse Response
        {
            get { return _response; }
            set { _response = value; }
        }

        private HttpCookie CreateSession(string sessionId)
        {
            HttpCookie sessionCookie = new HttpCookie(WebsiteUtility.CuemonSessionName);
            sessionCookie.HttpOnly = true;
            sessionCookie.Path = "/";
            sessionCookie.Value = sessionId;
            this.Response.Cookies.Add(sessionCookie);
            return sessionCookie;
        }

        /// <summary>
        /// Gets a value indicating whether the session was created with the current request.
        /// </summary>
        /// <value></value>
        /// <returns>true if the session was created with the current request; otherwise, false.
        /// </returns>
        public bool IsNewSession
        {
            get; private set;
        }

        private static bool IsCurrentRequestNotificationValid()
        {
            //RequestNotification currentNotification = HttpContext.Current.CurrentNotification;
            //if (currentNotification == RequestNotification.BeginRequest ||
            //    currentNotification == RequestNotification.AuthenticateRequest ||
            //    currentNotification == RequestNotification.AuthorizeRequest ||
            //    currentNotification == RequestNotification.ResolveRequestCache ||
            //    currentNotification == RequestNotification.MapRequestHandler ||
            //    currentNotification == RequestNotification.AcquireRequestState)
            //{ return false; }
            return true;
        }

        /// <summary>
        /// Ends the current session.
        /// </summary>
        public void Abandon()
        {
            this.Clear();
            HttpCookie session = this.Request.Cookies[WebsiteUtility.CuemonSessionName];
            if (session != null)
            {
                CachingManager.Cache.Remove(this.SessionID, WebsiteUtility.SessionCacheGroup);
                this.SessionID = null;
                CreateSession("");
            }
        }

        /// <summary>
        /// Adds a new item to the session-state collection.
        /// </summary>
        /// <param name="name">The name of the item to add to the session-state collection.</param>
        /// <param name="value">The value of the item to add to the session-state collection.</param>
        public void Add(string name, object value)
        {
            this[name] = value;
        }

        /// <summary>
        /// Clears all values from the session-state item collection.
        /// </summary>
        public void Clear()
        {
            this.RemoveAll();
        }

        /// <summary>
        /// Gets the number of items in the session-state item collection.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of items in the session-state item collection.
        /// </returns>
        public int Count
        {
            get
            {
                IDictionary<string, object> session;
                this.TryGetSession(out session);
                return session.Count;
            }
       }

        /// <summary>
        /// Returns an enumerator that can be used to read all the session-state item values in the current session.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> that can iterate through the values in the session-state item collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            IDictionary<string, object> session;
            this.TryGetSession(out session);
            session = new Dictionary<string, object>(session);
            return session.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether the session is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the session is read-only; otherwise, false.
        /// </returns>
        bool IHttpSessionState.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a collection of the keys for all values stored in the session-state item collection.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Collections.Specialized.NameObjectCollectionBase.KeysCollection"/> that contains all the session-item keys.
        /// </returns>
        public NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                IDictionary<string, object> session;
                this.TryGetSession(out session);
                return ConvertToKeysCollection(session.Keys);
            }
        }

        private static NameObjectCollectionBase.KeysCollection ConvertToKeysCollection(IEnumerable<string> c)
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (string key in c) { nvc.Add(key, null); }
            return nvc.Keys;
        }

        /// <summary>
        /// Deletes an item from the session-state item collection.
        /// </summary>
        /// <param name="name">The name of the item to delete from the session-state item collection.</param>
        public void Remove(string name)
        {
            IDictionary<string, object> session;
            if (this.TryGetSession(out session))
            {
                session.Remove(name);
            }
        }

        /// <summary>
        /// Clears all values from the session-state item collection.
        /// </summary>
        public void RemoveAll()
        {
            IDictionary<string, object> session;
            if (this.TryGetSession(out session))
            {
                session.Clear();
            }
        }

        private bool TryGetSession(out IDictionary<string, object> session)
        {
            if (!CachingManager.Cache.TryGetValue(this.SessionID, WebsiteUtility.SessionCacheGroup, out session))
            {
                lock (PadLock)
                {
                    if (!CachingManager.Cache.TryGetValue(SessionID, WebsiteUtility.SessionCacheGroup, out session))
                    {
                        session = new Dictionary<string, object>();
                        CachingManager.Cache.Add(this.SessionID, session, WebsiteUtility.SessionCacheGroup, this.Timeout);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the unique session identifier for the session.
        /// </summary>
        /// <returns>The session ID.</returns>
        public string SessionID
        {
            get; private set;
        }

        /// <summary>
        /// Gets and sets the time-out period allowed between requests before the session-state provider terminates the session.
        /// </summary>
        /// <returns>The time-out period.</returns>
        public TimeSpan Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get
            {
                if (this.IsSessionReady)
                {
                    IDictionary<string, object> session;
                    if (this.TryGetSession(out session))
                    {
                        if (session.ContainsKey(name))
                        {
                            return session[name];
                        }
                    }
                }
                return null;
            }
            set
            {
                if (this.IsSessionReady)
                {
                    IDictionary<string, object> session;
                    this.TryGetSession(out session);
                    session[name] = value;
                }
            }
        }

        private bool IsSessionReady
        {
            get
            {
                if (this.SessionID == null) { return false; }
                if (!CachingManager.Cache.ContainsKey(this.SessionID, WebsiteUtility.SessionCacheGroup)) { return false; } // special case where the current request notification is not yet valid
                return true;
            }
        }

        #region Hidden Members
        int IHttpSessionState.CodePage
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        HttpCookieMode IHttpSessionState.CookieMode
        {
            get { throw new NotSupportedException(); }
        }

        void IHttpSessionState.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        object IHttpSessionState.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        int IHttpSessionState.LCID
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool IHttpSessionState.IsCookieless
        {
            get { throw new NotSupportedException(); }
        }

        HttpStaticObjectsCollection IHttpSessionState.StaticObjects
        {
            get { throw new NotSupportedException(); }
        }

        SessionStateMode IHttpSessionState.Mode
        {
            get { throw new NotSupportedException(); }
        }

        bool IHttpSessionState.IsSynchronized
        {
            get { throw new NotSupportedException(); }
        }

        int IHttpSessionState.Timeout
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        object IHttpSessionState.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void IHttpSessionState.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}