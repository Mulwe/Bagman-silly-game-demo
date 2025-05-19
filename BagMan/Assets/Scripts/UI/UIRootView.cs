using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIRootView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _tipsScreen;

    private float delay = 2f;
    // [SerializeField] private Gameobject _UIMenu;

    private void Awake()
    {
        HideLoadingScreen();
        HideTipsScreen();
    }


    public void ShowLoadingScreen()
    {
        _loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        _loadingScreen.SetActive(false);
    }

    public Image GetImage()
    {
        if (_tipsScreen != null)
        {
            var Image = _tipsScreen.GetComponent<Image>();
            if (Image != null)
                return Image;
        }
        return null;
    }
    public void ShowTipsScreen()
    {
        _tipsScreen.SetActive(true);
    }

    public void HideTipsScreen()
    {
        _tipsScreen.SetActive(false);
    }

    public void FadingTipsScreen(int fadeTimeSeconds)
    {
        var image = _tipsScreen.GetComponent<Image>();
        if (image != null)
        {
            StartCoroutine(FadeOutCoroutine(image, fadeTimeSeconds));
        }
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
        HideTipsScreen();
    }
}
