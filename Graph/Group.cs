/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Extensions.Identity;
using Microsoft.Graph.Models;
using System;

namespace Extensions
{
	/// <summary>
	/// An extension class to simplify working with Groups.
	/// </summary>
    [Serializable]
    public static partial class GroupExtensions
    {
        /// <summary>
        /// A method to get the Group given its GUID ID.
        /// </summary>
        /// <param name="groupId">The Entra GUID ID of the target Group.</param>
        /// <returns>A Microsoft.Graph.Models.Group object if found, else
        /// null.</returns>
        public static Group GetGroup(string groupId)
        {
            try
            {
                return AuthMan.ActiveAuth.GraphClient
                    .Groups[groupId]
                    .GetAsync(C =>
                    {
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logit.Err(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// A method to determine if the given user is a Member of the given
        /// Group.
        /// </summary>
        /// <param name="group">The given Group to use for validation.</param>
        /// <param name="user">The User to validate.</param>
        /// <returns>True if the given user is a member, else false.</returns>
        public static bool HasMember(this Group group,
                                     User user)
        {
            return group.CheckMembership(
                user.Id,
                Constants.UserMembershipType.Members);
        }

        /// <summary>
        /// A method to determine if the given user is a Member of the given
        /// Group.
        /// </summary>
        /// <param name="group">The given Group to use for validation.</param>
        /// <param name="userId">The Entra ID of the User to validate.</param>
        /// <returns>True if the given user is a member, else false.</returns>
        public static bool HasMember(this Group group,
                                     string userId)
        {
            return group.CheckMembership(
                userId,
                Constants.UserMembershipType.Members);
        }

        /// <summary>
        /// A method to determine if the given user is an Owner of the given
        /// Group.
        /// </summary>
        /// <param name="group">The given Group to use for validation.</param>
        /// <param name="user">The User to validate.</param>
        /// <returns>True if the given user is an owner, else false.</returns>
        public static bool HasOwner(this Group group,
                                    User user)
        {
            return group.CheckMembership(
                user.Id,
                Constants.UserMembershipType.Owners);
        }

        /// <summary>
        /// A method to determine if the given user is an Owner of the given
        /// Group.
        /// </summary>
        /// <param name="group">The given Group to use for validation.</param>
        /// <param name="userId">The Entra ID of the User to validate.</param>
        /// <returns>True if the given user is an owner, else false.</returns>
        public static bool HasOwner(this Group group,
                                    string userId)
        {
            return group.CheckMembership(
                userId,
                Constants.UserMembershipType.Owners);
        }

        /// <summary>
        /// A method to determine if a given user is a member, owner or either
        /// of the given Group.
        /// </summary>
        /// <param name="group">The given Group to use for validation.</param>
        /// <param name="userId">The Entra ID of the User to validate.</param>
        /// <param name="userMembershipType">The type of validation to perform
        /// i.e. Members, Owners or All.  Defaults to Members.</param>
        /// <returns>True if the given user is a member or owner, else
        /// false.</returns>
        public static bool CheckMembership(
            this Group group,
            string userId,
            Constants.UserMembershipType userMembershipType =
                Constants.UserMembershipType.Members)
        {
            try
            {
                UserCollectionResponse response = null;
                switch (userMembershipType)
                {
                    case Constants.UserMembershipType.Members:
                        //Check if the user is a member of the group.
                        response = AuthMan.ActiveAuth.GraphClient
                            .Groups[group.Id]
                            .Members
                            .GraphUser
                            .GetAsync(C =>
                            {
                                C.QueryParameters.Filter = $"id eq '{userId}'";
                                C.Headers.Add("ConsistencyLevel", "eventual");
                            }).GetAwaiter().GetResult();
                        break;
                    case Constants.UserMembershipType.Owners:
                        //Check if the user is an owner of the group.
                        response = AuthMan.ActiveAuth.GraphClient
                            .Groups[group.Id]
                            .Owners
                            .GraphUser
                            .GetAsync(C =>
                            {
                                C.QueryParameters.Filter = $"id eq '{userId}'";
                                C.Headers.Add("ConsistencyLevel", "eventual");
                            }).GetAwaiter().GetResult();
                        break;
                    case Constants.UserMembershipType.All:
                        //Check if the user is a member of the group.
                        if (CheckMembership(group, userId, Constants.UserMembershipType.Members))
                        {
                            //User is a member.
                            return true;
                        }
                        //User is not a member.  Check if user is an owner.
                        if (CheckMembership(group, userId, Constants.UserMembershipType.Owners))
                        {
                            //User is an owner.
                            return true;
                        }
                        //User is neither member nor owner.
                        return false;
                        break;
                }
                //If we got no matches, user is not in the target collection.
                if ((response == null) ||
                    (response.Value == null) ||
                    (response.Value.Count == 0))
                {
                    return false;
                }
                //We got matches.
                return true;
            }
            catch (Exception ex)
            {
                Logit.Err(ex.ToString());
                return false;
            }
        }
    }
}
