using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#if false
namespace CareerManager
{
	internal class ToolbarTypes
	{
		

		internal readonly Type iToolbarManagerType;

		internal readonly Type functionVisibilityType;

		internal readonly Type functionDrawableType;

		internal readonly ButtonTypes button;

		internal ToolbarTypes()
		{
			this.iToolbarManagerType = ToolbarTypes.getType("Toolbar.IToolbarManager");
			this.functionVisibilityType = ToolbarTypes.getType("Toolbar.FunctionVisibility");
			this.functionDrawableType = ToolbarTypes.getType("Toolbar.FunctionDrawable");
			Type type = ToolbarTypes.getType("Toolbar.IButton");
			this.button = new ButtonTypes(type);
		}

		internal static Type getType(string name)
		{
			IEnumerable<AssemblyLoader.LoadedAssembly> arg_32_0 = AssemblyLoader.loadedAssemblies;
			Func<AssemblyLoader.LoadedAssembly, IEnumerable<Type>> arg_32_1;
			if ((arg_32_1 = ToolbarTypes.sealed1.Instance__5_0) == null)
			{
				arg_32_1 = (ToolbarTypes.sealed1.Instance__5_0 = new Func<AssemblyLoader.LoadedAssembly, IEnumerable<Type>>(ToolbarTypes.sealed1.Instance.<getType>b__5_0));
			}
			return arg_32_0.SelectMany(arg_32_1).SingleOrDefault((Type t) => t.FullName == name);
		}

		internal static PropertyInfo getProperty(Type type, string name)
		{
			return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
		}

		internal static PropertyInfo getStaticProperty(Type type, string name)
		{
			return type.GetProperty(name, BindingFlags.Static | BindingFlags.Public);
		}

		internal static EventInfo getEvent(Type type, string name)
		{
			return type.GetEvent(name, BindingFlags.Instance | BindingFlags.Public);
		}

		internal static MethodInfo getMethod(Type type, string name)
		{
			return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
		}
	}
}

#endif