﻿#pragma warning disable CA1416, CS0162, CS0168, CS1587, CS1591, CS1998, IDE0028, IDE0059

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace System
{
    /// <summary>
    /// Logging class with options for targeting screen, file, Event Log,
    /// SharePoint list, ILogger and database.
    /// </summary>
    [Serializable]
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
            public ILogger ILogger { get; private set; } = null;
            /// <summary>
            /// A Graph client for logging to SharePoint.
            /// </summary>
            public GraphServiceClient GraphClient { get; private set; } = null;
            /// <summary>
            /// The base URL of the SharePoint site housing the target list.
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
                activeLogitInstance = this;
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

        #region Globals
        /// <summary>
        /// The lock object to prevent file I/O clashes.
        /// </summary>
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        /// <summary>
        /// Lock object for marshalling static console location output.
        /// </summary>
        private static readonly object _lockStaticConsoleLocation = new object();
        /// <summary>
        /// Define the type of messages that can be written.
        /// </summary>
        public enum MessageType
        {
            Error,
            Warning,
            Information,
            Verbose
        }
        /// <summary>
        /// Define the types of logs that can be written.
        /// </summary>
        public enum LogType
        {
            Console,
            File,
            EventLog,
            SPList,
            ILogger,
            Database
        }
        /// <summary>
        /// The currently active Logit engine instance that will handle
        /// requests.
        /// </summary>
        public static Instance activeLogitInstance { get; set; }
            = new Instance(IsDebug);
        #endregion Globals

        #region Debug Switch
        /// <summary>
        /// Default delegate method to enable logging.
        /// </summary>
        /// <returns>Always true when no delegate is provided.</returns>
        public static bool IsDebug()
        {
            return true;
        }

        /// <summary>
        /// Intended method for calling delegate debug switch determinors.
        /// </summary>
        /// <param name="func">The delegate method to call.</param>
        /// <returns>The return value of the delegate method.</returns>
        public static bool IsDebug(Func<bool> func)
        {
            return func();
        }
        #endregion Debug Switch

        #region Utility Methods
        /// <summary>
        /// This method will save the current console foreground color
        /// and then write the parameter passed message to the console
        /// output in different colors based on the MessageType before
        /// restoring the console foreground to the original color it had
        /// before the method was called.  Colors are red for Error, yellow
        /// for Warning, gray for Information and cyan for Verbose.
        /// </summary>
        /// <param name="message">Output to write.</param>
        /// <param name="isDebugValidationMethod">The name of the delegate method
        /// to call to determine if debug logging should happen.</param>
        /// <param name="messageType">The type of message to write.  Default
        /// value is MessageType.Information.</param>
        private static void writeConsoleMessage(
            string message,
            Func<bool> isDebugValidationMethod = null,
            MessageType messageType = MessageType.Information)
        {
            //Save the current console foreground color.
            var foreground = Console.ForegroundColor;
            //Save the current console background color.
            var background = Console.BackgroundColor;
            try
            {
                //Check if a debug validation method was specified.
                if (isDebugValidationMethod != null)
                {
                    //Check if debug is on.  If not, no logging takes place.
                    if (IsDebug(isDebugValidationMethod))
                    {
                        //Set console output to:
                        //red for error,
                        //yellow for warning,
                        //gray for information,
                        //cyan for verbose.
                        switch (messageType)
                        {
                            case MessageType.Error:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case MessageType.Warning:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case MessageType.Information:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;
                            case MessageType.Verbose:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                        }
                        //Default the background to black.
                        Console.BackgroundColor = ConsoleColor.Black;
                        //Write the error.
                        Console.WriteLine(message);
                        //Reset the console colors.
                        Console.ForegroundColor = foreground;
                        Console.BackgroundColor = background;
                    }
                }
            }
            catch (Exception ex)
            {
                //Logging code may NEVER terminate its parent through exceptions.
                try
                {
                    //Write the error in red to the console.
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ForegroundColor = foreground;
                    Console.BackgroundColor = background;
                }
                catch (Exception ex2)
                {
                    //Just reset the console colors.
                    Console.ForegroundColor = foreground;
                    Console.BackgroundColor = background;
                }
            }
        }

        /// <summary>
        /// This method will save the current console foreground color
        /// and then write the parameter passed message to the console
        /// output in different colors based on the MessageType before
        /// restoring the console foreground to the original color it had
        /// before the method was called.  Colors are red for Error, yellow
        /// for Warning, gray for Information and cyan for Verbose.
        /// </summary>
        /// <param name="message">Output to write.</param>
        /// <param name="isDebugValidationMethod">The name of the delegate method
        /// to call to determine if debug logging should happen.</param>
        public static void writeConsoleError(
            string message,
            Func<bool> isDebugValidationMethod = null)
        {
            writeConsoleMessage(message, isDebugValidationMethod, MessageType.Error);
        }

        /// <summary>
        /// Turn the currently running assembly name into a pathed file name
        /// string that can be used as a log file name.
        /// This method can also be called with a (false) parameter to simply
        /// return the fully qualified path to the current executing assembly
        /// file name.
        /// </summary>
        /// <param name="noSpacesinFileName">If true, spaces are replaced
        /// with underscores.</param>
        /// <returns>Unique string representing the fully qualified path to
        /// the executing assembly with spaces in the file name portion of
        /// the string replaced by underscores e.g. 
        /// C:\Users\CvD\My Documents\App Name.exe
        /// becomes
        /// C:\Users\CvD\My_Documents\App_Name.exe</returns>
        public static string GetExecutingAssemblyFileName(
            bool noSpacesinFileName = true)
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
                return Path.GetDirectoryName(
                    Reflection.Assembly.GetEntryAssembly().Location)
                    .TrimEnd('\\') + "\\" + //Ensure no double trailing slash.
                    Uri.EscapeDataString(
                    Reflection.Assembly.GetEntryAssembly()
                    .ManifestModule.Name.Replace(" ", "_"));
            }
            catch (Exception ex)
            {
                //Write exception info to the console in default red.
                writeConsoleError(ex.ToString());
                //Return empty string instead of null as null could cause an
                //exception to be thrown by the calling code.
                return "";
            }
        }

        /// <summary>
        /// Strip "http://" and "https://" headers from URLs and replace
        /// forward slashes (/) with underscored (_) and spaces with
        /// dashes (-).
        /// This is useful for turning a web URL into a log file name or
        /// part thereof.
        /// </summary>
        /// <param name="url">The URL to transform.</param>
        /// <returns>The transformed URL value.</returns>
        public static string htmlStrip(string url)
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
                return url.ToLower().Replace("https://", "")
                                    .Replace("http://", "")
                                    .Replace("/", "_")
                                    .Replace(" ", "-");
            }
            catch (Exception ex)
            {
                //Write exception info to the console in default red.
                writeConsoleError(ex.ToString());
                //Return empty string instead of null as null could cause an
                //exception to be thrown by the calling code.
                return "";
            }
        }

        /// <summary>
        /// This method returns the current Zulu (GMT) date/time value as a
        /// string stamp in the format yyyy-MM-dd@hh.mm.ss.ttt 
        /// e.g. 2017-07-04@09.03.01.567Z
        /// </summary>
        /// <returns>String stamp in the format yyyy-MM-dd@hh.mm.ss.ttt
        /// e.g. 2017-07-04@09.03.01.567Z</returns>
        public static string timeStampZulu()
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
                var time = DateTime.UtcNow;
                return time.Year.ToString() + "-" +
                    time.Month.ToString("d2") + "-" +
                    time.Day.ToString("d2") + "@" +
                    time.Hour.ToString("d2") + "." +
                    time.Minute.ToString("d2") + "." +
                    time.Second.ToString("d2") + "." +
                    time.Millisecond.ToString("d3");
            }
            catch (Exception ex)
            {
                //Write exception info to the console in default red.
                writeConsoleError(ex.ToString());
                //Return empty string instead of null as null could cause an
                //exception to be thrown by the calling code.
                return "";
            }
        }

        /// <summary>
        /// This method returns the current local date/time value as a
        /// string stamp in the format yyyy-MM-dd@hh.mm.ss.ttt 
        /// e.g. 2017-07-04@09.03.01.567Z
        /// </summary>
        /// <returns>String stamp in the format yyyy-MM-dd@hh.mm.ss.ttt
        /// e.g. 2017-07-04@09.03.01.567Z</returns>
        public static string TimeStamp()
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
                var time = DateTime.UtcNow;
                return time.Year.ToString() + "-" +
                    time.Month.ToString("d2") + "-" +
                    time.Day.ToString("d2") + "@" +
                    time.Hour.ToString("d2") + "." +
                    time.Minute.ToString("d2") + "." +
                    time.Second.ToString("d2") + "." +
                    time.Millisecond.ToString("d3");
            }
            catch (Exception ex)
            {
                //Write exception info to the console in default red.
                writeConsoleError(ex.ToString());
                //Return empty string instead of null as null could cause an
                //exception to be thrown by the calling code.
                return "";
            }
        }

        /// <summary>
        /// This method returns the executing computer's Fully Qualified
        /// Domain Name.
        /// </summary>
        /// <returns>FQDN</returns>
        public static string getFQDN()
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
                string domainName =
                    System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                string hostName = System.Net.Dns.GetHostName();
                domainName = "." + domainName;
                if (!hostName.EndsWith(domainName))
                {
                    hostName += domainName;
                }
                return hostName;
            }
            catch (Exception ex)
            {
                //Write exception info to the console in default red.
                writeConsoleError(ex.ToString());
                //Return empty string instead of null as null could cause an
                //exception to be thrown by the calling code.
                return "";
            }
        }

        /// <summary>
        /// Used to build output text with the date/time stamp
        /// depending on the value of the includeTimeStamp switch.
        /// </summary>
        /// <param name="message">The message to stamp if needed.</param>
        /// <param name="includeTimeStamp"></param>
        /// <returns></returns>
        public static string prependTimeStamp(
            string message,
            bool includeTimeStamp = false)
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
                if (!includeTimeStamp)
                {
                    //No timestamp needed.  Simply return the message.
                    return message;
                }
                //The inclusion of a comma (,) between the timestamp and the
                //message is intentional.  This allows for importing of log
                //file output into a spreadsheet via comma separated value
                //(CSV) format while splitting timestamps and messages for
                //sorting and filtering purposes.
                return TimeStamp() + "  >>>   ," + message;
            }
            catch (Exception ex)
            {
                //Write exception info to the console in default red.
                writeConsoleError(ex.ToString());
                return ex.ToString();
            }
        }
        #endregion Utility Methods

        #region Worker Methods
        /// <summary>
        /// Method to write a log to console.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreground">the foreground color of the console
        /// for the message.</param>
        /// <param name="background">The background color of the console
        /// for the message.  Defaults to black.</param>
        public async static void Log(
            string message,
            ConsoleColor foreground,
            ConsoleColor background = ConsoleColor.Black)
        {
            Log(message,
                0,
                MessageType.Information,
                null,
                activeLogitInstance.IsDebugMethod,
                foreground,
                background);
        }

        /// <summary>
        /// Log the message to the log file, provided the logging was
        /// initialized.
        /// </summary>
        /// <param name="message">The message being logged.</param>
        /// <param name="eventId">An event ID to use if logging to the
        /// system Event Log.</param>
        /// <param name="messageType">The message type to use.</param>
        /// <param name="instance">The Logit instance to use.  If null,
        /// the default instance is used.</param>
        /// <param name="isDebugValidationMethod">A delegate method provided
        /// which when called, will determine if the caller wants debug logging
        /// to take place or not.</param>
        /// <param name="foreground">The foreground color to use when
        /// logging to console.  Defaults to gray.</param>
        /// <param name="background">The background color to use when
        /// logging to console.  Defaults to black.</param>
        public async static void Log(
            string message,
            int eventId = 0,
            MessageType messageType = MessageType.Information,
            Instance instance = null,
            Func<bool> isDebugValidationMethod = null,
            ConsoleColor foreground = ConsoleColor.Gray,
            ConsoleColor background = ConsoleColor.Black)
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
                if (isDebugValidationMethod == null)
                {
                    if (instance.IsDebugMethod == null)
                    {
                        return;
                    }
                    else
                    {
                        isDebugValidationMethod = instance.IsDebugMethod;
                    }
                }
                //Check if a debug func was specified.
                if (isDebugValidationMethod != null)
                {
                    //Check if debug is enabled.
                    if (IsDebug(isDebugValidationMethod))
                    {
                        //Check if a Logit instance was passed.
                        if (instance == null)
                        {
                            instance = activeLogitInstance;
                        }
                        //Check if we should log to Console.
                        if (instance.LogToConsole)
                        {
                            //Save the current console colors.
                            var foreColor = Console.ForegroundColor;
                            var backColor = Console.BackgroundColor;
                            //Set console output to:
                            //red for error,
                            //yellow for warning,
                            //gray for information,
                            //cyan for verbose.
                            switch (messageType)
                            {
                                case MessageType.Information:
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    break;
                                case MessageType.Warning:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                case MessageType.Error:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case MessageType.Verbose:
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;
                            }
                            //Check if an override foreground color was specified.
                            if (foreColor != foreground)
                            {
                                Console.ForegroundColor = foreground;
                            }
                            //Check if an override background color was specified.
                            if (backColor != background)
                            {
                                Console.BackgroundColor = background;
                            }
                            //Are we keeping output in a static location?
                            if (instance.StaticConsoleLocation)
                            {
                                //Leverage the lock to ensure multiple threads
                                //don't try to write to console concurrently.
                                lock (_lockStaticConsoleLocation)
                                {
                                    //Grab the current cursor position.
                                    var cursorTop = Console.CursorTop;
                                    var cursorLeft = Console.CursorLeft;
                                    //Output the message with a large trailing
                                    //blank to clear previous output from the static
                                    //console line.
                                    Console.WriteLine(
                                        prependTimeStamp(message)
                                        .PadRight(120, ' '));
                                    //Reset the cursor after output.
                                    Console.CursorTop = cursorTop;
                                    Console.CursorLeft = cursorLeft;
                                }
                            }
                            //No static output location is required.
                            else
                            {
                                //Simply write the message to the console.
                                Console.WriteLine(prependTimeStamp(message));
                            }
                            //Reset console colors.
                            Console.ForegroundColor = foreColor;
                            Console.BackgroundColor = backColor;
                        }
                        //Check if should log to EventLog
                        if (instance.LogToEventLog)
                        {
                            //Set the default EventLog type.
                            Diagnostics.EventLogEntryType targetType =
                                Diagnostics.EventLogEntryType.Error;
                            //Change the default to match the message type.
                            switch (messageType)
                            {
                                case MessageType.Warning:
                                    targetType =
                                        System.Diagnostics.EventLogEntryType.Warning;
                                    break;
                                case MessageType.Information:
                                    targetType =
                                        System.Diagnostics.EventLogEntryType.Information;
                                    break;
                                case MessageType.Verbose:
                                    targetType =
                                        System.Diagnostics.EventLogEntryType.Information;
                                    break;
                            }
                            //Check if an event ID was specified.
                            if (eventId != 0)
                            {
                                //Event ID specified so include it in the write.
                                System.Diagnostics.EventLog.WriteEntry(
                                    "Application",
                                    message,
                                    targetType);
                            }
                            else
                            {
                                //No event ID so write without it.
                                System.Diagnostics.EventLog.WriteEntry(
                                    "Application",
                                    message,
                                    targetType);
                            }
                        }
                        //Check if we should log to file.
                        if (instance.LogToFile)
                        {
                            //Extra try/catch/finally to handle file locking.
                            try
                            {
                                //Obtain a lock, waiting up to 10 seconds to do so.
                                if (_lock.TryEnterWriteLock(10000))
                                {
                                    //Write to the configured log file.
                                    System.IO.File.AppendAllText(
                                        instance.LogFile,
                                        messageType.ToString() +
                                            ", " +
                                            prependTimeStamp(message) + "\n");
                                }
                            }
                            catch (Exception ex)
                            {
                                //Write exception info to the console in default red.
                                writeConsoleError(ex.ToString());
                                System.Diagnostics.EventLog.WriteEntry(
                                    "Application",
                                    ex.ToString(),
                                    System.Diagnostics.EventLogEntryType.Error);
                            }
                            finally
                            {
                                _lock.ExitWriteLock();
                            }
                        }
                        //Check if logging should be done to a SharePoint list.
                        if (instance.LogToSPList)
                        {
                            var client = instance.GraphClient;
                            if (client != null)
                            {
                                try
                                {
                                    var dic = new Dictionary<string, object>();
                                    var msg = message.Split("|||".ToCharArray());
                                    if (msg.Length == 2)
                                    {
                                        dic.Add("Title", msg[0]);
                                        dic.Add("Message", msg[1]);
                                    }
                                    else
                                    {
                                        dic.Add("Title", message);
                                    }
                                    var listItem = new Microsoft.Graph.ListItem
                                    {
                                        Fields = new Microsoft.Graph.FieldValueSet
                                        {
                                            AdditionalData = dic
                                        }
                                    };
                                    var result = client.Sites["root"]
                                        .SiteWithPath($"/sites/{instance.LogSiteUrl}")
                                        .Lists[instance.LogListName]
                                        .Items
                                        .Request()
                                        .AddAsync(listItem)
                                        .GetAwaiter().GetResult();
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.EventLog.WriteEntry(
                                        "Application",
                                        ex.ToString());
                                }
                            }
                        }
                        //Check if message should be logged to ILogger.
                        if (instance.LogToILogger)
                        {
                            switch (messageType)
                            {
                                case MessageType.Information:
                                    instance.ILogger.LogInformation(message);
                                    break;
                                case MessageType.Warning:
                                    instance.ILogger.LogWarning(message);
                                    break;
                                case MessageType.Error:
                                    instance.ILogger.LogError(message);
                                    break;
                                case MessageType.Verbose:
                                    instance.ILogger.LogInformation(message);
                                    break;
                            }
                        }
                        //Check if message should be logged to database.
                        if (instance.LogToDB)
                        {
                            throw new NotImplementedException("DB logging coming soon!");
                        }
                    }
                }
            }
            //Something went wrong.
            catch (Exception ex)
            {
                //Logging code may NEVER terminate its parent through exceptions.
                try
                {
                    if (instance == null)
                    {
                        instance = activeLogitInstance;
                    }
                    //Log exception to console and event log!
                    var fore = Console.ForegroundColor;
                    var back = Console.BackgroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    //Write console output, stamped if needed.
                    Console.WriteLine(prependTimeStamp(ex.ToString()));
                    //Reset the console foreground color.
                    Console.ForegroundColor = fore;
                    Console.BackgroundColor = back;
                    System.Diagnostics.EventLog.WriteEntry(
                        "Application",
                        ex.ToString(),
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch (Exception ex2)
                {
                    //Swallow the error.
                }
            }
        }

        /// <summary>
        /// Called to write "Information" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public async static void Inf(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (instance == null)
            {
                instance = activeLogitInstance;
            }
            Log(message, eventId, MessageType.Information, instance);
        }

        /// <summary>
        /// Called to write "Warning" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public async static void Wrn(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (instance == null)
            {
                instance = activeLogitInstance;
            }
            Log(message, eventId, MessageType.Warning, instance);
        }

        /// <summary>
        /// Called to write "Error" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public async static void Err(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (instance == null)
            {
                instance = activeLogitInstance;
            }
            Log(message, eventId, MessageType.Error, instance);
        }

        /// <summary>
        /// Called to write "Verbose" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public async static void Vrb(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (instance == null)
            {
                instance = activeLogitInstance;
            }
            Log(message, eventId, MessageType.Verbose, instance);
        }
        #endregion Worker Methods
    }
}

#pragma warning restore CA1416, CS0162, CS0168, CS1587, CS1591, CS1998, IDE0028, IDE0059