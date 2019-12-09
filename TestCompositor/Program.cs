﻿using System.Threading.Tasks;
using WaylandSharp;

namespace TestCompositor {
	internal class Program {
		public static void Main(string[] args) {
			var server = new DisplayServer();
			server.NewClient += client => {
				client.AddGlobal(new WlOutput(client));
			};
			server.Run();
		}
	}
}