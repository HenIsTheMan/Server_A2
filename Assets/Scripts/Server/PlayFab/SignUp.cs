using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Server.PlayFab.SignUpStatuses;

namespace Server.PlayFab {
    internal sealed class SignUp: MonoBehaviour {
        #region Fields

        private bool canClick;

        private int doneCount;

        private string email;

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
            canClick = true;

            doneCount = 0;

            email = string.Empty;

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

        private void OnValidate() {
            UnityEngine.Assertions.Assert.IsTrue(signUpMsgs.Length == (int)SignUpStatus.Amt + 1);
            UnityEngine.Assertions.Assert.IsTrue(signUpMsgColors.Length == (int)SignUpStatus.Amt + 1);
        }

        private void Awake() { 
            signUpMsgTmp.text = string.Empty; 
        } 
 
        #endregion 
 
        public void OnClick() {
            if(!canClick) {
                return;
            }
            canClick = false;

            SignUpStatus status = SignUpStatus.None;

            string username = usernameInputField.text;
            if(string.IsNullOrEmpty(username)) {
                status = SignUpStatus.NoUsername;
                goto ShowSignInMsg;
            }

            email = emailInputField.text;
            if(string.IsNullOrEmpty(email)) {
                status = SignUpStatus.NoEmail;
                goto ShowSignInMsg;
            }

            string newPassword = newPasswordInputField.text;
            if(string.IsNullOrEmpty(newPassword)) {
                status = SignUpStatus.NoNewPassword;
                goto ShowSignInMsg;
            }

            string confirmPassword = confirmPasswordInputField.text;
            if(string.IsNullOrEmpty(confirmPassword)) {
                status = SignUpStatus.NoConfirmPassword;
                goto ShowSignInMsg;
            } else if(newPassword != confirmPassword) {
                status = SignUpStatus.PasswordsNotMatching;
                goto ShowSignInMsg;
            }

            if(username.Contains(' ')) {
                status = SignUpStatus.SpacesInUsername;
                goto ShowSignInMsg;
            }

            if(email.Contains(' ')) {
                status = SignUpStatus.SpacesInEmail;
                goto ShowSignInMsg;
            }

            int usernameLen = username.Length;
            if(usernameLen < 3 || usernameLen > 20) {
                status = SignUpStatus.InvalidUsernameLen;
                goto ShowSignInMsg;
            }

            int passwordLen = newPassword.Length;
            if(passwordLen < 6 || passwordLen > 100) {
                status = SignUpStatus.InvalidPasswordLen;
                goto ShowSignInMsg;
            }

            if(!username.All(char.IsLetterOrDigit)) {
                status = SignUpStatus.UsernameHasInvalidChars;
                goto ShowSignInMsg;
            }

            int atIndex = email.IndexOf('@');
            int dotIndex = email.IndexOf('.');
            int emailLen = email.Length;

            if(email.Count(myChar => myChar == '@') != 1
                || email.Count(myChar => myChar == '.') != 1
                || atIndex < 1
                || dotIndex < 3
                || atIndex > emailLen - 4
                || dotIndex > emailLen - 2
                || (atIndex >= dotIndex - 1)
            ) {
                status = SignUpStatus.InvalidEmail;
                goto ShowSignInMsg;
            } else {
                string substr0 = email.Substring(0, atIndex);
                string substr1 = email.Substring(atIndex + 1, dotIndex - atIndex - 1);
                string substr2 = email.Substring(dotIndex + 1, emailLen - dotIndex - 1);

                if(substr2.Length < 2
                    || !substr0.All(char.IsLetterOrDigit)
                    || !substr1.All(char.IsLetterOrDigit)
                    || !substr2.All(char.IsLetter)
                ) {
                    status = SignUpStatus.InvalidEmail;
                    goto ShowSignInMsg;
                }
            }

            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest {
                DisplayName = username,
                Email = email,
                Username = username,
                Password = newPassword,
            };

            PlayFabClientAPI.RegisterPlayFabUser(
                request,
                OnRegistrationSuccess,
                OnRegistrationFailure
            );

            signUpEllipsesControl.enabled = true;
            ShowSignInMsg(SignUpStatus.Processing, false);

            return;

        ShowSignInMsg:
            ShowSignInMsg(status);
        } 
 
        private void ShowSignInMsg(SignUpStatus status, bool shldSetCanClick = true) { 
            if(shldSetCanClick) {
                canClick = true;
            } 
 
            signUpMsgTmp.text = signUpMsgs[(int)status]; 
            signUpMsgTmp.color = signUpMsgColors[(int)status]; 
        }

		private void OnRegistrationSuccess(RegisterPlayFabUserResult result) {
            Console.Log("OG User Registration Successful!");

            doneCount = 0;

            PlayFabClientAPI.AddOrUpdateContactEmail(
                new AddOrUpdateContactEmailRequest {
                    EmailAddress = email
                },
                OnAddOrUpdateContactEmailSuccess,
                OnAddOrUpdateContactEmailFailure
            );

            string val = new SimpleJSON.JSONArray().ToString();

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "UpdateUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = result.PlayFabId,
                        Keys = new string[2] {
                            "FriendRequests",
                            "TradeRequests",
                        },
                        Vals = new string[2] {
                            val,
                            val
                        }
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptUpdateSuccess,
                OnExecuteCloudScriptUpdateFailure
            );
        }

        private void OnExecuteCloudScriptUpdateSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptUpdateSuccess!");

            if(doneCount == 1) {
                FullUserRegistrationSuccessful();
            } else {
                ++doneCount;
            }
        }

        private void OnExecuteCloudScriptUpdateFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptUpdateFailure!");
        }

        private void OnAddOrUpdateContactEmailSuccess(AddOrUpdateContactEmailResult _) {
            Console.Log("AddOrUpdateContactEmailSuccess!");

            if(doneCount == 1) {
                FullUserRegistrationSuccessful();
            } else {
                ++doneCount;
            }
        }

        private void FullUserRegistrationSuccessful() {
            Console.Log("Full User Registration Successful!");

            signUpEllipsesControl.enabled = false;
            ShowSignInMsg(SignUpStatus.Success);

            onRegistrationSuccess?.Invoke();
        }

        private void OnAddOrUpdateContactEmailFailure(PlayFabError _) {
            Console.LogError("AddOrUpdateContactEmailFailure!");
        }

        private void OnRegistrationFailure(PlayFabError error) {
            Console.LogError("User Registration Failed (" + error.GenerateErrorReport() + ")!");

            signUpEllipsesControl.enabled = false;
            switch(error.Error) {
                case PlayFabErrorCode.UsernameNotAvailable:
                    ShowSignInMsg(SignUpStatus.UsernameNotAvailable);
                    break;
                case PlayFabErrorCode.EmailAddressNotAvailable:
                    ShowSignInMsg(SignUpStatus.EmailNotAvailable);
                    break;
                default:
                    Console.LogError(error.Error.ToString());
                    UnityEngine.Assertions.Assert.IsTrue(false);
                    break;
            }

            onRegistrationFailure?.Invoke();
		}
	}
}