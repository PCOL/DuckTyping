namespace DuckTyping
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Duck typing extension methods.
    /// </summary>
    public static class DuckTypingExtensionMethods
    {
        public static IServiceProvider Services { get; set; }

        /// <summary>
        /// Checks if an object is a duck type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is a duck type.</returns>
        public static bool IsDuckType(this object obj)
        {
            return obj != null &&
                IsDuckType(obj.GetType());
        }

        /// <summary>
        /// Checks if an object is a duck type.
        /// </summary>
        /// <param name="type">The object to check.</param>
        /// <returns>True if the object is a duck type.</returns>
        public static bool IsDuckType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typeof(IDuckTypedObject).IsAssignableFrom(type);
        }

        /// <summary>
        /// Checks if a duck type can be created for a given object.
        /// </summary>
        /// <typeparam name="T">The type of duck to create</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the a duck type cna be created from the given object; otherwise false.</returns>
        public static bool IsDuck<T>(this object obj)
        {
            return obj != null && IsDuck<T>(obj.GetType());
        }

        /// <summary>
        /// Checks if a duck type can be created for a given object.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must be able to implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must be able to implement.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the a duck type cna be created from the given object; otherwise false.</returns>
        public static bool IsDuck<T1, T2>(this object obj)
        {
            return obj != null && IsDuck<T1, T2>(obj.GetType());
        }

        /// <summary>
        /// Checks if a duck type can be created for a given object.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must be able to implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must be able to implement.</typeparam>
        /// <typeparam name="T3">The third interface type the duck type must be able to implement.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the a duck type cna be created from the given object; otherwise false.</returns>
        public static bool IsDuck<T1, T2, T3>(this object obj)
        {
            return obj != null && IsDuck<T1, T2, T3>(obj.GetType());
        }

        /// <summary>
        /// Checks if a duck type can be created for a given type.
        /// </summary>
        /// <typeparam name="T">The type of duck to create</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the a duck type cna be created from the given type; otherwise false.</returns>
        public static bool IsDuck<T>(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return IsDuck(type, typeof(T));
        }

        /// <summary>
        /// Checks if a duck type can be created for a given type.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must be able to implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must be able to implement.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the a duck type cna be created from the given type; otherwise false.</returns>
        public static bool IsDuck<T1, T2>(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return IsDuck(type, typeof(T1), typeof(T2));
        }

        /// <summary>
        /// Checks if a duck type can be created for a given type.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must be able to implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must be able to implement.</typeparam>
        /// <typeparam name="T3">The third interface type the duck type must be able to implement.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the a duck type cna be created from the given type; otherwise false.</returns>
        public static bool IsDuck<T1, T2, T3>(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return IsDuck(type, typeof(T1), typeof(T2), typeof(T3));
        }

        /// <summary>
        /// Creates a new instance of a duck type.
        /// </summary>
        /// <typeparam name="T">The duck type to create.</typeparam>
        /// <param name="obj">The object instance to create the duck from.</param>
        /// <returns>An instance of the duck type.</returns>
        public static T CreateDuck<T>(this object obj)
        {
            return (T)CreateDuck(obj, typeof(T));
        }

        /// <summary>
        /// Creates a new instance of a duck type.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must implement.</typeparam>
        /// <param name="inst">The object instance the duck is created from.</param>
        /// <returns>An instance of the duck type.</returns>
        public static object CreateDuck<T1, T2>(this object inst)
        {
            return CreateDuck(inst, typeof(T1), typeof(T2));
        }

        /// <summary>
        /// Creates a new instance of a duck type.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must implement.</typeparam>
        /// <typeparam name="T3">The third interface type the duck type must implement.</typeparam>
        /// <param name="inst">The object instance the duck is created from.</param>
        /// <returns>An instance of the duck type.</returns>
        public static object CreateDuck<T1, T2, T3>(this object inst)
        {
            return CreateDuck(inst, typeof(T1), typeof(T2), typeof(T3));
        }

        /// <summary>
        /// Creates a new duck type.
        /// </summary>
        /// <typeparam name="T">The duck type to create.</typeparam>
        /// <param name="type">The type to create the duck from.</param>
        /// <returns>The new duck type.</returns>
        public static Type CreateDuckType<T>(this Type type)
        {
            return CreateDuckType(type, typeof(T));
        }

        /// <summary>
        /// Creates a new duck type.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must implement.</typeparam>
        /// <param name="type">The type to create the duck from.</param>
        /// <returns>The new duck type.</returns>
        public static Type CreateDuckType<T1, T2>(this Type type)
        {
            return CreateDuckType(type, typeof(T1), typeof(T2));
        }

        /// <summary>
        /// Creates a new duck type.
        /// </summary>
        /// <typeparam name="T1">The first interface type the duck type must implement.</typeparam>
        /// <typeparam name="T2">The second interface type the duck type must implement.</typeparam>
        /// <typeparam name="T3">The third interface type the duck type must implement.</typeparam>
        /// <param name="type">The type to create the duck from.</param>
        /// <returns>The new duck type.</returns>
        public static Type CreateDuckType<T1, T2, T3>(this Type type)
        {
            return CreateDuckType(type, typeof(T1), typeof(T2), typeof(T3));
        }

        private static bool IsDuck(Type baseType, params Type[] duckTypes)
        {
            if (duckTypes == null)
            {
                throw new ArgumentNullException(nameof(duckTypes));
            }

            if (duckTypes.Length == 0)
            {
                throw new ArgumentException("There must be at least one duck type.", nameof(duckTypes));
            }

            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            return GetGenerator()
                .IsDuck(baseType, duckTypes, Services);
        }

        private static object CreateDuck(object inst, params Type[] duckTypes)
        {
            if (inst == null)
            {
                return null;
            }

            return GetGenerator()
                .CreateDuck(inst, duckTypes);
        }

        private static Type CreateDuckType(Type type, params Type[] duckTypes)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return GetGenerator()
                .GetOrCreateDuckType(type, duckTypes, Services);
        }

        private static IDuckTypeGenerator GetGenerator()
        {
            IDuckTypeGenerator generator = null;

            if (Services != null)
            {
                generator = Services.GetService<IDuckTypeGenerator>();
            }

            return generator ?? new DuckTypeGenerator();
        }
    }
}
