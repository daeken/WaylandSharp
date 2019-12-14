using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mono.Unix.Native;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class DisplayServer {
		public static DisplayServer Instance;
		readonly TcpListener ServerSocket;
		readonly Socket ClientSocket;
		public readonly List<Client> Clients = new List<Client>();
		internal event Action Frame;
		public Func<byte[], int, int, int, int, IWlShm.Enum.Format, IImageData> ImageDataBuilder;
		readonly Stopwatch Stopwatch = Stopwatch.StartNew();
		public uint Time => (uint) Stopwatch.ElapsedMilliseconds; // TODO: Smarter approach. This will die in 49 days.
		
		public readonly ConcurrentQueue<(IWaylandObject Object, uint Opcode, byte[] Buffer,
			TaskCompletionSource<Exception> Cb)> WorkQueue =
			new ConcurrentQueue<(IWaylandObject, uint, byte[], TaskCompletionSource<Exception>)>();

		public event Action<Client> NewClient;
		public event Action<string> LogMessage;
		public event Action<string> ErrorMessage;
		
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
			Frame();
		}

		public void Process() {
			while(ServerSocket.Pending()) {
				var client = new Client(ServerSocket.AcceptSocket());
				Helper.Log("Got client!");
				Clients.Add(client);
				NewClient?.Invoke(client);
				client.Start();
				Helper.Log("Started client");
			}

			while(WorkQueue.TryDequeue(out var job)) {
				Helper.Log($"Got new job: opcode {job.Opcode} for id 0x{job.Object.Id:X} ({job.Object})");
				job.Object.ProcessRequest((int) job.Opcode, job.Buffer);
				job.Cb.SetResult(null);
			}
		}

		public void Log(string message) => LogMessage?.Invoke(message);
		public void Error(string message) => ErrorMessage?.Invoke(message);
	}
}