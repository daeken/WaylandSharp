using System;
using System.IO;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlBuffer : IWlBuffer {
		public WlBuffer(Client owner, byte[] buffer, int offset, int width, int height, int stride,
			IWlShm.Enum.Format format) : base(owner, null) {
			
		}

		public override void Destroy() => Helper.Log("Destroying WlBuffer");
	}
}