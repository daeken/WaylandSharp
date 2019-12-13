using System;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlShm : IWlShm {
		public WlShm(Client owner, uint? id = null) : base(owner, id) { }

		internal override void Setup() {
			Format(Enum.Format.Argb8888);
			Format(Enum.Format.Xrgb8888);
		}

		public override IWlShmPool CreatePool(int fd, int size) => new WlShmPool(Owner, fd, size);
	}

	public class WlShmPool : IWlShmPool {
		readonly byte[] Underlying;
		int Size;
		public WlShmPool(Client owner, int fd, int size) : base(owner, null) {
			Underlying = Owner.Files[fd];
			Size = size;
		}

		public override IWlBuffer CreateBuffer(int offset, int width, int height, int stride,
			IWlShm.Enum.Format format) =>
			new WlBuffer(Owner, Underlying, offset, width, height, stride, format);

		public override void Destroy() => Helper.Log("Destroying SHM pool");

		public override void Resize(int size) {
			Helper.Log($"Attempting shm pool resize to {size}");
			Size = size;
		}
	}
}