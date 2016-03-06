using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Cuemon.Xml;

namespace Cuemon.Web.UI
{
    /// <summary>
    /// This utility class is designed to make <see cref="Control"/> operations easier to work with.
    /// </summary>
    public static class ControlUtility
    {
        /// <summary>
        /// Resolves the partial unique identifier from the given <see cref="Control"/> object.
        /// </summary>
        /// <param name="source">The <see cref="Control"/> source to resolve the partial unique identifier from.</param>
        /// <returns>A partial unique identifier you then can complete with the respective Form og QueryString item name.</returns>
        public static string ResolvePartialUniqueId(Control source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            int hitCount = 0;
            string lastUniqueId = source.UniqueID;
            foreach (Control formControl in GetDescendantOrSelfControls(source, Filter.Include, typeof(HtmlForm)))
            {
                foreach (Control control in GetDescendantControls(formControl))
                {
                    string uniqueId = control.UniqueID;
                    if (uniqueId.Contains("$") && lastUniqueId.Contains("$"))
                    {
                        if (uniqueId.Substring(0, uniqueId.LastIndexOf("$", StringComparison.Ordinal)) == lastUniqueId.Substring(0, lastUniqueId.LastIndexOf("$", StringComparison.Ordinal)))
                        {
                            hitCount++;
                        }
                        else
                        {
                            hitCount = 0;
                        }
                    }
                    lastUniqueId = uniqueId;
                    if (hitCount >= 2) { return uniqueId.Substring(0, uniqueId.LastIndexOf("$", StringComparison.Ordinal)); }
                }
            }
            return null;
        }

