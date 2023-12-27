#pragma warning disable CA1416, CS0162, CS0168, CS1587, CS1591, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Logit;

namespace Extensions
{
    /// <summary>
    /// The instance class for telemetry.
    /// </summary>
    [Serializable]
    public class TelemetryInstance
    {
        /// <summary>
        /// The stopwatch of the instance.
        /// </summary>
        public Stopwatch timer { get; set; } = new Stopwatch();
        /// <summary>
        /// The number of threads used by the instance.
        /// </summary>
        public List<int> threads { get; set; } = new List<int>();
        /// <summary>
        /// The counter of the instance.
        /// </summary>
        public int counter { get; set; } = 0;
    }

    /// <summary>
    /// A class to measure telemetry in code.
    /// </summary>
    [Serializable]
    public static class Telemetry
    {
        /// <summary>
        /// The stack of telemetry measuring instances.
        /// </summary>
        public static Dictionary<string, TelemetryInstance> timers
        { get; private set; } = new();
        /// <summary>
        /// The name of the active telemetry instance.
        /// </summary>
        public static string activeTimer { get; set; } = "default";
        /// <summary>
        /// The delegate method for stopping telemetry on the instance.
        /// </summary>
        public static Action<object> stopTelemetryMethod { get; set; }
        /// <summary>
        /// The delegate method for starting telemetry on the instance.
        /// </summary>
        public static Action<object> startTelemetryMethod { get; set; }

        /// <summary>
        /// Method to add a new instance to the stack.
        /// </summary>
        /// <param name="timerName"></param>
        /// <param name="setAsActive"></param>
        /// <param name="startImmediately"></param>
        public static void AddTimer(string timerName,
                                    bool setAsActive = true,
                                    bool startImmediately = true)
        {
            try
            {
                //If timer doesn't already exist, add it.
                if (!timers.ContainsKey(timerName))
                {
                    timers.Add(timerName, new TelemetryInstance());
                }
            }
            catch (Exception ex)
            {
                //Swallow error if duplicate key.
                if (!ex.Message.Contains("An item with the same key has already been added"))
                {
                    Err(ex.ToString());
                    throw;
                }
            }
            //Set the activeTimer to the new timer if requested.
            if (setAsActive)
            {
                activeTimer = timerName;
            }
            //Start the new timer if requested.
            if (startImmediately)
            {
                Start(true);
            }
        }

        /// <summary>
        /// Clear all the timers.
        /// </summary>
        public static void ClearEverything()
        {
            timers.Clear();
        }

        /// <summary>
        /// Get the counter from the active timer.
        /// </summary>
        /// <returns>The activeTimer.counter value.</returns>
        public static int Count()
        {
            Initialize();
            return timers[activeTimer].counter;
        }

        /// <summary>
        /// Get the counter from a given timer.
        /// </summary>
        /// <param name="timerName">The name of the target timer.</param>
        /// <returns>The target timer's counter value.</returns>
        public static int Count(string timerName)
        {
            Initialize();
            return timers[timerName].counter;
        }

        /// <summary>
        /// Get the number of threads from the active timer.
        /// </summary>
        /// <returns>The activeTimer.threads.Count value.</returns>
        public static int ThreadCount()
        {
            return ThreadCount(activeTimer);
        }

        /// <summary>
        /// Get the number of threads from a given timer.
        /// </summary>
        /// <param name="timerName">The name of the target timer.</param>
        /// <returns>The target timer's threads.Count value.</returns>
        public static int ThreadCount(string timerName)
        {
            Initialize();
            return timers[timerName].threads.Count;
        }

        /// <summary>
        /// A method to display the latest telemetry status of the active timer.
        /// </summary>
        /// <returns>A string containing the number of operations, the 
        /// elapsed time, the number of threads used, the current RAM used and
        /// the current CPU used.</returns>
        public static string TelemetryStatus()
        {
            return TelemetryStatus(activeTimer);
        }

