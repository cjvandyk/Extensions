# Extensions.dll contains extension methods that enhance existing C# classes thus making life easier for developers.
[![icon](https://github.com/cjvandyk/Quix/blob/master/Extensions/Images/Extensions-64x64.png?raw=true)](https://github.com/cjvandyk/quix/Extensions)<br>
![GIF](https://github.com/cjvandyk/Quix/blob/master/Extensions/Images/Extensions.gif)<br>
![License](https://img.shields.io/github/license/cjvandyk/quix) ![Maintained](https://img.shields.io/maintenance/yes/2021) [![GitHub Release](https://img.shields.io/github/release/cjvandyk/quix.svg)](https://GitHub.com/cjvandyk/quix/releases/) ![NuGet Badge](https://buildstats.info/nuget/Extensions.CS) [![Repo Size](https://img.shields.io/github/repo-size/cjvandyk/quix)](https://github.com/cjvandyk/quix/Extensions) [![Closed Issues](https://img.shields.io/github/issues-closed/cjvandyk/quix.svg)](https://GitHub.com/cjvandyk/quix/issues?q=is%3Aissue+is%3Aclosed) [![Open Issues](https://img.shields.io/github/issues/cjvandyk/quix.svg)](https://github.com/cjvandyk/quix/issues) [![Contributors](https://img.shields.io/github/contributors/cjvandyk/quix.svg)](https://GitHub.com/cjvandyk/quix/graphs/contributors/) ![Languages](https://img.shields.io/github/languages/count/cjvandyk/quix.svg) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ExtensionsCS/Extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Discord](https://github.com/cjvandyk/Quix/blob/master/Extensions/Images/Discord.png?raw=true)](https://discord.com/channels/799027565465305088/799027565993394219) [![Twitter](https://img.shields.io/twitter/follow/cjvandyk?style=social)](https://twitter.com/intent/follow?screen_name=cjvandyk)

The following classes have been extended:

    - System.Array
    - System.Collections.Generic.Dictionary
    - System.Diagnostics.Process
    - System.Double
    - System.Net.WebException
    - System.Object
    - System.String
    - System.Text.StringBuilder

with these methods:

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

- ### **Elevate()**
    > _Restarts the current process with elevated permissions.<br>
        For example:<br>
            `System.Diagnostics.Process.GetCurrentProcess().Elevate(args)`<br>
        will restart the current console app in admin mode._

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

- ### **GetTimeZoneString()**
    > _Get the registry ID string that can be used with<br>
        TimeZoneInfo.FindSystemTimeZoneById() for time zone convertions.<br>
        For example:<br>

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

- ### **IsAlphabetic()**
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
         
- ### **IsEmail()**
    > _Validates that the given string object contains a valid email address.<br>
        For example:<br>
            `"noreply@crayveon.com".IsEmail()`<br>
        will return True whereas<br>
            `"noreplay-at-crayveon.com".IsEmail()`<br>
        will return False._

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

- ### **MorseCodeBeep()**
    > _Takes a given System.String representing Morse code and audiblize<br>
        it according to standards.<br>
        https://www.infoplease.com/encyclopedia/science/engineering/electrical/morse-code<br>
        Assumes the input value to be in Morse code format already.<br>
        Use `.ToMorseCode()` to pre-convert text if needed._

- ### **Print()**
    > _Print the byte[] to console, separated by spaces and space padded<br>
        on the right to allow proper alignment for debug/testing output.<br>
        For example:<br>
            `byte[] bytes = System.Text.Encoding.UTF8.GetBytes("blog.cjvandyk.com rocks!");`<br>
            `bytes.Print();`_

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
        before reattempting to HTTP call that caused the exception in the<br>
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

- ### **Words()**
    > _This method returns the number of words used in the given string<br>
        object.
        For example:<br>
            `"This is my test".Words()`<br>
        will return 4 whereas<br>
            `"ThisIsMyTest".Words()`<br>
        will return 1._
