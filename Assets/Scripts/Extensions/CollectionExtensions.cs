using System.Collections.Generic;

namespace ExtensionsPlus {
	
	public static class CollectionExtensions {
#region Lists
		public static bool IsNullOrEmpty<T>(this List<T> list) {
			return list == null || list.Count <= 0;
		}

		public static bool IsEmpty<T>(this List<T> list) {
			return list.Count <= 0;
		}
#endregion

#region Arrays
		public static bool IsNullOrEmpty<T>(this T[] array) {
			return array == null || array.Length <= 0;
		}
#endregion
	}
}