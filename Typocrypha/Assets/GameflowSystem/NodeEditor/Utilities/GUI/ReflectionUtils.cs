using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Gameflow
{
    //partial class to add all other GUI utility modules to
    namespace GUIUtilities
    { 
        public static class ReflectiveEnumerator
        {
            static ReflectiveEnumerator() { }
            public static IEnumerable<Type> GetAllSubclassTypes<T>() where T : class
            {
                List<Type> types = new List<Type>();
                foreach (Type type in
                    Assembly.GetAssembly(typeof(T)).GetExportedTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                {
                    types.Add(type);
                    //objects.Add((T)Activator.CreateInstance(type, constructorArgs));
                }
                //objects.Sort();
                return types;
            }
        }
    }
}
