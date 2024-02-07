#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0168, CS1587, CS1998, IDE0028, IDE0059, IDE0060

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    /// <summary>
    /// A class for exception capture and return during bulk operations.
    /// </summary>
    [Serializable]
    public partial class InstanceExceptionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<Exception> Exceptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> BinaryData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool MultiThreaded { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public InstanceExceptionInfo() { }

        /// <summary>
        /// Inline constructor.
        /// </summary>
        /// <param name="key">The object key.</param>
        /// <param name="exceptions">A list of Exception values to return.</param>
        /// <param name="metaData">A dictionary of metadata to return.</param>
        /// <param name="binaryData">A dictionary of any binary data to return.</param>
        /// <param name="multiThreaded">Boolean switch indicating if the operation was
        /// multi-threaded or single-threaded at the time.</param>
        public InstanceExceptionInfo(string key, 
                                     List<Exception> exceptions, 
                                     Dictionary<string, string> metaData, 
                                     Dictionary<string, object> binaryData, 
                                     bool multiThreaded)
        {
            Key = key;
            Exceptions = exceptions;
            MetaData = metaData;
            BinaryData = binaryData;
            MultiThreaded = multiThreaded;
        }
    }
}
#pragma warning restore CS0168, CS1587, CS1998, IDE0028, IDE0059, IDE0079
