namespace SharpDuck.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using FluentIL;

    /// <summary>
    /// IL Emit Extension methods
    /// </summary>
    internal static class EmitExtensionMethods
    {
        /// <summary>
        /// Emits optimized IL to load parameters.
        /// </summary>
        /// <param name="methodIL">The <see cref="IEmitter"/> to use.</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to emit the parameters for.</param>
        /// <returns>The passed in <see cref="IEmitter"/>.</returns>
        public static IEmitter EmitLoadParameters(this IEmitter methodIL, MethodInfo methodInfo)
        {
            return methodIL.EmitLoadParameters(methodInfo.GetParameters());
        }

        /// <summary>
        /// Emits optimized IL to load parameters.
        /// </summary>
        /// <param name="methodIL">The <see cref="IEmitter"/> to use.</param>
        /// <param name="parameters">The parameters loads to emit.</param>
        /// <returns>The passed in <see cref="IEmitter"/>.</returns>
        public static IEmitter EmitLoadParameters(this IEmitter methodIL, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                methodIL.EmitLdArg(i);
            }

            return methodIL;
        }

        /// <summary>
        /// Emits optimized IL to load an argument.
        /// </summary>
        /// <param name="methodIL">The <see cref="IEmitter"/> to use.</param>
        /// <param name="index">The arguement index.</param>
        /// <returns>The passed in <see cref="IEmitter"/>.</returns>
        public static IEmitter EmitLdArg(this IEmitter methodIL, int index)
        {
            if (index == 0)
            {
                methodIL.LdArg1();
            }
            else if (index == 1)
            {
                methodIL.LdArg2();
            }
            else if (index == 2)
            {
                methodIL.LdArg3();
            }
            else
            {
                methodIL.LdArg(index + 1);
            }

            return methodIL;
        }

        /// <summary>
        /// Implements the interfaces and any of their descendent interfaces.
        /// </summary>
        /// <param name="typeBuilder">A type builder.</param>
        /// <param name="interfaceTypes">The interface type.</param>
        /// <returns>The type builder.</returns>
        public static ITypeBuilder ImplementsInterfaces(this ITypeBuilder typeBuilder, params Type[] interfaceTypes)
        {
            foreach (var interfaceType in interfaceTypes)
            {
                typeBuilder.Implements(interfaceType);

                Type[] baseTypes = interfaceType.GetInterfaces();
                if (baseTypes.IsNullOrEmpty() == false)
                {
                    typeBuilder.ImplementsInterfaces(baseTypes);
                }
            }

            return typeBuilder;
        }
    }
}