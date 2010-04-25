using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace Sokoban.Lib
{
    public class CommonFunc
    {
		#region Methods (10) 

		// Public Methods (10) 

        /// <summary>
        /// Reads Int32 from array of bytes starting at byte @offset
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns>Int32</returns>
        public static int Block2Int32(byte[] data, ref int offset)
        {
            offset += 4;
            return BitConverter.ToInt32(data, offset - 4);
        }

        /// <summary>
        /// Reads Int32 from array of bytes starting at byte @offset
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns>Int64</returns>
        public static Int64 Block2Int64(byte[] data, ref int offset)
        {
            offset += 8;
            return BitConverter.ToInt64(data, offset - 8);
        }

        /// <summary>
        /// Copy @data in form of bytes in @message (first byte is stored at position @offset)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public static void BlockCopyEx(byte[] message, int data, ref int offset)
        {
            int tmpOffset = BitConverter.GetBytes(data).Length;

            if (offset + tmpOffset > message.Length) throw new Exception("BlockCopyEx: out of range exception");

            Buffer.BlockCopy(BitConverter.GetBytes(data), 0, message, offset, tmpOffset);
            offset += tmpOffset;
        }

        /// <summary>
        /// Copy @data in form of bytes in @message (first byte is stored at position @offset)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public static void BlockCopyEx(byte[] message, Int64 data, ref int offset)
        {
            int tmpOffset = BitConverter.GetBytes(data).Length;

            if (offset + tmpOffset > message.Length) throw new Exception("BlockCopyEx: out of range exception");

            Buffer.BlockCopy(BitConverter.GetBytes(data), 0, message, offset, tmpOffset);
            offset += tmpOffset;
        }

		#endregion Methods 
    }
}
