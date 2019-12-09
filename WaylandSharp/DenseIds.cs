using System.Collections.Generic;
using System.Linq;

namespace WaylandSharp {
	public class DenseIds {
		readonly SortedSet<uint> Freed = new SortedSet<uint>();
		readonly uint Min, Max;
		uint Current;

		public DenseIds(uint min, uint max) {
			Current = Min = min;
			Max = max;
		}

		public uint Next() {
			if(Freed.Count != 0) {
				var id = Freed.First();
				Freed.Remove(id);
				return id;
			}
			return Current++;
		}

		public void Free(uint id) => Freed.Add(id);
	}
}