        /// <summary>
        /// Determines whether the <see cref="Control"/> ID property is unique on the given and nested controls.
        /// </summary>
        /// <param name="source">The <see cref="Control"/> source object.</param>
        /// <param name="duplicateId">The duplicate id to display in exception messages or similar.</param>
        /// <returns>
        /// 	<c>true</c> if the <see cref="Control"/> ID property is unique on the given and nested controls; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasUniqueControlId(Control source, out string duplicateId)
        {
            IList<string> ids = new List<string>();
            duplicateId = null;
            // because of M$ odd way of programming, the ID property is not always the value it is assigned - therefor, 
            // one could recheck with ClientID, which again is not always as one would expect (the same goes for UniqueID, which in fact not always is unique - good job, MS).
            // i think i will let the user enable or disable this parse validation.
            foreach (Control control in GetDescendantOrSelfControls(source, Filter.Exclude, typeof(LiteralControl)))
            {
                if (!string.IsNullOrEmpty(control.UniqueID)) { ids.Add(control.UniqueID); }
            }

            if (ids.Count > 0)
            {
                foreach (string id in ids)
                {
                    duplicateId = id;
                    if (GetNoneUniqueControlIdCount(id, ids) > 0)
                    {
                        // check if id has been computed by M$
                        string partialUniqueId = ResolvePartialUniqueId(source);
                        if (partialUniqueId.Contains("$"))
                        {
                            partialUniqueId = partialUniqueId.Substring(0, partialUniqueId.IndexOf("$", StringComparison.Ordinal)); // if the id has been computed, it will (until changed by MS) be the first characters of the partial unique id like ex. ctl00
                        }
                        if (id != partialUniqueId) { return false; }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Control"/> sources contains one or more nested controls of them self.
        /// </summary>
        /// <param name="sources">The <see cref="Control"/> collection to match from and against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Control"/> sources contains one or more nested controls of them self; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsControl(IList<Control> sources)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            foreach (Control source in sources)
            {
                if (ContainsControl(source)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Control"/> source contains one or more nested controls of itself.
        /// </summary>
        /// <param name="source">The <see cref="Control"/> to match from and against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Control"/> source contains one or more nested controls of itself; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsControl(Control source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return ContainsControl(source, source.GetType());
        }

        /// <summary>
        /// Determines whether the specified <see cref="Control"/> source contains one or more nested controls of the by parameter specified target <see cref="Control"/>.
        /// </summary>
        /// <param name="source">The <see cref="Control"/> to match from.</param>
        /// <param name="targetControl">The <see cref="Control"/> to match against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Control"/> source contains one or more nested controls of the by parameter specified target <see cref="Control"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsControl(Control source, Control targetControl)
        {
            if (targetControl == null) throw new ArgumentNullException(nameof(targetControl));
            return ContainsControl(source, targetControl.GetType());
        }

        /// <summary>
        /// Determines whether the specified <see cref="Control"/> source contains one or more nested controls of the by parameter specified target control type.
        /// </summary>
        /// <param name="source">The <see cref="Control"/> to match from.</param>
        /// <param name="targetControlType">The control type to match against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Control"/> source contains one or more nested controls of the by parameter specified target control type; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsControl(Control source, Type targetControlType)
        {
            foreach (Control control in GetDescendantControls(source))
            {
                if (control.GetType() == targetControlType) { return true; }
            }
            return false;
        }

        private static int GetNoneUniqueControlIdCount(string compareId, IEnumerable<string> ids)
        {
            int count = -1; // since we know at least 1 is present (as we use the collection from caller), we initialize the value till -1 .. that means if count > 0, we have a duplicate id!
            foreach (string id in ids)
            {
                if (compareId == id) { count++; }
            }
            return count;
        }

        /// <summary>
        /// Gets the descendant controls from the specified source control.
        /// </summary>
        /// <param name="source">The source control to recursively retrieve descendant controls from.</param>
        /// <returns>A generic list of web controls.</returns>
        public static IList<Control> GetDescendantControls(Control source)
        {
            return GetDescendantOrSelfControlsCore(source, false, Filter.Exclude);
        }

        /// <summary>
        /// Gets the descendant controls from the specified source control.
        /// </summary>
        /// <param name="source">The source control to recursively retrieve descendant controls from.</param>
        /// <param name="filter">The filter action to apply to this method.</param>
        /// <param name="filterTypes">The filter statement to apply for the by parameter specified control and its controls hierarchy.</param>
        /// <returns>A generic list of web controls.</returns>
        public static IList<Control> GetDescendantControls(Control source, Filter filter, params Type[] filterTypes)
        {
            return GetDescendantOrSelfControlsCore(source, false, filter, filterTypes);
        }

        /// <summary>
        /// Gets the descendant or self controls from the specified source control.
        /// </summary>
        /// <param name="source">The source control to recursively retrieve descendant controls from.</param>
        /// <returns>A generic list of web controls.</returns>
        public static IList<Control> GetDescendantOrSelfControls(Control source)
        {
            return GetDescendantOrSelfControls(source, Filter.Exclude);
        }

        /// <summary>
        /// Gets the descendant or self controls from the specified source control.
        /// </summary>
        /// <param name="source">The source control to recursively retrieve descendant controls from.</param>
        /// <param name="filter">The filter action to apply to this method.</param>
        /// <param name="filterTypes">The filter statement to apply for the by parameter specified control and its controls hierarchy.</param>
        /// <returns>A generic list of web controls.</returns>
        public static IList<Control> GetDescendantOrSelfControls(Control source, Filter filter, params Type[] filterTypes)
        {
            return GetDescendantOrSelfControlsCore(source, true, filter, filterTypes);
        }

        /// <summary>
        /// Gets the descendant (or self) controls from the specified source control. Core method for the public methods GetDescendantOrSelfControls and GetDescendantControls.
        /// </summary>
        /// /// <param name="source">The source control to recursively retrieve descendant controls from.</param>
        /// <param name="includeSource">if set to <c>true</c> the <see cref="Control"/> source is the first element added to the collection.</param>
        /// <param name="filter">The filter action to apply to this method.</param>
        /// <param name="filterTypes">The filter statement to apply for the by parameter specified control and its controls hierarchy.</param>
        /// <returns>A generic list of web controls.</returns>
        private static IList<Control> GetDescendantOrSelfControlsCore(Control source, bool includeSource, Filter filter, params Type[] filterTypes)
        {
            List<Control> controls = new List<Control>();
            Stack<Control> stack = new Stack<Control>(controls.Count);
            stack.Push(source);
            while (stack.Count > 0)
            {
                bool processControl = true;
                Control control = stack.Pop();

                if (controls.Count == 0 && includeSource)
                {
                    processControl = false;
                    controls.Add(control);
                }

                if (processControl)
                {
                    if (filterTypes.Length > 0)
                    {
                        bool containsType = TypeUtility.ContainsType(control, filterTypes);
                        if (!containsType)
                        {
                            bool hasParentFilterType = false;
                            Control parentControl = control.Parent;
                            while (parentControl != null && !hasParentFilterType)
                            {
                                hasParentFilterType = TypeUtility.ContainsType(parentControl, filterTypes);
                                parentControl = parentControl.Parent;
                            }
                            containsType = hasParentFilterType;
                        }
                        switch (filter)
                        {
                            case Filter.Exclude:
                                if (!containsType) { controls.Add(control); }
                                break;
                            case Filter.Include:
                                if (containsType)
                                {
                                    foreach (Type filterType in filterTypes)
                                    {
                                        if (control.GetType() == filterType) { controls.Add(control); }
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        controls.Add(control);
                    }
                }

                foreach (Control c in control.Controls) { stack.Push(c); }
            }

            return controls;
        }

        /// <summary>
        /// Create and returns a XML stream representation of the control object supplied.
        /// </summary>
        /// <param name="control">The <see cref="System.Web.UI.Control"/> object to be represented as a XML stream.</param>
        /// <param name="excludePropertyBaseTypes">Filter out all control properties where the specified base type(s) is in the control property hierarchy.</param>
        /// <returns></returns>
        public static Stream ControlAsStream(Control control, params Type[] excludePropertyBaseTypes)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            Type controlType = control.GetType();
            PropertyInfo[] controlProperties = controlType.GetProperties();
            MemoryStream output;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream();
                using (XmlWriter writer = XmlWriter.Create(tempOutput, XmlWriterUtility.CreateSettings(Encoding.Unicode, true)))
                {
                    writer.WriteStartElement("Control");
                    writer.WriteAttributeString("type", controlType.Name);
                    writer.WriteStartElement("Properties");
                    foreach (PropertyInfo controlProperty in controlProperties)
                    {
                        string propertyName = controlProperty.Name;
                        if (propertyName == "InnerHtml" || propertyName == "InnerText") // these are ONLY allowed on Literal controls, and are very constly if not checked upon ..
                        {
                            if (!TypeUtility.ContainsType(controlType, typeof(LiteralControl), typeof(DataBoundLiteralControl))) { continue; }
                        }

                        bool excludeBaseType = false;
                        object controlPropertyValue = null;
                        try
                        {
                            controlPropertyValue = controlProperty.GetValue(control, null);
                            if (controlPropertyValue != null) // check if we have a valid control property
                            {
                                if (excludePropertyBaseTypes != null) { excludeBaseType = TypeUtility.ContainsType(controlPropertyValue, excludePropertyBaseTypes); }
                            }
                        }
                        catch (TargetInvocationException ex)
                        {
                            controlPropertyValue = ex.InnerException.Message;
                        }
                        finally
                        {
                            if (!excludeBaseType && controlPropertyValue != null)
                            {
                                EvaluateWriteControlPropertyXml(control, controlProperty, controlPropertyValue, writer);
                            }
                        }
                    }
                    writer.WriteEndElement(); // </Properties>
                    writer.WriteEndElement(); // </Control>
                    writer.Flush();
                }
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            return output;
        }

        /// <summary>
        /// Create and returns a XML stream representation of the control objects supplied.
        /// </summary>
        /// <param name="controls">The generic enumerable of control objects to be represented as a XML stream.</param>
        /// <param name="excludePropertyBaseTypes">Filter out all control properties specified base type(s) in the control property hierarchy.</param>
        /// <returns></returns>
        public static Stream ControlsAsStream(IEnumerable<Control> controls, params Type[] excludePropertyBaseTypes)
        {
            if (controls == null) throw new ArgumentNullException(nameof(controls));
            if (excludePropertyBaseTypes == null) throw new ArgumentNullException(nameof(excludePropertyBaseTypes));
            MemoryStream output;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream();
                using (XmlWriter writer = XmlWriter.Create(tempOutput, XmlWriterUtility.CreateSettings(Encoding.Unicode, true)))
                {
                    writer.WriteStartElement("Controls");
                    foreach (Control control in controls)
                    {
                        using (Stream stream = ControlAsStream(control, excludePropertyBaseTypes))
                        {
                            char[] chars = CharConverter.FromStream(stream, PreambleSequence.Remove);
                            writer.WriteRaw(chars, 0, chars.Length);
                        }
                    }
                    writer.WriteEndElement();
                    writer.Flush();
                }
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            return output;
        }

        private static void EvaluateWriteControlPropertyXml(Control control, PropertyInfo controlProperty, object controlPropertyValue, XmlWriter writer)
        {
            string propertyType = controlProperty.PropertyType.Name;
            string propertyValue = null;
            bool writeEntry;

            switch (propertyType)
            {
                case "Boolean":
                case "ControlCollection":
                case "Int16":
                case "Int32":
                case "Int64":
                case "CssStyleCollection":
                case "ListItemCollection":
                case "ListSelectionMode":
                case "String":
                case "TextBoxMode":
                case "DateTime":
                    writeEntry = true;
                    propertyValue = controlPropertyValue.ToString();
                    break;
                default:
                    writeEntry = false;
                    break;
            }

            if (writeEntry)
            {
                WriteControlPropertyXml(propertyType, propertyValue, control, controlProperty, controlPropertyValue, writer);
            }
        }

        private static void WriteControlPropertyXml(string propertyType, string propertyValue, Control control, PropertyInfo controlProperty, object controlPropertyValue, XmlWriter writer)
        {
            writer.WriteStartElement("Property");
            writer.WriteAttributeString("name", controlProperty.Name);
            writer.WriteAttributeString("type", propertyType);

            switch (propertyType)
            {
                case "Boolean":
                    writer.WriteString(controlPropertyValue.ToString().ToLowerInvariant());
                    break;
                case "ControlCollection":
                    WriteDescendantOrSelfControlsHierarchy(control, writer, typeof(Control));
                    break;
                case "Int16":
                case "Int32":
                case "Int64":
                case "ListSelectionMode":
                case "TextBoxMode":
                    writer.WriteString(controlPropertyValue.ToString());
                    break;
                case "CssStyleCollection":
                    CssStyleCollection styles = (CssStyleCollection)controlPropertyValue;
                    if (styles.Count > 0)
                    {
                        StringBuilder style = new StringBuilder();
                        foreach (string name in styles.Keys)
                        {
                            style.AppendFormat("{0}: {1};", name, styles[name]);
                        }
                        writer.WriteString(style.ToString());
                    }
                    break;
                case "ListItemCollection":
                    ListItemCollection listItems = (ListItemCollection)controlPropertyValue;
                    if (listItems.Count > 0)
                    {
                        writer.WriteStartElement("Items");
                        foreach (ListItem listItem in listItems)
                        {
                            writer.WriteStartElement("Item");
                            writer.WriteAttributeString("value", listItem.Value);
                            writer.WriteAttributeString("selected", listItem.Selected.ToString().ToLowerInvariant());
                            writer.WriteString(listItem.Text);
                            writer.WriteEndElement(); // </Item>
                        }
                        writer.WriteEndElement(); // </Items>
                    }
                    break;
                case "String":
                    if (!string.IsNullOrEmpty(propertyValue.Trim())) { writer.WriteString(propertyValue); }
                    break;
                case "DateTime":
                    writer.WriteString(((DateTime)controlPropertyValue).ToString("s", CultureInfo.InvariantCulture));
                    break;
            }

            writer.WriteEndElement(); // </Property>
        }

        private static void WriteDescendantOrSelfControlsHierarchy(Control control, XmlWriter writer, params Type[] targetTypes)
        {
            if (TypeUtility.ContainsType(control.Controls, true, targetTypes))
            {
                writer.WriteStartElement("Controls");
                foreach (Control childControl in control.Controls)
                {
                    if (TypeUtility.ContainsType(childControl, targetTypes))
                    {
                        writer.WriteStartElement("Control");
                        writer.WriteAttributeString("type", childControl.GetType().Name);
                        writer.WriteAttributeString("uniqueIdReference", childControl.UniqueID);
                        WriteDescendantOrSelfControlsHierarchy(childControl, writer, targetTypes);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }
        }
    }
}