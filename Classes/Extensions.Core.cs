/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.SharePoint.News.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using Extensions.Identity;
using Extensions.Tenant;
using static Extensions.Identity.Auth;
using static Extensions.Identity.AuthMan;
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
        public static TenantConfig ActiveTenant { get; set; } = null;

        /// <summary>
        /// TODO
        /// </summary>
        public static Config config { get; set; } = new Config();

        ///// <summary>
        ///// The tenant name e.g. for contoso.sharepoint.us it would be "contoso".
        ///// </summary>
        //public static string TenantUrl { get; }
        //    = $"{ActiveTenant.TenantString}.sharepoint.us";

        ///// <summary>
        ///// The URI of the tenant.
        ///// </summary>
        //public static Uri TenantUri { get; }
        //    = new Uri("https://" + TenantUrl);

        ///// <summary>
        ///// Method to get the valid Authority URL given the AzureEnvironment.
        ///// </summary>
        //public static string Authority
        //{
        //    get
        //    {
        //        switch (GetAzureEnvironment(ActiveTenant.AzureEnvironment))
        //        {
        //            case AzureEnvironment.USGovGCCHigh:
        //                return $"https://login.microsoftonline.us/{TenantUrl}";
        //                break;
        //            case AzureEnvironment.Commercial:
        //                return $"https://login.microsoftonline.com/{TenantUrl}";
        //                break;
        //            default:
        //                throw new NotImplementedException("Only GCCHigh and Commercial available.");
        //                break;
        //        }
        //    }
        //}

        /// <summary>
        /// Method to get the valid Graph endpoint URL given the AzureEnvironment.
        /// </summary>
        public static string GraphEndPointUrl
        {
            get
            {
                switch (ActiveTenant.AzureEnvironment)
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
        /// <param name="tenantString">The name of the tenant to initialize.</param>
        public static void InitializeTenant(string tenantString)
        {
            try
            {
                Inf($"Initializing Tenant [{tenantString}]");
                //Only initialize the tenant if it hasn't already been done.
                if ((!Tenants.ContainsKey(tenantString)) ||
                    (Tenants[tenantString].Settings == null))
                {
                    CoreBase.TenantString = tenantString;
                    LoadConfig(tenantString);
                    string[] sp = Scopes.SharePoint;
                    sp[0] = sp[0].Replace("Contoso", CoreBase.TenantString);
                    Scopes.SharePoint = sp;
                    GetAuth(GetEnv("TenantDirectoryId"),
                            GetEnv("ApplicationClientId"),
                            GetEnv("CertThumbprint"),
                            tenantString);
                    Tenants.Add(
                        tenantString,
                        ActiveAuth.TenantCfg);
                }
                else
                {
                    Inf($"Tenant [{tenantString}] is already in the pool.  " +
                        $"Using pool instance.");
                    LoadConfig(tenantString);
                    GetAuth();
                }
                ActiveTenant = Tenants[tenantString];
                Inf($"Initializing Tenant [{tenantString}] complete.");
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Internal method to add an Environment Variable (usually from the
        /// settings of an Azure Function) to the active config.
        /// </summary>
        /// <param name="key">The name of the EV to add to config.</param>
        internal static void AddEnvSetting(string key)
        {
            config.Settings.Add(key, GetEnv(key));
        }

        /// <summary>
        /// Internal method to add multiple Environment Variables (usually from
        /// the settings of an Azure Function) to the active config.
        /// </summary>
        /// <param name="keys">A string array containing the EVs to add to
        /// the active config.</param>
        internal static void AddEnvSetting(string[] keys)
        {
            foreach (string key in keys)
            {
                try
                {
                    config.Settings.Add(key, GetEnv(key));
                }
                catch (Exception ex)
                {
                    Err(ex.ToString());
                    throw new Exception($"The key [{key}] does not exist as an" +
                        $"environment variable.\n\n" + ex.ToString());
                }
            }
        }


        /// <summary>
        /// An internal method for loading environment variables from JSON
        /// config files when interactively debugging.
        /// </summary>
        /// <param name="tenantString">The name of the target tenant.</param>
        internal static void LoadConfig(string tenantString)
        {
            config.Settings = new Dictionary<string, string>();
            config.Settings.Add("TenantString", tenantString);
            if (Environment.UserInteractive)
            {
                Inf($"Loading config from [" +
                    $"{GetRunFolder()}]\\{GetConfigFileName(tenantString)}].");
                config.Settings = LoadJSON(
                    $"{Core.GetRunFolder()}" +
                    $"\\{GetConfigFileName(tenantString)}");
                Inf($"Loading sensitivity labels from [" +
                    $"{GetRunFolder()}]\\{GetLabelsFileName(tenantString)}].");
                config.Labels = LoadJSON(
                    $"{Core.GetRunFolder()}" +
                    $"\\{GetLabelsFileName(tenantString)}");
                Inf("Config and Labels loaded.");
            }
            else
            {
                //AddEnvSetting(
                //    "TenantDirectoryId," +
                //    "ApplicationClientId," +
                //    "AzureEnvironment," +
                //    "ConnectionString," +
                //    "DefaultBlobContainer," +
                //    "EmailSendingAccount," +
                //    "CertStoreLocation," +
                //    "CertThumbprint," +
                //    "DebugEnabled," +
                //    "MultiThreaded," +
                //    "LogitSiteBaseUrl," +
                //    "LogitSiteId," +
                //    "LogitDefaultListGuid," +
                //    "HomeSiteBaseUrl".Split(
                //        ',', 
                //        StringSplitOptions..TrimEntries |
                //            StringSplitOptions.RemoveEmptyEntries));
            }
        }

        /// <summary>
        /// Internal method to get the name of the universal config file
        /// based on the tenant name.
        /// </summary>
        /// <param name="tenantString">The name of the Tenant e.g. for the
        /// contoso.sharepoint.us tenant the value would be "contoso".</param>
        /// <returns>The name of a JSON file for the current Tenant.</returns>
        internal static string GetConfigFileName(string tenantString)
        {
            return $"UniversalConfig.{tenantString}.json";
        }

        /// <summary>
        /// Internal method to get the name of the sensitivity labels file
        /// based on the tenant name.
        /// </summary>
        /// <param name="tenantString">The name of the Tenant e.g. for the
        /// contoso.sharepoint.us tenant the value would be "contoso".</param>
        /// <returns>The name of a JSON file for the current Tenant.</returns>
        internal static string GetLabelsFileName(string tenantString)
        {
            return $"Labels.{tenantString}.json";
        }

        /// <summary>
        /// An internal method that loads a given file and attempts to 
        /// deserialize it to a Dictionary of string,string object for return.
        /// </summary>
        /// <param name="filePath">The full path and file name of the target
        /// file to load.</param>
        /// <returns>A Dictionary of string,string values or an empty 
        /// Dictionary if an error occus.</returns>
        internal static Dictionary<string, string> LoadJSON(string filePath)
        {
            var result = new Dictionary<string, string>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    result = JsonSerializer.Deserialize<
                        Dictionary<string, string>>(sr.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                //If something goes wrong while reading the file, we simply
                //return a blank dictionary by swallowing the error and logging
                //the exception.
                Err(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// A public method to allow settings values to be retrieved.
        /// </summary>
        /// <param name="key">The name of the setting value to retrieve.</param>
        /// <returns>If the requested setting exist in the config, its value is
        /// returned else a blank string is returned.</returns>
        public static string GetSetting(string key)
        {
            if (config.Settings.ContainsKey(key))
            { 
                return config.Settings[key]; 
            }
            return "";
        }
        #endregion InitializeTenant

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

        #region GetExecutingAssembly()
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
        /// A public method to retrieve the Tenant ID.
        /// </summary>
        /// <param name="tenantString">The Tenant name e.g. for 
        /// "contoso.sharepoint.us" it would be "contoso".</param>
        /// <returns>The Tenant ID.</returns>
        public static string GetTenantId(string tenantString)
        {
            var az = GetAuthorityDomain(GetAzureEnvironment("GCCHigh"));
            var http = new HttpClient();
            var res = http.GetAsync(
                $"https://login.microsoftonline{az}/{tenantString}" +
                $".onmicrosoft{az}/v2.0/.well-known" +
                $"/openid-configuration").GetAwaiter().GetResult();
            var raw = res.Content.ReadAsStringAsync().Result;
            var json = JsonSerializer.Deserialize<OpenId>(raw);
            return json.authorization_endpoint.ToLower()
                .Replace($"https://login.microsoftonline{az}/", "")
                .Replace("/oauth2/v2.0/authorize", "");
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
        #endregion GetExecutingAssembly()

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
                if (dict.ContainsKey(fieldName))
                {
                    return dict[fieldName].ToString();
                }
                else
                {
                    return "";
                }
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
            if (Environment.GetEnvironmentVariable(key) != null)
            {
                return Environment.GetEnvironmentVariable(key);
            }
            return GetSetting(key);
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

        #region GetAuthorityDomain
        /// <summary>
        /// A public method to get the domain extension.
        /// </summary>
        /// <param name="azureEnvironment">The name of the Azure environment 
        /// type.</param>
        /// <returns>".us" if the environment is USGovGCCHigh otherwise it
        /// will return ".com".</returns>
        public static string GetAuthorityDomain(
            AzureEnvironment azureEnvironment = AzureEnvironment.USGovGCCHigh)
        {
            if (azureEnvironment == AzureEnvironment.USGovGCCHigh)
            {
                return ".us";
            }
            return ".com";
        }
        #endregion GetAuthorityDomain

        #region Group
        /// <summary>
        /// A private method to get a group of list of groups where the 
        /// displayName matches the given group name.
        /// </summary>
        /// <param name="groupName">The displayName of the target group.</param>
        /// <returns>A List of Group or null if none found.</returns>
        private static List<Group> GetGroupByName(string groupName)
        {
            if (NoNull(groupName) != "")
            {
                return ActiveAuth.GraphClient.Groups.GetAsync((C) =>
                {
                    C.QueryParameters.Filter = $"displayName eq '{groupName}'";
                    C.Headers.Add("ConsistencyLevel", "eventual");
                }).GetAwaiter().GetResult().Value;
            }
            return new List<Group>();
        }

        /// <summary>
        /// A private method to retrieve all the Members of a Group.
        /// </summary>
        /// <param name="groupName">The displayName of the target Group.</param>
        /// <returns>A List of DirectoryObject values that represens all the 
        /// members of the Group.</returns>
        private static List<DirectoryObject> GetGroupMembers(string groupName)
        {
            List<DirectoryObject> members = new List<DirectoryObject>();
            var groups = GetGroupByName(groupName);
            if (groups.Count > 0)
            {
                var page = ActiveAuth.GraphClient.Groups[groups[0].Id].Members
                    .GetAsync().GetAwaiter().GetResult();
                var pageIterator =
                    PageIterator<DirectoryObject,
                                 DirectoryObjectCollectionResponse>
                    .CreatePageIterator(
                        ActiveAuth.GraphClient,
                        page,
                        (member) =>
                        {
                            members.Add(member);
                            return true;
                        });
                pageIterator.IterateAsync().GetAwaiter().GetResult();
            }
            return members;
        }

        /// <summary>
        /// A private method to retrieve all the Owners of a Group.
        /// </summary>
        /// <param name="groupName">The displayName of the target Group.</param>
        /// <returns>A List of DirectoryObject values that represens all the 
        /// owners of the Group.</returns>
        private static List<DirectoryObject> GetGroupOwners(string groupName)
        {
            List<DirectoryObject> owners = new List<DirectoryObject>();
            var groups = GetGroupByName(groupName);
            if (groups.Count > 0)
            {
                var page = ActiveAuth.GraphClient.Groups[groups[0].Id].Owners
                    .GetAsync().GetAwaiter().GetResult();
                var pageIterator =
                    PageIterator<DirectoryObject,
                                 DirectoryObjectCollectionResponse>
                    .CreatePageIterator(
                        ActiveAuth.GraphClient,
                        page,
                        (owner) =>
                        {
                            owners.Add(owner);
                            return true;
                        });
                pageIterator.IterateAsync().GetAwaiter().GetResult();
            }
            return owners;
        }
        #endregion Group

        #region GetUser
        /// <summary>
        /// Method to get the user given a user lookup id from SharePoint.
        /// </summary>
        /// <param name="id">The SharePoint list id of the user.</param>
        /// <param name="siteUsers">A reference to the list of users.</param>
        /// <returns>The user associated with the id or null.</returns>
        public static Microsoft.Graph.Models.ListItem GetUserFromLookupId(
            string id,
            ref List<Microsoft.Graph.Models.ListItem> siteUsers)
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
        /// Method to get a user's EMail or UserName.  Particulary useful for
        /// translating SharePoint UserLookupIds like LastModified or Author
        /// from a numeric value to the UPN string.
        /// </summary>
        /// <param name="listItem">A ListItem instance from the site's 
        /// UserInformation list.</param>
        /// <returns>The EMail or UserName value from a ListItem instance 
        /// from the UserInformation list.
        /// </returns>
        public static string GetUserEmailUpn(
            Microsoft.Graph.Models.ListItem listItem)
        {
            if (listItem.Fields.AdditionalData.Keys.Contains("EMail") &&
                listItem.Fields.AdditionalData["EMail"] != null)
            {
                return listItem.Fields.AdditionalData["EMail"].ToString();
            }
            if (listItem.Fields.AdditionalData.Keys.Contains("EMail") &&
                listItem.Fields.AdditionalData["EMail"] != null)
            {
                return listItem.Fields.AdditionalData["UserName"].ToString();
            }
            return "";
        }

        public static string GetUserEmailUpn(
            string id,
            GraphServiceClient client)
        {
            try
            {
                if ((id == null) ||
                    (id == ""))
                {
                    return "";
                }
                var userListItems = GetListItems(
                    "User Information List",
                    GetEnv("HomeSiteBaseUrl"),
                    id);
                if ((userListItems != null) &&
                    (userListItems.Count > 0) &&
                    (userListItems[0].Fields.AdditionalData.Keys.Contains("EMail")) &&
                    (userListItems[0].Fields.AdditionalData["EMail"] != null))
                {
                    return userListItems[0].Fields.AdditionalData["EMail"].ToString();
                }
                return userListItems[0].Fields.AdditionalData["UserName"].ToString();
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                return "";
            }
            finally
            { 
            }
        }

        /// <summary>
        /// Method to get the user's UPN given a SharePoint lookup id.  
        /// Particulary useful for translating SharePoint UserLookupIds like 
        /// LastModified or Author from a numeric value to the UPN string.
        /// </summary>
        /// <param name="id">The lookup id of the user.</param>
        /// <param name="siteUsers">A reference to the list of users.</param>
        /// <returns>The EMail or UserName value from a ListItem instance 
        /// from the UserInformation list.</returns>
        public static string GetUserEmailUpn(
            string id,
            ref List<Microsoft.Graph.Models.ListItem> siteUsers)
        {
            var item = GetUserFromLookupId(id, ref siteUsers);
            if (item == null)
            {
                return "";
            }
            try
            {
                if ((item.Fields.AdditionalData.Keys.Contains("EMail")) &&
                    (item.Fields.AdditionalData["EMail"] != null))
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

        /// <summary>
        /// Method to write out all setting values in both the ActiveAuth's
        /// tenant configuration as well as the Core configuration values.
        /// </summary>
        public static void DumpSettings()
        {
            Inf("Dumping ActiveAuth settings...");
            foreach (var setting in ActiveAuth.TenantCfg.Settings)
            {
                Inf($"[{setting.Key}] = [{setting.Value}]");
            }
            Inf("Dumping Core settings...");
            foreach (var setting in Extensions.Core.config.Settings)
            {
                Inf($"[{setting.Key}] = [{setting.Value}]");
            }
        }

        /// <summary>
        /// Method to get a Graph List object given the list name and the path
        /// of the containing site."
        /// </summary>
        /// <param name="listName">The name of the target List.</param>
        /// <param name="sitePath">The relative path of the site containing
        /// the target list e.g. "/sites/Extensions".</param>
        /// <returns>A Graph List object</returns>
        public static Microsoft.Graph.Models.List GetList(
            string listName, 
            string sitePath)
        {
            return ActiveAuth.GraphClient.Sites[Graph.GetSiteId(sitePath)]
                .Lists[Graph.GetListId(listName, sitePath)]
                .GetAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// A method to get all list items given a list name, the path of the
        /// containing site and optionally a filter or ID.  If an ID value is
        /// provided, a list containing a single item is returned.
        /// </summary>
        /// <param name="listName">The name of the target list.</param>
        /// <param name="sitePath">The path to the site containing said target
        /// list e.g. "/sites/Extensions".</param>
        /// <param name="id">An optional ID of a specific item in the 
        /// list.</param>
        /// <param name="filter">An optional filter to be used during selection
        /// of the resulting data set e.g. "name = 'Extensions'".</param>
        /// <returns>A list of items for the target list and containing site.
        /// If no filter or ID is provided, all items in the list is returned.
        /// If a filter is provided, the items conforming to the filter is
        /// returned.  If an ID is provided, the target item is returned.
        /// </returns>
        public static List<ListItem> GetListItems(
            string listName,
            string sitePath,
            string id = null,
            string filter = null)
        {
            return Graph.GetListItems(listName, sitePath, id, filter);
        }
    }
}
