//#pragma warning disable CS0162, CS0168//, CS1587, CS1998, IDE0028, IDE0059

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Logit;

namespace Extensions
{
    /// <summary>
    /// Core class for constants, enums and helper methods.
    /// </summary>
    [Serializable]
    public static partial class Core
    {
        #region Enums
        /// <summary>
        /// The list of valid AzureEnvironments used.
        /// </summary>
        public enum AzureEnvironment
        {
            /// <summary>
            /// China
            /// </summary>
            China,
            /// <summary>
            /// Default
            /// </summary>
            Commercial,
            /// <summary>
            /// Germany
            /// </summary>
            Germany,
            /// <summary>
            /// GCC/DoD
            /// </summary>
            USGovGCC,
            /// <summary>
            /// GCCHigh
            /// </summary>
            USGovGCCHigh
        }
        #endregion Enums

        #region Properties
        /// <summary>
        /// A dictionary of configurations for different tenants.
        /// </summary>
        public static Dictionary<string, TenantConfig> Tenants { get; private set; }
            = new Dictionary<string, TenantConfig>();

        /// <summary>
        /// The currently active tenant configuration in use.
        /// </summary>
        public static TenantConfig ActiveTenant { get; private set; } = null;

        /// <summary>
        /// The tenant name e.g. for contoso.sharepoint.us it would be "contoso".
        /// </summary>
        public static string TenantUrl { get; }
            = $"{ActiveTenant.TenantString}.sharepoint.us";

        /// <summary>
        /// The URI of the tenant.
        /// </summary>
        public static Uri TenantUri { get; }
            = new Uri("https://" + TenantUrl);

        /// <summary>
        /// Method to get the valid Authority URL given the AzureEnvironment.
        /// </summary>
        public static string Authority
        {
            get
            {
                switch (GetAzureEnvironment(ActiveTenant.AzureEnvironment))
                {
                    case AzureEnvironment.USGovGCCHigh:
                        return $"https://login.microsoftonline.us/{TenantUrl}";
                        break;
                    case AzureEnvironment.Commercial:
                        return $"https://login.microsoftonline.com/{TenantUrl}";
                        break;
                    default:
                        throw new NotImplementedException("Only GCCHigh and Commercial available.");
                        break;
                }
            }
        }

        /// <summary>
        /// Method to get the valid Graph endpoint URL given the AzureEnvironment.
        /// </summary>
        public static string GraphEndPointUrl
        {
            get
            {
                switch (GetAzureEnvironment(ActiveTenant.AzureEnvironment))
                {
                    case AzureEnvironment.USGovGCCHigh:
                        return $"https://graph.microsoft.us/v1.0";
                        break;
                    case AzureEnvironment.Commercial:
                        return $"https://graph.microsoft.com/v1.0";
                    default:
                        throw new NotImplementedException("Only GCCHigh and Commercial available.");
                        break;
                }
            }
        }

        /// <summary>
        /// Method to return the users endpoint for the given Graph context.
        /// </summary>
        public static string GraphUserEndPointUrl
        {
            get
            {
                return $"{GraphEndPointUrl}/users";
            }
        }

        /// <summary>
        /// The assembly used on out calls.
        /// </summary>
        private static System.Reflection.Assembly assembly;

        /// <summary>
        /// The private object used to manage locks on file I/O.
        /// </summary>
        private static readonly object LockManager = new object();
        #endregion Properties

        #region InitializeTenant
        /// <summary>
        /// Initialization method for tenant configuration.
        /// </summary>
        /// <param name="tenant">The name of the tenant to initialize.</param>
        public static void InitializeTenant(string tenant)
        {
            try
            {
                Inf($"Initializing Tenant [{tenant}]");
                //Only initialize the tenant if it hasn't already been done.
                if (!Tenants.ContainsKey(tenant))
                {
                    Inf($"Loading config from [{GetRunFolder() + "\\" +
                        $"UniversalConfig.{tenant}.json"}].");
                    using (StreamReader sr = new StreamReader(
                        GetRunFolder() + "\\" + $"UniversalConfig.{tenant}.json"))
                    {
                        Tenants.Add(
                            tenant,
                            JsonSerializer.Deserialize<TenantConfig>(
                                sr.ReadToEnd()));
                        ActiveTenant = Tenants[tenant];
                    }
                    Inf($"Done initializing Tenant [{tenant}]");
                }
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                throw;
            }
        }
        #endregion InitializeTenant

