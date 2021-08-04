using UnityEngine;
using UnityEngine.UI;

namespace Server.General {
	internal sealed partial class CommonSceneTransitionButton: MonoBehaviour {
		public static void MenuSceneToLobbyScenePrep() {
			SceneManager.globalObj.UnloadScene(
				UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
				UnloadSceneTypes.UnloadSceneType.UnloadAllEmbeddedSceneObjs,
				null
			);

			PlayerPrefs.SetInt("menuIndex", 1);

			GameObject backButtonGO = GameObject.Find("BackButton");
			Button button = backButtonGO.GetComponent<Button>();
			button.onClick.AddListener(() => {
				backButtonGO.GetComponent<CommonSceneTransitionButton>().OnClick();
				button.GetComponent<PlayAudio>().PlaySound("Press0");
			});
		}
    }
}