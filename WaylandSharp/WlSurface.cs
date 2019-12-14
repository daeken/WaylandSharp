using System;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public interface ICommitter {
		void Commit();
	}
	
	public class WlSurface : IWlSurface {
		internal ICommitter Committer;
		WlBuffer Buffer;

		WlCallback CurrentFrameCallback, PendingFrameCallback;

		public WlSurface(Client owner, uint? id = null) : base(owner, id) => 
			DisplayServer.Instance.Frame += () => {
				if(CurrentFrameCallback == null) return;
				CurrentFrameCallback.Done(DisplayServer.Instance.Time);
				Owner.Destroy(CurrentFrameCallback);
				CurrentFrameCallback = null;
			};

		internal override void Setup() => Owner.AddSurface(this);

		public override void Destroy() => throw new NotImplementedException();
		public override void Attach(IWlBuffer buffer, int x, int y) {
			Helper.Log($"Attempting to attach buffer to {x}x{y}");
			Buffer = (WlBuffer) buffer;
			var id = DisplayServer.Instance.ImageDataBuilder(Buffer.Buffer, Buffer.Offset, Buffer.Width, Buffer.Height,
				Buffer.Stride, Buffer.Format);
		}

		public override void Damage(int x, int y, int width, int height) {
			Helper.Log($"Setting damage region {x}x{y} {width}x{height}");
			Buffer?.Release();
		}

		public override IWlCallback Frame() => CurrentFrameCallback = new WlCallback(Owner);

		public override void SetOpaqueRegion(IWlRegion region) => Helper.Log($"Setting opaque region -- {((WlRegion) region)?.Region}");
		public override void SetInputRegion(IWlRegion region) => Helper.Log($"Setting input region for ID 0x{Id}");
		
		public override void Commit() {
			Helper.Log("WlSurface commit!");
			if(Committer != null)
				Committer.Commit();
			if(PendingFrameCallback != null) {
				CurrentFrameCallback = PendingFrameCallback;
				PendingFrameCallback = null;
			}
		}

		public override void SetBufferTransform(IWlOutput.Enum.Transform transform) => throw new System.NotImplementedException();
		public override void SetBufferScale(int scale) => throw new System.NotImplementedException();
		public override void DamageBuffer(int x, int y, int width, int height) => throw new System.NotImplementedException();
	}
}