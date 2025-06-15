using UnityEngine;

public class PauseManager
{
    private bool _paused = false;
    private EventBus _eventBus;


    public PauseManager(EventBus evntBus)
    {
        _eventBus = evntBus;
        AddListeners();
    }

    public void RemoveListeners()
    {
        _eventBus.UIRunTime.RemoveListener(TogglePause);
        _eventBus.UIPause.RemoveListener(PauseGame);
        _eventBus.UIResume.RemoveListener(ResumeGame);
    }

    private void AddListeners()
    {
        _eventBus.UIRunTime.AddListener(TogglePause);
        _eventBus.UIPause.AddListener(PauseGame);
        _eventBus.UIResume.AddListener(ResumeGame);
    }

    /// <summary>
    /// Останавливает или запускает всю физику, не показывает UI
    /// (true - stop | false - start)
    /// </summary>
    /// <param name="status"></param>
    public void TogglePause(bool status)
    {
        _paused = status;
        Time.timeScale = _paused ? 0f : 1f;
    }

    public void TogglePauseWithUI(bool status)
    {
        if (status)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public bool isGamePaused()
    {
        return _paused;
    }

    /// <summary>
    /// Запускает "время", Скрывает UI с паузой
    /// </summary>
    public void ResumeGame()
    {
        _paused = false;
        Time.timeScale = 1f;
        _eventBus.TriggerUIShowPause(_paused);
    }

    /// <summary>
    /// Останавливает "время", Выводит UI с паузой
    /// </summary>
    public void PauseGame()
    {
        _paused = true;
        Time.timeScale = 0f;
        _eventBus.TriggerUIShowPause(_paused);
    }



}
