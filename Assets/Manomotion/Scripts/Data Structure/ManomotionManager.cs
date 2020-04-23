
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

[AddComponentMenu("ManoMotion/ManoMotion Manager")]
[RequireComponent(typeof(ManoEvents))]
public class ManomotionManager : ManomotionBase
{
    #region Events

    public static Action OnManoMotionFrameProcessed;
    public static Action OnManoMotionLicenseInitialized;

    #endregion

    #region Singleton

    protected static ManomotionManager instance;

    #endregion

    #region Variables

    protected HandInfoUnity[] hand_infos;
    protected VisualizationInfo visualization_info;
    protected Session manomotion_session;

    protected int _width;
    protected int _height;
    protected int _fps;
    protected int _processing_time;

    private float fpsCooldown = 0;
    private int frameCount = 0;
    private List<int> processing_time_list = new List<int>();

    protected Color32[] framePixelColors;
    protected ManoLicense _manoLicense;

    protected ManoSettings _manoSettings;
    protected ImageFormat currentImageFormat;

    private bool _initialized;

    #endregion

    #region imports

#if UNITY_IOS
	const string library = "__Internal";
#elif UNITY_ANDROID
    const string library = "manomotion";
#else
    const string library = "manomotion";
#endif

    [DllImport(library)]
    private static extern void processFrame(ref HandInfo hand_info0, ref Session manomotion_session);

    [DllImport(library)]
    private static extern void setFrameArray(Color32[] frame);

    [DllImport(library)]
    private static extern void setResolution(int width, int height);

    [DllImport(library)]
    private static extern void stop();

    [DllImport(library)]
    private static extern ManoLicense init(ManoSettings settings);

    #endregion

    #region Init Wrapper Methods

    public void StopProcessing()
    {
#if !UNITY_EDITOR
		stop();
#endif
    }

    protected void SetResolution(int width, int height)
    {
        Debug.Log("Set resolution " + width + "," + height);
#if !UNITY_EDITOR

        setResolution(width, height);
#endif
    }

    protected void SetFrameArray(Color32[] pixels)
    {
        Debug.Log("Called setFrameArray with " + pixels.Length);

#if !UNITY_EDITOR
        setFrameArray(pixels);

#endif
    }

    #endregion

    #region Propperties

    internal int Processing_time
    {
        get
        {
            return _processing_time;
        }
    }

    internal int Fps
    {
        get
        {
            return _fps;
        }
    }

    internal int Height
    {
        get
        {
            return _height;
        }
    }

    internal int Width
    {
        get
        {
            return _width;
        }
    }

    internal VisualizationInfo Visualization_info
    {
        get
        {
            return visualization_info;
        }
    }

    internal HandInfoUnity[] Hand_infos
    {
        get
        {
            return hand_infos;
        }
    }

    public Session Manomotion_Session
    {
        get
        {
            return manomotion_session;
        }
        set
        {
            manomotion_session = value;
        }
    }

    public static ManomotionManager Instance
    {
        get
        {
            return instance;
        }
    }

    public string LicenseKey
    {
        get
        {
            return _licenseKey;
        }
        set
        {
            _licenseKey = value;
        }
    }

    public ManoLicense ManoLicense
    {
        get
        {
            return _manoLicense;
        }
        set
        {
            _manoLicense = value;
        }
    }

    public ManoSettings ManoSettings
    {
        get
        {
            return _manoSettings;
        }
        set
        {
            _manoSettings = value;
        }
    }

    #endregion

    #region Methods

