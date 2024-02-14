/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static Extensions.Core;  //NuGet Extensions.cs

namespace Extensions.Azure
{
    /// <summary>
    /// Container class for blob content.
    /// </summary>
    [Serializable]
    public partial class BlobObject:Object
    {
        /// <summary>
        /// The object being held.
        /// </summary>
        public object Obj { get; set; }

        /// <summary>
        /// Constructor to assign obj.
        /// </summary>
        /// <param name="obj">The object being contained.</param>
        public BlobObject(object obj)
        {
            Obj = obj;
        }
    }

    /// <summary>
    /// Class for working with blob data in Azure.
    /// </summary>
    [Serializable]
    public static partial class Blob
    {
        /// <summary>
        /// The currently active BlobServiceClient to use.
        /// </summary>
        public static BlobServiceClient ActiveService { get; private set; }
            = null;
        /// <summary>
        /// The currently active BlobContainerClient to use.
        /// </summary>
        public static BlobContainerClient ActiveContainer { get; private set; }
            = null;

        /// <summary>
        /// Initialization method for the class.
        /// </summary>
        public static void Init()
        {
            GetBlobServiceClient();
            GetBlobContainerClient();
        }

        /// <summary>
        /// Method returns the BlobServiceClient given the blobServiceUri.
        /// </summary>
        /// <param name="blobServiceUri">The URL of the client.</param>
        /// <returns>The BlobServiceClient.</returns>
        public static BlobServiceClient GetBlobServiceClient(
            string blobServiceUri = null)
        {
            try
            {
                //Check if we already have an active service.
                if (ActiveService != null)
                {
                    return ActiveService;
                }
                //Check if a URL was provided.
                if (blobServiceUri != null)
                {
                    //Create a BlobServiceClient from the provided URL.
                    Uri accountUri = new Uri(blobServiceUri);
                    ActiveService = new BlobServiceClient(
                        accountUri,
                        new DefaultAzureCredential());
                }
                else
                {
                    //Try and get the BlobServiceUri from environment variables.
                    Uri accountUri = new Uri(GetEnv("BlobServiceUri"));
                    ActiveService = new BlobServiceClient(
                        accountUri,
                        new DefaultAzureCredential());
                }
                return ActiveService;
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                return ActiveService;
            }
        }

        /// <summary>
        /// Method to get the BlobContainerClient given a connection string
        /// and container name.
        /// </summary>
        /// <param name="connectionString">The connection string for the
        /// client.</param>
        /// <param name="containerName">The name of the container to which
        /// connection is sought.</param>
        /// <returns>The BlobContainerClient object.</returns>
        public static BlobContainerClient GetBlobContainerClient(
            string connectionString = null,
            string containerName = null)
        {
            try
            {
                //If we already have an active container, return it.
                if (ActiveContainer != null)
                {
                    return ActiveContainer;
                }
                //If no connection string is given, get it from environment.
                if (connectionString == null)
                {
                    connectionString = GetEnv("ConnectionString");
                }
                //If no container name is given, get the default from environment.
                if (containerName == null)
                {
                    containerName = GetEnv("DefaultBlobContainer");
                    //If not environment container name is found, default it.
                    if (containerName == null)
                    {
                        containerName = "default-blob-container";
                    }
                    else
                    {
                        containerName = containerName.ToLower();
                    }
                }
                ActiveContainer = new BlobContainerClient(connectionString,
                                                          containerName);
                return ActiveContainer;
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                return ActiveContainer;
            }
        }

        /// <summary>
        /// Relay method to get blobs from an Azure container.
        /// </summary>
        /// <returns>A list of blobs.</returns>
        public static List<BlobItem> GetBlobs()
        {
            return GetBlobContainerClient().GetBlobs().ToList();
        }

        /// <summary>
        /// Relay method to get the BlobClient.
        /// </summary>
        /// <param name="blobName">The name of the client.</param>
        /// <returns>The BlobClient.</returns>
        public static BlobClient GetBlobClient(string blobName)
        {
            return GetBlobContainerClient().GetBlobClient(blobName);
        }

        /// <summary>
        /// Method to write an item to the blob.
        /// </summary>
        /// <param name="blobName">The name of the target blob.</param>
        /// <param name="item">The item to write to the blob.</param>
        /// <returns>The Azure response from the upload.</returns>
        public static object WriteToBlob(string blobName, object item)
        {
            var client = GetBlobClient(blobName);
            object result = null;
            //Encapsulate the item in a BlobObject.
            BlobObject obj = new BlobObject(item);
            using (MemoryStream ms = new MemoryStream())
            {
                //Read the encapsulated item to memory.
                JsonSerializer.Serialize(ms, obj);
                ms.Position = 0;
                //Upload the memory stream.
                result = client.Upload(ms);
            }
            return result;
        }

        /// <summary>
        /// Method to read from a blob.
        /// </summary>
        /// <param name="blobName">The name of the blob to read.</param>
        /// <returns>The content from the blob.</returns>
        public static BlobObject ReadFromBlob(string blobName)
        {
            var client = GetBlobClient(blobName);
            MemoryStream ms = new MemoryStream();
            //Download the blob content to memory.
            client.DownloadTo(ms);
            ms.Position = 0;
            //Write the memory stream to an object.
            BlobObject result = JsonSerializer.Deserialize<BlobObject>(ms);
            return result;
        }
    }
}
