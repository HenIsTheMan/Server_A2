using UnityEngine;

namespace Server.General {
	internal sealed partial class CommonSceneTransitionButton: MonoBehaviour {
		public static void JustUnloadPrep() {
			SceneManager.globalObj.UnloadScene(
				UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
				UnloadSceneTypes.UnloadSceneType.UnloadAllEmbeddedSceneObjs,
				null
			);
		}
    }
}