using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WaylandSharp {
	public class Client {
		internal readonly WlSocket Socket;
		readonly Dictionary<uint, IWaylandObject> Objects = new Dictionary<uint, IWaylandObject>();
		public Client(WlSocket socket) {
			Socket = socket;
			Objects[1] = new WlDisplay(this, 1);
		}

		internal void Start() =>
			new Thread(() => {
				var buf = new byte[8];
				while(true) {
					var cbt = new TaskCompletionSource<Exception>();
					
					Socket.Read(buf);
					var id = BitConverter.ToUInt32(buf, 0);
					var opl = BitConverter.ToUInt32(buf, 4);
					var opcode = opl & 0xFFFF;
					var length = opl >> 16;
					
					Console.WriteLine($"Got request for id 0x{id:X}!");
					Console.ReadLine();
					
					DisplayServer.Instance.WorkQueue.Enqueue((GetObject<IWaylandObject>(id), opcode, length, cbt));
					
					cbt.Task.Wait();
					if(cbt.Task.Result != null)
						throw cbt.Task.Result;
				}
			}).Start();
		
		internal uint NewId() => throw new NotImplementedException();

		public T GetObject<T>(uint id) where T : IWaylandObject => Objects[id] as T;

		public void SetObject<T>(uint id, T obj) where T : IWaylandObject {
			Console.WriteLine($"Setting id 0x{id:X} to {obj}");
			Debug.Assert(!Objects.ContainsKey(id));
			Objects[id] = obj;
			obj.Id = id;
			obj.Setup();
		}
	}
}