using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Cuemon.Collections.Generic;
using Cuemon.Net.Http;
using Cuemon.Reflection;
using Cuemon.Runtime.Caching;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// This utility class is designed to make <see cref="HttpRoute"/> operations easier to work with.
    /// </summary>
    public static class HttpRouteUtility
    {
        /// <summary>
        /// Parses the specified URI pattern for any route arguments (eg. {controller}/{model}?value={value}).
        /// </summary>
        /// <param name="uriPattern">The URI pattern to parse.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all matched route arguments.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="uriPattern"/> is null.
        /// </exception>
        public static IEnumerable<string> ParseRoute(string uriPattern)
        {
            if (uriPattern == null) { throw new ArgumentNullException(nameof(uriPattern)); }

            Regex rx = new Regex(@"{[\w]*?[^\d][\w]*?}");
            MatchCollection matches = rx.Matches(uriPattern);
            foreach (Match match in matches)
            {
                yield return match.Value.Remove(match.Value.Length - 1, 1).Remove(0, 1);
            }
        }

        /// <summary>
        /// Parses the specified <paramref name="routeHandler"/> for methods decorated with the <see cref="HttpRouteAttribute"/>.
        /// </summary>
        /// <param name="routeHandler">The handler to parse for methods decorated with the <see cref="HttpRouteAttribute"/>.</param>
        /// <returns>A sequence of <see cref="KeyValuePair{HttpRouteAttribute,MethodInfo}"/> structs.</returns>
        public static IEnumerable<KeyValuePair<HttpRouteAttribute, MethodInfo>> ParseRouteMethods(IHttpHandler routeHandler)
        {
            Validator.ThrowIfNull(routeHandler, nameof(routeHandler));
            Type routeHandlerType = routeHandler.GetType();
            return CachingManager.Cache.GetOrAdd(routeHandlerType.FullName, () =>
            {
                List<KeyValuePair<HttpRouteAttribute, MethodInfo>> httpMethods = new List<KeyValuePair<HttpRouteAttribute, MethodInfo>>();
                IEnumerable<MethodInfo> methods = ReflectionUtility.GetMethods(routeHandlerType, ReflectionUtility.BindingInstancePublic);
                foreach (MethodInfo method in methods)
                {
                    HttpRouteAttribute attribute = ReflectionUtility.GetAttribute<HttpRouteAttribute>(method);
                    if (attribute == null) { continue; }
                    httpMethods.Add(new KeyValuePair<HttpRouteAttribute, MethodInfo>(attribute, method));
                }
                return httpMethods;
            });
        }


        /// <summary>
        /// Parses the specified <paramref name="handler"/> for a matching <see cref="MethodInfo"/> that can be invoked with the result of <paramref name="parameters"/>.
        /// </summary>
        /// <param name="handler">The handler to parse for a matching <see cref="MethodInfo"/>.</param>
        /// <param name="requestUri">The URI of the current request.</param>
        /// <param name="currentVerb">The HTTP verb of the current request.</param>
        /// <param name="parameters">The parameter values of the resolved <see cref="MethodInfo"/>.</param>
        /// <returns>A matching <see cref="MethodInfo"/> that can be invoked with the result of <paramref name="parameters"/>.</returns>
        public static MethodInfo ParseRouteMethod(IHttpHandler handler, Uri requestUri, HttpMethods currentVerb, out object[] parameters)
        {
            return ParseRouteMethod(handler, HttpRequestUtility.GetHostAuthority(requestUri), requestUri, currentVerb, out parameters);
        }

        /// <summary>
        /// Parses the specified <paramref name="handler"/> for a matching <see cref="MethodInfo"/> that can be invoked with the result of <paramref name="parameters"/>.
        /// </summary>
        /// <param name="handler">The handler to parse for a matching <see cref="MethodInfo"/>.</param>
        /// <param name="handlerUri">The URI associated with <paramref name="handler"/>.</param>
        /// <param name="requestUri">The URI of the current request.</param>
        /// <param name="currentVerb">The HTTP verb of the current request.</param>
        /// <param name="parameters">The parameter values of the resolved <see cref="MethodInfo"/>.</param>
        /// <returns>A matching <see cref="MethodInfo"/> that can be invoked with the result of <paramref name="parameters"/>.</returns>
        public static MethodInfo ParseRouteMethod(IHttpHandler handler, Uri handlerUri, Uri requestUri, HttpMethods currentVerb, out object[] parameters)
        {
            try
            {
                IDictionary<MethodInfo, object[]> candidates = new Dictionary<MethodInfo, object[]>();
                foreach (KeyValuePair<HttpRouteAttribute, MethodInfo> httpMethod in ParseRouteMethods(handler))
                {
                    HttpRouteAttribute routeAttribute = httpMethod.Key;
                    if (!EnumUtility.HasFlag(routeAttribute.Methods, currentVerb)) { continue; }
                    Uri attributeUri = new Uri(handlerUri, routeAttribute.UriPattern);
                    if (!IsRelated(requestUri, attributeUri, httpMethod.Value)) { continue; }

                    List<object> result = new List<object>();
                    string[] requestSegments = StringUtility.Remove(requestUri.Segments, "/");
                    string[] attributeSegments = StringUtility.Remove(UrlDecode(attributeUri.Segments), "/", "{", "}");
                    NameValueCollection requestQuerystring = HttpRequestUtility.ParseFieldValuePairs(requestUri.Query);
                    NameValueCollection attributeQuerystring = HttpRequestUtility.ParseFieldValuePairs(attributeUri.Query);

                    List<string> requestParameters = new List<string>();
                    List<string> attributeParameters = new List<string>(attributeSegments);
                    List<string> compoundRequestParameters = new List<string>();
                    List<string> compoundAttributeParameters = new List<string>();

                    if (requestSegments.Length >= attributeSegments.Length)
                    {
                        for (int i = 0; i < requestSegments.Length; i++)
                        {
                            if (i < attributeSegments.Length && requestSegments[i].Equals(attributeSegments[i], StringComparison.OrdinalIgnoreCase))
                            {
                                attributeParameters.RemoveAt(0);
                                continue;
                            }
                            requestParameters.Add(requestSegments[i]);
                        }
                    }

                    foreach (string requestParameter in requestParameters)
                    {
                        compoundRequestParameters.AddRange(ParseParameter(requestParameter, routeAttribute.CompoundPathSegments));
                    }

                    foreach (string attributeParameter in attributeParameters)
                    {
                        compoundAttributeParameters.AddRange(ParseParameter(attributeParameter, routeAttribute.CompoundPathSegments));
                    }

                    if ((compoundRequestParameters.Count != compoundAttributeParameters.Count) || (requestQuerystring.Keys.Count < attributeQuerystring.Keys.Count)) { continue; }

                    List<ParameterInfo> httpMethodParameters = new List<ParameterInfo>(httpMethod.Value.GetParameters());

                    for (int i = 0; i < compoundAttributeParameters.Count; i++)
                    {
                        ParameterInfo parameter = GetParameterInfo(httpMethodParameters, compoundAttributeParameters[i]);
                        if (parameter == null) { continue; }
                        if (IsRouteHexString(routeAttribute, parameter.Name))
                        {
                            result.Add(StringConverter.FromHexadecimal(compoundRequestParameters[i], PreambleSequence.Remove, Encoding.UTF8)); // todo: support dynamic encoding from request
                        }
                        else
                        {
                            result.Add(ObjectConverter.ChangeType(compoundRequestParameters[i], parameter.ParameterType));
                        }
                    }

                    int expectedQuerystringParameters = 0;
                    foreach (string key in attributeQuerystring.Keys)
                    {
                        if (requestQuerystring[key] == null) { continue; }
                        if (!attributeQuerystring[key].Equals(requestQuerystring[key], StringComparison.OrdinalIgnoreCase))
                        {
                            string attributeValue = StringUtility.Remove(HttpUtility.UrlDecode(attributeQuerystring[key]), "/", "{", "}");
                            ParameterInfo parameter = GetParameterInfo(httpMethodParameters, attributeValue);
                            if (parameter == null) { continue; }
                            if (IsRouteHexString(routeAttribute, parameter.Name))
                            {
                                result.Add(StringConverter.FromHexadecimal(requestQuerystring[key], PreambleSequence.Remove, Encoding.UTF8)); // todo: support dynamic encoding from request
                            }
                            else
                            {
                                result.Add(ObjectConverter.ChangeType(requestQuerystring[key], parameter.ParameterType));
                            }
                            expectedQuerystringParameters++;
                        }
                    }

                    if (expectedQuerystringParameters == attributeQuerystring.Keys.Count) { candidates.Add(httpMethod.Value, result.ToArray()); }
                }

                if (candidates.Count == 0)
                {
                    throw ExceptionUtility.Refine(new HttpRouteNotFoundException(requestUri, currentVerb), MethodBase.GetCurrentMethod());
                }

                List<KeyValuePair<MethodInfo, object[]>> candidatesList = new List<KeyValuePair<MethodInfo, object[]>>(candidates);
                candidatesList.Sort(ComparisonOfValueSize);
                KeyValuePair<MethodInfo, object[]> mostLikelyCandidate = EnumerableUtility.LastOrDefault(candidatesList);
                parameters = mostLikelyCandidate.Value;
                return mostLikelyCandidate.Key;
            }
            catch (Exception ex)
            {
                if (ex is HttpRouteNotFoundException) { throw; }
                throw ExceptionUtility.Refine(new HttpRouteNotFoundException(requestUri, currentVerb, ex), MethodBase.GetCurrentMethod());
            }
        }

        private static int ComparisonOfValueSize(KeyValuePair<MethodInfo, object[]> firstPair, KeyValuePair<MethodInfo, object[]> nextPair)
        {
            return firstPair.Value.Length.CompareTo(nextPair.Value.Length);
        }

        private static ParameterInfo GetParameterInfo(IEnumerable<ParameterInfo> parameters, string parameterName)
        {
            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.Name.ToUpperInvariant() == parameterName.ToUpperInvariant()) { return parameter; }
            }
            return null;
        }

        private static bool IsRouteHexString(HttpRouteAttribute route, string parameterName)
        {
            if (route.RouteArgumentsAsHexString == null) { return false; }
            foreach (string hexParameter in route.RouteArgumentsAsHexString)
            {
                if (hexParameter.Equals(string.Concat("{", parameterName, "}"), StringComparison.OrdinalIgnoreCase)) { return true; }
            }
            return false;
        }

        private static IEnumerable<string> ParseParameter(string parameter, IEnumerable<char> compoundPathSegments)
        {
            bool yielded = false;
            foreach (char compoundPathSegment in compoundPathSegments)
            {
                string[] compoundSegments = parameter.Split(compoundPathSegment);
                if (compoundSegments.Length == 1) { continue; }

                yielded = true;
                foreach (string compoundSegment in compoundSegments)
                {
                    yield return compoundSegment;
                }
            }

            if (!yielded) { yield return parameter; }
        }

        private static string[] UrlDecode(IEnumerable<string> segments)
        {
            List<string> result = new List<string>();
            foreach (string encodedSegment in segments)
            {
                result.Add(HttpUtility.UrlDecode(encodedSegment));
            }
            return result.ToArray();
        }

        private static bool IsRelated(Uri request, Uri attribute, MethodInfo httpMethod)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            if (attribute == null) { throw new ArgumentNullException(nameof(attribute)); }

            if (attribute.Segments.Length != request.Segments.Length) { return false; }

            NameValueCollection attributeQuerystring = HttpRequestUtility.ParseFieldValuePairs(attribute.Query);
            NameValueCollection requestQuerystring = HttpRequestUtility.ParseFieldValuePairs(request.Query);

            if (attributeQuerystring.Count > requestQuerystring.Count) { return false; }
            foreach (string key in attributeQuerystring.Keys)
            {
                if (requestQuerystring[key] == null) { return false; }
            }

            if (!CanRequestImpersonateAttribute(request.Segments, attribute.Segments)) { return false; }

            int maxLength = NumberUtility.GetLowestValue(attribute.Segments.Length, request.Segments.Length) - 1;

            ParameterInfo[] attributeParameters = EnumerableConverter.ToArray(EnumerableUtility.Reverse(httpMethod.GetParameters()));
            StringBuilder attributeAbsolutePath = new StringBuilder();
            int attributeParametersIndex = 0;
            for (int i = maxLength; i >= 0; i--)
            {
                string segment = HttpUtility.UrlDecode(attribute.Segments[i]);
                int startCurlyBracket = segment.IndexOf("{", StringComparison.Ordinal);
                int endCurlyBracket = segment.LastIndexOf("}", StringComparison.Ordinal);
                if (startCurlyBracket >= 0 && endCurlyBracket >= 0)
                {
                    string requestSegment = request.Segments[i];
                    ParameterInfo currentHttpMethodParameter = attributeParameters[attributeParametersIndex];
                    if (IsValueValid(requestSegment.Replace("/", "").Replace("{", "").Replace("}", ""), currentHttpMethodParameter.ParameterType))
                    {
                        attributeAbsolutePath.Insert(0, HttpUtility.UrlDecode(requestSegment));
                    }
                    attributeParametersIndex++;
                }
                else
                {
                    attributeAbsolutePath.Insert(0, segment);
                }
            }

            return request.AbsolutePath.Equals(attributeAbsolutePath.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool CanRequestImpersonateAttribute(string[] requestSegments, string[] attributeSegments)
        {
            StringBuilder absolutePath = new StringBuilder();
            StringBuilder attributeAbsolutePath = new StringBuilder();
            for (int i = attributeSegments.Length - 1; i >= 0; i--)
            {
                string attributeSegment = HttpUtility.UrlDecode(attributeSegments[i]);
                string requestSegment = HttpUtility.UrlDecode(requestSegments[i]);
                int startCurlyBracket = attributeSegment.IndexOf("{", StringComparison.Ordinal);
                int endCurlyBracket = attributeSegment.LastIndexOf("}", StringComparison.Ordinal);
                attributeAbsolutePath.Insert(0, attributeSegment);
                absolutePath.Insert(0, startCurlyBracket >= 0 && endCurlyBracket >= 0 ? attributeSegment : requestSegment);
            }
            return absolutePath.ToString().Equals(attributeAbsolutePath.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsValueValid(string parameterValue, Type parameterType)
        {
            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.Boolean:
                    return TesterDoerUtility.TryExecuteFunction(bool.Parse, parameterValue);
                case TypeCode.Byte:
                    return TesterDoerUtility.TryExecuteFunction(byte.Parse, parameterValue);
                case TypeCode.Decimal:
                    return TesterDoerUtility.TryExecuteFunction(decimal.Parse, parameterValue);
                case TypeCode.Double:
                    return TesterDoerUtility.TryExecuteFunction(double.Parse, parameterValue);
                case TypeCode.Int16:
                    return TesterDoerUtility.TryExecuteFunction(short.Parse, parameterValue);
                case TypeCode.Int32:
                    return TesterDoerUtility.TryExecuteFunction(int.Parse, parameterValue);
                case TypeCode.Int64:
                    return TesterDoerUtility.TryExecuteFunction(long.Parse, parameterValue);
                case TypeCode.SByte:
                    return TesterDoerUtility.TryExecuteFunction(sbyte.Parse, parameterValue);
                case TypeCode.Single:
                    return TesterDoerUtility.TryExecuteFunction(float.Parse, parameterValue);
                case TypeCode.UInt16:
                    return TesterDoerUtility.TryExecuteFunction(ushort.Parse, parameterValue);
                case TypeCode.UInt32:
                    return TesterDoerUtility.TryExecuteFunction(uint.Parse, parameterValue);
                case TypeCode.UInt64:
                    return TesterDoerUtility.TryExecuteFunction(ulong.Parse, parameterValue);
            }
            return true;
        }
    }
}