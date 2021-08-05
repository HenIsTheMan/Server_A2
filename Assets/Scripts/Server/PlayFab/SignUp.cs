using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Server.General.SignUpStatuses;

namespace Server.PlayFab {
    internal sealed class SignUp: MonoBehaviour {
        #region Fields

        private bool canClick;

        [SerializeField]
        private TMP_InputField usernameInputField;

        [SerializeField]
        private TMP_InputField emailInputField;

        [SerializeField]
        private TMP_InputField newPasswordInputField;

        [SerializeField]
        private TMP_InputField confirmPasswordInputField;

        [SerializeField]
        private UnityEvent onRegistrationSuccess;

        [SerializeField]
        private UnityEvent onRegistrationFailure;

        [SerializeField]
        private TMP_Text signUpMsgTmp;

        [SerializeField]
        private EllipsesControl signUpEllipsesControl;

        [EnumIndices(typeof(SignUpStatus)), SerializeField]
        private string[] signUpMsgs;

        [EnumIndices(typeof(SignUpStatus)), SerializeField]
        private Color[] signUpMsgColors;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SignUp(): base() {
            usernameInputField = null;
            emailInputField = null;
            newPasswordInputField = null;
            confirmPasswordInputField = null;

            onRegistrationSuccess = null;
            onRegistrationFailure = null;

            signUpMsgTmp = null;
            signUpEllipsesControl = null;
            signUpMsgs = System.Array.Empty<string>();
            signUpMsgColors = System.Array.Empty<Color>();
        }

        static SignUp() {
        }

        #endregion

        #region Unity User Callback Event Funcs 
 
        private void Awake() { 
            signUpMsgTmp.text = string.Empty; 
        } 
 
        #endregion 
 
        public void OnClick() { 
 
        } 
 
        private void ShowSignInMsg(SignUpStatus status, bool shldSetCanClick = true) { 
            if(shldSetCanClick) { 
                canClick = true; 
            } 
 
            signUpMsgTmp.text = signUpMsgs[(int)status]; 
            signUpMsgTmp.color = signUpMsgColors[(int)status]; 
        }

        //private void OnRegistrationSuccess(RegistrationResult _) { 
        //    Console.Log("User Registration Successful!"); 

        //    signUpEllipsesControl.enabled = false; 
        //    ShowSignInMsg(SignUpStatus.Success); 

        //    onRegistrationSuccess?.Invoke(); 
        //}

        //private void OnRegistrationFailure() {
        //    onRegistrationFailure?.Invoke();
        //}
    }
}