﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MsgPack
{
	/// <summary>
	/// A byte string that can be used to interop with native code.
	/// It does this by using byte sized characters and guaranteeing that it's null terminated, equivalent to a (non-wide) c-string
	/// </summary>
	public class CString
	{
		internal readonly byte[] value;

		public unsafe CString(string str)
		{
			fixed (char* c = str)
			{
				if (str != null)
				{
					int strLength = str.Length;
					int byteCount = CString.UTF8EncodeLength(c, strLength);
					byte[] bytes = new byte[byteCount + 1]; // zero initializes all, so last byte will be and stay 0x0

					fixed (byte* b = bytes)
						CString.UTF8Encode(b, c, strLength);

					value = bytes;
				}
				else
					value = null;
			}
		}

		public static explicit operator CString(string str) => str != null ? new CString(str) : null;

		public static bool IsNull(CString str) => !(str?.value != null);
		public static bool IsNullOrEmpty(CString str) => !(str?.value?.Length != 1);

		public static unsafe int UTF8EncodeLength(char* src, int length)
		{
			char* c = src, end = src + length;
			length = 0;
			while (c < end)
			{
				char c1 = *c++;
				if (c1 <= 0x7F)
					length++;
				else if (c1 <= 0x7FF)
					length += 2;
				else
				{
					char c2 = *c++;
					if (c1 <= 0xFFFF)
						length += 3;
					else if (c2 <= 0x10FF)
						length += 4;
					else
						return length; // error
				}
			}

			return length;
		}

		public static unsafe int UTF8Encode(byte* dst, char* src, int length)
		{
			byte* s = dst;
			char* c = src, end = src + length;
			while (c < end)
			{
				char c1 = *c++;
				if (c1 <= 0x7F)
				{
					*s = (byte)c1;
					s++;
				}
				else if (c1 <= 0x7FF)
				{
					s[0] = (byte)(0xC0 | (c1 >> 6));            /* 110xxxxx */
					s[1] = (byte)(0x80 | (c1 & 0x3F));          /* 10xxxxxx */
					s += 2;
				}
				else
				{
					char c2 = *c++;
					if (c1 <= 0xFFFF)
					{
						s[0] = (byte)(0xE0 | (c2 >> 4 /*12-8*/));  /* 1110xxxx */
						s[1] = (byte)(0x80 | ((c1 >> 6) & 0x3F));   /* 10xxxxxx */
						s[2] = (byte)(0x80 | (c1 & 0x3F));          /* 10xxxxxx */
						s += 3;
					}
					else if (c2 <= 0x10FF)
					{
						s[0] = (byte)(0xF0 | (c2 >> 10));          /* 11110xxx */
						s[1] = (byte)(0x80 | ((c2 >> 4) & 0x3F));  /* 10xxxxxx */
						s[2] = (byte)(0x80 | ((c1 >> 6) & 0x3F));   /* 10xxxxxx */
						s[3] = (byte)(0x80 | (c1 & 0x3F));          /* 10xxxxxx */
						s += 4;
					}
					else
						return (int)(s - dst); // error
				}
			}

			return (int)(s - dst);
		}
	}
}
