using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlSubcompositor : IWlSubcompositor {
		public WlSubcompositor(Client owner, uint? id = null) : base(owner, id) { }
		
		public override void Destroy() => throw new System.NotImplementedException();
		public override IWlSubsurface GetSubsurface(IWlSurface surface, IWlSurface parent) =>
			throw new System.NotImplementedException();
	}
}