using System;
using System.IO;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlBuffer : IWlBuffer {
		public readonly byte[] Buffer;
		public readonly int Offset, Width, Height, Stride;
		public readonly IWlShm.Enum.Format Format;
		public WlBuffer(Client owner, byte[] buffer, int offset, int width, int height, int stride,
			IWlShm.Enum.Format format) : base(owner, null) {
			Buffer = buffer;
			Offset = offset;
			Width = width;
			Height = height;
			Stride = stride;
			Format = format;
		}

		public override void Destroy() => Helper.Log("Destroying WlBuffer");
	}
}