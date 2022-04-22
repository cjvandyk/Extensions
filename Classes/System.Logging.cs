#pragma warning disable CS1587

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;

using Extensions;
using static Extensions.Universal;

namespace System
{
    /// <summary>
    /// Class that provides easy logging mechanisms to screen, event log or file.
    /// </summary>
    public class Logging
    {
        /// <summary>
        /// Lock used when writing to log file.
        /// </summary>
        private static System.Threading.ReaderWriterLockSlim _lock = 
            new System.Threading.ReaderWriterLockSlim();
        /// <summary>
        /// The last cursor position top value.
        /// </summary>
        private static int _top { get; set; }
        /// <summary>
        /// The last cursor position left value.
        /// </summary>
        private static int _left { get; set; }
        /// <summary>
        /// The last console foreground color.
        /// </summary>
        private static ConsoleColor _text { get; set; }
        /// <summary>
        /// The last console background color.
        /// </summary>
        private static ConsoleColor _background { get; set; }
        /// <summary>
        /// The event log to use for writing.
        /// </summary>
        private static System.Diagnostics.EventLog _eventlog { get; set; }
        /// <summary>
        /// The file system writer used to log to file.
        /// </summary>
        private static System.IO.StreamWriter _file { get; set; }
        /// <summary>
        /// The file path to the current log file.
        /// </summary>
        private static string _logfile { get; set; }
        /// <summary>
        /// Private stringbuilder used to generate log messages.
        /// </summary>
        private static System.Text.StringBuilder _str { get; set; }
        #region Output Control Switches
        /// <summary>
        /// Trigger to control messages being written to the LogFile.
        /// </summary>
        public static bool WriteToFile { get; set; } = true;
        /// <summary>
        /// Trigger to control messages being written to the console.
        /// </summary>
        public static bool WriteToConsole { get; set; } = true;
        /// <summary>
        /// Trigger to control messages being written to the Event Log.
        /// </summary>
        public static bool WriteToEventLog { get; set; } = false;
        /// <summary>
        /// Switch to control if a date/time stamp is injected with the message.
        /// </summary>
        public static bool Stamp { get; set; }
        #endregion Output Control Switches

        #region Constructor
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="File">Optional log file path.  If no value
        /// is provided, the log file is created in the executing assembly's
        /// current folder.</param>
        /// <param name="UseTimeStamp">Optional flag used to control if
        /// date/time stamps are added to logging messages.</param>
        /// <param name="LogToConsole">Optional flag used to turn on/off
        /// logging to the console.</param>
        /// <param name="LogToEventLog">Optional flag used to turn on/off
        /// logging to the Windows event log.</param>
        /// <param name="LogToFile">Optional flag used to turn on/off
        /// logging to file.</param>
        public Logging(string File = "",
                       bool UseTimeStamp = true,
                       bool LogToConsole = true,
                       bool LogToEventLog = false,
                       bool LogToFile = true)
        {
            if (File == "")
            {
                _logfile = GetExecutingAssemblyFolder() +
                    "\\" +
                    GetExecutingAssemblyName().Replace(" ", "_") +
                    "." +
                    Universal.TimeStamp() +
                    ".csv";
            }
            else
            {
                _logfile = File;
            }
            _eventlog = new System.Diagnostics.EventLog();
            _eventlog.Source = "Application";
            _str = new System.Text.StringBuilder();
            WriteToConsole = LogToConsole;
            WriteToEventLog = LogToEventLog;
            WriteToFile = LogToFile;
            Stamp = UseTimeStamp;
        }
        #endregion Constructor

