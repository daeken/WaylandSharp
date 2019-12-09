using System;
using System.Net.Sockets;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;

namespace WaylandSharp {
	public class WlSocket {
		readonly Socket Socket;
		internal WlSocket(string path) {
			Socket = new Socket(AddressFamily.Unix, SocketType.Stream, 0);
			Socket.Bind(new UnixEndPoint(path));
			Socket.Listen(10);
		}
		internal WlSocket(Socket socket) => Socket = socket; 

		public void Start() =>
			new Thread(() => {
				while(true) {
					var csock = Socket.Accept();
					var client = new Client(new WlSocket(csock));
					DisplayServer.Instance.AddClient(client);
					client.Start();
				}
			}).Start();

		public unsafe int ReadWithFd(byte[] buffer, int offset = 0, int? length = null) {
			var alen = length ?? buffer.Length - offset;
			fixed(byte* buf = buffer) {
				var cmsg = new byte[Syscall.CMSG_SPACE(4)];
				var msg = new Msghdr {
					msg_iov = new[] { new Iovec { iov_base = (IntPtr) (buf + offset), iov_len = (ulong) alen } }, 
					msg_name = null, 
					msg_iovlen = 1, 
					msg_control = cmsg, 
					msg_controllen = cmsg.Length
				};
				var size = Syscall.recvmsg((int) Socket.Handle, msg, 0);
				if(size < 0) throw new Exception();
				var fd = BitConverter.ToInt32(cmsg, (int) Syscall.CMSG_DATA(msg, Syscall.CMSG_FIRSTHDR(msg)));
				offset += (int) size;
				alen -= (int) size;
				while(alen > 0) {
					size = Syscall.read((int) Socket.Handle, buf + offset, (ulong) alen);
					if(size < 0) throw new Exception();
					offset += (int) size;
					alen -= (int) size;
				}
				return fd;
			}
		}

		public unsafe void Read(byte[] buffer, int offset = 0, int? length = null) {
			var alen = length ?? buffer.Length - offset;
			fixed(byte* buf = buffer)
				while(alen > 0) {
					var size = Syscall.read((int) Socket.Handle, buf + offset, (ulong) alen);
					if(size < 0) throw new Exception();
					offset += (int) size;
					alen -= (int) size;
				}
		}

		public unsafe void WriteWithFd(byte[] buffer, int fd) {
			throw new NotImplementedException();
		}

		public unsafe void Write(byte[] buffer) {
			var offset = 0;
			var len = buffer.Length;
			fixed(byte* buf = buffer)
				while(len > 0) {
					var size = Syscall.write((int) Socket.Handle, buf + offset, (ulong) len);
					if(size < 0) throw new Exception();
					len -= (int) size;
					offset += (int) size;
				}
		}
	}
}