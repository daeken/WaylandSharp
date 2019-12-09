using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mono.Unix.Native;

namespace WaylandSharp {
	public class DisplayServer {
		public static DisplayServer Instance;
		readonly WlSocket ServerSocket;
		public readonly List<Client> Clients = new List<Client>();
		
		public readonly ConcurrentQueue<(IWaylandObject Object, uint Opcode, uint Length,
			TaskCompletionSource<Exception> Cb)> WorkQueue =
			new ConcurrentQueue<(IWaylandObject, uint, uint, TaskCompletionSource<Exception>)>();

		public event Action<Client> NewClient;
		
		public DisplayServer(string path = null) {
			Instance = this;
			ServerSocket = new WlSocket(path ?? $"/run/user/{Syscall.getuid()}/wayland-0"); // TODO: Proper automatic path
		}

		public void Run() {
			ServerSocket.Start();
			while(true) {
				if(!WorkQueue.TryDequeue(out var job)) {
					Thread.Sleep(10);
					continue;
				}

				try {
					job.Object.ProcessRequest((int) job.Opcode, (int) job.Length);
					job.Cb.SetResult(null);
				} catch(Exception e) {
					job.Cb.SetResult(e);
				}
			}
		}

		public void AddClient(Client client) {
			Clients.Add(client);
			NewClient?.Invoke(client);
		}
	}
}