using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WaylandSharp {
	public abstract class IWaylandObject {
		public readonly Client Owner;
		public uint Id;
		internal abstract void ProcessRequest(int opcode, int mlen);
		public abstract string InterfaceName { get; }
		public abstract int InterfaceVersion { get; }

		public IWaylandObject(Client owner, uint? id = null) {
			Owner = owner;
			Id = id ?? 0xFFFFFFFFU;
		}
		
		internal virtual void Setup() {}
		
		internal void SendEvent(int opcode, byte[] buf) {
			Debug.Assert((buf.Length & 3) == 0);
			var tbuf = new byte[buf.Length + 8];
			Array.Copy(BitConverter.GetBytes(Id), 0, tbuf, 0, 4);
			Array.Copy(BitConverter.GetBytes(((uint) tbuf.Length << 16) | (uint) opcode), 0, tbuf, 4, 4);
			Array.Copy(buf, 0, tbuf, 8, buf.Length);
			Console.WriteLine($"Writing buffer for event opcode {opcode} on object 0x{Id:X}");
			tbuf.Hexdump();
			Console.ReadLine();
			Owner.Socket.Write(tbuf);
		}

		internal void SendEventWithFd(int opcode, byte[] buf, int fd) {
			throw new NotImplementedException();
		}
	}
}