using UnityEngine;

public class GameManager
{
    //хранит зависимости и освобождает ресурсы при необходимости. 1 на всю сцену
    private UI_Reference _uiReference;
    private ButtonHandler _btnHandlerMenu;
    private EndLevelButtons _btnHandlerEndLevel;
    private PlayerController _playerController;
    private UI_StatsTracker _UI_StatsTracker;
    private EventBus _eventBus;
    private Gameplay _gameplay;
    private PlayerEventHandler _playerEventHandler;

    private bool _isCreated = false;

    public EventBus EventBus => _eventBus;
    public Gameplay GamePlay => _gameplay;
    public PlayerController PlayerController => _playerController;
    public ButtonHandler BtnHandlerMenu => _btnHandlerMenu;
    public EndLevelButtons BtnHandlerEndLevel => _btnHandlerEndLevel;
    public UI_Reference UI_Reference => _uiReference;
    public UI_StatsTracker UI_StatsTracker => _UI_StatsTracker;
    public PlayerEventHandler PlayerEventHandler => _playerEventHandler;

    //Player stats
    private CharacterStats _player;
    public CharacterStats PlayerStats => _player;
    public bool IsInit => _isCreated;

    public GameManager(UI_Reference ui, Handlers btnHandlers, UI_StatsTracker uI_Stats,
        PlayerController playerController, EventBus eventBus, Gameplay gameplay,
        PlayerEventHandler pEventHandler)
    {
        _uiReference = ui;
        _btnHandlerMenu = btnHandlers.GetButtonHandler();
        _btnHandlerEndLevel = btnHandlers.GetEndLevelHandler();
        _playerController = playerController;
        _eventBus = eventBus;
        _gameplay = gameplay;
        _UI_StatsTracker = uI_Stats;
        _playerEventHandler = pEventHandler;
    }

    public void Init()
    {
        if (_eventBus == null)
        {
            _eventBus = new EventBus();
            _isCreated = true;
            Debug.LogWarning($"{this}: EventBus was null. A New instance was created");
        }

        if (_uiReference == null || _btnHandlerMenu == null || _playerController == null || _gameplay == null
            || _UI_StatsTracker == null || _btnHandlerEndLevel == null)
        {
            Debug.LogError($"{this}: null reference");
            return;
        }

        _player ??= new CharacterStats(100, 200, 0.5f);
    }



    public CharacterStats GetPlayerStats()
    {
        if (_player == null)
        {
            Debug.LogError($"{this}: Player Stats is null!");
            return _player;
        }
        return _player;
    }

    public void ReInitializePlayer(int health, int stamina, float rate)
    {
        _player ??= new CharacterStats(health, stamina, rate);
    }

    private void Clear()
    {
        _uiReference = null;
        _btnHandlerMenu = null;
        _btnHandlerEndLevel = null;
        _playerController = null;
        _eventBus.RemoveAllListeners(); //!
        if (_isCreated)
        {
            _eventBus = null;
            _isCreated = false;
        }
        _gameplay = null;
    }

    ~GameManager()
    {
        //отписка от ивентов
        //очистка ресурсов
        //очистка ссылок
        Clear();
    }

}
