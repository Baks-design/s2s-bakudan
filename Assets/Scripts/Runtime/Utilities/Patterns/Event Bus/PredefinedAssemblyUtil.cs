using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Runtime.Utilities.Patterns.EventBus
{
    /// <summary>
    /// A utility class, PredefinedAssemblyUtil, provides methods to interact with predefined assemblies.
    /// It allows to get all types in the current AppDomain that implement from a specific Interface type.
    /// For more details, <see href="https://docs.unity3d.com/2023.3/Documentation/Manual/ScriptCompileOrderFolders.html">visit Unity Documentation</see>
    /// </summary>
    public static class PredefinedAssemblyUtil
    {
        static readonly ConcurrentDictionary<AssemblyType, Type[]> _assemblyTypesCache = new();

        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass
        }

        static AssemblyType? GetAssemblyType(string assemblyName) => assemblyName switch
        {
            "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
            "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
            "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
            "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
            _ => null
        };

        static Type[] GetTypesFromAssembly(Assembly assembly)
        {
            var assemblyType = GetAssemblyType(assembly.GetName().Name);
            if (assemblyType == null) return null;

            return _assemblyTypesCache.GetOrAdd((AssemblyType)assemblyType, _ => assembly.GetTypes());
        }

        static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results)
        {
            if (assemblyTypes == null)
            {
                Debug.LogError("Assembly types array is null.");
                return;
            }

            var implementingTypes = assemblyTypes.Where(type => type != interfaceType && interfaceType.IsAssignableFrom(type));
            foreach (var type in implementingTypes)
                results.Add(type);
        }

        public static List<Type> GetTypes(Type interfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var assemblyTypes = GetTypesFromAssembly(assembly);
                if (assemblyTypes == null) continue;

                AddTypesFromAssembly(assemblyTypes, interfaceType, types);
            }

            return types;
        }
    }
}