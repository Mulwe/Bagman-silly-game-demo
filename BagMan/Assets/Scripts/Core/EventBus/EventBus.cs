using UnityEngine.Events;
//using UnityEngine.Events; UnityEvent много минусов 

// не обязан быть monobehaivior
// EventBus pattern. The components more isolated and do not know about realisations
// evenbus реализует методы подписка, отписка, метод вызова события
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
    public UnityEvent StartTask { get; } = new UnityEvent();

    //UI
    public UnityEvent PlayerStaminaUpdateUI { get; } = new();
    public UnityEvent PlayerSpeedUpdateUI { get; } = new();
    public UnityEvent<float> PlayerCountUpdateUI = new();
    public UnityEvent TemperatureChangedUI { get; } = new();

    public UnityEvent PlayerStatsChanged { get; } = new();

    public UnityEvent<Timer> Timer { get; } = new();
    public UnityEvent TimerReceived { get; } = new();
    public UnityEvent AddPoint { get; } = new();

    public UnityEvent GameLevelComplete { get; } = new();
    public UnityEvent<ulong> GameCountScore { get; } = new();


    public void TriggerGameLevelComplete() => GameLevelComplete.Invoke();
    public void TriggerGameCountScore(ulong score) => GameCountScore.Invoke(score);

    //Timer triggers
    public void TriggerTimer(Timer timer) => Timer.Invoke(timer);
    public void TriggerTimerReceived() => TimerReceived.Invoke();

    public void TriggerPlayerStatsChanged() => PlayerStatsChanged.Invoke();
    //UI Updates
    public void TriggerPlayerStaminaUpdateUI() => PlayerStaminaUpdateUI.Invoke();
    public void TriggerPlayerSpeedUpdateUI() => PlayerSpeedUpdateUI.Invoke();
    public void TriggerPlayerCountUpdateUI(float count) => PlayerCountUpdateUI.Invoke(count);
    public void TriggerTemperatureChangedUI() => TemperatureChangedUI.Invoke();

    public void TriggerStartTask() => StartTask.Invoke();
    //System Game events
    public void TriggerExitGame() => GameExit.Invoke();
    public void TriggerPauseGame() => GamePause.Invoke();
    public void TriggerResumeGame() => GameResume.Invoke();
    // change flag when the time stops
    public void TriggerGameRunTime(bool status) => GameRunTime.Invoke(status);
    //System UI
    public void TriggerUIShowPause(bool status) => UI_Menu.Invoke(status);
    public void TriggerGameOver() => UI_GameOver.Invoke();

    public void RemoveAllListeners()
    {
        GameLevelComplete.RemoveAllListeners();
        GameCountScore.RemoveAllListeners();

        TimerReceived.RemoveAllListeners();
        AddPoint.RemoveAllListeners();
        Timer.RemoveAllListeners();
        StartTask.RemoveAllListeners();
        PlayerStatsChanged.RemoveAllListeners();
        PlayerStaminaUpdateUI.RemoveAllListeners();
        PlayerSpeedUpdateUI.RemoveAllListeners();
        PlayerCountUpdateUI.RemoveAllListeners();
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
