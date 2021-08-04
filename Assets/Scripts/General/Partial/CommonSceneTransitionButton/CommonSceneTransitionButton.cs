using UnityEngine;
using static Server.General.InitIDs;
using static Server.General.LoadSceneTypes;

namespace Server.General {
    internal sealed partial class CommonSceneTransitionButton: MonoBehaviour {
		public delegate void ScenePrepDelegate();

		#region Fields

		[SerializeField]
		private InitControl initControl;

		private bool isAlrClicked;

		[SerializeField]
		internal string sceneName;

		[SerializeField]
		internal string scenePrepMethodName;

		private ScenePrepDelegate scenePrepDelegate;

		#endregion

		#region Properties
		#endregion

		#region Ctors and Dtor

		internal CommonSceneTransitionButton(): base() {
			initControl = null;

			isAlrClicked = false;

			sceneName = string.Empty;

			scenePrepMethodName = string.Empty;
			scenePrepDelegate = null;
        }

        static CommonSceneTransitionButton() {
        }

		#endregion

		#region Unity User Callback Event Funcs

		private void OnEnable() {
			if(initControl != null) {
				initControl.AddMethod((uint)InitID.CommonSceneTransitionButton, Init);
			} else {
				Init(); //Workaround
			}
		}

		private void OnDisable() {
			if(initControl != null) {
				initControl.RemoveMethod((uint)InitID.CommonSceneTransitionButton, Init);
			}
		}

		#endregion

		private void Init() {
			scenePrepDelegate = scenePrepMethodName == string.Empty
				? null
				: (ScenePrepDelegate)GetType().GetMethod(scenePrepMethodName).CreateDelegate(typeof(ScenePrepDelegate));
		}

		public void SpecialOnClick() { //Ewwww
			if(transform.parent.childCount == 1 && !isAlrClicked) { //If no siblings and...
				OnClick();
				isAlrClicked = true;
			}
		}

		public void OnClick() {
			ExpandAndContract myComponent = FindObjectOfType<ExpandAndContract>();
			if(myComponent != null) {
				myComponent.animsForExpansion[0].animEndDelegate += () => {
					SceneManager.globalObj.LoadScene(sceneName, LoadSceneType.Additive, () => {
						scenePrepDelegate?.Invoke();

						myComponent.Contract();

						myComponent.animsForExpansion[0].animEndDelegate = null;
					});
				};

				myComponent.Expand();
			}
		}

		public static void UnloadSceneOnClick(ScenePrepDelegate myScenePrepDelegate) { //Ewwww
			ExpandAndContract myComponent = FindObjectOfType<ExpandAndContract>();
			if(myComponent != null) {
				myComponent.animsForExpansion[0].animEndDelegate += () => {
					SceneManager.globalObj.UnloadScene(
						UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
						UnloadSceneTypes.UnloadSceneType.UnloadAllEmbeddedSceneObjs,
						() => {
							myScenePrepDelegate?.Invoke();

							myComponent.Contract();

							myComponent.animsForExpansion[0].animEndDelegate = null;
						}
					);
				};

				myComponent.Expand();
			}
		}
    }
}