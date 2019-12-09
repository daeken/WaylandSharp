using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlRegistry : IWlRegistry {
		public WlRegistry(Client owner, uint? id = null) : base(owner, id) { }

		internal override void Setup() {
			Global(1, "wl_display", 1);
			Global(2, "wl_registry", 1);
			Global(3, "wl_output", 3);
			Global(4, "wl_output", 3);
		}

		public override void Bind(uint name, out IWaylandObject id) {
			Helper.Bailout($"WlRegistry::Bind({name})!");
			throw new System.NotImplementedException();
		}
	}
}