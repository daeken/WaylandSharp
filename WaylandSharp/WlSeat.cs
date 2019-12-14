using System.Collections.Generic;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlSeat : IWlSeat {
		public readonly List<WlKeyboard> Keyboards = new List<WlKeyboard>();

		public WlSeat(Client owner) : base(owner, null) { }

		internal override void Setup() => Capabilities(Enum.Capability.Keyboard);

		public override IWlPointer GetPointer() => throw new System.NotImplementedException();
		public override IWlKeyboard GetKeyboard() {
			var keyboard = new WlKeyboard(Owner);
			Keyboards.Add(keyboard);
			return keyboard;
		}

		public override IWlTouch GetTouch() => throw new System.NotImplementedException();
		public override void Release() => throw new System.NotImplementedException();
	}
}