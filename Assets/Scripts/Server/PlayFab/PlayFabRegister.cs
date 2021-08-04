using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace Server {
    internal sealed class PlayFabRegister: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal PlayFabRegister(): base() {
        }

        static PlayFabRegister() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnRegisterClicked() {
            RegisterPlayFabUserRequest req = new RegisterPlayFabUserRequest {
                //Email = "jimeroyesyes@test.com",
                //DisplayName = "johnjohn",
                Email = "abc@test.com",
                DisplayName = "hola",
                Password = "123456",
                RequireBothUsernameAndEmail = false
            };
            PlayFabClientAPI.RegisterPlayFabUser(req,
            // Callback function to handle successful registration
            // You can display the messages to the interface from here
            res => {
                Debug.Log("User is successfully registered");
            },
            // Callback function for any error encountered
            // You can display the messages to the interface from here
            err => {
                Debug.Log("Error: " + err.ErrorMessage);
            }
            );
        }
    }
}