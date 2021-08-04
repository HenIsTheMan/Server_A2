using UnityEngine;
using static Server.General.InitIDs;

namespace Server.General {
    internal sealed class EnableCamDelayed: MonoBehaviour {
		#region Fields

		[SerializeField]
		private InitControl initControl;

		[SerializeField]
		private Camera cam;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal EnableCamDelayed(): base() {
			initControl = null;

			cam = null;
        }

        static EnableCamDelayed() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void OnValidate() {
			UnityEngine.Assertions.Assert.IsFalse(cam.enabled, "cam.enabled");
		}

		private void OnEnable() {
			initControl.AddMethod((uint)InitID.EnableCamDelayed, Init);
		}

		private void OnDisable() {
			initControl.RemoveMethod((uint)InitID.EnableCamDelayed, Init);
		}

		#endregion

		private void Init() {
			_ = StartCoroutine(nameof(MyFunc));
		}

		private System.Collections.IEnumerator MyFunc() {
			yield return null;

			cam.enabled = true;
		}
	}
}