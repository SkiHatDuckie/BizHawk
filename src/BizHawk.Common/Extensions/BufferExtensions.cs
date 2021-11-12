﻿using System;
using System.IO;
using System.Text;

namespace BizHawk.Common.BufferExtensions
{
	public static class BufferExtensions
	{
		private static readonly char[] HexConvArr = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

		public static unsafe void SaveAsHexFast(this byte[] buffer, TextWriter writer)
		{
			fixed (char* table = HexConvArr)
			{
				if (buffer.Length > 0)
				{
					int len = buffer.Length;
					fixed (byte* src = buffer)
						for (int i = 0; i < len; i++)
						{
							writer.Write(table[src[i] >> 4]);
							writer.Write(table[src[i] & 15]);
						}
				}
			}

			writer.WriteLine();
		}

		/// <exception cref="Exception"><paramref name="buffer"/> can't hold the same number of bytes as <paramref name="hex"/></exception>
		public static unsafe void ReadFromHexFast(this byte[] buffer, string hex)
		{
			if (buffer.Length * 2 != hex.Length)
			{
				throw new Exception("Data size mismatch");
			}

			int count = buffer.Length;
			fixed (byte* _dst = buffer)
			fixed (char* _src = hex)
			{
				byte* dst = _dst;
				char* src = _src;
				while (count > 0)
				{
					// in my tests, replacing Hex2Int() with a 256 entry LUT slowed things down slightly
					*dst++ = (byte)(Hex2Int(*src++) << 4 | Hex2Int(*src++));
					count--;
				}
			}
		}

		/// <summary>
		/// Converts bytes to an uppercase string of hex numbers in upper case without any spacing or anything
		/// </summary>
		public static string BytesToHexString(this byte[] bytes)
		{
			var sb = new StringBuilder();
			foreach (var b in bytes)
			{
				sb.AppendFormat("{0:X2}", b);
			}

			return sb.ToString();
		}

		public static bool FindBytes(this byte[] array, byte[] pattern)
		{
			var fidx = 0;
			int result = Array.FindIndex(array, 0, array.Length, (byte b) =>
			{
				fidx = b == pattern[fidx] ? fidx + 1 : 0;
				return fidx == pattern.Length;
			});

			return result >= pattern.Length - 1;
		}

		private static int Hex2Int(char c)
		{
			if (c <= '9')
			{
				return c - '0';
			}

			if (c <= 'F')
			{
				return c - '7';
			}

			return c - 'W';
		}
	}
}
