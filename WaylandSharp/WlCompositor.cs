using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlCompositor : IWlCompositor {
		public WlCompositor(Client owner, uint? id = null) : base(owner, id) { }
		
		public override IWlSurface CreateSurface() => new WlSurface(Owner);
		public override IWlRegion CreateRegion() => new WlRegion(Owner);
	}
}