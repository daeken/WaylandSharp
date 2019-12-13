using System;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class ZxdgShellV6 : IZxdgShellV6 {
		public ZxdgShellV6(Client owner, uint? id = null) : base(owner, id) { }
		
		public override void Destroy() => throw new System.NotImplementedException();
		public override IZxdgPositionerV6 CreatePositioner() => throw new NotImplementedException();

		public override IZxdgSurfaceV6 GetXdgSurface(IWlSurface surface) => new ZxdgSurfaceV6(surface as WlSurface);
		
		public override void Pong(uint serial) => throw new System.NotImplementedException();
	}

	public class ZxdgSurfaceV6 : IZxdgSurfaceV6 {
		internal readonly WlSurface WlSurface;
		public ZxdgSurfaceV6(WlSurface surface) : base(surface.Owner, null) =>
			WlSurface = surface;

		public override void Destroy() => throw new NotImplementedException();
		public override IZxdgToplevelV6 GetToplevel() => new ZxdgToplevelV6(this);
		public override IZxdgPopupV6 GetPopup(IZxdgSurfaceV6 parent, IZxdgPositionerV6 positioner) =>
			throw new NotImplementedException();
		public override void SetWindowGeometry(int x, int y, int width, int height) =>
			Helper.Log($"Window geometry {x}x{y} {width}x{height}");
		public override void AckConfigure(uint serial) {}
	}

	public class ZxdgToplevelV6 : IZxdgToplevelV6, ICommitter {
		readonly ZxdgSurfaceV6 Surface;
		bool BeenSetup;
		public ZxdgToplevelV6(ZxdgSurfaceV6 surface) : base(surface.Owner, null) {
			Surface = surface;
			Surface.WlSurface.Committer = this;
		}

		public override void Destroy() => throw new System.NotImplementedException();
		public override void SetParent(IZxdgToplevelV6 parent) => throw new System.NotImplementedException();
		public override void SetTitle(string title) => Helper.Log($"Top level title being set to '{title}'");
		public override void SetAppId(string app_id) => throw new System.NotImplementedException();
		public override void ShowWindowMenu(IWlSeat seat, uint serial, int x, int y) => throw new System.NotImplementedException();
		public override void Move(IWlSeat seat, uint serial) => throw new System.NotImplementedException();
		public override void Resize(IWlSeat seat, uint serial, uint edges) => throw new System.NotImplementedException();
		public override void SetMaxSize(int width, int height) => throw new System.NotImplementedException();
		public override void SetMinSize(int width, int height) => throw new System.NotImplementedException();
		public override void SetMaximized() => throw new System.NotImplementedException();
		public override void UnsetMaximized() => throw new System.NotImplementedException();
		public override void SetFullscreen(IWlOutput output) => throw new System.NotImplementedException();
		public override void UnsetFullscreen() => throw new System.NotImplementedException();
		public override void SetMinimized() => throw new System.NotImplementedException();
		
		public void Commit() {
			Helper.Log($"ZxdgToplevelV6 committed! {BeenSetup}");
			if(!BeenSetup) {
				Configure(640, 480, new byte[0]);
				Surface.Configure(Owner.Serial);
				BeenSetup = true;
			}
		}
	}
}