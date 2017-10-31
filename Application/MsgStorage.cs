using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Reflection;
namespace FTBAPISERVER
{
    public class MsgStorage : IDisposable
    {
        #region CLZF 


        public class CLZF
        {

            static byte[] COMPRESSED_RTF_PREBUF;
            static string prebuf = "{\\rtf1\\ansi\\mac\\deff0\\deftab720{\\fonttbl;}" +
                "{\\f0\\fnil \\froman \\fswiss \\fmodern \\fscript " +
                "\\fdecor MS Sans SerifSymbolArialTimes New RomanCourier" +
                "{\\colortbl\\red0\\green0\\blue0\n\r\\par " +
                "\\pard\\plain\\f0\\fs20\\b\\i\\u\\tab\\tx";

            static uint[] CRC32_TABLE =
            {
                0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419,
                0x706AF48F, 0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4,
                0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07,
                0x90BF1D91, 0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
                0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
                0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
                0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4,
                0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
                0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3,
                0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A,
                0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599,
                0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
                0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190,
                0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
                0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E,
                0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
                0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED,
                0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
                0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3,
                0xFBD44C65, 0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
                0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A,
                0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5,
                0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
                0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
                0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17,
                0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6,
                0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615,
                0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
                0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344,
                0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
                0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A,
                0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
                0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1,
                0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C,
                0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF,
                0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
                0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE,
                0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31,
                0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C,
                0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
                0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
                0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
                0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1,
                0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
                0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278,
                0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7,
                0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66,
                0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
                0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605,
                0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
                0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B,
                0x2D02EF8D
            };

            static public int calculateCRC32(byte[] buf, int off, int len)
            {
                uint c = 0;
                int end = off + len;
                for (int i = off; i < end; i++)
                {
                    c = CRC32_TABLE[(c ^ buf[i]) & 0xFF] ^ (c >> 8);
                }
                return (int)c;
            }

            public static long getU32(byte[] buf, int offset)
            {
                return ((buf[offset] & 0xFF) | ((buf[offset + 1] & 0xFF) << 8) | ((buf[offset + 2] & 0xFF) << 16) | ((buf[offset + 3] & 0xFF) << 24)) & 0x00000000FFFFFFFFL;
            }

            public static int getU8(byte[] buf, int offset)
            {
                return buf[offset] & 0xFF;
            }

            public static byte[] decompressRTF(byte[] src)
            {
                byte[] dst;
                int inPos = 0;
                int outPos = 0;

                COMPRESSED_RTF_PREBUF = System.Text.Encoding.ASCII.GetBytes(prebuf);

                if (src == null || src.Length < 16)
                    throw new Exception("Invalid compressed-RTF header");

                int compressedSize = (int)getU32(src, inPos);
                inPos += 4;
                int uncompressedSize = (int)getU32(src, inPos);
                inPos += 4;
                int magic = (int)getU32(src, inPos);
                inPos += 4;
                int crc32 = (int)getU32(src, inPos);
                inPos += 4;

                if (compressedSize != src.Length - 4)
                    throw new Exception("compressed-RTF data size mismatch");

                if (crc32 != calculateCRC32(src, 16, src.Length - 16))
                    throw new Exception("compressed-RTF CRC32 failed");

                if (magic == 0x414c454d)
                {
                    dst = new byte[uncompressedSize];
                    Array.Copy(src, inPos, dst, outPos, uncompressedSize);
                }
                else if (magic == 0x75465a4c)
                {
                    dst = new byte[COMPRESSED_RTF_PREBUF.Length + uncompressedSize];
                    Array.Copy(COMPRESSED_RTF_PREBUF, 0, dst, 0, COMPRESSED_RTF_PREBUF.Length);
                    outPos = COMPRESSED_RTF_PREBUF.Length;
                    int flagCount = 0;
                    int flags = 0;
                    while (outPos < dst.Length)
                    {
                        flags = (flagCount++ % 8 == 0) ? getU8(src, inPos++) : flags >> 1;
                        if ((flags & 1) == 1)
                        {
                            int offset = getU8(src, inPos++);
                            int length = getU8(src, inPos++);
                            offset = (offset << 4) | (length >> 4);
                            length = (length & 0xF) + 2;
                            offset = (outPos / 4096) * 4096 + offset;
                            if (offset >= outPos)
                                offset -= 4096;
                            int end = offset + length;
                            while (offset < end)
                                dst[outPos++] = dst[offset++];
                        }
                        else
                        {
                            dst[outPos++] = src[inPos++];
                        }
                    }
                    src = dst;
                    dst = new byte[uncompressedSize];
                    Array.Copy(src, COMPRESSED_RTF_PREBUF.Length, dst, 0, uncompressedSize);
                }
                else
                {
                    throw new Exception("Unknown compression type (magic number " + magic + ")");
                }

                return dst;
            }
        }

