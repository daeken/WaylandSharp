using System;
using System.Linq;
using System.Net.Sockets;

namespace WaylandSharp {
	internal static class Extensions {
		const string Printable = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-[]{}`~!@#$%^&*()-=\\|;:'\",./<>?";
		internal static void Hexdump(this byte[] buffer) {
			for(var i = 0; i < buffer.Length; i += 16) {
				var ret = $"{i:X4} | ";
				for(var j = 0; j < 16; ++j) {
					ret += i + j >= buffer.Length ? $"   " : $"{buffer[i + j]:X2} ";
					if(j == 7) ret += " ";
				}
				ret += "| ";
				for(var j = 0; j < 16; ++j) {
					if(i + j >= buffer.Length) break;
					ret += Printable.Contains((char) buffer[i + j]) ? new string((char) buffer[i + j], 1) : ".";
					if(j == 7) ret += " ";
				}
				Helper.Log(ret);
			}
			Helper.Log($"{buffer.Length:X4}");
		}

		internal static void ReceiveAll(this Socket socket, byte[] buf) {
			var tlen = buf.Length;
			var offset = 0;
			while(tlen > 0) {
				var rlen = socket.Receive(buf, offset, tlen, SocketFlags.None);
				if(rlen < 0) throw new Exception();
				offset += rlen;
				tlen -= rlen;
			}
		}

		internal static void SendAll(this Socket socket, byte[] buf) {
			var tlen = buf.Length;
			var offset = 0;
			while(tlen > 0) {
				var rlen = socket.Send(buf, offset, tlen, SocketFlags.None);
				if(rlen < 0) throw new Exception();
				offset += rlen;
				tlen -= rlen;
			}
		}
	}
}