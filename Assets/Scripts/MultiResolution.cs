using UnityEngine;

public class MultiResolution : MonoBehaviour
{
    private const float CAMERA_Z_POSITION = -10.0f;

    private static readonly float m_MatchHeight = 0.0f;
    public static float ScaleFactor { get; private set; }

    [SerializeField]
    private GameObject m_World = null;
    [SerializeField]
    private Camera m_Camera = null;

    public void Awake()
    {
        if (DeviceTypeChecker.GetDeviceType() == ENUM_Device_Type.Tablet)
        {
            CalculateScaleFactor();
            SetupCamera();
            SetupWorld();
        }
        else
            ScaleFactor = 1;
    }

    void SetupWorld()
    {
        m_World.transform.localScale = Vector3.one * ScaleFactor;
    }

    //This Funtion does 1 thing.
    //1. Camera's View Port should exactly match the Screen Size.
    //2. Coinside World's (0,0) co-ordinate with bottom-left corner of the Camera View Port.
    void SetupCamera()
    {
        float l_CameraYPos = Screen.height / (2f * 100);
        float l_AspectRation = Screen.width / (float)Screen.height;

        m_Camera.orthographicSize = l_CameraYPos;
    }

    //Calculate Factor to Scale the world So that it can fit in Screen
    public static void CalculateScaleFactor()
    {
        float l_HeightComponent = m_MatchHeight * (Screen.height / 1920);
        float l_WidthComponent = (1.0f - m_MatchHeight) * (Screen.width / 1080);

        ScaleFactor = l_HeightComponent + l_WidthComponent;
    }

    public static float GetScaleFactorWithHeightReference()
    {
        float l_HeightScaleFactor = Screen.height / 1920;
        return l_HeightScaleFactor;
    }

    public static float GetScaleFactorWithWidthReference()
    {
        float l_WidthScaleFactor = Screen.width / 1080;
        return l_WidthScaleFactor;
    }
}