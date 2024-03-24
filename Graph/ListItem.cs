/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;

namespace Extensions
{
	/// <summary>
	/// An extension class to simplify getting data from ListItems.
	/// </summary>
    [Serializable]
    public static partial class ListItemExtensions
    {
		public static System.Collections.Generic.List<ListItemVersion> GetItemVersions(
			this ListItem listItem,
			ref GraphServiceClient graphClient)
		{
			//Create the container.
			System.Collections.Generic.List<ListItemVersion> listItemVersions =
				new System.Collections.Generic.List<ListItemVersion>();
			//Get the first page.
			var listItemVersionsPage = graphClient
				.Sites[listItem.ParentReference.SiteId]
				.Lists[listItem.Fields.AdditionalData["ParentListId"].ToString()]
				.Items[listItem.Id]
				.Versions
				.GetAsync(C =>
				{
					C.Headers.Add("ConsistencyLevel", "eventual");
					C.QueryParameters.Expand = new string[] { "fields" };
				}).GetAwaiter().GetResult();
			//If there are results, aggregate them.
			if (listItemVersionsPage.Value != null)
			{
				lock (listItemVersions)
				{
					listItemVersions.AddRange(listItemVersionsPage.Value);
				}
			}
			//If there are more pages of results.
			while ((listItemVersionsPage.Value != null) &&
				   (!string.IsNullOrEmpty(listItemVersionsPage.OdataNextLink)))
			{
				//Get the next page of results.
				listItemVersionsPage = graphClient
					.Sites[listItem.ParentReference.SiteId]
					.Lists[listItem.Fields.AdditionalData["ParentListId"].ToString()]
					.Items[listItem.Id]
					.Versions
					.WithUrl(listItemVersionsPage.OdataNextLink)
					.GetAsync(C =>
					{
						C.Headers.Add("ConsistencyLevel", "eventual");
						C.QueryParameters.Expand = new string[] { "fields" };
					}).GetAwaiter().GetResult();
				//Aggregate the results.
				lock (listItemVersions)
				{
					listItemVersions.AddRange(listItemVersionsPage.Value);
				}
			}
			return listItemVersions;
        }

        /// <summary>
        /// Method to get a boolean field from ListItem.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <param name="fieldName">The field name to get.</param>
        /// <returns></returns>
        public static bool GetJsonBool(
			this ListItem item, 
			string fieldName)
        {
            var result = false;

			try
			{
				result = Convert.ToBoolean(
					item.Fields.AdditionalData[fieldName]);
			}
			catch (Exception ex)
			{
				//Err(ex.ToString());
				result = false;
			}
			finally
			{
			}
			return result;
        }

		/// <summary>
		/// Method to get a string field from ListItem.
		/// </summary>
		/// <param name="item">The item in question.</param>
		/// <param name="fieldName">The field name to get.</param>
		/// <returns></returns>
		public static string GetJsonString(
			this ListItem item, 
			string fieldName)
		{
			var result = "";

			try
			{
				result = Convert.ToString(
					item.Fields.AdditionalData[fieldName]);
			}
			catch (Exception ex)
			{
				//Err(ex.ToString());
				result = "";
			}
			finally
			{
			}
			return result;
		}

        /// <summary>
        /// A method to return the ListItem objects in the "User Information List"
        /// of the Site containing the parent List of the given ListItem e.g.
		/// if the ListItem is from the "Breakthroughts" list located here: 
		/// "https://blog.cjvandyk.com/sites/Research/Lists/Breakthroughs"
		/// then the method will return all the items in the "Research" site's
		/// "User Information List" i.e. all the Site Users.  This can then be
		/// leveraged to match the UserField"LookupId" from Lists in the Site
		/// to a user and leveraging that user item's "EMail" extended
		/// information value, can be translated to an Entra ID.
        /// </summary>
        /// <param name="item">The given ListItem to use.</param>
        /// <returns>A list of ListItem objects in the "User Information List"
        /// of the Site containing the parent List of the given 
		/// ListItem.</returns>
        public static List<ListItem> GetSiteUserInformationList(
			this ListItem item)
		{
			return Graph.GetListItems("User Information List",
									  item.WebUrl.GetSiteRelativeUrl());
		}

		public static System.Collections.Generic.Dictionary<string, byte[]> GetVersions(
			this ListItem listItem,
			GraphServiceClient graphClient)
		{
			//Create the aggregation container.
			System.Collections.Generic.Dictionary<string, byte[]> listItemVersionsBytes =
				new System.Collections.Generic.Dictionary<string, byte[]>();
			//Get the list of DriveItemVersion objects.
			var listItemVersions = listItem.GetItemVersions(ref graphClient);
			//Iterate each version in the list and process.
			bool currentVersion = true;
			int versionNumber = listItemVersions.Count;
			for (int C = listItemVersions.Count - 1; C >= 0; C--)
			{
				graphClient.Sites[listItem.ParentReference.SiteId]
					.Lists[listItem.Fields.AdditionalData["ParentListId"].ToString()]
					.Items[listItem.Id]
					.Versions[listItemVersions[C].Id]
					.RestoreVersion
					.PostAsync()
					.GetAwaiter().GetResult();
			}
			return listItemVersionsBytes;
        }

        /// <summary>
        /// A method to return the Microsoft.Graph.Models.User object for the
        /// given ListItem's people picker field name e.g. 
        /// listItem.ToEntraUser("Author") will return the User object for
        /// the user who created the given ListItem whereas
        /// listItem.ToEntraUser("Editor") will return the User object for
        /// the user who last modified the given ListItem whereas
		/// listItem.ToEntraUser("MyPeopleField") will return the User object
		/// for the user populated in the "MyPeopleField" field of the given
		/// ListItem.
        /// </summary>
        /// <param name="item">The target ListItem to use.</param>
        /// <param name="userFieldName">The string name of the target people
		/// picker field in the list.</param>
        /// <returns>A Microsoft.Graph.Models.User object if found, else
		/// null.</returns>
        public static User ToEntraUser(this ListItem item,
									   string userFieldName)
		{
			try
			{
				var userList = item.GetSiteUserInformationList();
				return Core.GetUserByEmail(
					Core.GetUserEmailUpn(
						item.Fields.AdditionalData[
							userFieldName + "LookupId"].ToString(),
						ref userList));
			}
			catch (Exception ex)
			{
				Logit.Err(ex.ToString());
				return null;
			}
		}
    }
}
