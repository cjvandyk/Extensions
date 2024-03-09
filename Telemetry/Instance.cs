/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public Stopwatch Timer { get; set; } = new Stopwatch();

        /// <summary>
        /// The number of threads used by the instance.
        /// </summary>
        public List<int> Threads { get; set; } = new List<int>();

        /// <summary>
        /// The counter of the instance.
        /// </summary>
        public int Counter { get; set; } = 0;
    }
}
