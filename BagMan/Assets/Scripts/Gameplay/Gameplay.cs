using System;
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
    public bool IsInitialized => _initialized && _isStarted;

    public Timer LevelTimer;

    /// <summary>
    /// Level timer duration
    /// </summary>
    private float _timeDuration = 30f;

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
        if (gm == null)
        {
            Debug.Log($"{this}: gm is null");
        }
        if (gm?.BtnHandler == null || gm?.PlayerController == null)
            Debug.LogError("Gameplay: References Init error");
        else
            GameAndSpawnInitialization(gm);
    }

    public void StartGameplay()
    {
        if (_isStarted && _initialized && _gameManager != null)
        {
            // StartCoroutine(DelayTimer(LevelTimer, 3));
            _gameManager.EventBus.Timer.AddListener(OnFirstPlayerScored);
            StartCoroutine(WaitPlayersInteractions(LevelTimer, 10f));
        }
    }

    IEnumerator WaitPlayersInteractions(Timer timer, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (!timer.IsRunning)
        {
            //show tip
            _gameManager.EventBus.Timer.RemoveListener(OnFirstPlayerScored);
            timer.Start();
        }
    }

    public void OnPlayerStatsChanged()
    {
        if (_gameManager != null)
        {
            if (_dropZoneController != null)
                _gameManager.EventBus.TriggerPlayerCountUpdateUI(_dropZoneController.Count);
            _gameManager.EventBus.TriggerPlayerStaminaUpdateUI();
            _gameManager.EventBus.TriggerPlayerSpeedUpdateUI();
        }
    }


    private void HandleLevelTimerFinished()
    {
        _dropZoneController = GetComponentInChildren<DropZoneController>();
        if (_dropZoneController != null)
            Debug.Log($"Time out! Your Score is [{_dropZoneController.Count}]");
        if (LevelTimer != null)
            LevelTimer.Stop();
    }

    //Listener OnPlayerStatsChanged
    private void GameAndSpawnInitialization(GameManager gm)
    {
        _spawner.Initialize();
        if (_spawner.GetList() != null && gm != null)
        {
            gm.Init();
            ChangeDefaultPlayerSpeed(_playerSpeed);

            LevelTimer = new Timer(_timeDuration);

            AddListeners();

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

    private void AddListeners()
    {
        GameManager gm = _gameManager;
        gm?.EventBus.PlayerStatsChanged.AddListener(OnPlayerStatsChanged);
        gm?.EventBus.GameCountScore.AddListener(OnGetScore);

        LevelTimer.OnTimerComplete += HandleLevelTimerFinished;
        _haveListeners = true;
    }

    private void RemoveListeners()
    {
        if (_haveListeners && _gameManager != null)
        {
            _gameManager.EventBus.PlayerStatsChanged.RemoveListener(OnPlayerStatsChanged);
            LevelTimer.OnTimerComplete -= HandleLevelTimerFinished;
        }
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void OnValidate()
    {
        if (_spawner == null)
            _spawner = transform.gameObject.GetComponent<SpawnedObjects>();
    }

    private void Update()
    {
        if (LevelTimer != null && LevelTimer.GetRemainingTime() > 0)
        {
            LevelTimer.Update();
        }
    }

    private void OnGetScore(ulong score)
    {
        //update score
        score = _dropZoneController.Count;
        _gameManager?.EventBus?.TriggerGameCountScore(score);
    }

    private void OnFirstPlayerScored(Timer timer)
    {
        if (LevelTimer != null && !LevelTimer.IsRunning)
        {
            _gameManager.EventBus.Timer.RemoveListener(OnFirstPlayerScored);
            LevelTimer.Start();
            _gameManager?.EventBus?.TriggerTimer(LevelTimer);
            Debug.Log("Timer trigered by cart");
        }
    }


}