        #region LogitCore
        /// <summary>
        /// Method to write error messages to console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public static void E(string message)
        {
            L(message, ConsoleColor.Red, ConsoleColor.Black);
        }

        /// <summary>
        /// Method to write warning messages to console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public static void W(string message)
        {
            L(message, ConsoleColor.Yellow, ConsoleColor.Black);
        }

        /// <summary>
        /// Method to write information messages to console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public static void I(string message)
        {
            L(message, ConsoleColor.Gray, ConsoleColor.Black);
        }

        /// <summary>
        /// Method to write verbose messages to console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public static void V(string message)
        {
            L(message, ConsoleColor.Cyan, ConsoleColor.Black);
        }

        /// <summary>
        /// Method to write a message to console in specified colors.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreground">The foreground color to use.</param>
        /// <param name="background">The background color to use.</param>
        public static void L(string message,
                             ConsoleColor foreground = ConsoleColor.Gray,
                             ConsoleColor background = ConsoleColor.Black)
        {
            //Save the current console colors.
            var foreColor = Console.ForegroundColor;
            var backColor = Console.BackgroundColor;
            //Check if an override foreground color was specified.
            if (foreColor != foreground)
            {
                Console.ForegroundColor = foreground;
            }
            //Check if an override background color was specified.
            if (backColor != background)
            {
                Console.BackgroundColor = background;
            }
            //Simply write the message to the console.
            Console.WriteLine(message);
            //Reset console colors.
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
        }
        #endregion LogitCore

        #region GetCVersion()
        /// <summary>
        /// Method to get the CVersion.
        /// </summary>
        /// <param name="filePath">Path to the json file containing the 
        /// version.</param>
        /// <returns>The CVersion.</returns>
        public static string GetCVersion(string filePath = null)
        {
            try
            {
                if (filePath == null)
                {
                    filePath = "version.json";
                }
                string json = "";
                using (FileStream fs = new FileStream(filePath,
                                                      FileMode.Open,
                                                      FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        json = sr.ReadToEnd().ToString();
                    }
                }
                JsonNode node = JsonNode.Parse(json);
                return "CVersion-" + node["cversion"].ToString();
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                return ex.ToString();
            }
            finally
            {
            }
        }

        /// <summary>
        /// Method to get the CVersion from IConfiguration.
        /// </summary>
        /// <param name="config">Instance of IConfiguration.</param>
        /// <returns>The CVersion.</returns>
        public static string GetCVersion(
            Microsoft.Extensions.Configuration.IConfiguration config)
        {
            try
            {
                return "CVersion-" + config.GetSection(
                    "AzureFunctionsJobHost:cversion").Value;
            }
            catch (Exception ex)
            {
                Wrn(ex.ToString());
                return GetCVersion();
            }
            finally
            {
            }
        }
        #endregion

        #region GetExecutingAssembly...()
        /// <summary>
        /// Gets the current assembly through reflection.
        /// </summary>
        /// <returns>The current Entry or Executing assembly.</returns>
        public static System.Reflection.Assembly GetExecutingAssembly()
        {
            return System.Reflection.Assembly.GetEntryAssembly() == null ?
                System.Reflection.Assembly.GetExecutingAssembly() :
                System.Reflection.Assembly.GetEntryAssembly();
        }

