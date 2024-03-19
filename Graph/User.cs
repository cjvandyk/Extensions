/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Extensions.Identity;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;

namespace Extensions
{
    /// <summary>
    /// An extension class to simplify working with Users.
    /// </summary>
    [Serializable]
    public static partial class UserExtensions
    {
        /// <summary>
        /// A method to get a Microsoft.Graph.Models.User object given its
        /// email address.
        /// </summary>
        /// <param name="email">The email address of the target user.</param>
        /// <returns>A Microsoft.Graph.Models.User object if found, else 
        /// null.</returns>
        public static User GetUserByEmail(string email)
        {
            try
            {
                return AuthMan.ActiveAuth.GraphClient
                    .Users
                    .GetAsync(C =>
                    {
                        C.QueryParameters.Filter = $"mail eq '{email}'";
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult().Value[0];
            }
            catch (Exception ex)
            {
                Logit.Err(ex.ToString());
                return null;
            }
        }
    }
}
