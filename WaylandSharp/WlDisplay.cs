using System;
using System.Threading.Tasks;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlDisplay : IWlDisplay {
		public WlDisplay(Client owner, uint? id = null) : base(owner, id) { }
		public override void Sync(out IWlCallback callback) => throw new System.NotImplementedException();

		public override void GetRegistry(out IWlRegistry registry) =>
			registry = new WlRegistry(Owner);
	}
}