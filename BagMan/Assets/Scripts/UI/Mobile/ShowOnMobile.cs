using UnityEngine;

#pragma warning disable
using System.Runtime.InteropServices;



//Mobile detection
public class ShowOnMobile : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern int isMobile();  

#endif

    private CanvasGroup _canvasGroup;

    public void SetVisible(bool isVisible)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = isVisible ? 1f : 0f;
            _canvasGroup.interactable = isVisible;
            _canvasGroup.blocksRaycasts = isVisible;
        }
    }


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SetVisible(false);

#if UNITY_WEBGL
        Debug.Log("UNITY_WEBGL defined");
#else
    Debug.Log("UNITY_WEBGL NOT defined");
#endif

#if UNITY_WEBGL && !UNITY_EDITOR     
         
       bool isJSMobile = false;

        try
        {
            isJSMobile = isMobile() == 1;
        }
        catch (System.EntryPointNotFoundException)
        {
            Debug.LogWarning("isMobile() not found in WebGL build. Make sure .jslib is correct.");
        }

        if (isJSMobile || Application.isMobilePlatform || SystemInfo.deviceType == DeviceType.Handheld)
        {
           SetVisible(true);
        }
        else
            SetVisible(false);
     
#endif


    }

}