        /// <summary>
        /// A method to display the latest telemetry status of a given timer.
        /// </summary>
        /// <param name="timerName">The name of the timer to use.</param>
        /// <returns>A string containing the number of operations, the 
        /// elapsed time, the number of threads used, the current RAM used and
        /// the current CPU used.</returns>
        public static string TelemetryStatus(string timerName)
        {
            return $"\nProcessed [" +
                Count(timerName) +
                $"] item(s) in [" +
                Elapsed(timerName) +
                $"] using [" +
                ThreadCount(timerName) +
                $"] thread(s), [" +
                RAM() +
                $"] RAM, [" +
                CPU() +
                $"] CPU.";
        }

        /// <summary>
        /// A method to return the timer.Elapsed value of the active timer.
        /// </summary>
        /// <returns>The activeTimer.timer.Elapsed value.</returns>
        public static TimeSpan Elapsed()
        {
            return Elapsed(activeTimer);
        }

        /// <summary>
        /// A method to return the timer.Elapsed value of a given timer.
        /// </summary>
        /// <param name="timerName">The name of the timer to use.</param>
        /// <returns>The timer.Elapsed value of the given timer.</returns>
        public static TimeSpan Elapsed(string timerName)
        {
            Initialize(timerName);
            return timers[timerName].timer.Elapsed;
        }

        /// <summary>
        /// A method to return the timer.ElapsedMilliseconds value of the
        /// active timer.
        /// </summary>
        /// <returns>The activeTimer.timer.ElapsedMilliseconds value.</returns>
        public static long ElapsedMilliseconds()
        {
            return ElapsedMilliseconds(activeTimer);
        }

        /// <summary>
        /// A method to return the timer.ElapsedMilliseconds value of the
        /// active timer.
        /// </summary>
        /// <param name="timerName">The name of the timer to use.</param>
        /// <returns>The timer.Elapsedmilliseconds value of the given 
        /// timer.</returns>
        public static long ElapsedMilliseconds(string timerName)
        {
            Initialize(timerName);
            return timers[timerName].timer.ElapsedMilliseconds;
        }

        /// <summary>
        /// A method to return the timer.ElapsedTicks value of the active timer.
        /// </summary>
        /// <returns>The activeTimer.timer.ElapsedTicks value.</returns>
        public static long ElapsedTicks()
        {
            return ElapsedTicks(activeTimer);
        }

        /// <summary>
        /// A method to return the timer.ElapsedTicks value of the active timer.
        /// </summary>
        /// <param name="timerName">The name of the timer to use.</param>
        /// <returns>The timer.ElapsedTicks value of the given timer.</returns>
        public static long ElapsedTicks(string timerName)
        {
            Initialize(timerName);
            return timers[timerName].timer.ElapsedTicks;
        }

        /// <summary>
        /// Initialization method for the timers.
        /// </summary>
        /// <param name="timerName">The name of the timer to use.</param>
        public static void Initialize(string timerName = "default")
        {
            if (!timers.ContainsKey(timerName))
            {
                AddTimer(timerName);
            }
        }

        /// <summary>
        /// Method to check if the activeTimer is running.
        /// </summary>
        /// <returns>The value of activeTimer.timer.IsRunning</returns>
        public static bool IsRunning()
        {
            return IsRunning(activeTimer);
        }

        /// <summary>
        /// Method to check if a given timer is running.
        /// </summary>
        /// <param name="timerName">The name of the timer to use.</param>
        /// <returns>The value of the given timer's timer.IsRunning</returns>
        public static bool IsRunning(string timerName)
        {
            Initialize(timerName);
            return timers[timerName].timer.IsRunning;
        }

        /// <summary>
        /// Method to load the default "Telemetry" list of timers.
        /// </summary>
        public static void Load()
        {
            try
            {
                timers = (Dictionary<string, TelemetryInstance>)
                           State.LoadStateList(
                               "Telemetry",
                               typeof(Dictionary<string, TelemetryInstance>),
                               false,
                               9990);
            }
            catch (Exception ex)
            {
                //If timers fail to load, initialize a new set.
                timers = new Dictionary<string, TelemetryInstance>();
            }
        }

