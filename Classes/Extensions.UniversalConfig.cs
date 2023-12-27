#pragma warning disable CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Online.SharePoint.TenantAdministration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static Extensions.Core;  //NuGet Extensions.cs
using static System.Logit;     //Nuget Extensions.Logit

namespace Extensions
{
    [Serializable]
    public static partial class UniversalConfig
    {
        public static void InitializeTenant(string tenantString)
        {
            try
            {
                string target = 
                    $"{GetRunFolder()}\\UniversalConfig.{tenantString}.json";
                Inf($"Initializing tenant [{tenantString}]");
                if (!Tenants.ContainsKey(tenantString))
                {
                    Inf($"Loading config from [{target}]");
                    using (StreamReader sr = new StreamReader(target))
                    {
                        Tenants.Add(
                            tenantString,
                            JsonSerializer.Deserialize<TenantConfig>(
                                sr.ReadToEnd()));
                        ActiveTenant = Tenants[tenantString];
                        Inf($"Added tenant [{tenantString}] to the pool now " +
                            $"containing [{Tenants.Count}] configs.");
                    }
                }
                else
                {
                    Inf($"Tenant [{tenantString}] is already in the pool.  " +
                        $"Using pool instance.");
                    ActiveTenant = Tenants[tenantString];
                }
                Inf($"Done initializing tenant [{tenantString}]");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

#pragma warning restore CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028
