using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vuforia;

namespace Assets.Scripts
{
    class InputManagerVuforiaImage : InputManagerBaseClass
    {
        #region consts

        const int STARTING_WIDTH = 640;
        const int STARTING_HEIGHT = 480;

        #endregion

        #region  #Variables

        public ManoMotionFrame currentManoMotionFrame;

        #endregion

        private void Awake()
        {
            ForceApplicationPermissions();
        }

        private void Start()
        {
            VuforiaARController.Instance.RegisterVuforiaStartedCallback(StartAfterVuforia);
        }

        private void StartAfterVuforia()
        {
            // Vuforia has started, now register camera image format  
            if (CameraDevice.Instance.SetFrameFormat(PIXEL_FORMAT.RGBA8888, true))
            {
                Debug.Log("Successfully registered pixel format " + PIXEL_FORMAT.RGBA8888.ToString());
            }
            else
            {
                Debug.LogError(
                  "Failed to register pixel format " + PIXEL_FORMAT.RGBA8888.ToString() +
                  "\n the format may be unsupported by your device;" +
                  "\n consider using a different pixel format.");
            }

            InitializeInputParameters();
            InitializeManoMotionFrame();
        }

        /// <summary>
        /// Initializes the parameters of the Device Camera.
        /// </summary>
        protected override void InitializeInputParameters()
        {
            ManoUtils.OnOrientationChanged += HandleOrientationChanged;

            if (OnAddonSet != null)
            {
                OnAddonSet(AddOn.DEFAULT);
            }
        }

        void HandleOrientationChanged()
        {
            currentManoMotionFrame.orientation = ManoUtils.Instance.currentOrientation;
        }

        /// <summary>
        /// Initializes the ManoMotion Frame and lets the subscribers of the event know of its information.
        /// </summary>
        private void InitializeManoMotionFrame()
        {
            currentManoMotionFrame = new ManoMotionFrame();
            ResizeManoMotionFrameResolution(STARTING_WIDTH, STARTING_HEIGHT);

            if (OnFrameInitialized != null)
            {
                OnFrameInitialized(currentManoMotionFrame);
                Debug.Log("Initialized input parameters");
            }
            else
            {
                Debug.LogWarning("None is subscribing to OnFrameInitialized");
            }
        }

        /// <summary>
        /// Gets the camera frame pixel colors.
        /// </summary>
        protected void GetCameraFrameInformation()
        {
            if (VuforiaCameraImageAccess.ActualImage == null)
            {
                Debug.LogError("No device camera available");
                return;
            }

            if (VuforiaCameraImageAccess.GetPixels32().Length < 300)
            {
                Debug.LogWarning("The frame from the camera is too small. Pixel array length:  " + VuforiaCameraImageAccess.GetPixels32().Length);
                return;
            }

            if (currentManoMotionFrame.pixels.Length != VuforiaCameraImageAccess.GetPixels32().Length)
            {
                ResizeManoMotionFrameResolution(VuforiaCameraImageAccess.ActualImage.Width, VuforiaCameraImageAccess.ActualImage.Height);
                return;
            }

            currentManoMotionFrame.pixels = VuforiaCameraImageAccess.GetPixels32();
            currentManoMotionFrame.texture.SetPixels32(VuforiaCameraImageAccess.GetPixels32());
            currentManoMotionFrame.texture.Apply();

            OnFrameUpdated?.Invoke(currentManoMotionFrame);
        }

        /// <summary>
        /// Sets the resolution of the currentManoMotion frame that is passed to the subscribers that want to make use of the input camera feed.
        /// </summary>
        /// <param name="newWidth">Requires a width value.</param>
        /// <param name="newHeight">Requires a height value.</param>
        protected void ResizeManoMotionFrameResolution(int newWidth, int newHeight)
        {
            currentManoMotionFrame.width = newWidth;
            currentManoMotionFrame.height = newHeight;
            currentManoMotionFrame.pixels = new Color32[newWidth * newHeight];
            currentManoMotionFrame.texture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, true);
            currentManoMotionFrame.texture.Apply();

            if (OnFrameResized != null)
            {
                OnFrameResized(currentManoMotionFrame);
            }
        }


        void Update()
        //void OnPostRender()
        {
            GetCameraFrameInformation();
        }

        private void OnDisable()
        {
        }

        #region Application on Background

        bool isPaused = false;

        void OnApplicationFocus(bool hasFocus)
        {
            isPaused = !hasFocus;
            if (isPaused)
            {
                ManomotionManager.Instance.StopProcessing();
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            isPaused = pauseStatus;
            if (isPaused)
            {
                ManomotionManager.Instance.StopProcessing();
            }
        }

        #endregion
    }
}
