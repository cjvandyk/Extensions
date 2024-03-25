# Extensions.cs contains extension methods that enhance existing C# classes thus making life easier for developers.
[![icon](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions-64x64.png)](https://github.com/cjvandyk/Extensions)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions.gif)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Easy%20Date%20convertion%20GIF.gif)
[![License](https://img.shields.io/github/license/cjvandyk/Extensions)](https://github.com/cjvandyk/Extensions/blob/main/LICENSE) [![Maintained](https://img.shields.io/maintenance/yes/2024)](https://github.com/cjvandyk/extensions/releases) [![GitHub Release](https://img.shields.io/github/release/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/releases/) [![NuGet Badge](https://buildstats.info/nuget/Extensions.CS)](https://www.nuget.org/packages/Extensions.cs) [![Repo Size](https://img.shields.io/github/repo-size/cjvandyk/extensions)](https://github.com/cjvandyk/Extensions) [![Closed Issues](https://img.shields.io/github/issues-closed/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/issues?q=is%3Aissue+is%3Aclosed) [![Open Issues](https://img.shields.io/github/issues/cjvandyk/extensions.svg)](https://github.com/cjvandyk/extensions/issues) [![Contributors](https://img.shields.io/github/contributors/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/graphs/contributors/) [![Languages](https://img.shields.io/github/languages/count/cjvandyk/extensions.svg)](https://github.com/cjvandyk/Extensions/search?l=c%23) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ExtensionsCS/Extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Discord](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Discord.png?raw=true)](https://discord.com/channels/799027565465305088/799027565993394219) [![Twitter](https://img.shields.io/twitter/follow/cjvandyk?style=social)](https://twitter.com/intent/follow?screen_name=cjvandyk)

The following classes have been extended:

    - System.Array
    - System.Collections.Generic.Dictionary
    - System.Collections.Generic.List
    - System.DateTime
    - System.Diagnostics.Process
    - System.Double
    - System.Int16
    - System.Int32
    - System.Int64
    - System.Logging (added)
    - System.Long
    - System.Net.WebException
    - System.Object
    - System.String
    - System.Text.StringBuilder
    - System.Timer
    - System.TimeZoneInfo
    - System.UInt16
    - System.UInt32
    - System.UInt64
    - System.ULong

with these methods:

- ### **BeginsWith()**
    > _Reduces checking if a given string starts with another given string<br>
        from this:<br>
            `(str.ToLower().Substring(0, target.Length) == target.ToLower())`<br>
        to this:<br>
            `str.BeginsWith(target, true)`_<br>
        ![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/String.BeginsWith.gif)<br>

- ### **Bigest()**
    > _Return the bigest of two given values.<br>
        For example:<br>
            `Bigest(23, 31)`<br>
        will return<br>
            `31`_

- ### **CompoundInterest()**
    > _Calculate compounded interest end value given an amount, percent<br>
        interest per year and number of years.<br>
        For example:<br>
            `double val = 100.00;`<br>
            `val.CompoundInterest(5,`<br>
                                 `10,`<br>
                                 `Constants.CompoundFrequency.Yearly);`<br>
        will return 162.889462677744 _

- ### **ContainsAny()**
    > _Checks if the given string contains any of a list of characters or<br>
        strings provided.<br>
        For example:<br>
            `"abcdef1234567890".ContainsAny(Constants.HexChars)`<br>
        will return True._<br>
        ![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/String.ContainsAny.gif)<br>

- ### **ContainsOnly()**
    > _Checks if the given string contains only characters or strings<br>
        in the list provided.<br>
        For example:<br>
            `"abcdef1234567890".ContainsOnly(Constants.HexChars)`<br>
        will return True while<br>
            `"abcdefg1234567890".ContainsOnly(Constants.HexChars)`<br>
        will return False because of the "g"._

- ### **CopyTo()**
    > _Copies a given length of bytes from a byte[] starting at a definable<br>
        offset.<br>
        For example:<br>
            `byte[] b1 = System.Text.Encoding.UTF8.GetBytes("blog.cjvandyk.com rocks!");`<br>
            `byte[] b2 = b1.CopyTo(10);`<br>
            `byte[] b3 = b1.CopyTo(10, 5);`<br>
        will result in the following arrays:<br>
            `98  108 111 103 46  99  106 118 97  110 100 121 107 46  99  111 109 32  114 111 99  107 115 33`<br>
            `98  108 111 103 46  99  106 118 97  110`<br>
            `                    99  106 118 97  110 100 121 107 46  99`_

- ### **DoubleQuote()**
    > _Return the given string encased in double quotes.<br>
        For example:<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks");`<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks".DoubleQuote());`<br>
        will return<br>
            `https://blog.cjvandyk.com/sites/Rocks`<br>
            `"https://blog.cjvandyk.com/sites/Rocks"`_

- ### **Elevate()**
    > _Restarts the current process with elevated permissions.<br>
        For example:<br>
            `System.Diagnostics.Process.GetCurrentProcess().Elevate(args)`<br>
        will restart the current console app in admin mode._

- ### **Err()**
    > _Write an Error message to active channels (console, event log, file)<br>
        using the System.Logging class._

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

- ### **Get()**
    > _Language extension for properties.  Use to set the value of the<br>
        extension property in question.<br>
        For example:<br>
            `Microsoft.SharePoint.Client client = new Microsoft.SharePoint.Client("https://blog.cjvandyk.com");`<br>
            `client.ExecutingWebRequest += ClientContext_ExecutingWebRequest;`<br>
            `client.Set("HeaderDecoration", "NONISV|Crayveon|MyApp/1.0");`<br>
        This allows the creation of the extension property "HeaderDecoration"<br>
        which can be changed as needed.  Later in the delegate method we<br>
        refer back to the extension property value thus:<br>
            `private void ClientContext_ExecutingWebRequest(object sender, WebRequestEventArgs e)`<br>
            `{`<br>
                `e.WebRequestExecutor.WebRequest.UserAgent = (string)e.Get("HeaderDecoration");`<br>
            `}`<br>
    NOTE: We did not have to access the ClientContext class in order to<br>
        retrieve the "HeaderDecoration" value since the extension was<br>
        done against the System.Object class.  As such, any object can<br>
        be used to retrieve the extension property value, as long as<br>
        you know the key value under which the property was stored and<br>
        you know the type to which the returned value needs to be cast.<br>
        A derived override method for Get() and Set() can be defined<br>
        using specific class objects if finer controls is needed.<br>_

- ### **GetExecutingAssembly()**
    > _Gets the current Entry or Executing assembly through reflection._

- ### **GetExecutingAssemblyName()**
    > _Gets the name of the current assembly, optionally escaped._

- ### **GetExecutingAssemblyFolder()**
    > _Gets the folder location of the current assembly, optionally escaped._

- ### **GetExecutingAssemblyFullPath()**
    > _Gets the full path and file name of the current assembly, optionally<br>
        escaped._

- ### **GetFQDN()**
    > _Get the current computer Fully Qualified Domain Name._

- ### **GetNthPrime()**
    > _Get the Nth prime number.  It will serialize the list of discovered<br>
        prime numbers to file in order to eliminate duplicate calculation<br>
		of prime numbers.  Use `Universal.PrimeStatePath` to override the<br>
		path where the discovered list of prime numbers is saved.<br>
        For example:<br>
            `Extensions.Universal.GetNthPrime(1000)`<br>
        will return the 1000th prime number - 7919._

- ### **GetNthPrimeAsync()**
    > _Get the Nth prime number using multi threading and asynchronous<br>
        processing.  It will serialize the list of discovered<br>
        prime numbers to file in order to eliminate duplicate calculation<br>
		of prime numbers.  Use `Universal.PrimeStatePath` to override the<br>
		path where the discovered list of prime numbers is saved.<br>
        For example:<br>
            `Extensions.Universal.GetNthPrime(1000)`<br>
        will return the 1000th prime number - 7919._

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

- ### **GetTimeZoneString()**
    > _Get the registry ID string that can be used with<br>
        TimeZoneInfo.FindSystemTimeZoneById() for time zone convertions.<br>
        For example:<br>
            `System.TimeZoneInfo.FindSystemTimeZoneById(`<br>
                `Extensions.TimeZoneInfo.GetTimeZoneString(`<br>
                    `Constants.TimeZone myZone))`<br>
        will return the proper string to use in the call._

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

- ### **Inf()**
    > _Write an Information message to active channels (console, event log, file)<br>
        using the System.Logging class._

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

- ### **IsEven()**
    > _Checks if the given number is even.<br>
        For example:<br>
            `234.IsEven()`<br>
        will return True whereas<br>
            `339.IsEven()`<br>
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

- ### **IsOdd()**
    > _Checks if the given number is odd.<br>
        For example:<br>
            `234.IsOdd()`<br>
        will return False whereas<br>
            `339.IsOdd()`<br>
        will return True._

- ### **IsPrime()**
    > _Checks if the given number is a prime number.<br>
        For example:<br>
            `27.IsPrime()`<br>
        will return False whereas<br>
            `29.IsPrime()`<br>
        will return True._

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

- ### **IsZipCode()**
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

- ### **Load()**
    > _Language extension providing a universal method to all objects<br>
        that allows them to be deserialized from disk.<br>
        Does NOT require the `[Serializable]` property on object.<br>
        For example:<br>
            `ComplexClass myClass = new ComplexClass();`<br>
            `myClass = myClass.Load("My file path");`<br>
        Use `.Save()` to save objects to disk._

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

- ### **NewCustomGuid()**
    > _Returns a custom GUID starting with a custom string.<br>
        For example:<br>
            `Extensions.NewCustomGuid("012")`<br>
        will return a new GUID that starts with "012"._

- ### **Print()**
    > _Print the byte[] to console, separated by spaces and space padded<br>
        on the right to allow proper alignment for debug/testing output.<br>
        For example:<br>
            `byte[] bytes = System.Text.Encoding.UTF8.GetBytes("blog.cjvandyk.com rocks!");`<br>
            `bytes.Print();`_

- ### **printf()**
    > _Simple printf method for console output with color control.  Both<br>
        text color and background color is returned to previous state<br>
        after the string has been written to console.<br>
        For example:<br>
            `printf("Hello World!", ConsoleColor.Red, ConsoleColor.White);`<br>
        will output the string to console in red text on a white background._

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
    > _Return the given string encased in requested quotes.<br>
        Default is Constants.QuoteType.Double.<br>
        For example:<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks");`<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks").Quote();`<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks".Quote(`<br>
                `Constants.QuoteType.Single));`<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks".Quote(`<br>
                `Constants.QuoteType.Double));`<br>
        will return<br>
            `https://blog.cjvandyk.com/sites/Rocks`<br>
            `"https://blog.cjvandyk.com/sites/Rocks"`<br>
            `'https://blog.cjvandyk.com/sites/Rocks'`<br>
            `"https://blog.cjvandyk.com/sites/Rocks"`_

- ### **RemoveExtraSpace()**
    > _Trims leading and trailing white space and then removes all extra<br>
        white space in the given string returning a single spaced result.<br>
        For example:<br>
            `"  blog.cjvandyk.com    rocks   !   ".RemoveExtraSpace()`<br>
        will return<br>
            `"blog.cjvandyk.com rocks !"`_

- ### **ReplaceTokens()**
    > _Takes a given string object and replaces 1 to n tokens in the string<br>
        with replacement tokens as defined in the given Dictionary of strings._

- ### **Retry()**
    > _Checks if a System.Net.WebException contains a "Retry-After" header.<br>
        If it does, it sleeps the thread for that period (+ 60 seconds)<br>
        before reattempting the HTTP call that caused the exception in the<br>
        first place.  If no "Retry-After" header exist, the exception is<br>
        simply rethrown.<br>
        For example:<br>
            `System.Net.HttpWebRequest request ...`<br>
            `Try`<br>
            `{`<br>
                `request.GetResponse();`<br>
            `}`<br>
            `Catch (System.Net.WebException ex)`<br>
            `{`<br>
                `ex.Retry(request);`<br>
            `}`<br>_

- ### **Save()**
    > _Language extension providing a universal method to all objects<br>
        that allows them to be serialized to disk.<br>
        Does NOT require the `[Serializable]` property on object.<br>
        For example:<br>
            `ComplexClass myClass = new ComplexClass(...<constructor parms>...);`<br>
            `myClass.Save("My file path");`<br>
        Use `.Load()` to reload objects back from disk._

- ### **Set()**
    > _Language extension for properties.  Use to set the value of the<br>
        extension property in question.<br>
        For example:<br>
            `Microsoft.SharePoint.Client client = new Microsoft.SharePoint.Client("https://blog.cjvandyk.com");`<br>
            `client.ExecutingWebRequest += ClientContext_ExecutingWebRequest;`<br>
            `client.Set("HeaderDecoration", "NONISV|Crayveon|MyApp/1.0");`<br>
        This allows the creation of the extension property "HeaderDecoration"<br>
        which can be changed as needed.  Later in the delegate method we<br>
        refer back to the extension property value thus:<br>
            `private void ClientContext_ExecutingWebRequest(object sender, WebRequestEventArgs e)`<br>
            `{`<br>
                `e.WebRequestExecutor.WebRequest.UserAgent = (string)e.Get("HeaderDecoration");`<br>
            `}`<br>
        NOTE: We did not have to access the ClientContext class in order to<br>
        retrieve the "HeaderDecoration" value since the extension was<br>
        done against the System.Object class.  As such, any object can<br>
        be used to retrieve the extension property value, as long as<br>
        you know the key value under which the property was stored and<br>
        you know the type to which the returned value needs to be cast.<br>
        A derived override method for Get() and Set() can be defined<br>
        using specific class objects if finer controls is needed.<br>

- ### **SingleQuote()**
    > _Return the given string encased in single quotes.<br>
        For example:<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks");`<br>
            `printf("https://blog.cjvandyk.com/sites/Rocks".SingleQuote());`<br>
        will return<br>
            `https://blog.cjvandyk.com/sites/Rocks`<br>
            `'https://blog.cjvandyk.com/sites/Rocks'`_

- ### **Singularize()**
    > _Parses the given string removing multiples of a given character.<br>
        For example:<br>
            `string searchMask = "***??abc*";`<br>
            `searchMask.Singularize('*')`<br>
        will return<br>
            `"*??abc*"`_

- ### **Smallest()**
    > _Return the smallest of two given values.<br>
        For example:<br>
            `Smallest(23, 31)`<br>
        will return<br>
            `23`_

- ### **Substring()**
    > _Extends the `.Substring(startIndex)` and `.Substring(startIndex, length)`<br>
        methods to the `System.Text.StringBuilder` class.<br>
        For example:<br>
            `System.Text.StringBuilder sb = new System.Text.StringBuilder();`<br>
            `sb.Append("abc1abc2abc3abc4");`<br>
            `sb.Substring(5);`<br>
        will return `bc2abc3abc4`<br>
            `sb.Substring(5, 3);`<br>
        will return `bc2`<br>
        Adds the FromHead/FromTail overloaded methods.<br>
        FromHead returns the "length" of characters from the head of the given<br>
        string.<br>
        For example:<br>
            `sb.Substring(3, Constants.SubstringType.FromHead);`<br>
            `sb.Substring(5, Constants.SubstringType.FromHead);`<br>
            `sb.Substring(8, Constants.SubstringType.FromHead);`<br>
        will return<br>
            `abc`<br>
            `abc1a`<br>
            `abc1abc2`<br>
        FromTail returns the "length" of characters from the tail of the given<br>
        string.<br>
        For example:<br>
            `sb.Substring(3, Constants.SubstringType.FromTail);`<br>
            `sb.Substring(5, Constants.SubstringType.FromTail);`<br>
            `sb.Substring(8, Constants.SubstringType.FromTail);`<br>
        will return<br>
            `bc4`<br>
            `3abc4`<br>
            `abc3abc4`<br>
        Adds the LeftOfIndex/RightOfIndex overloaded methods.<br>
        LeftOfIndex returns the "length" of characters to the LEFT of the<br>
        located index representing the "occurence"th match of the "index"<br>
        string.<br>
        For example:<br>
            `sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 0);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 1);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 2);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 3);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 4);`<br>
        will return<br>
            ``<br>
            ``<br>
            `abc1`<br>
            `1abc2`<br>
            `2abc3`<br>
        RightOfIndex returns the "length" of characters to the RIGHT of the<br>
        located index representing the "occurence"th match of the "index"<br>
        string.<br>
        For example:<br>
            `sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 0);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 1);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 2);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 3);`<br>
            `sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 4);`<br>
        will return<br>
            ``<br>
            `1abc2`<br>
            `2abc3`<br>
            `3abc4`<br>
            `4`_

- ### **System.Timer class**
	> _This class provides and easy way to time things like a stopwatch.<br>
        `.Start()` starts the timer.<br>
	    `.Stop()` stops the timer.<br>
	    `.Pause()` pauses the timer.<br>
	    `.Resume()` resumes the timer.<br>
	    `.Reset()` resets the timer.<br>
        For example:<br>
            `System.Timer timer = new System.Timer();`<br>
            `timer.Start();`<br>
            ` <DO STUFF> `<br>
            `System.TimeSpan howlong = timer.Stop();`_

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

- ### **ToBinary()**
    > _This method returns the given string represented in 1s and 0s as<br>
        a binary result.<br>
        For example:<br>
            `"This test".ToBinary()`<br>
        will return <br>
            `1010100 1101000 1101001 1110011 100000 1110100 1100101 1110011 1110100`_

- ### **ToEnum()**
    > _This method matches a given string to the given enum set and returns<br>
        the matched enum.<br>
        For example:<br>
            `enum testEnum { first, second, third };`<br>
            `var testEnumResult = "first".ToEnum<testEnum>();`<br>
            `Console.WriteLine(testEnumResult == testEnum.first);`<br>
        will return<br>
        `True`_

- ### **ToMorseCode()**
    > _Convert given string to its Morse code representation.<br>
        Undefined characters will return in the format:<br>
        <Undefined:[char=""]><br>
        For example:<br>
            `"sos@".ToMorseCode()`<br>
        will return<br>
        `"...---...<Undefined:[@]>"`_

- ### **ToQueryString()**
    > _Convert given Dictionary<string, string> into a querystring.<br>
        For example:<br>
            `Dictionary<string, string> dic1 = new Dictionary<string, string>();`<br>
            `dic1.Add("Parm1", "Val1");`<br>
            `dic1.Add("Parm2", "Val2");`<br>
            `dic1.Add("Parm3", "Val3");`<br>
            `Console.WriteLine(dic1.ToQueryString());`<br>
        will return<br>
        `"?Parm1=Val1&Parm2=Val2&Parm3=Val3"`_

- ### **Binary Data Size Convertions**<br>
    - `System.Double.ToNumberBytes()` >>> _Returns the given number expressed as Bytes._<br>
	- `System.Double.ToKB()` >>> _Returns the given number expressed as Kilobytes (2^10)._<br>
	- `System.Double.ToMB()` >>> _Returns the given number expressed as Megabytes (2^20)._<br>
	- `System.Double.ToGB()` >>> _Returns the given number expressed as Gigabytes (2^30)._<br>
	- `System.Double.ToTB()` >>> _Returns the given number expressed as Terrabytes (2^40)._<br>
	- `System.Double.ToPB()` >>> _Returns the given number expressed as Petabytes (2^50)._<br>
	- `System.Double.ToEB()` >>> _Returns the given number expressed as Exabytes (2^60)._<br>
	- `System.Double.ToZB()` >>> _Returns the given number expressed as Zettabytes (2^70)._<br>
	- `System.Double.ToYB()` >>> _Returns the given number expressed as Yottabytes (2^80)._<br>
	- `System.Double.ToBB()` >>> _Returns the given number expressed as Brontobytes (2^90)._<br>
	- `System.Double.ToGpB()` >>> _Returns the given number expressed as Geopbytes (2^100)._<br>
	- `System.Double.ToSB()` >>> _Returns the given number expressed as Saganbytes (2^110)._<br>
	- `System.Double.ToPaB()` >>> _Returns the given number expressed as Pijabytes (2^120)._<br>
	- `System.Double.ToAB()` >>> _Returns the given number expressed as Alphabytes (2^130)._<br>
	- `System.Double.ToPlB()` >>> _Returns the given number expressed as Pectrolbytes (2^140)._<br>
	- `System.Double.ToBrB()` >>> _Returns the given number expressed as Bolgerbytes (2^150)._<br>
	- `System.Double.ToSoB()` >>> _Returns the given number expressed as Sambobytes (2^160)._<br>
	- `System.Double.ToQB()` >>> _Returns the given number expressed as Quesabytes (2^170)._<br>
	- `System.Double.ToKaB()` >>> _Returns the given number expressed as Kinsabytes (2^180)._<br>
	- `System.Double.ToRB()` >>> _Returns the given number expressed as Rutherbytes (2^190)._<br>
	- `System.Double.ToDB()` >>> _Returns the given number expressed as Dubnibytes (2^200)._<br>
	- `System.Double.ToHB()` >>> _Returns the given number expressed as Hassiubytes (2^210)._<br>
	- `System.Double.ToMrB()` >>> _Returns the given number expressed as Meitnerbytes (2^220)._<br>
	- `System.Double.ToDdB()` >>> _Returns the given number expressed as Darmstadbytes (2^230)._<br>
	- `System.Double.ToRtB()` >>> _Returns the given number expressed as Roentbytes (2^240)._<br>
	- `System.Double.ToShB()` >>> _Returns the given number expressed as Sophobytes (2^250)._<br>
	- `System.Double.ToCB()` >>> _Returns the given number expressed as Coperbytes (2^260)._<br>
	- `System.Double.ToKkB()` >>> _Returns the given number expressed as Koentekbytes (2^270)._<br>
    > _For example:<br>
            `double dbl = 1;`<br>
            `Console.WriteLine(dbl.ToKB(Constants.NumberType.TB));`<br>
            `Console.WriteLine(dbl.ToKB(Constants.NumberType.GB));`<br>
            `Console.WriteLine(dbl.ToKB(Constants.NumberType.ZB));`<br>
        will return<br>
            `1073741824`<br>
            `1048576`<br>
            `1.15292150460685E+18`_

- ### **ToTimeZone()**
    > _Convert given DateTime between different time zones with ease.<br>
        For example:<br>
            `System.DateTime now = System.DateTime.UtcNow;`<br>
            `now.ToTimeZone(`<br>
                `Constants.TimeZone.UTC,`<br>
                `Constants.TimeZone.EasternStandardTime));`<br>
        will return the current UTC time as Eastern time._

- ### **TrimLength()**
    > _Returns part of the given System.Text.StringBuilder object<br>
        tuncated to the requested length minus the length of the<br>
        suffix.<br>
        If the string is null or empty, it returns said value.<br>
        If the string is shorter than the requested length, it returns<br>
        the whole string.<br>
        For example:<br>
            `"The Extensions.cs NuGet package rocks!".TrimLength(20)`<br>
        will return "The Extensions.cs..." while<br>
            `"The Extensions.cs NuGet package rocks!".TrimLength(20, "")`<br>
        will return "The Extensions.cs Nu" and<br>
            `"The Extensions.cs NuGet package rocks!".TrimLength(20, ">>")`<br>
        will return "The Extensions.cs >>"<br>_

- ### **TrimStart()**
    > _Trims a given string rather than just a character, from the start of<br>
        the target string.  The traditional Trim() only allowed char values<br>
        to be trimmed.  TrimStart() solves that limitation in an easier to<br>
        fashion that using Substring().<br>
        For example:<br>
            `"https://blog.cjvandyk.com".TrimStart("https://")`<br>
            will return<br>
            `"blog.cjvandyk.com"`_

- ### **Validate()**
    > _Makes quick work of conducting multiple types of validations on all<br>
        parameters.  It takes a parameter array of ErrorType and conducts<br>
        the appropriate validation such as null checking, non-zero checking<br>
        etc. against the parameter array passed.<br>
        For example:<br>
            `Validate(Constants.ErrorTypeAll, amount, percent, years, frequency);`<br>
            will perform all defined error checks against the `amount`, `percent`,<br>
            `years` and `frequency` parameters._

	
	
	

	- Add `Logging.ConstructMessage()`.<br>
	- Add `Logging.ConsoleMessage()`.<br>
	- Add `Logging.EventLogMessage()`.<br>
	- Add `Logging.FileMessage()`.<br>


- ### **ValidateNoNulls()**
    > _Makes quick work of null validating all parameters you pass to it.<br>
        This method takes a variable number of parameters and validates that<br>
        all parameters are not null.  If a parameter is found to be null, a<br>
        ArgumentNullException is thrown.<br>
        For example:<br>
            `void MyMethod(string str, double dbl, MyClass cls)`<br>
            `{`<br>
            `    Universal.ValidateNoNulls(str, dbl, cls);<br>
            `    ...Your code here...`<br>
            `}`<br>
        You do not have to pass all parameters, but can instead do this:<br>
            `void MyMethod(string str, double dbl, MyClass cls)`<br>
            `{`<br>
            `    Universal.ValidateNoNulls(str, cls);<br>
            `    ...Your code here...`<br>
            `}`<br>
        where we chose NOT to validate the `double dbl` in this case._

- ### **Words()**
    > _This method returns the number of words used in the given string<br>
        object.
        For example:<br>
            `"This is my test".Words()`<br>
        will return 4 whereas<br>
            `"ThisIsMyTest".Words()`<br>
        will return 1._

- ### **Wrn()**
    > _Write a Warning message to active channels (console, event log, file)<br>
        using the System.Logging class._

![Visitor Count](https://profile-counter.glitch.me/{cjvandyk}/count.svg)
