using System; // Keep for .NET 4.6

namespace RibbonXml
{
    /// <summary>
    /// Indicates that the associated code element participates in reflective data
    /// transfer operations within the CAD system.
    /// This attribute is typically applied to members whose values are intended
    /// to be read via reflection and assigned to corresponding members in another
    /// object instance.
    /// </summary>
    /// <remarks>
    /// This attribute serves as a marker for reflection-based copy or synchronization
    /// processes. It does not contain additional logic or metadata.
    /// It supports multiple usages on a single element and is inherited by derived
    /// classes or overridden members.
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    internal sealed class XmlOutAttribute : Attribute
    { }
}