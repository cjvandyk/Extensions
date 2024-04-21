/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using static Extensions.Constants;

namespace System
{
    /// <summary>
    /// Logging class with options for targeting screen, file, Event Log,
    /// SharePoint list, ILogger and database.
    /// </summary>
    public static partial class Logit
    {
        /// <summary>
        /// Class containing the instance of the Logit engine.
        /// </summary>
        [Serializable]
        public partial class Instance
        {
            #region Globals
            /// <summary>
            /// The delegate method to call in order to determine if debug
            /// logging should take place.
            /// </summary>
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
            public ILogger ILogger { get; set; } = null;

            /// <summary>
            /// A Graph client for logging to SharePoint.
            /// </summary>
            public GraphServiceClient GraphClient { get; set; } = null;

            /// <summary>
            /// The base URL of the SharePoint site housing the target list.
            /// Specify only the base URL of the site e.g. for the site
            /// "contoso.sharepoint.us/sites/LogData" the value supplied would
            /// be just "LogData".
            /// </summary>
            public string LogSiteUrl { get; private set; } = null;

            /// <summary>
            /// The list name for the target list when logging to SharePoint.
            /// </summary>
            public string LogListName { get; private set; } = null;
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
            /// Fast constructor for using console logging.
            /// </summary>
            /// <param name="isDebugMethod">Optional delegate method to regulate logging.</param>
            public Instance(Func<bool> isDebugMethod = null)
            {
                LogToConsole = true;
                Finalize(isDebugMethod);
            }

            /// <summary>
            /// Fast constructor for using File logging.
            /// </summary>
            /// <param name="fileName">The target log file name.</param>
            /// <param name="isDebugMethod">Optional delegate method to regulate logging.</param>
            public Instance(string fileName,
                            Func<bool> isDebugMethod = null)
            {
                LogToFile = true;
                LogFile = GetExecutingAssemblyFileName() + TimeStamp() + ".log";
                Finalize(isDebugMethod);
            }

            /// <summary>
            /// Fast constructor for using the iLogger.
            /// </summary>
            /// <param name="iLogger">The iLogger to target.</param>
            /// <param name="isDebugMethod">Optional delegate method to regulate logging.</param>
            public Instance(ILogger iLogger,
                            Func<bool> isDebugMethod = null)
            {
                LogToILogger = true;
                ILogger = iLogger;
                Finalize(isDebugMethod);
            }

            /// <summary>
            /// Fast constructor for a single type of logging mechanism.
            /// </summary>
            /// <param name="logType">The type of logging mechanism needed.</param>
            /// <param name="isDebugMethod">Optional delegate method to regulate logging.</param>
            /// <param name="obj">Optional parameter with additional data.</param>
            /// <exception cref="NotImplementedException">Thrown if invalid type is passed.</exception>
            public Instance(LogType logType, 
                            Func<bool> isDebugMethod = null,
                            object obj = null)
            {
                switch (logType)
                {
                    case LogType.Console:
                        LogToConsole = true;
                        break;
                    case LogType.Database:
                        LogToDB = true;
                        break;
                    case LogType.EventLog:
                        LogToEventLog = true;
                        break;
                    case LogType.File:
                        LogToFile = true;
                        LogFile = GetExecutingAssemblyFileName() + TimeStamp() + ".log";
                        break;
                    case LogType.ILogger:
                        LogToILogger = true;
                        ILogger = (ILogger)obj;
                        break;
                    case LogType.SPList:
                        LogToSPList = true;
                        break;
                    default:
                        throw new NotImplementedException(
                            $"LogType: [{logType.ToString()}] is not valid.");
                }
                Finalize(isDebugMethod);
            }

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
                ILogger iLogger = null)
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
                ActiveLogitInstance = this;
            }

            /// <summary>
            /// Default constructor that logs only to the console.
            /// </summary>
            /// <param name="isDebugMethod">A delegate method called to determine
            /// if debug logging should happen.</param>
            /// <param name="graphClient">The GraphServiceClient to use for
            /// writing SharePoint list entries.</param>
            /// <param name="spSiteUrl">The base URL of the SharePoint site
            /// that houses the target list.</param>
            /// <param name="spListName">The GUID of the target SharePoint list
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
                string spSiteUrl,
                string spListName,
                MessageType defaultLogMessageType =
                    MessageType.Information,
                bool logToConsole = true,
                bool logToFile = false,
                bool logToEventLog = false,
                bool logToSPList = false,
                bool logToDatabase = false,
                bool logToILogger = false,
                ILogger iLogger = null)
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
                LogSiteUrl = spSiteUrl;
                //Configure the default SharePoint list for this session.
                LogListName = spListName;
                //Configure if logging should take place to a database.
                LogToDB = logToDatabase;
                //Configure the default log message type for this session.
                DefaultLogMessageType = defaultLogMessageType;
                //Update default instance to this.
                ActiveLogitInstance = this;
            }
            #endregion Constructors

            /// <summary>
            /// Internal method to finalize instance configuration.
            /// </summary>
            /// <param name="isDebugMethod">A delegate method if supplied.</param>
            internal void Finalize(Func<bool> isDebugMethod)
            {
                if (isDebugMethod != null)
                {
                    IsDebugMethod = isDebugMethod;
                }
                ActiveLogitInstance = this;
            }
        }
    }
}
