/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using Extensions.Identity;
using static Extensions.Constants;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using System.Linq;

namespace Extensions
{
    /// <summary>
    /// Core class for constants, enums and helper methods.
    /// </summary>
    [Serializable]
    public static partial class Core
    {
        #region Properties
        /// <summary>
        /// A dictionary of configurations for different tenants.
        /// </summary>
        public static Dictionary<string, TenantConfig> Tenants { get; private set; }
            = new Dictionary<string, TenantConfig>();

        /// <summary>
        /// The currently active tenant configuration in use.
        /// </summary>
        public static TenantConfig ActiveTenant
        {
            get
            {
                if (ActiveAuth == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    AuthMan.GetAuth(tenantConfig.TenantDirectoryId,
                                    tenantConfig.ApplicationClientId,
                                    tenantConfig.CertThumbprint,
                                    tenantConfig.TenantString);
                }
                return ActiveAuth.TenantCfg;
            }
            set { }
        }

        /// <summary>
        /// The currently active Auth object from the stack.
        /// </summary>
        public static Auth ActiveAuth
        {
            get
            {
                if (AuthMan.ActiveAuth == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    AuthMan.GetAuth(tenantConfig.TenantDirectoryId,
                                    tenantConfig.ApplicationClientId,
                                    tenantConfig.CertThumbprint,
                                    tenantConfig.TenantString);
                }
                return AuthMan.ActiveAuth;
            }
        }

        /// <summary>
        /// The currently active Auth's authority domain.
        /// </summary>
        public static string AuthorityDomain
        {
            get
            {
                if (ActiveAuth == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    AuthMan.GetAuth(tenantConfig.TenantDirectoryId,
                                    tenantConfig.ApplicationClientId,
                                    tenantConfig.CertThumbprint,
                                    tenantConfig.TenantString);
                }
                return ActiveAuth.TenantCfg.AuthorityDomain;
            }
        }

