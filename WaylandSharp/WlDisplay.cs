using System;
using System.Threading.Tasks;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlDisplay : IWlDisplay {
		public WlDisplay(Client owner, uint? id = null) : base(owner, id) { }

		public override IWlCallback Sync() => new WlCallback(Owner, cb => cb.Done(Owner.Serial));
		public override IWlRegistry GetRegistry() => Owner.Registry;
	}
}