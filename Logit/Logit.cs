/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static Extensions.Constants;

namespace System
{
    /// <summary>
    /// Logging class with options for targeting screen, file, Event Log,
    /// SharePoint list, ILogger and database.
    /// </summary>
    [Serializable]
    public static partial class Logit
    {
        #region Globals
        /// <summary>
        /// The lock object to prevent file I/O clashes.
        /// </summary>
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Lock object for marshalling static console location output.
        /// </summary>
        private static readonly object _lockStaticConsoleLocation = new object();

        /// <summary>
        /// The currently active Logit engine instance that will handle
        /// requests.
        /// </summary>
        public static Instance ActiveLogitInstance { get; set; }
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
        private static void WriteConsoleMessage(
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
                catch
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
        public static void WriteConsoleError(
            string message,
            Func<bool> isDebugValidationMethod = null)
        {
            WriteConsoleMessage(message, isDebugValidationMethod, MessageType.Error);
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
                if (noSpacesinFileName)
                {
                    return Path.GetDirectoryName(
                        Reflection.Assembly.GetEntryAssembly().Location)
                        .TrimEnd('\\') + "\\" + //Ensure no double trailing slash.
                        Uri.EscapeDataString(
                        Reflection.Assembly.GetEntryAssembly()
                        .ManifestModule.Name.Replace(" ", "_"));
                }
                else
                {
                    return Path.GetDirectoryName(
                        Reflection.Assembly.GetEntryAssembly().Location)
                        .TrimEnd('\\') + "\\" + //Ensure no double trailing slash.
                        Uri.EscapeDataString(
                        Reflection.Assembly.GetEntryAssembly()
                        .ManifestModule.Name);
                }
            }
            catch (Exception ex)
            {
                //Write exception info to the console in default red.
                WriteConsoleError(ex.ToString());
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
        public static string HtmlStrip(string url)
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
                WriteConsoleError(ex.ToString());
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
        public static string TimeStampZulu()
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
                WriteConsoleError(ex.ToString());
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
                WriteConsoleError(ex.ToString());
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
        public static string GetFQDN()
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
                WriteConsoleError(ex.ToString());
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
        public static string PrependTimeStamp(
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
                WriteConsoleError(ex.ToString());
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
        public static void Log(
            string message,
            ConsoleColor foreground,
            ConsoleColor background = ConsoleColor.Black)
        {
            if (Extensions.Identity.AuthMan.TargetTenantConfig != null)
            {
                return;
            }
            Log(message,
                0,
                MessageType.Information,
                null,
                ActiveLogitInstance.IsDebugMethod,
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
        public static void Log(
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
                if (Extensions.Identity.AuthMan.TargetTenantConfig != null)
                {
                    return;
                }
                //Append the caller method to the back of the message.
                var stackTrace = new System.Diagnostics.StackTrace();
                //Iterate the stack trace frames.
                for (int C = 0; C < stackTrace.FrameCount; C++)
                {
                    //Get the method name.
                    var method = stackTrace.GetFrame(C).GetMethod();
                    var name = method.DeclaringType.FullName + "." + method.Name;
                    //Keep traversing up the stack if the method name is any
                    //of the Extensions logging methods in order to get from
                    //where logging was actually called.
                    if ((name != "System.Logit.Log") &&
                        (name != "System.Logit.Inf") &&
                        (name != "Extensions.Core.Inf") &&
                        (name != "Extensions.StackTraceExtensions.Parent"))
                    {
                        message += $">>>in>>>[{name}]";
                        //Found it, now abort traversion.
                        break;
                    }
                }
                //Check if a debug func was specified.
                if (isDebugValidationMethod != null)
                {
                    //Check if debug is enabled.
                    if (!IsDebug(isDebugValidationMethod))
                    {
                        return;
                    }
                }
                //Check if a Logit instance was passed.
                if (instance == null)
                {
                    instance = ActiveLogitInstance;
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
                                PrependTimeStamp(message)
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
                        Console.WriteLine(PrependTimeStamp(message));
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
                                    PrependTimeStamp(message) + "\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        //Write exception info to the console in default red.
                        WriteConsoleError(ex.ToString());
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
                            var listItem = new Microsoft.Graph.Models.ListItem
                            {
                                Fields = new Microsoft.Graph.Models.FieldValueSet
                                {
                                    AdditionalData = dic
                                }
                            };
                            var result = client.Sites[GetSiteId(
                                    instance.LogSiteUrl, 
                                    client)]
                                .Lists[GetListId(
                                    instance.LogListName,
                                    instance.LogSiteUrl,
                                    client)]
                                .Items
                                .PostAsync(listItem)
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
            //Something went wrong.
            catch (Exception ex)
            {
                //Logging code may NEVER terminate its parent through exceptions.
                try
                {
                    if (instance == null)
                    {
                        instance = ActiveLogitInstance;
                    }
                    //Log exception to console and event log!
                    var fore = Console.ForegroundColor;
                    var back = Console.BackgroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    //Write console output, stamped if needed.
                    Console.WriteLine(PrependTimeStamp(ex.ToString()));
                    //Reset the console foreground color.
                    Console.ForegroundColor = fore;
                    Console.BackgroundColor = back;
                    System.Diagnostics.EventLog.WriteEntry(
                        "Application",
                        ex.ToString(),
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch
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
        public static void Inf(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (Extensions.Identity.AuthMan.TargetTenantConfig != null)
            {
                return;
            }
            if (instance == null)
            {
                instance = ActiveLogitInstance;
            }
            Log(message, eventId, MessageType.Information, instance);
        }

        /// <summary>
        /// Called to write "Warning" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public static void Wrn(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (Extensions.Identity.AuthMan.TargetTenantConfig != null)
            {
                return;
            }
            if (instance == null)
            {
                instance = ActiveLogitInstance;
            }
            Log(message, eventId, MessageType.Warning, instance);
        }

        /// <summary>
        /// Called to write "Error" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public static void Err(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (Extensions.Identity.AuthMan.TargetTenantConfig != null)
            {
                return;
            }
            if (instance == null)
            {
                instance = ActiveLogitInstance;
            }
            Log(message, eventId, MessageType.Error, instance);
        }

        /// <summary>
        /// Called to write "Verbose" entries.
        /// </summary>
        /// <param name="message">The string message to log.</param>
        /// <param name="eventId">The Event Log event ID to use.</param>
        /// <param name="instance">Return value from Log() method.</param>
        public static void Vrb(
            string message,
            int eventId = 0,
            Instance instance = null)
        {
            if (Extensions.Identity.AuthMan.TargetTenantConfig == null)
            {
                return;
            }
            if (instance == null)
            {
                instance = ActiveLogitInstance;
            }
            Log(message, eventId, MessageType.Verbose, instance);
        }
        #endregion Worker Methods

        #region Internal
        /// <summary>
        /// Get the site ID (GUID) of the specified site.
        /// </summary>
        /// <param name="relativeSitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <param name="client">A validated GraphServiceClient object.</param>
        /// <returns>A GUID value representing the ID of the site.</returns>
        internal static string GetSiteId(string relativeSitePath,
                                         GraphServiceClient client)
        {
            return (client.Sites[$"root:{relativeSitePath}"]
                .GetAsync().GetAwaiter().GetResult()).Id;
        }

        /// <summary>
        /// Get the list ID (GUID) of the specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <param name="client">A validated GraphServiceClient object.</param>
        /// <returns>A GUID value representing the ID of the list.</returns>
        internal static string GetListId(
            string listName,
            string sitePath,
            GraphServiceClient client)
        {
            return client.Sites[GetSiteId(sitePath, client)]
                .Lists.GetAsync((C) =>
                {
                    C.QueryParameters.Filter = $"displayName eq '{listName}'";
                    C.Headers.Add("ConsistencyLevel", "eventual");
                }).GetAwaiter().GetResult().Value[0].Id;
        }
        #endregion
    }
}
