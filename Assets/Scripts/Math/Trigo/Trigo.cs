namespace Server.Math {
	internal static partial class Trigo: object {
		#region Fields
		#endregion

		#region Properties

		internal static System.Type Type { //So classes no need to find it in their defs
			get;
			private set;
		}

		#endregion

		#region Ctors and Dtor

		static Trigo() {
			Type = typeof(Trigo);
		}

		#endregion
	}
}