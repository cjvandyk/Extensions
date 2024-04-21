/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

namespace Extensions
{
    /// <summary>
    /// Constants used in the Extensions methods.
    /// </summary>
    [Serializable]
    public static partial class Constants
    {
        #region Auth
        /// <summary>
        /// The different types of authentication efforts to use.
        /// </summary>
        public enum AuthPublicAppResultType
        {
            /// <summary>
            /// Interactively attempt to obtain authentication.
            /// </summary>
            Interactive,

            /// <summary>
            /// Prompt the user for authentication.
            /// </summary>
            Prompt,

            /// <summary>
            /// Silently attempt to obtain authentication.
            /// </summary>
            Silent
        }

        /// <summary>
        /// An enum for the type of Azure environment.
        /// </summary>
        public enum AzureEnvironmentName
        {
            /// <summary>
            /// China national cloud.
            /// </summary>
            O365China,

            /// <summary>
            /// Default commercial cloud.
            /// </summary>
            O365Default,  //Commercial tenant

            /// <summary>
            /// Germany national cloud.
            /// </summary>
            O365GermanyCloud,

            /// <summary>
            /// US Government Department of Defense cloud.
            /// </summary>
            O365USGovDoD,

            /// <summary>
            /// US Government High Security cloud.
            /// </summary>
            O365USGovGCCHigh  //GCCHigh tenant
        }

        /// <summary>
        /// The type of ClientApplication to use.
        /// </summary>
        public enum ClientApplicationType
        {
            /// <summary>
            /// Confidential
            /// </summary>
            Confidential,

            /// <summary>
            /// Public
            /// </summary>
            Public
        }
        #endregion Auth

        #region Binary Constants

        /// <summary>
        /// Enum of the binary number types.
        /// </summary>
        public enum NumberType
        {
            Bytes,
            KB,
            MB,
            GB,
            TB,
            PB,
            EB,
            ZB,
            YB,
            BB,
            GpB,
            SB,
            PaB,
            AB,
            PlB,
            BrB,
            SoB,
            QB,
            KaB,
            RB,
            DB,
            HB,
            MrB,
            DdB,
            RtB,
            ShB,
            CB,
            KkB
        }

        /// <summary>
        /// Number of bits per byte.
        /// </summary>
        public const double BitsPerByte = 8;

        /// <summary>
        /// Number of bytes in a Kilobyte. *1024^3
        /// </summary>
        public const double KB = 1024;

        /// <summary>
        /// Number of bytes in a Megabyte. *1024^6
        /// </summary>
        public const double MB = KB * 1024;

        /// <summary>
        /// Number of bytes in a Gigabyte. *1024^9
        /// </summary>
        public const double GB = MB * 1024;

        /// <summary>
        /// Number of bytes in a Terabyte. *1024^12
        /// </summary>
        public const double TB = GB * 1024;

        /// <summary>
        /// Number of bytes in a Petabyte. *1024^15
        /// </summary>
        public const double PB = TB * 1024;

        /// <summary>
        /// Number of bytes in a Exabyte. *1024^18
        /// </summary>
        public const double EB = PB * 1024;

        /// <summary>
        /// Number of bytes in a Zettabyte. *1024^21
        /// </summary>
        public const double ZB = EB * 1024;

        /// <summary>
        /// Number of bytes in a Yottabyte. *1024^24
        /// </summary>
        public const double YB = ZB * 1024;

        /// <summary>
        /// Number of bytes in a Brontobyte. *1024^27
        /// </summary>
        public const double BB = YB * 1024;

        /// <summary>
        /// Number of bytes in a Geopbyte. *1024^30
        /// </summary>
        public const double GpB = BB * 1024;

        /// <summary>
        /// Number of bytes in a Saganbyte. *1024^33
        /// </summary>
        public const double SB = GpB * 1024;

        /// <summary>
        /// Number of bytes in a Pijabyte. *1024^36
        /// </summary>
        public const double PaB = SB * 1024;

        /// <summary>
        /// Number of bytes in a Alphabyte. *1024^39
        /// </summary>
        public const double AB = PaB * 1024;

        /// <summary>
        /// Number of bytes in a Pectrolbyte. *1024^42
        /// </summary>
        public const double PlB = AB * 1024;

        /// <summary>
        /// Number of bytes in a Bolgerbyte. *1024^45
        /// </summary>
        public const double BrB = PlB * 1024;

        /// <summary>
        /// Number of bytes in a Sambobyte. *1024^48
        /// </summary>
        public const double SoB = BrB * 1024;

        /// <summary>
        /// Number of bytes in a Quesabyte. *1024^51
        /// </summary>
        public const double QB = SoB * 1024;

        /// <summary>
        /// Number of bytes in a Kinsabyte. *1024^54
        /// </summary>
        public const double KaB = QB * 1024;

        /// <summary>
        /// Number of bytes in a Rutherbyte. *1024^57
        /// </summary>
        public const double RB = KaB * 1024;

        /// <summary>
        /// Number of bytes in a Dubnibyte. *1024^60
        /// </summary>
        public const double DB = RB * 1024;

        /// <summary>
        /// Number of bytes in a Hassiubyte. *1024^63
        /// </summary>
        public const double HB = DB * 1024;

        /// <summary>
        /// Number of bytes in a Meitnerbyte. *1024^66
        /// </summary>
        public const double MrB = HB * 1024;

