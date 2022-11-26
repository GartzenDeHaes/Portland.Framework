using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Portland.Reflection
{
	/// <summary>
	/// 
	/// </summary>
	public static class ReflectionUtils
	{
		/// <summary>
		/// Get all the fields of a class
		/// </summary>
		/// <param name="type">Type object of that class</param>
		/// <param name="flags">Binding flags</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance/* | BindingFlags.DeclaredOnly*/)
		{
			//if (type == null)
			//{
			//	return Enumerable.Empty<FieldInfo>();
			//}

			return type.GetFields(flags);//.Union(GetAllFields(type.BaseType, flags));
		}

		/// <summary>
		/// Get all properties of a class
		/// </summary>
		/// <param name="type">Type object of that class</param>
		/// <param name="flags">Binding flags</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<PropertyInfo> GetAllProperties(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy/* | BindingFlags.DeclaredOnly*/)
		{
			//if (type == null)
			//{
			//	return Enumerable.Empty<PropertyInfo>();
			//}

			return type.GetProperties(flags);//.Union(GetAllProperties(type.BaseType, flags));
		}

		/// <summary>
		/// Get all constructors of a class
		/// </summary>
		/// <param name="type">Type object of that class</param>
		/// <param name="flags">Binding flags</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<ConstructorInfo> GetAllConstructors(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
		{
			//if (type == null)
			//{
			//	return Enumerable.Empty<ConstructorInfo>();
			//}

			return type.GetConstructors(flags);
		}

		/// <summary>
		/// Get all methods of a class
		/// </summary>
		/// <param name="type">Type object for that class</param>
		/// <param name="flags">Binding flags</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<MethodInfo> GetAllMethods(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance/* | BindingFlags.DeclaredOnly*/)
		{
			//if (type == null)
			//{
			//	return Enumerable.Empty<MethodInfo>();
			//}

			return type.GetMethods(flags);//.Union(GetAllMethods(type.BaseType, flags));
		}

		public static bool DoesTypeSupportInterface(Type type, Type inter)
		{
			if (inter.IsAssignableFrom(type))
				return true;
			if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == inter))
				return true;
			return false;
		}

		public static IEnumerable<Assembly> GetReferencingAssemblies(Assembly assembly)
		{
			return AppDomain
				 .CurrentDomain
				 .GetAssemblies().Where(asm => asm.GetReferencedAssemblies().Any(asmName => AssemblyName.ReferenceMatchesDefinition(asmName, assembly.GetName())));
		}

		public static IEnumerable<Type> TypesImplementingInterface(Type desiredType)
		{
			var assembliesToSearch = new Assembly[] { desiredType.Assembly }
				 .Concat(GetReferencingAssemblies(desiredType.Assembly));
			return assembliesToSearch.SelectMany(assembly => assembly.GetTypes())
				 .Where(type => DoesTypeSupportInterface(type, desiredType));
		}

		public static IEnumerable<Type> NonAbstractTypesImplementingInterface(Type desiredType)
		{
			return TypesImplementingInterface(desiredType).Where(t => !t.IsAbstract);
		}
	}
}
