namespace EmotivUnityPlugin
{
    /// <summary>
    /// Stores API parameters
    /// </summary>
    static class Config
    {
        /// <summary>ClientId of your application.
        /// <para>To get a client id and a client secret, you must connect to your Emotiv
        /// account on emotiv.com and create a Cortex app.
        /// https://www.emotiv.com/my-account/cortex-apps/.</para></summary>
        public static string AppClientId = "rsa8GBfSBf1sc9q5FKqv5YBzXK1Lv80ZHO2XXO3z";
        public static string AppClientSecret = "ENqLDk0ihytkaV8ERob6D6aK85xrejTCqMKFn1wEkosbfbPaOB9B6L3CYqrB2bOcmeZcrTgeKj4R5HDlqlSBJaDJkNw3zp0Xka4r97SYNcdW6ljRAHzgvGtcLWS4zwpj";

        public static string AppUrl = "wss://localhost:6868"; // default
        public static string AppVersion = "0 Dev"; // default
        public static string AppName = "Curl!"; // default app name

        /// <summary>
        /// Name of directory where contain tmp data and logs file.
        /// </summary>
        public static string TmpAppDataDir = "Curl!";
        public static string EmotivAppsPath = "C:\\Program Files\\EmotivApps";
        public static string TmpVersionFileName = "version.ini";
        public static string TmpDataFileName = "data.dat";
        public static string ProfilesDir = "Profiles";
        public static string LogsDir = "logs";
        public static int QUERY_HEADSET_TIME = 1000;
        public static int TIME_CLOSE_STREAMS = 1000;
        public static int RETRY_CORTEXSERVICE_TIME = 5000;
        public static int WAIT_USERLOGIN_TIME = 5000;

        public static int MENTAL_COMMAND_BUFFER_SIZE;

        public static System.Collections.Generic.List<string> dataStreams = new System.Collections.Generic.List<string>
            { DataStreamName.MentalCommands, DataStreamName.SysEvents, DataStreamName.DevInfos };

        // If you use an Epoc Flex headset, then you must put your configuration here
        // TODO: need detail here
        public static string FlexMapping = @"{
                                  'CMS':'TP8', 'DRL':'P6',
                                  'RM':'TP10','RN':'P4','RO':'P8'}";
    }

    public static class DataStreamName
    {
        public const string DevInfos = "dev";
        public const string EEG = "eeg";
        public const string Motion = "mot";
        public const string PerformanceMetrics = "met";
        public const string BandPower = "pow";
        public const string MentalCommands = "com";
        public const string FacialExpressions = "fac";
        public const string SysEvents = "sys";   // System events of the mental commands and facial expressions
        public const string EQ = "eq"; // EEG quality
    }

    public static class WarningCode
    {
        public const int StreamStop = 0;
        public const int SessionAutoClosed = 1;
        public const int UserLogin = 2;
        public const int UserLogout = 3;
        public const int ExtenderExportSuccess = 4;
        public const int ExtenderExportFailed = 5;
        public const int UserNotAcceptLicense = 6;
        public const int UserNotHaveAccessRight = 7;
        public const int UserRequestAccessRight = 8;
        public const int AccessRightGranted = 9;
        public const int AccessRightRejected = 10;
        public const int CannotDetectOSUSerInfo = 11;
        public const int CannotDetectOSUSername = 12;
        public const int ProfileLoaded = 13;
        public const int ProfileUnloaded = 14;
        public const int CortexAutoUnloadProfile = 15;
        public const int UserLoginOnAnotherOsUser = 16;
        public const int EULAAccepted = 17;
        public const int StreamWritingClosed = 18;
        public const int HeadsetWrongInformation = 100;
        public const int HeadsetCannotConnected = 101;
        public const int HeadsetConnectingTimeout = 102;
        public const int HeadsetDataTimeOut = 103;
        public const int HeadsetConnected = 104;
        public const int BTLEPermissionNotGranted = 31;
    }

    public static class DevStreamParams
    {
        public const string battery = "Battery";
        public const string signal = "Signal";
    }
}