    protected virtual void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Awake method of the ManoMotionManager");
            instance = this;
            ManoUtils.OnOrientationChanged += HandleOrientationChanged;
            InputManagerBaseClass.OnAddonSet += HandleAddOnSet;
            InputManagerBaseClass.OnFrameInitialized += HandleManoMotionFrameInitialized;
            InputManagerBaseClass.OnFrameUpdated += HandleNewFrame;
            InputManagerBaseClass.OnFrameResized += HandleManoMotionFrameResized;
        }
        else
        {
            this.gameObject.SetActive(false);
            Debug.LogWarning("More than 1 Manomotionmanager in scene");
        }
    }

    /// <summary>
    /// Respond to the event of a ManoMotionFrame resized.
    /// </summary>
    /// <param name="newFrame"></param>
    void HandleManoMotionFrameResized(ManoMotionFrame newFrame)
    {
        Debug.Log("Handled ManoMotionFrameResized Event");
        SetResolutionValues(newFrame.width, newFrame.height);
    }

    /// <summary>
    /// Respond to the event of a ManoMotionFrame being initialized.
    /// </summary>
    /// <param name="newFrame"></param>
    void HandleManoMotionFrameInitialized(ManoMotionFrame newFrame)
    {
        Debug.Log("Handle ManoMotion Frame Initialized" + newFrame.width);
        SetResolutionValues(newFrame.width, newFrame.height);
        InstantiateVisualisationInfo();
    }

    /// <summary>
    /// Respond to the event of a ManoMotionFrame being sent for processing.
    /// </summary>
    /// <param name="newFrame"></param>
    void HandleNewFrame(ManoMotionFrame newFrame)
    {
        GetCameraFramePixelColors(newFrame);
        UpdateTexturesWithNewInfo();
    }

    protected void Start()
    {
        SetManoMotionSettings(ImageFormat.BGRA_FORMAT, _licenseKey);
        InstantiateSession();
        InstantiateHandInfos();
        InitiateLibrary();
        SetUnityConditions();
    }

    /// <summary>
    /// Fills in the information needed in the manosettings Struct in order to initialize ManoMotion Tech.
    /// </summary>
    /// <param name="newPlatform">Requires the platform the app is going to be used in.</param>
    /// <param name="newImageFormat">Requires the image format that ManoMotion tech is going to process</param>
    /// <param name="newLicenseKey">Requires a Serial Key that is valid for ManoMotion tech and it linked with the current boundle ID used in the application.</param>
    public void SetManoMotionSettings(ImageFormat newImageFormat, string newLicenseKey)
    {
#if UNITY_IOS
		_manoSettings.platform = Platform.UNITY_IOS;
#endif

#if UNITY_ANDROID
        _manoSettings.platform = Platform.UNITY_ANDROID;
#endif
        _manoSettings.image_format = newImageFormat;
        _manoSettings.serial_key = newLicenseKey;
    }

    /// <summary>
    /// Picks the resolution.
    /// </summary>
    /// <param name="width">Requires a width value.</param>
    /// <param name="height">Requires a height value.</param>
    protected override void SetResolutionValues(int width, int height)
    {
        _width = width;
        _height = height;
        SetResolution(width, height);
        visualization_info.rgb_image = new Texture2D(_width, _height);
        framePixelColors = new Color32[_width * _height];
        SetFrameArray(framePixelColors);
    }

    /// <summary>
    /// Instantiates the manager info.
    /// </summary>
    protected override void InstantiateSession()
    {
        manomotion_session = new Session();
        manomotion_session.orientation = ManoUtils.Instance.currentOrientation;
        manomotion_session.smoothing_controller = 0.15f;
        manomotion_session.gesture_smoothing_controller = 0.5f;
        manomotion_session.enabled_features.pinch_poi = 1;
    }

    /// <summary>
    /// Gets the correct manomotion_session addon from the inputManager
    /// </summary>
    /// <param name="addon">The addon used with the inputManager</param>
    private void HandleAddOnSet(AddOn addon)
    {
        manomotion_session.add_on = addon;
    }

    /// <summary>
    /// Initializes the values for the hand information.
    /// </summary>
    private void InstantiateHandInfos()
    {
        hand_infos = new HandInfoUnity[1];
        for (int i = 0; i < hand_infos.Length; i++)
        {
            hand_infos[i].hand_info = new HandInfo();
            hand_infos[i].hand_info.gesture_info = new GestureInfo();
            hand_infos[i].hand_info.gesture_info.mano_class = ManoClass.NO_HAND;
            hand_infos[i].hand_info.gesture_info.hand_side = HandSide.None;
            hand_infos[i].hand_info.tracking_info = new TrackingInfo();
            hand_infos[i].hand_info.tracking_info.bounding_box = new BoundingBox();
            hand_infos[i].hand_info.tracking_info.bounding_box.top_left = new Vector3();
        }
    }

    /// <summary>
    /// Instantiates the visualisation info.
    /// </summary>
    private void InstantiateVisualisationInfo()
    {
        visualization_info = new VisualizationInfo();
        Debug.Log("Initializing visualization info with " + _width + " " + _height);
        visualization_info.rgb_image = new Texture2D(_width, _height);
    }

    /// <summary>
    /// Initiates the library.
    /// </summary>
    protected void InitiateLibrary()
    {
        _manoLicense = new ManoLicense();
        string originalKey = _licenseKey;
        int maxSerialKeyCharacters = 23;
        List<string> allCharacters = new List<string>();

        //Check and remove spaces here
        if (LicenseKey.Length > maxSerialKeyCharacters)
        {
            string removeExtraCharactersAndSpaceString = _licenseKey.Substring(0, maxSerialKeyCharacters);
            LicenseKey = removeExtraCharactersAndSpaceString;
        }

        Init(LicenseKey);
    }

    /// <summary>
    /// Sets the Application to not go to sleep mode as well as the requested framerate.
    /// </summary>
    protected override void SetUnityConditions()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /// <summary>
    /// Lets the core know that it needs to calculate the Point of Interaction
    /// </summary>
    /// <param name="condition"></param>
    public void ShouldCalculatePOI(bool condition)
    {
        if (condition)
        {
            manomotion_session.enabled_features.pinch_poi = 1;
        }
        else
        {
            manomotion_session.enabled_features.pinch_poi = 0;
        }
    }

    /// <summary>
    /// Sets the mano motion smoothing value throught the gizmo slider, the bigger the value the stronger the smoothing.
    /// </summary>
    /// <param name="slider">Slider.</param>
    public void SetManoMotionSmoothingValue(Slider slider)
    {
        manomotion_session.smoothing_controller = slider.value;
    }

    /// <summary>
    /// Sets the mano motion Gesture smoothing value throught the gizmo slider, the bigger the value the stronger the smoothing.
    /// </summary>
    /// <param name="slider">Slider.</param>
    public void SetManoMotionGestureSmoothingValue(Slider slider)
    {
        manomotion_session.gesture_smoothing_controller = slider.value;
    }

    #endregion

    #region update_methods

    protected void Update()
    {
        if (_initialized)
        {
            CalculateFPSAndProcessingTime();
        }
    }

    /// <summary>
    /// Updates the orientation information as captured from the device to the Session
    /// </summary>
    protected void HandleOrientationChanged()
    {
        manomotion_session.orientation = ManoUtils.Instance.currentOrientation;
    }

    /// <summary>
    /// Updates the RGB Frame of Visualization Info with the pixels captured from the camera.
    /// </summary>
    protected override void UpdateTexturesWithNewInfo()
    {
        if (framePixelColors.Length > 255)
        {
            if (visualization_info.rgb_image.width * visualization_info.rgb_image.height == framePixelColors.Length)
            {
                visualization_info.rgb_image.SetPixels32(framePixelColors);
                visualization_info.rgb_image.Apply();
                ProcessManomotion();
                if (OnManoMotionFrameProcessed != null)
                {
                    OnManoMotionFrameProcessed();
                }
            }
            else
            {
                Debug.LogErrorFormat("UpdateTexturesWithNewInfo error rgb_image width {0} height{1} framepixelcolors length {2}", visualization_info.rgb_image.width, visualization_info.rgb_image.height, framePixelColors.Length);
            }
        }
        else
        {
            Debug.LogError("Frame Pixel colors error");
        }
    }

    /// <summary>
    /// Gets the camera frame pixel colors.
    /// </summary>
    protected void GetCameraFramePixelColors(ManoMotionFrame newFrame)
    {
        if (framePixelColors.Length != newFrame.pixels.Length || visualization_info.rgb_image.width != newFrame.texture.width || visualization_info.rgb_image.height != newFrame.texture.height)
        {
            Debug.LogError("GetCameraFramePixelColors lengths were not matching, called resize");
            Handheld.Vibrate();
            SetResolutionValues(newFrame.width, newFrame.height);
            SetFrameArray(framePixelColors);
        }
        try
        {
            framePixelColors = newFrame.pixels;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// Evaluates the dimension of the pixel color array and if it matches the dimensions proceeds with Processing the Frame. 
    /// </summary>
    protected override void ProcessManomotion()
    {
        if (framePixelColors.Length == Width * Height)
        {
            try
            {
                long start = System.DateTime.UtcNow.Millisecond + System.DateTime.UtcNow.Second * 1000 + System.DateTime.UtcNow.Minute * 60000;
                ProcessFrame();
                long end = System.DateTime.UtcNow.Millisecond + System.DateTime.UtcNow.Second * 1000 + System.DateTime.UtcNow.Minute * 60000;
                if (start < end)
                    processing_time_list.Add((int)(end - start));
            }
            catch (System.Exception ex)
            {
                Debug.Log("exeption: " + ex.ToString());
            }
        }
        else
        {
            //If the frame is to small 16x16
            Debug.Log("camera size doesent match: " + framePixelColors.Length + " != " + Width * Height);
        }
    }

    /// <summary>
    /// Calculates the Frames Per Second in the application and retrieves the estimated Processing time.
    /// </summary>
    protected override void CalculateFPSAndProcessingTime()
    {
        fpsCooldown += Time.deltaTime;
        frameCount++;
        if (fpsCooldown >= 1)
        {
            _fps = frameCount;
            frameCount = 0;
            fpsCooldown -= 1;
            CalculateProcessingTime();
        }
    }

    /// <summary>
    /// Calculates the elapses time needed for processing the frame.
    /// </summary>
    protected void CalculateProcessingTime()
    {
        if (processing_time_list.Count > 0)
        {
            int sum = 0;
            for (int i = 0; i < processing_time_list.Count; i++)
            {
                sum += processing_time_list[i];
            }
            sum /= processing_time_list.Count;
            _processing_time = sum;
            processing_time_list.Clear();
        }
    }

    #endregion

    #region update_wrappers

    /// <summary>
    /// Wrapper method that calls the ManoMotion core tech to process the frame in order to perform hand tracking and gesture analysis
    /// </summary>
    protected void ProcessFrame()
    {
#if !UNITY_EDITOR || UNITY_STANDALONE
 processFrame(ref hand_infos[0].hand_info, ref manomotion_session);
#endif

    }

    #endregion

    protected override void Init(string serial_key)
    {

#if !UNITY_EDITOR || UNITY_STANDALONE
        _manoLicense = init(_manoSettings);
        _initialized = true;

        if (OnManoMotionLicenseInitialized != null)
        {
            OnManoMotionLicenseInitialized();
        }
#endif
    }
}