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
        ![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/String.BeginsWith.gif)<br>

- ### **ContainsAny()**
    > _Checks if the given string contains any of the characters or strings<br>
        provided in the IEnumerable.<br>
        This is useful for validating a given set of characters, like<br>
        special characters, or a given set of string, like bad words, is<br>
        not present in the target string._<br>
        ![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/String.ContainsAny.gif)<br>

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

- ### **ExceedsLength()**
    > _Checks if a referenced offset exceeds the length of the string.<br>
        Optionally increments the offset as well.<br>
        For example:<br>
            `"https://blog.cjvandyk.com Rocks!".ExceedsLength(30)`<br>
        will return False while<br>
            `"https://blog.cjvandyk.com Rocks!".ExceedsLength(31, false)`<br>
        will also return False and<br>
            `"https://blog.cjvandyk.com Rocks!".ExceedsLength(31)`<br>
        will return True._

- ### **GetSiteUrl()**
    > _Given a full SharePoint Online object URL, this method will return<br>
        the site collection part of the URL.<br>
        For example:<br>
            `"https://crayveon.sharepoint.com/sites/TheSite/lists/TheList".GetTenantUrl()`<br>
        would return `https://crayveon.sharepoint.com/sites/TheSite`_

- ### **GetTenantUrl()**
    > _Given a full SharePoint Online object URL, this method will return<br>
        only the tenant part of the URL.<br>
        For example:<br>
            `"https://crayveon.sharepoint.com/sites/TheSite/lists/TheList".GetTenantUrl()`<br>
        would return `https://crayveon.sharepoint.com`_

- ### **GetUrlRoot()**
    > _Get the URL root for the given string object containing a URL.<br>
        For example:<br>
            `"https://blog.cjvandyk.com".GetUrlRoot()`<br>
        will return "https://blog.cjvandyk.com" whereas<br>
            `"https://blog.cjvandyk.com/sites/Approval".GetUrlRoot()`<br>
        will also return "https://blog.cjvandyk.com"._

- ### **HasLower()**
    > _Validates that the given string object contains a lower case character.<br>
        For example:<br>
            `"abc".HasLower()`<br>
        will return True whereas<br>
            `"ABC".HasLower()`<br>
        will return False and<br>
            `"AbC".HasLower()`<br>
        will return True._

- ### **HasNumeric()**
    > _Validates that the given string object contains a number character.<br>
        For example:<br>
            `"abc".HasNumeric()`<br>
        will return False whereas<br>
            `"ABC123".HasNumeric()`<br>
        will return True and<br>
            `"A2C".HasNumeric()`<br>
        will return True._

- ### **HasSymbol()**
    > _Validates that the given string object contains a symbol or special<br>
        character.<br>
        For example:<br>
            `"abc".HasSymbol()`<br>
        will return False whereas<br>
            `"ABC$".HasSymbol()`<br>
        will return True and<br>
            `"A@C".HasSymbol()`<br>
        will return True._

- ### **HasUpper()**
    > _Validates that the given string object contains a lower case character.<br>
        For example:<br>
            `"abc".HasUpper()`<br>
        will return False whereas<br>
            `"ABC".HasUpper()`<br>
        will return True and<br>
            `"AbC".HasUpper()`<br>
        will return True._

- ### **HtmlDecode()**
    > _Decode the HTML escaped components in a given string returning the<br>
        given source string without HTML escaped components.<br>
        For example:<br>
            `"https://blog.cjvandyk.com/sites/Rocks &lt;&amp;&gt; Rolls!".HtmlDecode()`<br>
        will return<br>
            `https://blog.cjvandyk.com/sites/Rocks <&> Rolls!`_

- ### **HtmlEncode()**
    > _Encode the given string to be HTML safe.<br>
        For example:<br>
            `"https://blog.cjvandyk.com/sites/Rocks <&> Rolls!".HtmlEncode()`<br>
        will return<br>
            `https://blog.cjvandyk.com/sites/Rocks &lt;&amp;&gt; Rolls!`_

- ### **IsAlpha()**
    > _Validates that the given string object contains all alphabetic<br>
        characters (a-z and A-Z) returning True if it does and False if<br>
        it doesn't.<br>
        For example:<br>
            `"abcXYZ".IsAlphabetic()`<br>
        will return True whereas<br>
            `"abc123".IsAlphabetic()`<br>
        will return False._

- ### **IsAlphaNumeric()**
    > _Validates that the given string object contains all alphabetic<br>
        and/or numeric characters (a-z and A-Z and 0-9) returning True if it<br>
        does and False  if it doesn't.<br>
        For example:<br>
            `"abc123".IsAlphaNumeric()`<br>
        will return True whereas<br>
            `"abcxyz".IsAlphaNumeric()`<br>
        will also return True and<br>
            `"123456".IsAlphaNumeric()`<br>
        will also return True but<br>
            `"abc!@#".IsAlphaNumeric()`<br>
        will return False._

