#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;

namespace Extensions.Identity
{
    /// <summary>
    /// A class containing the default tenant configuration variable values.
    /// </summary>
    [Serializable]
    public partial class TenantConfig
    {
        /// <summary>
        /// The Tenant/Directory ID (GUID) value.
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// The Application/Client ID (GUID) value.
        /// </summary>
        public string AppClientId { get; set; }
        /// <summary>
        /// The certificate thumbprint value.
        /// </summary>
        public string CertThumbPrint { get; set; }
        /// <summary>
        /// The tenant string value e.g. "contoso" for "contoso.sharepoint.us".
        /// </summary>
        public string TenantString { get; set; }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
