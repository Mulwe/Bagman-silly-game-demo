using UnityEngine;

public class UIRootView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _levelCompleteScreen;

    [Header("Main HUD:")]
    [SerializeField] private GameObject _playerHud;

    // [SerializeField] private Gameobject _UIMenu;

    private void Awake()
    {
        HideLoadingScreen();

        HideLevelCompletedScreen();
    }

    public void ShowLoadingScreen()
    {
        ToogleObjectActive(_loadingScreen, true);
    }

    public void HideLoadingScreen()
    {
        ToogleObjectActive(_loadingScreen, false);
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

    private void ToogleObjectActive(GameObject obj, bool status)
    {
        obj.SetActive(status);
    }

}
