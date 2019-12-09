using System;
using System.Collections.Generic;
using WaylandSharp.Generated;

namespace WaylandSharp {
	public class WlRegistry : IWlRegistry {
		readonly Dictionary<uint, IWaylandObject> Globals = new Dictionary<uint,IWaylandObject>();
		readonly DenseIds Ids = new DenseIds(1, uint.MaxValue);
		bool BeenSetup = false;
		
		public WlRegistry(Client owner, uint? id = null) : base(owner, id) { }

		internal override void Setup() {
			Console.WriteLine("wl_registry setup???");
			BeenSetup = true;
			foreach(var elem in Globals)
				Global(elem.Key, elem.Value.InterfaceName, (uint) elem.Value.InterfaceVersion);
		}

		public override void Bind(uint name, out IWaylandObject id) =>
			id = Globals[name];

		internal void Add(IWaylandObject obj) {
			var name = Ids.Next();
			Globals[name] = obj;
			if(BeenSetup)
				Global(name, obj.InterfaceName, (uint) obj.InterfaceVersion);
		}
	}
}