        #endregion

        #region NativeMethods

        protected class NativeMethods
        {
            [DllImport("kernel32.dll")]
            static extern IntPtr GlobalLock(IntPtr hMem);

            [DllImport("ole32.DLL")]
            public static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out ILockBytes ppLkbyt);

            [DllImport("ole32.DLL")]
            public static extern int StgIsStorageILockBytes(ILockBytes plkbyt);

            [DllImport("ole32.DLL")]
            public static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, STGM grfMode, uint reserved, out IStorage ppstgOpen);

            [DllImport("ole32.DLL")]
            public static extern void StgOpenStorageOnILockBytes(ILockBytes plkbyt, IStorage pstgPriority, STGM grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstgOpen);

            [DllImport("ole32.DLL")]
            public static extern int StgIsStorageFile([MarshalAs(UnmanagedType.LPWStr)] string wcsName);

            [DllImport("ole32.DLL")]
            public static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IStorage pstgPriority, STGM grfMode, IntPtr snbExclude, int reserved, out IStorage ppstgOpen);

            [ComImport, Guid("0000000A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface ILockBytes
            {
                void ReadAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pcbRead);
                void WriteAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pcbWritten);
                void Flush();
                void SetSize([In, MarshalAs(UnmanagedType.U8)] long cb);
                void LockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);
                void UnlockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);
                void Stat([Out]out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In, MarshalAs(UnmanagedType.U4)] int grfStatFlag);
            }

            [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000B-0000-0000-C000-000000000046")]
            public interface IStorage
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                ComTypes.IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);
                [return: MarshalAs(UnmanagedType.Interface)]
                ComTypes.IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr reserved1, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved2);
                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);
                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr pstgPriority, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.U4)] int reserved);
                void CopyTo(int ciidExclude, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] pIIDExclude, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);
                void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In, MarshalAs(UnmanagedType.U4)] int grfFlags);
                void Commit(int grfCommitFlags);
                void Revert();
                void EnumElements([In, MarshalAs(UnmanagedType.U4)] int reserved1, IntPtr reserved2, [In, MarshalAs(UnmanagedType.U4)] int reserved3, [MarshalAs(UnmanagedType.Interface)] out IEnumSTATSTG ppVal);
                void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);
                void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);
                void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
                void SetClass([In] ref Guid clsid);
                void SetStateBits(int grfStateBits, int grfMask);
                void Stat([Out]out System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
            }

            [ComImport, Guid("0000000D-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IEnumSTATSTG
            {
                void Next(uint celt, [MarshalAs(UnmanagedType.LPArray), Out] System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt, out uint pceltFetched);
                void Skip(uint celt);
                void Reset();
                [return: MarshalAs(UnmanagedType.Interface)]
                IEnumSTATSTG Clone();
            }

            public enum STGM : int
            {
                DIRECT = 0x00000000,
                TRANSACTED = 0x00010000,
                SIMPLE = 0x08000000,
                READ = 0x00000000,
                WRITE = 0x00000001,
                READWRITE = 0x00000002,
                SHARE_DENY_NONE = 0x00000040,
                SHARE_DENY_READ = 0x00000030,
                SHARE_DENY_WRITE = 0x00000020,
                SHARE_EXCLUSIVE = 0x00000010,
                PRIORITY = 0x00040000,
                DELETEONRELEASE = 0x04000000,
                NOSCRATCH = 0x00100000,
                CREATE = 0x00001000,
                CONVERT = 0x00020000,
                FAILIFTHERE = 0x00000000,
                NOSNAPSHOT = 0x00200000,
                DIRECT_SWMR = 0x00400000
            }

            public const ushort PT_UNSPECIFIED = 0;
            public const ushort PT_NULL = 1;
            public const ushort PT_I2 = 2;
            public const ushort PT_LONG = 3;
            public const ushort PT_R4 = 4;
            public const ushort PT_DOUBLE = 5;
            public const ushort PT_CURRENCY = 6;
            public const ushort PT_APPTIME = 7;
            public const ushort PT_ERROR = 10;
            public const ushort PT_BOOLEAN = 11;
            public const ushort PT_OBJECT = 13;
            public const ushort PT_I8 = 20;
            public const ushort PT_STRING8 = 30;
            public const ushort PT_UNICODE = 31;
            public const ushort PT_SYSTIME = 64;
            public const ushort PT_CLSID = 72;
            public const ushort PT_BINARY = 258;

            public static IStorage CloneStorage(IStorage source, bool closeSource)
            {
                NativeMethods.IStorage memoryStorage = null;
                NativeMethods.ILockBytes memoryStorageBytes = null;
                try
                {
                    NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                    NativeMethods.StgCreateDocfileOnILockBytes(memoryStorageBytes, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, out memoryStorage);

                    source.CopyTo(0, null, IntPtr.Zero, memoryStorage);
                    memoryStorageBytes.Flush();
                    memoryStorage.Commit(0);

                    ReferenceManager.AddItem(memoryStorage);
                }
                catch
                {
                    if (memoryStorage != null)
                    {
                        Marshal.ReleaseComObject(memoryStorage);
                    }
                }
                finally
                {
                    if (memoryStorageBytes != null)
                    {
                        Marshal.ReleaseComObject(memoryStorageBytes);
                    }

                    if (closeSource)
                    {
                        Marshal.ReleaseComObject(source);
                    }
                }

                return memoryStorage;
            }
        }

        #endregion

        #region ReferenceManager

        private class ReferenceManager
        {
            public static void AddItem(object track)
            {
                lock (instance)
                {
                    if (!instance.trackingObjects.Contains(track))
                    {
                        instance.trackingObjects.Add(track);
                    }
                }
            }

            public static void RemoveItem(object track)
            {
                lock (instance)
                {
                    if (instance.trackingObjects.Contains(track))
                    {
                        instance.trackingObjects.Remove(track);
                    }
                }
            }

            private static ReferenceManager instance = new ReferenceManager();

            private List<object> trackingObjects = new List<object>();

            private ReferenceManager() { }

            ~ReferenceManager()
            {
                foreach (object trackingObject in trackingObjects)
                {
                    Marshal.ReleaseComObject(trackingObject);
                }
            }
        }

        #endregion

        #region Nested Classes

        public enum RecipientType
        {
            To,
            CC,
            Unknown
        }

        public class Recipient : MsgStorage
        {
            #region Property(s)

            public string DisplayName
            {
                get { return this.GetMapiPropertyString(MsgStorage.PR_DISPLAY_NAME); }
            }

            public string Email
            {
                get
                {
                    string email = this.GetMapiPropertyString(MsgStorage.PR_EMAIL);
                    if (String.IsNullOrEmpty(email))
                    {
                        email = this.GetMapiPropertyString(MsgStorage.PR_EMAIL_2);
                    }
                    return email;
                }
            }

            public RecipientType Type
            {
                get
                {
                    int recipientType = this.GetMapiPropertyInt32(MsgStorage.PR_RECIPIENT_TYPE);
                    switch (recipientType)
                    {
                        case MsgStorage.MAPI_TO:
                            return RecipientType.To;

                        case MsgStorage.MAPI_CC:
                            return RecipientType.CC;
                    }
                    return RecipientType.Unknown;
                }
            }

            #endregion

            #region Constructor(s)

            public Recipient(MsgStorage message)
                : base(message.storage)
            {
                GC.SuppressFinalize(message);
                this.propHeaderSize = MsgStorage.PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP;
            }

            #endregion
        }

        public class Attachment : MsgStorage
        {
            #region Property(s)

            public string Filename
            {
                get
                {
                    string filename = this.GetMapiPropertyString(MsgStorage.PR_ATTACH_LONG_FILENAME);
                    if (String.IsNullOrEmpty(filename))
                    {
                        filename = this.GetMapiPropertyString(MsgStorage.PR_ATTACH_FILENAME);
                    }
                    if (String.IsNullOrEmpty(filename))
                    {
                        filename = this.GetMapiPropertyString(MsgStorage.PR_DISPLAY_NAME);
                    }
                    return filename;
                }
            }

            public byte[] Data
            {
                get { return this.GetMapiPropertyBytes(MsgStorage.PR_ATTACH_DATA); }
            }

            public string ContentId
            {
                get { return this.GetMapiPropertyString(MsgStorage.PR_ATTACH_CONTENT_ID); }
            }

            public int RenderingPosisiton
            {
                get { return this.GetMapiPropertyInt32(MsgStorage.PR_RENDERING_POSITION); }
            }

            #endregion

            #region Constructor(s)

            public Attachment(MsgStorage message)
                : base(message.storage)
            {
                GC.SuppressFinalize(message);
                this.propHeaderSize = MsgStorage.PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP;
            }

            #endregion
        }

        public class Message : MsgStorage
        {
            #region Property(s)

            public List<Recipient> Recipients
            {
                get { return this.recipients; }
            }
            private List<Recipient> recipients = new List<Recipient>();

            public List<Attachment> Attachments
            {
                get { return this.attachments; }
            }
            private List<Attachment> attachments = new List<Attachment>();

            public List<Message> Messages
            {
                get { return this.messages; }
            }
            private List<Message> messages = new List<Message>();

            public String From
            {
                get { return this.GetMapiPropertyString(MsgStorage.PR_SENDER_NAME); }
            }

            public String Subject
            {
                get { return this.GetMapiPropertyString(MsgStorage.PR_SUBJECT); }
            }

            public String BodyText
            {
                get { return this.GetMapiPropertyString(MsgStorage.PR_BODY); }
            }

            public String BodyRTF
            {
                get
                {
                    byte[] rtfBytes = this.GetMapiPropertyBytes(MsgStorage.PR_RTF_COMPRESSED);

                    if (rtfBytes == null || rtfBytes.Length == 0)
                    {
                        return null;
                    }

                    rtfBytes = CLZF.decompressRTF(rtfBytes);

                    return Encoding.ASCII.GetString(rtfBytes);
                }
            }

            #endregion

            #region Constructor(s)

            public Message(string msgfile) : base(msgfile) { }

            public Message(Stream storageStream) : base(storageStream) { }

            private Message(NativeMethods.IStorage storage)
                : base(storage)
            {
                this.propHeaderSize = MsgStorage.PROPERTIES_STREAM_HEADER_TOP;
            }

            #endregion

            #region Methods(LoadStorage)

            protected override void LoadStorage(NativeMethods.IStorage storage)
            {
                base.LoadStorage(storage);

                foreach (ComTypes.STATSTG storageStat in this.subStorageStatistics.Values)
                {
                    NativeMethods.IStorage subStorage = this.storage.OpenStorage(storageStat.pwcsName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0);

                    if (storageStat.pwcsName.StartsWith(MsgStorage.RECIP_STORAGE_PREFIX))
                    {
                        Recipient recipient = new Recipient(new MsgStorage(subStorage));
                        this.recipients.Add(recipient);
                    }
                    else if (storageStat.pwcsName.StartsWith(MsgStorage.ATTACH_STORAGE_PREFIX))
                    {
                        this.LoadAttachmentStorage(subStorage);
                    }
                    else
                    {
                        Marshal.ReleaseComObject(subStorage);
                    }
                }
            }

            private void LoadAttachmentStorage(NativeMethods.IStorage storage)
            {
                Attachment attachment = new Attachment(new MsgStorage(storage));

                int attachMethod = attachment.GetMapiPropertyInt32(MsgStorage.PR_ATTACH_METHOD);
                if (attachMethod == MsgStorage.ATTACH_EMBEDDED_MSG)
                {
                    Message subMsg = new Message(attachment.GetMapiProperty(MsgStorage.PR_ATTACH_DATA) as NativeMethods.IStorage);
                    subMsg.parentMessage = this;
                    subMsg.propHeaderSize = MsgStorage.PROPERTIES_STREAM_HEADER_EMBEDED;

                    this.messages.Add(subMsg);
                }
                else
                {
                    this.attachments.Add(attachment);
                }
            }

            #endregion

            #region Methods(Save)

            public void Save(string fileName)
            {
                FileStream saveFileStream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
                this.Save(saveFileStream);
                saveFileStream.Close();
            }

            public void Save(Stream stream)
            {
                MsgStorage saveMsg = this;

                byte[] memoryStorageContent;
                NativeMethods.IStorage memoryStorage = null;
                NativeMethods.IStorage nameIdStorage = null;
                NativeMethods.IStorage nameIdSourceStorage = null;
                NativeMethods.ILockBytes memoryStorageBytes = null;
                try
                {
                    NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                    NativeMethods.StgCreateDocfileOnILockBytes(memoryStorageBytes, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, out memoryStorage);

                    saveMsg.storage.CopyTo(0, null, IntPtr.Zero, memoryStorage);
                    memoryStorageBytes.Flush();
                    memoryStorage.Commit(0);

                    if (!this.IsTopParent)
                    {
                        nameIdStorage = memoryStorage.CreateStorage(MsgStorage.NAMEID_STORAGE, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        nameIdSourceStorage = this.TopParent.storage.OpenStorage(MsgStorage.NAMEID_STORAGE, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0);

                        nameIdSourceStorage.CopyTo(0, null, IntPtr.Zero, nameIdStorage);

                        byte[] props = saveMsg.GetStreamBytes(MsgStorage.PROPERTIES_STREAM);

                        byte[] newProps = new byte[props.Length + 8];

                        Buffer.BlockCopy(props, 0, newProps, 0, 24);
                        Buffer.BlockCopy(props, 24, newProps, 32, props.Length - 24);

                        memoryStorage.DestroyElement(MsgStorage.PROPERTIES_STREAM);

                        ComTypes.IStream propStream = memoryStorage.CreateStream(MsgStorage.PROPERTIES_STREAM, NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        propStream.Write(newProps, newProps.Length, IntPtr.Zero);
                    }

                    memoryStorage.Commit(0);
                    memoryStorageBytes.Flush();

                    ComTypes.STATSTG memoryStorageBytesStat;
                    memoryStorageBytes.Stat(out memoryStorageBytesStat, 1);

                    memoryStorageContent = new byte[memoryStorageBytesStat.cbSize];
                    memoryStorageBytes.ReadAt(0, memoryStorageContent, memoryStorageContent.Length, null);

                    stream.Write(memoryStorageContent, 0, memoryStorageContent.Length);
                }
                finally
                {
                    if (nameIdSourceStorage != null)
                    {
                        Marshal.ReleaseComObject(nameIdSourceStorage);
                    }

                    if (memoryStorage != null)
                    {
                        Marshal.ReleaseComObject(memoryStorage);
                    }

                    if (memoryStorageBytes != null)
                    {
                        Marshal.ReleaseComObject(memoryStorageBytes);
                    }
                }
            }

            #endregion

            #region Methods(Disposing)

            protected override void Disposing()
            {
                foreach (MsgStorage subMsg in this.messages)
                {
                    subMsg.Dispose();
                }

                foreach (MsgStorage recip in this.recipients)
                {
                    recip.Dispose();
                }

                foreach (MsgStorage attach in this.attachments)
                {
                    attach.Dispose();
                }
            }

            #endregion
        }

        #endregion

        #region Constants

        private const string ATTACH_STORAGE_PREFIX = "__attach_version1.0_#";
        private const string PR_ATTACH_FILENAME = "3704";
        private const string PR_ATTACH_LONG_FILENAME = "3707";
        private const string PR_ATTACH_DATA = "3701";
        private const string PR_ATTACH_METHOD = "3705";
        private const string PR_RENDERING_POSITION = "370B";
        private const string PR_ATTACH_CONTENT_ID = "3712";
        private const int ATTACH_BY_VALUE = 1;
        private const int ATTACH_EMBEDDED_MSG = 5;

        private const string RECIP_STORAGE_PREFIX = "__recip_version1.0_#";
        private const string PR_DISPLAY_NAME = "3001";
        private const string PR_EMAIL = "39FE";
        private const string PR_EMAIL_2 = "403E";
        private const string PR_RECIPIENT_TYPE = "0C15";
        private const int MAPI_TO = 1;
        private const int MAPI_CC = 2;

        private const string PR_SUBJECT = "0037";
        private const string PR_BODY = "1000";
        private const string PR_RTF_COMPRESSED = "1009";
        private const string PR_SENDER_NAME = "0C1A";

        private const string PROPERTIES_STREAM = "__properties_version1.0";
        private const int PROPERTIES_STREAM_HEADER_TOP = 32;
        private const int PROPERTIES_STREAM_HEADER_EMBEDED = 24;
        private const int PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP = 8;

        private const string NAMEID_STORAGE = "__nameid_version1.0";

        private const string PR_RECEIVED_DATE = "007D";
        private const string PR_RECEIVED_DATE_2 = "0047";


        #endregion

        #region Property(s)

        private MsgStorage TopParent
        {
            get
            {
                if (this.parentMessage != null)
                {
                    return this.parentMessage.TopParent;
                }
                return this;
            }
        }

        private bool IsTopParent
        {
            get
            {
                if (this.parentMessage != null)
                {
                    return false;
                }
                return true;
            }
        }

        private NativeMethods.IStorage storage;

        private int propHeaderSize = MsgStorage.PROPERTIES_STREAM_HEADER_TOP;

        private MsgStorage parentMessage = null;

        public Dictionary<string, ComTypes.STATSTG> streamStatistics = new Dictionary<string, ComTypes.STATSTG>();

        public Dictionary<string, ComTypes.STATSTG> subStorageStatistics = new Dictionary<string, ComTypes.STATSTG>();

        private bool disposed = false;

        #endregion

        #region Constructor(s)

        private MsgStorage(string storageFilePath)
        {
            if (NativeMethods.StgIsStorageFile(storageFilePath) != 0)
            {
                throw new ArgumentException("The provided file is not a valid IStorage", "storageFilePath");
            }

            NativeMethods.IStorage fileStorage;
            NativeMethods.StgOpenStorage(storageFilePath, null, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_DENY_WRITE, IntPtr.Zero, 0, out fileStorage);
            this.LoadStorage(fileStorage);
        }

        private MsgStorage(Stream storageStream)
        {
            NativeMethods.IStorage memoryStorage = null;
            NativeMethods.ILockBytes memoryStorageBytes = null;
            try
            {
                byte[] buffer = new byte[storageStream.Length];
                storageStream.Read(buffer, 0, buffer.Length);

                NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                memoryStorageBytes.WriteAt(0, buffer, buffer.Length, null);

                if (NativeMethods.StgIsStorageILockBytes(memoryStorageBytes) != 0)
                {
                    throw new ArgumentException("The provided stream is not a valid IStorage", "storageStream");
                }

                NativeMethods.StgOpenStorageOnILockBytes(memoryStorageBytes, null, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_DENY_WRITE, IntPtr.Zero, 0, out memoryStorage);
                this.LoadStorage(memoryStorage);
            }
            catch
            {
                if (memoryStorage != null)
                {
                    Marshal.ReleaseComObject(memoryStorage);
                }
            }
            finally
            {
                if (memoryStorageBytes != null)
                {
                    Marshal.ReleaseComObject(memoryStorageBytes);
                }
            }
        }

        private MsgStorage(NativeMethods.IStorage storage)
        {
            this.LoadStorage(storage);
        }

        ~MsgStorage()
        {
            this.Dispose();
        }

        #endregion

        #region Methods(LoadStorage)
        

        protected virtual void LoadStorage(NativeMethods.IStorage storage)
        {
            this.storage = storage;

            ReferenceManager.AddItem(this.storage);

            NativeMethods.IEnumSTATSTG storageElementEnum = null;
            try
            {
                storage.EnumElements(0, IntPtr.Zero, 0, out storageElementEnum);

                while (true)
                {
                    uint elementStatCount;
                    ComTypes.STATSTG[] elementStats = new ComTypes.STATSTG[1];
                    storageElementEnum.Next(1, elementStats, out elementStatCount);

                    if (elementStatCount != 1)
                    {
                        break;
                    }

                    ComTypes.STATSTG elementStat = elementStats[0];
                    switch (elementStat.type)
                    {
                        case 1:
                            subStorageStatistics.Add(elementStat.pwcsName, elementStat);
                            break;

                        case 2:
                            streamStatistics.Add(elementStat.pwcsName, elementStat);
                            break;
                    }
                }
            }
            finally
            {
                if (storageElementEnum != null)
                {
                    Marshal.ReleaseComObject(storageElementEnum);
                }
            }
        }

        #endregion

        #region Methods(GetStreamBytes, GetStreamAsString)

        public byte[] GetStreamBytes(string streamName)
        {
            ComTypes.STATSTG streamStatStg = this.streamStatistics[streamName];

            byte[] iStreamContent;
            ComTypes.IStream stream = null;
            try
            {
                stream = this.storage.OpenStream(streamStatStg.pwcsName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, 0);

                iStreamContent = new byte[streamStatStg.cbSize];
                stream.Read(iStreamContent, iStreamContent.Length, IntPtr.Zero);
            }
            finally
            {
                if (stream != null)
                {
                    Marshal.ReleaseComObject(stream);
                }
            }

            return iStreamContent;
        }

        public string GetStreamAsString(string streamName, Encoding streamEncoding)
        {
            StreamReader streamReader = new StreamReader(new MemoryStream(this.GetStreamBytes(streamName)), streamEncoding);
            string streamContent = streamReader.ReadToEnd();
            streamReader.Close();

            return streamContent;
        }

        #endregion

        #region Methods(GetMapiProperty)



        /// <summary>
        /// Gets the date the message was received.
        /// </summary>
        public DateTime ReceivedDate
        {
            get
            {
                if (_dateRevieved == DateTime.MinValue)
                {
                    string dateMess = this.GetMapiPropertyString(MsgStorage.PR_RECEIVED_DATE);
                    if (String.IsNullOrEmpty(dateMess))
                    {
                        dateMess = this.GetMapiPropertyString(MsgStorage.PR_RECEIVED_DATE_2);
                    }
                    _dateRevieved = ExtractDate(dateMess);
                }
                return _dateRevieved;
                //return ExtractDate(dateMess);
            }
        }

        private DateTime _dateRevieved = DateTime.MinValue;

        private DateTime ExtractDate(string dateMess)
        {
            string matchStr = "Date:";

            string[] lines = dateMess.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith(matchStr))
                {
                    string dateStr = line.Substring(matchStr.Length);
                    DateTime response;
                    if (DateTime.TryParse(dateStr, out response))
                    {
                        return response;
                    }
                }
            }
            return DateTime.MinValue;
        }


        public object GetMapiProperty(string propIdentifier)
        {
            object propValue = this.GetMapiPropertyFromStreamOrStorage(propIdentifier);

            if (propValue == null)
            {
                propValue = this.GetMapiPropertyFromPropertyStream(propIdentifier);
            }

            return propValue;
        }

        private object GetMapiPropertyFromStreamOrStorage(string propIdentifier)
        {
            List<string> propKeys = new List<string>();
            propKeys.AddRange(this.streamStatistics.Keys);
            propKeys.AddRange(this.subStorageStatistics.Keys);

            string propTag = null;
            ushort propType = NativeMethods.PT_UNSPECIFIED;
            foreach (string propKey in propKeys)
            {
                if (propKey.StartsWith("__substg1.0_" + propIdentifier))
                {
                    propTag = propKey.Substring(12, 8);
                    propType = ushort.Parse(propKey.Substring(16, 4), System.Globalization.NumberStyles.HexNumber);
                    break;
                }
            }

            string containerName = "__substg1.0_" + propTag;
            switch (propType)
            {
                case NativeMethods.PT_UNSPECIFIED:
                    return null;

                case NativeMethods.PT_STRING8:
                    return this.GetStreamAsString(containerName, Encoding.UTF8);

                case NativeMethods.PT_UNICODE:
                    return this.GetStreamAsString(containerName, Encoding.Unicode);

                case NativeMethods.PT_BINARY:
                    return this.GetStreamBytes(containerName);

                case NativeMethods.PT_OBJECT:
                    return NativeMethods.CloneStorage(this.storage.OpenStorage(containerName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0), true);

                default:
                    throw new ApplicationException("MAPI property has an unsupported type and can not be retrieved.");
            }
        }

        private object GetMapiPropertyFromPropertyStream(string propIdentifier)
        {
            if (!this.streamStatistics.ContainsKey(MsgStorage.PROPERTIES_STREAM))
            {
                return null;
            }

            byte[] propBytes = this.GetStreamBytes(MsgStorage.PROPERTIES_STREAM);

            for (int i = this.propHeaderSize; i < propBytes.Length; i = i + 16)
            {
                ushort propType = BitConverter.ToUInt16(propBytes, i);

                byte[] propIdent = new byte[] { propBytes[i + 3], propBytes[i + 2] };
                string propIdentString = BitConverter.ToString(propIdent).Replace("-", "");

                if (propIdentString != propIdentifier)
                {
                    continue;
                }

                switch (propType)
                {
                    case NativeMethods.PT_I2:
                        return BitConverter.ToInt16(propBytes, i + 8);

                    case NativeMethods.PT_LONG:
                        return BitConverter.ToInt32(propBytes, i + 8);

                    default:
                        throw new ApplicationException("MAPI property has an unsupported type and can not be retrieved.");
                }
            }

            return null;
        }

        public string GetMapiPropertyString(string propIdentifier)
        {
            return this.GetMapiProperty(propIdentifier) as string;
        }

        public Int16 GetMapiPropertyInt16(string propIdentifier)
        {
            return (Int16)this.GetMapiProperty(propIdentifier);
        }

        public int GetMapiPropertyInt32(string propIdentifier)
        {
            return (int)this.GetMapiProperty(propIdentifier);
        }

        public byte[] GetMapiPropertyBytes(string propIdentifier)
        {
            return (byte[])this.GetMapiProperty(propIdentifier);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (!this.disposed)
                {
                    this.disposed = true;

                    this.Disposing();

                    ReferenceManager.RemoveItem(this.storage);
                    Marshal.ReleaseComObject(this.storage);
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception eee)
            {

                //throw;
            }
        }

        protected virtual void Disposing() { }

        #endregion

    }

}