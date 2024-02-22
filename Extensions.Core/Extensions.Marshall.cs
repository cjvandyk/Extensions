/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    /// <summary>
    /// An Assembly marshall class.
    /// </summary>
    [Serializable]
    public static partial class Marshall
    {
        /// <summary>
        /// Dictionary containing marshalled assemblies.
        /// </summary>
        internal static Dictionary<string, Assembly> Assemblies =
            new Dictionary<string, Assembly>();

        /// <summary>
        /// Check if a given assembly is available for use.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to check.</param>
        /// <returns>True if the assembly is available, else false.</returns>
        public static bool IsAvailable(string assemblyName)
        {
            if (Assemblies.ContainsKey(assemblyName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to load a new Assembly to the Assemblies stack.  If the
        /// Assembly already exist, update it if needed.
        /// </summary>
        /// <param name="assemblyName">The name of the Assembly.</param>
        /// <param name="assembly">The Assembly object to load.</param>
        /// <returns>True if loaded successfully, esle false.</returns>
        public static bool Load(string assemblyName, Assembly assembly = null)
        {
            try
            {
                if (assembly == null)
                {
                    assembly = Assembly.Load(assemblyName);
                }
                if (!Assemblies.ContainsKey(assemblyName))
                {
                    Assemblies.Add(assemblyName, assembly);
                }
                else
                {
                    if (Assemblies[assemblyName] != assembly)
                    {
                        Assemblies[assemblyName] = assembly;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to invoke member methods of the target Assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the Assembly to target.</param>
        /// <param name="methodName">The name of the Method to invoke.</param>
        /// <param name="args">An object array of parameter values to pass
        /// to the invoked Method.</param>
        /// <returns>The return value of the invoked Method.</returns>
        /// <exception cref="Exception">If the Assembly is not in the stack
        /// it will attempt to load it, but failing that, an exception is
        /// thrown noting the problem.</exception>
        public static object Invoke(string assemblyName, 
                                    string methodName,
                                    object[] args)
        {
            if (!Assemblies.ContainsKey(assemblyName))
            {
                if (!Load(assemblyName))
                {
                    throw new Exception($"Assembly [{assemblyName}] not " +
                        $"loaded.  Try using Marshall.Load() to properly" +
                        $"load the target Assembly before calling .Invoke().");
                }
            }
            Type type = Assemblies[assemblyName].GetTypes()
                .Where(C => C.Name == methodName).First();
            MethodInfo method = type.GetMethod(methodName);
            return type.InvokeMember(methodName, 
                                     BindingFlags.InvokeMethod, 
                                     null, 
                                     type, 
                                     args);
        }
    }
}
