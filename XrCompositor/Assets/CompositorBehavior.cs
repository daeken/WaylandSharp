using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WaylandSharp;

public class CompositorBehavior : MonoBehaviour {
	DisplayServer Server;
	// Start is called before the first frame update
	void Start() {
		new Thread(() => {
			Server = new DisplayServer("10.0.0.37", 1337);
			Server.LogMessage += msg => Debug.Log(msg);
			Server.NewClient += client => {
				client.AddGlobal(new WlShm(client));
				client.AddGlobal(new WlCompositor(client));
				client.AddGlobal(new WlSubcompositor(client));
				client.AddGlobal(new ZxdgShellV6(client));
				client.AddGlobal(new WlOutput(client));
			};
			Server.Run();
		}).Start();
	}

	// Update is called once per frame
	void Update() =>
		Server?.OnFrame();
}