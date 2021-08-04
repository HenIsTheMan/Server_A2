using UnityEngine;

namespace Server.General {
	internal sealed partial class LoadSceneImmediate: MonoBehaviour {
		public static void BootToSplashDone() {
			PlayerPrefs.SetInt("menuIndex", 0);

			PtrManager ptrManager = PtrManager.globalObj;
			ptrManager.displacementFromCam = 5.0f;
			ptrManager.camComponent = FindObjectOfType<Camera>();
			ptrManager.ChangeCursorCentered("GenesisCursor", CursorModes.CursorMode.Auto);
		}
	}
}