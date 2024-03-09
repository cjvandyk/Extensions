/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;

namespace Extensions
{
    /// <summary>
    /// An extension class to simplify getting data from DriveItem.
    /// </summary>
    [Serializable]
    public static partial class DriveItemExtensions
    {
        /// <summary>
        /// A method to download all the historical binary versions of a
        /// Microsoft.Graph.Models.DriveItem object to a specified folder or
        /// if no folder is specified, to the current user's Downloads folder.
        /// </summary>
        /// <param name="driveItem">The target DriveItem to use for getting 
        /// versions.</param>
        /// <param name="graphClient">An authenticated GraphServiceClient to
        /// use in the retrieval operation.</param>
        /// <param name="downloadPath">An optional string parameter to specify
        /// the target download folder.  NOTE: The folder must already exist
        /// and the current user must have write permissions to it.</param>
        /// <returns>An integer representing the number of files that was
        /// downloaded.</returns>
        public static int DownloadVersions(this DriveItem driveItem,
                                           GraphServiceClient graphClient,
                                           string downloadPath = "")
        {
            //If no downloadPath was specified, default to the user's Downloads.
            if (downloadPath == "")
            {
                downloadPath = "%appdata%\\..\\..\\Downloads\\";
            }
            else
            {
                //If specified, ensure the downloadPath ends in a backslash.
                downloadPath = downloadPath.TrimEnd('\\') + "\\";
            }
            //Get the versions.
            var versions = driveItem.GetVersions(graphClient);
            //Iterate the versions and write to file.
            foreach (var version in versions)
            {
                System.IO.File.WriteAllBytes(downloadPath + version.Key, 
                                             version.Value);
            }
            return versions.Count;
        }

        /// <summary>
        /// A method to get all the historical binary versions of a
        /// Microsoft.Graph.Models.DriveItem object.
        /// </summary>
        /// <param name="driveItem">The target DriveItem to use for getting 
        /// versions.</param>
        /// <param name="graphClient">An authenticated GraphServiceClient to
        /// use in the retrieval operation.</param>
        /// <returns>A dictionary of all binary versions of the given 
        /// DriveItem.</returns>
        /// <exception cref="Exception">Exceptions are thrown if version
        /// information is corrupted.</exception>
        public static System.Collections.Generic.Dictionary<string, byte[]> GetVersions(
            this DriveItem driveItem,
            GraphServiceClient graphClient)
        {
            //Create the aggregation container.
            System.Collections.Generic.Dictionary<string, byte[]> driveItemVersionsBytes =
                new System.Collections.Generic.Dictionary<string, byte[]>();
            //Get the list of DriveItemVersion objects.
            var driveItemVersions = driveItem.GetDriveItemVersions(ref graphClient);
            //Iterate each version in the list and process.
            bool currentVersion = true;
            int versionNumber = driveItemVersions.Count;
            foreach (var driveItemVersion in driveItemVersions)
            {
                //Ensure the version has a valid size.
                if (driveItemVersion.Size == null)
                {
                    throw new Exception($"DriveItemVersion [{driveItemVersion.Id}] " +
                        $"Size is null.");
                }
                //Define the receiving buffer for the version file.
                var bytes = new byte[(int)driveItemVersion.Size];
                //Open a stream to the version file.
                System.IO.Stream stream = null;
                if (currentVersion)
                {
                    stream = graphClient
                        .Drives[driveItem.ParentReference.DriveId]
                        .Items[driveItem.Id]
                        .Content
                        .GetAsync((C) =>
                        {
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    currentVersion = false;
                }
                else
                {
                    stream = graphClient
                        .Drives[driveItem.ParentReference.DriveId]
                        .Items[driveItem.Id]
                        .Versions[driveItemVersion.Id]
                        .Content
                        .GetAsync((C) =>
                        {
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                }
                //Ensure the stream is valid.
                if (stream == null)
                {
                    throw new Exception("DriveItem Content stream is empty.");
                }
                //Read the stream to the buffer.
                int read = stream.Read(bytes, 0, (int)driveItemVersion.Size);
                //Ensure the bytes read match the size reported.
                if (read != (int)driveItemVersion.Size)
                {
                    byte[] file = new byte[read]; 
                    Buffer.BlockCopy(bytes, 0, file, 0, read);
                    //Lock the aggregation container before aggregating results.
                    lock (driveItemVersionsBytes)
                    {
                        //Aggregate to the dictionary using the name of the 
                        //DriveItem as well as the modification date/time of
                        //the DriveItemVersion in string format.
                        driveItemVersionsBytes.Add(
                            driveItem.Name + "-" +
                                driveItemVersion.LastModifiedDateTime
                                .Value.DateTime
                                .ToString("yyyyMMdd-HHmmss.fff"),
                            file);
                    }
                }
                else
                {
                    //Lock the aggregation container before aggregating results.
                    lock (driveItemVersionsBytes)
                    {
                        //Aggregate to the dictionary using the name of the 
                        //DriveItem as well as the modification date/time of
                        //the DriveItemVersion in string format.
                        driveItemVersionsBytes.Add(
                            driveItem.Name + "-" +
                                driveItemVersion.LastModifiedDateTime
                                .Value.DateTime
                                .ToString("yyyyMMdd-HHmmss.fff"),
                            bytes);
                    }
                }
            }
            return driveItemVersionsBytes;
        }

        /// <summary>
        /// A method for retrieving all the versions of a 
        /// Microsoft.Graph.Models.DriveItem object.
        /// </summary>
        /// <param name="driveItem">The target DriveItem for which to get the
        /// versions.</param>
        /// <param name="graphClient">An authenticated GraphServiceClient to
        /// use in the retrieval operation.</param>
        /// <returns>A list of DriveItemVersion objects.</returns>
        public static System.Collections.Generic.List<DriveItemVersion> GetDriveItemVersions(
            this DriveItem driveItem,
            ref GraphServiceClient graphClient)
        {
            //Create the container.
            System.Collections.Generic.List<DriveItemVersion> driveItemVersions =
                new System.Collections.Generic.List<DriveItemVersion>();
            //Get the first page.
            var driveItemVersionsPage = graphClient
                .Drives[driveItem.ParentReference.DriveId]
                .Items[driveItem.Id]
                .Versions
                .GetAsync((C) =>
                {
                    C.Headers.Add("ConsistencyLevel", "eventual");
                }).GetAwaiter().GetResult();
            //If there are results, aggregate them.
            if (driveItemVersionsPage.Value != null)
            {
                lock (driveItemVersions)
                {
                    driveItemVersions.AddRange(driveItemVersionsPage.Value);
                }
            }
            //If there are more pages of results.
            while ((driveItemVersionsPage.Value != null) &&
                   (!string.IsNullOrEmpty(driveItemVersionsPage.OdataNextLink)))
            {
                //Get the next page of results.
                driveItemVersionsPage = graphClient
                    .Drives[driveItem.ParentReference.DriveId]
                    .Items[driveItem.Id].Versions
                    .WithUrl(driveItemVersionsPage.OdataNextLink)
                    .GetAsync((C) =>
                    {
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
                //Aggregate the results.
                lock (driveItemVersions)
                {
                    driveItemVersions.AddRange(driveItemVersionsPage.Value);
                }
            }
            return driveItemVersions;
        }
    }
}
