using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WaylandSharp {
	public abstract class IWaylandObject {
		public readonly Client Owner;
		public uint Id;
		internal abstract void ProcessRequest(int opcode, byte[] tbuf);
		public abstract string InterfaceName { get; }
		public abstract int InterfaceVersion { get; }

		public IWaylandObject(Client owner, uint? id = null) {
			Owner = owner;
			Id = id ?? 0xFFFFFFFFU;
		}
		
		internal virtual void Setup() {}
		
		internal void SendEvent(int opcode, byte[] buf) {
			Helper.AssertEqual(buf.Length & 3, 0);
			var tbuf = new byte[buf.Length + 12];
			Helper.Log("Attempting to construct buffer...");
			Array.Copy(BitConverter.GetBytes((uint) tbuf.Length << 5), 0, tbuf, 0, 4);
			Array.Copy(BitConverter.GetBytes(Id), 0, tbuf, 4, 4);
			Array.Copy(BitConverter.GetBytes(((uint) (tbuf.Length - 4) << 16) | (uint) opcode), 0, tbuf, 8, 4);
			Array.Copy(buf, 0, tbuf, 12, buf.Length);
			Helper.Log($"Writing buffer for event opcode {opcode} on object 0x{Id:X}");
			tbuf.Hexdump();
			Owner.Socket.Send(tbuf);
		}

		internal void SendEventWithFd(int opcode, byte[] buf, int fd) {
			throw new NotImplementedException();
		}
	}
}