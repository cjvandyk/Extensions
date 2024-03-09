/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

namespace Extensions
{
    /// <summary>
    /// A class for exception capture and return during bulk operations.
    /// </summary>
    [Serializable]
    public partial class InstanceExceptionInfo
    {
        /// <summary>
        /// The string key of this instance.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// A list of Exceptions to be captured.
        /// </summary>
        public List<Exception> Exceptions { get; set; }

        /// <summary>
        /// A dictionary of any string metadata to be captured.
        /// </summary>
        public Dictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// A dictionary of any binary data to be captured.
        /// </summary>
        public Dictionary<string, object> BinaryData { get; set; }

        /// <summary>
        /// A boolean value indicating if the exception occurred while the
        /// app was configured for multi-threading.
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
        /// <param name="exceptions">A list of Exception values to capture.</param>
        /// <param name="metaData">A dictionary of metadata to capture.</param>
        /// <param name="binaryData">A dictionary of any binary data to capture.</param>
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

        /// <summary>
        /// Inline constructor.
        /// </summary>
        /// <param name="key">The object key.</param>
        /// <param name="exception">An Exception value to capture.</param>
        /// <param name="metaData">A string of metadata to capture.</param>
        /// <param name="binaryData">An object of any binary data to capture.</param>
        /// <param name="multiThreaded">Boolean switch indicating if the operation was
        /// multi-threaded or single-threaded at the time.</param>
        public InstanceExceptionInfo(string key,
                                     Exception exception,
                                     string metaData,
                                     object binaryData,
                                     bool multiThreaded)
        {
            Key = key;
            Exceptions.Add(exception);
            MetaData.Add(key, metaData);
            BinaryData.Add(key, binaryData);
            MultiThreaded = multiThreaded;
        }
    }
}