        /// <summary>
        /// Number of bytes in a Darmstadbyte. *1024^69
        /// </summary>
        public const double DdB = MrB * 1024;

        /// <summary>
        /// Number of bytes in a Roentbyte. *1024^72
        /// </summary>
        public const double RtB = DdB * 1024;

        /// <summary>
        /// Number of bytes in a Sophobyte. *1024^75
        /// </summary>
        public const double ShB = RtB * 1024;

        /// <summary>
        /// Number of bytes in a Coperbyte. *1024^78
        /// </summary>
        public const double CB = ShB * 1024; 

        /// <summary>
        /// Number of bytes in a Koentekbyte. *1024^81
        /// </summary>
        public const double KkB = CB * 1024;

        /// <summary>
        /// Number of bytes in a CornelionByte. *1024^100000000
        /// Cornelion = "1 x 10 ^ 100,000,000";
        /// </summary>
        public const string CornelionByte = "1 x 10 ^ 100,000,000";
        #endregion Binary Constants

        #region CacheType
        /// <summary>
        /// The types of cache that can be used.
        /// </summary>
        public enum CacheType
        {
            Azure,
            LocalDisk
        }
        #endregion CacheType

        #region CompoundFrequency
        /// <summary>
        /// Enum of possible frequency in which interest compounding is done.
        /// </summary>
        public enum CompoundFrequency 
        { 
            /// <summary>
            /// Frequency 12 time per year.
            /// </summary>
            Monthly, 

            /// <summary>
            /// Frequency 4 times per year.
            /// </summary>
            Quarterly,

            /// <summary>
            /// Frequency once per year.
            /// </summary>
            Yearly 
        };
        #endregion CompoundFrequency

        #region Cryptography
        /// <summary>
        /// List of encryption providers to use.
        /// </summary>
        public enum EncryptionProvider
        {
            AES,
            DES,
            DSA,
            MD5,
            RNG,
            RSA,
            SHA1,
            SHA256,
            SHA384,
            SHA512,
            TrippleDES
        }
        #endregion Cryptography

        #region Enumeration
        /// <summary>
        /// Enum of possible enumerable types.
        /// </summary>
        public enum EnumerableType 
        { 
            Array,
            List,
            Dictionary
        }
        #endregion Enumeration

        #region ErrorType
        /// <summary>
        /// Enum of error types to validate.
        /// </summary>
        public enum ErrorType 
        { 
            Null, 
            IntNonNegative 
        };

        /// <summary>
        /// Array holding all error types for easier passing.
        /// </summary>
        public static ErrorType[] ErrorTypeAll =
        {
            ErrorType.Null,
            ErrorType.IntNonNegative
        };
        #endregion ErrorType

        #region Graph Constants
        /// <summary>
        /// The types of data that can be returned after the duplication of
        /// an M365 Group i.e. the GUID ID of the Group or the Group itself.
        /// </summary>
        public enum DuplicateGroupReturnType
        {
            /// <summary>
            /// Return the Microsoft.Graph.Models.Group value.
            /// </summary>
            Group,

            /// <summary>
            /// Return just the GUID ID value of the Group.
            /// </summary>
            Id
        }

        /// <summary>
        /// The types of parent containers that could hold 
        /// Microsoft.Graph.Models.Drive child items.
        /// </summary>
        public enum GraphDriveParentType
        {
            /// <summary>
            /// A Microsoft.Graph.Models.Group parent.
            /// </summary>
            Group,

            /// <summary>
            /// A Microsoft.Graph.Models.Site parent.
            /// </summary>
            Site,

            /// <summary>
            /// A Microsoft.Graph.Models.User parent.
            /// </summary>
            User
        }

        /// <summary>
        /// The types of Microsoft.Graph.Models objects that can be returned
        /// through the generic Extensions.Graph.Get() method.
        /// </summary>
        public enum GraphObjectType
        {
            //ActivityHistoryItem,
            //Agreement,
            //Alert,
            //AllowedValue,
            //Application,
            //AppLog,
            //AppRoleAssignment,
            //Approval,
            //Attachment,
            //AttributeSet,
            //AuditEvent,
            //Callendar,
            //Call,
            //ChangeNotification,
            //Channel,
            //Chat,
            //Contact,
            //ContentType,
            //Device,
            //DirectoryObject,
            //Domain,
            Drive,
            DriveItem,
            //DriveItemVersion,
            //Endpoint,
            //Event,
            //Extension,
            Group,
            //IdentityProvider,
            List,
            ListItem,
            //ListItemVersion,
            //LongRunningOperation,
            //MailFolder,
            //ManagedDevice,
            //ManagedMobileApp,
            //Message,
            //MobileApp,
            //NamedLocation,
            //Notebook,
            //Organization,
            //Participant,
            //Payload,
            //Permission,
            //Person,
            //Post,
            //Presence,
            //ProfilePhoto,
            //ResourceOperation,
            //RiskyServicePrincipal,
            //RiskyUser,
            //RoleAssignment,
            //RoleDefinition,
            //ServiceHealth,
            //ServicePrincipal,
            //SharedDriveItem,
            //SignIn,
            Site,
            //SubscribedSku,
            //Subscription,
            Team,
            //TeamsApp,
            //TeamsTab,
            //ThreatAssessmentRequest,
            //ThreadAssessmentResult,
            //UnifiedRoleAssignment,
            //UnifiedRoleDefinition,
            //UserActivity,
            User,
            //WindowsInformationProtectionPolicy,
            //WindowsInformationProtectionResource,
            //WorkbookChart,
            //WorkbookNamedItem,
            //WorkbookOperation,
            //WorkbookPivotTable,
            //WorkbookTable,
            //WorkbookTableColumn,
            //WorkbookTableRow,
            //WorkbookWorksheet
        }

