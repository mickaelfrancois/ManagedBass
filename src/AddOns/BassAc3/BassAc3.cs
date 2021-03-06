﻿using System;
using System.Runtime.InteropServices;

namespace ManagedBass.Ac3
{
    /// <summary>
    /// BassAc3 is a BASS addon enabling the playback of AC3 encoded files, including internet and user file streams.
    /// </summary> 
    /// <remarks>
    /// Supports .ac3
    /// </remarks>
    public static class BassAc3
    {
        const string DllName = "bass_ac3";
        
        [DllImport(DllName, CharSet = CharSet.Unicode)]
        static extern int BASS_AC3_StreamCreateFile(bool mem, string file, long offset, long length, BassFlags flags);

        [DllImport(DllName)]
        static extern int BASS_AC3_StreamCreateFile(bool mem, IntPtr file, long offset, long length, BassFlags flags);

        /// <summary>Create a stream from file.</summary>
        public static int CreateStream(string File, long Offset = 0, long Length = 0, BassFlags Flags = BassFlags.Default)
        {
            return BASS_AC3_StreamCreateFile(false, File, Offset, Length, Flags | BassFlags.Unicode);
        }

        /// <summary>Create a stream from Memory (IntPtr).</summary>
        public static int CreateStream(IntPtr Memory, long Offset, long Length, BassFlags Flags = BassFlags.Default)
        {
            return BASS_AC3_StreamCreateFile(true, new IntPtr(Memory.ToInt64() + Offset), 0, Length, Flags);
        }

        /// <summary>Create a stream from Memory (byte[]).</summary>
        public static int CreateStream(byte[] Memory, long Offset, long Length, BassFlags Flags)
        {
            return GCPin.CreateStreamHelper(Pointer => CreateStream(Pointer, Offset, Length, Flags), Memory);
        }

        [DllImport(DllName)]
        static extern int BASS_AC3_StreamCreateFileUser(StreamSystem system, BassFlags flags, [In, Out] FileProcedures procs, IntPtr user);

        /// <summary>Create a stream using User File Procedures.</summary>
        public static int CreateStream(StreamSystem System, BassFlags Flags, FileProcedures Procedures, IntPtr User = default(IntPtr))
        {
            var h = BASS_AC3_StreamCreateFileUser(System, Flags, Procedures, User);

            if (h != 0)
                ChannelReferences.Add(h, 0, Procedures);

            return h;
        }

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        static extern int BASS_AC3_StreamCreateURL(string Url, int Offset, BassFlags Flags, DownloadProcedure Procedure, IntPtr User);

        /// <summary>Create a stream from Url.</summary>
        public static int CreateStream(string Url, int Offset, BassFlags Flags, DownloadProcedure Procedure, IntPtr User = default(IntPtr))
        {
            var h = BASS_AC3_StreamCreateURL(Url, Offset, Flags | BassFlags.Unicode, Procedure, User);

            if (h != 0)
                ChannelReferences.Add(h, 0, Procedure);

            return h;
        }

        /// <summary>
        /// Enable Dynamic Range Compression (default is false).
        /// </summary>
        public static bool DRC
        {
            get => Bass.GetConfigBool(Configuration.AC3DynamicRangeCompression);
            set => Bass.Configure(Configuration.AC3DynamicRangeCompression, value);
        }
    }
}