#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Server.General {
	[CustomPropertyDrawer(typeof(EnumIndices))]
	internal sealed class EnumIndicesPropertyDrawer: PropertyDrawer {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        public EnumIndicesPropertyDrawer(): base() {
        }

        static EnumIndicesPropertyDrawer() {
        }

		#endregion

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EnumIndices attrib = attribute as EnumIndices;

			label.text = attrib.names[System.Convert.ToInt32(
				property.propertyPath.Substring(property.propertyPath.IndexOf("[")).Replace("[", "").Replace("]", "")
			)];

			EditorGUI.PropertyField(position, property, label, true);
		}
	}
}

#endif