# Extensions.Core contains relay and helper methods for the Extensions set of solutions.
[![icon](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions-64x64.png)](https://github.com/cjvandyk/Extensions)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions.gif)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Easy%20Date%20convertion%20GIF.gif)
[![License](https://img.shields.io/github/license/cjvandyk/Extensions)](https://github.com/cjvandyk/Extensions/blob/main/LICENSE) [![Maintained](https://img.shields.io/maintenance/yes/2023)](https://github.com/cjvandyk/extensions/releases) [![GitHub Release](https://img.shields.io/github/release/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/releases/) [![NuGet Badge](https://buildstats.info/nuget/Extensions.CS)](https://www.nuget.org/packages/Extensions.cs) [![Repo Size](https://img.shields.io/github/repo-size/cjvandyk/extensions)](https://github.com/cjvandyk/Extensions) [![Closed Issues](https://img.shields.io/github/issues-closed/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/issues?q=is%3Aissue+is%3Aclosed) [![Open Issues](https://img.shields.io/github/issues/cjvandyk/extensions.svg)](https://github.com/cjvandyk/extensions/issues) [![Contributors](https://img.shields.io/github/contributors/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/graphs/contributors/) [![Languages](https://img.shields.io/github/languages/count/cjvandyk/extensions.svg)](https://github.com/cjvandyk/Extensions/search?l=c%23) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ExtensionsCS/Extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Discord](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Discord.png?raw=true)](https://discord.com/channels/799027565465305088/799027565993394219) [![Twitter](https://img.shields.io/twitter/follow/cjvandyk?style=social)](https://twitter.com/intent/follow?screen_name=cjvandyk)

The following classes have been extended:

    - Extensions.Core (added)
    - Extensions.Marshall (added)

with these methods:

- ### **Invoke()**
    > _Method to invoke a member method of a loaded Assembly._

- ### **IsAvailable()**
    > _Checks if a given Assembly is loaded._

- ### **Load()**
    > _Method to load a given Assembly to the Marshall stack for later use._

- ### **TimeStamp()**
    > _Returns a string representing the current local date time stamp to<br>
        either just the day or down to the millisecond.  Used for creating<br>
        unique log file names.<br>
        For example:<br>
            `TimeStamp()`<br>
        will return<br>
            `2021-03-01@06.01.02.003`<br>
        whereas<br>
            `TimeStamp(true)`<br>
        will return<br>
            `2021-03-01`_

- ### **Vrb()**
    > _Write a Verbose message to active channels (console, event log, file),<br>
        SharePoint list, database) using the System.Logit class._

![Visitor Count](https://profile-counter.glitch.me/{cjvandyk}/count.svg)
