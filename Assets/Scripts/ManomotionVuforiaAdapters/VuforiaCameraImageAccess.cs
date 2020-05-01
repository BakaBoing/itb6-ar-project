using UnityEngine;
using System.Collections;
using Vuforia;

public class VuforiaCameraImageAccess : MonoBehaviour
{
    public static Image ActualImage { get; private set; }

    #region PRIVATE_MEMBERS
    private PIXEL_FORMAT _pixelFormat = PIXEL_FORMAT.UNKNOWN_FORMAT;
    private bool _accessCameraImage = true;
    private bool _formatRegistered = false;
    private static Texture2D _tempTexture;
    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
#if UNITY_EDITOR
        _pixelFormat = PIXEL_FORMAT.GRAYSCALE; // Need Grayscale for Editor
#else
        _pixelFormat = PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
#endif

        _tempTexture = new Texture2D(1, 1);

        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
    }
    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS
    private void OnVuforiaStarted()
    {
        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(_pixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + _pixelFormat.ToString());
            _formatRegistered = true;
        }
        else
        {
            Debug.LogError(
                "\nFailed to register pixel format: " + _pixelFormat.ToString() +
                "\nThe format may be unsupported by your device." +
                "\nConsider using a different pixel format.\n");
            _formatRegistered = false;
        }
    }
    /// 
    /// Called each time the Vuforia state is updated
    /// 
    void OnTrackablesUpdated()
    {
        if (_formatRegistered)
        {
            if (_accessCameraImage)
            {
                ActualImage = CameraDevice.Instance.GetCameraImage(_pixelFormat);
            }
        }
    }
    /// 
    /// Called when app is paused / resumed
    /// 
    void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }
    /// 
    /// Register the camera pixel format
    /// 
    void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(_pixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + _pixelFormat.ToString());
            _formatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + _pixelFormat.ToString());
            _formatRegistered = false;
        }
    }
    /// 
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// 
    void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + _pixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(_pixelFormat, false);
        _formatRegistered = false;
    }
    #endregion //PRIVATE_METHODS

    public static Color32[] GetPixels32()
    {
        ActualImage?.CopyBufferToTexture(_tempTexture);
        return _tempTexture.GetPixels32();
    }
}