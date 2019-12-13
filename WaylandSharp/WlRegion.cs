using System;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlRegion : IWlRegion {
		public WlRegion(Client owner) : base(owner, null) { }
		
		public override void Destroy() => Helper.Log("Destroying region");
		public override void Add(int x, int y, int width, int height) => Helper.Log($"Adding region {x}x{y} {width}x{height}");
		public override void Subtract(int x, int y, int width, int height) => throw new System.NotImplementedException();
	}
}