using UnityEngine;

namespace Server.General {
    internal sealed class SkyMtlControl: MonoBehaviour {
		#region Fields

		[SerializeField]
		private float rotationVel;

		[SerializeField]
		private Material skyMtl;

		#endregion

		#region Properties
		#endregion

		#region Ctors and Dtor

		internal SkyMtlControl(): base() {
			rotationVel = 0.0f;
			skyMtl = null;
		}

        static SkyMtlControl() {
        }

		#endregion

		#region Unity User Callback Event Funcs

		private void FixedUpdate() {
			skyMtl.SetFloat("_Rotation", Time.time * rotationVel);
		}

		private void OnDisable() {
			skyMtl.SetFloat("_Rotation", 0.0f);
		}

		#endregion
	}
}