using UnityEngine.Events;
//using UnityEngine.Events; UnityEvent ����� ������� 

// �� ������ ���� monobehaivior
// EventBus pattern. The components more isolated and do not know about realisations
// evenbus ��������� ������ ��������, �������, ����� ������ �������
//
public class EventBus
{
    public UnityEvent<bool> UI_Menu { get; } = new();
    public UnityEvent UI_GameOver { get; } = new UnityEvent();
    public UnityEvent<bool> PlayerControl { get; } = new();
    public UnityEvent<bool> GameRunTime { get; } = new();
    public UnityEvent GamePause { get; } = new();
    public UnityEvent GameResume { get; } = new();
    public UnityEvent GameExit { get; } = new UnityEvent();

    //UI
    public UnityEvent PlayerStaminaUpdateUI { get; } = new();
    public UnityEvent PlayerSpeedUpdateUI { get; } = new();
    public UnityEvent PlayerHealthUpdateUI { get; } = new();
    public UnityEvent TemperatureChangedUI { get; } = new();


    public UnityEvent PlayerStatsChanged { get; } = new();

    /* ������� ������� ����� + ��������� �������������*/
    public void TriggerPlayerStatsChanged() => PlayerStatsChanged.Invoke();



    //UI Updates
    public void TriggerPlayerStaminaUpdateUI() => PlayerStaminaUpdateUI.Invoke();
    public void TriggerPlayerSpeedUpdateUI() => PlayerSpeedUpdateUI.Invoke();
    public void TriggerPlayerHealthUpdateUI() => PlayerHealthUpdateUI.Invoke();
    public void TriggerTemperatureChangedUI() => TemperatureChangedUI.Invoke();

    //System Game events
    public void TriggerExitGame() => GameExit.Invoke();
    public void TriggerPauseGame() => GamePause.Invoke();
    public void TriggerResumeGame() => GameResume.Invoke();
    // change flag when the time stops
    public void TriggerGameRunTime(bool status) => GameRunTime.Invoke(status);

    public void TriggerUIShowPause(bool status) => UI_Menu.Invoke(status);

    public void TriggerGameOver() => UI_GameOver.Invoke();

    //�� ������������
    //public void TriggerPlayerControl(bool status) => PlayerControl.Invoke(status);




    public void RemoveAllListeners()
    {

        PlayerStaminaUpdateUI.RemoveAllListeners();
        PlayerSpeedUpdateUI.RemoveAllListeners();
        PlayerHealthUpdateUI.RemoveAllListeners();

        TemperatureChangedUI.RemoveAllListeners();

        UI_Menu.RemoveAllListeners();
        UI_GameOver.RemoveAllListeners();
        PlayerControl.RemoveAllListeners();
        GameRunTime.RemoveAllListeners();
        GameExit.RemoveAllListeners();
        GamePause.RemoveAllListeners();
        GameResume.RemoveAllListeners();
    }
}