- ### **IsChar()**
    > _This method takes a char[] as one of its arguments against which the<br>
        given string object is validated.  If the given string object contains<br>
        only characters found in the char[] it will return True, otherwise it<br>
        will return False.<br>
        For example:<br>
            `"aacc".IsChar(new char[] {'a', 'c'})`<br>
        will return True whereas<br>
            `"abc123".IsChar(new char[] {'a', 'c'})`<br>
        will return False._

- ### **IsDateTime()**
    > _Validates that the given string is a valid date/time given the format.<br>
        It provides an easy way to validate date string input.<br>
        For example these:<br>
            `"20170704033333".IsDateTime("yyyMMddHHmmss")`<br>
            `"07/27/2017 03:33:33".IsDateTime("MM/dd/yyyy HH:mm:ss")`<br>
            `"27/07/2017 03:33:33".IsDateTime("dd/MM/yyyy HH:mm:ss")`<br>
            `"27/07/2017".IsDateTime("dd/MM/yyyy")`<br>
        will all return True whereas these:<br>
            `"2017070403333".IsDateTime("yyyMMddHHmmss")`<br>
                2017/07/04 03:33:3 is clearly missing another digit.<br>
            `"07/27/2017".IsDateTime("dd/MM/yyyy HH:mm:ss")`<br>
                07/27 fails because there isn't 27 months.<br>
            `"27/07/2017".IsDateTime("MM/dd/yyyy")`<br>
                27/07 fails for the same reason.<br>
        will return False._

- ### **IsEmail()**
    > _Validates that the given string object contains a valid email address.<br>
        For example:<br>
            `"noreply@crayveon.com".IsEmail()`<br>
        will return True whereas<br>
            `"noreplay-at-crayveon.com".IsEmail()`<br>
        will return False._

- ### **IsHex()**
    > _Checks if the given string represents hex based numbers.<br>
        For example:<br>
            `"9723FDC".IsHex()`<br>
        will return True whereas<br>
            `"9723FDT.IsHex()`<br>
        will return False because T is not a hex character._

- ### **IsLower()**
    > _Validates that the given string object contains only lower case letters.<br>
        For example:<br>
            `"IsLower test".IsLower()`<br>
        will return False while<br>
            `"islower test".IsLower()`<br>
        will return True and<br>
            `"islower test".IsLower(false)`<br>
        will return False._

- ### **IsNumeric()**
    > _Validates that the given string object contains all numeric<br>
        characters (0-9) returning True if it does and False  if it<br>
        doesn't.<br>
        For example:<br>
            `"123456".IsNumeric()`<br>
        will return True whereas<br>
            `"abc123".IsNumeric()`<br>
        will return False._

- ### **IsStrong()**
    > _Validates that the given string object contains a strong password string.<br>
        For example:<br>
            `"abc123XYZ!@#".IsStrong()`<br>
        will return True whereas<br>
            `"abc123XYZ".IsStrong()`<br>
        will return False and<br>
            `"abc123XYZ".IsStrong(3)`<br>
        will return True and<br>
            `"abc123XYZ".IsStrong(2)`<br>
        will return True.<br>
        The number parameter for IsStrong() indicates the number of criteria<br>
        that has to be true before the string is considered strong.  Valid<br>
        values are 1 through 4 with the default value being 4._

- ### **IsUpper()**
    > _Validates that the given string object contains only upper case letters.<br>
        For example:<br>
            `"IsUpper test".IsUpper()`<br>
        will return False while<br>
            `"ISUPPER TEST".IsUpper()`<br>
        will return True and<br>
            `"ISUPPER TEST".IsUpper(false)`<br>
        will return False._

- ### **IsUrlRoot()**
    > _Check if the given string object containing a URL, is that of the<br>
        URL root only.  Returns True if so, False if not.<br>
        For example:<br>
            `"https://blog.cjvandyk.com".IsUrlRootOnly()`<br>
        will return True whereas<br>
            `"https://blog.cjvandyk.com/sites/Approval".IsUrlRootOnly()`<br>
        will return False._

- ### **IsVowel()**
    > _Checks if the given char/string is an English vowel.<br>
        This allows the developer the ability to check a string without<br>
        having to first convert to a char e.g. as a substring return.<br>
        For example:<br>
            `"test".Substring(2, 1).IsVowel()`<br>
        will return False since the "s" is checked whereas<br>
            `"test".Substring(1, 1).IsVowel()`<br>
        will return True since the "e" is checked._

- ### **IsZip()**
    > _Checks if the given string object is in the valid format<br>
        of a United States zip code i.e. nnnnn-nnnn or just nnnnn.<br>
        For example:<br>
            `"12345-6789".IsZipCode()`<br>
        will return True whereas<br>
            `"1234-56789".IsZipCode()`<br>
        will return False.<br>
            `"12345".IsZipCode()`<br>
        will return True.<br>            
            `"123456".IsZipCode()`<br>
        will return False.<br>
            `"1234".IsZipCode()`<br>
        will return False._

