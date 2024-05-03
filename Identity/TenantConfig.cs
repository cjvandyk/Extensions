/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Extensions.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static Extensions.Constants;

namespace Extensions
{
    /// <summary>
    /// Class that contains tenant configuration settings.
    /// </summary>
    [Serializable]
    public partial class TenantConfig
    {
        /// <summary>
        /// The name of the tenant e.g. "contoso.sharepoint.us" would 
        /// be "contoso".  Default is "Contoso".
        /// </summary>
        public string TenantString { get; set; } = "Contoso";

        /// <summary>
        /// Dictionary containing settings for the given tenant.
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Dictionary containing the AIP labels for a given tenant.
        /// </summary>
        public Dictionary<string, string> Labels { get; set; }

        /// <summary>
        /// The Azure environment e.g. Commercial or USGovGCCHigh etc.  Default
        /// value is "USGovGCCHigh".
        /// </summary>
        public AzureEnvironmentName AzureEnvironment { get; set; }
            = AzureEnvironmentName.O365USGovGCCHigh;

        /// <summary>
        /// The authority domain extension.
        /// </summary>
        public string AuthorityDomain
        {
            get
            {
                if (AzureEnvironment == AzureEnvironmentName.O365USGovGCCHigh)
                {
                    return ".us";
                }
                return ".com";
            }
        }

        /// <summary>
        /// String containing the URL for the given tenant.
        /// </summary>
        public string TenantUrl
        {
            get
            {
                return TenantString + ".sharepoint" + AuthorityDomain;
            }
        }

        /// <summary>
        /// URI for the given tenant.
        /// </summary>
        public Uri TenantUri
        {
            get
            {
                return new Uri("https://" + TenantUrl);
            }
        }

        /// <summary>
        /// String containing the Authority for the given tenant.
        /// </summary>
        public string Authority
        {
            get
            {
                return $"https://login.microsoftonline{AuthorityDomain}/{TenantUrl}/";
            }
        }

        /// <summary>
        /// String containing the URL of the endpoint for the Graph User API.
        /// </summary>
        public string GraphUserEndPointUrl
        {
            get
            {
                return $"https://graph.microsoft{AuthorityDomain}/v1.0/users";
            }
        }

        /// <summary>
        /// The Tenant or Directory ID to use for this instance.  Default is
        /// all zeros.
        /// </summary>
        public string TenantDirectoryId { get; set; }
            = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The Application or Client ID to use for this instance.  Default is
        /// all zeros.
        /// </summary>
        public string ApplicationClientId { get; set; }
            = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The certificate store location to use where the certificate
        /// associated with the instance's CertThumbprint is installed.
        /// Default value is "CurrentUser" with "LocalMachine" as the 
        /// other alternative.
        /// </summary>
        public string CertStoreLocation { get; set; } = "CurrentUser";

        /// <summary>
        /// The thumbprint of the certificate associated with the 
        /// ApplicationClientId of this instance.  Default is a blank string.
        /// </summary>
        public string CertThumbprint { get; set; } = "";

        /// <summary>
        /// Switch to enable or disable debuging for this instance.  Default
        /// is true.
        /// </summary>
        public bool DebugEnabled { get; set; } = true;

        /// <summary>
        /// Switch to enable or disable multi-threading for this instance.
        /// Default is true.
        /// </summary>
        public bool MultiThreaded { get; set; } = true;

        /// <summary>
        /// The base URL of the site that Extensions.Logit should use for debug
        /// logging.  Default is "Logit".
        /// </summary>
        public string LogitSiteBaseUrl { get; set; } = "Logit";

        /// <summary>
        /// The ID of the site that Extensions.Logit should use for debug
        /// logging.  Default is all zeros.
        /// </summary>
        public string LogitSiteId { get; set; }
            = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The ID of the list that Extensions.Logit should use for debug
        /// logging.  Default is all zeros.
        /// </summary>
        public string LogitDefaultListGuid { get; set; }
            = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public TenantConfig()
        {
            if (Environment.GetEnvironmentVariable("TenantString") == null)
            {
                throw new Exception("Environment variable for TenantString not found.");
            }
            TenantString = Environment.GetEnvironmentVariable("TenantString").Trim();
            LoadConfig();
            TenantDirectoryId = Settings["TenantDirectoryId"];
            ApplicationClientId = Settings["ApplicationClientId"];
            CertThumbprint = Settings["CertThumbprint"];
            DebugEnabled = Convert.ToBoolean(Settings["DebugEnabled"]);
            MultiThreaded = Convert.ToBoolean(Settings["MultiThreaded"]);
        }

