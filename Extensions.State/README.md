# Extensions.State contains methods to make instrumentation and telemety a breeze.
[![icon](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions-64x64.png)](https://github.com/cjvandyk/Extensions)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions.gif)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Easy%20Date%20convertion%20GIF.gif)
[![License](https://img.shields.io/github/license/cjvandyk/Extensions)](https://github.com/cjvandyk/Extensions/blob/main/LICENSE) [![Maintained](https://img.shields.io/maintenance/yes/2023)](https://github.com/cjvandyk/extensions/releases) [![GitHub Release](https://img.shields.io/github/release/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/releases/) [![NuGet Badge](https://buildstats.info/nuget/Extensions.CS)](https://www.nuget.org/packages/Extensions.cs) [![Repo Size](https://img.shields.io/github/repo-size/cjvandyk/extensions)](https://github.com/cjvandyk/Extensions) [![Closed Issues](https://img.shields.io/github/issues-closed/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/issues?q=is%3Aissue+is%3Aclosed) [![Open Issues](https://img.shields.io/github/issues/cjvandyk/extensions.svg)](https://github.com/cjvandyk/extensions/issues) [![Contributors](https://img.shields.io/github/contributors/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/graphs/contributors/) [![Languages](https://img.shields.io/github/languages/count/cjvandyk/extensions.svg)](https://github.com/cjvandyk/Extensions/search?l=c%23) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ExtensionsCS/Extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Discord](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Discord.png?raw=true)](https://discord.com/channels/799027565465305088/799027565993394219) [![Twitter](https://img.shields.io/twitter/follow/cjvandyk?style=social)](https://twitter.com/intent/follow?screen_name=cjvandyk)

The following classes have been extended:

    - GCCHigh.Extensions.State

with these methods:

- ### **LoadState()**
    > _A delegate method for loading state._

- ### **SaveState()**
    > _A delegate method for saving state._

- ### **LoadStateList()**
    > _The basic method to load a list from disk.  It only requires one<br>
        parameter which is the name of the list to load._

- ### **SaveStateList()**
    > _Save a given list state to disk._

- ### **LoadStateListToRef()**
    > _Method to load a very large list of Graph ListItems from disk.  It<br>
        requires all three parameters in order to load the list.  In order<br>
        to minimize RAM usage, the list is loaded directly to the eventual<br>
        target container list thus eliminating in-memory duplication of<br>
        the large list._

- ### **SaveStateListFromRef()**
    > _Save a given referenced list state to disk._

![Visitor Count](https://profile-counter.glitch.me/{cjvandyk}/count.svg)
 