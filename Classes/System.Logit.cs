#pragma warning disable CA1416, CS0162, CS0168, CS1587, CS1591, CS1998, IDE0059, IDE0028

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

namespace System
{
    /// <summary>
    /// Logging class with options for targeting screen, file, Event Log,
    /// SharePoint list and database.
    /// </summary>
    [Serializable]
    public static partial class Logit
    {
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
            Database
        }
        public static global::Logit.Instance activeLogitInstance { get; set; }
            = new global::Logit.Instance(IsDebug);
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
        /// <param name="messageType">The type of message to write.</param>
        private static void writeConsoleMessage(
            string message,
            Func<bool> isDebugValidationMethod = null,
            MessageType messageType = MessageType.Error)
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
                writeConsoleMessage(ex.ToString());
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
                writeConsoleMessage(ex.ToString());
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
                writeConsoleMessage(ex.ToString());
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
                writeConsoleMessage(ex.ToString());
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
                writeConsoleMessage(ex.ToString());
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
                writeConsoleMessage(ex.ToString());
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
            global::Logit.Instance instance = null,
            Func<bool> isDebugValidationMethod = null,
            ConsoleColor foreground = ConsoleColor.Gray,
            ConsoleColor background = ConsoleColor.Black)
        {
            //Logging code may NEVER terminate its parent through exceptions.
            try
            {
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
                            System.Diagnostics.EventLogEntryType targetType =
                                System.Diagnostics.EventLogEntryType.Error;
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
                                writeConsoleMessage(ex.ToString());
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
                                    var listItem = new Microsoft.Graph.ListItem
                                    {
                                        Fields = new Microsoft.Graph.FieldValueSet
                                        {
                                            AdditionalData = new Dictionary<string, object>()
                                            {
                                                {"Title", message}
                                            }
                                        }
                                    };
                                    var result = client.Sites["root"]
                                        .SiteWithPath($"/sites/{instance.LogSiteBaseUrl}")
                                        .Lists[instance.LogListGuid]
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
            global::Logit.Instance instance = null)
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
            global::Logit.Instance instance = null)
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
            global::Logit.Instance instance = null)
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
            global::Logit.Instance instance = null)
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

#pragma warning restore CA1416, CS0162, CS0168, CS1587, CS1591, CS1998, IDE0059, IDE0028
