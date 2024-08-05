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
        //Debug.Log("DeviceTypeChecker ::: " + DeviceTypeChecker.GetDeviceType());

        if (DeviceTypeChecker.GetDeviceType() == ENUM_Device_Type.Tablet)
        {
            CalculateScaleFactor();
            SetupCamera();
            SetupWorld();
        }
        else
            ScaleFactor = 1;

       //Debug.Log("ScaleFactor :: " + ScaleFactor);
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
        //float l_CameraYPos = Screen.width / (2.0f * Constant.PIXEL_PER_UNIT);
        float l_AspectRation = Screen.width / (float)Screen.height;
        //float l_CameraXpos = l_CameraYPos * l_AspectRation;

        //Debug.Log("l_CameraXpos ::: " + l_AspectRation);
        //Debug.Log("l_CameraYPos ::: " + l_CameraYPos);

        m_Camera.orthographicSize = l_CameraYPos;
    }

    //Calculate Factor to Scale the world So that it can fit in Screen
    public static void CalculateScaleFactor()
    {
        float l_HeightComponent = m_MatchHeight * (Screen.height / 1920);
        float l_WidthComponent = (1.0f - m_MatchHeight) * (Screen.width / 1080);

        //Debug.Log("l_HeightComponent ::: " + l_HeightComponent);
        //Debug.Log("l_WidthComponent ::: " + l_WidthComponent);

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