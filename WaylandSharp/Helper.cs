using System;
using System.Diagnostics;
using System.Text;

namespace WaylandSharp {
	internal static class Helper {
		internal static byte[] ReadArray(byte[] buf, ref int offset) {
			var size = BitConverter.ToUInt32(buf, offset);
			offset += 4;
			var ret = new byte[size];
			Array.Copy(buf, offset, ret, 0, size);
			offset += (int) size;
			while((offset & 3) != 0)
				offset++;
			return ret;
		}

		internal static string ReadString(byte[] buf, ref int offset) {
			var sbuf = ReadArray(buf, ref offset);
			AssertEqual((uint) sbuf[sbuf.Length - 1], 0U);
			return Encoding.UTF8.GetString(sbuf, 0, sbuf.Length - 1);
		}

		internal static uint ReadUint(byte[] buf, ref int offset) {
			var val = BitConverter.ToUInt32(buf, offset);
			offset += 4;
			return val;
		}

		internal static int ReadInt(byte[] buf, ref int offset) {
			var val = BitConverter.ToInt32(buf, offset);
			offset += 4;
			return val;
		}

		internal static int StringSize(string data) {
			var len = 4 + Encoding.UTF8.GetBytes(data).Length + 1;
			while((len & 3) != 0)
				len++;
			return len;
		}

		internal static int ArraySize(byte[] data) {
			var len = 4 + data.Length;
			while((len & 3) != 0)
				len++;
			return len;
		}

		internal static void WriteUint(byte[] buf, ref int offset, uint val) {
			Array.Copy(BitConverter.GetBytes(val), 0, buf, offset, 4);
			offset += 4;
		}

		internal static void WriteInt(byte[] buf, ref int offset, int val) {
			Array.Copy(BitConverter.GetBytes(val), 0, buf, offset, 4);
			offset += 4;
		}

		internal static void WriteString(byte[] buf, ref int offset, string val) {
			var data = Encoding.UTF8.GetBytes(val + '\0');
			Array.Copy(BitConverter.GetBytes(data.Length), 0, buf, offset, 4);
			offset += 4;
			Array.Copy(data, 0, buf, offset, data.Length);
			offset += data.Length;
			while((offset & 3) != 0)
				offset++;
		}

		internal static void WriteArray(byte[] buf, ref int offset, byte[] data) {
			Array.Copy(BitConverter.GetBytes(data.Length), 0, buf, offset, 4);
			offset += 4;
			Array.Copy(data, 0, buf, offset, data.Length);
			offset += data.Length;
			while((offset & 3) != 0)
				offset++;
		}

		internal static void Assert(bool expr) {
			if(!expr)
				throw new Exception("Assertion failed!");
		}

		internal static void AssertEqual(int left, int right) {
			if(left != right)
				throw new Exception($"Assertion failed: {left} != {right}");
		}
		
		internal static void AssertEqual(uint left, uint right) {
			if(left != right)
				throw new Exception($"Assertion failed: 0x{left:X} != 0x{right:X}");
		}

		internal static void Log<T>(T log) => DisplayServer.Instance.Log(log.ToString());

		internal static void Error<T>(T log) => DisplayServer.Instance.Error(log.ToString());
	}
}