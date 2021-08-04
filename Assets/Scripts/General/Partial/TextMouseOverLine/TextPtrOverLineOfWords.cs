using TMPro;
using UnityEngine;
using static Server.General.InitIDs;

namespace Server.General {
    internal sealed partial class TextPtrOverLineOfWords: MonoBehaviour {
		private delegate void MyDelegate();

		#region Fields

		[SerializeField]
		private InitControl initControl;

		private bool isPtrOver;

		private Rect rect;
		private Vector2 pt;

		[SerializeField]
		private TMP_Text tmpTextComponent;

		[SerializeField]
		private string ptrOverMethodName;

		[SerializeField]
		private string ptrNotOverMethodName;

		private MyDelegate ptrOverDelegate;
		private MyDelegate ptrNotOverDelegate;

		private static TextPtrOverLineOfWords globalObj;

		#endregion

		#region Properties
		#endregion

		#region Ctors and Dtor

		internal TextPtrOverLineOfWords(): base() {
			initControl = null;

			isPtrOver = false;

			rect = Rect.zero;
			pt = Vector2.zero;

			tmpTextComponent = null;
			ptrOverMethodName = string.Empty;
			ptrNotOverMethodName = string.Empty;

			ptrOverDelegate = null;
			ptrNotOverDelegate = null;
		}

        static TextPtrOverLineOfWords() {
			globalObj = null;
		}

		#endregion

		#region Unity User Callback Event Funcs

		private void OnEnable() {
			initControl.AddMethod((uint)InitID.TextPtrOverLineOfWords, Init);
		}

		private void Update() {
			rect.width = tmpTextComponent.textBounds.size.x;
			rect.height = tmpTextComponent.textBounds.size.y;

			rect.x = Screen.width * 0.5f
				+ tmpTextComponent.transform.localPosition.x
				- rect.width * 0.5f;
			rect.y = Screen.height * 0.5f
				- tmpTextComponent.transform.localPosition.y
				- rect.height * 0.5f;

			pt.x = Input.mousePosition.x;
			pt.y = Screen.height - Input.mousePosition.y;

			if(!isPtrOver && rect.Contains(pt)) {
				ptrOverDelegate?.Invoke();
				isPtrOver = true;
			} else if(isPtrOver && !rect.Contains(pt)) {
				ptrNotOverDelegate?.Invoke();
				isPtrOver = false;
			}
		}

		private void OnDisable() {
			initControl.RemoveMethod((uint)InitID.TextPtrOverLineOfWords, Init);
		}

		#endregion

		private void Init() {
			globalObj = this;

			if(ptrOverMethodName != string.Empty) {
				ptrOverDelegate = (MyDelegate)GetType()
					.GetMethod(ptrOverMethodName)
					.CreateDelegate(typeof(MyDelegate));
			}

			if(ptrNotOverMethodName != string.Empty) {
				ptrNotOverDelegate = (MyDelegate)GetType()
					.GetMethod(ptrNotOverMethodName)
					.CreateDelegate(typeof(MyDelegate));
			}

			rect = ((RectTransform)tmpTextComponent.transform).rect;
			tmpTextComponent.ForceMeshUpdate();
		}
	}
}