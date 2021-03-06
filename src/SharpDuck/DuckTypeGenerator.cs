﻿namespace SharpDuck
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading.Tasks;

    using FluentIL;
    using SharpDuck.Reflection;

    /// <summary>
    /// A factory for building duck types.
    /// </summary>
    public class DuckTypeGenerator
        : IDuckTypeGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuckTypeGenerator"/> class.
        /// </summary>
        public DuckTypeGenerator()
        {
        }

        /// <summary>
        /// Gets the name of a duck type.
        /// </summary>
        /// <param name="baseType">The type base type.</param>
        /// <param name="duckTypes">The duck types.</param>
        /// <returns>The type name.</returns>
        public static string TypeName(Type baseType, Type[] duckTypes)
        {
            return string.Format("Dynamic.Ducks.{0}_{1}",
                baseType.Name,
                string.Join("_", duckTypes.Select(t => t.Name)));
        }

        /// <summary>
        /// Creates an instance of a duck type.
        /// </summary>
        /// <typeparam name="T">The type of duck type to create.</typeparam>
        /// <param name="inst">The object instance to create the duck from.</param>
        /// <param name="serviceProvider">A service provider.</param>
        /// <returns>An instance of the duck type.</returns>
        public T CreateDuck<T>(object inst, IServiceProvider serviceProvider = null)
        {
            if (inst == null)
            {
                return default(T);
            }

            return (T)this.CreateDuck(inst, new Type[] { typeof(T) }, serviceProvider);
        }

        /// <summary>
        /// Creates an instance of a duck type.
        /// </summary>
        /// <typeparam name="T1">The first type the duck type must implement.</typeparam>
        /// <typeparam name="T2">The second type the duck type must implement.</typeparam>
        /// <param name="inst">The object instance to create the duck from.</param>
        /// <param name="serviceProvider">The current dependency injection scope.</param>
        /// <returns>An instance of the duck type.</returns>
        public object CreateDuck<T1, T2>(object inst, IServiceProvider serviceProvider = null)
        {
            if (inst == null)
            {
                return null;
            }

            return this.CreateDuck(inst, new Type[] { typeof(T1), typeof(T2) }, serviceProvider);
        }

        /// <summary>
        /// Checks if a one or more duck types can be created from a given base type.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="duckTypes">The duck type interfaces.</param>
        /// <param name="serviceProvider">The current dependency injection scope.</param>
        /// <returns>True if the duck type can be created; otherwise false.</returns>
        public bool IsDuck(Type baseType, Type[] duckTypes, IServiceProvider serviceProvider = null)
        {
            foreach (Type t in duckTypes)
            {
                foreach (MethodInfo method in t.GetMethods())
                {
                    MethodInfo mi = baseType.GetMethodWithParameters(method.Name, method.GetParameters());
                    if (mi == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Creates an instance of a duck type.
        /// </summary>
        /// <param name="inst">The object instance that the duck type is based on.</param>
        /// <param name="duckTypes">An array of duck type interfaces that must be implemented on the duck type.</param>
        /// <param name="serviceProvider">The current dependency injection scope.</param>
        /// <returns>An instance of the desired type.</returns>
        public object CreateDuck(object inst, Type[] duckTypes, IServiceProvider serviceProvider = null)
        {
            if (inst == null)
            {
                return null;
            }

            Type duckType = this.GetOrCreateDuckType(inst.GetType(), duckTypes, null);
            return Activator.CreateInstance(duckType, inst, null);
        }

        /// <summary>
        /// Gets or creates a duck type.
        /// </summary>
        /// <typeparam name="T">The duck type.</typeparam>
        /// <param name="baseType">The ducks base type.</param>
        /// <returns>An instance of the duck type.</returns>
        public Type GetOrCreateDuckType<T>(Type baseType)
        {
            Utility.ThrowIfArgumentNull(baseType, nameof(baseType));
            return this.GetOrCreateDuckType(baseType, new Type[] { typeof(T) }, null);
        }

        /// <summary>
        /// Gets or creates a duck type.
        /// </summary>
        /// <typeparam name="T1">The first duck type interface.</typeparam>
        /// <typeparam name="T2">The second duck type interface.</typeparam>
        /// <param name="baseType">The ducks base type.</param>
        /// <returns>An instance of the duck type.</returns>
        public Type GetOrCreateDuckType<T1, T2>(Type baseType)
        {
            Utility.ThrowIfArgumentNull(baseType, nameof(baseType));
            return this.GetOrCreateDuckType(baseType, new Type[] { typeof(T1), typeof(T2) }, null);
        }

        /// <summary>
        /// Gets or creates a <see cref="Type"/> that represent the desired duck types.
        /// </summary>
        /// <param name="baseType">The type being ducked.</param>
        /// <param name="duckTypes">The duck type interfaces.</param>
        /// <param name="serviceProvider">The current dependency injection scope.</param>
        /// <returns>A <see cref="Type"/> that represents the duck types.</returns>
        public Type GetOrCreateDuckType(Type baseType, Type[] duckTypes, IServiceProvider serviceProvider)
        {
            Type duckType = TypeFactory.Default.GetType(TypeName(baseType, duckTypes), true);
            if (duckType == null)
            {
                duckType = this.GenerateType(baseType, duckTypes, serviceProvider);
            }

            return duckType;
        }

        /// <summary>
        /// Generates the type.
        /// </summary>
        /// <param name="baseType">The base type being ducked.</param>
        /// <param name="duckTypes">The duck type interfaces.</param>
        /// <param name="serviceProvider">The current dependency scope.</param>
        /// <returns>A duck type.</returns>
        private Type GenerateType(Type baseType, Type[] duckTypes, IServiceProvider serviceProvider)
        {
            var typeBuilder = TypeFactory
                .Default
                .NewType(TypeName(baseType, duckTypes))
                .Public()
                .Class()
                .Implements<IDuckTypedObject>()
                .ImplementsInterfaces(duckTypes);

            var baseTypeField = typeBuilder
                .NewField("target", baseType)
                .Private();

            var serviceProviderField = typeBuilder
                .NewField<IServiceProvider>("serviceProvider")
                .Private();

            foreach (Type duckType in duckTypes)
            {
                var context = new TypeFactoryContext(typeBuilder, duckType, baseType, serviceProvider, baseTypeField, serviceProviderField);
                this.ImplementInterfaces(context);
            }

            // Implement the IDuckTypedObject interface.
            this.ImplementDuckTypedObjectInterface(typeBuilder, baseTypeField);

            // Add a constructor to the type.
            this.AddConstructor(typeBuilder, baseType, baseTypeField, serviceProviderField);

            // Create the type.
            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Adds a constructor to the mixin type.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> use to construct the type.</param>
        /// <param name="baseType">The base <see cref="Type"/> being ducked.</param>
        /// <param name="baseTypeField">The <see cref="FieldBuilder"/> which will hold the instances of the base types.</param>
        /// <param name="serviceProviderField">The <see cref="FieldBuilder"/> which will hold the instance of the dependency injection resolver.</param>
        private void AddConstructor(
            ITypeBuilder typeBuilder,
            Type baseType,
            IFieldBuilder baseTypeField,
            IFieldBuilder serviceProviderField)
        {
            // Build Constructor.
            var constructorBuilder = typeBuilder
                .NewConstructor()
                .Public()
                .HideBySig()
                .SpecialName()
                .RTSpecialName()
                .CallingConvention(CallingConventions.HasThis)
                .Param(baseType, "target")
                .Param<IServiceProvider>("serviceProvider");

            constructorBuilder
                .Body()
                .LdArg0()
                .Call(baseType.GetConstructor(new Type[0]))

                .LdArg0()
                .LdArg1()
                .StFld(baseTypeField)

                .LdArg0()
                .LdArg2()
                .StFld(serviceProviderField)
                .Ret();
        }

        /// <summary>
        /// Implements the <see cref="IDuckTypedObject"/> interface on the duck type.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> use to construct the type.</param>
        /// <param name="baseTypeField">The <see cref="FieldBuilder"/> which will hold the instance of the type being duck typed.</param>
        private void ImplementDuckTypedObjectInterface(
            ITypeBuilder typeBuilder,
            IFieldBuilder baseTypeField)
        {
            var propertyAdaptedObject = typeBuilder
                .NewProperty<object>("DuckTypedObject")
                .Getter(m => m
                    .Public()
                    .Virtual()
                    .HideBySig()
                    .NewSlot()
                    .CallingConvention(CallingConventions.HasThis)
                    .Body(il => il
                        .LdArg0()
                        .LdFld(baseTypeField)
                        .Ret()));
        }

        private void ImplementInterfaces(TypeFactoryContext context)
        {
            var propertyMethods = new Dictionary<string, IMethodBuilder>();

            foreach (var memberInfo in context.NewType.GetMembers())
            {
                if (memberInfo.MemberType == MemberTypes.Method)
                {
                    MethodInfo methodInfo = (MethodInfo)memberInfo;
                    Type[] methodArgs = methodInfo
                    .GetParameters().Select(p => p.ParameterType).ToArray();

                    if (methodInfo.ContainsGenericParameters == true)
                    {
                        Type[] genericArguments = methodInfo.GetGenericArguments();

                        var methodBuilder = context
                            .TypeBuilder
                            .NewMethod(methodInfo.Name)
                            .Public()
                            .Virtual()
                            .Params(methodArgs)
                            .Returns(methodInfo.ReturnType);

                        methodBuilder
                            .NewGenericParameters(
                                genericArguments.Select(t => t.Name).ToArray(),
                                (gps) =>
                                {
                                    for (int m = 0; m < gps.Length; m++)
                                    {
                                        gps[m].Attributes = genericArguments[m].GenericParameterAttributes;
                                    }
                                });

/*
                        GenericTypeParameterBuilder[] genericTypeParameterBuilder = methodBuilder
                            .DefineGenericParameters(
                                genericArguments
                                    .Select(t => t.Name)
                                        .ToArray());

                        for (int m = 0; m < genericTypeParameterBuilder.Length; m++)
                        {
                            genericTypeParameterBuilder[m].SetGenericParameterAttributes(genericArguments[m].GetTypeInfo().GenericParameterAttributes);
                        }
*/

                        var methodIL = methodBuilder.Body();

                        if (context.BaseType.GetMethod(methodInfo.Name, methodInfo.GetGenericArguments()) == null)
                        {
                            // Throw NotImplementedException
                            methodIL.ThrowException(typeof(NotImplementedException));
                            continue;
                        }

                        ILocal methodReturn = null;
                        if (methodInfo.ReturnType != typeof(void))
                        {
                            methodIL.DeclareLocal(methodInfo.ReturnType, out methodReturn);
                        }

                        methodIL
                            .LdArg0()
                            .LdFld(context.BaseObjectField)
                            .EmitLoadParameters(methodInfo);

                        MethodInfo callMethod1 = context.BaseObjectField.FieldType.GetMethod(memberInfo.Name, genericArguments);
                        MethodInfo callMethod = context.BaseObjectField.FieldType.GetMethod(memberInfo.Name, methodArgs).MakeGenericMethod(genericArguments);

                        methodIL
                            .CallVirt(callMethod1);

                        if (methodReturn != null)
                        {
                            methodIL
                                .StLoc(methodReturn)
                                .LdLoc(methodReturn);
                        }

                        methodIL.Ret();
                    }
                    else
                    {
                        MethodAttributes attrs = methodInfo.Attributes & ~MethodAttributes.Abstract;
                        var methodBuilder = context
                            .TypeBuilder
                            .NewMethod(
                                methodInfo.Name,
                                attrs,
                                CallingConventions.HasThis,
                                methodInfo.ReturnType)
                            .Params(methodArgs);

                        var methodIL = methodBuilder.Body();

                        if (context.BaseType.GetMethod(methodInfo.Name, methodArgs) == null)
                        {
                            // Throw NotImplementedException
                            methodIL.ThrowException(typeof(NotImplementedException));
                            continue;
                        }

                        ILocal methodReturn = null;
                        if (methodInfo.ReturnType != typeof(void))
                        {
                            methodIL.DeclareLocal(methodInfo.ReturnType, out methodReturn);
                        }

                        methodIL
                            .LdArg0()
                            .LdFld(context.BaseObjectField)
                            .EmitLoadParameters(methodInfo);

                        MethodInfo callMethod = context.BaseObjectField.FieldType.GetMethod(memberInfo.Name, methodArgs);
                        methodIL
                            .CallVirt(callMethod);

                        if (methodReturn != null)
                        {
                            methodIL
                                .StLoc(methodReturn)
                                .LdLoc(methodReturn);
                        }

                        methodIL.Ret();

                        if (methodInfo.IsProperty() == true)
                        {
                            propertyMethods.Add(methodInfo.Name, methodBuilder);
                        }
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    var propertyBuilder = context
                        .TypeBuilder
                        .NewProperty(
                            memberInfo.Name,
                            ((PropertyInfo)memberInfo).PropertyType)
                        .Attributes(PropertyAttributes.SpecialName);

                    if (propertyMethods.TryGetValue(memberInfo.PropertyGetName(), out IMethodBuilder getMethod) == true)
                    {
                        propertyBuilder.GetMethod = getMethod;
                    }

                    if (propertyMethods.TryGetValue(memberInfo.PropertySetName(), out IMethodBuilder setMethod) == true)
                    {
                        propertyBuilder.SetMethod = setMethod;
                    }
                }
            }
        }
    }
}
