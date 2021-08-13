using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Server.General {
    internal sealed class ObjPool: MonoBehaviour {
		#region Fields

		private List<GameObject> activeObjs;
		private List<GameObject> inactiveObjs;

		#endregion

		#region Properties
		#endregion

		#region Ctors and Dtor

		internal ObjPool(): base() {
			activeObjs = null;
			inactiveObjs = null;
        }

        static ObjPool() {
        }

		#endregion

		#region Unity User Callback Event Funcs
		#endregion

		internal void InitMe(int size, [JetBrains.Annotations.NotNull] GameObject prefab, Transform parentTransform, bool worldPosStays) {
			activeObjs = new List<GameObject>(size);
			inactiveObjs = new List<GameObject>(size);

			for(int i = 0; i < size; ++i) {
				GameObject GO = Instantiate(prefab);
				GO.transform.SetParent(parentTransform, worldPosStays);

				GO.SetActive(false);
				inactiveObjs.Add(GO);
			}
		}

		internal GameObject ActivateObj() {
			GameObject GO = inactiveObjs[0];
			GO.SetActive(true);
			inactiveObjs.RemoveAt(0);
			activeObjs.Add(GO);
			return GO;
		}

		internal void DeactivateObj(GameObject obj) {
			GameObject GO = activeObjs.Where(x => x == obj).SingleOrDefault();

			if(GO != null && activeObjs.Remove(GO)) {
				GO.SetActive(false);
				inactiveObjs.Add(GO);
			}
		}
    }
}