- ### **Left()**
    > _This method returns text to the left of the index string.  Use negative
        values for occurrence if the occurrence count should start from the end
        instead of its default from the beginning of the string._

- ### **Lines()**
    > _This method returns the number of lines/sentences in the given string<br>
        object._

- ### **LoremIpsum()**
    > _Poplates the given string with a given number of paragraphs of dummy<br>
        text in the lorem ipsum style.
        For example:<br>
            `"".LoremIpsum(2)`<br>
        would yield<br>
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer<br> 
            aliquam arcu rhoncus erat consectetur, quis rutrum augue tincidunt.<br> 
            Suspendisse elit ipsum, lobortis lobortis tellus eu, vulputate<br> 
            fringilla lorem. Cras molestie nibh sed turpis dapibus sollicitudin<br> 
            ut a nulla. Suspendisse blandit suscipit egestas. Nunc et ante mattis<br> 
            nulla vehicula rhoncus. Vivamus commodo nunc id ultricies accumsan.<br> 
            Mauris vitae ante ut justo venenatis tempus.<br>
            <br>
            Nunc posuere, nisi eu convallis convallis, quam urna sagittis ipsum,<br> 
            et tempor ante libero ac ex. Aenean lacus mi, blandit non eros luctus,<br> 
            ultrices consectetur nunc. Vivamus suscipit justo odio, a porta massa<br> 
            posuere ac. Aenean varius leo non ipsum porttitor eleifend. Phasellus<br> 
            accumsan ultrices massa et finibus. Nunc vestibulum augue ut bibendum<br> 
            facilisis. Donec est massa, lobortis quis molestie at, placerat a<br> 
            neque. Donec quis bibendum leo. Pellentesque ultricies ac odio id<br> 
            pharetra. Nulla enim massa, lacinia nec nunc nec, egestas pulvinar<br> 
            odio. Sed pulvinar molestie justo, eu hendrerit nunc blandit eu.<br> 
            Suspendisse et sapien quis ipsum scelerisque rutrum."<br>_

- ### **Match()**
    > _Checks if the current string matches a given search mask.<br>
        It ignores duplicate '*' in the mask.  '*' is matched against<br>
        0 or more characters.  Duplicate '?' is treated as requiring<br>
        the number of characters.  '?' is matched against 1 or more<br>
        characters.<br>
        For example:<br>
            `"abcdefgh".Match("***a?c*")`<br>
        will return True while<br>
            `"abcdefgh".Match("***ac*")`<br>
        will return False but<br>
            `"abcdefgh".Match("?a?c*")`<br>
        will also return False because the first '?' requires a character<br>
        before 'a'._

- ### **MorseCodeBeep()**
    > _Takes a given System.String representing Morse code and audiblize<br>
        it according to standards.<br>
        https://www.infoplease.com/encyclopedia/science/engineering/electrical/morse-code<br>
        Assumes the input value to be in Morse code format already.<br>
        Use `.ToMorseCode()` to pre-convert text if needed._

- ### **QueryStringToDictionary()**
    > _Converts the current string (QueryString) to a dictionary of string<br>
        objects for example:<br>
            `"?id=123&url=blog.cjvandyk.com&rating=awesome".QueryStringToDictionary()`<br>
        will return a dictionary with 3 entries thus:<br>
            `["id"] = "123"`<br>
            `["url"] = "blog.cjvandyk.com"`<br>
            `["rating"] = "awesome"`<br>_

- ### **QueryStringToNameValueCollection()**
    > _Converts the current string (QueryString) to a NameValueCollection<br>
        a List of KeyValue pairs.  The main difference from `.QueryStringToDictionary()`<br>
        is that duplicates can be contained in the list for example:<br>
            `"?id=123&url=blog.cjvandyk.com&id=789".QueryStringToNameValueCollection()`<br>
        will return a dictionary with 3 entries thus:<br>
            `["id"] = "123"`<br>
            `["url"] = "blog.cjvandyk.com"`<br>
            `["id"] = "789"`<br>_

- ### **Quote()**

- ### **RemoveExtraSpace()**

- ### **ReplaceTokens()**

- ### **Right()**

- ### **SingleQuote()**

- ### **Singularize()**

- ### **SubString()**

- ### **TailString()**

- ### **ToBinary()**

- ### **ToEnglishAlphaChars()**

- ### **ToEnum()**

- ### **ToJson()**

- ### **ToMorseCode()**

- ### **TrimLength()**

- ### **TrimStart()**

- ### **Validate()**

- ### **ValidateNoNulls()**

- ### **Words()**

![Visitor Count](https://profile-counter.glitch.me/{cjvandyk}/count.svg)
