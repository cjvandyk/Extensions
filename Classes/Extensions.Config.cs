using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Online.SharePoint.TenantAdministration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Extensions.Core;
using static Extensions.Identity.AuthMan;
using static System.Logit;

namespace Extensions
{
    /// <summary>
    /// Class that carries the configuration for a given tenant.
    /// </summary>
    [Serializable]
    public partial class Config
    {
        /// <summary>
        /// Settings contains the dictionary of all settings.
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }
            = new Dictionary<string, string>();
        /// <summary>
        /// Labels contains the Azure Information Protection labels for the tenant.
        /// </summary>
        public Dictionary<string, string> Labels { get; set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Config() 
        {
        }

        /// <summary>
        /// Method to generate the configuration JSON filename based on the 
        /// given tenantString.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. for 
        /// contoso.sharepoint.us it would be 'contoso'.</param>
        /// <returns>The filename of the configuration JSON file.</returns>
        internal string GetConfigFileName(string tenantString)
        {
            return $"UniversalConfig.{tenantString}.json";
        }

        /// <summary>
        /// Method to generate the labels JSON filename based on the 
        /// given tenantString.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. for 
        /// contoso.sharepoint.us it would be 'contoso'.</param>
        /// <returns>The filename of the label JSON file.</returns>
        internal string GetLabelsFileName(string tenantString)
        {
            return $"Labels.{tenantString}.json";
        }

        /// <summary>
        /// Method to load the configuration JSON files.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. for 
        /// contoso.sharepoint.us it would be 'contoso'.</param>
        internal void LoadConfig(string tenantString)
        {
            Inf("Loading config from [" +
                $"{Core.GetRunFolder()}\\{GetConfigFileName(tenantString)}]");
            Settings = LoadJSON($"{Core.GetRunFolder()}" +
                                $"\\{GetConfigFileName(tenantString)}");
            Labels = LoadJSON($"{Core.GetRunFolder()}" +
                              $"\\{GetLabelsFileName(tenantString)}");
            Inf($"[{Settings.Count}] settings and [{Labels.Count}] labels loaded.");
        }

        /// <summary>
        /// Method to load a JSON file with dictionary values.
        /// </summary>
        /// <param name="filePath">The name of the file to load.</param>
        /// <returns>A dictionary of strings from the JSON file.</returns>
        internal Dictionary<string, string> LoadJSON(string filePath)
        {
            var result = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                result = JsonSerializer.Deserialize<
                    Dictionary<string, string>>(reader.ReadToEnd());
            }
            return result;
        }

        /// <summary>
        /// Method to initialize a given tenant for further operations.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. for 
        /// contoso.sharepoint.us it would be 'contoso'.</param>
        /// <returns>The TenantConfig object fully initialized and authorized
        /// with HttpClient and GraphServiceClient objects ready for use.</returns>
        public TenantConfig InitializeTenant(string tenantString)
        {
            try
            {
                Inf($"Initializing tenant [{tenantString}] settings");
                if (Tenants.ContainsKey(tenantString))
                {
                    ActiveTenant = Tenants[tenantString];
                    return ActiveTenant;
                }
                var tenant = new TenantConfig(tenantString);
                LoadConfig(tenantString);
                tenant.AzureEnvironment = GetAzureEnvironment(
                    GetEnv("AzureEnvironment"));
                tenant.TenantDirectoryId = GetEnv("TenantDirectoryId");
                tenant.ApplicationClientId = GetEnv("ApplicationClientId");
                tenant.CertStoreLocation = GetEnv("CertStoreLocation");
                tenant.CertThumbprint = GetEnv("CertThumbprint");
                tenant.DebugEnabled = 
                    GetEnv("DebugEnabled") == "" ? 
                    true : 
                    Convert.ToBoolean(GetEnv("DebugEnabled"));
                tenant.MultiThreaded =
                    GetEnv("MultiThreaded") == "" ?
                    true :
                    Convert.ToBoolean(GetEnv("MultiThreaded"));
                tenant.LogitSiteBaseUrl = GetEnv("LogitSiteBaseUrl");
                tenant.LogitSiteId = GetEnv("LogitSiteId");
                tenant.LogitDefaultListGuid = GetEnv("LogitDefaultListGuid");
                if (GetEnv("DefaultBlobContainer") == "")
                {
                    SetEnv("DefaultBlobContainer", "provisioning-blob");
                }
                if (GetEnv("EmailSendingAccount") == "")
                {
                    SetEnv("DefaultBlobContainer", "C@contoso.com");
                }
                CoreBase.Initialize(tenantString);
                return tenant;
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                throw;
            }
        }
    }
}
