/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using static Extensions.Constants;

namespace Extensions.Identity
{
    /// <summary>
    /// Class for working with certificates.
    /// </summary>
    [Serializable]
    public static partial class Cert
    {
#if NET472_OR_GREATER || NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Method to generate a new RSA based X509Certificate2 object and
        /// optionally export its PFX and/or CER files.
        /// </summary>
        /// <param name="displayName">The display name of the cert i.e. the
        /// part after 'cn='.</param>
        /// <param name="expireIn">An integer representing the numeric number
        /// of units to offset the cert expiration.</param>
        /// <param name="expireType">The type of unit to use in the offset
        /// calculation of the cert expiration.</param>
        /// <param name="exportPFX">A boolean switch to trigger the export of
        /// the cert to a .pfx file.  NOTE: If exportPFX is true, pfxPassword
        /// and exportPath must be a valid string.</param>
        /// <param name="pfxPassword">The password to use with the generated
        /// .pfx file.  NOTE: If exportPFX is true, pfxPassword and exportPath
        /// must be a valid string.</param>
        /// <param name="exportPath">The file system path where the cert files
        /// should be saved.  NOTE: If exportPFX or exportCER is true, 
        /// exportPath must be a valid string.</param>
        /// <param name="exportCER">A boolean switch to trigger the export of
        /// the cert to a .cer file.  NOTE: If exportCER is true, exportPath
        /// must be a valid string.</param>
        /// <returns>The generated X509Certificate2 object.</returns>
        public static X509Certificate2 NewSelfSigned(string displayName,
                                                     int expireIn,
                                                     TimeSpanType expireType,
                                                     bool exportPFX = false,
                                                     string pfxPassword = "",
                                                     string exportPath = "",
                                                     bool exportCER = false)
        {
            var rsa = RSA.Create();
            var req = new CertificateRequest($"cn={displayName}",
                                             rsa,
                                             HashAlgorithmName.SHA512,
                                             RSASignaturePadding.Pkcs1);
            DateTimeOffset offset = DateTime.Now;
            switch (expireType)
            {
                case TimeSpanType.Seconds:
                    offset = DateTimeOffset.Now.AddSeconds(expireIn);
                    break;
                case TimeSpanType.Minutes:
                    offset = DateTimeOffset.Now.AddMinutes(expireIn);
                    break;
                case TimeSpanType.Hours:
                    offset = DateTimeOffset.Now.AddHours(expireIn);
                    break;
                case TimeSpanType.Days:
                    offset = DateTimeOffset.Now.AddDays(expireIn);
                    break;
                case TimeSpanType.Weeks:
                    offset = DateTimeOffset.Now.AddDays(expireIn * 7);
                    break;
                case TimeSpanType.Months:
                    offset = DateTimeOffset.Now.AddMonths(expireIn);
                    break;
                case TimeSpanType.Years:
                    offset = DateTimeOffset.Now.AddYears(expireIn);
                    break;
                case TimeSpanType.Decades:
                    offset = DateTimeOffset.Now.AddYears(expireIn * 10);
                    break;
                case TimeSpanType.Centuries:
                    offset = DateTimeOffset.Now.AddYears(expireIn * 100);
                    break;
            }
            var cert = req.CreateSelfSigned(DateTimeOffset.Now, offset);
            if (exportPFX)
            {
                if (pfxPassword == "")
                {
                    throw new Exception("Argument [pfxPassword] is required!");
                }
                if (exportPath == "")
                {
                    throw new Exception("Argument [exportPath] is required!");
                }
                //Create PFX (PKCS #12) with private key.
                System.IO.File.WriteAllBytes(exportPath.TrimEnd('\\') +
                                                $@"\{displayName}.pfx",
                                             cert.Export(X509ContentType.Pfx,
                                                         pfxPassword));
            }
            if (exportCER)
            {
                if (exportPath == "")
                {
                    throw new Exception("Argument [exportPath] is required!");
                }
                //Create Base64 encoded CER (public key only).
                System.IO.File.WriteAllText(
                    exportPath.TrimEnd('\\') + $@"\{displayName}.cer",
                    "-----BEGIN CERTIFICATE-----\r\n" +
                    Convert.ToBase64String(
                        cert.Export(X509ContentType.Cert),
                        Base64FormattingOptions.InsertLineBreaks) +
                    "\r\n-----END CERTIFICATE-----");
            }
            return cert;
        }
#endif

        /// <summary>
        /// Get a certificate from the CurrentUser store based on thumbprint.
        /// If the certificate is not found, the LocalMachine store is searched
        /// for the matching certificate.
        /// </summary>
        /// <param name="thumbPrint">The thumbprint of the target 
        /// certificate.</param>
        /// <returns>The certificate if found, else null.</returns>
        public static X509Certificate2 GetCertByThumbPrint(string thumbPrint)
        {
            //Check if the cert is in the CurrentUser store.
            var cert = GetCertByThumbPrint(thumbPrint,
                                           StoreName.My,
                                           StoreLocation.CurrentUser);
            //If it is not in the CurrentUser store, attempt to locate the
            //cert in the LocalMachine store.
            return cert == null ?
                GetCertByThumbPrint(thumbPrint,
                                    StoreName.My,
                                    StoreLocation.LocalMachine) : cert;
        }

        /// <summary>
        /// Get a certificate from the specified StoreName and StoreLocation.
        /// </summary>
        /// <param name="thumbPrint">The thumbprint of the target 
        /// certificate.</param>
        /// <param name="storeName">The name of the certificate store to
        /// search e.g. My.</param>
        /// <param name="storeLocation">The location of the certificate
        /// store to search e.g. CurrentUser, LocalMachine etc.</param>
        /// <returns>The certificate if found, else null.</returns>
        public static X509Certificate2 GetCertByThumbPrint(
            string thumbPrint,
            StoreName storeName,
            StoreLocation storeLocation)
        {
            //Define the store with the specified StoreName and StoreLocation.
            X509Store store = new X509Store(storeName, storeLocation);
            X509Certificate2 result = null;
            try
            {
                //Attempt to open the store in read mode.
                store.Open(OpenFlags.ReadOnly);
                //Get the collection of certificates in the store.
                X509Certificate2Collection certCollection = store.Certificates;
                //Filter the collection of certificates by matching thumbprints.
                //Only return valid certificates.  Should only return 1 cert.
                certCollection = certCollection.Find(X509FindType.FindByThumbprint,
                                                     thumbPrint,
                                                     false);  //For self signed cert.
                //If a valid cert was found, return it, otherwise return null.
                result = certCollection.Count == 0 ? null : certCollection[0];
            }
            catch (Exception ex)
            {
                //Err(ex.ToString());
            }
            finally
            {
                //Close the store before returning.
                store.Close();
            }
            //Throw exception if we fail to load the cert.
            if ((storeLocation == StoreLocation.LocalMachine) &&
                (result == null))
            {
                throw new Exception($"Unable to load certificate [{thumbPrint}]");
            }
            return result;
        }
    }
}
