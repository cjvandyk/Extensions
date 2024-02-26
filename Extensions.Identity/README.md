# GCCHigh.Extensions.Identity contains methods to make modern OAuth2 authentication in Azure a breeze.
[![icon](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions-64x64.png)](https://github.com/cjvandyk/Extensions)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Extensions.gif)
![GIF](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Easy%20Date%20convertion%20GIF.gif)
[![License](https://img.shields.io/github/license/cjvandyk/Extensions)](https://github.com/cjvandyk/Extensions/blob/main/LICENSE) [![Maintained](https://img.shields.io/maintenance/yes/2023)](https://github.com/cjvandyk/extensions/releases) [![GitHub Release](https://img.shields.io/github/release/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/releases/) [![NuGet Badge](https://buildstats.info/nuget/Extensions.CS)](https://www.nuget.org/packages/Extensions.cs) [![Repo Size](https://img.shields.io/github/repo-size/cjvandyk/extensions)](https://github.com/cjvandyk/Extensions) [![Closed Issues](https://img.shields.io/github/issues-closed/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/issues?q=is%3Aissue+is%3Aclosed) [![Open Issues](https://img.shields.io/github/issues/cjvandyk/extensions.svg)](https://github.com/cjvandyk/extensions/issues) [![Contributors](https://img.shields.io/github/contributors/cjvandyk/extensions.svg)](https://GitHub.com/cjvandyk/extensions/graphs/contributors/) [![Languages](https://img.shields.io/github/languages/count/cjvandyk/extensions.svg)](https://github.com/cjvandyk/Extensions/search?l=c%23) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ExtensionsCS/Extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Discord](https://raw.githubusercontent.com/cjvandyk/Extensions/master/Images/Discord.png?raw=true)](https://discord.com/channels/799027565465305088/799027565993394219) [![Twitter](https://img.shields.io/twitter/follow/cjvandyk?style=social)](https://twitter.com/intent/follow?screen_name=cjvandyk)

The following classes have been exposed:

    - Extensions.TenantConfig
    - Extensions.Constants
    - Extensions.Identity.App
    - Extensions.Identity.Auth
    - Extensions.Identity.AuthMan
    - Extensions.Identity.Cert
    - Extensions.Identity.Scopes

with these methods:

- ### **Auth()**
    > _Multiple overloaded constructors allows for the construction and<br>
        population of the class instance properties.  Each instance internally<br>
        manages its access token and renews it 5 minutes prior to expiration.<br>
        The class exposes the following useful objects:<br>
          GraphClient - Authenticated Microsoft.Graph.ServiceClient ready for use.<br>
          GraphBetaClient - Authenticated Microsoft.Graph.ServiceClient pointing<br>
                            to the /beta endpoint and ready for use.<br>
          HttpClient - Authenticated Microsoft.Net.Http.HttpClient ready for use.<br>
        The HttpClient is useful when making direct REST calls.<br>_

- ### **AuthMan()**
    > _This class maintains and exposes the following two important objects:<br>
          ActiveAuth - The currently active Auth object.<br>
          AuthStack - A dictionary of all valid Auth objects.<br>
        The ActiveAuth object of type Auth which represents the currently active<br>
        Auth context in use.  The AuthStack which is a dictionary containing<br>
        all valid Auth objects.  When a request for a new Auth is made<br>
        through GetAuth(), the AuthStack is checked for a match.  If the<br>
        matching Auth object is already in the AuthStack, it is simply reused<br>
        instead of expending the cycles and resources to generate a new one.<br>
        Because the Auth class maintains its access token internally, the Auth<br>
        object is always valid.  The greatly improves performance especially in<br>
        cases where Auth switching between Graph and SharePoint happens regularly.<br>_

- ### **GetAuth()**
    > _Multiple overloads allows for the retrieval of a fully constructed Auth<br>
        object which is then stored in the managed AuthStack.  Future requests<br>
        for the same Auth are then simply popped from the AuthStack for use.<br>_

- ### **GetApp()**
    > _Multiple overloads allows the retrieval of either an authenticated<br>
        IConfidentialClientApplication or IPublicClientApplication.<br>_

- ### **GetCertByThumbprint(string thumbPrint)**
    > _Returns a X509Certificate2 if available, given the thumbPrint string.<br>
        It will first attempt to load the cert from the CurrentUser context.<br>
        If that fails, it will attempt to load the cert from the<br>
        LocalMachine context.  If this fails, it will return null.<br>
        For example:<br>
            `var cert = GetCertByThumbPrint(certificateThumbPrint)`<br>
        will return the X509Certificate2 or null.<br>
        For more fine grained control, an overload exist that allows the<br>
        specification of a targeted StoreName and StoreLocation.<br>_

- ### **GetScopes(ScopeType scopeType)**
    > _Returns the string array representing the scopes for the given scopeType._

- ### **GetScopeType(string[] scopes)**
    > _Returns the ScopeType given the string array of scopes._

![Visitor Count](https://profile-counter.glitch.me/{cjvandyk}/count.svg)
