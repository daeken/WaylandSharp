using System.Text;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlKeyboard : IWlKeyboard {
		public WlKeyboard(Client owner) : base(owner, null) { }

		const string KeymapData = @"
xkb_keymap {
        xkb_keycodes  { include ""evdev+aliases(qwerty)"" };
		xkb_types     { include ""complete""      };
		xkb_compat    { include ""complete""      };
		xkb_symbols   { include ""pc+us+inet(evdev)""     };
		xkb_geometry  { include ""pc(pc105)""     }; 
};";
		internal override void Setup() {
			Keymap(Enum.KeymapFormat.XkbV1, Encoding.ASCII.GetBytes(KeymapData),
				(uint) Encoding.ASCII.GetBytes(KeymapData).Length);
			RepeatInfo(25, 600);
		}

		public override void Release() => throw new System.NotImplementedException();

		public void KeyDown(uint code) => Key(Owner.Serial, DisplayServer.Instance.Time, code, Enum.KeyState.Pressed);
		public void KeyUp(uint code) => Key(Owner.Serial, DisplayServer.Instance.Time, code, Enum.KeyState.Released);
		public void Modifiers(uint mask) => Modifiers(Owner.Serial, mask, 0, 0, 0);
	}
}