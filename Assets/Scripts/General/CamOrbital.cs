using UnityEngine;
using static Server.General.InitIDs;

namespace Server.General {
	[RequireComponent(typeof(Camera))]
	internal sealed class CamOrbital: MonoBehaviour {
		#region Fields

		[SerializeField]
		private InitControl initControl;

		private bool isOrbiting;
		private float fixedTimeMark;

		private WaitForSeconds startingWaitForSeconds;
		private WaitForSeconds orbitingWaitForSeconds;

		[Min(0.0f), SerializeField]
		private float startTimeOffset;

		[Min(0.0f), SerializeField]
		private float orbitalDuration;

		[SerializeField]
		private float radius;

		[SerializeField]
		private Transform targetTransform;

		[SerializeField]
		private Vector3 posOffset;

		[SerializeField]
		private Vector3 rotationOffset;

		[SerializeField]
		private Vector3 rotationVel;

		#endregion

		#region Properties

		internal bool IsOrbiting {
			get {
				return isOrbiting;
			}
		}

        #endregion

        #region Ctors and Dtor

        internal CamOrbital(): base() {
			initControl = null;

			isOrbiting = false;
			fixedTimeMark = 0.0f;

			startingWaitForSeconds = null;
			orbitingWaitForSeconds = null;

			startTimeOffset = 0.0f;
			orbitalDuration = 0.0f;

			radius = 0.0f;

			targetTransform = null;

			posOffset = Vector3.zero;
			rotationOffset = Vector3.zero;
			rotationVel = Vector3.zero;
		}

        static CamOrbital() {
        }

		#endregion

		#region Unity User Callback Event Funcs

		private void OnEnable() {
			initControl.AddMethod((uint)InitID.CamOrbital, Init);
		}

		private void FixedUpdate() {
			if(isOrbiting) {
				transform.localRotation = Quaternion.Euler(rotationVel * (Time.fixedTime - fixedTimeMark) + rotationOffset);

				transform.localPosition = targetTransform.position - transform.forward * radius + posOffset;
			}
		}

		private void OnDisable() {
			initControl.RemoveMethod((uint)InitID.CamOrbital, Init);
		}

		#endregion

		private void Init() {
			transform.localPosition = targetTransform.position - transform.forward * radius + posOffset;
			transform.localRotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);

			if(!Mathf.Approximately(startTimeOffset, 0.0f)) {
				startingWaitForSeconds = new WaitForSeconds(startTimeOffset);
			}

			if(!Mathf.Approximately(orbitalDuration, 0.0f)) {
				orbitingWaitForSeconds = new WaitForSeconds(orbitalDuration);
			}

			_ = StartCoroutine(nameof(OrbitalControlFunc));
		}

		private System.Collections.IEnumerator OrbitalControlFunc() {
			if(startingWaitForSeconds != null) {
				yield return startingWaitForSeconds;
			}

			isOrbiting = true;
			fixedTimeMark = Time.fixedTime;

			if(orbitingWaitForSeconds != null) {
				yield return orbitingWaitForSeconds;
			}

			isOrbiting = false;
			enabled = false;
		}
	}
}