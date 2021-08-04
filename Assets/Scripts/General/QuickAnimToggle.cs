using Server.Anim;
using UnityEngine;

namespace Server.General {
    internal sealed class QuickAnimToggle: MonoBehaviour {
		#region Fields

		[SerializeField]
		internal AbstractAnim[] anims;

		#endregion

		#region Properties
		#endregion

		#region Ctors and Dtor

		internal QuickAnimToggle(): base() {
			anims = System.Array.Empty<AbstractAnim>();
        }

        static QuickAnimToggle() {
        }

		#endregion

		#region Unity User Callback Event Funcs
		#endregion

		public void TurnOnAnimsIfLMB() {
			if(Input.GetMouseButton(0)) {
				TurnOnAnims();
			}
		}

		public void TurnOnAnimsIfNotLMB() {
			if(!Input.GetMouseButton(0)) {
				TurnOnAnims();
			}
		}

		public void TurnOnAnims() {
			foreach(AbstractAnim anim in anims) {
				anim.IsUpdating = true;
			}
		}

		public void TurnOffAnims() {
			foreach(AbstractAnim anim in anims) {
				anim.IsUpdating = false;
			}
		}
	}
}