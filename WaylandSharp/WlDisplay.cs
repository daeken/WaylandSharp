using System;
using System.Threading.Tasks;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlDisplay : IWlDisplay {
		public WlDisplay(Client owner, uint? id = null) : base(owner, id) { }

		public override void Sync(out IWlCallback callback) =>
			callback = new WlCallback(Owner, cb => cb.Done(Owner.Serial));

		public override void GetRegistry(out IWlRegistry registry) =>
			registry = Owner.Registry;
	}
}