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
        public static System.Collections.Generic.Dictionary<string, byte[]> DownloadVersions(
            this DriveItem driveItem,
            GraphServiceClient graphClient)
        {
            //Create the aggregation container.
            System.Collections.Generic.Dictionary<string, byte[]> driveItemVersionsBytes =
                new System.Collections.Generic.Dictionary<string, byte[]>();
            //Get the list of DriveItemVersion objects.
            var driveItemVersions = driveItem.GetDriveItemVersions(ref graphClient);
            //Iterate each version in the list and process.
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
                using (var stream = graphClient
                    .Drives[driveItem.ParentReference.DriveId]
                    .Items[driveItemVersion.Id]
                    .Content
                    .GetAsync((C) =>
                    {
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult())
                {
                    //Ensure the stream is valid.
                    if (stream == null)
                    {
                        throw new Exception("DriveItem Content stream is empty.");
                    }
                    //Ensure stream is at the beginning.
                    stream.Position = 0;
                    //Read the stream to the buffer.
                    int read = stream.Read(bytes, 0, (int)driveItemVersion.Size);
                    //Ensure the bytes read match the size reported.
                    if (read != (int)driveItemVersion.Size)
                    {
                        throw new Exception(
                            $"Expected bytes [{(int)driveItemVersion.Size}] " +
                            $"Received bytes [{read}]");
                    }
                    //Lock the aggregation container before aggregating results.
                    lock (driveItemVersionsBytes)
                    {
                        //Aggregate to the dictionary using the name of the 
                        //DriveItem as well as the modification date/time of
                        //the DriveItemVersion in string format.
                        driveItemVersionsBytes.Add(
                            driveItem.Name + "-" + Convert.ToDateTime(
                                driveItemVersion.LastModifiedDateTime)
                                .ToString("yyyyMMdd-HHmmss.fff"), bytes);
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
