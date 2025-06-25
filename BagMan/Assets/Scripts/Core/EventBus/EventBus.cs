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
    public UnityEvent<bool> UIRunTime { get; } = new();
    public UnityEvent UIPause { get; } = new();
    public UnityEvent UIResume { get; } = new();
    public UnityEvent StartTask { get; } = new UnityEvent();

    //UI
    public UnityEvent PlayerStaminaUpdateUI { get; } = new();
    public UnityEvent PlayerSpeedUpdateUI { get; } = new();
    public UnityEvent<ulong> PlayerCountUpdateUI = new();
    public UnityEvent TemperatureChangedUI { get; } = new();

    public UnityEvent PlayerStatsChanged { get; } = new();
    public UnityEvent<float> PlayerDefaultSpeed { get; } = new();

    public UnityEvent ResetPlayerData { get; } = new();

    public UnityEvent<Timer> Timer { get; } = new();
    public UnityEvent TimerReceived { get; } = new();

    public UnityEvent GameLevelComplete { get; } = new();
    public UnityEvent GameLevelGoalComplete { get; } = new();
    public UnityEvent<ulong> GameCountScore { get; } = new();
    public UnityEvent GameClearScore { get; } = new();


    public UnityEvent GameExit { get; } = new UnityEvent();
    public UnityEvent GameRestart { get; } = new();
    public UnityEvent HideLevelStats { get; } = new();
    public UnityEvent ShowLevelStats { get; } = new();




    public UnityEvent<bool> Sound { get; } = new();
    public UnityEvent<bool> SoundBackground { get; } = new();
    public UnityEvent<bool> SoundFx { get; } = new();

    //Highlight
    public UnityEvent<bool, float> OutlineDropzone { get; } = new();
    public UnityEvent TutorialFinished { get; } = new();

    public void TriggerOutlineDropzone(bool state, float duration) => OutlineDropzone.Invoke(state, duration);
    public void TriggerTutorialFinished() => TutorialFinished.Invoke();

    /// <summary> 
    /// True = On, False = Off
    /// </summary>
    /// <param name="status"></param>
    public void TriggerSoundToogle(bool status) => Sound.Invoke(status);

    /// <summary> 
    /// True = On, False = Off
    /// </summary>
    /// <param name="status"></param>
    public void TriggerSoundBackgroundToogle(bool status) => SoundBackground.Invoke(status);

    /// <summary> 
    /// True = On, False = Off
    /// </summary>
    /// <param name="status"></param>
    public void TriggerSoundFxToogle(bool status) => SoundFx.Invoke(status);


    public void TriggerPlayerDefaultSpeed(float newSpeed)
    {
        PlayerDefaultSpeed.Invoke(newSpeed);
    }

    public void TriggerHideLevelStats()
        => HideLevelStats.Invoke();

    public void TriggerShowLevelStats()
        => ShowLevelStats.Invoke();



    public void TriggerRestartGame()
        => GameRestart.Invoke();

    public void TriggerGameLevelComplete()
        => GameLevelComplete.Invoke();

    public void TriggerGameLevelGoalComplete()
        => GameLevelGoalComplete.Invoke();

    public void TriggerGameSetScore(ulong score)
        => GameCountScore.Invoke(score);

    public void TriggerGameClearScore()
        => GameClearScore.Invoke();

    public void TriggerPlayerStatsChanged()
        => PlayerStatsChanged.Invoke();

    public void TriggerResetGameData()
        => ResetPlayerData.Invoke();


    //Timer triggers
    public void TriggerTimer(Timer timer)
        => Timer.Invoke(timer);

    public void TriggerTimerReceived()
        => TimerReceived.Invoke();

    //UI EndLevel

    //UI Updates
    public void TriggerTimerUI(Timer timer)
        => Timer.Invoke(timer);

    public void TriggerPlayerStaminaUpdateUI()
        => PlayerStaminaUpdateUI.Invoke();

    public void TriggerPlayerSpeedUpdateUI()
        => PlayerSpeedUpdateUI.Invoke();

    public void TriggerPlayerCountUpdateUI(ulong count)
        => PlayerCountUpdateUI.Invoke(count);

    public void TriggerTemperatureChangedUI()
        => TemperatureChangedUI.Invoke();


    //Cart on spawnzone - trigger timer
    public void TriggerStartTask()
        => StartTask.Invoke();

    //System Game events
    public void TriggerExitGame()
        => GameExit.Invoke();

    public void TriggerResumeGame()
        => UIResume.Invoke();

    public void TriggerPauseGame()
        => UIPause.Invoke();
    // change flag when the time stops
    public void TriggerUIRunTime(bool status)
        => UIRunTime.Invoke(status);

    //System UI
    public void TriggerUIShowPause(bool status)
        => UI_Menu.Invoke(status);

    public void TriggerGameOver()
        => UI_GameOver.Invoke();

    public void RemoveAllListeners()
    {

        UI_Menu.RemoveAllListeners();
        UI_GameOver.RemoveAllListeners();
        UIRunTime.RemoveAllListeners();
        UIPause.RemoveAllListeners();
        UIResume.RemoveAllListeners();
        StartTask.RemoveAllListeners();
        //UI
        PlayerStaminaUpdateUI.RemoveAllListeners();
        PlayerSpeedUpdateUI.RemoveAllListeners();
        PlayerCountUpdateUI.RemoveAllListeners();
        TemperatureChangedUI.RemoveAllListeners();
        PlayerStatsChanged.RemoveAllListeners();
        PlayerDefaultSpeed.RemoveAllListeners();
        ResetPlayerData.RemoveAllListeners();
        Timer.RemoveAllListeners();
        TimerReceived.RemoveAllListeners();
        GameLevelComplete.RemoveAllListeners();
        GameLevelGoalComplete.RemoveAllListeners();
        GameCountScore.RemoveAllListeners();
        GameClearScore.RemoveAllListeners();
        GameExit.RemoveAllListeners();
        GameRestart.RemoveAllListeners();
        HideLevelStats.RemoveAllListeners();
        ShowLevelStats.RemoveAllListeners();
        Sound.RemoveAllListeners();
        SoundBackground.RemoveAllListeners();
        SoundFx.RemoveAllListeners();
        //Highlight
        OutlineDropzone.RemoveAllListeners();
        TutorialFinished.RemoveAllListeners();
    }
}
