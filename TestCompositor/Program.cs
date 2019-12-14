using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WaylandSharp;

namespace TestCompositor {
	internal class Program {
		public static void Main(string[] args) {
			var server = new DisplayServer(args[0], Int32.Parse(args[1]));
			server.LogMessage += Console.WriteLine;
			server.NewClient += client => {
				client.AddGlobal(new WlShm(client));
				client.AddGlobal(new WlCompositor(client));
				client.AddGlobal(new WlSubcompositor(client));
				client.AddGlobal(new ZxdgShellV6(client));
				client.AddGlobal(new WlOutput(client));
			};
			server.ImageDataBuilder = (bytes, i, arg3, arg4, arg5, arg6) => null;
			var iter = 0;
			while(true) {
				server.Process();
				if(iter++ == 1000) {
					iter = 0;
					server.OnFrame();
				}
			}
		}
	}
}