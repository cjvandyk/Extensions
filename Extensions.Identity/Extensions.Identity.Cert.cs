/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Security.Cryptography.X509Certificates;

namespace Extensions.Identity
{
    /// <summary>
    /// Class for working with certificates.
    /// </summary>
    [Serializable]
    public static partial class Cert
    {
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
