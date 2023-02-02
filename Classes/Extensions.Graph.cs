#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

using static Extensions.Identity.AuthMan;
using Microsoft.Graph;

namespace Extensions
{
    /// <summary>
    /// An extension class that makes working with Microsoft.Graph in GCCHigh
    /// environments easy.
    /// </summary>
    public static class Graph
    {
        /// <summary>
        /// Get the site ID (GUID) of the specified site.
        /// </summary>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <returns>A GUID value representing the ID of the site.</returns>
        public static string GetSiteId(string sitePath)
        {
            return (ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Request()
                        .GetAsync().GetAwaiter().GetResult()).Id;
        }

        /// <summary>
        /// Get a list of all sites in SharePoint.
        /// </summary>
        /// <returns>A list of all sites in SharePoint.</returns>
        public static List<Site> GetSites()
        {
            //Create the aggregation container.
            var sites = new List<Site>();
            //Get the first page of sites.
            var sitePage1 = ActiveAuth.GraphClient.Sites.Request()
                .GetAsync().GetAwaiter().GetResult();
            //Create the iterator for multiple pages.
            var sitePageIterator = PageIterator<Site>.CreatePageIterator(
                ActiveAuth.GraphClient, 
                sitePage1,
                (C) =>
                {
                    sites.Add(C);
                    return true;
                });
            //Iterate pages to get all data.
            sitePageIterator.IterateAsync().GetAwaiter().GetResult();
            return sites;
        }

        /// <summary>
        /// Get the list ID (GUID) of the specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <returns>A GUID value representing the ID of the list.</returns>
        public static string GetListId(string listName, string sitePath)
        {
            return (ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Lists[listName]
                        .Request()
                        .GetAsync().GetAwaiter().GetResult()).Id;
        }

        /// <summary>
        /// Get a specific item in a specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <param name="id">The ID value of the item e.g. "3"</param>
        /// <returns>A list of ListItem containing the single item.</returns>
        public static List<ListItem> GetListItem(
            string listName,
            string sitePath,
            string id)
        {
            return GetListItems(listName, sitePath, id);
        }

        /// <summary>
        /// Get all or just one item(s) in a specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <param name="id">The ID value of the item e.g. "3".  This value
        /// defaults to null and in such case, will result in all items being
        /// returned.</param>
        /// <returns>A list of ListItem containing the item(s).</returns>
        public static List<ListItem> GetListItems(
            string listName,
            string sitePath,
            string id = null)
        {
            //Create the aggregation container.
            List<ListItem> listItems = new List<ListItem>();
            //Check if a specific item was requested.
            if (id != null)
            {
                try
                {
                    //Get the specified item.
                    var listItem = ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Lists[listName]
                        .Items[id]
                        .Request()
                        .Expand("Fields")
                        .GetAsync().GetAwaiter().GetResult();
                    //Check if the item was found.
                    if (listItem != null)
                    {
                        listItems.Add(listItem);
                    }                        
                }
                catch (Exception ex)
                {
                    //Swallow exception if no with given id exist.
                }
            }
            else
            {
                //Create the first page.
                IListItemsCollectionPage listItemsPage1 = null;
                try
                {
                    //Get the first page of results.
                    listItemsPage1 = ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Lists[listName]
                        .Items
                        .Request()
                        .Expand("Fields")
                        .GetAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Requested site could not be found"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }
                //If not results were found, return the empty list.
                if (listItemsPage1 == null)
                {
                    return listItems;
                }
                //Results were found so create the page iterator.
                var listItemsPageIterator = PageIterator<ListItem>
                    .CreatePageIterator(ActiveAuth.GraphClient,
                                        listItemsPage1,
                                        (C) =>
                                        {
                                            listItems.Add(C);
                                            return true;
                                        });
                //Get all pages.
                listItemsPageIterator.IterateAsync().GetAwaiter().GetResult();
            }
            //Return the aggregated list.
            return listItems;
        }

        /// <summary>
        /// Update a specified list item with specified values.
        /// </summary>
        /// <param name="siteGuid">The site ID (GUID) value.</param>
        /// <param name="listGuid">The list ID (GUID) value.</param>
        /// <param name="listItemId">The ID value of the item e.g. "3"</param>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to update.</param>
        public static void UpdateListItemFields(
            string siteGuid,
            string listGuid,
            string listItemId,
            Dictionary<string, object> newFields)
        {
            ActiveAuth.GraphClient.Sites[siteGuid]
                                  .Lists[listGuid]
                                  .Items[listItemId]
                                  .Fields
                                  .Request()
                                  .UpdateAsync(CreateFieldValueSet(newFields))
                                  .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a new list item with specified values.
        /// </summary>
        /// <param name="siteGuid">The site ID (GUID) value.</param>
        /// <param name="listGuid">The list ID (GUID) value.</param>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to update.</param>
        public static void CreateListItem(
            string siteGuid,
            string listGuid,
            Dictionary<string, object> newFields)
        {
            ActiveAuth.GraphClient.Sites[siteGuid]
                                  .Lists[listGuid]
                                  .Items
                                  .Request()
                                  .AddAsync(CreateListItemJSON(newFields))
                                  .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a new FieldValueSet JSON object.
        /// </summary>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to use.</param>
        /// <returns>A FieldValueSet object populated with the values from
        /// the dictionary.</returns>
        internal static FieldValueSet CreateFieldValueSet(
            Dictionary<string, object> newFields)
        {
            return new FieldValueSet
            {
                AdditionalData = newFields
            };
        }

        /// <summary>
        /// Create a new ListItem JSON object.
        /// </summary>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to use.</param>
        /// <returns>A ListItem object populated with the values from the
        /// dictionary.</returns>
        internal static ListItem CreateListItemJSON(
            Dictionary<string, object> newFields)
        {
            return new ListItem
            {
                Fields = CreateFieldValueSet(newFields)
            };
        }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