        /// <summary>
        /// The type of Group membership users to retrieve i.e. Owners of the
        /// Group, Members of the Group or both.
        /// </summary>
        [Obsolete("GroupUserMembershipType is deprecated.  Please use UserMembershipType instead.")]
        public enum GroupUserMembershipType
        {
            /// <summary>
            /// Return both Members and Owners of the Group.
            /// </summary>
            All,

            /// <summary>
            /// Return only Members of the Group.
            /// </summary>
            Members,

            /// <summary>
            /// Return only Owners of the Group.
            /// </summary>
            Owners
        }

        /// <summary>
        /// The type of string being passed to the .UpdateGroup() method i.e.
        /// the display name of the Group or the Group's GUID ID value.
        /// </summary>
        public enum GroupUpdateType
        {
            /// <summary>
            /// String value being passed to .UpdateGroup() represents the Display Name of the Group.
            /// </summary>
            DisplayName,

            /// <summary>
            /// /// String value being passed to .UpdateGroup() represents the GUID ID value of the Group.
            /// </summary>
            Guid
        }

        /// <summary>
        /// The types of Graph user info to retrieve.  The id, mail and
        /// userProfileName values use camel case to allow their .ToString()
        /// values to be used in the query constructor.
        /// </summary>
        public enum UserInfoType
        {
            /// <summary>
            /// Return all the entire Microsoft.Graph.Models.User object.
            /// </summary>
            All,

            /// <summary>
            /// Return just the ID value of the User object.
            /// Intentionally lower case as the value string is used in Graph
            /// data field selection queries.
            /// </summary>
            id,

            /// <summary>
            /// Return just the Email value of the User object.
            /// Intentionally lower case as the value string is used in Graph
            /// data field selection queries.
            /// </summary>
            mail,

            /// <summary>
            /// Return just the User Principal Name (UPN) of the User object.
            /// Intentionally lower case as the value string is used in Graph
            /// data field selection queries.
            /// </summary>
            userProfileName
        }

        /// <summary>
        /// The type of membership of users to retrieve i.e. Owners, Members 
        /// or both.
        /// </summary>
        public enum UserMembershipType
        {
            /// <summary>
            /// Both Members and Owners.
            /// </summary>
            All,

            /// <summary>
            /// Only Members.
            /// </summary>
            Members,

            /// <summary>
            /// Only Owners.
            /// </summary>
            Owners,

            /// <summary>
            /// Only Visitors.
            /// </summary>
            Visitors
        }
        #endregion Graph Constants

        #region Help
        /// <summary>
        /// String array of args that will trigger help text.
        /// </summary>
        public readonly static string[] HelpStrings =
        {
            "/?",
            "?",
            "/help",
            "help",
            "-help",
            "/huh",
            "huh",
            "-huh"
        };
        #endregion Help

        #region Hexadecimal
        /// <summary>
        /// Character array of all hexadecimal lower case characters.
        /// </summary>
        public readonly static char[] HexChars =
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F'
        };
        #endregion Hexadecimal

        #region Logging
        /// <summary>
        /// Define the type of messages that can be written.
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Error message type that writes red output to console.
            /// </summary>
            Error,

            /// <summary>
            /// Warning message type that writes yellow output to console.
            /// </summary>
            Warning,

            /// <summary>
            /// Information message type that writes gray output to console.
            /// </summary>
            Information,

