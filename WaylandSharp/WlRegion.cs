using System;
using System.Collections.Generic;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class Rectangle {
		public int X, Y, Width, Height;

		public override string ToString() => $"[{X}x{Y} {Width}x{Height}]";
	}
	
	public class WlRegion : IWlRegion {
		public Rectangle Region;
		
		public WlRegion(Client owner) : base(owner, null) { }
		
		public override void Destroy() => Owner.Destroy(this);
		public override void Add(int x, int y, int width, int height) {
			if(Region == null)
				Region = new Rectangle { X = x, Y = y, Width = width, Height = height };
			else {
				var mx = Math.Min(x, Region.X);
				var my = Math.Min(y, Region.Y);
				Region = new Rectangle {
					X = mx, Y = my,
					Width = Math.Max(x + width, Region.X + Region.Width) - mx, 
					Height = Math.Max(y + height, Region.Y + Region.Height) - my
				};
			}
		}

		public override void Subtract(int x, int y, int width, int height) => throw new System.NotImplementedException();
	}
}