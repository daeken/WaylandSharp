using System;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlCallback : IWlCallback {
		readonly Action<WlCallback> OnSetup;

		public WlCallback(Client owner, Action<WlCallback> onSetup = null, uint? id = null) : base(owner, id) =>
			OnSetup = onSetup;

		internal override void Setup() => OnSetup?.Invoke(this);
	}
}