            /// <summary>
            /// Verbose message type that writes cyan output to console.
            /// </summary>
            Verbose
        }

        /// <summary>
        /// Define the types of logs that can be written.
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// Write to console.
            /// </summary>
            Console,

            /// <summary>
            /// Write to a logging file.
            /// </summary>
            File,

            /// <summary>
            /// Write to the Event Log.
            /// </summary>
            EventLog,

            /// <summary>
            /// Write to a SharePoint list.
            /// </summary>
            SPList,

            /// <summary>
            /// Write to a given ILogger.
            /// </summary>
            ILogger,

            /// <summary>
            /// Write to database.
            /// </summary>
            Database
        }
        #endregion Logging

        #region LoremIpsum
        /// <summary>
        /// String array of lorem ipsum text.
        /// </summary>
        public readonly static string[] LoremIpsum =
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer aliquam arcu rhoncus erat consectetur, quis rutrum augue tincidunt. Suspendisse elit ipsum, lobortis lobortis tellus eu, vulputate fringilla lorem. Cras molestie nibh sed turpis dapibus sollicitudin ut a nulla. Suspendisse blandit suscipit egestas. Nunc et ante mattis nulla vehicula rhoncus. Vivamus commodo nunc id ultricies accumsan. Mauris vitae ante ut justo venenatis tempus.",
            "Nunc posuere, nisi eu convallis convallis, quam urna sagittis ipsum, et tempor ante libero ac ex. Aenean lacus mi, blandit non eros luctus, ultrices consectetur nunc. Vivamus suscipit justo odio, a porta massa posuere ac. Aenean varius leo non ipsum porttitor eleifend. Phasellus accumsan ultrices massa et finibus. Nunc vestibulum augue ut bibendum facilisis. Donec est massa, lobortis quis molestie at, placerat a neque. Donec quis bibendum leo. Pellentesque ultricies ac odio id pharetra. Nulla enim massa, lacinia nec nunc nec, egestas pulvinar odio. Sed pulvinar molestie justo, eu hendrerit nunc blandit eu. Suspendisse et sapien quis ipsum scelerisque rutrum.",
            "Mauris eget convallis lorem, rutrum venenatis risus. Cras turpis risus, convallis nec lectus id, blandit varius ante. Morbi id turpis vel neque gravida consequat in elementum tellus. Fusce venenatis ex eget quam tincidunt varius. Mauris non mauris est. Vestibulum eget pharetra risus, sit amet accumsan elit. Etiam rhoncus tristique mauris. Ut convallis dignissim dictum. Vivamus dolor augue, vulputate a consequat ut, euismod finibus mi. Morbi sit amet pellentesque lectus.",
            "Nullam mattis cursus lorem ut venenatis. Praesent et sapien at tellus feugiat varius non eget orci. Maecenas sodales orci vitae rhoncus posuere. Aliquam erat volutpat. Nullam nulla sapien, faucibus sit amet porttitor vitae, fermentum ut orci. Etiam accumsan lacus quis tortor posuere, vitae tristique urna porta. Proin urna velit, lobortis ut gravida sit amet, iaculis vitae tellus. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Etiam porttitor metus eu finibus malesuada. Vivamus lobortis mauris id erat condimentum, ac laoreet mauris tristique. Fusce sit amet arcu purus. Donec quam enim, ultrices luctus nisi eget, vestibulum porta enim. Fusce sed iaculis metus, nec fermentum nisl. Pellentesque in condimentum risus, a fringilla turpis.",
            "In in interdum lectus. Quisque aliquet sem ac ante tincidunt, at sodales libero mollis. In suscipit felis vitae mauris ultricies, at commodo magna hendrerit. Pellentesque convallis, justo a fermentum dapibus, augue quam tempor lectus, ac dignissim magna tellus luctus odio. Morbi sed vestibulum diam. Proin sodales urna vitae ex cursus volutpat. Cras dapibus quam velit, eu ultrices est rhoncus id. Sed sit amet ligula eget nisl tempus iaculis. Donec et lacus a tellus volutpat suscipit sed in nisi. Vivamus placerat semper ex et pellentesque.",
            "Sed pulvinar felis ut ipsum feugiat sollicitudin. Mauris ut nisi vel nibh vestibulum pellentesque. Sed dignissim rhoncus mattis. Duis in placerat magna. Duis interdum lorem sed consequat molestie. Proin maximus dolor sit amet placerat pellentesque. Proin porttitor magna at ante vestibulum, sed egestas tortor maximus. Vestibulum nec tristique neque, eget euismod mauris. Aliquam non enim metus. Curabitur iaculis tellus dui, id tempor metus fringilla id. Phasellus ante tellus, egestas eu risus in, lacinia molestie purus. Fusce ultricies vehicula massa a vehicula. Proin at magna quis nunc faucibus ultricies eget eu orci. Aenean cursus lorem eros, in tempor magna mattis sed. Donec ac bibendum risus. Phasellus urna ante, sodales at leo eu, lobortis faucibus risus.",
            "Cras aliquam metus vel purus suscipit vehicula. Curabitur dignissim velit eget ante mollis sollicitudin. Sed id lorem nec mi varius aliquet at nec lorem. Nullam vel lorem libero. Mauris rhoncus dolor non facilisis feugiat. Suspendisse cursus vel elit non porta. Duis sodales vel nisl non pharetra.",
            "Integer mi libero, tincidunt id erat ut, sollicitudin laoreet est. Aliquam non lectus luctus, placerat ligula semper, vestibulum arcu. Fusce ut lobortis quam. Nunc et mollis lectus. Curabitur condimentum ac nisi quis congue. Interdum et malesuada fames ac ante ipsum primis in faucibus. Nam porttitor elit eget elit finibus, sed sagittis tortor aliquet. Sed et accumsan dui, lobortis luctus eros. Cras a dolor turpis. Aenean mollis, nulla sit amet eleifend cursus, arcu erat dignissim neque, non egestas ligula metus at purus. Etiam hendrerit magna ut vehicula sodales. Duis dictum lorem in magna bibendum eleifend. Cras sed diam ut sem pretium vestibulum sed non nibh. Morbi finibus enim nec lacus aliquet, id convallis sapien vulputate.",
            "In hac habitasse platea dictumst. Vivamus sit amet ornare magna. Curabitur sollicitudin viverra nibh, sit amet tempor nisl dictum nec. Integer in auctor sem. Phasellus accumsan ante mollis dui feugiat, non semper augue posuere. Cras ac faucibus nulla, id elementum ante. Nulla ipsum felis, semper id eleifend in, placerat nec ipsum. Nunc ut leo tincidunt, facilisis leo egestas, sollicitudin lacus. Curabitur at ex eget libero convallis volutpat.",
            "Nulla facilisi. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque eu tristique lacus, sed porta nisl. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vestibulum libero neque, faucibus sed condimentum sed, aliquet vitae massa. Sed dignissim a velit tempus feugiat. Etiam sollicitudin, neque sit amet posuere semper, enim enim facilisis quam, ac sollicitudin lectus purus ut urna. Vivamus interdum odio nec felis interdum mollis. Praesent et massa quis augue accumsan laoreet. Nullam ultrices, dui ac condimentum cursus, lectus elit molestie velit, vitae consequat elit arcu sed massa. Praesent at lacinia ante, varius lacinia urna. Donec pretium gravida nunc, sed congue mi tincidunt eu. Aenean ut diam eget orci dignissim placerat. Pellentesque fermentum aliquet velit et fermentum. Etiam ut risus dapibus, dictum augue ac, ornare metus."
        };
        #endregion LoremIpsum

        #region Mersenne
        /// <summary>
        /// Enum of possible comparison types in the Mersenne type.
        /// </summary>
        public enum MersenneComparisonType 
        { 
            /// <summary>
            /// Use a greater than comparrison.
            /// </summary>
            Greater, 

            /// <summary>
            /// Use a less than comparrison.
            /// </summary>
            Less 
        };
        #endregion Mersenne

        #region Morse
        /// <summary>
        /// Dictionary of alphabetic to Morse code translation values.
        /// </summary>
        public readonly static Dictionary<char, string> MorseCode = new Dictionary<char, string>()
        {
            {'a', ".-"},
            {'b', "-..."},
            {'c', "-.-."},
            {'d', "-.."},
            {'e', "."},
            {'f', "..-."},
            {'g', "--."},
            {'h', "...."},
            {'i', ".."},
            {'j', ".---"},
            {'k', "-.-"},
            {'l', ".-.."},
            {'m', "--"},
            {'n', "-."},
            {'o', "---"},
            {'p', ".--."},
            {'q', "--.-"},
            {'r', ".-."},
            {'s', "..."},
            {'t', "-"},
            {'u', "..-"},
            {'v', "...-"},
            {'w', ".--"},
            {'x', "-..-"},
            {'y', "-.--"},
            {'z', "--.."},
            {'0', "-----"},
            {'1', ".----"},
            {'2', "..---"},
            {'3', "...--"},
            {'4', "....-"},
            {'5', "....."},
            {'6', "-...."},
            {'7', "--..."},
            {'8', "---.."},
            {'9', "----."},
            {'.',".-.-.-"},
            {',',"--.--"},
            {'?',"..--.."},
            {' ',"-...-"}
        };
        #endregion Morse

        #region Quote
        /// <summary>
        /// Enum of possible quote types.
        /// </summary>
        public enum QuoteType 
        { 
            /// <summary>
            /// Single quotes i.e. 'C'.
            /// </summary>
            Single, 

            /// <summary>
            /// Double quotes i.e. "C".
            /// </summary>
            Double 
        };
        #endregion Quote

        #region Substring
        /// <summary>
        /// List of processing options for .Substring() method.
        /// </summary>
        public enum SubstringType 
        { 
            /// <summary>
            /// The substring is calculated from the front of the string.
            /// </summary>
            FromHead, 

            /// <summary>
            /// The substring is calculated from the back of the string.
            /// </summary>
            FromTail, 

            /// <summary>
            /// The substring saught is to the left of the matching index location.
            /// </summary>
            LeftOfIndex, 

            /// <summary>
            /// The substring saught is to the right of the matching index location.
            /// </summary>
            RigthOfIndex, 

            /// <summary>
            /// The substring is the value of the index.
            /// </summary>
            IndexValue 
        };
        #endregion Substring

        #region TimeSpan
        [Obsolete("Use TimeSpanType instead.")]
        /// <summary>
        /// Enum for the timespan type returned from .Sum()
        /// </summary>
        public enum TimeSpanSumType 
        { 
            /// <summary>
            /// Number of seconds.
            /// </summary>
            Seconds, 

            /// <summary>
            /// Number of minutes.
            /// </summary>
            Minutes, 

            /// <summary>
            /// Number of hours.
            /// </summary>
            Hours, 

            /// <summary>
            /// Number of days.
            /// </summary>
            Days, 

            /// <summary>
            /// Number of weeks.
            /// </summary>
            Weeks, 

            /// <summary>
            /// Number of years.
            /// </summary>
            Years, 

            /// <summary>
            /// Number of decades.
            /// </summary>
            Decades, 

            /// <summary>
            /// Number of centuries.
            /// </summary>
            Centuries, 

            /// <summary>
            /// Number of mellinnia.
            /// </summary>
            Mellinnia
        };

        /// <summary>
        /// The type of timespan.
        /// </summary>
        public enum TimeSpanType
        {
            /// <summary>
            /// Number of seconds.
            /// </summary>
            Seconds,

            /// <summary>
            /// Number of minutes.
            /// </summary>
            Minutes,

            /// <summary>
            /// Number of hours.
            /// </summary>
            Hours,

            /// <summary>
            /// Number of days.
            /// </summary>
            Days,

            /// <summary>
            /// Number of weeks.
            /// </summary>
            Weeks,

            /// <summary>
            /// Number of months.
            /// </summary>
            Months,

            /// <summary>
            /// Number of years.
            /// </summary>
            Years,

            /// <summary>
            /// Number of decades.
            /// </summary>
            Decades,

            /// <summary>
            /// Number of centuries.
            /// </summary>
            Centuries,

            /// <summary>
            /// Number of mellinnia.
            /// </summary>
            Mellinnia
        };
        #endregion TimeSpan

        #region TimeZone
        /// <summary>
        /// Enum of possible time zones.
        /// </summary>
        public enum TimeZone
        {
            AfghanistanStandardTime,
            AlaskanStandardTime,
            AleutianStandardTime,
            AltaiStandardTime,
            ArabStandardTime,
            ArabianStandardTime,
            ArabicStandardTime,
            ArgentinaStandardTime,
            AstrakhanStandardTime,
            AtlanticStandardTime,
            AUSCentralStandardTime,
            AusCentralWStandardTime,
            AUSEasternStandardTime,
            AzerbaijanStandardTime,
            AzoresStandardTime,
            BahiaStandardTime,
            BangladeshStandardTime,
            BelarusStandardTime,
            BougainvilleStandardTime,
            CanadaCentralStandardTime,
            CapeVerdeStandardTime,
            CaucasusStandardTime,
            CenAustraliaStandardTime,
            CentralAmericaStandardTime,
            CentralAsiaStandardTime,
            CentralBrazilianStandardTime,
            CentralEuropeStandardTime,
            CentralEuropeanStandardTime,
            CentralPacificStandardTime,
            CentralStandardTime,
            CentralStandardTimeMexico,
            ChathamIslandsStandardTime,
            ChinaStandardTime,
            CubaStandardTime,
            DatelineStandardTime,
            EAfricaStandardTime,
            EAustraliaStandardTime,
            EEuropeStandardTime,
            ESouthAmericaStandardTime,
            EasterIslandStandardTime,
            EasternStandardTime,
            EasternStandardTimeMexico,
            EgyptStandardTime,
            EkaterinburgStandardTime,
            FijiStandardTime,
            FLEStandardTime,
            GeorgianStandardTime,
            GMTStandardTime,
            GreenlandStandardTime,
            GreenwichStandardTime,
            GTBStandardTime,
            HaitiStandardTime,
            HawaiianStandardTime,
            IndiaStandardTime,
            IranStandardTime,
            IsraelStandardTime,
            JordanStandardTime,
            KaliningradStandardTime,
            KamchatkaStandardTime,
            KoreaStandardTime,
            LibyaStandardTime,
            LineIslandsStandardTime,
            LordHoweStandardTime,
            MagadanStandardTime,
            MagallanesStandardTime,
            MarquesasStandardTime,
            MauritiusStandardTime,
            MidAtlanticStandardTime,
            MiddleEastStandardTime,
            MontevideoStandardTime,
            MoroccoStandardTime,
            MountainStandardTime,
            MountainStandardTimeMexico,
            MyanmarStandardTime,
            NCentralAsiaStandardTime,
            NamibiaStandardTime,
            NepalStandardTime,
            NewZealandStandardTime,
            NewfoundlandStandardTime,
            NorfolkStandardTime,
            NorthAsiaEastStandardTime,
            NorthAsiaStandardTime,
            NorthKoreaStandardTime,
            OmskStandardTime,
            PacificSAStandardTime,
            PacificStandardTime,
            PacificStandardTimeMexico,
            PakistanStandardTime,
            ParaguayStandardTime,
            QyzylordaStandardTime,
            RomanceStandardTime,
            RussiaTimeZone10,
            RussiaTimeZone11,
            RussiaTimeZone3,
            RussianStandardTime,
            SAEasternStandardTime,
            SAPacificStandardTime,
            SAWesternStandardTime,
            SaintPierreStandardTime,
            SakhalinStandardTime,
            SamoaStandardTime,
            SaoTomeStandardTime,
            SaratovStandardTime,
            SEAsiaStandardTime,
            SingaporeStandardTime,
            SouthAfricaStandardTime,
            SriLankaStandardTime,
            SudanStandardTime,
            SyriaStandardTime,
            TaipeiStandardTime,
            TasmaniaStandardTime,
            TocantinsStandardTime,
            TokyoStandardTime,
            TomskStandardTime,
            TongaStandardTime,
            TransbaikalStandardTime,
            TurkeyStandardTime,
            TurksAndCaicosStandardTime,
            UlaanbaatarStandardTime,
            USEasternStandardTime,
            USMountainStandardTime,
            UTC,
            UTCPlus12,
            UTCPlus13,
            UTCMinus02,
            UTCMinus08,
            UTCMinus09,
            UTCMinus11,
            VenezuelaStandardTime,
            VladivostokStandardTime,
            VolgogradStandardTime,
            WAustraliaStandardTime,
            WCentralAfricaStandardTime,
            WEuropeStandardTime,
            WMongoliaStandardTime,
            WestAsiaStandardTime,
            WestBankStandardTime,
            WestPacificStandardTime,
            YakutskStandardTime,
            YukonStandardTime
        };

        /// <summary>
        /// Dictionary of time zones and their TimeZoneInfo registry strings.
        /// </summary>
        public readonly static Dictionary<TimeZone, string> TimeZones = new Dictionary<TimeZone, string>()
        {
            { TimeZone.AfghanistanStandardTime, "Afghanistan Standard Time" },
            { TimeZone.AlaskanStandardTime, "Alaskan Standard Time" },
            { TimeZone.AleutianStandardTime, "Aleutian Standard Time" },
            { TimeZone.AltaiStandardTime, "Altai Standard Time" },
            { TimeZone.ArabStandardTime, "Arab Standard Time" },
            { TimeZone.ArabianStandardTime, "Arabian Standard Time" },
            { TimeZone.ArabicStandardTime, "Arabic Standard Time" },
            { TimeZone.ArgentinaStandardTime, "Argentina Standard Time" },
            { TimeZone.AstrakhanStandardTime, "Astrakhan Standard Time" },
            { TimeZone.AtlanticStandardTime, "Atlantic Standard Time" },
            { TimeZone.AUSCentralStandardTime, "AUS Central Standard Time" },
            { TimeZone.AusCentralWStandardTime, "Aus Central W. Standard Time" },
            { TimeZone.AUSEasternStandardTime, "AUS Eastern Standard Time" },
            { TimeZone.AzerbaijanStandardTime, "Azerbaijan Standard Time" },
            { TimeZone.AzoresStandardTime, "Azores Standard Time" },
            { TimeZone.BahiaStandardTime, "Bahia Standard Time" },
            { TimeZone.BangladeshStandardTime, "Bangladesh Standard Time" },
            { TimeZone.BelarusStandardTime, "Belarus Standard Time" },
            { TimeZone.BougainvilleStandardTime, "Bougainville Standard Time" },
            { TimeZone.CanadaCentralStandardTime, "Canada Central Standard Time" },
            { TimeZone.CapeVerdeStandardTime, "Cape Verde Standard Time" },
            { TimeZone.CaucasusStandardTime, "Caucasus Standard Time" },
            { TimeZone.CenAustraliaStandardTime, "Cen. Australia Standard Time" },
            { TimeZone.CentralAmericaStandardTime, "Central America Standard Time" },
            { TimeZone.CentralAsiaStandardTime, "Central Asia Standard Time" },
            { TimeZone.CentralBrazilianStandardTime, "Central Brazilian Standard Time" },
            { TimeZone.CentralEuropeStandardTime, "Central Europe Standard Time" },
            { TimeZone.CentralEuropeanStandardTime, "Central European Standard Time" },
            { TimeZone.CentralPacificStandardTime, "Central Pacific Standard Time" },
            { TimeZone.CentralStandardTime, "Central Standard Time" },
            { TimeZone.CentralStandardTimeMexico, "Central Standard Time (Mexico)" },
            { TimeZone.ChathamIslandsStandardTime, "Chatham Islands Standard Time" },
            { TimeZone.ChinaStandardTime, "China Standard Time" },
            { TimeZone.CubaStandardTime, "Cuba Standard Time" },
            { TimeZone.DatelineStandardTime, "Dateline Standard Time" },
            { TimeZone.EAfricaStandardTime, "E. Africa Standard Time" },
            { TimeZone.EAustraliaStandardTime, "E. Australia Standard Time" },
            { TimeZone.EEuropeStandardTime, "E. Europe Standard Time" },
            { TimeZone.ESouthAmericaStandardTime, "E. South America Standard Time" },
            { TimeZone.EasterIslandStandardTime, "Easter Island Standard Time" },
            { TimeZone.EasternStandardTime, "Eastern Standard Time" },
            { TimeZone.EasternStandardTimeMexico, "Eastern Standard Time (Mexico)" },
            { TimeZone.EgyptStandardTime, "Egypt Standard Time" },
            { TimeZone.EkaterinburgStandardTime, "Ekaterinburg Standard Time" },
            { TimeZone.FijiStandardTime, "Fiji Standard Time" },
            { TimeZone.FLEStandardTime, "FLE Standard Time" },
            { TimeZone.GeorgianStandardTime, "Georgian Standard Time" },
            { TimeZone.GMTStandardTime, "GMT Standard Time" },
            { TimeZone.GreenlandStandardTime, "Greenland Standard Time" },
            { TimeZone.GreenwichStandardTime, "Greenwich Standard Time" },
            { TimeZone.GTBStandardTime, "GTB Standard Time" },
            { TimeZone.HaitiStandardTime, "Haiti Standard Time" },
            { TimeZone.HawaiianStandardTime, "Hawaiian Standard Time" },
            { TimeZone.IndiaStandardTime, "India Standard Time" },
            { TimeZone.IranStandardTime, "Iran Standard Time" },
            { TimeZone.IsraelStandardTime, "Israel Standard Time" },
            { TimeZone.JordanStandardTime, "Jordan Standard Time" },
            { TimeZone.KaliningradStandardTime, "Kaliningrad Standard Time" },
            { TimeZone.KamchatkaStandardTime, "Kamchatka Standard Time" },
            { TimeZone.KoreaStandardTime, "Korea Standard Time" },
            { TimeZone.LibyaStandardTime, "Libya Standard Time" },
            { TimeZone.LineIslandsStandardTime, "Line Islands Standard Time" },
            { TimeZone.LordHoweStandardTime, "Lord Howe Standard Time" },
            { TimeZone.MagadanStandardTime, "Magadan Standard Time" },
            { TimeZone.MagallanesStandardTime, "Magallanes Standard Time" },
            { TimeZone.MarquesasStandardTime, "Marquesas Standard Time" },
            { TimeZone.MauritiusStandardTime, "Mauritius Standard Time" },
            { TimeZone.MidAtlanticStandardTime, "Mid-Atlantic Standard Time" },
            { TimeZone.MiddleEastStandardTime, "Middle East Standard Time" },
            { TimeZone.MontevideoStandardTime, "Montevideo Standard Time" },
            { TimeZone.MoroccoStandardTime, "Morocco Standard Time" },
            { TimeZone.MountainStandardTime, "Mountain Standard Time" },
            { TimeZone.MountainStandardTimeMexico, "Mountain Standard Time (Mexico)" },
            { TimeZone.MyanmarStandardTime, "Myanmar Standard Time" },
            { TimeZone.NCentralAsiaStandardTime, "N. Central Asia Standard Time" },
            { TimeZone.NamibiaStandardTime, "Namibia Standard Time" },
            { TimeZone.NepalStandardTime, "Nepal Standard Time" },
            { TimeZone.NewZealandStandardTime, "New Zealand Standard Time" },
            { TimeZone.NewfoundlandStandardTime, "Newfoundland Standard Time" },
            { TimeZone.NorfolkStandardTime, "Norfolk Standard Time" },
            { TimeZone.NorthAsiaEastStandardTime, "North Asia East Standard Time" },
            { TimeZone.NorthAsiaStandardTime, "North Asia Standard Time" },
            { TimeZone.NorthKoreaStandardTime, "North Korea Standard Time" },
            { TimeZone.OmskStandardTime, "Omsk Standard Time" },
            { TimeZone.PacificSAStandardTime, "Pacific SA Standard Time" },
            { TimeZone.PacificStandardTime, "Pacific Standard Time" },
            { TimeZone.PacificStandardTimeMexico, "Pacific Standard Time (Mexico)" },
            { TimeZone.PakistanStandardTime, "Pakistan Standard Time" },
            { TimeZone.ParaguayStandardTime, "Paraguay Standard Time" },
            { TimeZone.QyzylordaStandardTime, "Qyzylorda Standard Time" },
            { TimeZone.RomanceStandardTime, "Romance Standard Time" },
            { TimeZone.RussiaTimeZone10, "Russia Time Zone 10" },
            { TimeZone.RussiaTimeZone11, "Russia Time Zone 11" },
            { TimeZone.RussiaTimeZone3, "Russia Time Zone 3" },
            { TimeZone.RussianStandardTime, "Russian Standard Time" },
            { TimeZone.SAEasternStandardTime, "SA Eastern Standard Time" },
            { TimeZone.SAPacificStandardTime, "SA Pacific Standard Time" },
            { TimeZone.SAWesternStandardTime, "SA Western Standard Time" },
            { TimeZone.SaintPierreStandardTime, "Saint Pierre Standard Time" },
            { TimeZone.SakhalinStandardTime, "Sakhalin Standard Time" },
            { TimeZone.SamoaStandardTime, "Samoa Standard Time" },
            { TimeZone.SaoTomeStandardTime, "Sao Tome Standard Time" },
            { TimeZone.SaratovStandardTime, "Saratov Standard Time" },
            { TimeZone.SEAsiaStandardTime, "SE Asia Standard Time" },
            { TimeZone.SingaporeStandardTime, "Singapore Standard Time" },
            { TimeZone.SouthAfricaStandardTime, "South Africa Standard Time" },
            { TimeZone.SriLankaStandardTime, "Sri Lanka Standard Time" },
            { TimeZone.SudanStandardTime, "Sudan Standard Time" },
            { TimeZone.SyriaStandardTime, "Syria Standard Time" },
            { TimeZone.TaipeiStandardTime, "Taipei Standard Time" },
            { TimeZone.TasmaniaStandardTime, "Tasmania Standard Time" },
            { TimeZone.TocantinsStandardTime, "Tocantins Standard Time" },
            { TimeZone.TokyoStandardTime, "Tokyo Standard Time" },
            { TimeZone.TomskStandardTime, "Tomsk Standard Time" },
            { TimeZone.TongaStandardTime, "Tonga Standard Time" },
            { TimeZone.TransbaikalStandardTime, "Transbaikal Standard Time" },
            { TimeZone.TurkeyStandardTime, "Turkey Standard Time" },
            { TimeZone.TurksAndCaicosStandardTime, "Turks And Caicos Standard Time" },
            { TimeZone.UlaanbaatarStandardTime, "Ulaanbaatar Standard Time" },
            { TimeZone.USEasternStandardTime, "US Eastern Standard Time" },
            { TimeZone.USMountainStandardTime, "US Mountain Standard Time" },
            { TimeZone.UTC, "UTC" },
            { TimeZone.UTCPlus12, "UTC+12" },
            { TimeZone.UTCPlus13, "UTC+13" },
            { TimeZone.UTCMinus02, "UTC-02" },
            { TimeZone.UTCMinus08, "UTC-08" },
            { TimeZone.UTCMinus09, "UTC-09" },
            { TimeZone.UTCMinus11, "UTC-11" },
            { TimeZone.VenezuelaStandardTime, "Venezuela Standard Time" },
            { TimeZone.VladivostokStandardTime, "Vladivostok Standard Time" },
            { TimeZone.VolgogradStandardTime, "Volgograd Standard Time" },
            { TimeZone.WAustraliaStandardTime, "W. Australia Standard Time" },
            { TimeZone.WCentralAfricaStandardTime, "W. Central Africa Standard Time" },
            { TimeZone.WEuropeStandardTime, "W. Europe Standard Time" },
            { TimeZone.WMongoliaStandardTime, "W. Mongolia Standard Time" },
            { TimeZone.WestAsiaStandardTime, "West Asia Standard Time" },
            { TimeZone.WestBankStandardTime, "West Bank Standard Time" },
            { TimeZone.WestPacificStandardTime, "West Pacific Standard Time" },
            { TimeZone.YakutskStandardTime, "Yakutsk Standard Time" },
            { TimeZone.YukonStandardTime, "Yukon Standard Time" }
        };
        #endregion TimeZone
    }
}