        /// <summary>
        /// Private method to write to active message receptors.
        /// </summary>
        /// <param name="Message">Message to write.</param>
        /// <param name="ID">An event ID to include with the log entry.</param>
        /// <param name="MessageType">The type of message i.e. INF, WRN or ERR.</param>
        private static void Msg(string Message, 
                                int ID = 0,
                                string MessageType = "INF")
        {
            if (WriteToConsole)
            {
                switch (MessageType.ToUpper())
                {
                    case "INF":
                        ConsoleMessage(Message,
                                       ID,
                                       ConsoleColor.White,
                                       ConsoleColor.Black);
                        break;
                    case "WRN":
                        ConsoleMessage(Message,
                                       ID,
                                       ConsoleColor.Yellow,
                                       ConsoleColor.Black);
                        break;
                    case "ERR":
                        ConsoleMessage(Message,
                                       ID,
                                       ConsoleColor.Red,
                                       ConsoleColor.Black);
                        break;
                }
            }
            if (WriteToEventLog)
            {
                switch (MessageType.ToUpper())
                {
                    case "INF":
                        EventLogMessage(Message,
                                        Diagnostics.EventLogEntryType.Information,
                                        ID);
                        break;
                    case "WRN":
                        EventLogMessage(Message,
                                        Diagnostics.EventLogEntryType.Warning,
                                        ID);
                        break;
                    case "ERR":
                        EventLogMessage(Message,
                                        Diagnostics.EventLogEntryType.Error,
                                        ID);
                        break;
                }
            }
            if (WriteToFile)
            {
                FileMessage(Message,
                            MessageType,
                            ID);
            }
        }

        /// <summary>
        /// Write an Information message to logs.
        /// </summary>
        /// <param name="Message">Message to write.</param>
        /// <param name="ID">Optional event ID to include.</param>
        public static void Inf(string Message, 
                               int ID = 0)
        {
            Msg(Message, ID, "INF");
        }

        /// <summary>
        /// Write a Warning message to logs.
        /// </summary>
        /// <param name="Message">Message to write.</param>
        /// <param name="ID">Optional event ID to include.</param>
        public static void Wrn(string Message, 
                               int ID = 0)
        {
            Msg(Message, ID, "WRN");
        }

        /// <summary>
        /// Write an Error message to logs.
        /// </summary>
        /// <param name="Message">Message to write.</param>
        /// <param name="ID">Optional event ID to include.</param>
        public static void Err(string Message,
                               int ID = 0)
        {
            Msg(Message, ID, "INF");
        }

        /// <summary>
        /// Construct the final message by prepending the date/time stamp if
        /// the switch is active, appending the ID and 1-N string arguments.
        /// </summary>
        /// <param name="ID">The event ID to include.</param>
        /// <param name="Text">An array of text strings to append.</param>
        /// <returns></returns>
        public static string ConstructMessage(int ID, 
                                              params string[] Text)
        {
            _str.Clear();
            if (Stamp)
            {
                _str.Append(Universal.TimeStamp() + ", ");
            }
            _str.Append(ID + ", ");
            foreach (string str in Text)
            {
                _str.Append(str + ", ");
            }
            return _str.ToString();
        }

        #region Console
        /// <summary>
        /// Write a message to the command console.
        /// </summary>
        /// <param name="Message">The message to write to the console.</param>
        /// <param name="ID">Optional event ID to include.</param>
        private static void ConsoleMessage(string Message, 
                                           int ID = 0)
        {
            if (Message == null)
            {
                Console.WriteLine(ConstructMessage(ID, "NULL"));
            }
            else
            {
                Console.WriteLine(ConstructMessage(ID, Message));
            }
        }

        /// <summary>
        /// Write a message to the command console.
        /// </summary>
        /// <param name="Message">The message to write to the console.</param>
        /// <param name="ID">Optional event ID to include.</param>
        /// <param name="FixedLocation">Optional switch to keep the cursor
        /// fixed in its location.  This is useful when writing progress
        /// messages like percent complete.</param>
        private static void ConsoleMessage(string Message, 
                                           int ID = 0,
                                           bool FixedLocation = false)
        {
            if (FixedLocation)
            {
                SetCursor();
                ConsoleMessage(Message, 
                               ID);
                SetCursor(true);
            }
            else
            {
                ConsoleMessage(Message, 
                               ID);
            }
        }

