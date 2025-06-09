using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIRootView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _tipsScreen;
    [SerializeField] private GameObject _levelCompleteScreen;


    [Header("Main HUD:")]
    [SerializeField] private GameObject _playerHud;


    private float delay = 2f;
    // [SerializeField] private Gameobject _UIMenu;

    private void Awake()
    {
        HideLoadingScreen();
        HideTipsScreen();
        HideLevelCompletedScreen();
        ShowLevelCompletedScreen();
    }


    public void ShowLoadingScreen()
    {
        ToogleObjectActive(_loadingScreen, true);
    }

    public void HideLoadingScreen()
    {
        ToogleObjectActive(_loadingScreen, false);
    }

    public void ShowTipsScreen()
    {
        ToogleObjectActive(_tipsScreen, true);
    }

    public void HideTipsScreen()
    {
        ToogleObjectActive(_tipsScreen, false);
    }

    public void ShowLevelCompletedScreen()
    {
        ToogleObjectActive(_levelCompleteScreen, true);
    }

    public void HideLevelCompletedScreen()
    {
        ToogleObjectActive(_levelCompleteScreen, false);
    }

    public void ShowPlayerHud()
    {
        ToogleObjectActive(_playerHud, true);
    }

    public void HidePlayerHud()
    {
        ToogleObjectActive(_playerHud, false);
    }

    public void FadingTipsScreen(int fadeTimeSeconds)
    {
        var image = _tipsScreen.GetComponent<Image>();
        if (image != null)
        {
            StartCoroutine(FadeOutCoroutine(image, fadeTimeSeconds));
        }
    }

    private void ToogleObjectActive(GameObject obj, bool status)
    {

        obj.SetActive(status);
    }

    private IEnumerator FadeOutCoroutine(Image image, float fadeTime)
    {
        if (image == null)
            yield break;
        Color originalColor = image.color;
        float t = 0f;
        yield return new WaitForSeconds(this.delay);

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float progress = t / fadeTime;
            float brightness = Mathf.Lerp(1f, 0f, progress);
            image.color = new Color
                (originalColor.r * brightness, originalColor.g * brightness, originalColor.b * brightness,
                originalColor.a * brightness);
            yield return null;
        }
        ShowTipsScreen();
    }

    private Image GetImage()
    {
        if (_tipsScreen != null)
        {
            var Image = _tipsScreen.GetComponent<Image>();
            if (Image != null)
                return Image;
        }
        return null;
    }
}
