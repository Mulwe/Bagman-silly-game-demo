using UnityEngine;

public class GameManager
{
    //хранит зависимости и освобождает ресурсы при необходимости. 1 на всю сцену
    private UI_Reference _uiReference;
    private ButtonHandler _buttonHandler;
    private PlayerController _playerController;
    private UI_StatsTracker _UI_StatsTracker;
    private EventBus _eventBus;
    private Gameplay _gameplay;
    private PlayerEventHandler _playerEventHandler;

    private bool _isCreated = false;

    public EventBus EventBus => _eventBus;
    public Gameplay GamePlay => _gameplay;
    public PlayerController PlayerController => _playerController;
    public ButtonHandler BtnHandler => _buttonHandler;
    public UI_Reference UI_Reference => _uiReference;
    public UI_StatsTracker UI_StatsTracker => _UI_StatsTracker;
    public PlayerEventHandler PlayerEventHandler => _playerEventHandler;

    //Player stats
    private CharacterStats _player;
    public CharacterStats PlayerStats => _player;




    public GameManager(UI_Reference ui, ButtonHandler btnHandler, UI_StatsTracker uI_Stats,
        PlayerController playerController, EventBus eventBus, Gameplay gameplay,
        PlayerEventHandler pEventHandler)
    {
        _uiReference = ui;
        _buttonHandler = btnHandler;
        _playerController = playerController;
        _eventBus = eventBus;
        _gameplay = gameplay;
        _UI_StatsTracker = uI_Stats;
        _playerEventHandler = pEventHandler;
        _player = new CharacterStats();
    }



    public void Init()
    {
        if (_eventBus == null)
        {
            _eventBus = new EventBus();
            _isCreated = true;
            Debug.LogWarning($"{this}: EventBus was null. A New instance was created");
        }

        if (_uiReference == null || _buttonHandler == null || _playerController == null || _gameplay == null
            || _UI_StatsTracker == null)
        {
            Debug.LogError($"{this}: null reference");
            return;
        }
        InitPlayerStats(null);
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

    private void InitPlayerStats(CharacterStats PlayerStats)
    {
        _player = new CharacterStats();
        //_player = new CharacterStats(100, 200, 0.5f);
        if (PlayerStats != null)
        {
            _player = PlayerStats;
        }
        Debug.Log("Player stats are initialized");
    }

    private void Clear()
    {
        _uiReference = null;
        _buttonHandler = null;
        _playerController = null;
        _eventBus.RemoveAllListeners();
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
