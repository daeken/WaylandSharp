using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using WaylandSharp;

public class CompositorBehavior : MonoBehaviour {
	class TextureImageData : IImageData {
		public Texture2D Texture;
		public TextureImageData(Texture2D texture) => Texture = texture;
	}
	DisplayServer Server;
	readonly Queue<Action> WorkQueue = new Queue<Action>();

	event Action<uint> KeyDown;
	event Action<uint> KeyUp;
	event Action<uint> Modifiers;
	// Start is called before the first frame update
	void Start() {
		Server = new DisplayServer("10.0.0.40", 1337);
		Server.ImageDataBuilder = (buffer, offset, width, height, stride, format) => {
			var id = new TextureImageData(null);
			var data = new byte[width * height * 4];
			for(var y = 0; y < height; ++y)
				Array.Copy(buffer, offset + y * stride, data, (height - y - 1) * width * 4, width * 4);
			var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
			tex.LoadRawTextureData(data);
			tex.Apply();
			//using(var fp = File.OpenWrite("/Users/cbrocious/tempraw.bin"))
			//	fp.Write(buffer, 0, buffer.Length);
			GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", tex);
			id.Texture = tex;
			transform.GetChild(0).transform.localScale = new Vector3(width / 1.5f, height / 1.5f, 1);
			return id;
		};
		//Server.LogMessage += msg => Debug.Log(msg);
		Server.ErrorMessage += msg => Debug.LogError(msg);
		Server.NewClient += client => {
			client.AddGlobal(new WlShm(client));
			client.AddGlobal(new WlCompositor(client));
			client.AddGlobal(new WlSubcompositor(client));
			client.AddGlobal(new ZxdgShellV6(client));
			var output = new WlOutput(client);
			client.AddGlobal(output);
			var seat = new WlSeat(client);
			var surfaces = new List<WlSurface>();
			client.NewSurface += surface => surfaces.Add(surface);
			KeyDown += code => {
				foreach(var surface in surfaces) {
					if(surface.Id == 10) continue;
					foreach(var keyboard in seat.Keyboards) {
						keyboard.Enter(client.Serial, surface, new byte[0]);
						keyboard.KeyDown(code);
						//keyboard.Leave(client.Serial, surface);
					}
				}
			};
			KeyUp += code => {
				foreach(var surface in surfaces) {
					if(surface.Id == 10) continue;
					foreach(var keyboard in seat.Keyboards) {
						keyboard.Enter(client.Serial, surface, new byte[0]);
						keyboard.KeyUp(code);
						//keyboard.Leave(client.Serial, surface);
					}
					break;
				}
			};
			Modifiers += mask => {
				foreach(var surface in surfaces) {
					if(surface.Id == 10) continue;
					foreach(var keyboard in seat.Keyboards) {
						keyboard.Enter(client.Serial, surface, new byte[0]);
						keyboard.Modifiers(mask);
						//keyboard.Leave(client.Serial, surface);
					}
					break;
				}
			};
			client.AddGlobal(seat);
		};
	}
	
	bool[] KeyState = new bool[512];
	
	readonly static Dictionary<KeyCode, int> KeyMap = new Dictionary<KeyCode, int> {
		[KeyCode.Alpha1] = 10, 
		[KeyCode.Alpha2] = 11, 
		[KeyCode.Alpha3] = 12, 
		[KeyCode.Alpha4] = 13, 
		[KeyCode.Alpha5] = 14, 
		[KeyCode.Alpha6] = 15, 
		[KeyCode.Alpha7] = 16, 
		[KeyCode.Alpha8] = 17, 
		[KeyCode.Alpha9] = 18, 
		[KeyCode.Alpha0] = 19, 
		[KeyCode.Minus] = 20, 
		[KeyCode.Equals] = 21, 
		[KeyCode.Backspace] = 22, 
		[KeyCode.Tab] = 23, 
		
		[KeyCode.Q] = 24, 
		[KeyCode.W] = 25, 
		[KeyCode.E] = 26, 
		[KeyCode.R] = 27, 
		[KeyCode.T] = 28, 
		[KeyCode.Y] = 29, 
		[KeyCode.U] = 30, 
		[KeyCode.I] = 31, 
		[KeyCode.O] = 32, 
		[KeyCode.P] = 33,
		[KeyCode.LeftBracket] = 34, 
		[KeyCode.RightBracket] = 35,
		[KeyCode.Return] = 36, 
		[KeyCode.LeftControl] = 37, 
		
		[KeyCode.A] = 38,
		[KeyCode.S] = 39, 
		[KeyCode.D] = 40, 
		[KeyCode.F] = 41, 
		[KeyCode.G] = 42, 
		[KeyCode.H] = 43, 
		[KeyCode.J] = 44, 
		[KeyCode.K] = 45, 
		[KeyCode.L] = 46,
		[KeyCode.Semicolon] = 47, 
		[KeyCode.Quote] = 48, 
		[KeyCode.Tilde] = 49, 
		[KeyCode.LeftShift] = 50, 
		[KeyCode.Backslash] = 51, 
		
		[KeyCode.Z] = 52, 
		[KeyCode.X] = 53, 
		[KeyCode.C] = 54, 
		[KeyCode.V] = 55, 
		[KeyCode.B] = 56, 
		[KeyCode.N] = 57, 
		[KeyCode.M] = 58, 
		[KeyCode.Comma] = 59, 
		[KeyCode.Period] = 60, 
		[KeyCode.Slash] = 61, 
		[KeyCode.RightShift] = 62,
		
		[KeyCode.KeypadMultiply] = 63, 
		[KeyCode.LeftAlt] = 64,
		[KeyCode.Space] = 65, 
		[KeyCode.CapsLock] = 66, 
		
		[KeyCode.RightControl] = 105, 
		[KeyCode.RightAlt] = 108, 
		
		[KeyCode.UpArrow] = 111, 
		[KeyCode.LeftArrow] = 113, 
		[KeyCode.RightArrow] = 114, 
		[KeyCode.DownArrow] = 116, 
	};

	bool IsModifier(KeyCode kc) =>
		kc == KeyCode.LeftAlt || kc == KeyCode.LeftControl || kc == KeyCode.LeftCommand || kc == KeyCode.LeftShift ||
		kc == KeyCode.RightAlt || kc == KeyCode.RightControl || kc == KeyCode.RightCommand || kc == KeyCode.RightShift;

	int ModifierMap(KeyCode kc) {
		switch(kc) {
			case KeyCode.LeftAlt: case KeyCode.RightAlt:
				return 8;
			case KeyCode.LeftControl: case KeyCode.RightControl:
				return 4;
			case KeyCode.LeftShift: case KeyCode.RightShift:
				return 1;
			default:
				return 0;
		}
	}

	readonly Dictionary<KeyCode, bool> ModifierState = new Dictionary<KeyCode, bool>();

	// Update is called once per frame
	void Update() {
		foreach(var elem in Enum.GetValues(typeof(KeyCode))) {
			var kc = (KeyCode) elem;
			var state = Input.GetKey(kc);
			var code = (int) kc;
			if(KeyState[code] != state) {
				KeyState[code] = state;
				if(!KeyMap.TryGetValue(kc, out code))
					continue;
				code -= 8;
				if(IsModifier(kc)) {
					ModifierState[kc] = state;
					Modifiers((uint) ModifierState.Select(x => x.Value ? ModifierMap(x.Key) : 0).Sum());
				} else if(state)
					KeyDown((uint) code);
				else
					KeyUp((uint) code);
			}
		}
		Server.OnFrame();
		Server.Process();
	}
}