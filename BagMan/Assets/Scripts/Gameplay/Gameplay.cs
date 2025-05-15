using UnityEngine;

public class Gameplay : MonoBehaviour
{
    [Header("PlayerController:")]
    [SerializeField] private PlayerController _control;
    [Header("Spawner — SpawnObjects class:")]
    [SerializeField] private SpawnedObjects _spawner;

    private ButtonHandler _buttonHandler;
    private CharacterStats _playerStats;
    private GameManager _gameManager;

    private bool _initialized = false;
    private bool _isStarted = false;
    private bool _haveListeners = false;

    public void Run()
    {
        if (_initialized == true)
        {
            StartGameplay();
        }
        else
            Debug.Log("Gameplay components are not init. GamePlay not started");
    }

    public void Initialize(GameManager gm)
    {
        _buttonHandler = gm?.BtnHandler;
        _control = gm?.PlayerController;
        if (_buttonHandler == null || _control == null)
        {
            Debug.LogError("Gameplay: References Init error");
        }
        else
        {
            _gameManager = gm;
            _spawner.Initialize();
            if (_spawner.GetList() != null && _gameManager != null)
                _initialized = true;
            else
                Debug.Log($"{this}: failed in intialization!");
        }
    }


    private void StartGameplay()
    {
        //GameManager 
        _gameManager.Init();
        Debug.Log($"Check Player stats:" +
            $"\n Stamina: {_playerStats?.Stamina}" +
            $"\n Health: {_playerStats?.Health}");
        RunPlayerDependencies(_gameManager);

    }

    private void RunPlayerDependencies(GameManager gm)
    {
        gm.PlayerEventHandler.Initialize(gm);
        //Listeners
        //gm.EventBus.
        //gm.EventBus.
        //gm.EventBus.
        //gm.EventBus.
        _haveListeners = true;
        _isStarted = true;
    }

    private void OnDisable()
    {
        //remove listeners if init
        if (_haveListeners)
        {

        }
    }

    private void OnValidate()
    {
        if (_spawner == null)
            _spawner = transform.gameObject.GetComponent<SpawnedObjects>();
    }

    private void Update()
    {
        if (_isStarted == true)
        {

        }
    }

    public void OnPlayerHealthChanged()
    {
        //
        _gameManager.EventBus.TriggerPlayerHealthUpdateUI();
    }

    public void OnPlayerStaminaChanged()
    {
        //
        _gameManager.EventBus.TriggerPlayerStaminaUpdateUI();
    }

    public void OnPlayerSpeedChanged()
    {
        //
        _gameManager.EventBus.TriggerPlayerSpeedUpdateUI();
    }
}

