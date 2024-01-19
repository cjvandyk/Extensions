/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using static System.Logit;

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
        /// String containing the URL for the given tenant.
        /// </summary>
        public string TenantUrl { get; private set; }

        /// <summary>
        /// URI for the given tenant.
        /// </summary>
        public Uri TenantUri { get; private set; }

        /// <summary>
        /// String containing the Authority for the given tenant.
        /// </summary>
        public string Authority { get; private set; }

        /// <summary>
        /// String containing the URL of the endpoint for the Graph User API.
        /// </summary>
        public string GraphUserEndPointUrl { get; private set; }

        /// <summary>
        /// The Azure environment e.g. Commercial or USGovGCCHigh etc.  Default
        /// value is "USGovGCCHigh".
        /// </summary>
        public Core.AzureEnvironment AzureEnvironment { get; set; }
            = Core.AzureEnvironment.USGovGCCHigh;

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
            Init(null);
        }

        /// <summary>
        /// Constructor for a given tenant.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. for 
        /// contoso.sharepoint.us it would be 'contoso'.</param>
        public TenantConfig(string tenantString)
        {
            Init(tenantString);
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
        /// <param name="certStoreLocation">The certificate store location to 
        /// use where the certificate associated with the instance's 
        /// CertThumbprint is installed.  Default value is "CurrentUser" with 
        /// "LocalMachine" as the other alternative.</param>
        /// <param name="certThumbprint">The thumbprint of the certificate 
        /// associated with the ApplicationClientId of this instance.</param>
        /// <param name="debugEnabled">Switch to enable or disable debuging 
        /// for this instance.</param>
        /// <param name="multiThreaded">The ID of the site that 
        /// Extensions.Logit should use for debug logging.</param>
        /// <param name="logitSiteId">The ID of the site that Extensions.Logit
        /// should use for debug logging.</param>
        /// <param name="logitDefaultListGuid">The ID of the list that 
        /// Extensions.Logit should use for debug logging.</param>
        public TenantConfig(string tenantString,
                            Core.AzureEnvironment azureEnvironment,
                            string tenantDirectoryId,
                            string applicationClientId,
                            string certStoreLocation,
                            string certThumbprint,
                            bool debugEnabled,
                            bool multiThreaded,
                            string logitSiteId,
                            string logitDefaultListGuid)
        {
            TenantString = tenantString;
            AzureEnvironment = azureEnvironment;
            Init(tenantString);
            TenantDirectoryId = tenantDirectoryId;
            ApplicationClientId = applicationClientId;
            CertStoreLocation = certStoreLocation;
            CertThumbprint = certThumbprint;
            DebugEnabled = debugEnabled;
            MultiThreaded = multiThreaded;
            LogitSiteId = logitSiteId;
            LogitDefaultListGuid = logitDefaultListGuid;
            foreach (var prop in this.GetType().GetProperties())
            {
                Inf($"[{prop.Name}] = [{prop.GetValue(this)}]");
            }
        }

        /// <summary>
        /// Method to initialize base values.
        /// </summary>
        /// <param name="tenantString">The name of the tenant e.g. for 
        /// contoso.sharepoint.us it would be 'contoso'.</param>
        public void Init(string tenantString)
        {
            if (tenantString != null)
            {
                string authority = Core.GetAuthorityDomain(AzureEnvironment);
                TenantString = tenantString.Trim();
                TenantUrl = TenantString + ".sharepoint" + authority;                            
                TenantUri = new Uri("https://" + TenantUrl);
                Authority = 
                    $"https://login.microsoftonline{authority}/{TenantUrl}/";
                GraphUserEndPointUrl = 
                    $"https://graph.microsoft{authority}/v1.0/users";
            }
        }
    }
}
