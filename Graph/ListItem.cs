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
using System.IO;
using System.Linq;

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
				.Sites[listItem.GetParentSiteId()]
				.Lists[listItem.GetParentListId()]
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
        /// A method to return the List Id GUID of the parent List for the 
        /// current ListItem.
        /// </summary>
        /// <param name="item">The current ListItem target.</param>
        /// <returns>The List Id GUID of the parent List.</returns>
        public static string GetParentListId(this ListItem item)
		{
			return item.AdditionalData["fields@odata.context"]
				.ToString()
				.ToLower()
				.Right("')/lists('")
				.Left("')/items('");
		}

        /// <summary>
        /// A method to return the part of the URL of the parent List that
		/// represents the list itself, for the current ListItem e.g. if the
		/// current ListItem's WebUrl value is "/sites/Research/Docs/a.docx"
		/// the return value would be "Docs" since this is a Document Library
		/// whereas if the value was "/sites/Research/lists/Questions/Q1.xml"
		/// the return value would be "lists/Questions" since it would be a
		/// SharePoint List.
        /// </summary>
        /// <param name="item">The current ListItem target.</param>
        /// <returns>The part of the URL of the parent List that represents
		/// the list iself e.g. "Docs" for Document Libraries or 
		/// "lists/Questions" in the case of SharePoint Lists.</returns>
		public static string GetParentListUrl(this ListItem item)
		{
			var uri = new Uri(item.WebUrl.ToLower());
			return uri.AbsolutePath.Right("/", 3)
								   .Left("/", -1);
		}

        /// <summary>
        /// A method to return the Site Id GUID of the parent Site for the 
        /// current ListItem.
        /// </summary>
        /// <param name="item">The current ListItem target.</param>
        /// <returns>The Site Id GUID of the parent Site.</returns>
		public static string GetParentSiteId(this ListItem item)
		{
			return item.ParentReference.SiteId.Split(',')[1];
		}

        /// <summary>
        /// A method to return the relative Site path of the parent Site for
		/// the current ListItem e.g. "/sites/research".
        /// </summary>
        /// <param name="item">The current ListItem target.</param>
        /// <returns>The relative Site path of the parent Site for the current
		/// ListItem.</returns>
		public static string GetParentSiteRelativeUrl(this ListItem item)
		{
			return $"/sites/{item.WebUrl.ToLower().Right("/sites/")
												  .Left("/")}";
		}

        /// <summary>
        /// A method to return the Site path of the parent Site for
		/// the current ListItem e.g. "research".
        /// </summary>
        /// <param name="item">The current ListItem target.</param>
        /// <returns>The relative Site path of the parent Site for the current
		/// ListItem.</returns>
		public static string GetParentSiteUrl(this ListItem item)
		{
			return item.WebUrl.ToLower().Replace(
				item.WebUrl.ToLower().Right(GetParentSiteRelativeUrl(item)),
				"");
		}

        /// <summary>
        /// A method to return the Tenant host value of the parent Tenant for
		/// the current ListItem e.g. "crayveon.sharepoint.us".
        /// </summary>
        /// <param name="item">The current ListItem target.</param>
        /// <returns>The Tenant host value of the parent Tenant.</returns>
		public static string GetParentTenantHost(this ListItem item)
		{
			return item.ParentReference.SiteId.Split(',')[0];
		}

        /// <summary>
        /// A method to return the Web Id GUID of the parent Web for the 
        /// current ListItem.
        /// </summary>
        /// <param name="item">The current ListItem target.</param>
        /// <returns>The Web Id GUID of the parent Web.</returns>
		public static string GetParentWebId(this ListItem item)
        {
            return item.ParentReference.SiteId.Split(',')[2];
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

		/// <summary>
		/// A method to take the given field from the current ListItem and
		/// convert it to the LookupId field in the parent list, then take
		/// the value from that field and cross correlate it with the related
		/// user from the parent Site's User Information List which yields the
		/// actual user's Email that is then returned.
		/// </summary>
		/// <param name="item">The current ListItem.</param>
		/// <param name="userFieldName">The name of the Field in the current
		/// ListItem that is to be used in finding the user's Email e.g. for
		/// the User who created a given ListItem in SharePoint, the Field is
		/// named "Author" in SharePoint, which translates to the Field name
		/// "AuthorLookupId" in Graph.</param>
		/// <returns>The target User's Email address.</returns>
		public static string GetSiteUserEmail(this ListItem item,
											  string userFieldName)
		{
			return Graph.GetUserEmailUpn(
				item.Fields.AdditionalData[userFieldName + "LookupId"].ToString(),
				Identity.AuthMan.ActiveAuth.GraphClient);
		}

		public static System.Collections.Generic.Dictionary<string, byte[]> GetVersions(
			this ListItem listItem,
			GraphServiceClient graphClient,
			string metaDataFieldName = null)
		{
			//Create the aggregation container.
			System.Collections.Generic.Dictionary<string, byte[]> listItemVersionsBytes =
				new System.Collections.Generic.Dictionary<string, byte[]>();
			//Push the parent List and Site to their stacks.
			if (!Identity.AuthMan.ActiveAuth.ActiveSiteStack.ContainsKey(listItem.WebUrl))
			{
				Identity.AuthMan.ActiveAuth.ActiveSiteStack.Add(
					listItem.WebUrl,
					graphClient.Sites[listItem.GetParentSiteId()]
					.GetAsync().GetAwaiter().GetResult());
			}
			if (!Identity.AuthMan.ActiveAuth.ActiveListStack.ContainsKey(listItem.WebUrl))
			{
				Identity.AuthMan.ActiveAuth.ActiveListStack.Add(
					listItem.WebUrl,
					graphClient.Sites[listItem.GetParentSiteId()]
					.Lists[listItem.GetParentListId()]
					.GetAsync().GetAwaiter().GetResult());
			}
			//Get the list of ListItemVersion objects.
			var listItemVersions = listItem.GetItemVersions(ref graphClient);
			listItemVersions.Reverse();
			//Switch Auth context to SharePoint for CSOM.
			Identity.AuthMan.GetAuth(Identity.ScopeType.SharePoint);
            //Get the list of file versions.
            var ctx = Identity.AuthMan.GetClientContext(listItem.GetParentSiteUrl());
			var file = ctx.Web.GetFileByUrl(listItem.WebUrl);
			ctx.Load(file);
			ctx.Load(file.Versions);
			ctx.ExecuteQuery();
			//Iterate each version in the list and process.
			for (int C = 0; C < listItemVersions.Count - 1; C++)
			{
				var binaryStream = file.Versions[C].OpenBinaryStream();
				ctx.ExecuteQuery();
				MemoryStream memoryStream = new MemoryStream();
				binaryStream.Value.CopyTo(memoryStream);
				binaryStream.Value.Close();
				listItemVersionsBytes.Add(
					$"{listItem.WebUrl}---" +
					$"{file.Versions[C].VersionLabel}" +
					(metaDataFieldName == null ? "" :
						$"---{listItemVersions[C].Fields.AdditionalData[metaDataFieldName]}") +
					$".txt",
					memoryStream.ToArray());
				memoryStream.Close();
			}
			//Switch Auth context back to Graph.
			Identity.AuthMan.GetAuth();
			byte[] bytes = new byte[(int)listItem.DriveItem.Size];
			//Get the current version of the item using REST.
			var response = Identity.AuthMan.ActiveAuth.HttpClient.GetAsync(
				listItem.DriveItem.AdditionalData["@microsoft.graph.downloadUrl"].ToString())
				.GetAwaiter().GetResult();
			var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
			stream.Read(bytes, 0, (int)stream.Length);
			stream.Close();
            listItemVersionsBytes.Add(
                $"{listItem.WebUrl}---" +
                $"{listItemVersions[listItemVersions.Count - 1].Id}" +
                (metaDataFieldName == null ? "" :
                    $"---{listItemVersions[listItemVersions.Count - 1]
						.Fields.AdditionalData[metaDataFieldName]}") +
                $".txt",
                bytes);
			//Return the aggregated results.
            return listItemVersionsBytes;
        }

		/// <summary>
		/// A method to restore a given Version of the current ListItem.
		/// </summary>
		/// <param name="item">The current ListItem.</param>
		/// <param name="version">The version string of the target version that
		/// needs to be restored e.g. "3.0".</param>
		/// <param name="graphClient">An optional GraphServiceClient to use for
		/// the action.  If not specified, it will use the 
		/// ActiveAuth.GraphClient value.</param>
		public static void RestoreVersion(this ListItem item,
                                          string version,
                                          GraphServiceClient graphClient = null)
		{
			if (graphClient == null)
			{
				graphClient = Identity.AuthMan.ActiveAuth.GraphClient;
			}
            graphClient.Sites[item.ParentReference.SiteId]
				.Lists[item.Fields.AdditionalData["ParentListId"].ToString()]
				.Items[item.Id]
				.Versions[version]
				.RestoreVersion
				.PostAsync()
				.GetAwaiter().GetResult();
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
				var emailUpn = item.GetSiteUserEmail(userFieldName);
                return Graph.GetUserByEmail(emailUpn);
			}
			catch (Exception ex)
			{
				Logit.Err(ex.ToString());
				return null;
			}
		}
    }
}