        /// <summary>
        /// Constructor for a given tenant.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. for 
        /// contoso.sharepoint.us it would be 'contoso'.</param>
        public TenantConfig(string tenantString)
        {
            if (tenantString == null)
            {
                throw new ArgumentNullException("tenantString");
            }
            TenantString = tenantString.Trim();
            LoadConfig();
            TenantDirectoryId = Settings["TenantDirectoryId"];
            ApplicationClientId = Settings["ApplicationClientId"];
            CertThumbprint = Settings["CertThumbprint"];
            DebugEnabled = Convert.ToBoolean(Settings["DebugEnabled"]);
            MultiThreaded = Convert.ToBoolean(Settings["MultiThreaded"]);
        }

        /// <summary>
        /// Parameterized constructor for the class.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. 
        /// "contoso.sharepoint.us" would be "contoso".  Default is 
        /// "Contoso".</param>
        /// <param name="azureEnvironment">The Azure environment e.g. 
        /// Commercial or USGovGCCHigh etc.  Default value is 
        /// "USGovGCCHigh".</param>
        /// <param name="tenantDirectoryId">The Tenant or Directory ID to use 
        /// for this instance.</param>
        /// <param name="applicationClientId">The Application or Client ID to 
        /// use for this instance.</param>
        /// <param name="certThumbprint">The thumbprint of the certificate 
        /// associated with the ApplicationClientId of this instance.</param>
        /// <param name="logitSiteId">The ID of the site that Extensions.Logit
        /// should use for debug logging.</param>
        /// <param name="logitDefaultListGuid">The ID of the list that 
        /// Extensions.Logit should use for debug logging.</param>
        /// <param name="logitSiteBaseUrl">Optional string of the base URL of 
        /// the site to be used by System.Logit.  Defaults to "Logit".</param>
        /// <param name="certStoreLocation">The certificate store location to 
        /// use where the certificate associated with the instance's 
        /// CertThumbprint is installed.  Default value is "CurrentUser" with 
        /// "LocalMachine" as the other alternative.</param>
        /// <param name="debugEnabled">Switch to enable or disable debuging 
        /// for this instance.</param>
        /// <param name="multiThreaded">The ID of the site that 
        /// Extensions.Logit should use for debug logging.</param>
        public TenantConfig(string tenantString,
                            AzureEnvironmentName azureEnvironment,
                            string tenantDirectoryId,
                            string applicationClientId,
                            string certThumbprint,
                            string logitSiteId,
                            string logitDefaultListGuid,
                            string logitSiteBaseUrl = "Logit",
                            string certStoreLocation = "CurrentUser",
                            bool debugEnabled = true,
                            bool multiThreaded = true)
        {
            TenantString = tenantString;
            AzureEnvironment = azureEnvironment;
            TenantString = tenantString.Trim();
            TenantDirectoryId = tenantDirectoryId;
            ApplicationClientId = applicationClientId;
            CertThumbprint = certThumbprint;
            LogitSiteId = logitSiteId;
            LogitDefaultListGuid = logitDefaultListGuid;
            LogitSiteBaseUrl = logitSiteBaseUrl;
            CertStoreLocation = certStoreLocation;
            DebugEnabled = debugEnabled;
            MultiThreaded = multiThreaded;
            LoadConfig();
        }

        /// <summary>
        /// A method for loading environment variables from JSON
        /// config files when interactively debugging.
        /// </summary>
        public void LoadConfig()
        {
            if ((TenantString == null) ||
                (TenantString == "Contoso"))
            {
                if (Environment.GetEnvironmentVariable("TenantString") != null)
                {
                    TenantString = Environment.GetEnvironmentVariable("TenantString");
                }
                else
                {
                    throw new Exception("ERROR!!!  No TenantString environment variable defined.");
                }
            }
            if ((AuthMan.TargetTenantConfig != null) &&
                (AuthMan.TargetTenantConfig.TenantString == TenantString))
            {
                Settings = AuthMan.TargetTenantConfig.Settings;
                Labels = AuthMan.TargetTenantConfig.Labels;
            }
            else
            {
                Settings = new Dictionary<string, string>();
                Settings.Add("TenantString", TenantString);
                if (Environment.UserInteractive)
                {
                    Core.SetEnv("RUNNING_IN_AZURE", "False");
                    if (AuthMan.TargetTenantConfig != null)
                    {
                        Logit.Inf($"Loading config from [" +
                            $"{GetRunFolder()}\\{GetConfigFileName()}].");
                    }
                    Settings = LoadJSON($"{GetRunFolder()}" + 
                        $"\\{GetConfigFileName()}");
                    if (AuthMan.TargetTenantConfig != null)
                    {
                        Logit.Inf($"Config loaded.\nLoading labels from [" +
                            $"{GetRunFolder()}\\{GetLabelsFileName()}].");
                    }
                    Labels = LoadJSON($"{GetRunFolder()}" +
                        $"\\{GetLabelsFileName()}");
                    if (AuthMan.TargetTenantConfig != null)
                    {
                        Logit.Inf("Labels loaded.");
                    }
                }
                else
                {
                    Settings.Add("TenantDirectoryId", GetEnv("TenantDirectoryId"));
                    Settings.Add("ApplicationClientId", GetEnv("ApplicationClientId"));
                    Settings.Add("CertThumbprint", GetEnv("CertThumbprint"));
                    Settings.Add("LogitSiteId", GetEnv("LogitSiteId"));
                    Settings.Add("LogitDefaultListGuid", GetEnv("LogitDefaultListGuid"));
                    Settings.Add("LogitSiteBaseUrl", GetEnv("LogitSiteBaseUrl"));
                    Settings.Add("CertStoreLocation", GetEnv("CertStoreLocation"));
                    Settings.Add("DebugEnabled", GetEnv("DebugEnabled"));
                    Settings.Add("MultiThreaded", GetEnv("MultiThreaded"));
                    //The value of the Environment Variable "Settings" is a
                    //comma separated string containing the names of the 
                    //settings to be added for the current tenant e.g.
                    //"AzureEnvironment,ConnectionString" etc.
                    var settings = GetEnv("Settings").Split(',');
                    foreach (string setting in settings)
                    {
                        Settings.Add(setting, GetEnv(setting));
                    }
                    //The value of the Environment Variable "Labels" is a 
                    //comma separated string containing the compound strings
                    //for each label pair.  Each pair is in turn colon 
                    //separated e.g.
                    //"Secret:SecretGUID","TopSecret:TopSecretGUID" etc. where
                    //the Secret label will be added to the dictionary with a
                    //key of "Secret" and a value of whatever is stored in the
                    //Environment Variable called "SecretGUID".
                    Labels = new Dictionary<string, string>();
                    var labels = GetEnv("Labels").Split(',');
                    foreach (string label in labels)
                    {
                        Labels.Add(label.Left(":").Trim(), label.Right(":").Trim());
                    }
                }
                AuthMan.TargetTenantConfig = this;
            }
        }