        /// <summary>
        /// Method to return the users endpoint for the given Graph context.
        /// </summary>
        public static string GraphUserEndPointUrl
        {
            get
            {
                if (ActiveAuth == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    AuthMan.GetAuth(tenantConfig.TenantDirectoryId,
                                    tenantConfig.ApplicationClientId,
                                    tenantConfig.CertThumbprint,
                                    tenantConfig.TenantString);
                }
                return ActiveAuth.TenantCfg.GraphUserEndPointUrl;
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

        #region Auth
        /// <summary>
        /// Method to get a matching Auth object from the stack or if it
        /// doesn't exist on the stack, generate the new Auth object and
        /// push it to the stack.
        /// </summary>
        /// <param name="scopeType">The scope type of the Auth.  Default value
        /// is Graph.</param>
        /// <param name="authStackReset">Boolean to trigger a clearing of the 
        /// Auth stack.  Default value is false.</param>
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetAuth(
            ScopeType scopeType = ScopeType.Graph,
            bool authStackReset = false)
        {
            if (ActiveAuth == null)
            {
                TenantConfig tenantConfig = new TenantConfig();
                tenantConfig.LoadConfig();
                AuthMan.GetAuth(tenantConfig.TenantDirectoryId,
                                tenantConfig.ApplicationClientId,
                                tenantConfig.CertThumbprint,
                                tenantConfig.TenantString);
            }
            return AuthMan.GetAuth(
                ActiveTenant.TenantDirectoryId,
                ActiveTenant.ApplicationClientId,
                ActiveTenant.CertThumbprint,
                ActiveTenant.TenantString,
                scopeType,
                authStackReset);
        }
        #endregion Auth

        #region ForEach
        /// <summary>
        /// A method to do parallel foreach processing in batches.  This is
        /// especially useful when the Action specified in body executes
        /// complex operations like making REST calls against big data sources
        /// e.g. having to call the /_api/web/ensureuser REST method in 
        /// SharePoint when validating 200,000 users will inevitably lead to
        /// thread timeouts since the CPU just can't handle that many parallel
        /// threads concurrently.
        /// </summary>
        /// <typeparam name="TSource">The type of objects contained in the
        /// collection being iterated.</typeparam>
        /// <param name="batchSize">The size of batches to process.  A safe
        /// value is roughly half the number of logical CPU cores in the host
        /// system.  If you're unsure, specify -1 and the method will
        /// automatically calculate the value for you.</param>
        /// <param name="source">The collection being iterated.</param>
        /// <param name="body">The Action being taken on each item in the 
        /// source.</param>
        /// <param name="debugOutput">An optional boolean switch that controls
        /// if debug output is produced.  This is useful for visual feedback
        /// when processing large collections, like that 200,000 users noted
        /// earlier.  Default is false i.e. no debug output is done.</param>
        /// <param name="exceptionOnError">An optional boolean switch that
        /// controls if execution is aborted when something goes wrong in the
        /// Parallel.ForEach call.  This should theoretically never 
        /// happen.  Defaults to false.</param>
        /// <returns>The ParallelLoopResult from the last batch that was
        /// processed.</returns>
        /// <exception cref="Exception">An optional exception is thrown if
        /// the exceptionOnError boolean switch is true and something goes 
        /// wrong in the Parallel.ForEach call.</exception>
        public static ParallelLoopResult ForEach<TSource>(
            int batchSize,
            IEnumerable<TSource> source,
            Action<TSource> body,
            bool debugOutput = false,
            bool exceptionOnError = false)
        {
            ParallelLoopResult result = new ParallelLoopResult();
            var clone = source.ToList();
            while (clone.Count > 0)
            {
                var batch = clone.TakeAndRemove(batchSize);
                result = Parallel.ForEach(batch, body);
                if (!result.IsCompleted)
                {
                    if (debugOutput)
                    {
                        Err("Parallel.ForEach failed!");
                        if (exceptionOnError)
                        {
                            throw new Exception("Parallel.ForEach failed!");
                        }
                    }
                }
                if (debugOutput)
                {
                    Inf($"[{clone.Count}] remain...");
                }
            }
            return result;
        }
        #endregion ForEach

        #region InitializeTenant
        /// <summary>
        /// Initialization method for tenant configuration.
        /// </summary>
        /// <param name="tenantString">The name of the tenant to initialize.</param>
        public static void InitializeTenant(string tenantString)
        {
            try
            {
                if (tenantString == null)
                {
                    throw new Exception("TenantString cannot be null.");
                }
                else
                {
                    Inf($"Initializing Tenant [{tenantString}]");
                    SetEnv("TenantString", tenantString);
                    TenantConfig tenantConfig = new TenantConfig(tenantString);
                    AuthMan.TargetTenantConfig = tenantConfig;
                    AuthMan.GetAuth(GetEnv("TenantDirectoryId"),
                                    GetEnv("ApplicationClientId"),
                                    GetEnv("CertThumbprint"),
                                    tenantString,
                                    ScopeType.Graph);
                    Logit.ActiveLogitInstance.GraphClient = AuthMan.GetGraphServiceClient();
                }
                Inf($"Initializing Tenant [{tenantString}] complete.");
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// A public method to allow settings values to be retrieved.
        /// </summary>
        /// <param name="key">The name of the setting value to retrieve.</param>
        /// <returns>If the requested setting exist in the config, its value is
        /// returned else a blank string is returned.</returns>
        public static string GetSetting(string key)
        {
            return TenantConfig.GetSetting(key);
        }
        #endregion InitializeTenant

        #region Logit
        /// <summary>
        /// Called to write "Information" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public static void Inf(
            string message,
            int eventId = 0,
            Logit.Instance instance = null)
        {
            Logit.Inf(message, eventId, instance);
        }

        /// <summary>
        /// Called to write "Warning" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public static void Wrn(
            string message,
            int eventId = 0,
            Logit.Instance instance = null)
        {
            Logit.Wrn(message, eventId, instance);
        }

        /// <summary>
        /// Called to write "Error" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public static void Err(
            string message,
            int eventId = 0,
            Logit.Instance instance = null)
        {
            Logit.Err(message, eventId, instance);
        }

        /// <summary>
        /// Called to write "Verbose" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public static void Vrb(
            string message,
            int eventId = 0,
            Logit.Instance instance = null)
        {
            Logit.Vrb(message, eventId, instance);
        }
        #endregion Logit

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
            if (GetEnv("RUNNING_IN_AZURE") == "True")
            {
                return @"C:\home\site\wwwroot";
            }
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
            var http = new HttpClient();
            var res = http.GetAsync(
                $"https://login.microsoftonline{AuthorityDomain}/{tenantString}" +
                $".onmicrosoft{AuthorityDomain}/v2.0/.well-known" +
                $"/openid-configuration").GetAwaiter().GetResult();
            var raw = res.Content.ReadAsStringAsync().Result;
            var json = JsonSerializer.Deserialize<Tenant.OpenId>(raw);
            return json.authorization_endpoint.ToLower()
                .Replace($"https://login.microsoftonline{AuthorityDomain}/", "")
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
                if (dict.TryGetValue(fieldName, out object result))
                {
                    return result.ToString();
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
        public static void Printf(object msg,
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
        /// A method to get an EnvironmentVariable value.  If not found, the
        /// variable is sought in Settings instead.
        /// </summary>
        /// <param name="key">The target variable name.</param>
        /// <returns>The value of the EnvironmentVariable or if not found, the
        /// return value of the GetSetting() method.</returns>
        public static string GetEnv(string key)
        {
            return TenantConfig.GetEnv(key);
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
        public static AzureEnvironmentName GetAzureEnvironment(string env)
        {
            if (env.ToLower().Contains("gcchigh"))
            {
                return AzureEnvironmentName.O365USGovGCCHigh;
            }
            if (env.ToLower().Contains("china"))
            {
                return AzureEnvironmentName.O365China;
            }
            if (env.ToLower().Contains("germany"))
            {
                return AzureEnvironmentName.O365GermanyCloud;
            }
            if (env.ToLower().Contains("gcc"))
            {
                return AzureEnvironmentName.O365Default;
            }
            if (env.ToLower().Contains("dod"))
            {
                return AzureEnvironmentName.O365USGovDoD;
            }
            return AzureEnvironmentName.O365Default;
        }
        #endregion GetAzureEnvironment

        #region Group
        /// <summary>
        /// A method to get the Group given its GUID ID.
        /// </summary>
        /// <param name="groupId">The Entra GUID ID of the target Group.</param>
        /// <returns>A Microsoft.Graph.Models.Group object if found, else
        /// null.</returns>
        public static Group GetGroup(string groupId)
        {
            return GroupExtensions.GetGroup(groupId);
        }

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
        public static List<DirectoryObject> GetGroupOwners(string groupName)
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

        #region GetHttpClient
        /// <summary>
        /// A method to return a valid HttpClient for the given 
        /// AuthenticationResult.
        /// </summary>
        /// <param name="authResult">The AuthenticationResult to use during
        /// HttpClient construction.</param>
        /// <returns>A valid HttpClient using the given 
        /// AuthenticationResult.</returns>
        public static HttpClient GetHttpClient(AuthenticationResult authResult)
        {
            return AuthMan.GetHttpClient(authResult);
        }
        #endregion GetHttpClient

        #region User
        /// <summary>
        /// A method for adding a given User to a given Site's Owners group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <returns>True if user was added, else false.</returns>
        public static bool AddSiteOwner(string url,
                                        string email)
        {
            return AuthMan.AddSiteUser(url, email, UserMembershipType.Owners);
        }

        /// <summary>
        /// A method for adding a given User to a given Site's Members group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <returns>True if user was added, else false.</returns>
        public static bool AddSiteMember(string url,
                                         string email)
        {
            return AuthMan.AddSiteUser(url, email, UserMembershipType.Members);
        }

        /// <summary>
        /// A method for adding a given User to a given Site's Visitors group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <returns>True if user was added, else false.</returns>
        public static bool AddSiteVisitor(string url,
                                          string email)
        {
            return AuthMan.AddSiteUser(url, email, UserMembershipType.Visitors);
        }

        /// <summary>
        /// A method for adding a given list of Users to a given Site's Owners
        /// group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="emails">The list of emails of the target Users.</param>
        /// <returns>True if all users were added, else false.</returns>
        public static bool AddSiteOwners(string url,
                                         List<string> emails)
        {
            return AuthMan.AddSiteUsers(url, emails, UserMembershipType.Owners);
        }

        /// <summary>
        /// A method for adding a given list of Users to a given Site's Members
        /// group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="emails">The list of emails of the target Users.</param>
        /// <returns>True if all users were added, else false.</returns>
        public static bool AddSiteMembers(string url,
                                          List<string> emails)
        {
            return AuthMan.AddSiteUsers(url, emails, UserMembershipType.Members);
        }

        /// <summary>
        /// A method for adding a given list of Users to a given Site's Visitors
        /// group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="emails">The list of emails of the target Users.</param>
        /// <returns>True if all users were added, else false.</returns>
        public static bool AddSiteVisitors(string url,
                                           List<string> emails)
        {
            return AuthMan.AddSiteUsers(url, emails, UserMembershipType.Visitors);
        }

        /// <summary>
        /// A method for removing a given User from a given Site's Owners group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <returns>True if user was removed, else false.</returns>
        public static bool RemoveSiteOwner(string url,
                                           string email)
        {
            return AuthMan.RemoveSiteUser(url, email, UserMembershipType.Owners);
        }

        /// <summary>
        /// A method for removing a given User from a given Site's Members group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <returns>True if user was removed, else false.</returns>
        public static bool RemoveSiteMember(string url,
                                            string email)
        {
            return AuthMan.RemoveSiteUser(url, email, UserMembershipType.Members);
        }

        /// <summary>
        /// A method for removing a given User from a given Site's Visitors group.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <returns>True if user was removed, else false.</returns>
        public static bool RemoveSiteVisitor(string url,
                                             string email)
        {
            return AuthMan.RemoveSiteUser(url, email, UserMembershipType.Visitors);
        }
        #endregion User

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
        internal static bool TryAdd(
            this Dictionary<object, object> dic,
            object key,
            object val)
        {
            try
            {
                if (!dic.ContainsKey(val))
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
        internal static bool TryAdd(
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
        internal static bool TryAdd(
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
