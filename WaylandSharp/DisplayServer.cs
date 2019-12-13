using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mono.Unix.Native;

namespace WaylandSharp {
	public class DisplayServer {
		public static DisplayServer Instance;
		readonly TcpListener ServerSocket;
		readonly Socket ClientSocket;
		public readonly List<Client> Clients = new List<Client>();
		internal event Action Frame;
		
		public readonly ConcurrentQueue<(IWaylandObject Object, uint Opcode, byte[] Buffer,
			TaskCompletionSource<Exception> Cb)> WorkQueue =
			new ConcurrentQueue<(IWaylandObject, uint, byte[], TaskCompletionSource<Exception>)>();

		public event Action<Client> NewClient;
		public event Action<string> LogMessage;
		
		public DisplayServer(string host, int port) {
			Instance = this;
			ServerSocket = new TcpListener(IPAddress.Any, 0);
			ServerSocket.Start();
			var client = new TcpClient(host, port);
			ClientSocket = client.Client;
			ClientSocket.Send(BitConverter.GetBytes(0xDEADBEEFU));
			ClientSocket.Send(BitConverter.GetBytes((ushort) ((IPEndPoint) ServerSocket.LocalEndpoint).Port));
			var cmd = "weston-terminal";
			ClientSocket.Send(BitConverter.GetBytes(cmd.Length));
			ClientSocket.Send(Encoding.ASCII.GetBytes(cmd));
		}

		public void OnFrame() {
			if(Frame == null) return;
			lock(this) {
				Frame();
			}
		}

		public void Run() {
			while(true) {
				if(ServerSocket.Pending()) {
					var client = new Client(ServerSocket.AcceptSocket());
					Helper.Log("Got client!");
					Clients.Add(client);
					NewClient?.Invoke(client);
					client.Start();
					Helper.Log("Started client");
				}
				if(!WorkQueue.TryDequeue(out var job)) {
					Thread.Sleep(10);
					continue;
				}

				lock(this) {
					//try {
						Helper.Log($"Got new job: opcode {job.Opcode} for id 0x{job.Object.Id:X} ({job.Object})");
						job.Object.ProcessRequest((int) job.Opcode, job.Buffer);
						job.Cb.SetResult(null);
					/*} catch(Exception e) {
						job.Cb.SetResult(e);
					}*/
				}
			}
		}

		public void Log(string message) => LogMessage?.Invoke(message);
	}
}