using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlOutput : IWlOutput {
		public WlOutput(Client owner, uint? id = null) : base(owner, id) { }

		internal override void Setup() {
			Geometry(640, 480, 6400, 4800, Enum.Subpixel.HorizontalRgb, "Daeken", "Fake-ass Output 9001",
				Enum.Transform.Normal);
		}

		public override void Release() {
			throw new System.NotImplementedException();
		}
	}
}