        /// <summary>
        /// Gets the name of the current assembly.
        /// </summary>
        /// <param name="asm">Out parm to hold the assembly.</param>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the name of the current assembly, optionally 
        /// escaped.</returns>
        private static string GetExecutingAssemblyName(
            out System.Reflection.Assembly asm,
            bool escaped = false)
        {
            asm = GetExecutingAssembly();
            string result = asm.ManifestModule.Name;
            if (escaped)
            {
                return System.Uri.EscapeDataString(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the name of the current assembly.
        /// </summary>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the name of the current assembly, optionally 
        /// escaped.</returns>
        public static string GetExecutingAssemblyName(bool escaped = false)
        {
            return GetExecutingAssemblyName(out assembly, escaped);
        }

        /// <summary>
        /// Gets the folder path of the current assembly.
        /// </summary>
        /// <param name="asm">Out parm to hold the assembly.</param>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the folder path of the current assembly, 
        /// optionally escaped.</returns>
        private static string GetExecutingAssemblyFolder(
            out System.Reflection.Assembly asm,
            bool escaped = false)
        {
            asm = GetExecutingAssembly();
            string result = System.IO.Path.GetDirectoryName(asm.Location)
                .TrimEnd('\\');
            if (escaped)
            {
                return System.Uri.EscapeDataString(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the folder path of the current assembly.
        /// </summary>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the folder path of the current assembly, 
        /// optionally escaped.</returns>
        public static string GetExecutingAssemblyFolder(bool escaped = false)
        {
            return GetExecutingAssemblyFolder(out assembly, escaped);
        }

        /// <summary>
        /// Get the location where the assembly stack started executing.
        /// </summary>
        /// <returns>The folder path.</returns>
        public static string GetRunFolder()
        {
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetEntryAssembly()
                .Location.TrimEnd('\\'));  //Ensure no trailing slash.
        }

        /// <summary>
        /// Gets the full path and file name of the current assembly.
        /// </summary>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the full path and file name of the current
        /// assembly, optionally escaped.</returns>
        public static string GetExecutingAssemblyFullPath(bool escaped = false)
        {
            string result = GetExecutingAssemblyFolder(out assembly) + "\\" +
                assembly.ManifestModule.Name;
            if (escaped)
            {
                return System.Uri.EscapeDataString(result);
            }
            return result;
        }
        #endregion GetExecutingAssembly...()

        #region GetFQDN()
        /// <summary>
        /// Returns the current computer Fully Qualified Domain Name.
        /// </summary>
        /// <returns>Returns the current computer Fully Qualified Domain Name.</returns>
        public static string GetFQDN()
        {
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = System.Net.Dns.GetHostName();
            if (domainName == "")
            {
                return hostName;
            }
            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))
            {
                hostName = domainName;
            }
            return hostName;
        }
        #endregion GetFQDN()

        #region Null
        /// <summary>
        /// Method to ensure a given object is not null.
        /// </summary>
        /// <param name="str">The object to check.</param>
        /// <returns>The string value of the object.</returns>
        public static string NoNull(object str)
        {
            if (str == null)
            {
                return "";
            }
            return str.ToString();
        }

        /// <summary>
        /// Method to ensure a value in a dictionary is not null.
        /// </summary>
        /// <param name="dict">The dictionary containing the variable.</param>
        /// <param name="fieldName">The name of the variable to check.</param>
        /// <returns></returns>
        public static string NoNull(IDictionary<string, object> dict,
                                    string fieldName)
        {
            try
            {
                return dict[fieldName] as string;
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
            }
        }

        /// <summary>
        /// Extension method for object array to validate all object are not
        /// null.
        /// </summary>
        /// <param name="objects">The array of objects to check.</param>
        /// <returns>True if any object in the array is null, else false.</returns>
        public static bool AnyNull(this object[] objects)
        {
            foreach (object obj in objects)
            {
                if (obj == null)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion Null

        #region printf()
        /// <summary>
        /// Simple printf method for console output with color control.  Both
        /// text color and background color is returned to previous state
        /// after the string has been written to console.
        /// </summary>
        /// <param name="msg">String to print to console.</param>
        /// <param name="foreground">Overrideable text color, default to white.</param>
        /// <param name="background">Overrideable background color, default to
        /// black.</param>
        public static void printf(object msg, 
                                  ConsoleColor foreground = ConsoleColor.White, 
                                  ConsoleColor background = ConsoleColor.Black)
        {
            ConsoleColor fore = Console.ForegroundColor;
            ConsoleColor back = Console.BackgroundColor;
            if (foreground != fore)
            {
                Console.ForegroundColor = foreground;
            }
            if (background != back)
            {
                Console.BackgroundColor = background;
            }
            Console.WriteLine(Convert.ToString(msg));
            if (foreground != fore)
            {
                Console.ForegroundColor = fore;
            }
            if (background != back)
            {
                Console.BackgroundColor = back;
            }
        }
        #endregion printf()

        #region TimeStamp()
        /// <summary>
        /// Returns the current local date time stamp as a string in either
        /// "YYYY-MM-DD" or "YYYY-MM-DD@hh.mm.ss.nnn" format.
        /// </summary>
        /// <param name="DateOnly">If true, return the current local date
        /// time stamp as "YYYY-MM-DD" else return it as 
        /// "YYYY-MM-DD@hh.mm.ss.nnn"</param>
        /// <returns>Returns the current local date time stamp as a string in
        /// either "YYYY-MM-DD" or "YYYY-MM-DD@hh.mm.ss.nnn" format.</returns>
        public static string TimeStamp(bool DateOnly = false)
        {
            System.DateTime now = System.DateTime.Now;
            if (DateOnly)
            {
                return now.Year.ToString() + "-" +
                    now.Month.ToString("d2") + "-" +
                    now.Day.ToString("d2");
            }
            else
            {
                return now.Year.ToString() + "-" +
                    now.Month.ToString("d2") + "-" +
                    now.Day.ToString("d2") + "@" +
                    now.Hour.ToString("d2") + "." +
                    now.Minute.ToString("d2") + "." +
                    now.Second.ToString("d2") + "." +
                    now.Millisecond.ToString("d3");
            }
        }
        #endregion TimeStamp

        #region Validate()
        /// <summary>
        /// Makes quick work of null validating all parameters you pass to it.
        /// This method takes a variable number of parameters and validates that
        /// all parameters are not null.  If a parameter is found to be null, a
        /// ArgumentNullException is thrown.
        /// For example:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.ValidateNoNulls(str, dbl, cls);
        ///         ...Your code here...
        ///     }
        /// You do not have to pass all parameters, but can instead do this:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.ValidateNoNulls(str, cls);
        ///         ...Your code here...
        ///     }
        /// where we chose NOT to validate the double dbl in this case.
        /// </summary>
        /// <param name="parms">The variable set of parameters.</param>
        public static bool ValidateNoNulls(params object[] parms)
        {
            for (int C = 0; C < parms.Length; C++)
            {
                if (parms[C] == null)
                {
                    throw new ArgumentNullException("Parameter #" + C);
                }
            }
            return true;
        }

        /// <summary>
        /// Makes quick work of validating all parameters you pass to it.
        /// This method takes a variable number of parameters and validates 
        /// all parameters based on ErrorType and object type.  Null validation
        /// is seamless.  If a parameter is found to be null, a
        /// ArgumentNullException is thrown which notes the number of the 
        /// parameter since parmeters are treated as objects and thus the
        /// parameter names are inaccessible.
        /// For example:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.Validate(ErrorType.Null, str, dbl, cls);
        ///         ...Your code here...
        ///     }
        /// You do not have to pass all parameters, but can instead do this:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.Validate(ErrorType.Null, str, cls);
        ///         ...Your code here...
        ///     }
        /// where we chose NOT to validate the double dbl in this case.
        /// Alternatively, you can validate that dbl is a non-negative
        /// number by doing this:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.Validate(
        ///             {ErrorType.Null, ErrorType.NonNegative}, 
        ///             str, cls);
        ///         ...Your code here...
        ///     }
        /// </summary>
        /// <param name="errors">The array of error types to validate.</param>
        /// <param name="parms">The variable set of parameters.</param>
        public static bool Validate(Constants.ErrorType[] errors,
                                    params object[] parms)
        {
            foreach (Constants.ErrorType error in errors)
            {
                for (int C = 0; C < parms.Length; C++)
                {
                    switch (error)
                    {
                        case Constants.ErrorType.Null:
                            if (parms[C] == null)
                            {
                                throw new ArgumentNullException("Parameter #" + C);
                            }
                            break;
                        case Constants.ErrorType.IntNonNegative:
                            if (parms[C].GetType() == typeof(int))
                            {
                                if ((int)parms[C] < 0)
                                {
                                    throw new ArgumentOutOfRangeException(
                                        "Parameter #" + C,
                                        "Value must be >= 0");
                                }
                            }
                            break;
                    }
                }
            }
            return true;
        }
        #endregion Validate()

        #region EnvironmentVariables
        /// <summary>
        /// A method to get an EnvironmentVariable value.
        /// </summary>
        /// <param name="key">The target variable name.</param>
        /// <returns>The value of the EnvironmentVariable or.</returns>
        public static string GetEnv(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        /// <summary>
        /// A method to set an EnvironmentVariable value.
        /// </summary>
        /// <param name="target">The variable to set.</param>
        /// <param name="value">The value to which to set the variable.</param>
        public static void SetEnv(string target, string value)
        {
            Environment.SetEnvironmentVariable(target, value);
        }
        #endregion EnvironmentVariables

        #region GetAzureEnvironment
        /// <summary>
        /// Method to return the AzureEnvironment based on the given string.
        /// </summary>
        /// <param name="env">The environment to get.</param>
        /// <returns>The enum value of the environment.</returns>
        public static AzureEnvironment GetAzureEnvironment(string env)
        {
            if (env.ToLower().Contains("gcchigh"))
            {
                return AzureEnvironment.USGovGCCHigh;
            }
            if (env.ToLower().Contains("china"))
            {
                return AzureEnvironment.China;
            }
            if (env.ToLower().Contains("germany"))
            {
                return AzureEnvironment.Germany;
            }
            if (env.ToLower().Contains("gcc"))
            {
                return AzureEnvironment.USGovGCC;
            }
            if (env.ToLower().Contains("dod"))
            {
                return AzureEnvironment.USGovGCC;
            }
            return AzureEnvironment.Commercial;
        }
        #endregion GetAzureEnvironment

        #region GetUser
        /// <summary>
        /// Method to get the user given a user lookup id from SharePoint.
        /// </summary>
        /// <param name="id">The SharePoint list id of the user.</param>
        /// <param name="siteUsers">A reference to the list of users.</param>
        /// <returns>The user associated with the id or null.</returns>
        public static Microsoft.Graph.ListItem GetUserFromLookupId(
            string id,
            ref List<Microsoft.Graph.ListItem> siteUsers)
        {
            foreach (var item in siteUsers)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Method to get the user's UPN given a SharePoint lookup id.
        /// </summary>
        /// <param name="id">The lookup id of the user.</param>
        /// <param name="siteUsers">A reference to the list of users.</param>
        /// <returns></returns>
        public static string GetUserEmailUPN(
            string id,
            ref List<Microsoft.Graph.ListItem> siteUsers)
        {
            var item = GetUserFromLookupId(id, ref siteUsers);
            if (item == null)
            {
                return "";
            }
            try
            {
                if (item.Fields.AdditionalData["EMail"] != null)
                {
                    return item.Fields.AdditionalData["EMail"].ToString();
                }
                return item.Fields.AdditionalData["UserName"].ToString();
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                return item.Fields.AdditionalData["UserName"].ToString();
            }
            finally
            {
            }
        }
        #endregion GetUser

        #region TryAdd
        /// <summary>
        /// Extension method for Dictionary to add an item if it doesn't
        /// already exist in the dictionary.
        /// </summary>
        /// <param name="dic">The dictionary to which the instance should be
        /// added.</param>
        /// <param name="key">The key to use for the instance.</param>
        /// <param name="val">The value to add to the dictionary.</param>
        /// <returns>True if add successful, false if not.</returns>
        public static bool TryAdd(
            this Dictionary<object, object> dic,
            object key,
            object val)
        {
            try
            {
                if (!dic.Values.Contains(val))
                {
                    dic.Add(key, val);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Extension method for List to add a string item if it doesn't
        /// already exist in the list.
        /// </summary>
        /// <param name="lst">The list to which the object should be added.</param>
        /// <param name="obj">The string to add to the list.</param>
        /// <returns>True if add successful, false if not.</returns>
        public static bool TryAdd(
            this List<string> lst,
            string obj)
        {
            try
            {
                if (!lst.Contains(obj))
                {
                    lst.Add(obj);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Extension method for List to add an int item if it doesn't
        /// already exist in the list.
        /// </summary>
        /// <param name="lst">The list to which the object should be added.</param>
        /// <param name="obj">The int to add to the list.</param>
        /// <returns>True if add successful, false if not.</returns>
        public static bool TryAdd(
            this List<int> lst,
            int obj)
        {
            try
            {
                if (!lst.Contains(obj))
                {
                    lst.Add(obj);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion TryAdd
    }
}

//#pragma warning restore CS0162, CS0168//, CS1587, CS1998, IDE0028, IDE0059