        /// <summary>
        /// A method to return the file name of the active tenant config file.
        /// </summary>
        /// <param name="tenantString">An optional parameter that specifies
        /// the TenantString e.g. for the "https://crayveon.sharepoint.us"
        /// Tenant, the string would be "crayveon".  If not supplied, the
        /// TenantString of the active Tenant is used.</param>
        /// <returns>The file name of the active tenant config file.</returns>
        public static string GetConfigFileName(string tenantString = null)
        {
            if (tenantString != null)
            {
                return $"UniversalConfig.{tenantString}.json";
            }
            return $"UniversalConfig.{GetEnv("TenantString")}.json";
        }

        /// <summary>
        /// A method to return the file name of the active tenant sensitivity
        /// labels file.
        /// </summary>
        /// <param name="tenantString">An optional parameter that specifies
        /// the TenantString e.g. for the "https://crayveon.sharepoint.us"
        /// Tenant, the string would be "crayveon".  If not supplied, the
        /// TenantString of the active Tenant is used.</param>
        /// <returns>The file name of the active tenant sensitivity labels
        /// file.</returns>
        public static string GetLabelsFileName(string tenantString = null)
        {
            if (tenantString != null)
            {
                return $"Labels.{tenantString}.json";
            }
            return $"Labels.{GetEnv("TenantString")}.json";
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
            if (!File.Exists(filePath))
            {
                throw new Exception($"{filePath} does not exist.");
            }
            using (StreamReader sr = new StreamReader(filePath))
            {
                result = JsonSerializer.Deserialize<
                    Dictionary<string, string>>(sr.ReadToEnd());
            }
            return result;
        }

        /// <summary>
        /// Get the location where the assembly stack started executing.
        /// </summary>
        /// <returns>The folder path.</returns>
        internal static string GetRunFolder()
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
        /// A method to get an EnvironmentVariable value.  If not found, the
        /// variable is sought in Settings instead.
        /// </summary>
        /// <param name="key">The target variable name.</param>
        /// <returns>The value of the EnvironmentVariable or if not found, the
        /// return value of the GetSetting() method.</returns>
        public static string GetEnv(string key)
        {
            if (Environment.GetEnvironmentVariable(key) != null)
            {
                return Environment.GetEnvironmentVariable(key);
            }
            return GetSetting(key);
        }

        /// <summary>
        /// A public method to allow settings values to be retrieved.
        /// </summary>
        /// <param name="key">The name of the setting value to retrieve.</param>
        /// <returns>If the requested setting exist in the config, its value is
        /// returned else a blank string is returned.</returns>
        public static string GetSetting(string key)
        {
            if (AuthMan.TargetTenantConfig == null)
            {
                TenantConfig tenantConfig = new TenantConfig();
                tenantConfig.LoadConfig();
                AuthMan.TargetTenantConfig = tenantConfig;
            }
            if (AuthMan.TargetTenantConfig.Settings.TryGetValue(key, out string result))
            {
                return result;
            }
            return "";
        }
    }
}
