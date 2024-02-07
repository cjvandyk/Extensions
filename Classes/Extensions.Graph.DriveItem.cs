/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Extensions
{
    /// <summary>
    /// Extension class to add Extension Properties to any class.
    /// </summary>
    [Serializable]
    public static partial class GraphExtensions
    {
        [Serializable]
        [DataContract]
        public class DriveItem
        {
            [DataMember]
            public Microsoft.Graph.Models.IdentitySet CreatedBy { get; set; }

        
        }
    }
}