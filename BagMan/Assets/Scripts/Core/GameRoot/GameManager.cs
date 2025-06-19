using UnityEngine;

public class GameManager
{
    //хранит зависимости и освобождает ресурсы при необходимости. 1 на всю сцену
    public UI_Reference UI_Reference { get; private set; }
    public EndLevelStatsPopup EndLevelPopUp { get; private set; }
    public Handlers Handlers { get; private set; }
    public ButtonHandler BtnHandlerMenu { get; private set; }
    public EndLevelButtons BtnHandlerEndLevel { get; private set; }

    // Player contrroller & PlayerEventHandler (player's events, connected gameobjects etc) 
    public PlayerController PlayerController { get; private set; }
    public PlayerEventHandler PlayerEventHandler { get; private set; }

    public UI_StatsTracker UI_StatsTracker { get; private set; }
    public EventBus EventBus { get; private set; }
    public Gameplay GamePlay { get; private set; }
    public SoundEventHandler SoundHandler { get; private set; }


    private bool _isCreated = false;


    //Player stats
    public CharacterStats PlayerStats { get; private set; }

    public bool IsInit => _isCreated;

    public GameManager(UI_Reference ui, Handlers btnHandlers, UI_StatsTracker uI_Stats,
        PlayerController playerController, EventBus eventBus, Gameplay gameplay,
        PlayerEventHandler pEventHandler, EndLevelStatsPopup endLevelPopUp, SoundEventHandler soundEventHandler)
    {
        UI_Reference = ui;
        Handlers = btnHandlers;
        BtnHandlerMenu = btnHandlers.GetButtonHandler();
        BtnHandlerEndLevel = btnHandlers.GetEndLevelHandler();
        PlayerController = playerController;
        EventBus = eventBus;
        GamePlay = gameplay;
        UI_StatsTracker = uI_Stats;
        PlayerEventHandler = pEventHandler;
        EndLevelPopUp = endLevelPopUp;
        SoundHandler = soundEventHandler;
    }

    public void Init()
    {
        if (EventBus == null)
        {
            EventBus = new EventBus();
            _isCreated = true;
            Debug.LogWarning($"{this}: EventBus was null. A New instance was created");
        }

        if (UI_Reference == null || BtnHandlerMenu == null || PlayerController == null || GamePlay == null
            || UI_StatsTracker == null || BtnHandlerEndLevel == null)
        {
            Debug.LogError($"{this}: null reference");
            return;
        }

        PlayerStats ??= new CharacterStats(100, 200, 0.5f);
    }

    public CharacterStats GetPlayerStats()
    {
        if (PlayerStats == null)
        {
            Debug.LogError($"{this}: Player Stats is null!");
            return PlayerStats;
        }
        return PlayerStats;
    }

    public void InitPlayerStats(int health, int stamina, float rate)
    {
        if (PlayerStats == null)
            PlayerStats = new CharacterStats();
        if (PlayerStats != null)
        {
            PlayerStats.ChangeMaxHealth(health);
            PlayerStats.ChangeMaxStamina(stamina);
            PlayerStats.SetStaminaRate(rate);
        }
    }

    public void ClearPlayerData(EventBus evntBus)
    {
        evntBus?.TriggerResetGameData();
    }

    public void ClearPlayerData(EventBus evntBus, float newDefaultSpeed)
    {
        evntBus?.TriggerResetGameData();
        if (PlayerStats != null)
        {
            PlayerStats.SetCharacterSpeed(newDefaultSpeed);
            evntBus?.TriggerPlayerDefaultSpeed(PlayerStats.GetCharacterSpeed());
        }
    }

    private void Clear()
    {
        EventBus.RemoveAllListeners(); //!
        if (_isCreated)
        {
            EventBus = null;
            _isCreated = false;
        }

        GamePlay.gameObject.SetActive(false);
        GamePlay = null;
        PlayerEventHandler.gameObject.SetActive(false);
        PlayerEventHandler = null;
        PlayerStats = null;
    }

}
