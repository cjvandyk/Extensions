/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Extensions
{
    /// <summary>
    /// A container class to encapsulate objects serialized with .Load() and
    /// .Save().
    /// </summary>
    [Serializable]
    [KnownType(typeof(object))]
    [KnownType(typeof(List<DriveItem>))]
    [KnownType(typeof(System.Text.Json.JsonElement))]
    [KnownType(typeof(LoadSaveContainer))]
    [DataContract]
    public class LoadSaveContainer
    {
        /// <summary>
        /// The object being serialized.
        /// </summary>
        [DataMember]
        public object Value { get; set; }
    }
}