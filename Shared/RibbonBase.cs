using System; // Keep for .NET 4.6
using System.Collections;
using System.ComponentModel;
using System.Linq; // Keep for .NET 4.6
using System.Reflection;
using System.Xml.Serialization;

using System.Diagnostics;

namespace RibbonXml
{
    public abstract class RibbonBase
    {
        public readonly string UUID = Guid.NewGuid().ToString("N").Substring(0, 8);

        [XmlOut]
        [XmlAttribute("Id")]
        [DefaultValue("")]
        [Description("Gets or sets the item id. " +
            "This id is used as the automation id for the corresponding control in the UI. " +
            "The framework does not otherwise use or validate it. " +
            "It is up to the application to set and use this id. " +
            "The default value is null.")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

        [XmlIgnore]
        public virtual string Cookie { get; set; } = string.Empty;

        /// <summary>
        /// Returns a string representation of the object, including only properties marked with <see cref="XmlOutAttribute"/>
        /// that have non-null values.
        /// </summary>
        /// <remarks>
        /// - Properties must be public instance properties.<br/>
        /// - Properties are included only if decorated with <c>[XmlOut]</c> and their value is not <c>null</c>.<br/>
        /// - If a property's value is a collection (excluding strings), the output will list the items, joined by commas.<br/>
        /// - If an item in the collection is of type <see cref="RibbonBase"/>, its <c>ToString()</c> result is used.
        /// </remarks>
        /// <returns>
        /// A formatted string like: <c>ClassName(Property1=Value1, Property2=[Item1, Item2], ...)</c>.
        /// </returns>
        public override string ToString()
        {
            var values = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                // Only [XmlOut] will be taken care of
                .Where(property => property.GetCustomAttribute<XmlOutAttribute>() != null)
                .Where(property => property.GetValue(this) != null)
                .Select(property =>
                {
                    object value = property.GetValue(this);
                    if (value != null && value is IEnumerable enumerable && !(value is string))
                    {
                        var list = string.Join(", ", enumerable.Cast<object>()
                            .Select(item => item is RibbonBase xml ? xml.ToString() : item?.ToString()));
                        return $"{property.Name}=[{list}]";
                    }
                    return $"{property.Name}={value}";
                }).ToList();
            values.Add($"Cookie={Cookie}"); values.Add($"UUID={UUID}");
            return $"{GetType().Name}({string.Join(", ", values)})";
        }

        internal Target Transform<Target>(Target target) => Transform(target, this);

        /// <summary>
        /// Copies values from members of a source object to matching members of a target object
        /// using reflection, based on the <see cref="XmlOutAttribute"/> marker.
        /// </summary>
        /// <typeparam name="Target">The type of the target object.</typeparam>
        /// <typeparam name="Source">
        /// The type of the source object, which must derive from <see cref="RibbonBase"/>.
        /// </typeparam>
        /// <param name="target">The object to receive the copied values.</param>
        /// <param name="source">The object providing the values.</param>
        /// <returns>
        /// The updated <paramref name="target"/> instance, or <see langword="default"/> if either
        /// argument is <see langword="null"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method scans all public instance properties of the <paramref name="source"/> object
        /// (including those inherited from base classes) that are decorated with
        /// <see cref="XmlOutAttribute"/>. For each matching property found in
        /// <paramref name="target"/>:
        /// </para>
        /// <list type="bullet">
        /// <item>Properties must be readable on the source.</item>
        /// <item>Properties must be writable on the target and have a compatible type, or a 
        /// nullable-to-non-nullable type conversion will be attempted.</item>
        /// <item>If the property does not exist on the target (due to CAD version differences),
        /// a debug message is logged and the property is skipped.</item>
        /// </list>
        /// <para>
        /// This method is designed to support multiple CAD environments (e.g., AutoCAD, ZWCAD)
        /// where property sets may differ between versions or platforms.
        /// </para>
        /// <para>
        /// Any exceptions thrown during assignment are caught, and details are written to the debug
        /// output to avoid runtime disruption.
        /// </para>
        /// </remarks>
        internal static Target Transform<Target, Source>(Target target, Source source)
            where Source : RibbonBase
        {
            if (target == null || source == null)
                return default;
            PropertyInfo[] applyableProperties = source.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                    .Where(property => property.GetCustomAttribute<XmlOutAttribute>() != null)
                    .ToArray();
            foreach (PropertyInfo sourceProperty in applyableProperties)
            {
                if (!sourceProperty.CanRead)
                    continue; // Very weird? Must be manipulated in memory
                try
                {
                    // Since we are supporting multiple versions of CAD
                    // we have to first check if property exists in current running version
                    // if not we will just print information into a debug console and call it a day
                    PropertyInfo targetProperty = target.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                        .FirstOrDefault(property => property.Name == sourceProperty.Name && // If there are overloads with the same name but different property types,
                                                                                            // filter to those assignable from the source type
                                                                                            // attempt to fix AmbiguousMatchException
                                                    property.PropertyType.IsAssignableFrom(sourceProperty.PropertyType));
                    // Property not found in API
                    if (targetProperty == null)
                    {
                        Debug.WriteLine($"[&RibbonXml] Transform: {target.GetType().Name}:{sourceProperty.Name} was not found");
                        continue;
                    }
                    if (sourceProperty.GetMethod == null || sourceProperty.GetMethod.GetMethodBody() == null)
                        continue; // prevention of NotImplementedException 
                    var sourceValue = sourceProperty.GetValue(source);
                    if (targetProperty.CanWrite == true
                        && targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                        targetProperty.SetValue(target, sourceValue, null);
                    // Type? -> Type conversion
                    else if (Nullable.GetUnderlyingType(sourceProperty.PropertyType) == targetProperty.PropertyType
                            && sourceValue != null)
                    {
                        try
                        {
                            // Unwrap Nullable<T> to T
                            targetProperty.SetValue(target, Convert.ChangeType(sourceValue, targetProperty.PropertyType), null);
                        }
                        catch { } // Silent catch, its a dirty way to convert bool? to bool if not null
                                  // however it saves a lot of time and provides "user friendly" approach
                    }
                    else if (sourceValue != null &&
                            // Fail IsAssignableFrom when types have the same name but come from different assemblies
                            targetProperty.PropertyType.FullName == sourceProperty.PropertyType.FullName &&
                            targetProperty.PropertyType.IsEnum && sourceProperty.PropertyType.IsEnum)
                    {
                        // Cross-assembly enum fix
                        targetProperty.SetValue(target, Enum.Parse(targetProperty.PropertyType, sourceValue.ToString()));
                    }
                    else
                    {
                        Debug.WriteLine($"[&RibbonXml] {sourceProperty.Name}: " +
                            $"Has different type target:{targetProperty.PropertyType} from source:{sourceProperty.PropertyType}");
                    }
                }
                catch (Exception)
                { }
            }
            return target;
        }
    }
}