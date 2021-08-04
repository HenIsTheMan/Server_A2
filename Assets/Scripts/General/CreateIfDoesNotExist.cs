using UnityEngine;

namespace Server.General {
    internal sealed class CreateIfDoesNotExist: MonoBehaviour {
        #region Fields

		[SerializeField]
		private GameObject prefab;

		[SerializeField]
		private string myName;

		[SerializeField]
		private Vector3 pos;

		[SerializeField]
		private Quaternion rotation;

		[SerializeField]
		private Transform parentTransform;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal CreateIfDoesNotExist(): base() {
			prefab = null;
			myName = string.Empty;

			pos = Vector3.zero;
			rotation = Quaternion.identity;
			parentTransform = null;
        }

        static CreateIfDoesNotExist() {
        }

		#endregion

		#region Unity User Callback Event Funcs

		private void Awake() { //I guess
			if(!GameObject.Find(myName)) {
				Instantiate(prefab, pos, rotation, parentTransform).name = myName;
			}
			Destroy(gameObject);
		}

		#endregion
	}
}