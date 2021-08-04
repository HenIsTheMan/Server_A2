using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.General {
    internal static class ObjCopier: object {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        static ObjCopier() {
        }

		#endregion

		internal static T DeepCopy<T>(this T src) {
			if(!typeof(T).IsSerializable) {
				throw new ArgumentException("T is not serializable!", "src");
			}

			if(src is null) {
				return default; //default(T) also can
			}

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();

			using(stream) {
				formatter.Serialize(stream, src);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
			}
		}
	}
}