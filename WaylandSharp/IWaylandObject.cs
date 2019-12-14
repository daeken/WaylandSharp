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
			Array.Copy(BitConverter.GetBytes((uint) tbuf.Length << 5), 0, tbuf, 0, 4);
			Array.Copy(BitConverter.GetBytes(Id), 0, tbuf, 4, 4);
			Array.Copy(BitConverter.GetBytes(((uint) (tbuf.Length - 4) << 16) | (uint) opcode), 0, tbuf, 8, 4);
			Array.Copy(buf, 0, tbuf, 12, buf.Length);
			Owner.Socket.SendAll(tbuf);
		}

		internal void SendEventWithFd(int opcode, byte[] buf, byte[] fd_data) {
			var ofbuf = new byte[12];
			var fd = Owner.ServerFd++;
			Array.Copy(BitConverter.GetBytes(((uint) ofbuf.Length << 5) | (uint) Client.WmsgType.OpenFile), 0, ofbuf, 0, 4);
			Array.Copy(BitConverter.GetBytes(fd), 0, ofbuf, 4, 4);
			Array.Copy(BitConverter.GetBytes(fd_data.Length), 0, ofbuf, 8, 4);
			Owner.Socket.SendAll(ofbuf);
			var pad = (fd_data.Length & 3) == 0 ? 0 : 4 - (fd_data.Length & 3);
			ofbuf = new byte[16 + fd_data.Length + pad];
			Array.Copy(BitConverter.GetBytes(((uint) (ofbuf.Length - pad) << 5) | (uint) Client.WmsgType.BufferFill), 0, ofbuf, 0, 4);
			Array.Copy(BitConverter.GetBytes(fd), 0, ofbuf, 4, 4);
			Array.Copy(BitConverter.GetBytes(0), 0, ofbuf, 8, 4);
			Array.Copy(BitConverter.GetBytes(fd_data.Length), 0, ofbuf, 12, 4);
			Array.Copy(fd_data, 0, ofbuf, 16, fd_data.Length);
			Owner.Socket.SendAll(ofbuf);
			ofbuf = new byte[8];
			Array.Copy(BitConverter.GetBytes(((uint) ofbuf.Length << 5) | (uint) Client.WmsgType.InjectRids), 0, ofbuf, 0, 4);
			Array.Copy(BitConverter.GetBytes(fd), 0, ofbuf, 4, 4);
			Owner.Socket.SendAll(ofbuf);
			SendEvent(opcode, buf);
		}
	}
}