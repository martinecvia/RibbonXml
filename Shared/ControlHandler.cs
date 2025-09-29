using System;

namespace RibbonXml
{
    /// <summary>
    /// Base class for all Ribbon control wrappers.
    /// Provides a strongly-typed reference to the underlying Ribbon item
    /// and stores its associated <c>Id</c> for identification.
    /// </summary>
    /// <typeparam name="RibbonRef">The type of the underlying Ribbon item (e.g., RibbonLabel, RibbonButton).</typeparam>
    public abstract class ControlHandler<RibbonRef>
        where RibbonRef : class, new()
    {
        /// <summary>
        /// Gets the strongly-typed reference to the underlying Ribbon item.
        /// </summary>
        public RibbonRef Target { get; }

        /// <summary>
        /// Gets the strongly-typed reference to the underlying Ribbon definition.
        /// </summary>
        public RibbonBase Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlHandler{RibbonRef}"/> class.
        /// </summary>
        /// <param name="target">
        /// A reference to the underlying Ribbon item instance.
        /// Must not be null.
        /// </param>
        /// <param name="source">
        /// A reference to the underlying RibbonXml definition instance.
        /// Must not be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="target"/> is null or empty, or if <paramref name="source"/> is null.
        /// </exception>
        public ControlHandler(RibbonRef target, RibbonBase source)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target),
                "Ribbon target reference cannot be null.");
            Source = source ?? throw new ArgumentNullException(nameof(source),
                "Ribbon source reference cannot be null.");
        }
    }
}