using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WaylandSharp {
	public class Client {
		enum WmsgType {
			Protocol, 
			InjectRids, 
			OpenFile, 
			ExtendFile, 
			OpenDmabuf, 
			BufferFill, 
			BufferDiff, 
			OpenIrPipe, 
			OpenIwPipe, 
			OpenRwPipe, 
			PipeTransfer, 
			PipeShutdownR, 
			PipeShutdownW, 
			OpenDmavidSrc, 
			OpenDmavidDst, 
			SendDmavidPacket, 
			AckNBlocks, 
			Restart, 
			Close
		}

		internal readonly Socket Socket;
		readonly Dictionary<uint, IWaylandObject> Objects = new Dictionary<uint, IWaylandObject>();

		uint _Serial = 0;
		public uint Serial => _Serial++;

		public readonly WlDisplay Display;
		public readonly WlRegistry Registry;

		public readonly IDictionary<int, byte[]> Files = new Dictionary<int, byte[]>();
		internal readonly Queue<int> Rids = new Queue<int>();
		
		public Client(Socket socket) {
			Socket = socket;
			Objects[1] = Display = new WlDisplay(this, 1);
			Registry = new WlRegistry(this);
		}

		internal void Start() =>
			new Thread(() => {
				try {
					var hbuf = new byte[4];
					var pads = new[] { null, new byte[3], new byte[2], new byte[1] };
					var msgcount = 0;
					while(true) {
						Socket.ReceiveAll(hbuf);
						var size_and_type = BitConverter.ToUInt32(hbuf, 0);
						var size = size_and_type >> 5;
						var type = (WmsgType) (size_and_type & 0x1FU);
						msgcount++;
						if(size == 0 && type == 0) continue;
						//Console.WriteLine($"size {size} type {type}");

						var abuf = new byte[size - 4];
						Socket.ReceiveAll(abuf);
						if((size & 3) != 0)
							Socket.ReceiveAll(pads[size & 3]);
						//if(type != WmsgType.BufferFill)
						//	abuf.Hexdump();

						switch(type) {
							case WmsgType.Protocol: {
								var offset = 0;
								while(offset < size - 4) {
									var cbt = new TaskCompletionSource<Exception>();
									//Console.WriteLine($"Attempting to read wayland packet from 0x{offset:X}");
									//abuf.Hexdump();
									var id = BitConverter.ToUInt32(abuf, offset);
									var opl = BitConverter.ToUInt32(abuf, offset + 4);
									offset += 8;
									var opcode = opl & 0xFFFF;
									var length = (int) (opl >> 16);
									Debug.Assert(offset + length - 8 <= size - 4);
									var tbuf = new byte[length - 8];
									Array.Copy(abuf, offset, tbuf, 0, length - 8);
									DisplayServer.Instance.WorkQueue.Enqueue((GetObject<IWaylandObject>(id), opcode,
										tbuf,
										cbt));
									cbt.Task.Wait();
									if(cbt.Task.Result != null)
										throw cbt.Task.Result;
									offset += length - 8;
								}

								break;
							}
							case WmsgType.OpenFile: {
								var fd = BitConverter.ToInt32(abuf, 0);
								var fsize = BitConverter.ToInt32(abuf, 4);
								Helper.Log($"Opened file {fd} with size 0x{fsize:X} bytes");
								Files[fd] = new byte[fsize];
								break;
							}
							case WmsgType.BufferFill: {
								var fd = BitConverter.ToInt32(abuf, 0);
								var start = BitConverter.ToUInt32(abuf, 4);
								var end = BitConverter.ToUInt32(abuf, 8);
								Helper.AssertEqual((uint) abuf.Length - 12, end - start); // COMP_NONE only for now
								Helper.Log($"Writing 0x{end - start:X} bytes to fd {fd} 0x{start:X}-0x{end:X}");
								Array.Copy(abuf, 12, Files[fd], start, end - start);
								break;
							}
							case WmsgType.InjectRids: {
								var offset = 0;
								while(offset < size - 4) {
									Rids.Enqueue(BitConverter.ToInt32(abuf, offset));
									offset += 4;
								}
								break;
							}
							case WmsgType.Close:
								Helper.Log("Client closed pipe");
								return;
							case WmsgType.AckNBlocks:
								Helper.Log(
									$"Waypipe acked {BitConverter.ToUInt32(abuf, 0)} messages; acking {msgcount}");
								var ackbuf = new byte[8];
								Array.Copy(BitConverter.GetBytes(((uint) WmsgType.AckNBlocks) | (8 << 5)), 0, ackbuf, 0,
									4);
								Array.Copy(BitConverter.GetBytes(msgcount), 0, ackbuf, 4, 4);
								Socket.Send(ackbuf);
								break;
							default:
								throw new NotImplementedException(
									$"Wire message type {type} with length {size} not implemented");
						}
					}
				} catch(Exception e) {
					Helper.Log("Client got exception; disconnecting");
					Helper.Log(e);
				}
			}).Start();
		
		internal uint NewId() => throw new NotImplementedException();
		
		internal int GetNextFd() => Rids.Dequeue();

		public T GetObject<T>(uint id) where T : IWaylandObject => Objects[id] as T;

		public void SetObject<T>(uint id, T obj) where T : IWaylandObject {
			Helper.Log($"Setting id 0x{id:X} to {obj}");
			Helper.Assert(!Objects.ContainsKey(id));
			Objects[id] = obj;
			obj.Id = id;
			obj.Setup();
		}

		public void AddGlobal(IWaylandObject obj) => Registry.Add(obj);
	}
}