using System; // Keep for .NET 4.6

namespace RibbonXml
{
    public interface IControlHandler
    {
        /// <summary>
        /// Gets the strongly-typed reference to the underlying Ribbon definition.
        /// </summary>
        RibbonBase Source { get; }
        object TargetObject { get; }
    }
    /// <summary>
    /// Base class for all Ribbon control wrappers.
    /// Provides a strongly-typed reference to the underlying Ribbon item
    /// and stores its associated <c>Id</c> for identification.
    /// </summary>
    /// <typeparam name="R">The type of the underlying Ribbon item (e.g., RibbonLabel, RibbonButton).</typeparam>
    public abstract class ControlHandler<R> : IControlHandler
        where R : class, new()
    {
        /// <summary>
        /// Gets the strongly-typed reference to the underlying Ribbon item.
        /// </summary>
        public R Target { get; }
        object IControlHandler.TargetObject => Target;

        /// <summary>
        /// Gets the strongly-typed reference to the underlying Ribbon definition.
        /// </summary>
        public RibbonBase Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlHandler{R}"/> class.
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
        public ControlHandler(R target, RibbonBase source)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target),
                "Ribbon target reference cannot be null.");
            Source = source ?? throw new ArgumentNullException(nameof(source),
                "Ribbon source reference cannot be null.");
        }
    }
}