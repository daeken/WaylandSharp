using System;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public interface ICommitter {
		void Commit();
	}
	
	public class WlSurface : IWlSurface {
		internal ICommitter Committer;
		IWlBuffer Buffer;
		public WlSurface(Client owner, uint? id = null) : base(owner, id) { }
		
		public override void Destroy() => throw new System.NotImplementedException();
		public override void Attach(IWlBuffer buffer, int x, int y) {
			Helper.Log($"Attempting to attach buffer to {x}x{y}");
			Buffer = buffer;
		}

		public override void Damage(int x, int y, int width, int height) {
			Helper.Log($"Setting damage region {x}x{y} {width}x{height}");
			Buffer?.Release();
		}

		public override IWlCallback Frame() =>
			new WlCallback(Owner, cb => DisplayServer.Instance.Frame += () => cb.Done(Owner.Serial));
		public override void SetOpaqueRegion(IWlRegion region) => Helper.Log($"Setting opaque region");
		public override void SetInputRegion(IWlRegion region) => Helper.Log($"Setting input region");
		
		public override void Commit() {
			Helper.Log("WlSurface commit!");
			if(Committer != null)
				Committer.Commit();
		}

		public override void SetBufferTransform(IWlOutput.Enum.Transform transform) => throw new System.NotImplementedException();
		public override void SetBufferScale(int scale) => throw new System.NotImplementedException();
		public override void DamageBuffer(int x, int y, int width, int height) => throw new System.NotImplementedException();
	}
}