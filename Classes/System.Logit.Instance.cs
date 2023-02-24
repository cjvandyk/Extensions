#pragma warning disable CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using static System.Logit;

namespace Logit
{
    /// <summary>
    /// Class containing the instance of the Logit engine.
    /// </summary>
    [Serializable]
    public partial class Instance
    {
        #region Globals
        public Func<bool> IsDebugMethod { get; private set; }
        /// <summary>
        /// The file to which logging will be done if the logToFile
        /// switch is set to true.
        /// </summary>
        public string LogFile { get; private set; } =
            GetExecutingAssemblyFileName() + TimeStamp() + ".log";
        /// <summary>
        /// The default message type if none is specified.
        /// </summary>
        public MessageType DefaultLogMessageType { get; private set; }
            = MessageType.Information;
        /// <summary>
        /// The ILogger instance to use for logging.
        /// </summary>
        public Microsoft.Extensions.Logging.ILogger ILogger { get; private set; }
            = null;
        /// <summary>
        /// A Graph client for logging to SharePoint.
        /// </summary>
        public GraphServiceClient GraphClient { get; private set; } = null;
        /// <summary>
        /// The base URL of the SharePoint site housing the target list.
        /// </summary>
        public string LogSiteBaseUrl { get; private set; } = null;
        /// <summary>
        /// The list ID for the target list when logging to SharePoint.
        /// </summary>
        public string LogListGuid { get; private set; } = null;
        #endregion Globals

        #region Switches
        /// <summary>
        /// When set to true, will output logging content to the console.
        /// Default = true;
        /// </summary>
        public bool LogToConsole { get; set; } = true;
        /// <summary>
        /// When set to true, will output logging content to the log file.
        /// Default = false;
        /// </summary>
        public bool LogToFile { get; set; } = false;
        /// <summary>
        /// When set to true, will output logging content to the Event Log.
        /// Default = false;
        /// </summary>
        public bool LogToEventLog { get; set; } = false;
        /// <summary>
        /// When set to true, will output logging content to a provided
        /// ILogger client.
        /// </summary>
        public bool LogToILogger { get; set; } = false;
        /// <summary>
        /// When set to true, will output logging content to a SharePoint list.
        /// Default = false;
        /// </summary>
        public bool LogToSPList { get; set; } = false;
        /// <summary>
        /// When set to true, will output logging content to a database.
        /// Default = false;
        /// </summary>
        public bool LogToDB { get; set; } = false;
        /// <summary>
        /// Define if timestamps should be prepended to messages.
        /// Default = true;
        /// </summary>
        public bool IncludeTimeStamp { get; set; } = true;
        /// <summary>
        /// Define if console output should be in a static location thus
        /// constantly overwriting the previous output.  This is useful in
        /// status updates like % complete etc.
        /// Default = false;
        /// </summary>
        public bool StaticConsoleLocation { get; set; } = false;
        #endregion Switches

        #region Constructors
        /// <summary>
        /// Default constructor that logs only to the console.
        /// </summary>
        /// <param name="isDebugMethod">A delegate method called to determine
        /// if debug logging should happen.</param>
        /// <param name="defaultLogMessageType">The default message type to
        /// use in logging if type isn't specified.</param>
        /// <param name="logToConsole">Bool determining if logging is done to
        /// the console.</param>
        /// <param name="logToFile">Bool determining if logging is done to
        /// a file.</param>
        /// <param name="logToEventLog">Bool determining if logging is done to
        /// the Event Log.</param>
        /// <param name="logToILogger">Bool determining if logging is done to
        /// the provided ILogger.</param>
        /// <param name="iLogger">The ILogger instance to use for logging.</param>
        public Instance(
            Func<bool> isDebugMethod,
            MessageType defaultLogMessageType = MessageType.Information,
            bool logToConsole = true,
            bool logToFile = false,
            bool logToEventLog = false,
            bool logToILogger = false,
            Microsoft.Extensions.Logging.ILogger iLogger = null)
        {
            //Capture the delegate method.
            IsDebugMethod = isDebugMethod;
            //Configure if logging should take place to the console.
            LogToConsole = logToConsole;
            //Configure if logging should take place to file.
            LogToFile = logToFile;
            //Configure the default log file for this session.
            LogFile = GetExecutingAssemblyFileName() + TimeStamp() + ".log";
            //Configure if logging should take place to the Event Log.
            LogToEventLog = logToEventLog;
            //Configure the default log message type for this session.
            DefaultLogMessageType = defaultLogMessageType;
            //Configure if logging should take place to ILogger.
            LogToILogger = logToILogger;
            //Configure the default ILogger instance for this session.
            ILogger = iLogger;
            //Update default instance to this.
            activeLogitInstance = this;
        }

