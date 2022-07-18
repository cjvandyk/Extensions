#pragma warning disable CS1587

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

using static Extensions.Universal;
using static System.Logging;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Extensions.GCCH
{
    [Serializable]
    /// <summary>
    /// Extension methods for the GCCH set of classes.
    /// </summary>
    public class CertAuth
    {
        private string certStoreName { get; set; }
        private string certStoreLocation { get; set; }
        private string certThumbPrint { get; set; }
        private string appClientId { get; set; }
        private string tenantDomain { get; set; }
        private string spBaseUrl { get; set; }
        private Dictionary<string, string> certs = 
            new Dictionary<string, string>();
        private X509Certificate2 appCert { get; set; }
        private Logging log = new Logging("CertAuth.log", true, true, true, true);

        public enum CertType
        {
            AzureAD,
            Exchange,
            GlobalReader,
            PowerBI,
            SharePoint,
            Teams
        }

        #region CertAuth()
        /// <summary>
        /// Constructor for the CertAuth class.
        /// </summary>
        /// <param name="timeZone">The target time zone from Constants.TimeZone.</param>
        /// <returns>The string that can be used with 
        /// TimeZoneInfo.FindSystemTimeZoneById() for time zone convertions.</returns>
        public CertAuth()
        {
            try
            {
                certStoreName = ConfigurationSettings.AppSettings["CertStoreName"].ToString();
                certStoreLocation = ConfigurationSettings.AppSettings["CertStoreLocation"].ToString();
                certThumbPrint = ConfigurationSettings.AppSettings["CertThumbPrint"].ToString();
                appClientId = ConfigurationSettings.AppSettings["CertAppClientId"].ToString();
                tenantDomain = ConfigurationSettings.AppSettings["CertTenantDomain"].ToString();
                spBaseUrl = ConfigurationSettings.AppSettings["CertSPBaseUrl"].ToString();
                StoreName storeName;
                Enum.TryParse(certStoreName, out storeName);
                StoreLocation storeLocation;
                Enum.TryParse(certStoreLocation, out storeLocation);
                X509Store x509Store = new X509Store(storeName, storeLocation);
                x509Store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection x509Certificate2s =
                    x509Store.Certificates.Find(X509FindType.FindByThumbprint,
                                                certThumbPrint,
                                                false); //For self signed cert.
                x509Store.Close();
                if (x509Certificate2s.Count > 0)
                {
                    appCert = x509Certificate2s[0];
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private X509Certificate2 GetCertificate()
        {
            return null;
        }

        //public static string CertAuth()
        //{
        //    ValidateNoNulls(timeZone);
        //    return Constants.TimeZones[timeZone];
        //}
        #endregion GCCH()
    }
}
#pragma warning restore CS1587
