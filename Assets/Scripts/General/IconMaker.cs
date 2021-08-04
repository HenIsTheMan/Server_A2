using System.IO;
using UnityEngine;

namespace Server.General {
    [ExecuteInEditMode]
    internal sealed class IconMaker: MonoBehaviour {
        #region Fields

        [SerializeField]
        private bool create;

        [SerializeField]
        private RenderTexture renderTex;

        [SerializeField]
        private Camera bakeCam;

        [SerializeField]
        private string imgName;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal IconMaker(): base() {
            create = false;

            renderTex = null;

            bakeCam = null;

            imgName = string.Empty;
        }

        static IconMaker() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Update() {
            if(create) {
                CreateIcon();
                create = false;
            }
        }

        #endregion

        private void CreateIcon() {
            string filePath = Application.streamingAssetsPath + "/Icons/";

            if(!Directory.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
            }

            filePath += imgName;

            bakeCam.targetTexture = renderTex;
            RenderTexture currRenderTex = RenderTexture.active;
            bakeCam.targetTexture.Release();
            RenderTexture.active = bakeCam.targetTexture;
            bakeCam.Render();

            Texture2D imgTex = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
            imgTex.ReadPixels(new Rect(0.0f, 0.0f, bakeCam.targetTexture.width, bakeCam.targetTexture.height), 0, 0);
            imgTex.Apply();

            RenderTexture.active = currRenderTex;
            byte[] bytesPNG = imgTex.EncodeToPNG();
            File.WriteAllBytes(filePath + ".png", bytesPNG);
        }
    }
}