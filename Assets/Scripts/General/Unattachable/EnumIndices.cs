using UnityEngine;
using System;

namespace Server.General {
    internal sealed class EnumIndices: PropertyAttribute {
        #region Fields

		[SerializeField]
		internal string[] names;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        private EnumIndices(): base() {
			names = Array.Empty<string>();
        }

		internal EnumIndices(Type type): base() {
			names = Enum.GetNames(type);
		}

		static EnumIndices() {
        }

        #endregion
    }
}