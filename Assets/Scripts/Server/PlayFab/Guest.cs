using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Guest: MonoBehaviour {
        #region Fields

        [SerializeField]
        private int len;

        [SerializeField]
        private int[] key;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Guest(): base() {
            len = 0;
            key = System.Array.Empty<int>();
        }

        static Guest() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void OnValidate() {
            UnityEngine.Assertions.Assert.IsTrue(key.Length == 4);

            UnityEngine.Assertions.Assert.IsTrue(key[0] * key[3] - key[1] * key[2] == 1); //For keyInverse to have int elements/entries
        }

        #endregion

        public void OnClick() {
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest {
                    CustomId = GenEncryptedCustomID(len, key),
                    CreateAccount = true
                },
                OnLoginSuccess,
                OnLoginFailure
            );
        }

        private string GenEncryptedCustomID(int len, int[] key) {
            string customID = string.Empty;

            for(int i = 0; i < len; ++i) {
                customID += (char)Random.Range(33, 127);
            }

            int[] valsASCII = new int[(len & 1) == 1 ? len + 1 : len];
            int valsASCIILen = valsASCII.Length;

            for(int i = 0; i < len; ++i) {
                valsASCII[i] = customID[i];
            }
            if((len & 1) == 1) {
                valsASCII[valsASCIILen - 1] = -1; //Invalid val
            }

            int[] encryptedValsASCII = new int[valsASCIILen];
            int limit = valsASCIILen / 2;
            int index0;
            int index1;

            for(int i = 0; i < limit; ++i) {
                index0 = 2 * i;
                index1 = index0 + 1;

                encryptedValsASCII[index0] = key[0] * valsASCII[index0] + key[2] * valsASCII[index1];
                encryptedValsASCII[index1] = key[1] * valsASCII[index0] + key[3] * valsASCII[index1];
            }

            JSONNode node = new JSONArray();

            for(int i = 0; i < valsASCIILen; ++i) {
                node.Add(encryptedValsASCII[i]);
            }

            return node.ToString();
        }

        private void OnLoginSuccess(LoginResult _) {
            Console.Log("Guest Login Successful!");
        }

        private void OnLoginFailure(PlayFabError error) {
            Console.Log("Guest Login Failed (" + error.GenerateErrorReport() + ")!");
        }
    }
}