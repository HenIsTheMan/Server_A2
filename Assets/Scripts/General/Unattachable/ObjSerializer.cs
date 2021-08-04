using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.General {
	internal static class ObjSerializer: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Ctors and Dtor

        static ObjSerializer() {
        }

		#endregion

		internal static byte[] Serialize(this object obj) {
			if(obj == null) {
				return null;
			}

			var bf = new BinaryFormatter();
			using(var ms = new MemoryStream()) {
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		internal static T Deserialize<T>(this byte[] byteArr) where T: class {
			if(byteArr == null) {
				return null;
			}

			using(var memStream = new MemoryStream()) {
				var binForm = new BinaryFormatter();
				memStream.Write(byteArr, 0, byteArr.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				var obj = (T)binForm.Deserialize(memStream);
				return obj;
			}
		}
	}
}