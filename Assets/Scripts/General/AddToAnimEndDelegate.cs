using Server.Anim;
using UnityEngine;
using UnityEngine.Events;
using static Server.General.InitIDs;

namespace Server.General {
    internal sealed class AddToAnimEndDelegate: MonoBehaviour {
		#region Fields

		[SerializeField]
		private InitControl initControl;

		private bool shldTrigger;

		[SerializeField]
		private AbstractAnim anim;

		[SerializeField]
		public UnityEvent myUnityEvent;

		#endregion

		#region Properties
		#endregion

		#region Ctors and Dtor

		internal AddToAnimEndDelegate(): base() {
			initControl = null;

			shldTrigger = false;

			anim = null;

			myUnityEvent = null;
        }

        static AddToAnimEndDelegate() {
        }

		#endregion

		#region Unity User Callback Event Funcs

		private void OnEnable() {
			if(initControl != null) {
				initControl.AddMethod((uint)InitID.AddToAnimEndDelegate, Init);
			} else {
				Init(); //Workaround
			}
		}

		private void OnDisable() {
			if(initControl != null) {
				initControl.RemoveMethod((uint)InitID.AddToAnimEndDelegate, Init);
			}
		}

		#endregion

		private void Init() {
			anim.animEndDelegate += () => {
				if(shldTrigger) {
					myUnityEvent?.Invoke();
					shldTrigger = false;
				}
			};
		}

		public void YesToShldTrigger() {
			shldTrigger = true;
		}
	}
}