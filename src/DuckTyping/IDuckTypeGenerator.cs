namespace DuckTyping
{
    using System;

    /// <summary>
    /// Defines the duck factory interface.
    /// </summary>
    public interface IDuckTypeGenerator
    {
        /// <summary>
        /// Creates an instance of a duck type.
        /// </summary>
        /// <param name="inst">The object instance that the duck type is based on.</param>
        /// <param name="duckTypes">An array of duck type interfaces that must be implemented on the duck type.</param>
        /// <param name="serviceProvider">The current dependency injection scope.</param>
        /// <returns>An instance of the desired type.</returns>
        object CreateDuck(object inst, Type[] duckTypes, IServiceProvider serviceProvider = null);

        /// <summary>
        /// Checks if a duck type can be created from the given type.
        /// </summary>
        /// <param name="baseType">The type to create the duck from.</param>
        /// <param name="duckTypes">An array of duck type interfaces that must be implemented on the duck type.</param>
        /// <param name="serviceProvider">The current dependency injection scope.</param>
        /// <returns>True if a duck type can be created; otherwise false.</returns>
        bool IsDuck(Type baseType, Type[] duckTypes, IServiceProvider serviceProvider = null);

        /// <summary>
        /// Gets or creates a <see cref="Type"/> that represent the desired duck types.
        /// </summary>
        /// <param name="baseType">The type being ducked.</param>
        /// <param name="duckTypes">The duck type interfaces.</param>
        /// <param name="serviceProvider">The current dependency injection scope.</param>
        /// <returns>A <see cref="Type"/> that represents the duck types.</returns>
        Type GetOrCreateDuckType(Type baseType, Type[] duckTypes, IServiceProvider serviceProvider = null);
    }
}
