#pragma warning disable 0219
#pragma warning disable 1998
#pragma warning disable 8321
using System;
using System.Threading.Tasks;
namespace WaylandSharp.Generated {
	/// <summary>
	/// Core global object
	/// </summary>
	/// <remarks>
	/// The core global object.  This is a special singleton object.  It
	/// is used for internal Wayland protocol features.
	/// </remarks>
	public abstract class IWlDisplay : IWaylandObject {
		protected IWlDisplay(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Global error values
			/// </summary>
			/// <remarks>
			/// These errors are global and can be emitted in response to any
			/// server request.
			/// </remarks>
			public enum Error {
				/// <summary>
				/// Server couldn't find object
				/// </summary>
				InvalidObject = 0,
				/// <summary>
				/// Method doesn't exist on the specified interface or malformed request
				/// </summary>
				InvalidMethod = 1,
				/// <summary>
				/// Server is out of memory
				/// </summary>
				NoMemory = 2,
				/// <summary>
				/// Implementation error in compositor
				/// </summary>
				Implementation = 3,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var pre_callback = _offset;
					_offset += 4;
					Sync(out var callback);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_callback), callback);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var pre_registry = _offset;
					_offset += 4;
					GetRegistry(out var registry);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_registry), registry);
					break;
				}
			}
		}
		/// <summary>
		/// Asynchronous roundtrip
		/// </summary>
		/// <remarks>
		/// The sync request asks the server to emit the 'done' event
		/// on the returned wl_callback object.  Since requests are
		/// handled in-order and events are delivered in-order, this can
		/// be used as a barrier to ensure all previous requests and the
		/// resulting events have been handled.
		/// 
		/// The object returned by this request will be destroyed by the
		/// compositor after the callback is fired and as such the client must not
		/// attempt to use it after that point.
		/// 
		/// The callback_data passed in the callback is the event serial.
		/// </remarks>
		/// <param name="callback">Callback object for the sync request</param>
		public abstract void Sync(out IWlCallback callback);
		/// <summary>
		/// Get global registry object
		/// </summary>
		/// <remarks>
		/// This request creates a registry object that allows the client
		/// to list and bind the global objects available from the
		/// compositor.
		/// 
		/// It should be noted that the server side resources consumed in
		/// response to a get_registry request can only be released when the
		/// client disconnects, not when the client side proxy is destroyed.
		/// Therefore, clients should invoke get_registry as infrequently as
		/// possible to avoid wasting memory.
		/// </remarks>
		/// <param name="registry">Global registry object</param>
		public abstract void GetRegistry(out IWlRegistry registry);
		/// <summary>
		/// Fatal error event
		/// </summary>
		/// <remarks>
		/// The error event is sent out when a fatal (non-recoverable)
		/// error has occurred.  The object_id argument is the object
		/// where the error occurred, most often in response to a request
		/// to that object.  The code identifies the error and is defined
		/// by the object interface.  As such, each interface defines its
		/// own set of error codes.  The message is a brief description
		/// of the error, for (debugging) convenience.
		/// </remarks>
		/// <param name="object_id">Object where the error occurred</param>
		/// <param name="code">Error code</param>
		/// <param name="message">Error description</param>
		public void Error(IWaylandObject object_id, uint code, string message) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += Helper.StringSize(message);
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, object_id.Id);
			Helper.WriteUint(tbuf, ref _offset, (uint) code);
			Helper.WriteString(tbuf, ref _offset, message);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Acknowledge object ID deletion
		/// </summary>
		/// <remarks>
		/// This event is used internally by the object ID management
		/// logic. When a client deletes an object that it had created,
		/// the server will send this event to acknowledge that it has
		/// seen the delete request. When the client receives this event,
		/// it will know that it can safely reuse the object ID.
		/// </remarks>
		/// <param name="id">Deleted object ID</param>
		public void DeleteId(uint id) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) id);
			SendEvent(1, tbuf);
		}
	}
	/// <summary>
	/// Global registry object
	/// </summary>
	/// <remarks>
	/// The singleton global registry object.  The server has a number of
	/// global objects that are available to all clients.  These objects
	/// typically represent an actual object in the server (for example,
	/// an input device) or they are singleton objects that provide
	/// extension functionality.
	/// 
	/// When a client creates a registry object, the registry object
	/// will emit a global event for each global currently in the
	/// registry.  Globals come and go as a result of device or
	/// monitor hotplugs, reconfiguration or other events, and the
	/// registry will send out global and global_remove events to
	/// keep the client up to date with the changes.  To mark the end
	/// of the initial burst of events, the client can use the
	/// wl_display.sync request immediately after calling
	/// wl_display.get_registry.
	/// 
	/// A client can bind to a global object by using the bind
	/// request.  This creates a client-side handle that lets the object
	/// emit events to the client and lets the client invoke requests on
	/// the object.
	/// </remarks>
	public abstract class IWlRegistry : IWaylandObject {
		protected IWlRegistry(Client owner, uint? id) : base(owner, id) {}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var name = (uint) Helper.ReadUint(tbuf, ref _offset);
					var pre_id = _offset;
					_offset += 4;
					Bind(name, out var id);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
			}
		}
		/// <summary>
		/// Bind an object to the display
		/// </summary>
		/// <remarks>
		/// Binds a new, client-created object to the server using the
		/// specified name as the identifier.
		/// </remarks>
		/// <param name="name">Unique numeric name of the object</param>
		/// <param name="id">Bounded object</param>
		public abstract void Bind(uint name, out IWaylandObject id);
		/// <summary>
		/// Announce global object
		/// </summary>
		/// <remarks>
		/// Notify the client of global objects.
		/// 
		/// The event notifies the client that a global object with
		/// the given name is now available, and it implements the
		/// given version of the given interface.
		/// </remarks>
		/// <param name="name">Numeric name of the global object</param>
		/// <param name="interface">Interface implemented by the object</param>
		/// <param name="version">Interface version</param>
		public void Global(uint name, string @interface, uint version) {
			var _offset = 0;
			_offset += 4;
			_offset += Helper.StringSize(@interface);
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) name);
			Helper.WriteString(tbuf, ref _offset, @interface);
			Helper.WriteUint(tbuf, ref _offset, (uint) version);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Announce removal of global object
		/// </summary>
		/// <remarks>
		/// Notify the client of removed global objects.
		/// 
		/// This event notifies the client that the global identified
		/// by name is no longer available.  If the client bound to
		/// the global using the bind request, the client should now
		/// destroy that object.
		/// 
		/// The object remains valid and requests to the object will be
		/// ignored until the client destroys it, to avoid races between
		/// the global going away and a client sending a request to it.
		/// </remarks>
		/// <param name="name">Numeric name of the global object</param>
		public void GlobalRemove(uint name) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) name);
			SendEvent(1, tbuf);
		}
	}
	/// <summary>
	/// Callback object
	/// </summary>
	/// <remarks>
	/// Clients can handle the 'done' event to get notified when
	/// the related request is done.
	/// </remarks>
	public abstract class IWlCallback : IWaylandObject {
		protected IWlCallback(Client owner, uint? id) : base(owner, id) {}
		internal override void ProcessRequest(int opcode, int mlen) {
			throw new NotSupportedException();
		}
		/// <summary>
		/// Bind an object to the display
		/// </summary>
		/// <remarks>
		/// Binds a new, client-created object to the server using the
		/// specified name as the identifier.
		/// </remarks>
		/// <param name="name">Unique numeric name of the object</param>
		/// <param name="id">Bounded object</param>
		public abstract void Bind(uint name, out IWaylandObject id);
		/// <summary>
		/// Done event
		/// </summary>
		/// <remarks>
		/// Notify the client when the related request is done.
		/// </remarks>
		/// <param name="callback_data">Request-specific data for the callback</param>
		public void Done(uint callback_data) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) callback_data);
			SendEvent(0, tbuf);
		}
	}
	/// <summary>
	/// The compositor singleton
	/// </summary>
	/// <remarks>
	/// A compositor.  This object is a singleton global.  The
	/// compositor is in charge of combining the contents of multiple
	/// surfaces into one displayable output.
	/// </remarks>
	public abstract class IWlCompositor : IWaylandObject {
		protected IWlCompositor(Client owner, uint? id) : base(owner, id) {}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					CreateSurface(out var id);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					CreateRegion(out var id);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
			}
		}
		/// <summary>
		/// Create new surface
		/// </summary>
		/// <remarks>
		/// Ask the compositor to create a new surface.
		/// </remarks>
		/// <param name="id">The new surface</param>
		public abstract void CreateSurface(out IWlSurface id);
		/// <summary>
		/// Create new region
		/// </summary>
		/// <remarks>
		/// Ask the compositor to create a new region.
		/// </remarks>
		/// <param name="id">The new region</param>
		public abstract void CreateRegion(out IWlRegion id);
	}
	/// <summary>
	/// A shared memory pool
	/// </summary>
	/// <remarks>
	/// The wl_shm_pool object encapsulates a piece of memory shared
	/// between the compositor and client.  Through the wl_shm_pool
	/// object, the client can allocate shared memory wl_buffer objects.
	/// All objects created through the same pool share the same
	/// underlying mapped memory. Reusing the mapped memory avoids the
	/// setup/teardown overhead and is useful when interactively resizing
	/// a surface or for many small buffers.
	/// </remarks>
	public abstract class IWlShmPool : IWaylandObject {
		protected IWlShmPool(Client owner, uint? id) : base(owner, id) {}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					var offset = (int) Helper.ReadInt(tbuf, ref _offset);
					var width = (int) Helper.ReadInt(tbuf, ref _offset);
					var height = (int) Helper.ReadInt(tbuf, ref _offset);
					var stride = (int) Helper.ReadInt(tbuf, ref _offset);
					var format = (IWlShm.Enum.Format) Helper.ReadUint(tbuf, ref _offset);
					CreateBuffer(out var id, offset, width, height, stride, format);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					var size = (int) Helper.ReadInt(tbuf, ref _offset);
					Resize(size);
					break;
				}
			}
		}
		/// <summary>
		/// Create a buffer from the pool
		/// </summary>
		/// <remarks>
		/// Create a wl_buffer object from the pool.
		/// 
		/// The buffer is created offset bytes into the pool and has
		/// width and height as specified.  The stride argument specifies
		/// the number of bytes from the beginning of one row to the beginning
		/// of the next.  The format is the pixel format of the buffer and
		/// must be one of those advertised through the wl_shm.format event.
		/// 
		/// A buffer will keep a reference to the pool it was created from
		/// so it is valid to destroy the pool immediately after creating
		/// a buffer from it.
		/// </remarks>
		/// <param name="id">Buffer to create</param>
		/// <param name="offset">Buffer byte offset within the pool</param>
		/// <param name="width">Buffer width, in pixels</param>
		/// <param name="height">Buffer height, in pixels</param>
		/// <param name="stride">Number of bytes from the beginning of one row to the beginning of the next row</param>
		/// <param name="format">Buffer pixel format</param>
		public abstract void CreateBuffer(out IWlBuffer id, int offset, int width, int height, int stride, IWlShm.Enum.Format format);
		/// <summary>
		/// Destroy the pool
		/// </summary>
		/// <remarks>
		/// Destroy the shared memory pool.
		/// 
		/// The mmapped memory will be released when all
		/// buffers that have been created from this pool
		/// are gone.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// Change the size of the pool mapping
		/// </summary>
		/// <remarks>
		/// This request will cause the server to remap the backing memory
		/// for the pool from the file descriptor passed when the pool was
		/// created, but using the new size.  This request can only be
		/// used to make the pool bigger.
		/// </remarks>
		/// <param name="size">New size of the pool, in bytes</param>
		public abstract void Resize(int size);
	}
	/// <summary>
	/// Shared memory support
	/// </summary>
	/// <remarks>
	/// A singleton global object that provides support for shared
	/// memory.
	/// 
	/// Clients can create wl_shm_pool objects using the create_pool
	/// request.
	/// 
	/// At connection setup time, the wl_shm object emits one or more
	/// format events to inform clients about the valid pixel formats
	/// that can be used for buffers.
	/// </remarks>
	public abstract class IWlShm : IWaylandObject {
		protected IWlShm(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Wl_shm error values
			/// </summary>
			/// <remarks>
			/// These errors can be emitted in response to wl_shm requests.
			/// </remarks>
			public enum Error {
				/// <summary>
				/// Buffer format is not known
				/// </summary>
				InvalidFormat = 0,
				/// <summary>
				/// Invalid size or stride during pool or buffer creation
				/// </summary>
				InvalidStride = 1,
				/// <summary>
				/// Mmapping the file descriptor failed
				/// </summary>
				InvalidFd = 2,
			}
			/// <summary>
			/// Pixel formats
			/// </summary>
			/// <remarks>
			/// This describes the memory layout of an individual pixel.
			/// 
			/// All renderers should support argb8888 and xrgb8888 but any other
			/// formats are optional and may not be supported by the particular
			/// renderer in use.
			/// 
			/// The drm format codes match the macros defined in drm_fourcc.h, except
			/// argb8888 and xrgb8888. The formats actually supported by the compositor
			/// will be reported by the format event.
			/// </remarks>
			public enum Format {
				/// <summary>
				/// 32-bit ARGB format, [31:0] A:R:G:B 8:8:8:8 little endian
				/// </summary>
				Argb8888 = 0,
				/// <summary>
				/// 32-bit RGB format, [31:0] x:R:G:B 8:8:8:8 little endian
				/// </summary>
				Xrgb8888 = 1,
				/// <summary>
				/// 8-bit color index format, [7:0] C
				/// </summary>
				C8 = 0x20203843,
				/// <summary>
				/// 8-bit RGB format, [7:0] R:G:B 3:3:2
				/// </summary>
				Rgb332 = 0x38424752,
				/// <summary>
				/// 8-bit BGR format, [7:0] B:G:R 2:3:3
				/// </summary>
				Bgr233 = 0x38524742,
				/// <summary>
				/// 16-bit xRGB format, [15:0] x:R:G:B 4:4:4:4 little endian
				/// </summary>
				Xrgb4444 = 0x32315258,
				/// <summary>
				/// 16-bit xBGR format, [15:0] x:B:G:R 4:4:4:4 little endian
				/// </summary>
				Xbgr4444 = 0x32314258,
				/// <summary>
				/// 16-bit RGBx format, [15:0] R:G:B:x 4:4:4:4 little endian
				/// </summary>
				Rgbx4444 = 0x32315852,
				/// <summary>
				/// 16-bit BGRx format, [15:0] B:G:R:x 4:4:4:4 little endian
				/// </summary>
				Bgrx4444 = 0x32315842,
				/// <summary>
				/// 16-bit ARGB format, [15:0] A:R:G:B 4:4:4:4 little endian
				/// </summary>
				Argb4444 = 0x32315241,
				/// <summary>
				/// 16-bit ABGR format, [15:0] A:B:G:R 4:4:4:4 little endian
				/// </summary>
				Abgr4444 = 0x32314241,
				/// <summary>
				/// 16-bit RBGA format, [15:0] R:G:B:A 4:4:4:4 little endian
				/// </summary>
				Rgba4444 = 0x32314152,
				/// <summary>
				/// 16-bit BGRA format, [15:0] B:G:R:A 4:4:4:4 little endian
				/// </summary>
				Bgra4444 = 0x32314142,
				/// <summary>
				/// 16-bit xRGB format, [15:0] x:R:G:B 1:5:5:5 little endian
				/// </summary>
				Xrgb1555 = 0x35315258,
				/// <summary>
				/// 16-bit xBGR 1555 format, [15:0] x:B:G:R 1:5:5:5 little endian
				/// </summary>
				Xbgr1555 = 0x35314258,
				/// <summary>
				/// 16-bit RGBx 5551 format, [15:0] R:G:B:x 5:5:5:1 little endian
				/// </summary>
				Rgbx5551 = 0x35315852,
				/// <summary>
				/// 16-bit BGRx 5551 format, [15:0] B:G:R:x 5:5:5:1 little endian
				/// </summary>
				Bgrx5551 = 0x35315842,
				/// <summary>
				/// 16-bit ARGB 1555 format, [15:0] A:R:G:B 1:5:5:5 little endian
				/// </summary>
				Argb1555 = 0x35315241,
				/// <summary>
				/// 16-bit ABGR 1555 format, [15:0] A:B:G:R 1:5:5:5 little endian
				/// </summary>
				Abgr1555 = 0x35314241,
				/// <summary>
				/// 16-bit RGBA 5551 format, [15:0] R:G:B:A 5:5:5:1 little endian
				/// </summary>
				Rgba5551 = 0x35314152,
				/// <summary>
				/// 16-bit BGRA 5551 format, [15:0] B:G:R:A 5:5:5:1 little endian
				/// </summary>
				Bgra5551 = 0x35314142,
				/// <summary>
				/// 16-bit RGB 565 format, [15:0] R:G:B 5:6:5 little endian
				/// </summary>
				Rgb565 = 0x36314752,
				/// <summary>
				/// 16-bit BGR 565 format, [15:0] B:G:R 5:6:5 little endian
				/// </summary>
				Bgr565 = 0x36314742,
				/// <summary>
				/// 24-bit RGB format, [23:0] R:G:B little endian
				/// </summary>
				Rgb888 = 0x34324752,
				/// <summary>
				/// 24-bit BGR format, [23:0] B:G:R little endian
				/// </summary>
				Bgr888 = 0x34324742,
				/// <summary>
				/// 32-bit xBGR format, [31:0] x:B:G:R 8:8:8:8 little endian
				/// </summary>
				Xbgr8888 = 0x34324258,
				/// <summary>
				/// 32-bit RGBx format, [31:0] R:G:B:x 8:8:8:8 little endian
				/// </summary>
				Rgbx8888 = 0x34325852,
				/// <summary>
				/// 32-bit BGRx format, [31:0] B:G:R:x 8:8:8:8 little endian
				/// </summary>
				Bgrx8888 = 0x34325842,
				/// <summary>
				/// 32-bit ABGR format, [31:0] A:B:G:R 8:8:8:8 little endian
				/// </summary>
				Abgr8888 = 0x34324241,
				/// <summary>
				/// 32-bit RGBA format, [31:0] R:G:B:A 8:8:8:8 little endian
				/// </summary>
				Rgba8888 = 0x34324152,
				/// <summary>
				/// 32-bit BGRA format, [31:0] B:G:R:A 8:8:8:8 little endian
				/// </summary>
				Bgra8888 = 0x34324142,
				/// <summary>
				/// 32-bit xRGB format, [31:0] x:R:G:B 2:10:10:10 little endian
				/// </summary>
				Xrgb2101010 = 0x30335258,
				/// <summary>
				/// 32-bit xBGR format, [31:0] x:B:G:R 2:10:10:10 little endian
				/// </summary>
				Xbgr2101010 = 0x30334258,
				/// <summary>
				/// 32-bit RGBx format, [31:0] R:G:B:x 10:10:10:2 little endian
				/// </summary>
				Rgbx1010102 = 0x30335852,
				/// <summary>
				/// 32-bit BGRx format, [31:0] B:G:R:x 10:10:10:2 little endian
				/// </summary>
				Bgrx1010102 = 0x30335842,
				/// <summary>
				/// 32-bit ARGB format, [31:0] A:R:G:B 2:10:10:10 little endian
				/// </summary>
				Argb2101010 = 0x30335241,
				/// <summary>
				/// 32-bit ABGR format, [31:0] A:B:G:R 2:10:10:10 little endian
				/// </summary>
				Abgr2101010 = 0x30334241,
				/// <summary>
				/// 32-bit RGBA format, [31:0] R:G:B:A 10:10:10:2 little endian
				/// </summary>
				Rgba1010102 = 0x30334152,
				/// <summary>
				/// 32-bit BGRA format, [31:0] B:G:R:A 10:10:10:2 little endian
				/// </summary>
				Bgra1010102 = 0x30334142,
				/// <summary>
				/// Packed YCbCr format, [31:0] Cr0:Y1:Cb0:Y0 8:8:8:8 little endian
				/// </summary>
				Yuyv = 0x56595559,
				/// <summary>
				/// Packed YCbCr format, [31:0] Cb0:Y1:Cr0:Y0 8:8:8:8 little endian
				/// </summary>
				Yvyu = 0x55595659,
				/// <summary>
				/// Packed YCbCr format, [31:0] Y1:Cr0:Y0:Cb0 8:8:8:8 little endian
				/// </summary>
				Uyvy = 0x59565955,
				/// <summary>
				/// Packed YCbCr format, [31:0] Y1:Cb0:Y0:Cr0 8:8:8:8 little endian
				/// </summary>
				Vyuy = 0x59555956,
				/// <summary>
				/// Packed AYCbCr format, [31:0] A:Y:Cb:Cr 8:8:8:8 little endian
				/// </summary>
				Ayuv = 0x56555941,
				/// <summary>
				/// 2 plane YCbCr Cr:Cb format, 2x2 subsampled Cr:Cb plane
				/// </summary>
				Nv12 = 0x3231564e,
				/// <summary>
				/// 2 plane YCbCr Cb:Cr format, 2x2 subsampled Cb:Cr plane
				/// </summary>
				Nv21 = 0x3132564e,
				/// <summary>
				/// 2 plane YCbCr Cr:Cb format, 2x1 subsampled Cr:Cb plane
				/// </summary>
				Nv16 = 0x3631564e,
				/// <summary>
				/// 2 plane YCbCr Cb:Cr format, 2x1 subsampled Cb:Cr plane
				/// </summary>
				Nv61 = 0x3136564e,
				/// <summary>
				/// 3 plane YCbCr format, 4x4 subsampled Cb (1) and Cr (2) planes
				/// </summary>
				Yuv410 = 0x39565559,
				/// <summary>
				/// 3 plane YCbCr format, 4x4 subsampled Cr (1) and Cb (2) planes
				/// </summary>
				Yvu410 = 0x39555659,
				/// <summary>
				/// 3 plane YCbCr format, 4x1 subsampled Cb (1) and Cr (2) planes
				/// </summary>
				Yuv411 = 0x31315559,
				/// <summary>
				/// 3 plane YCbCr format, 4x1 subsampled Cr (1) and Cb (2) planes
				/// </summary>
				Yvu411 = 0x31315659,
				/// <summary>
				/// 3 plane YCbCr format, 2x2 subsampled Cb (1) and Cr (2) planes
				/// </summary>
				Yuv420 = 0x32315559,
				/// <summary>
				/// 3 plane YCbCr format, 2x2 subsampled Cr (1) and Cb (2) planes
				/// </summary>
				Yvu420 = 0x32315659,
				/// <summary>
				/// 3 plane YCbCr format, 2x1 subsampled Cb (1) and Cr (2) planes
				/// </summary>
				Yuv422 = 0x36315559,
				/// <summary>
				/// 3 plane YCbCr format, 2x1 subsampled Cr (1) and Cb (2) planes
				/// </summary>
				Yvu422 = 0x36315659,
				/// <summary>
				/// 3 plane YCbCr format, non-subsampled Cb (1) and Cr (2) planes
				/// </summary>
				Yuv444 = 0x34325559,
				/// <summary>
				/// 3 plane YCbCr format, non-subsampled Cr (1) and Cb (2) planes
				/// </summary>
				Yvu444 = 0x34325659,
				/// <summary>
				/// [7:0] R
				/// </summary>
				R8 = 0x20203852,
				/// <summary>
				/// [15:0] R little endian
				/// </summary>
				R16 = 0x20363152,
				/// <summary>
				/// [15:0] R:G 8:8 little endian
				/// </summary>
				Rg88 = 0x38384752,
				/// <summary>
				/// [15:0] G:R 8:8 little endian
				/// </summary>
				Gr88 = 0x38385247,
				/// <summary>
				/// [31:0] R:G 16:16 little endian
				/// </summary>
				Rg1616 = 0x32334752,
				/// <summary>
				/// [31:0] G:R 16:16 little endian
				/// </summary>
				Gr1616 = 0x32335247,
				/// <summary>
				/// [63:0] x:R:G:B 16:16:16:16 little endian
				/// </summary>
				Xrgb16161616F = 0x48345258,
				/// <summary>
				/// [63:0] x:B:G:R 16:16:16:16 little endian
				/// </summary>
				Xbgr16161616F = 0x48344258,
				/// <summary>
				/// [63:0] A:R:G:B 16:16:16:16 little endian
				/// </summary>
				Argb16161616F = 0x48345241,
				/// <summary>
				/// [63:0] A:B:G:R 16:16:16:16 little endian
				/// </summary>
				Abgr16161616F = 0x48344241,
				/// <summary>
				/// [31:0] X:Y:Cb:Cr 8:8:8:8 little endian
				/// </summary>
				Xyuv8888 = 0x56555958,
				/// <summary>
				/// [23:0] Cr:Cb:Y 8:8:8 little endian
				/// </summary>
				Vuy888 = 0x34325556,
				/// <summary>
				/// Y followed by U then V, 10:10:10. Non-linear modifier only
				/// </summary>
				Vuy101010 = 0x30335556,
				/// <summary>
				/// [63:0] Cr0:0:Y1:0:Cb0:0:Y0:0 10:6:10:6:10:6:10:6 little endian per 2 Y pixels
				/// </summary>
				Y210 = 0x30313259,
				/// <summary>
				/// [63:0] Cr0:0:Y1:0:Cb0:0:Y0:0 12:4:12:4:12:4:12:4 little endian per 2 Y pixels
				/// </summary>
				Y212 = 0x32313259,
				/// <summary>
				/// [63:0] Cr0:Y1:Cb0:Y0 16:16:16:16 little endian per 2 Y pixels
				/// </summary>
				Y216 = 0x36313259,
				/// <summary>
				/// [31:0] A:Cr:Y:Cb 2:10:10:10 little endian
				/// </summary>
				Y410 = 0x30313459,
				/// <summary>
				/// [63:0] A:0:Cr:0:Y:0:Cb:0 12:4:12:4:12:4:12:4 little endian
				/// </summary>
				Y412 = 0x32313459,
				/// <summary>
				/// [63:0] A:Cr:Y:Cb 16:16:16:16 little endian
				/// </summary>
				Y416 = 0x36313459,
				/// <summary>
				/// [31:0] X:Cr:Y:Cb 2:10:10:10 little endian
				/// </summary>
				Xvyu2101010 = 0x30335658,
				/// <summary>
				/// [63:0] X:0:Cr:0:Y:0:Cb:0 12:4:12:4:12:4:12:4 little endian
				/// </summary>
				Xvyu1216161616 = 0x36335658,
				/// <summary>
				/// [63:0] X:Cr:Y:Cb 16:16:16:16 little endian
				/// </summary>
				Xvyu16161616 = 0x38345658,
				/// <summary>
				/// [63:0]   A3:A2:Y3:0:Cr0:0:Y2:0:A1:A0:Y1:0:Cb0:0:Y0:0  1:1:8:2:8:2:8:2:1:1:8:2:8:2:8:2 little endian
				/// </summary>
				Y0L0 = 0x304c3059,
				/// <summary>
				/// [63:0]   X3:X2:Y3:0:Cr0:0:Y2:0:X1:X0:Y1:0:Cb0:0:Y0:0  1:1:8:2:8:2:8:2:1:1:8:2:8:2:8:2 little endian
				/// </summary>
				X0L0 = 0x304c3058,
				/// <summary>
				/// [63:0]   A3:A2:Y3:Cr0:Y2:A1:A0:Y1:Cb0:Y0  1:1:10:10:10:1:1:10:10:10 little endian
				/// </summary>
				Y0L2 = 0x324c3059,
				/// <summary>
				/// [63:0]   X3:X2:Y3:Cr0:Y2:X1:X0:Y1:Cb0:Y0  1:1:10:10:10:1:1:10:10:10 little endian
				/// </summary>
				X0L2 = 0x324c3058,
				Yuv4208Bit = 0x38305559,
				Yuv42010Bit = 0x30315559,
				Xrgb8888A8 = 0x38415258,
				Xbgr8888A8 = 0x38414258,
				Rgbx8888A8 = 0x38415852,
				Bgrx8888A8 = 0x38415842,
				Rgb888A8 = 0x38413852,
				Bgr888A8 = 0x38413842,
				Rgb565A8 = 0x38413552,
				Bgr565A8 = 0x38413542,
				/// <summary>
				/// Non-subsampled Cr:Cb plane
				/// </summary>
				Nv24 = 0x3432564e,
				/// <summary>
				/// Non-subsampled Cb:Cr plane
				/// </summary>
				Nv42 = 0x3234564e,
				/// <summary>
				/// 2x1 subsampled Cr:Cb plane, 10 bit per channel
				/// </summary>
				P210 = 0x30313250,
				/// <summary>
				/// 2x2 subsampled Cr:Cb plane 10 bits per channel
				/// </summary>
				P010 = 0x30313050,
				/// <summary>
				/// 2x2 subsampled Cr:Cb plane 12 bits per channel
				/// </summary>
				P012 = 0x32313050,
				/// <summary>
				/// 2x2 subsampled Cr:Cb plane 16 bits per channel
				/// </summary>
				P016 = 0x36313050,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					var fd = (IntPtr) Owner.Socket.ReadWithFd(tbuf);
					var pre_id = _offset;
					_offset += 4;
					var size = (int) Helper.ReadInt(tbuf, ref _offset);
					CreatePool(out var id, fd, size);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
			}
		}
		/// <summary>
		/// Create a shm pool
		/// </summary>
		/// <remarks>
		/// Create a new wl_shm_pool object.
		/// 
		/// The pool can be used to create shared memory based buffer
		/// objects.  The server will mmap size bytes of the passed file
		/// descriptor, to use as backing memory for the pool.
		/// </remarks>
		/// <param name="id">Pool to create</param>
		/// <param name="fd">File descriptor for the pool</param>
		/// <param name="size">Pool size, in bytes</param>
		public abstract void CreatePool(out IWlShmPool id, IntPtr fd, int size);
		/// <summary>
		/// Pixel format description
		/// </summary>
		/// <remarks>
		/// Informs the client about a valid pixel format that
		/// can be used for buffers. Known formats include
		/// argb8888 and xrgb8888.
		/// </remarks>
		/// <param name="format">Buffer pixel format</param>
		public void Format(Enum.Format format) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) format);
			SendEvent(0, tbuf);
		}
	}
	/// <summary>
	/// Content for a wl_surface
	/// </summary>
	/// <remarks>
	/// A buffer provides the content for a wl_surface. Buffers are
	/// created through factory interfaces such as wl_drm, wl_shm or
	/// similar. It has a width and a height and can be attached to a
	/// wl_surface, but the mechanism by which a client provides and
	/// updates the contents is defined by the buffer factory interface.
	/// </remarks>
	public abstract class IWlBuffer : IWaylandObject {
		protected IWlBuffer(Client owner, uint? id) : base(owner, id) {}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
			}
		}
		/// <summary>
		/// Destroy a buffer
		/// </summary>
		/// <remarks>
		/// Destroy a buffer. If and how you need to release the backing
		/// storage is defined by the buffer factory interface.
		/// 
		/// For possible side-effects to a surface, see wl_surface.attach.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// Compositor releases buffer
		/// </summary>
		/// <remarks>
		/// Sent when this wl_buffer is no longer used by the compositor.
		/// The client is now free to reuse or destroy this buffer and its
		/// backing storage.
		/// 
		/// If a client receives a release event before the frame callback
		/// requested in the same wl_surface.commit that attaches this
		/// wl_buffer to a surface, then the client is immediately free to
		/// reuse the buffer and its backing storage, and does not need a
		/// second buffer for the next surface content update. Typically
		/// this is possible, when the compositor maintains a copy of the
		/// wl_surface contents, e.g. as a GL texture. This is an important
		/// optimization for GL(ES) compositors with wl_shm clients.
		/// </remarks>
		public void Release() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(0, tbuf);
		}
	}
	/// <summary>
	/// Offer to transfer data
	/// </summary>
	/// <remarks>
	/// A wl_data_offer represents a piece of data offered for transfer
	/// by another client (the source client).  It is used by the
	/// copy-and-paste and drag-and-drop mechanisms.  The offer
	/// describes the different mime types that the data can be
	/// converted to and provides the mechanism for transferring the
	/// data directly from the source client.
	/// </remarks>
	public abstract class IWlDataOffer : IWaylandObject {
		protected IWlDataOffer(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			public enum Error {
				/// <summary>
				/// Finish request was called untimely
				/// </summary>
				InvalidFinish = 0,
				/// <summary>
				/// Action mask contains invalid values
				/// </summary>
				InvalidActionMask = 1,
				/// <summary>
				/// Action argument has an invalid value
				/// </summary>
				InvalidAction = 2,
				/// <summary>
				/// Offer doesn't accept this request
				/// </summary>
				InvalidOffer = 3,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					var mime_type = Helper.ReadString(tbuf, ref _offset);
					Accept(serial, mime_type);
					break;
				}
				case 1: {
					var fd = (IntPtr) Owner.Socket.ReadWithFd(tbuf);
					var mime_type = Helper.ReadString(tbuf, ref _offset);
					Receive(mime_type, fd);
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
				case 3: {
					Owner.Socket.Read(tbuf);
					Finish();
					break;
				}
				case 4: {
					Owner.Socket.Read(tbuf);
					var dnd_actions = (uint) Helper.ReadUint(tbuf, ref _offset);
					var preferred_action = (uint) Helper.ReadUint(tbuf, ref _offset);
					SetActions(dnd_actions, preferred_action);
					break;
				}
			}
		}
		/// <summary>
		/// Accept one of the offered mime types
		/// </summary>
		/// <remarks>
		/// Indicate that the client can accept the given mime type, or
		/// NULL for not accepted.
		/// 
		/// For objects of version 2 or older, this request is used by the
		/// client to give feedback whether the client can receive the given
		/// mime type, or NULL if none is accepted; the feedback does not
		/// determine whether the drag-and-drop operation succeeds or not.
		/// 
		/// For objects of version 3 or newer, this request determines the
		/// final result of the drag-and-drop operation. If the end result
		/// is that no mime types were accepted, the drag-and-drop operation
		/// will be cancelled and the corresponding drag source will receive
		/// wl_data_source.cancelled. Clients may still use this event in
		/// conjunction with wl_data_source.action for feedback.
		/// </remarks>
		/// <param name="serial">Serial number of the accept request</param>
		/// <param name="mime_type">Mime type accepted by the client</param>
		public abstract void Accept(uint serial, string mime_type);
		/// <summary>
		/// Request that the data is transferred
		/// </summary>
		/// <remarks>
		/// To transfer the offered data, the client issues this request
		/// and indicates the mime type it wants to receive.  The transfer
		/// happens through the passed file descriptor (typically created
		/// with the pipe system call).  The source client writes the data
		/// in the mime type representation requested and then closes the
		/// file descriptor.
		/// 
		/// The receiving client reads from the read end of the pipe until
		/// EOF and then closes its end, at which point the transfer is
		/// complete.
		/// 
		/// This request may happen multiple times for different mime types,
		/// both before and after wl_data_device.drop. Drag-and-drop destination
		/// clients may preemptively fetch data or examine it more closely to
		/// determine acceptance.
		/// </remarks>
		/// <param name="mime_type">Mime type desired by receiver</param>
		/// <param name="fd">File descriptor for data transfer</param>
		public abstract void Receive(string mime_type, IntPtr fd);
		/// <summary>
		/// Destroy data offer
		/// </summary>
		/// <remarks>
		/// Destroy the data offer.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// The offer will no longer be used
		/// </summary>
		/// <remarks>
		/// Notifies the compositor that the drag destination successfully
		/// finished the drag-and-drop operation.
		/// 
		/// Upon receiving this request, the compositor will emit
		/// wl_data_source.dnd_finished on the drag source client.
		/// 
		/// It is a client error to perform other requests than
		/// wl_data_offer.destroy after this one. It is also an error to perform
		/// this request after a NULL mime type has been set in
		/// wl_data_offer.accept or no action was received through
		/// wl_data_offer.action.
		/// 
		/// If wl_data_offer.finish request is received for a non drag and drop
		/// operation, the invalid_finish protocol error is raised.
		/// </remarks>
		public abstract void Finish();
		/// <summary>
		/// Set the available/preferred drag-and-drop actions
		/// </summary>
		/// <remarks>
		/// Sets the actions that the destination side client supports for
		/// this operation. This request may trigger the emission of
		/// wl_data_source.action and wl_data_offer.action events if the compositor
		/// needs to change the selected action.
		/// 
		/// This request can be called multiple times throughout the
		/// drag-and-drop operation, typically in response to wl_data_device.enter
		/// or wl_data_device.motion events.
		/// 
		/// This request determines the final result of the drag-and-drop
		/// operation. If the end result is that no action is accepted,
		/// the drag source will receive wl_drag_source.cancelled.
		/// 
		/// The dnd_actions argument must contain only values expressed in the
		/// wl_data_device_manager.dnd_actions enum, and the preferred_action
		/// argument must only contain one of those values set, otherwise it
		/// will result in a protocol error.
		/// 
		/// While managing an "ask" action, the destination drag-and-drop client
		/// may perform further wl_data_offer.receive requests, and is expected
		/// to perform one last wl_data_offer.set_actions request with a preferred
		/// action other than "ask" (and optionally wl_data_offer.accept) before
		/// requesting wl_data_offer.finish, in order to convey the action selected
		/// by the user. If the preferred action is not in the
		/// wl_data_offer.source_actions mask, an error will be raised.
		/// 
		/// If the "ask" action is dismissed (e.g. user cancellation), the client
		/// is expected to perform wl_data_offer.destroy right away.
		/// 
		/// This request can only be made on drag-and-drop offers, a protocol error
		/// will be raised otherwise.
		/// </remarks>
		/// <param name="dnd_actions">Actions supported by the destination client</param>
		/// <param name="preferred_action">Action preferred by the destination client</param>
		public abstract void SetActions(uint dnd_actions, uint preferred_action);
		/// <summary>
		/// Advertise offered mime type
		/// </summary>
		/// <remarks>
		/// Sent immediately after creating the wl_data_offer object.  One
		/// event per offered mime type.
		/// </remarks>
		/// <param name="mime_type">Offered mime type</param>
		public void Offer(string mime_type) {
			var _offset = 0;
			_offset += Helper.StringSize(mime_type);
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteString(tbuf, ref _offset, mime_type);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Notify the source-side available actions
		/// </summary>
		/// <remarks>
		/// This event indicates the actions offered by the data source. It
		/// will be sent right after wl_data_device.enter, or anytime the source
		/// side changes its offered actions through wl_data_source.set_actions.
		/// </remarks>
		/// <param name="source_actions">Actions offered by the data source</param>
		public void SourceActions(uint source_actions) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) source_actions);
			SendEvent(1, tbuf);
		}
		/// <summary>
		/// Notify the selected action
		/// </summary>
		/// <remarks>
		/// This event indicates the action selected by the compositor after
		/// matching the source/destination side actions. Only one action (or
		/// none) will be offered here.
		/// 
		/// This event can be emitted multiple times during the drag-and-drop
		/// operation in response to destination side action changes through
		/// wl_data_offer.set_actions.
		/// 
		/// This event will no longer be emitted after wl_data_device.drop
		/// happened on the drag-and-drop destination, the client must
		/// honor the last action received, or the last preferred one set
		/// through wl_data_offer.set_actions when handling an "ask" action.
		/// 
		/// Compositors may also change the selected action on the fly, mainly
		/// in response to keyboard modifier changes during the drag-and-drop
		/// operation.
		/// 
		/// The most recent action received is always the valid one. Prior to
		/// receiving wl_data_device.drop, the chosen action may change (e.g.
		/// due to keyboard modifiers being pressed). At the time of receiving
		/// wl_data_device.drop the drag-and-drop destination must honor the
		/// last action received.
		/// 
		/// Action changes may still happen after wl_data_device.drop,
		/// especially on "ask" actions, where the drag-and-drop destination
		/// may choose another action afterwards. Action changes happening
		/// at this stage are always the result of inter-client negotiation, the
		/// compositor shall no longer be able to induce a different action.
		/// 
		/// Upon "ask" actions, it is expected that the drag-and-drop destination
		/// may potentially choose a different action and/or mime type,
		/// based on wl_data_offer.source_actions and finally chosen by the
		/// user (e.g. popping up a menu with the available options). The
		/// final wl_data_offer.set_actions and wl_data_offer.accept requests
		/// must happen before the call to wl_data_offer.finish.
		/// </remarks>
		/// <param name="dnd_action">Action selected by the compositor</param>
		public void Action(uint dnd_action) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) dnd_action);
			SendEvent(2, tbuf);
		}
	}
	/// <summary>
	/// Offer to transfer data
	/// </summary>
	/// <remarks>
	/// The wl_data_source object is the source side of a wl_data_offer.
	/// It is created by the source client in a data transfer and
	/// provides a way to describe the offered data and a way to respond
	/// to requests to transfer the data.
	/// </remarks>
	public abstract class IWlDataSource : IWaylandObject {
		protected IWlDataSource(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			public enum Error {
				/// <summary>
				/// Action mask contains invalid values
				/// </summary>
				InvalidActionMask = 0,
				/// <summary>
				/// Source doesn't accept this request
				/// </summary>
				InvalidSource = 1,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var mime_type = Helper.ReadString(tbuf, ref _offset);
					Offer(mime_type);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					var dnd_actions = (uint) Helper.ReadUint(tbuf, ref _offset);
					SetActions(dnd_actions);
					break;
				}
			}
		}
		/// <summary>
		/// Add an offered mime type
		/// </summary>
		/// <remarks>
		/// This request adds a mime type to the set of mime types
		/// advertised to targets.  Can be called several times to offer
		/// multiple types.
		/// </remarks>
		/// <param name="mime_type">Mime type offered by the data source</param>
		public abstract void Offer(string mime_type);
		/// <summary>
		/// Destroy the data source
		/// </summary>
		/// <remarks>
		/// Destroy the data source.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// Set the available drag-and-drop actions
		/// </summary>
		/// <remarks>
		/// Sets the actions that the source side client supports for this
		/// operation. This request may trigger wl_data_source.action and
		/// wl_data_offer.action events if the compositor needs to change the
		/// selected action.
		/// 
		/// The dnd_actions argument must contain only values expressed in the
		/// wl_data_device_manager.dnd_actions enum, otherwise it will result
		/// in a protocol error.
		/// 
		/// This request must be made once only, and can only be made on sources
		/// used in drag-and-drop, so it must be performed before
		/// wl_data_device.start_drag. Attempting to use the source other than
		/// for drag-and-drop will raise a protocol error.
		/// </remarks>
		/// <param name="dnd_actions">Actions supported by the data source</param>
		public abstract void SetActions(uint dnd_actions);
		/// <summary>
		/// A target accepts an offered mime type
		/// </summary>
		/// <remarks>
		/// Sent when a target accepts pointer_focus or motion events.  If
		/// a target does not accept any of the offered types, type is NULL.
		/// 
		/// Used for feedback during drag-and-drop.
		/// </remarks>
		/// <param name="mime_type">Mime type accepted by the target</param>
		public void Target(string mime_type) {
			var _offset = 0;
			_offset += Helper.StringSize(mime_type);
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteString(tbuf, ref _offset, mime_type);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Send the data
		/// </summary>
		/// <remarks>
		/// Request for data from the client.  Send the data as the
		/// specified mime type over the passed file descriptor, then
		/// close it.
		/// </remarks>
		/// <param name="mime_type">Mime type for the data</param>
		/// <param name="fd">File descriptor for the data</param>
		public void Send(string mime_type, IntPtr fd) {
			var _offset = 0;
			_offset += Helper.StringSize(mime_type);
			var _fd = (int) fd;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteString(tbuf, ref _offset, mime_type);
			SendEventWithFd(1, tbuf, _fd);
		}
		/// <summary>
		/// Selection was cancelled
		/// </summary>
		/// <remarks>
		/// This data source is no longer valid. There are several reasons why
		/// this could happen:
		/// 
		/// - The data source has been replaced by another data source.
		/// - The drag-and-drop operation was performed, but the drop destination
		/// did not accept any of the mime types offered through
		/// wl_data_source.target.
		/// - The drag-and-drop operation was performed, but the drop destination
		/// did not select any of the actions present in the mask offered through
		/// wl_data_source.action.
		/// - The drag-and-drop operation was performed but didn't happen over a
		/// surface.
		/// - The compositor cancelled the drag-and-drop operation (e.g. compositor
		/// dependent timeouts to avoid stale drag-and-drop transfers).
		/// 
		/// The client should clean up and destroy this data source.
		/// 
		/// For objects of version 2 or older, wl_data_source.cancelled will
		/// only be emitted if the data source was replaced by another data
		/// source.
		/// </remarks>
		public void Cancelled() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(2, tbuf);
		}
		/// <summary>
		/// The drag-and-drop operation physically finished
		/// </summary>
		/// <remarks>
		/// The user performed the drop action. This event does not indicate
		/// acceptance, wl_data_source.cancelled may still be emitted afterwards
		/// if the drop destination does not accept any mime type.
		/// 
		/// However, this event might however not be received if the compositor
		/// cancelled the drag-and-drop operation before this event could happen.
		/// 
		/// Note that the data_source may still be used in the future and should
		/// not be destroyed here.
		/// </remarks>
		public void DndDropPerformed() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(3, tbuf);
		}
		/// <summary>
		/// The drag-and-drop operation concluded
		/// </summary>
		/// <remarks>
		/// The drop destination finished interoperating with this data
		/// source, so the client is now free to destroy this data source and
		/// free all associated data.
		/// 
		/// If the action used to perform the operation was "move", the
		/// source can now delete the transferred data.
		/// </remarks>
		public void DndFinished() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(4, tbuf);
		}
		/// <summary>
		/// Notify the selected action
		/// </summary>
		/// <remarks>
		/// This event indicates the action selected by the compositor after
		/// matching the source/destination side actions. Only one action (or
		/// none) will be offered here.
		/// 
		/// This event can be emitted multiple times during the drag-and-drop
		/// operation, mainly in response to destination side changes through
		/// wl_data_offer.set_actions, and as the data device enters/leaves
		/// surfaces.
		/// 
		/// It is only possible to receive this event after
		/// wl_data_source.dnd_drop_performed if the drag-and-drop operation
		/// ended in an "ask" action, in which case the final wl_data_source.action
		/// event will happen immediately before wl_data_source.dnd_finished.
		/// 
		/// Compositors may also change the selected action on the fly, mainly
		/// in response to keyboard modifier changes during the drag-and-drop
		/// operation.
		/// 
		/// The most recent action received is always the valid one. The chosen
		/// action may change alongside negotiation (e.g. an "ask" action can turn
		/// into a "move" operation), so the effects of the final action must
		/// always be applied in wl_data_offer.dnd_finished.
		/// 
		/// Clients can trigger cursor surface changes from this point, so
		/// they reflect the current action.
		/// </remarks>
		/// <param name="dnd_action">Action selected by the compositor</param>
		public void Action(uint dnd_action) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) dnd_action);
			SendEvent(5, tbuf);
		}
	}
	/// <summary>
	/// Data transfer device
	/// </summary>
	/// <remarks>
	/// There is one wl_data_device per seat which can be obtained
	/// from the global wl_data_device_manager singleton.
	/// 
	/// A wl_data_device provides access to inter-client data transfer
	/// mechanisms such as copy-and-paste and drag-and-drop.
	/// </remarks>
	public abstract class IWlDataDevice : IWaylandObject {
		protected IWlDataDevice(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			public enum Error {
				/// <summary>
				/// Given wl_surface has another role
				/// </summary>
				Role = 0,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var source = Owner.GetObject<IWlDataSource>(Helper.ReadUint(tbuf, ref _offset));
					var origin = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					var icon = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					StartDrag(source, origin, icon, serial);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var source = Owner.GetObject<IWlDataSource>(Helper.ReadUint(tbuf, ref _offset));
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					SetSelection(source, serial);
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					Release();
					break;
				}
			}
		}
		/// <summary>
		/// Start drag-and-drop operation
		/// </summary>
		/// <remarks>
		/// This request asks the compositor to start a drag-and-drop
		/// operation on behalf of the client.
		/// 
		/// The source argument is the data source that provides the data
		/// for the eventual data transfer. If source is NULL, enter, leave
		/// and motion events are sent only to the client that initiated the
		/// drag and the client is expected to handle the data passing
		/// internally.
		/// 
		/// The origin surface is the surface where the drag originates and
		/// the client must have an active implicit grab that matches the
		/// serial.
		/// 
		/// The icon surface is an optional (can be NULL) surface that
		/// provides an icon to be moved around with the cursor.  Initially,
		/// the top-left corner of the icon surface is placed at the cursor
		/// hotspot, but subsequent wl_surface.attach request can move the
		/// relative position. Attach requests must be confirmed with
		/// wl_surface.commit as usual. The icon surface is given the role of
		/// a drag-and-drop icon. If the icon surface already has another role,
		/// it raises a protocol error.
		/// 
		/// The current and pending input regions of the icon wl_surface are
		/// cleared, and wl_surface.set_input_region is ignored until the
		/// wl_surface is no longer used as the icon surface. When the use
		/// as an icon ends, the current and pending input regions become
		/// undefined, and the wl_surface is unmapped.
		/// </remarks>
		/// <param name="source">Data source for the eventual transfer</param>
		/// <param name="origin">Surface where the drag originates</param>
		/// <param name="icon">Drag-and-drop icon surface</param>
		/// <param name="serial">Serial number of the implicit grab on the origin</param>
		public abstract void StartDrag(IWlDataSource source, IWlSurface origin, IWlSurface icon, uint serial);
		/// <summary>
		/// Copy data to the selection
		/// </summary>
		/// <remarks>
		/// This request asks the compositor to set the selection
		/// to the data from the source on behalf of the client.
		/// 
		/// To unset the selection, set the source to NULL.
		/// </remarks>
		/// <param name="source">Data source for the selection</param>
		/// <param name="serial">Serial number of the event that triggered this request</param>
		public abstract void SetSelection(IWlDataSource source, uint serial);
		/// <summary>
		/// Destroy data device
		/// </summary>
		/// <remarks>
		/// This request destroys the data device.
		/// </remarks>
		public abstract void Release();
		/// <summary>
		/// Introduce a new wl_data_offer
		/// </summary>
		/// <remarks>
		/// The data_offer event introduces a new wl_data_offer object,
		/// which will subsequently be used in either the
		/// data_device.enter event (for drag-and-drop) or the
		/// data_device.selection event (for selections).  Immediately
		/// following the data_device_data_offer event, the new data_offer
		/// object will send out data_offer.offer events to describe the
		/// mime types it offers.
		/// </remarks>
		/// <param name="id">The new data_offer object</param>
		public void DataOffer(IWlDataOffer id) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, id.Id);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Initiate drag-and-drop session
		/// </summary>
		/// <remarks>
		/// This event is sent when an active drag-and-drop pointer enters
		/// a surface owned by the client.  The position of the pointer at
		/// enter time is provided by the x and y arguments, in surface-local
		/// coordinates.
		/// </remarks>
		/// <param name="serial">Serial number of the enter event</param>
		/// <param name="surface">Client surface entered</param>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		/// <param name="id">Source data_offer object</param>
		public void Enter(uint serial, IWlSurface surface, Fixed x, Fixed y, IWlDataOffer id) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, surface.Id);
			Helper.WriteUint(tbuf, ref _offset, x.UintValue);
			Helper.WriteUint(tbuf, ref _offset, y.UintValue);
			Helper.WriteUint(tbuf, ref _offset, id.Id);
			SendEvent(1, tbuf);
		}
		/// <summary>
		/// End drag-and-drop session
		/// </summary>
		/// <remarks>
		/// This event is sent when the drag-and-drop pointer leaves the
		/// surface and the session ends.  The client must destroy the
		/// wl_data_offer introduced at enter time at this point.
		/// </remarks>
		public void Leave() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(2, tbuf);
		}
		/// <summary>
		/// Drag-and-drop session motion
		/// </summary>
		/// <remarks>
		/// This event is sent when the drag-and-drop pointer moves within
		/// the currently focused surface. The new position of the pointer
		/// is provided by the x and y arguments, in surface-local
		/// coordinates.
		/// </remarks>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		public void Motion(uint time, Fixed x, Fixed y) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteUint(tbuf, ref _offset, x.UintValue);
			Helper.WriteUint(tbuf, ref _offset, y.UintValue);
			SendEvent(3, tbuf);
		}
		/// <summary>
		/// End drag-and-drop session successfully
		/// </summary>
		/// <remarks>
		/// The event is sent when a drag-and-drop operation is ended
		/// because the implicit grab is removed.
		/// 
		/// The drag-and-drop destination is expected to honor the last action
		/// received through wl_data_offer.action, if the resulting action is
		/// "copy" or "move", the destination can still perform
		/// wl_data_offer.receive requests, and is expected to end all
		/// transfers with a wl_data_offer.finish request.
		/// 
		/// If the resulting action is "ask", the action will not be considered
		/// final. The drag-and-drop destination is expected to perform one last
		/// wl_data_offer.set_actions request, or wl_data_offer.destroy in order
		/// to cancel the operation.
		/// </remarks>
		public void Drop() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(4, tbuf);
		}
		/// <summary>
		/// Advertise new selection
		/// </summary>
		/// <remarks>
		/// The selection event is sent out to notify the client of a new
		/// wl_data_offer for the selection for this device.  The
		/// data_device.data_offer and the data_offer.offer events are
		/// sent out immediately before this event to introduce the data
		/// offer object.  The selection event is sent to a client
		/// immediately before receiving keyboard focus and when a new
		/// selection is set while the client has keyboard focus.  The
		/// data_offer is valid until a new data_offer or NULL is received
		/// or until the client loses keyboard focus.  The client must
		/// destroy the previous selection data_offer, if any, upon receiving
		/// this event.
		/// </remarks>
		/// <param name="id">Selection data_offer object</param>
		public void Selection(IWlDataOffer id) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, id.Id);
			SendEvent(5, tbuf);
		}
	}
	/// <summary>
	/// Data transfer interface
	/// </summary>
	/// <remarks>
	/// The wl_data_device_manager is a singleton global object that
	/// provides access to inter-client data transfer mechanisms such as
	/// copy-and-paste and drag-and-drop.  These mechanisms are tied to
	/// a wl_seat and this interface lets a client get a wl_data_device
	/// corresponding to a wl_seat.
	/// 
	/// Depending on the version bound, the objects created from the bound
	/// wl_data_device_manager object will have different requirements for
	/// functioning properly. See wl_data_source.set_actions,
	/// wl_data_offer.accept and wl_data_offer.finish for details.
	/// </remarks>
	public abstract class IWlDataDeviceManager : IWaylandObject {
		protected IWlDataDeviceManager(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Drag and drop actions
			/// </summary>
			/// <remarks>
			/// This is a bitmask of the available/preferred actions in a
			/// drag-and-drop operation.
			/// 
			/// In the compositor, the selected action is a result of matching the
			/// actions offered by the source and destination sides.  "action" events
			/// with a "none" action will be sent to both source and destination if
			/// there is no match. All further checks will effectively happen on
			/// (source actions ∩ destination actions).
			/// 
			/// In addition, compositors may also pick different actions in
			/// reaction to key modifiers being pressed. One common design that
			/// is used in major toolkits (and the behavior recommended for
			/// compositors) is:
			/// 
			/// - If no modifiers are pressed, the first match (in bit order)
			/// will be used.
			/// - Pressing Shift selects "move", if enabled in the mask.
			/// - Pressing Control selects "copy", if enabled in the mask.
			/// 
			/// Behavior beyond that is considered implementation-dependent.
			/// Compositors may for example bind other modifiers (like Alt/Meta)
			/// or drags initiated with other buttons than BTN_LEFT to specific
			/// actions (e.g. "ask").
			/// </remarks>
			[Flags]
			public enum DndAction : uint {
				/// <summary>
				/// No action
				/// </summary>
				None = 0,
				/// <summary>
				/// Copy action
				/// </summary>
				Copy = 1,
				/// <summary>
				/// Move action
				/// </summary>
				Move = 2,
				/// <summary>
				/// Ask action
				/// </summary>
				Ask = 4,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					CreateDataSource(out var id);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					var seat = Owner.GetObject<IWlSeat>(Helper.ReadUint(tbuf, ref _offset));
					GetDataDevice(out var id, seat);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
			}
		}
		/// <summary>
		/// Create a new data source
		/// </summary>
		/// <remarks>
		/// Create a new data source.
		/// </remarks>
		/// <param name="id">Data source to create</param>
		public abstract void CreateDataSource(out IWlDataSource id);
		/// <summary>
		/// Create a new data device
		/// </summary>
		/// <remarks>
		/// Create a new data device for a given seat.
		/// </remarks>
		/// <param name="id">Data device to create</param>
		/// <param name="seat">Seat associated with the data device</param>
		public abstract void GetDataDevice(out IWlDataDevice id, IWlSeat seat);
	}
	/// <summary>
	/// Create desktop-style surfaces
	/// </summary>
	/// <remarks>
	/// This interface is implemented by servers that provide
	/// desktop-style user interfaces.
	/// 
	/// It allows clients to associate a wl_shell_surface with
	/// a basic surface.
	/// 
	/// Note! This protocol is deprecated and not intended for production use.
	/// For desktop-style user interfaces, use xdg_shell.
	/// </remarks>
	public abstract class IWlShell : IWaylandObject {
		protected IWlShell(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			public enum Error {
				/// <summary>
				/// Given wl_surface has another role
				/// </summary>
				Role = 0,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					var surface = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					GetShellSurface(out var id, surface);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
			}
		}
		/// <summary>
		/// Create a shell surface from a surface
		/// </summary>
		/// <remarks>
		/// Create a shell surface for an existing surface. This gives
		/// the wl_surface the role of a shell surface. If the wl_surface
		/// already has another role, it raises a protocol error.
		/// 
		/// Only one shell surface can be associated with a given surface.
		/// </remarks>
		/// <param name="id">Shell surface to create</param>
		/// <param name="surface">Surface to be given the shell surface role</param>
		public abstract void GetShellSurface(out IWlShellSurface id, IWlSurface surface);
	}
	/// <summary>
	/// Desktop-style metadata interface
	/// </summary>
	/// <remarks>
	/// An interface that may be implemented by a wl_surface, for
	/// implementations that provide a desktop-style user interface.
	/// 
	/// It provides requests to treat surfaces like toplevel, fullscreen
	/// or popup windows, move, resize or maximize them, associate
	/// metadata like title and class, etc.
	/// 
	/// On the server side the object is automatically destroyed when
	/// the related wl_surface is destroyed. On the client side,
	/// wl_shell_surface_destroy() must be called before destroying
	/// the wl_surface object.
	/// </remarks>
	public abstract class IWlShellSurface : IWaylandObject {
		protected IWlShellSurface(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Edge values for resizing
			/// </summary>
			/// <remarks>
			/// These values are used to indicate which edge of a surface
			/// is being dragged in a resize operation. The server may
			/// use this information to adapt its behavior, e.g. choose
			/// an appropriate cursor image.
			/// </remarks>
			[Flags]
			public enum Resize : uint {
				/// <summary>
				/// No edge
				/// </summary>
				None = 0,
				/// <summary>
				/// Top edge
				/// </summary>
				Top = 1,
				/// <summary>
				/// Bottom edge
				/// </summary>
				Bottom = 2,
				/// <summary>
				/// Left edge
				/// </summary>
				Left = 4,
				/// <summary>
				/// Top and left edges
				/// </summary>
				TopLeft = 5,
				/// <summary>
				/// Bottom and left edges
				/// </summary>
				BottomLeft = 6,
				/// <summary>
				/// Right edge
				/// </summary>
				Right = 8,
				/// <summary>
				/// Top and right edges
				/// </summary>
				TopRight = 9,
				/// <summary>
				/// Bottom and right edges
				/// </summary>
				BottomRight = 10,
			}
			/// <summary>
			/// Details of transient behaviour
			/// </summary>
			/// <remarks>
			/// These flags specify details of the expected behaviour
			/// of transient surfaces. Used in the set_transient request.
			/// </remarks>
			[Flags]
			public enum Transient : uint {
				/// <summary>
				/// Do not set keyboard focus
				/// </summary>
				Inactive = 0x1,
			}
			/// <summary>
			/// Different method to set the surface fullscreen
			/// </summary>
			/// <remarks>
			/// Hints to indicate to the compositor how to deal with a conflict
			/// between the dimensions of the surface and the dimensions of the
			/// output. The compositor is free to ignore this parameter.
			/// </remarks>
			public enum FullscreenMethod {
				/// <summary>
				/// No preference, apply default policy
				/// </summary>
				Default = 0,
				/// <summary>
				/// Scale, preserve the surface's aspect ratio and center on output
				/// </summary>
				Scale = 1,
				/// <summary>
				/// Switch output mode to the smallest mode that can fit the surface, add black borders to compensate size mismatch
				/// </summary>
				Driver = 2,
				/// <summary>
				/// No upscaling, center on output and add black borders to compensate size mismatch
				/// </summary>
				Fill = 3,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					Pong(serial);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var seat = Owner.GetObject<IWlSeat>(Helper.ReadUint(tbuf, ref _offset));
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					Move(seat, serial);
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					var seat = Owner.GetObject<IWlSeat>(Helper.ReadUint(tbuf, ref _offset));
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					var edges = (Enum.Resize) Helper.ReadUint(tbuf, ref _offset);
					Resize(seat, serial, edges);
					break;
				}
				case 3: {
					Owner.Socket.Read(tbuf);
					SetToplevel();
					break;
				}
				case 4: {
					Owner.Socket.Read(tbuf);
					var parent = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					var flags = (Enum.Transient) Helper.ReadUint(tbuf, ref _offset);
					SetTransient(parent, x, y, flags);
					break;
				}
				case 5: {
					Owner.Socket.Read(tbuf);
					var method = (Enum.FullscreenMethod) Helper.ReadUint(tbuf, ref _offset);
					var framerate = (uint) Helper.ReadUint(tbuf, ref _offset);
					var output = Owner.GetObject<IWlOutput>(Helper.ReadUint(tbuf, ref _offset));
					SetFullscreen(method, framerate, output);
					break;
				}
				case 6: {
					Owner.Socket.Read(tbuf);
					var seat = Owner.GetObject<IWlSeat>(Helper.ReadUint(tbuf, ref _offset));
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					var parent = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					var flags = (Enum.Transient) Helper.ReadUint(tbuf, ref _offset);
					SetPopup(seat, serial, parent, x, y, flags);
					break;
				}
				case 7: {
					Owner.Socket.Read(tbuf);
					var output = Owner.GetObject<IWlOutput>(Helper.ReadUint(tbuf, ref _offset));
					SetMaximized(output);
					break;
				}
				case 8: {
					Owner.Socket.Read(tbuf);
					var title = Helper.ReadString(tbuf, ref _offset);
					SetTitle(title);
					break;
				}
				case 9: {
					Owner.Socket.Read(tbuf);
					var class_ = Helper.ReadString(tbuf, ref _offset);
					SetClass(class_);
					break;
				}
			}
		}
		/// <summary>
		/// Respond to a ping event
		/// </summary>
		/// <remarks>
		/// A client must respond to a ping event with a pong request or
		/// the client may be deemed unresponsive.
		/// </remarks>
		/// <param name="serial">Serial number of the ping event</param>
		public abstract void Pong(uint serial);
		/// <summary>
		/// Start an interactive move
		/// </summary>
		/// <remarks>
		/// Start a pointer-driven move of the surface.
		/// 
		/// This request must be used in response to a button press event.
		/// The server may ignore move requests depending on the state of
		/// the surface (e.g. fullscreen or maximized).
		/// </remarks>
		/// <param name="seat">Seat whose pointer is used</param>
		/// <param name="serial">Serial number of the implicit grab on the pointer</param>
		public abstract void Move(IWlSeat seat, uint serial);
		/// <summary>
		/// Start an interactive resize
		/// </summary>
		/// <remarks>
		/// Start a pointer-driven resizing of the surface.
		/// 
		/// This request must be used in response to a button press event.
		/// The server may ignore resize requests depending on the state of
		/// the surface (e.g. fullscreen or maximized).
		/// </remarks>
		/// <param name="seat">Seat whose pointer is used</param>
		/// <param name="serial">Serial number of the implicit grab on the pointer</param>
		/// <param name="edges">Which edge or corner is being dragged</param>
		public abstract void Resize(IWlSeat seat, uint serial, Enum.Resize edges);
		/// <summary>
		/// Make the surface a toplevel surface
		/// </summary>
		/// <remarks>
		/// Map the surface as a toplevel surface.
		/// 
		/// A toplevel surface is not fullscreen, maximized or transient.
		/// </remarks>
		public abstract void SetToplevel();
		/// <summary>
		/// Make the surface a transient surface
		/// </summary>
		/// <remarks>
		/// Map the surface relative to an existing surface.
		/// 
		/// The x and y arguments specify the location of the upper left
		/// corner of the surface relative to the upper left corner of the
		/// parent surface, in surface-local coordinates.
		/// 
		/// The flags argument controls details of the transient behaviour.
		/// </remarks>
		/// <param name="parent">Parent surface</param>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		/// <param name="flags">Transient surface behavior</param>
		public abstract void SetTransient(IWlSurface parent, int x, int y, Enum.Transient flags);
		/// <summary>
		/// Make the surface a fullscreen surface
		/// </summary>
		/// <remarks>
		/// Map the surface as a fullscreen surface.
		/// 
		/// If an output parameter is given then the surface will be made
		/// fullscreen on that output. If the client does not specify the
		/// output then the compositor will apply its policy - usually
		/// choosing the output on which the surface has the biggest surface
		/// area.
		/// 
		/// The client may specify a method to resolve a size conflict
		/// between the output size and the surface size - this is provided
		/// through the method parameter.
		/// 
		/// The framerate parameter is used only when the method is set
		/// to "driver", to indicate the preferred framerate. A value of 0
		/// indicates that the client does not care about framerate.  The
		/// framerate is specified in mHz, that is framerate of 60000 is 60Hz.
		/// 
		/// A method of "scale" or "driver" implies a scaling operation of
		/// the surface, either via a direct scaling operation or a change of
		/// the output mode. This will override any kind of output scaling, so
		/// that mapping a surface with a buffer size equal to the mode can
		/// fill the screen independent of buffer_scale.
		/// 
		/// A method of "fill" means we don't scale up the buffer, however
		/// any output scale is applied. This means that you may run into
		/// an edge case where the application maps a buffer with the same
		/// size of the output mode but buffer_scale 1 (thus making a
		/// surface larger than the output). In this case it is allowed to
		/// downscale the results to fit the screen.
		/// 
		/// The compositor must reply to this request with a configure event
		/// with the dimensions for the output on which the surface will
		/// be made fullscreen.
		/// </remarks>
		/// <param name="method">Method for resolving size conflict</param>
		/// <param name="framerate">Framerate in mHz</param>
		/// <param name="output">Output on which the surface is to be fullscreen</param>
		public abstract void SetFullscreen(Enum.FullscreenMethod method, uint framerate, IWlOutput output);
		/// <summary>
		/// Make the surface a popup surface
		/// </summary>
		/// <remarks>
		/// Map the surface as a popup.
		/// 
		/// A popup surface is a transient surface with an added pointer
		/// grab.
		/// 
		/// An existing implicit grab will be changed to owner-events mode,
		/// and the popup grab will continue after the implicit grab ends
		/// (i.e. releasing the mouse button does not cause the popup to
		/// be unmapped).
		/// 
		/// The popup grab continues until the window is destroyed or a
		/// mouse button is pressed in any other client's window. A click
		/// in any of the client's surfaces is reported as normal, however,
		/// clicks in other clients' surfaces will be discarded and trigger
		/// the callback.
		/// 
		/// The x and y arguments specify the location of the upper left
		/// corner of the surface relative to the upper left corner of the
		/// parent surface, in surface-local coordinates.
		/// </remarks>
		/// <param name="seat">Seat whose pointer is used</param>
		/// <param name="serial">Serial number of the implicit grab on the pointer</param>
		/// <param name="parent">Parent surface</param>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		/// <param name="flags">Transient surface behavior</param>
		public abstract void SetPopup(IWlSeat seat, uint serial, IWlSurface parent, int x, int y, Enum.Transient flags);
		/// <summary>
		/// Make the surface a maximized surface
		/// </summary>
		/// <remarks>
		/// Map the surface as a maximized surface.
		/// 
		/// If an output parameter is given then the surface will be
		/// maximized on that output. If the client does not specify the
		/// output then the compositor will apply its policy - usually
		/// choosing the output on which the surface has the biggest surface
		/// area.
		/// 
		/// The compositor will reply with a configure event telling
		/// the expected new surface size. The operation is completed
		/// on the next buffer attach to this surface.
		/// 
		/// A maximized surface typically fills the entire output it is
		/// bound to, except for desktop elements such as panels. This is
		/// the main difference between a maximized shell surface and a
		/// fullscreen shell surface.
		/// 
		/// The details depend on the compositor implementation.
		/// </remarks>
		/// <param name="output">Output on which the surface is to be maximized</param>
		public abstract void SetMaximized(IWlOutput output);
		/// <summary>
		/// Set surface title
		/// </summary>
		/// <remarks>
		/// Set a short title for the surface.
		/// 
		/// This string may be used to identify the surface in a task bar,
		/// window list, or other user interface elements provided by the
		/// compositor.
		/// 
		/// The string must be encoded in UTF-8.
		/// </remarks>
		/// <param name="title">Surface title</param>
		public abstract void SetTitle(string title);
		/// <summary>
		/// Set surface class
		/// </summary>
		/// <remarks>
		/// Set a class for the surface.
		/// 
		/// The surface class identifies the general class of applications
		/// to which the surface belongs. A common convention is to use the
		/// file name (or the full path if it is a non-standard location) of
		/// the application's .desktop file as the class.
		/// </remarks>
		/// <param name="class_">Surface class</param>
		public abstract void SetClass(string class_);
		/// <summary>
		/// Ping client
		/// </summary>
		/// <remarks>
		/// Ping a client to check if it is receiving events and sending
		/// requests. A client is expected to reply with a pong request.
		/// </remarks>
		/// <param name="serial">Serial number of the ping</param>
		public void Ping(uint serial) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Suggest resize
		/// </summary>
		/// <remarks>
		/// The configure event asks the client to resize its surface.
		/// 
		/// The size is a hint, in the sense that the client is free to
		/// ignore it if it doesn't resize, pick a smaller size (to
		/// satisfy aspect ratio or resize in steps of NxM pixels).
		/// 
		/// The edges parameter provides a hint about how the surface
		/// was resized. The client may use this information to decide
		/// how to adjust its content to the new size (e.g. a scrolling
		/// area might adjust its content position to leave the viewable
		/// content unmoved).
		/// 
		/// The client is free to dismiss all but the last configure
		/// event it received.
		/// 
		/// The width and height arguments specify the size of the window
		/// in surface-local coordinates.
		/// </remarks>
		/// <param name="edges">How the surface was resized</param>
		/// <param name="width">New width of the surface</param>
		/// <param name="height">New height of the surface</param>
		public void Configure(Enum.Resize edges, int width, int height) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) edges);
			Helper.WriteInt(tbuf, ref _offset, (int) width);
			Helper.WriteInt(tbuf, ref _offset, (int) height);
			SendEvent(1, tbuf);
		}
		/// <summary>
		/// Popup interaction is done
		/// </summary>
		/// <remarks>
		/// The popup_done event is sent out when a popup grab is broken,
		/// that is, when the user clicks a surface that doesn't belong
		/// to the client owning the popup surface.
		/// </remarks>
		public void PopupDone() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(2, tbuf);
		}
	}
	/// <summary>
	/// An onscreen surface
	/// </summary>
	/// <remarks>
	/// A surface is a rectangular area that may be displayed on zero
	/// or more outputs, and shown any number of times at the compositor's
	/// discretion. They can present wl_buffers, receive user input, and
	/// define a local coordinate system.
	/// 
	/// The size of a surface (and relative positions on it) is described
	/// in surface-local coordinates, which may differ from the buffer
	/// coordinates of the pixel content, in case a buffer_transform
	/// or a buffer_scale is used.
	/// 
	/// A surface without a "role" is fairly useless: a compositor does
	/// not know where, when or how to present it. The role is the
	/// purpose of a wl_surface. Examples of roles are a cursor for a
	/// pointer (as set by wl_pointer.set_cursor), a drag icon
	/// (wl_data_device.start_drag), a sub-surface
	/// (wl_subcompositor.get_subsurface), and a window as defined by a
	/// shell protocol (e.g. wl_shell.get_shell_surface).
	/// 
	/// A surface can have only one role at a time. Initially a
	/// wl_surface does not have a role. Once a wl_surface is given a
	/// role, it is set permanently for the whole lifetime of the
	/// wl_surface object. Giving the current role again is allowed,
	/// unless explicitly forbidden by the relevant interface
	/// specification.
	/// 
	/// Surface roles are given by requests in other interfaces such as
	/// wl_pointer.set_cursor. The request should explicitly mention
	/// that this request gives a role to a wl_surface. Often, this
	/// request also creates a new protocol object that represents the
	/// role and adds additional functionality to wl_surface. When a
	/// client wants to destroy a wl_surface, they must destroy this 'role
	/// object' before the wl_surface.
	/// 
	/// Destroying the role object does not remove the role from the
	/// wl_surface, but it may stop the wl_surface from "playing the role".
	/// For instance, if a wl_subsurface object is destroyed, the wl_surface
	/// it was created for will be unmapped and forget its position and
	/// z-order. It is allowed to create a wl_subsurface for the same
	/// wl_surface again, but it is not allowed to use the wl_surface as
	/// a cursor (cursor is a different role than sub-surface, and role
	/// switching is not allowed).
	/// </remarks>
	public abstract class IWlSurface : IWaylandObject {
		protected IWlSurface(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Wl_surface error values
			/// </summary>
			/// <remarks>
			/// These errors can be emitted in response to wl_surface requests.
			/// </remarks>
			public enum Error {
				/// <summary>
				/// Buffer scale value is invalid
				/// </summary>
				InvalidScale = 0,
				/// <summary>
				/// Buffer transform value is invalid
				/// </summary>
				InvalidTransform = 1,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var buffer = Owner.GetObject<IWlBuffer>(Helper.ReadUint(tbuf, ref _offset));
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					Attach(buffer, x, y);
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					var width = (int) Helper.ReadInt(tbuf, ref _offset);
					var height = (int) Helper.ReadInt(tbuf, ref _offset);
					Damage(x, y, width, height);
					break;
				}
				case 3: {
					Owner.Socket.Read(tbuf);
					var pre_callback = _offset;
					_offset += 4;
					Frame(out var callback);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_callback), callback);
					break;
				}
				case 4: {
					Owner.Socket.Read(tbuf);
					var region = Owner.GetObject<IWlRegion>(Helper.ReadUint(tbuf, ref _offset));
					SetOpaqueRegion(region);
					break;
				}
				case 5: {
					Owner.Socket.Read(tbuf);
					var region = Owner.GetObject<IWlRegion>(Helper.ReadUint(tbuf, ref _offset));
					SetInputRegion(region);
					break;
				}
				case 6: {
					Owner.Socket.Read(tbuf);
					Commit();
					break;
				}
				case 7: {
					Owner.Socket.Read(tbuf);
					var transform = (IWlOutput.Enum.Transform) Helper.ReadInt(tbuf, ref _offset);
					SetBufferTransform(transform);
					break;
				}
				case 8: {
					Owner.Socket.Read(tbuf);
					var scale = (int) Helper.ReadInt(tbuf, ref _offset);
					SetBufferScale(scale);
					break;
				}
				case 9: {
					Owner.Socket.Read(tbuf);
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					var width = (int) Helper.ReadInt(tbuf, ref _offset);
					var height = (int) Helper.ReadInt(tbuf, ref _offset);
					DamageBuffer(x, y, width, height);
					break;
				}
			}
		}
		/// <summary>
		/// Delete surface
		/// </summary>
		/// <remarks>
		/// Deletes the surface and invalidates its object ID.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// Set the surface contents
		/// </summary>
		/// <remarks>
		/// Set a buffer as the content of this surface.
		/// 
		/// The new size of the surface is calculated based on the buffer
		/// size transformed by the inverse buffer_transform and the
		/// inverse buffer_scale. This means that the supplied buffer
		/// must be an integer multiple of the buffer_scale.
		/// 
		/// The x and y arguments specify the location of the new pending
		/// buffer's upper left corner, relative to the current buffer's upper
		/// left corner, in surface-local coordinates. In other words, the
		/// x and y, combined with the new surface size define in which
		/// directions the surface's size changes.
		/// 
		/// Surface contents are double-buffered state, see wl_surface.commit.
		/// 
		/// The initial surface contents are void; there is no content.
		/// wl_surface.attach assigns the given wl_buffer as the pending
		/// wl_buffer. wl_surface.commit makes the pending wl_buffer the new
		/// surface contents, and the size of the surface becomes the size
		/// calculated from the wl_buffer, as described above. After commit,
		/// there is no pending buffer until the next attach.
		/// 
		/// Committing a pending wl_buffer allows the compositor to read the
		/// pixels in the wl_buffer. The compositor may access the pixels at
		/// any time after the wl_surface.commit request. When the compositor
		/// will not access the pixels anymore, it will send the
		/// wl_buffer.release event. Only after receiving wl_buffer.release,
		/// the client may reuse the wl_buffer. A wl_buffer that has been
		/// attached and then replaced by another attach instead of committed
		/// will not receive a release event, and is not used by the
		/// compositor.
		/// 
		/// If a pending wl_buffer has been committed to more than one wl_surface,
		/// the delivery of wl_buffer.release events becomes undefined. A well
		/// behaved client should not rely on wl_buffer.release events in this
		/// case. Alternatively, a client could create multiple wl_buffer objects
		/// from the same backing storage or use wp_linux_buffer_release.
		/// 
		/// Destroying the wl_buffer after wl_buffer.release does not change
		/// the surface contents. However, if the client destroys the
		/// wl_buffer before receiving the wl_buffer.release event, the surface
		/// contents become undefined immediately.
		/// 
		/// If wl_surface.attach is sent with a NULL wl_buffer, the
		/// following wl_surface.commit will remove the surface content.
		/// </remarks>
		/// <param name="buffer">Buffer of surface contents</param>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		public abstract void Attach(IWlBuffer buffer, int x, int y);
		/// <summary>
		/// Mark part of the surface damaged
		/// </summary>
		/// <remarks>
		/// This request is used to describe the regions where the pending
		/// buffer is different from the current surface contents, and where
		/// the surface therefore needs to be repainted. The compositor
		/// ignores the parts of the damage that fall outside of the surface.
		/// 
		/// Damage is double-buffered state, see wl_surface.commit.
		/// 
		/// The damage rectangle is specified in surface-local coordinates,
		/// where x and y specify the upper left corner of the damage rectangle.
		/// 
		/// The initial value for pending damage is empty: no damage.
		/// wl_surface.damage adds pending damage: the new pending damage
		/// is the union of old pending damage and the given rectangle.
		/// 
		/// wl_surface.commit assigns pending damage as the current damage,
		/// and clears pending damage. The server will clear the current
		/// damage as it repaints the surface.
		/// 
		/// Note! New clients should not use this request. Instead damage can be
		/// posted with wl_surface.damage_buffer which uses buffer coordinates
		/// instead of surface coordinates.
		/// </remarks>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		/// <param name="width">Width of damage rectangle</param>
		/// <param name="height">Height of damage rectangle</param>
		public abstract void Damage(int x, int y, int width, int height);
		/// <summary>
		/// Request a frame throttling hint
		/// </summary>
		/// <remarks>
		/// Request a notification when it is a good time to start drawing a new
		/// frame, by creating a frame callback. This is useful for throttling
		/// redrawing operations, and driving animations.
		/// 
		/// When a client is animating on a wl_surface, it can use the 'frame'
		/// request to get notified when it is a good time to draw and commit the
		/// next frame of animation. If the client commits an update earlier than
		/// that, it is likely that some updates will not make it to the display,
		/// and the client is wasting resources by drawing too often.
		/// 
		/// The frame request will take effect on the next wl_surface.commit.
		/// The notification will only be posted for one frame unless
		/// requested again. For a wl_surface, the notifications are posted in
		/// the order the frame requests were committed.
		/// 
		/// The server must send the notifications so that a client
		/// will not send excessive updates, while still allowing
		/// the highest possible update rate for clients that wait for the reply
		/// before drawing again. The server should give some time for the client
		/// to draw and commit after sending the frame callback events to let it
		/// hit the next output refresh.
		/// 
		/// A server should avoid signaling the frame callbacks if the
		/// surface is not visible in any way, e.g. the surface is off-screen,
		/// or completely obscured by other opaque surfaces.
		/// 
		/// The object returned by this request will be destroyed by the
		/// compositor after the callback is fired and as such the client must not
		/// attempt to use it after that point.
		/// 
		/// The callback_data passed in the callback is the current time, in
		/// milliseconds, with an undefined base.
		/// </remarks>
		/// <param name="callback">Callback object for the frame request</param>
		public abstract void Frame(out IWlCallback callback);
		/// <summary>
		/// Set opaque region
		/// </summary>
		/// <remarks>
		/// This request sets the region of the surface that contains
		/// opaque content.
		/// 
		/// The opaque region is an optimization hint for the compositor
		/// that lets it optimize the redrawing of content behind opaque
		/// regions.  Setting an opaque region is not required for correct
		/// behaviour, but marking transparent content as opaque will result
		/// in repaint artifacts.
		/// 
		/// The opaque region is specified in surface-local coordinates.
		/// 
		/// The compositor ignores the parts of the opaque region that fall
		/// outside of the surface.
		/// 
		/// Opaque region is double-buffered state, see wl_surface.commit.
		/// 
		/// wl_surface.set_opaque_region changes the pending opaque region.
		/// wl_surface.commit copies the pending region to the current region.
		/// Otherwise, the pending and current regions are never changed.
		/// 
		/// The initial value for an opaque region is empty. Setting the pending
		/// opaque region has copy semantics, and the wl_region object can be
		/// destroyed immediately. A NULL wl_region causes the pending opaque
		/// region to be set to empty.
		/// </remarks>
		/// <param name="region">Opaque region of the surface</param>
		public abstract void SetOpaqueRegion(IWlRegion region);
		/// <summary>
		/// Set input region
		/// </summary>
		/// <remarks>
		/// This request sets the region of the surface that can receive
		/// pointer and touch events.
		/// 
		/// Input events happening outside of this region will try the next
		/// surface in the server surface stack. The compositor ignores the
		/// parts of the input region that fall outside of the surface.
		/// 
		/// The input region is specified in surface-local coordinates.
		/// 
		/// Input region is double-buffered state, see wl_surface.commit.
		/// 
		/// wl_surface.set_input_region changes the pending input region.
		/// wl_surface.commit copies the pending region to the current region.
		/// Otherwise the pending and current regions are never changed,
		/// except cursor and icon surfaces are special cases, see
		/// wl_pointer.set_cursor and wl_data_device.start_drag.
		/// 
		/// The initial value for an input region is infinite. That means the
		/// whole surface will accept input. Setting the pending input region
		/// has copy semantics, and the wl_region object can be destroyed
		/// immediately. A NULL wl_region causes the input region to be set
		/// to infinite.
		/// </remarks>
		/// <param name="region">Input region of the surface</param>
		public abstract void SetInputRegion(IWlRegion region);
		/// <summary>
		/// Commit pending surface state
		/// </summary>
		/// <remarks>
		/// Surface state (input, opaque, and damage regions, attached buffers,
		/// etc.) is double-buffered. Protocol requests modify the pending state,
		/// as opposed to the current state in use by the compositor. A commit
		/// request atomically applies all pending state, replacing the current
		/// state. After commit, the new pending state is as documented for each
		/// related request.
		/// 
		/// On commit, a pending wl_buffer is applied first, and all other state
		/// second. This means that all coordinates in double-buffered state are
		/// relative to the new wl_buffer coming into use, except for
		/// wl_surface.attach itself. If there is no pending wl_buffer, the
		/// coordinates are relative to the current surface contents.
		/// 
		/// All requests that need a commit to become effective are documented
		/// to affect double-buffered state.
		/// 
		/// Other interfaces may add further double-buffered surface state.
		/// </remarks>
		public abstract void Commit();
		/// <summary>
		/// Sets the buffer transformation
		/// </summary>
		/// <remarks>
		/// This request sets an optional transformation on how the compositor
		/// interprets the contents of the buffer attached to the surface. The
		/// accepted values for the transform parameter are the values for
		/// wl_output.transform.
		/// 
		/// Buffer transform is double-buffered state, see wl_surface.commit.
		/// 
		/// A newly created surface has its buffer transformation set to normal.
		/// 
		/// wl_surface.set_buffer_transform changes the pending buffer
		/// transformation. wl_surface.commit copies the pending buffer
		/// transformation to the current one. Otherwise, the pending and current
		/// values are never changed.
		/// 
		/// The purpose of this request is to allow clients to render content
		/// according to the output transform, thus permitting the compositor to
		/// use certain optimizations even if the display is rotated. Using
		/// hardware overlays and scanning out a client buffer for fullscreen
		/// surfaces are examples of such optimizations. Those optimizations are
		/// highly dependent on the compositor implementation, so the use of this
		/// request should be considered on a case-by-case basis.
		/// 
		/// Note that if the transform value includes 90 or 270 degree rotation,
		/// the width of the buffer will become the surface height and the height
		/// of the buffer will become the surface width.
		/// 
		/// If transform is not one of the values from the
		/// wl_output.transform enum the invalid_transform protocol error
		/// is raised.
		/// </remarks>
		/// <param name="transform">Transform for interpreting buffer contents</param>
		public abstract void SetBufferTransform(IWlOutput.Enum.Transform transform);
		/// <summary>
		/// Sets the buffer scaling factor
		/// </summary>
		/// <remarks>
		/// This request sets an optional scaling factor on how the compositor
		/// interprets the contents of the buffer attached to the window.
		/// 
		/// Buffer scale is double-buffered state, see wl_surface.commit.
		/// 
		/// A newly created surface has its buffer scale set to 1.
		/// 
		/// wl_surface.set_buffer_scale changes the pending buffer scale.
		/// wl_surface.commit copies the pending buffer scale to the current one.
		/// Otherwise, the pending and current values are never changed.
		/// 
		/// The purpose of this request is to allow clients to supply higher
		/// resolution buffer data for use on high resolution outputs. It is
		/// intended that you pick the same buffer scale as the scale of the
		/// output that the surface is displayed on. This means the compositor
		/// can avoid scaling when rendering the surface on that output.
		/// 
		/// Note that if the scale is larger than 1, then you have to attach
		/// a buffer that is larger (by a factor of scale in each dimension)
		/// than the desired surface size.
		/// 
		/// If scale is not positive the invalid_scale protocol error is
		/// raised.
		/// </remarks>
		/// <param name="scale">Positive scale for interpreting buffer contents</param>
		public abstract void SetBufferScale(int scale);
		/// <summary>
		/// Mark part of the surface damaged using buffer coordinates
		/// </summary>
		/// <remarks>
		/// This request is used to describe the regions where the pending
		/// buffer is different from the current surface contents, and where
		/// the surface therefore needs to be repainted. The compositor
		/// ignores the parts of the damage that fall outside of the surface.
		/// 
		/// Damage is double-buffered state, see wl_surface.commit.
		/// 
		/// The damage rectangle is specified in buffer coordinates,
		/// where x and y specify the upper left corner of the damage rectangle.
		/// 
		/// The initial value for pending damage is empty: no damage.
		/// wl_surface.damage_buffer adds pending damage: the new pending
		/// damage is the union of old pending damage and the given rectangle.
		/// 
		/// wl_surface.commit assigns pending damage as the current damage,
		/// and clears pending damage. The server will clear the current
		/// damage as it repaints the surface.
		/// 
		/// This request differs from wl_surface.damage in only one way - it
		/// takes damage in buffer coordinates instead of surface-local
		/// coordinates. While this generally is more intuitive than surface
		/// coordinates, it is especially desirable when using wp_viewport
		/// or when a drawing library (like EGL) is unaware of buffer scale
		/// and buffer transform.
		/// 
		/// Note: Because buffer transformation changes and damage requests may
		/// be interleaved in the protocol stream, it is impossible to determine
		/// the actual mapping between surface and buffer damage until
		/// wl_surface.commit time. Therefore, compositors wishing to take both
		/// kinds of damage into account will have to accumulate damage from the
		/// two requests separately and only transform from one to the other
		/// after receiving the wl_surface.commit.
		/// </remarks>
		/// <param name="x">Buffer-local x coordinate</param>
		/// <param name="y">Buffer-local y coordinate</param>
		/// <param name="width">Width of damage rectangle</param>
		/// <param name="height">Height of damage rectangle</param>
		public abstract void DamageBuffer(int x, int y, int width, int height);
		/// <summary>
		/// Surface enters an output
		/// </summary>
		/// <remarks>
		/// This is emitted whenever a surface's creation, movement, or resizing
		/// results in some part of it being within the scanout region of an
		/// output.
		/// 
		/// Note that a surface may be overlapping with zero or more outputs.
		/// </remarks>
		/// <param name="output">Output entered by the surface</param>
		public void Enter(IWlOutput output) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, output.Id);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Surface leaves an output
		/// </summary>
		/// <remarks>
		/// This is emitted whenever a surface's creation, movement, or resizing
		/// results in it no longer having any part of it within the scanout region
		/// of an output.
		/// </remarks>
		/// <param name="output">Output left by the surface</param>
		public void Leave(IWlOutput output) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, output.Id);
			SendEvent(1, tbuf);
		}
	}
	/// <summary>
	/// Group of input devices
	/// </summary>
	/// <remarks>
	/// A seat is a group of keyboards, pointer and touch devices. This
	/// object is published as a global during start up, or when such a
	/// device is hot plugged.  A seat typically has a pointer and
	/// maintains a keyboard focus and a pointer focus.
	/// </remarks>
	public abstract class IWlSeat : IWaylandObject {
		protected IWlSeat(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Seat capability bitmask
			/// </summary>
			/// <remarks>
			/// This is a bitmask of capabilities this seat has; if a member is
			/// set, then it is present on the seat.
			/// </remarks>
			[Flags]
			public enum Capability : uint {
				/// <summary>
				/// The seat has pointer devices
				/// </summary>
				Pointer = 1,
				/// <summary>
				/// The seat has one or more keyboards
				/// </summary>
				Keyboard = 2,
				/// <summary>
				/// The seat has touch devices
				/// </summary>
				Touch = 4,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					GetPointer(out var id);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					GetKeyboard(out var id);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					GetTouch(out var id);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
				case 3: {
					Owner.Socket.Read(tbuf);
					Release();
					break;
				}
			}
		}
		/// <summary>
		/// Return pointer object
		/// </summary>
		/// <remarks>
		/// The ID provided will be initialized to the wl_pointer interface
		/// for this seat.
		/// 
		/// This request only takes effect if the seat has the pointer
		/// capability, or has had the pointer capability in the past.
		/// It is a protocol violation to issue this request on a seat that has
		/// never had the pointer capability.
		/// </remarks>
		/// <param name="id">Seat pointer</param>
		public abstract void GetPointer(out IWlPointer id);
		/// <summary>
		/// Return keyboard object
		/// </summary>
		/// <remarks>
		/// The ID provided will be initialized to the wl_keyboard interface
		/// for this seat.
		/// 
		/// This request only takes effect if the seat has the keyboard
		/// capability, or has had the keyboard capability in the past.
		/// It is a protocol violation to issue this request on a seat that has
		/// never had the keyboard capability.
		/// </remarks>
		/// <param name="id">Seat keyboard</param>
		public abstract void GetKeyboard(out IWlKeyboard id);
		/// <summary>
		/// Return touch object
		/// </summary>
		/// <remarks>
		/// The ID provided will be initialized to the wl_touch interface
		/// for this seat.
		/// 
		/// This request only takes effect if the seat has the touch
		/// capability, or has had the touch capability in the past.
		/// It is a protocol violation to issue this request on a seat that has
		/// never had the touch capability.
		/// </remarks>
		/// <param name="id">Seat touch interface</param>
		public abstract void GetTouch(out IWlTouch id);
		/// <summary>
		/// Release the seat object
		/// </summary>
		/// <remarks>
		/// Using this request a client can tell the server that it is not going to
		/// use the seat object anymore.
		/// </remarks>
		public abstract void Release();
		/// <summary>
		/// Seat capabilities changed
		/// </summary>
		/// <remarks>
		/// This is emitted whenever a seat gains or loses the pointer,
		/// keyboard or touch capabilities.  The argument is a capability
		/// enum containing the complete set of capabilities this seat has.
		/// 
		/// When the pointer capability is added, a client may create a
		/// wl_pointer object using the wl_seat.get_pointer request. This object
		/// will receive pointer events until the capability is removed in the
		/// future.
		/// 
		/// When the pointer capability is removed, a client should destroy the
		/// wl_pointer objects associated with the seat where the capability was
		/// removed, using the wl_pointer.release request. No further pointer
		/// events will be received on these objects.
		/// 
		/// In some compositors, if a seat regains the pointer capability and a
		/// client has a previously obtained wl_pointer object of version 4 or
		/// less, that object may start sending pointer events again. This
		/// behavior is considered a misinterpretation of the intended behavior
		/// and must not be relied upon by the client. wl_pointer objects of
		/// version 5 or later must not send events if created before the most
		/// recent event notifying the client of an added pointer capability.
		/// 
		/// The above behavior also applies to wl_keyboard and wl_touch with the
		/// keyboard and touch capabilities, respectively.
		/// </remarks>
		/// <param name="capabilities">Capabilities of the seat</param>
		public void Capabilities(Enum.Capability capabilities) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) capabilities);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Unique identifier for this seat
		/// </summary>
		/// <remarks>
		/// In a multiseat configuration this can be used by the client to help
		/// identify which physical devices the seat represents. Based on
		/// the seat configuration used by the compositor.
		/// </remarks>
		/// <param name="name">Seat identifier</param>
		public void Name(string name) {
			var _offset = 0;
			_offset += Helper.StringSize(name);
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteString(tbuf, ref _offset, name);
			SendEvent(1, tbuf);
		}
	}
	/// <summary>
	/// Pointer input device
	/// </summary>
	/// <remarks>
	/// The wl_pointer interface represents one or more input devices,
	/// such as mice, which control the pointer location and pointer_focus
	/// of a seat.
	/// 
	/// The wl_pointer interface generates motion, enter and leave
	/// events for the surfaces that the pointer is located over,
	/// and button and axis events for button presses, button releases
	/// and scrolling.
	/// </remarks>
	public abstract class IWlPointer : IWaylandObject {
		protected IWlPointer(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			public enum Error {
				/// <summary>
				/// Given wl_surface has another role
				/// </summary>
				Role = 0,
			}
			/// <summary>
			/// Physical button state
			/// </summary>
			/// <remarks>
			/// Describes the physical state of a button that produced the button
			/// event.
			/// </remarks>
			public enum ButtonState {
				/// <summary>
				/// The button is not pressed
				/// </summary>
				Released = 0,
				/// <summary>
				/// The button is pressed
				/// </summary>
				Pressed = 1,
			}
			/// <summary>
			/// Axis types
			/// </summary>
			/// <remarks>
			/// Describes the axis types of scroll events.
			/// </remarks>
			public enum Axis {
				/// <summary>
				/// Vertical axis
				/// </summary>
				VerticalScroll = 0,
				/// <summary>
				/// Horizontal axis
				/// </summary>
				HorizontalScroll = 1,
			}
			/// <summary>
			/// Axis source types
			/// </summary>
			/// <remarks>
			/// Describes the source types for axis events. This indicates to the
			/// client how an axis event was physically generated; a client may
			/// adjust the user interface accordingly. For example, scroll events
			/// from a "finger" source may be in a smooth coordinate space with
			/// kinetic scrolling whereas a "wheel" source may be in discrete steps
			/// of a number of lines.
			/// 
			/// The "continuous" axis source is a device generating events in a
			/// continuous coordinate space, but using something other than a
			/// finger. One example for this source is button-based scrolling where
			/// the vertical motion of a device is converted to scroll events while
			/// a button is held down.
			/// 
			/// The "wheel tilt" axis source indicates that the actual device is a
			/// wheel but the scroll event is not caused by a rotation but a
			/// (usually sideways) tilt of the wheel.
			/// </remarks>
			public enum AxisSource {
				/// <summary>
				/// A physical wheel rotation
				/// </summary>
				Wheel = 0,
				/// <summary>
				/// Finger on a touch surface
				/// </summary>
				Finger = 1,
				/// <summary>
				/// Continuous coordinate space
				/// </summary>
				Continuous = 2,
				/// <summary>
				/// A physical wheel tilt
				/// </summary>
				WheelTilt = 3,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					var serial = (uint) Helper.ReadUint(tbuf, ref _offset);
					var surface = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					var hotspot_x = (int) Helper.ReadInt(tbuf, ref _offset);
					var hotspot_y = (int) Helper.ReadInt(tbuf, ref _offset);
					SetCursor(serial, surface, hotspot_x, hotspot_y);
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					Release();
					break;
				}
			}
		}
		/// <summary>
		/// Set the pointer surface
		/// </summary>
		/// <remarks>
		/// Set the pointer surface, i.e., the surface that contains the
		/// pointer image (cursor). This request gives the surface the role
		/// of a cursor. If the surface already has another role, it raises
		/// a protocol error.
		/// 
		/// The cursor actually changes only if the pointer
		/// focus for this device is one of the requesting client's surfaces
		/// or the surface parameter is the current pointer surface. If
		/// there was a previous surface set with this request it is
		/// replaced. If surface is NULL, the pointer image is hidden.
		/// 
		/// The parameters hotspot_x and hotspot_y define the position of
		/// the pointer surface relative to the pointer location. Its
		/// top-left corner is always at (x, y) - (hotspot_x, hotspot_y),
		/// where (x, y) are the coordinates of the pointer location, in
		/// surface-local coordinates.
		/// 
		/// On surface.attach requests to the pointer surface, hotspot_x
		/// and hotspot_y are decremented by the x and y parameters
		/// passed to the request. Attach must be confirmed by
		/// wl_surface.commit as usual.
		/// 
		/// The hotspot can also be updated by passing the currently set
		/// pointer surface to this request with new values for hotspot_x
		/// and hotspot_y.
		/// 
		/// The current and pending input regions of the wl_surface are
		/// cleared, and wl_surface.set_input_region is ignored until the
		/// wl_surface is no longer used as the cursor. When the use as a
		/// cursor ends, the current and pending input regions become
		/// undefined, and the wl_surface is unmapped.
		/// </remarks>
		/// <param name="serial">Serial number of the enter event</param>
		/// <param name="surface">Pointer surface</param>
		/// <param name="hotspot_x">Surface-local x coordinate</param>
		/// <param name="hotspot_y">Surface-local y coordinate</param>
		public abstract void SetCursor(uint serial, IWlSurface surface, int hotspot_x, int hotspot_y);
		/// <summary>
		/// Release the pointer object
		/// </summary>
		/// <remarks>
		/// Using this request a client can tell the server that it is not going to
		/// use the pointer object anymore.
		/// 
		/// This request destroys the pointer proxy object, so clients must not call
		/// wl_pointer_destroy() after using this request.
		/// </remarks>
		public abstract void Release();
		/// <summary>
		/// Enter event
		/// </summary>
		/// <remarks>
		/// Notification that this seat's pointer is focused on a certain
		/// surface.
		/// 
		/// When a seat's focus enters a surface, the pointer image
		/// is undefined and a client should respond to this event by setting
		/// an appropriate pointer image with the set_cursor request.
		/// </remarks>
		/// <param name="serial">Serial number of the enter event</param>
		/// <param name="surface">Surface entered by the pointer</param>
		/// <param name="surface_x">Surface-local x coordinate</param>
		/// <param name="surface_y">Surface-local y coordinate</param>
		public void Enter(uint serial, IWlSurface surface, Fixed surface_x, Fixed surface_y) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, surface.Id);
			Helper.WriteUint(tbuf, ref _offset, surface_x.UintValue);
			Helper.WriteUint(tbuf, ref _offset, surface_y.UintValue);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Leave event
		/// </summary>
		/// <remarks>
		/// Notification that this seat's pointer is no longer focused on
		/// a certain surface.
		/// 
		/// The leave notification is sent before the enter notification
		/// for the new focus.
		/// </remarks>
		/// <param name="serial">Serial number of the leave event</param>
		/// <param name="surface">Surface left by the pointer</param>
		public void Leave(uint serial, IWlSurface surface) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, surface.Id);
			SendEvent(1, tbuf);
		}
		/// <summary>
		/// Pointer motion event
		/// </summary>
		/// <remarks>
		/// Notification of pointer location change. The arguments
		/// surface_x and surface_y are the location relative to the
		/// focused surface.
		/// </remarks>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="surface_x">Surface-local x coordinate</param>
		/// <param name="surface_y">Surface-local y coordinate</param>
		public void Motion(uint time, Fixed surface_x, Fixed surface_y) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteUint(tbuf, ref _offset, surface_x.UintValue);
			Helper.WriteUint(tbuf, ref _offset, surface_y.UintValue);
			SendEvent(2, tbuf);
		}
		/// <summary>
		/// Pointer button event
		/// </summary>
		/// <remarks>
		/// Mouse button click and release notifications.
		/// 
		/// The location of the click is given by the last motion or
		/// enter event.
		/// The time argument is a timestamp with millisecond
		/// granularity, with an undefined base.
		/// 
		/// The button is a button code as defined in the Linux kernel's
		/// linux/input-event-codes.h header file, e.g. BTN_LEFT.
		/// 
		/// Any 16-bit button code value is reserved for future additions to the
		/// kernel's event code list. All other button codes above 0xFFFF are
		/// currently undefined but may be used in future versions of this
		/// protocol.
		/// </remarks>
		/// <param name="serial">Serial number of the button event</param>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="button">Button that produced the event</param>
		/// <param name="state">Physical state of the button</param>
		public void Button(uint serial, uint time, uint button, Enum.ButtonState state) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteUint(tbuf, ref _offset, (uint) button);
			Helper.WriteUint(tbuf, ref _offset, (uint) state);
			SendEvent(3, tbuf);
		}
		/// <summary>
		/// Axis event
		/// </summary>
		/// <remarks>
		/// Scroll and other axis notifications.
		/// 
		/// For scroll events (vertical and horizontal scroll axes), the
		/// value parameter is the length of a vector along the specified
		/// axis in a coordinate space identical to those of motion events,
		/// representing a relative movement along the specified axis.
		/// 
		/// For devices that support movements non-parallel to axes multiple
		/// axis events will be emitted.
		/// 
		/// When applicable, for example for touch pads, the server can
		/// choose to emit scroll events where the motion vector is
		/// equivalent to a motion event vector.
		/// 
		/// When applicable, a client can transform its content relative to the
		/// scroll distance.
		/// </remarks>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="axis">Axis type</param>
		/// <param name="value">Length of vector in surface-local coordinate space</param>
		public void Axis(uint time, Enum.Axis axis, Fixed value) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteUint(tbuf, ref _offset, (uint) axis);
			Helper.WriteUint(tbuf, ref _offset, value.UintValue);
			SendEvent(4, tbuf);
		}
		/// <summary>
		/// End of a pointer event sequence
		/// </summary>
		/// <remarks>
		/// Indicates the end of a set of events that logically belong together.
		/// A client is expected to accumulate the data in all events within the
		/// frame before proceeding.
		/// 
		/// All wl_pointer events before a wl_pointer.frame event belong
		/// logically together. For example, in a diagonal scroll motion the
		/// compositor will send an optional wl_pointer.axis_source event, two
		/// wl_pointer.axis events (horizontal and vertical) and finally a
		/// wl_pointer.frame event. The client may use this information to
		/// calculate a diagonal vector for scrolling.
		/// 
		/// When multiple wl_pointer.axis events occur within the same frame,
		/// the motion vector is the combined motion of all events.
		/// When a wl_pointer.axis and a wl_pointer.axis_stop event occur within
		/// the same frame, this indicates that axis movement in one axis has
		/// stopped but continues in the other axis.
		/// When multiple wl_pointer.axis_stop events occur within the same
		/// frame, this indicates that these axes stopped in the same instance.
		/// 
		/// A wl_pointer.frame event is sent for every logical event group,
		/// even if the group only contains a single wl_pointer event.
		/// Specifically, a client may get a sequence: motion, frame, button,
		/// frame, axis, frame, axis_stop, frame.
		/// 
		/// The wl_pointer.enter and wl_pointer.leave events are logical events
		/// generated by the compositor and not the hardware. These events are
		/// also grouped by a wl_pointer.frame. When a pointer moves from one
		/// surface to another, a compositor should group the
		/// wl_pointer.leave event within the same wl_pointer.frame.
		/// However, a client must not rely on wl_pointer.leave and
		/// wl_pointer.enter being in the same wl_pointer.frame.
		/// Compositor-specific policies may require the wl_pointer.leave and
		/// wl_pointer.enter event being split across multiple wl_pointer.frame
		/// groups.
		/// </remarks>
		public void Frame() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(5, tbuf);
		}
		/// <summary>
		/// Axis source event
		/// </summary>
		/// <remarks>
		/// Source information for scroll and other axes.
		/// 
		/// This event does not occur on its own. It is sent before a
		/// wl_pointer.frame event and carries the source information for
		/// all events within that frame.
		/// 
		/// The source specifies how this event was generated. If the source is
		/// wl_pointer.axis_source.finger, a wl_pointer.axis_stop event will be
		/// sent when the user lifts the finger off the device.
		/// 
		/// If the source is wl_pointer.axis_source.wheel,
		/// wl_pointer.axis_source.wheel_tilt or
		/// wl_pointer.axis_source.continuous, a wl_pointer.axis_stop event may
		/// or may not be sent. Whether a compositor sends an axis_stop event
		/// for these sources is hardware-specific and implementation-dependent;
		/// clients must not rely on receiving an axis_stop event for these
		/// scroll sources and should treat scroll sequences from these scroll
		/// sources as unterminated by default.
		/// 
		/// This event is optional. If the source is unknown for a particular
		/// axis event sequence, no event is sent.
		/// Only one wl_pointer.axis_source event is permitted per frame.
		/// 
		/// The order of wl_pointer.axis_discrete and wl_pointer.axis_source is
		/// not guaranteed.
		/// </remarks>
		/// <param name="axis_source">Source of the axis event</param>
		public void AxisSource(Enum.AxisSource axis_source) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) axis_source);
			SendEvent(6, tbuf);
		}
		/// <summary>
		/// Axis stop event
		/// </summary>
		/// <remarks>
		/// Stop notification for scroll and other axes.
		/// 
		/// For some wl_pointer.axis_source types, a wl_pointer.axis_stop event
		/// is sent to notify a client that the axis sequence has terminated.
		/// This enables the client to implement kinetic scrolling.
		/// See the wl_pointer.axis_source documentation for information on when
		/// this event may be generated.
		/// 
		/// Any wl_pointer.axis events with the same axis_source after this
		/// event should be considered as the start of a new axis motion.
		/// 
		/// The timestamp is to be interpreted identical to the timestamp in the
		/// wl_pointer.axis event. The timestamp value may be the same as a
		/// preceding wl_pointer.axis event.
		/// </remarks>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="axis">The axis stopped with this event</param>
		public void AxisStop(uint time, Enum.Axis axis) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteUint(tbuf, ref _offset, (uint) axis);
			SendEvent(7, tbuf);
		}
		/// <summary>
		/// Axis click event
		/// </summary>
		/// <remarks>
		/// Discrete step information for scroll and other axes.
		/// 
		/// This event carries the axis value of the wl_pointer.axis event in
		/// discrete steps (e.g. mouse wheel clicks).
		/// 
		/// This event does not occur on its own, it is coupled with a
		/// wl_pointer.axis event that represents this axis value on a
		/// continuous scale. The protocol guarantees that each axis_discrete
		/// event is always followed by exactly one axis event with the same
		/// axis number within the same wl_pointer.frame. Note that the protocol
		/// allows for other events to occur between the axis_discrete and
		/// its coupled axis event, including other axis_discrete or axis
		/// events.
		/// 
		/// This event is optional; continuous scrolling devices
		/// like two-finger scrolling on touchpads do not have discrete
		/// steps and do not generate this event.
		/// 
		/// The discrete value carries the directional information. e.g. a value
		/// of -2 is two steps towards the negative direction of this axis.
		/// 
		/// The axis number is identical to the axis number in the associated
		/// axis event.
		/// 
		/// The order of wl_pointer.axis_discrete and wl_pointer.axis_source is
		/// not guaranteed.
		/// </remarks>
		/// <param name="axis">Axis type</param>
		/// <param name="discrete">Number of steps</param>
		public void AxisDiscrete(Enum.Axis axis, int discrete) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) axis);
			Helper.WriteInt(tbuf, ref _offset, (int) discrete);
			SendEvent(8, tbuf);
		}
	}
	/// <summary>
	/// Keyboard input device
	/// </summary>
	/// <remarks>
	/// The wl_keyboard interface represents one or more keyboards
	/// associated with a seat.
	/// </remarks>
	public abstract class IWlKeyboard : IWaylandObject {
		protected IWlKeyboard(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Keyboard mapping format
			/// </summary>
			/// <remarks>
			/// This specifies the format of the keymap provided to the
			/// client with the wl_keyboard.keymap event.
			/// </remarks>
			public enum KeymapFormat {
				/// <summary>
				/// No keymap; client must understand how to interpret the raw keycode
				/// </summary>
				NoKeymap = 0,
				/// <summary>
				/// Libxkbcommon compatible; to determine the xkb keycode, clients must add 8 to the key event keycode
				/// </summary>
				XkbV1 = 1,
			}
			/// <summary>
			/// Physical key state
			/// </summary>
			/// <remarks>
			/// Describes the physical state of a key that produced the key event.
			/// </remarks>
			public enum KeyState {
				/// <summary>
				/// Key is not pressed
				/// </summary>
				Released = 0,
				/// <summary>
				/// Key is pressed
				/// </summary>
				Pressed = 1,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Release();
					break;
				}
			}
		}
		/// <summary>
		/// Release the keyboard object
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		public abstract void Release();
		/// <summary>
		/// Keyboard mapping
		/// </summary>
		/// <remarks>
		/// This event provides a file descriptor to the client which can be
		/// memory-mapped to provide a keyboard mapping description.
		/// 
		/// From version 7 onwards, the fd must be mapped with MAP_PRIVATE by
		/// the recipient, as MAP_SHARED may fail.
		/// </remarks>
		/// <param name="format">Keymap format</param>
		/// <param name="fd">Keymap file descriptor</param>
		/// <param name="size">Keymap size, in bytes</param>
		public void Keymap(Enum.KeymapFormat format, IntPtr fd, uint size) {
			var _offset = 0;
			_offset += 4;
			var _fd = (int) fd;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) format);
			Helper.WriteUint(tbuf, ref _offset, (uint) size);
			SendEventWithFd(0, tbuf, _fd);
		}
		/// <summary>
		/// Enter event
		/// </summary>
		/// <remarks>
		/// Notification that this seat's keyboard focus is on a certain
		/// surface.
		/// </remarks>
		/// <param name="serial">Serial number of the enter event</param>
		/// <param name="surface">Surface gaining keyboard focus</param>
		/// <param name="keys">The currently pressed keys</param>
		public void Enter(uint serial, IWlSurface surface, byte[] keys) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, surface.Id);
			SendEvent(1, tbuf);
		}
		/// <summary>
		/// Leave event
		/// </summary>
		/// <remarks>
		/// Notification that this seat's keyboard focus is no longer on
		/// a certain surface.
		/// 
		/// The leave notification is sent before the enter notification
		/// for the new focus.
		/// </remarks>
		/// <param name="serial">Serial number of the leave event</param>
		/// <param name="surface">Surface that lost keyboard focus</param>
		public void Leave(uint serial, IWlSurface surface) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, surface.Id);
			SendEvent(2, tbuf);
		}
		/// <summary>
		/// Key event
		/// </summary>
		/// <remarks>
		/// A key was pressed or released.
		/// The time argument is a timestamp with millisecond
		/// granularity, with an undefined base.
		/// </remarks>
		/// <param name="serial">Serial number of the key event</param>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="key">Key that produced the event</param>
		/// <param name="state">Physical state of the key</param>
		public void Key(uint serial, uint time, uint key, Enum.KeyState state) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteUint(tbuf, ref _offset, (uint) key);
			Helper.WriteUint(tbuf, ref _offset, (uint) state);
			SendEvent(3, tbuf);
		}
		/// <summary>
		/// Modifier and group state
		/// </summary>
		/// <remarks>
		/// Notifies clients that the modifier and/or group state has
		/// changed, and it should update its local state.
		/// </remarks>
		/// <param name="serial">Serial number of the modifiers event</param>
		/// <param name="mods_depressed">Depressed modifiers</param>
		/// <param name="mods_latched">Latched modifiers</param>
		/// <param name="mods_locked">Locked modifiers</param>
		/// <param name="group">Keyboard layout</param>
		public void Modifiers(uint serial, uint mods_depressed, uint mods_latched, uint mods_locked, uint group) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, (uint) mods_depressed);
			Helper.WriteUint(tbuf, ref _offset, (uint) mods_latched);
			Helper.WriteUint(tbuf, ref _offset, (uint) mods_locked);
			Helper.WriteUint(tbuf, ref _offset, (uint) group);
			SendEvent(4, tbuf);
		}
		/// <summary>
		/// Repeat rate and delay
		/// </summary>
		/// <remarks>
		/// Informs the client about the keyboard's repeat rate and delay.
		/// 
		/// This event is sent as soon as the wl_keyboard object has been created,
		/// and is guaranteed to be received by the client before any key press
		/// event.
		/// 
		/// Negative values for either rate or delay are illegal. A rate of zero
		/// will disable any repeating (regardless of the value of delay).
		/// 
		/// This event can be sent later on as well with a new value if necessary,
		/// so clients should continue listening for the event past the creation
		/// of wl_keyboard.
		/// </remarks>
		/// <param name="rate">The rate of repeating keys in characters per second</param>
		/// <param name="delay">Delay in milliseconds since key down until repeating starts</param>
		public void RepeatInfo(int rate, int delay) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteInt(tbuf, ref _offset, (int) rate);
			Helper.WriteInt(tbuf, ref _offset, (int) delay);
			SendEvent(5, tbuf);
		}
	}
	/// <summary>
	/// Touchscreen input device
	/// </summary>
	/// <remarks>
	/// The wl_touch interface represents a touchscreen
	/// associated with a seat.
	/// 
	/// Touch interactions can consist of one or more contacts.
	/// For each contact, a series of events is generated, starting
	/// with a down event, followed by zero or more motion events,
	/// and ending with an up event. Events relating to the same
	/// contact point can be identified by the ID of the sequence.
	/// </remarks>
	public abstract class IWlTouch : IWaylandObject {
		protected IWlTouch(Client owner, uint? id) : base(owner, id) {}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Release();
					break;
				}
			}
		}
		/// <summary>
		/// Release the touch object
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		public abstract void Release();
		/// <summary>
		/// Touch down event and beginning of a touch sequence
		/// </summary>
		/// <remarks>
		/// A new touch point has appeared on the surface. This touch point is
		/// assigned a unique ID. Future events from this touch point reference
		/// this ID. The ID ceases to be valid after a touch up event and may be
		/// reused in the future.
		/// </remarks>
		/// <param name="serial">Serial number of the touch down event</param>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="surface">Surface touched</param>
		/// <param name="id">The unique ID of this touch point</param>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		public void Down(uint serial, uint time, IWlSurface surface, int id, Fixed x, Fixed y) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteUint(tbuf, ref _offset, surface.Id);
			Helper.WriteInt(tbuf, ref _offset, (int) id);
			Helper.WriteUint(tbuf, ref _offset, x.UintValue);
			Helper.WriteUint(tbuf, ref _offset, y.UintValue);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// End of a touch event sequence
		/// </summary>
		/// <remarks>
		/// The touch point has disappeared. No further events will be sent for
		/// this touch point and the touch point's ID is released and may be
		/// reused in a future touch down event.
		/// </remarks>
		/// <param name="serial">Serial number of the touch up event</param>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="id">The unique ID of this touch point</param>
		public void Up(uint serial, uint time, int id) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) serial);
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteInt(tbuf, ref _offset, (int) id);
			SendEvent(1, tbuf);
		}
		/// <summary>
		/// Update of touch point coordinates
		/// </summary>
		/// <remarks>
		/// A touch point has changed coordinates.
		/// </remarks>
		/// <param name="time">Timestamp with millisecond granularity</param>
		/// <param name="id">The unique ID of this touch point</param>
		/// <param name="x">Surface-local x coordinate</param>
		/// <param name="y">Surface-local y coordinate</param>
		public void Motion(uint time, int id, Fixed x, Fixed y) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) time);
			Helper.WriteInt(tbuf, ref _offset, (int) id);
			Helper.WriteUint(tbuf, ref _offset, x.UintValue);
			Helper.WriteUint(tbuf, ref _offset, y.UintValue);
			SendEvent(2, tbuf);
		}
		/// <summary>
		/// End of touch frame event
		/// </summary>
		/// <remarks>
		/// Indicates the end of a set of events that logically belong together.
		/// A client is expected to accumulate the data in all events within the
		/// frame before proceeding.
		/// 
		/// A wl_touch.frame terminates at least one event but otherwise no
		/// guarantee is provided about the set of events within a frame. A client
		/// must assume that any state not updated in a frame is unchanged from the
		/// previously known state.
		/// </remarks>
		public void Frame() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(3, tbuf);
		}
		/// <summary>
		/// Touch session cancelled
		/// </summary>
		/// <remarks>
		/// Sent if the compositor decides the touch stream is a global
		/// gesture. No further events are sent to the clients from that
		/// particular gesture. Touch cancellation applies to all touch points
		/// currently active on this client's surface. The client is
		/// responsible for finalizing the touch points, future touch points on
		/// this surface may reuse the touch point ID.
		/// </remarks>
		public void Cancel() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(4, tbuf);
		}
		/// <summary>
		/// Update shape of touch point
		/// </summary>
		/// <remarks>
		/// Sent when a touchpoint has changed its shape.
		/// 
		/// This event does not occur on its own. It is sent before a
		/// wl_touch.frame event and carries the new shape information for
		/// any previously reported, or new touch points of that frame.
		/// 
		/// Other events describing the touch point such as wl_touch.down,
		/// wl_touch.motion or wl_touch.orientation may be sent within the
		/// same wl_touch.frame. A client should treat these events as a single
		/// logical touch point update. The order of wl_touch.shape,
		/// wl_touch.orientation and wl_touch.motion is not guaranteed.
		/// A wl_touch.down event is guaranteed to occur before the first
		/// wl_touch.shape event for this touch ID but both events may occur within
		/// the same wl_touch.frame.
		/// 
		/// A touchpoint shape is approximated by an ellipse through the major and
		/// minor axis length. The major axis length describes the longer diameter
		/// of the ellipse, while the minor axis length describes the shorter
		/// diameter. Major and minor are orthogonal and both are specified in
		/// surface-local coordinates. The center of the ellipse is always at the
		/// touchpoint location as reported by wl_touch.down or wl_touch.move.
		/// 
		/// This event is only sent by the compositor if the touch device supports
		/// shape reports. The client has to make reasonable assumptions about the
		/// shape if it did not receive this event.
		/// </remarks>
		/// <param name="id">The unique ID of this touch point</param>
		/// <param name="major">Length of the major axis in surface-local coordinates</param>
		/// <param name="minor">Length of the minor axis in surface-local coordinates</param>
		public void Shape(int id, Fixed major, Fixed minor) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteInt(tbuf, ref _offset, (int) id);
			Helper.WriteUint(tbuf, ref _offset, major.UintValue);
			Helper.WriteUint(tbuf, ref _offset, minor.UintValue);
			SendEvent(5, tbuf);
		}
		/// <summary>
		/// Update orientation of touch point
		/// </summary>
		/// <remarks>
		/// Sent when a touchpoint has changed its orientation.
		/// 
		/// This event does not occur on its own. It is sent before a
		/// wl_touch.frame event and carries the new shape information for
		/// any previously reported, or new touch points of that frame.
		/// 
		/// Other events describing the touch point such as wl_touch.down,
		/// wl_touch.motion or wl_touch.shape may be sent within the
		/// same wl_touch.frame. A client should treat these events as a single
		/// logical touch point update. The order of wl_touch.shape,
		/// wl_touch.orientation and wl_touch.motion is not guaranteed.
		/// A wl_touch.down event is guaranteed to occur before the first
		/// wl_touch.orientation event for this touch ID but both events may occur
		/// within the same wl_touch.frame.
		/// 
		/// The orientation describes the clockwise angle of a touchpoint's major
		/// axis to the positive surface y-axis and is normalized to the -180 to
		/// +180 degree range. The granularity of orientation depends on the touch
		/// device, some devices only support binary rotation values between 0 and
		/// 90 degrees.
		/// 
		/// This event is only sent by the compositor if the touch device supports
		/// orientation reports.
		/// </remarks>
		/// <param name="id">The unique ID of this touch point</param>
		/// <param name="orientation">Angle between major axis and positive surface y-axis in degrees</param>
		public void Orientation(int id, Fixed orientation) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteInt(tbuf, ref _offset, (int) id);
			Helper.WriteUint(tbuf, ref _offset, orientation.UintValue);
			SendEvent(6, tbuf);
		}
	}
	/// <summary>
	/// Compositor output region
	/// </summary>
	/// <remarks>
	/// An output describes part of the compositor geometry.  The
	/// compositor works in the 'compositor coordinate system' and an
	/// output corresponds to a rectangular area in that space that is
	/// actually visible.  This typically corresponds to a monitor that
	/// displays part of the compositor space.  This object is published
	/// as global during start up, or when a monitor is hotplugged.
	/// </remarks>
	public abstract class IWlOutput : IWaylandObject {
		protected IWlOutput(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			/// <summary>
			/// Subpixel geometry information
			/// </summary>
			/// <remarks>
			/// This enumeration describes how the physical
			/// pixels on an output are laid out.
			/// </remarks>
			public enum Subpixel {
				/// <summary>
				/// Unknown geometry
				/// </summary>
				Unknown = 0,
				/// <summary>
				/// No geometry
				/// </summary>
				None = 1,
				/// <summary>
				/// Horizontal RGB
				/// </summary>
				HorizontalRgb = 2,
				/// <summary>
				/// Horizontal BGR
				/// </summary>
				HorizontalBgr = 3,
				/// <summary>
				/// Vertical RGB
				/// </summary>
				VerticalRgb = 4,
				/// <summary>
				/// Vertical BGR
				/// </summary>
				VerticalBgr = 5,
			}
			/// <summary>
			/// Transform from framebuffer to output
			/// </summary>
			/// <remarks>
			/// This describes the transform that a compositor will apply to a
			/// surface to compensate for the rotation or mirroring of an
			/// output device.
			/// 
			/// The flipped values correspond to an initial flip around a
			/// vertical axis followed by rotation.
			/// 
			/// The purpose is mainly to allow clients to render accordingly and
			/// tell the compositor, so that for fullscreen surfaces, the
			/// compositor will still be able to scan out directly from client
			/// surfaces.
			/// </remarks>
			public enum Transform {
				/// <summary>
				/// No transform
				/// </summary>
				Normal = 0,
				/// <summary>
				/// 90 degrees counter-clockwise
				/// </summary>
				_90 = 1,
				/// <summary>
				/// 180 degrees counter-clockwise
				/// </summary>
				_180 = 2,
				/// <summary>
				/// 270 degrees counter-clockwise
				/// </summary>
				_270 = 3,
				/// <summary>
				/// 180 degree flip around a vertical axis
				/// </summary>
				Flipped = 4,
				/// <summary>
				/// Flip and rotate 90 degrees counter-clockwise
				/// </summary>
				Flipped90 = 5,
				/// <summary>
				/// Flip and rotate 180 degrees counter-clockwise
				/// </summary>
				Flipped180 = 6,
				/// <summary>
				/// Flip and rotate 270 degrees counter-clockwise
				/// </summary>
				Flipped270 = 7,
			}
			/// <summary>
			/// Mode information
			/// </summary>
			/// <remarks>
			/// These flags describe properties of an output mode.
			/// They are used in the flags bitfield of the mode event.
			/// </remarks>
			[Flags]
			public enum Mode : uint {
				/// <summary>
				/// Indicates this is the current mode
				/// </summary>
				Current = 0x1,
				/// <summary>
				/// Indicates this is the preferred mode
				/// </summary>
				Preferred = 0x2,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Release();
					break;
				}
			}
		}
		/// <summary>
		/// Release the output object
		/// </summary>
		/// <remarks>
		/// Using this request a client can tell the server that it is not going to
		/// use the output object anymore.
		/// </remarks>
		public abstract void Release();
		/// <summary>
		/// Properties of the output
		/// </summary>
		/// <remarks>
		/// The geometry event describes geometric properties of the output.
		/// The event is sent when binding to the output object and whenever
		/// any of the properties change.
		/// 
		/// The physical size can be set to zero if it doesn't make sense for this
		/// output (e.g. for projectors or virtual outputs).
		/// 
		/// Note: wl_output only advertises partial information about the output
		/// position and identification. Some compositors, for instance those not
		/// implementing a desktop-style output layout or those exposing virtual
		/// outputs, might fake this information. Instead of using x and y, clients
		/// should use xdg_output.logical_position. Instead of using make and model,
		/// clients should use xdg_output.name and xdg_output.description.
		/// </remarks>
		/// <param name="x">X position within the global compositor space</param>
		/// <param name="y">Y position within the global compositor space</param>
		/// <param name="physical_width">Width in millimeters of the output</param>
		/// <param name="physical_height">Height in millimeters of the output</param>
		/// <param name="subpixel">Subpixel orientation of the output</param>
		/// <param name="make">Textual description of the manufacturer</param>
		/// <param name="model">Textual description of the model</param>
		/// <param name="transform">Transform that maps framebuffer to output</param>
		public void Geometry(int x, int y, int physical_width, int physical_height, Enum.Subpixel subpixel, string make, string model, Enum.Transform transform) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += Helper.StringSize(make);
			_offset += Helper.StringSize(model);
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteInt(tbuf, ref _offset, (int) x);
			Helper.WriteInt(tbuf, ref _offset, (int) y);
			Helper.WriteInt(tbuf, ref _offset, (int) physical_width);
			Helper.WriteInt(tbuf, ref _offset, (int) physical_height);
			Helper.WriteInt(tbuf, ref _offset, (int) subpixel);
			Helper.WriteString(tbuf, ref _offset, make);
			Helper.WriteString(tbuf, ref _offset, model);
			Helper.WriteInt(tbuf, ref _offset, (int) transform);
			SendEvent(0, tbuf);
		}
		/// <summary>
		/// Advertise available modes for the output
		/// </summary>
		/// <remarks>
		/// The mode event describes an available mode for the output.
		/// 
		/// The event is sent when binding to the output object and there
		/// will always be one mode, the current mode.  The event is sent
		/// again if an output changes mode, for the mode that is now
		/// current.  In other words, the current mode is always the last
		/// mode that was received with the current flag set.
		/// 
		/// The size of a mode is given in physical hardware units of
		/// the output device. This is not necessarily the same as
		/// the output size in the global compositor space. For instance,
		/// the output may be scaled, as described in wl_output.scale,
		/// or transformed, as described in wl_output.transform. Clients
		/// willing to retrieve the output size in the global compositor
		/// space should use xdg_output.logical_size instead.
		/// 
		/// The vertical refresh rate can be set to zero if it doesn't make
		/// sense for this output (e.g. for virtual outputs).
		/// 
		/// Clients should not use the refresh rate to schedule frames. Instead,
		/// they should use the wl_surface.frame event or the presentation-time
		/// protocol.
		/// 
		/// Note: this information is not always meaningful for all outputs. Some
		/// compositors, such as those exposing virtual outputs, might fake the
		/// refresh rate or the size.
		/// </remarks>
		/// <param name="flags">Bitfield of mode flags</param>
		/// <param name="width">Width of the mode in hardware units</param>
		/// <param name="height">Height of the mode in hardware units</param>
		/// <param name="refresh">Vertical refresh rate in mHz</param>
		public void Mode(Enum.Mode flags, int width, int height, int refresh) {
			var _offset = 0;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteUint(tbuf, ref _offset, (uint) flags);
			Helper.WriteInt(tbuf, ref _offset, (int) width);
			Helper.WriteInt(tbuf, ref _offset, (int) height);
			Helper.WriteInt(tbuf, ref _offset, (int) refresh);
			SendEvent(1, tbuf);
		}
		/// <summary>
		/// Sent all information about output
		/// </summary>
		/// <remarks>
		/// This event is sent after all other properties have been
		/// sent after binding to the output object and after any
		/// other property changes done after that. This allows
		/// changes to the output properties to be seen as
		/// atomic, even if they happen via multiple events.
		/// </remarks>
		public void Done() {
			var _offset = 0;
			var tbuf = new byte[_offset];
			_offset = 0;
			SendEvent(2, tbuf);
		}
		/// <summary>
		/// Output scaling properties
		/// </summary>
		/// <remarks>
		/// This event contains scaling geometry information
		/// that is not in the geometry event. It may be sent after
		/// binding the output object or if the output scale changes
		/// later. If it is not sent, the client should assume a
		/// scale of 1.
		/// 
		/// A scale larger than 1 means that the compositor will
		/// automatically scale surface buffers by this amount
		/// when rendering. This is used for very high resolution
		/// displays where applications rendering at the native
		/// resolution would be too small to be legible.
		/// 
		/// It is intended that scaling aware clients track the
		/// current output of a surface, and if it is on a scaled
		/// output it should use wl_surface.set_buffer_scale with
		/// the scale of the output. That way the compositor can
		/// avoid scaling the surface, and the client can supply
		/// a higher detail image.
		/// </remarks>
		/// <param name="factor">Scaling factor of output</param>
		public void Scale(int factor) {
			var _offset = 0;
			_offset += 4;
			var tbuf = new byte[_offset];
			_offset = 0;
			Helper.WriteInt(tbuf, ref _offset, (int) factor);
			SendEvent(3, tbuf);
		}
	}
	/// <summary>
	/// Region interface
	/// </summary>
	/// <remarks>
	/// A region object describes an area.
	/// 
	/// Region objects are used to describe the opaque and input
	/// regions of a surface.
	/// </remarks>
	public abstract class IWlRegion : IWaylandObject {
		protected IWlRegion(Client owner, uint? id) : base(owner, id) {}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					var width = (int) Helper.ReadInt(tbuf, ref _offset);
					var height = (int) Helper.ReadInt(tbuf, ref _offset);
					Add(x, y, width, height);
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					var width = (int) Helper.ReadInt(tbuf, ref _offset);
					var height = (int) Helper.ReadInt(tbuf, ref _offset);
					Subtract(x, y, width, height);
					break;
				}
			}
		}
		/// <summary>
		/// Destroy region
		/// </summary>
		/// <remarks>
		/// Destroy the region.  This will invalidate the object ID.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// Add rectangle to region
		/// </summary>
		/// <remarks>
		/// Add the specified rectangle to the region.
		/// </remarks>
		/// <param name="x">Region-local x coordinate</param>
		/// <param name="y">Region-local y coordinate</param>
		/// <param name="width">Rectangle width</param>
		/// <param name="height">Rectangle height</param>
		public abstract void Add(int x, int y, int width, int height);
		/// <summary>
		/// Subtract rectangle from region
		/// </summary>
		/// <remarks>
		/// Subtract the specified rectangle from the region.
		/// </remarks>
		/// <param name="x">Region-local x coordinate</param>
		/// <param name="y">Region-local y coordinate</param>
		/// <param name="width">Rectangle width</param>
		/// <param name="height">Rectangle height</param>
		public abstract void Subtract(int x, int y, int width, int height);
	}
	/// <summary>
	/// Sub-surface compositing
	/// </summary>
	/// <remarks>
	/// The global interface exposing sub-surface compositing capabilities.
	/// A wl_surface, that has sub-surfaces associated, is called the
	/// parent surface. Sub-surfaces can be arbitrarily nested and create
	/// a tree of sub-surfaces.
	/// 
	/// The root surface in a tree of sub-surfaces is the main
	/// surface. The main surface cannot be a sub-surface, because
	/// sub-surfaces must always have a parent.
	/// 
	/// A main surface with its sub-surfaces forms a (compound) window.
	/// For window management purposes, this set of wl_surface objects is
	/// to be considered as a single window, and it should also behave as
	/// such.
	/// 
	/// The aim of sub-surfaces is to offload some of the compositing work
	/// within a window from clients to the compositor. A prime example is
	/// a video player with decorations and video in separate wl_surface
	/// objects. This should allow the compositor to pass YUV video buffer
	/// processing to dedicated overlay hardware when possible.
	/// </remarks>
	public abstract class IWlSubcompositor : IWaylandObject {
		protected IWlSubcompositor(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			public enum Error {
				/// <summary>
				/// The to-be sub-surface is invalid
				/// </summary>
				BadSurface = 0,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var pre_id = _offset;
					_offset += 4;
					var surface = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					var parent = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					GetSubsurface(out var id, surface, parent);
					Owner.SetObject(BitConverter.ToUInt32(tbuf, pre_id), id);
					break;
				}
			}
		}
		/// <summary>
		/// Unbind from the subcompositor interface
		/// </summary>
		/// <remarks>
		/// Informs the server that the client will not be using this
		/// protocol object anymore. This does not affect any other
		/// objects, wl_subsurface objects included.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// Give a surface the role sub-surface
		/// </summary>
		/// <remarks>
		/// Create a sub-surface interface for the given surface, and
		/// associate it with the given parent surface. This turns a
		/// plain wl_surface into a sub-surface.
		/// 
		/// The to-be sub-surface must not already have another role, and it
		/// must not have an existing wl_subsurface object. Otherwise a protocol
		/// error is raised.
		/// 
		/// Adding sub-surfaces to a parent is a double-buffered operation on the
		/// parent (see wl_surface.commit). The effect of adding a sub-surface
		/// becomes visible on the next time the state of the parent surface is
		/// applied.
		/// 
		/// This request modifies the behaviour of wl_surface.commit request on
		/// the sub-surface, see the documentation on wl_subsurface interface.
		/// </remarks>
		/// <param name="id">The new sub-surface object ID</param>
		/// <param name="surface">The surface to be turned into a sub-surface</param>
		/// <param name="parent">The parent surface</param>
		public abstract void GetSubsurface(out IWlSubsurface id, IWlSurface surface, IWlSurface parent);
	}
	/// <summary>
	/// Sub-surface interface to a wl_surface
	/// </summary>
	/// <remarks>
	/// An additional interface to a wl_surface object, which has been
	/// made a sub-surface. A sub-surface has one parent surface. A
	/// sub-surface's size and position are not limited to that of the parent.
	/// Particularly, a sub-surface is not automatically clipped to its
	/// parent's area.
	/// 
	/// A sub-surface becomes mapped, when a non-NULL wl_buffer is applied
	/// and the parent surface is mapped. The order of which one happens
	/// first is irrelevant. A sub-surface is hidden if the parent becomes
	/// hidden, or if a NULL wl_buffer is applied. These rules apply
	/// recursively through the tree of surfaces.
	/// 
	/// The behaviour of a wl_surface.commit request on a sub-surface
	/// depends on the sub-surface's mode. The possible modes are
	/// synchronized and desynchronized, see methods
	/// wl_subsurface.set_sync and wl_subsurface.set_desync. Synchronized
	/// mode caches the wl_surface state to be applied when the parent's
	/// state gets applied, and desynchronized mode applies the pending
	/// wl_surface state directly. A sub-surface is initially in the
	/// synchronized mode.
	/// 
	/// Sub-surfaces have also other kind of state, which is managed by
	/// wl_subsurface requests, as opposed to wl_surface requests. This
	/// state includes the sub-surface position relative to the parent
	/// surface (wl_subsurface.set_position), and the stacking order of
	/// the parent and its sub-surfaces (wl_subsurface.place_above and
	/// .place_below). This state is applied when the parent surface's
	/// wl_surface state is applied, regardless of the sub-surface's mode.
	/// As the exception, set_sync and set_desync are effective immediately.
	/// 
	/// The main surface can be thought to be always in desynchronized mode,
	/// since it does not have a parent in the sub-surfaces sense.
	/// 
	/// Even if a sub-surface is in desynchronized mode, it will behave as
	/// in synchronized mode, if its parent surface behaves as in
	/// synchronized mode. This rule is applied recursively throughout the
	/// tree of surfaces. This means, that one can set a sub-surface into
	/// synchronized mode, and then assume that all its child and grand-child
	/// sub-surfaces are synchronized, too, without explicitly setting them.
	/// 
	/// If the wl_surface associated with the wl_subsurface is destroyed, the
	/// wl_subsurface object becomes inert. Note, that destroying either object
	/// takes effect immediately. If you need to synchronize the removal
	/// of a sub-surface to the parent surface update, unmap the sub-surface
	/// first by attaching a NULL wl_buffer, update parent, and then destroy
	/// the sub-surface.
	/// 
	/// If the parent wl_surface object is destroyed, the sub-surface is
	/// unmapped.
	/// </remarks>
	public abstract class IWlSubsurface : IWaylandObject {
		protected IWlSubsurface(Client owner, uint? id) : base(owner, id) {}
		public static class Enum {
			public enum Error {
				/// <summary>
				/// Wl_surface is not a sibling or the parent
				/// </summary>
				BadSurface = 0,
			}
		}
		internal override void ProcessRequest(int opcode, int mlen) {
			var tbuf = new byte[mlen];
			var _offset = 0;
			switch(opcode) {
				case 0: {
					Owner.Socket.Read(tbuf);
					Destroy();
					break;
				}
				case 1: {
					Owner.Socket.Read(tbuf);
					var x = (int) Helper.ReadInt(tbuf, ref _offset);
					var y = (int) Helper.ReadInt(tbuf, ref _offset);
					SetPosition(x, y);
					break;
				}
				case 2: {
					Owner.Socket.Read(tbuf);
					var sibling = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					PlaceAbove(sibling);
					break;
				}
				case 3: {
					Owner.Socket.Read(tbuf);
					var sibling = Owner.GetObject<IWlSurface>(Helper.ReadUint(tbuf, ref _offset));
					PlaceBelow(sibling);
					break;
				}
				case 4: {
					Owner.Socket.Read(tbuf);
					SetSync();
					break;
				}
				case 5: {
					Owner.Socket.Read(tbuf);
					SetDesync();
					break;
				}
			}
		}
		/// <summary>
		/// Remove sub-surface interface
		/// </summary>
		/// <remarks>
		/// The sub-surface interface is removed from the wl_surface object
		/// that was turned into a sub-surface with a
		/// wl_subcompositor.get_subsurface request. The wl_surface's association
		/// to the parent is deleted, and the wl_surface loses its role as
		/// a sub-surface. The wl_surface is unmapped immediately.
		/// </remarks>
		public abstract void Destroy();
		/// <summary>
		/// Reposition the sub-surface
		/// </summary>
		/// <remarks>
		/// This schedules a sub-surface position change.
		/// The sub-surface will be moved so that its origin (top left
		/// corner pixel) will be at the location x, y of the parent surface
		/// coordinate system. The coordinates are not restricted to the parent
		/// surface area. Negative values are allowed.
		/// 
		/// The scheduled coordinates will take effect whenever the state of the
		/// parent surface is applied. When this happens depends on whether the
		/// parent surface is in synchronized mode or not. See
		/// wl_subsurface.set_sync and wl_subsurface.set_desync for details.
		/// 
		/// If more than one set_position request is invoked by the client before
		/// the commit of the parent surface, the position of a new request always
		/// replaces the scheduled position from any previous request.
		/// 
		/// The initial position is 0, 0.
		/// </remarks>
		/// <param name="x">X coordinate in the parent surface</param>
		/// <param name="y">Y coordinate in the parent surface</param>
		public abstract void SetPosition(int x, int y);
		/// <summary>
		/// Restack the sub-surface
		/// </summary>
		/// <remarks>
		/// This sub-surface is taken from the stack, and put back just
		/// above the reference surface, changing the z-order of the sub-surfaces.
		/// The reference surface must be one of the sibling surfaces, or the
		/// parent surface. Using any other surface, including this sub-surface,
		/// will cause a protocol error.
		/// 
		/// The z-order is double-buffered. Requests are handled in order and
		/// applied immediately to a pending state. The final pending state is
		/// copied to the active state the next time the state of the parent
		/// surface is applied. When this happens depends on whether the parent
		/// surface is in synchronized mode or not. See wl_subsurface.set_sync and
		/// wl_subsurface.set_desync for details.
		/// 
		/// A new sub-surface is initially added as the top-most in the stack
		/// of its siblings and parent.
		/// </remarks>
		/// <param name="sibling">The reference surface</param>
		public abstract void PlaceAbove(IWlSurface sibling);
		/// <summary>
		/// Restack the sub-surface
		/// </summary>
		/// <remarks>
		/// The sub-surface is placed just below the reference surface.
		/// See wl_subsurface.place_above.
		/// </remarks>
		/// <param name="sibling">The reference surface</param>
		public abstract void PlaceBelow(IWlSurface sibling);
		/// <summary>
		/// Set sub-surface to synchronized mode
		/// </summary>
		/// <remarks>
		/// Change the commit behaviour of the sub-surface to synchronized
		/// mode, also described as the parent dependent mode.
		/// 
		/// In synchronized mode, wl_surface.commit on a sub-surface will
		/// accumulate the committed state in a cache, but the state will
		/// not be applied and hence will not change the compositor output.
		/// The cached state is applied to the sub-surface immediately after
		/// the parent surface's state is applied. This ensures atomic
		/// updates of the parent and all its synchronized sub-surfaces.
		/// Applying the cached state will invalidate the cache, so further
		/// parent surface commits do not (re-)apply old state.
		/// 
		/// See wl_subsurface for the recursive effect of this mode.
		/// </remarks>
		public abstract void SetSync();
		/// <summary>
		/// Set sub-surface to desynchronized mode
		/// </summary>
		/// <remarks>
		/// Change the commit behaviour of the sub-surface to desynchronized
		/// mode, also described as independent or freely running mode.
		/// 
		/// In desynchronized mode, wl_surface.commit on a sub-surface will
		/// apply the pending state directly, without caching, as happens
		/// normally with a wl_surface. Calling wl_surface.commit on the
		/// parent surface has no effect on the sub-surface's wl_surface
		/// state. This mode allows a sub-surface to be updated on its own.
		/// 
		/// If cached state exists when wl_surface.commit is called in
		/// desynchronized mode, the pending state is added to the cached
		/// state, and applied as a whole. This invalidates the cache.
		/// 
		/// Note: even if a sub-surface is set to desynchronized, a parent
		/// sub-surface may override it to behave as synchronized. For details,
		/// see wl_subsurface.
		/// 
		/// If a surface's parent surface behaves as desynchronized, then
		/// the cached state is applied on set_desync.
		/// </remarks>
		public abstract void SetDesync();
	}
}
