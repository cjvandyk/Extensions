# Extensions.String contains methods to make logging to console, file, EventLog, SharePoint list, and database a breeze.
[![icon](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions-64x64.png)](https://github.com/cjvandyk/Extensions)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions.gif)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Easy%20Date%20convertion%20GIF.gif)
[![License](https://img.shields.io/github/license/cjvandyk/Extensions)](https://github.com/cjvandyk/Extensions/blob/main/LICENSE) [![Maintained](https://img.shields.io/maintenance/yes/2023)](https://github.com/cjvandyk/extensions/releases) [![GitHub Release](https://img.shields.io/github/release/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/releases/) [![NuGet Badge](https://buildstats.info/nuget/Extensions.CS)](https://www.nuget.org/packages/Extensions.cs) [![Repo Size](https://img.shields.io/github/repo-size/cjvandyk/extensions)](https://github.com/cjvandyk/Extensions) [![Closed Issues](https://img.shields.io/github/issues-closed/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/issues?q=is%3Aissue+is%3Aclosed) [![Open Issues](https://img.shields.io/github/issues/cjvandyk/extensions.svg)](https://github.com/cjvandyk/extensions/issues) [![Contributors](https://img.shields.io/github/contributors/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/graphs/contributors/) [![Languages](https://img.shields.io/github/languages/count/cjvandyk/extensions.svg)](https://github.com/cjvandyk/Extensions/search?l=c%23) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ExtensionsCS/Extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Discord](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Discord.png?raw=true)](https://discord.com/channels/799027565465305088/799027565993394219) [![Twitter](https://img.shields.io/twitter/follow/cjvandyk?style=social)](https://twitter.com/intent/follow?screen_name=cjvandyk)

The following classes have been extended:

    - System.String

with these methods:

- ### **BeginsWith()**
    > _Checks if the current string begins with the given target string.<br>
        For example:<br>
            `"GCCHigh.Extensions rock!".BeginsWith("GCCHigh")`<br>
        will return<br>
            `true`<br>
        whereas<br>
            `"GCCHigh.Extensions rock!".BeginsWith("gcchigh")`<br>
        will also return<br>
            `true`<br>
        because the ignorecase switch defaults to true.  Using the <br>
        ignorecase switch like this<br>
            `"GCCHigh.Extensions rock!".BeginsWith("gcchigh", false)`<br>
        will return<br>
            `false`<br>_

- ### **ContainsAny()**
    > _Checks if the given string contains any of the characters or strings<br>
        provided in the IEnumerable.<br>
        This is useful for validating a given set of characters, like<br>
        special characters, or a given set of string, like bad words, is<br>
        not present in the target string.<br>_

- ### **ContainsOnly()**
    > _Checks if the given string contains only the characters or strings<br>
        provided in the IEnumerable.<br>
        This is useful for validating a string contains only hex chars etc.<br>_

- ### **DoubleQuote()**
    > _Return the given string encased in double quotes.  This is useful<br>
        when working with multi-layer quotes and strings where strings<br>
        contain quoted strings.<br>_

- ### **EncodeAsXml()**
    > _Encode a given string as XML by encoding all ampersand, single <br>
        quote, greater than, less than and double quote characters into<br>
        their proper XML equivalent.<br>_

![Visitor Count](https://profile-counter.glitch.me/{cjvandyk}/count.svg)