        /// <summary>
        /// Default constructor that logs only to the console.
        /// </summary>
        /// <param name="isDebugMethod">A delegate method called to determine
        /// if debug logging should happen.</param>
        /// <param name="graphClient">The GraphServiceClient to use for
        /// writing SharePoint list entries.</param>
        /// <param name="spSiteBaseUrl">The base URL of the SharePoint site
        /// that houses the target list.</param>
        /// <param name="spListGuid">The GUID of the target SharePoint list
        /// to which logging will be done.</param>
        /// <param name="defaultLogMessageType">The default message type to
        /// use in logging if type isn't specified.</param>
        /// <param name="logToConsole">Bool determining if logging is done to
        /// the console.</param>
        /// <param name="logToFile">Bool determining if logging is done to
        /// a file.</param>
        /// <param name="logToEventLog">Bool determining if logging is done to
        /// the Event Log.</param>
        /// <param name="logToSPList">Bool determining if logging is done to
        /// a SharePoint list.</param>
        /// <param name="logToDatabase">Bool determining if logging is done to
        /// a database.</param>
        /// <param name="logToILogger">Bool determining if logging is done to
        /// the provided ILogger.</param>
        /// <param name="iLogger">The ILogger instance to use for logging.</param>
        public Instance(
            Func<bool> isDebugMethod,
            GraphServiceClient graphClient,
            string spSiteBaseUrl,
            string spListGuid,
            MessageType defaultLogMessageType =
                MessageType.Information,
            bool logToConsole = true,
            bool logToFile = false,
            bool logToEventLog = false,
            bool logToSPList = false,
            bool logToDatabase = false,
            bool logToILogger = false,
            Microsoft.Extensions.Logging.ILogger iLogger = null)
        {
            //Capture the delegate method.
            IsDebugMethod = isDebugMethod;
            //Configure if logging should take place to the console.
            LogToConsole = logToConsole;
            //Configure if logging should take place to file.
            LogToFile = logToFile;
            //Configure the default log file for this session.
            LogFile = GetExecutingAssemblyFileName() + TimeStamp() + ".log";
            //Configure if logging should take place to the Event Log.
            LogToEventLog = logToEventLog;
            //Configure if logging should take place to ILogger.
            LogToILogger = logToILogger;
            //Configure the default ILogger instance for this session.
            ILogger = iLogger;
            //Configure if logging should take place to a SharePoint list.
            LogToSPList = logToSPList;
            //Configure the default GraphServiceClient to use for logging.
            GraphClient = graphClient;
            //Configure the default SharePoint site for this session.
            LogSiteBaseUrl = spSiteBaseUrl;
            //Configure the default SharePoint list for this session.
            LogListGuid = spListGuid;
            //Configure if logging should take place to a database.
            LogToDB = logToDatabase;
            //Configure the default log message type for this session.
            DefaultLogMessageType = defaultLogMessageType;
            //Update default instance to this.
            activeLogitInstance = this;
        }
        #endregion Constructors

        #region Worker Methods
        /// <summary>
        /// Called to write "Information" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        public async void Inf(
            string message,
            int eventId = 0)
        {
            System.Logit.Inf(message, eventId, this);
        }

        /// <summary>
        /// Called to write "Warning" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        public async void Wrn(
            string message,
            int eventId = 0)
        {
            System.Logit.Wrn(message, eventId, this);
        }

        /// <summary>
        /// Called to write "Error" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        public async void Err(
            string message,
            int eventId = 0)
        {
            System.Logit.Err(message, eventId, this);
        }

        /// <summary>
        /// Called to write "Verbose" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        public async void Vrb(
            string message,
            int eventId = 0)
        {
            System.Logit.Vrb(message, eventId, this);
        }
        #endregion Worker Methods
    }
}

#pragma warning restore CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028
