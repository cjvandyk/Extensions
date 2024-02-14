#pragma warning disable IDE1006 // Naming Styles

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

namespace System
{
    /// <summary>
    /// Timer class for timing things like a stopwatch.
    /// </summary>
    [Serializable]
    public partial class Timer
    {
        private System.DateTime? start { get; set; }
        private System.DateTime? stop { get; set; }
        /// <summary>
        /// The cumulated running time of the timer.
        /// </summary>
        public System.TimeSpan? runtime { get; private set; }

        #region Timer
        /// <summary>
        /// Constructor method.
        /// </summary>
        public Timer()
        {
            start = null;
            stop = null;
            runtime = null;
        }

        /// <summary>
        /// Start the timer.
        /// </summary>
        public void Start()
        {
            start = DateTime.UtcNow;
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        /// <returns>Return the cumulated running time of the timer.</returns>
        public System.TimeSpan Stop()
        {
            stop = DateTime.UtcNow;
            if (runtime == null)
            {
                runtime = stop - start;
            }
            else
            {
                runtime += stop - start;
            }
            return (System.TimeSpan)runtime;
        }

        /// <summary>
        /// Pause the timer.
        /// </summary>
        /// <returns>Return the cumulated running time of the timer.</returns>
        public System.TimeSpan Pause()
        {
            return Stop();
        }

        /// <summary>
        /// Resume the timer.
        /// </summary>
        public void Resume()
        {
            Start();
        }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        public void Reset()
        {
            start = null;
            stop = null;
            runtime = null;
        }
        #endregion Timer
    }
}
#pragma warning restore IDE1006 // Naming Styles
