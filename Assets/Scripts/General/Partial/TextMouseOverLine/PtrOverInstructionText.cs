using Server.Anim;
using TMPro;
using UnityEngine;

namespace Server.General {
    internal sealed partial class TextPtrOverLineOfWords: MonoBehaviour {
		public static void PtrOverInstructionText() {
			GameObject instructionText = globalObj.tmpTextComponent.gameObject;
			RectTransformScaleAnim[] scaleAnimScripts = instructionText.GetComponents<RectTransformScaleAnim>();

			scaleAnimScripts[0].IsUpdating = false;
			scaleAnimScripts[1].IsUpdating = false;

			instructionText.transform.localScale = Vector3.one;
			instructionText.GetComponent<TextMeshProUGUI>().color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
		}
	}
}