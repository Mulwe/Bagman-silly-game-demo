using System.Collections;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    [Header("Spawner — SpawnObjects class:")]
    [SerializeField] private SpawnedObjects _spawner;
    [Range(1, 50)]
    [SerializeField] private float _playerSpeed = 8f;

    private GameManager _gameManager;
    private DropZoneController _dropZoneController;

    private bool _initialized = false;
    private bool _isStarted = false;
    private bool _haveListeners = false;
    public SpawnedObjects SpawnedObjects => _spawner;
    public GameManager GameManager => _gameManager;


    private int Points = 0;
    private bool _Timeout = false;

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
        _gameManager = gm;

        if (gm?.BtnHandler == null || gm?.PlayerController == null)
        {
            Debug.LogError("Gameplay: References Init error");
        }
        else
        {
            GameAndSpawnInitialization(gm);
        }
    }

    public void StartGameplay()
    {
        if (_isStarted && _initialized && _gameManager != null)
        {

            // событие игрок или тележка на поле

            // отстегнуть
            // переместить
            // добавить очко

        }
    }

    IEnumerator DropZoneTracker()
    {
        yield return null;
    }

    //Listener OnPlayerStatsChanged
    private void GameAndSpawnInitialization(GameManager gm)
    {
        _spawner.Initialize();
        if (_spawner.GetList() != null && _gameManager != null)
        {
            gm.Init();
            ChangeDefaultPlayerSpeed(_playerSpeed);
            gm.EventBus.PlayerStatsChanged.AddListener(OnPlayerStatsChanged);
            //gm.EventBus.PlayerStatsChanged.AddListener(OnPlayerIsOnDropZone);
            _haveListeners = true;
            _isStarted = true;
            _initialized = true;
        }
        else
            Debug.Log($"{this}: failed in intialization!");
    }

    private void ChangeDefaultPlayerSpeed(float newSpeed)
    {
        GameManager gm = _gameManager;
        if (gm != null && gm.PlayerStats != null && gm.PlayerController)
        {
            if (newSpeed > 0)
            {

                gm.PlayerStats.SetCharacterSpeed(newSpeed);
                gm.PlayerController.UpdatePlayerSpeed(newSpeed);
            }
        }
    }

    private void OnDisable()
    {
        //remove listeners if init
        if (_haveListeners)
        {
            _gameManager.EventBus.PlayerStatsChanged.RemoveListener(OnPlayerStatsChanged);
            //gm.EventBus.PlayerStatsChanged.RemoveListener(OnPlayerIsOnDropZone);
        }
    }

    private void OnValidate()
    {
        if (_spawner == null)
            _spawner = transform.gameObject.GetComponent<SpawnedObjects>();
    }

    private void Update()
    {

    }



    public void OnPlayerStatsChanged()
    {
        //very lazy way
        _gameManager.EventBus.TriggerPlayerHealthUpdateUI();
        _gameManager.EventBus.TriggerPlayerStaminaUpdateUI();
        _gameManager.EventBus.TriggerPlayerSpeedUpdateUI();
    }

}