        /// <summary>
        /// Write a message to the command console.
        /// </summary>
        /// <param name="Message">The message to write to the console.</param>
        /// <param name="ID">Optional event ID to include.</param>
        /// <param name="TextColor">Control the text color of the output.</param>
        /// <param name="FixedLocation">Optional switch to keep the cursor
        /// fixed in its location.  This is useful when writing progress
        /// messages like percent complete.</param>
        private static void ConsoleMessage(
            string Message,
            int ID = 0,
            ConsoleColor TextColor = ConsoleColor.Gray, 
            bool FixedLocation = false)
        {
            if (Console.ForegroundColor != TextColor)
            {
                _text = Console.ForegroundColor;
                Console.ForegroundColor = TextColor;
                ConsoleMessage(Message, 
                               ID, 
                               FixedLocation);
                Console.ForegroundColor = _text;
            }
            else
            {
                ConsoleMessage(Message, 
                               ID, 
                               FixedLocation);
            }
        }

        /// <summary>
        /// Write a message to the command console.
        /// </summary>
        /// <param name="Message">The message to write to the console.</param>
        /// <param name="ID">Optional event ID to include.</param>
        /// <param name="TextColor">Control the text color of the output.</param>
        /// <param name="BackgroundColor">Control the background color.</param>
        /// <param name="FixedLocation">Optional switch to keep the cursor
        /// fixed in its location.  This is useful when writing progress
        /// messages like percent complete.</param>
        public static void ConsoleMessage(
            string Message,
            int ID = 0,
            ConsoleColor TextColor = ConsoleColor.Gray,
            ConsoleColor BackgroundColor = ConsoleColor.Black,
            bool FixedLocation = false)
        {
            if (Console.BackgroundColor != BackgroundColor)
            {
                _background = Console.BackgroundColor;
                Console.BackgroundColor = BackgroundColor;
                ConsoleMessage(Message, 
                               ID, 
                               TextColor, 
                               FixedLocation);
                Console.BackgroundColor = _background;
            }
            else
            {
                ConsoleMessage(Message, 
                               ID, 
                               TextColor, 
                               FixedLocation);
            }
        }

        /// <summary>
        /// Set or capture the cursor position on the console.
        /// </summary>
        /// <param name="reset">If true, set the console cursor to the last
        /// captured coordinates.  If false, capture the current console
        /// cursor coordinates.  Default it false.</param>
        private static void SetCursor(bool reset = false)
        {
            if (reset)
            {
                Console.CursorTop = _top;
                Console.CursorLeft = _left;
            }
            else
            {
                _top = Console.CursorTop;
                _left = Console.CursorLeft;
            }
        }
        #endregion Console

        #region EventLog
        /// <summary>
        /// Write a message to the Windows Event Log.
        /// </summary>
        /// <param name="Message">The message to write.</param>
        /// <param name="EntryType">The type of entry to make i.e. Error,
        /// Warning or Information.</param>
        /// <param name="ID">Optional event ID to include.</param>
        public static void EventLogMessage(
            string Message, 
            System.Diagnostics.EventLogEntryType EntryType =
                System.Diagnostics.EventLogEntryType.Information,
            int ID = 0)
        {
            _eventlog.WriteEntry(ConstructMessage(ID, Message), 
                                 EntryType, 
                                 ID);
        }
        #endregion EventLog

        #region File
        /// <summary>
        /// Write a message to file.
        /// </summary>
        /// <param name="Message">Message to write.</param>
        /// <param name="EntryType">Type of message i.e. ERR, WRN or INF.</param>
        /// <param name="ID">Optional ID to include.</param>
        public static void FileMessage(string Message,
                                       string EntryType = "INF",
                                       int ID = 0)
        {
            if (!_lock.IsWriteLockHeld)
            {
                _lock.EnterWriteLock();
                _file = new IO.StreamWriter(_logfile, true);
                _file.WriteLine(
                    ConstructMessage(ID, 
                                     EntryType, 
                                     Message));
                _file.Close();
                _lock.ExitWriteLock();
            }
        }
        #endregion File
    }
}