        /// <summary>
        /// Method to load a given list of timers.
        /// </summary>
        /// <param name="savedTimers">The name of the list of timers to use.</param>
        public static void Load(Dictionary<string, TelemetryInstance> savedTimers)
        {
            if (savedTimers != null)
            {
                timers = savedTimers;
            }
        }

        /// <summary>
        /// Method to reset the active timer.
        /// </summary>
        public static void Reset()
        {
            Reset(activeTimer);
        }

        /// <summary>
        /// Method to reset a given timer.
        /// </summary>
        /// <param name="timerName">The name of the timer to reset.</param>
        public static void Reset(string timerName)
        {
            Initialize(timerName);
            timers[timerName].timer.Reset();
        }

        /// <summary>
        /// Method to restart the active timer.
        /// </summary>
        public static void Restart()
        {
            Restart(activeTimer);
        }

        /// <summary>
        /// Method to restart a given timer.
        /// </summary>
        /// <param name="timerName">The name of the timer to restart.</param>
        public static void Restart(string timerName)
        {
            Initialize(timerName);
            timers[timerName].timer.Restart();
        }

        /// <summary>
        /// Method to stop and save all timers in the default "Telemetry" list.
        /// </summary>
        public static void Save()
        {
            StopAllTimers();
            State.SaveStateList("Telemetry", timers);
        }

        /// <summary>
        /// Method to start a timer using the calling method's name.
        /// </summary>
        public static void Start()
        {
            Start((new StackTrace()).GetFrame(1).GetMethod().Name);
        }

        /// <summary>
        /// Method to start the active timer.
        /// </summary>
        /// <param name="active">Not used.</param>
        public static void Start(bool active = true)
        {
            Start(activeTimer);
        }

        /// <summary>
        /// Method to start a given timer.
        /// </summary>
        /// <param name="timerName">The name of the timer to use.</param>
        public static void Start(string timerName)
        {
            Initialize(timerName);
            timers[timerName].timer.Start();
        }

        /// <summary>
        /// Method to stop the timer using the calling method's name.
        /// </summary>
        public static void Stop()
        {
            Stop((new StackTrace()).GetFrame(1).GetMethod().Name);
        }

        /// <summary>
        /// Method to stop the active timer.
        /// </summary>
        /// <param name="active">Not used.</param>
        public static void Stop(bool active = true)
        {
            Stop(activeTimer);
        }

        /// <summary>
        /// Method to stop a given timer.
        /// </summary>
        /// <param name="timerName">Name of the timer to stop.</param>
        public static void Stop(string timerName)
        {
            Initialize(timerName);
            timers[timerName].timer.Stop();
        }

        /// <summary>
        /// Method to stop all timers.
        /// </summary>
        public static void StopAllTimers()
        {
            foreach (var timer in timers.Values)
            {
                if (timer != null)
                {
                    timer.timer.Stop();
                }
            }
        }

        /// <summary>
        /// Method to stop telemetry.
        /// </summary>
        /// <param name="parms"></param>
        public static void StopTelemetry(object parms)
        {
            stopTelemetryMethod(parms);
        }

        /// <summary>
        /// Method to calculate the current RAM consumption.
        /// </summary>
        /// <returns>The amount of RAM used in MB.</returns>
        public static string RAM()
        {
            return $"{(Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024)} MB";
        }

        /// <summary>
        /// Method to calculate the current CPU consumption.
        /// </summary>
        /// <returns>The percentable of overall CPU use.</returns>
        public static string CPU()
        {
            try
            {
                var proc = Process.GetCurrentProcess();
                var cpu = new System.Diagnostics.PerformanceCounter
                    ("Process", "% Processor Time", proc.ProcessName);
                cpu.NextValue();
                return $"{(int)(cpu.NextValue() / Environment.ProcessorCount)} %";
            }
            catch
            {
                //Errors are non-critical so swallow any errors.
                return "";
            }
        }
    }
}

#pragma warning restore CA1416, CS0162, CS0168, CS1587, CS1591, CS1998, IDE0059, IDE0028
