using System;
using System.Collections;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    [Header("Spawner Ч SpawnObjects class:")]
    [SerializeField] private SpawnedObjects _spawner;
    [Range(1, 50)]
    [SerializeField] private readonly float _playerSpeed = 8f;
    private Tutorial _tutorial;

    //[SerializeField] private AudioClip _collectedCart;
    [Header("PICK-UP Sound clips:")]
    [SerializeField] private AudioClip[] _collectedSoundClip;
    [SerializeField] private AudioClip _GameOverSoundClip;
    [SerializeField] private AudioClip _GameWinSoundClip;

    private GameManager _gameManager;
    private DropZoneController _dropZoneController;

    private bool _initialized = false;
    private bool _isStarted = false;
    private bool _haveListeners = false;

    public SpawnedObjects SpawnedObjects => _spawner;
    public GameManager GameManager => _gameManager;
    public bool IsInitialized => _initialized && _isStarted;

    public Timer LevelTimer;
    [Header("Level Timer:")]
    [SerializeField] private float _timeDuration = 5f;
    [Header("Pre-trigger delay of inactivity:")]
    [SerializeField] private float _delay = 10f;

    private int _goalCondition = 0;
    private int _score = 0;

    public void Run()
    {
        if (_initialized == true)
        {
            _tutorial.StartTutorial();
        }
        else
            Debug.Log("Gameplay components are not init. GamePlay not started");
    }

    public void Initialize(GameManager gm)
    {
        _gameManager = gm;
        _score = 0;
        _goalCondition = 0;
        if (gm == null || gm?.BtnHandlerMenu == null || gm?.PlayerController == null)
        {
            Debug.LogError("Gameplay: References Init error");
            return;
        }


        _dropZoneController = GetComponentInChildren<DropZoneController>();
        InitTutorial();

        if (_spawner != null && !_spawner.IsInit)
            _spawner.Initialize();

        if (_spawner.GetList() != null && gm != null)
        {
            _goalCondition = _spawner.GetList().Count;
            gm.Init();
            LevelTimer = new Timer(_timeDuration);
            PlayerInit(gm);
            AddListeners();
            _isStarted = true;
            _initialized = true;
        }
    }

    public void InitTutorial()
    {
        if (_tutorial == null)
        {
            _tutorial = GetComponent<Tutorial>();
            _tutorial.SetDropZoneController(_dropZoneController);
            _tutorial.TutorialFinished += OnTutorialFinished;
            _gameManager.EventBus?.Timer.AddListener(OnFirstPlayerScored);
        }
    }

    private void OnTutorialFinished()
    {
        StartGameplay(_delay);
        _tutorial.TutorialFinished -= OnTutorialFinished;
    }

    public void StartGameplay(float delayTime)
    {
        if (_isStarted && _initialized && _gameManager != null)
        {
            StartCoroutine(WaitPlayersInteractions(LevelTimer, delayTime));
        }
    }


    private IEnumerator WaitPlayersInteractions(Timer timer, float seconds)
    {
        //show tips for player. if no interactions start the timer
        //Trigger text ""
        yield return new WaitForSeconds(seconds);

        if (timer != null && !timer.IsRunning)
        {
            //show tip Ч encourage player to action           
            _gameManager.EventBus.TriggerTimerUI(timer);
            timer.Start();
            _gameManager.EventBus.TriggerSoundBackgroundToogle(true);

        }
        else
            Debug.Log("Timer is still running");
    }

    private void OnPlayerStatsChanged()
    {
        if (_gameManager != null)
        {
            _gameManager.EventBus.TriggerPlayerStaminaUpdateUI();
            _gameManager.EventBus.TriggerPlayerSpeedUpdateUI();
        }
    }

    private void HandleLevelTimerFinished()
    {
        _gameManager?.EventBus?.TriggerGameLevelComplete();
    }

    private void InitTimer(Timer timer)
    {
        if (timer == null)
            timer = new Timer(_timeDuration);
        else
            timer.Reset();
    }

    private void PrepareToRestart(GameManager gm)
    {
        //Debug.Log("<color=green>PrepareToRestart()</color>");
        if (_spawner != null)
        {
            int rnd = UnityEngine.Random.Range(1, 20);
            _spawner.Dispose();
            _spawner.Initialize(rnd);
            _goalCondition = _spawner.GetList().Count;
        }
        InitTimer(LevelTimer);
        AddListeners();
        PlayerInit(gm);
    }

    //»грок загнал тележку сам
    private void OnFirstPlayerScored(Timer timer)
    {

        if (_tutorial != null && _tutorial.IsFinished == false)
        {
            //Debug.Log("»грок начала раньше туториала");
            OnTutorialFinished();
            _tutorial.InterruptTutorial();
            _tutorial.enabled = false;
        }

        if (LevelTimer != null && !LevelTimer.IsRunning)
        {
            StopAllCoroutines();
            _gameManager.EventBus.Timer.RemoveListener(OnFirstPlayerScored);
            _gameManager.EventBus.TriggerSoundBackgroundToogle(true);
            LevelTimer.Start();
            //Debug.Log("Timer trigered by cart");
        }
    }

    //”ровень завершен
    private void OnLevelComplete()
    {
        // Debug.Log("OnLevelComplete()");
        if (_gameManager != null)
        {
            _gameManager.EventBus?.TriggerSoundBackgroundToogle(false);
            _gameManager.EventBus?.Timer.RemoveListener(OnFirstPlayerScored);
            RemoveListeners();
            _gameManager.EventBus.GameRestart.AddListener(OnGameRestarted);
            _gameManager.PlayerController.UpdateCurrentPlayerSpeed(0f);
        }
    }

    //”ровень перезапускают
    private void OnGameRestarted()
    {
        //  Debug.Log("OnGameRestarted()");
        if (_gameManager != null && _gameManager.EventBus != null)
        {
            _gameManager.EventBus.GameRestart.RemoveListener(OnGameRestarted);
            PrepareToRestart(_gameManager);
            _gameManager.EventBus.TriggerGameClearScore();
            _gameManager.EventBus.TriggerHideLevelStats();
            _delay = 1f;
            _gameManager.EventBus?.TriggerSoundBackgroundToogle(true);
            _gameManager.PlayerController.UpdateCurrentPlayerSpeed(_playerSpeed);
            StartGameplay(_delay);
        }
    }

    private void OnGoalAchived()
    {
        //send extra setups
        //Debug.Log("OnGoalAchived()");
        _gameManager.EventBus.TriggerGameLevelComplete();
        SoundFXManager.Instance.PlaySoundFXClip(_GameWinSoundClip, transform, 1f);
    }

    private void AddListeners()
    {
        GameManager gm = _gameManager;
        gm?.EventBus.PlayerStatsChanged.AddListener(OnPlayerStatsChanged);
        gm?.EventBus.GameCountScore.AddListener(OnGetScore);
        gm?.EventBus.GameLevelComplete.AddListener(OnLevelComplete);
        gm?.EventBus.GameLevelGoalComplete.AddListener(OnGoalAchived);
        if (LevelTimer != null)
            LevelTimer.OnTimerComplete += HandleLevelTimerFinished;
        gm?.EventBus.TutorialFinished.AddListener(OnTutorialFinished);
        _haveListeners = true;
    }

    private void RemoveListeners()
    {
        if (_haveListeners && _gameManager != null)
        {
            _gameManager.EventBus?.PlayerStatsChanged.RemoveListener(OnPlayerStatsChanged);
            _gameManager.EventBus?.GameCountScore.RemoveListener(OnGetScore);
            _gameManager.EventBus?.GameLevelComplete.RemoveListener(OnLevelComplete);
            _gameManager.EventBus?.GameLevelGoalComplete.RemoveListener(OnGoalAchived);
            _gameManager.EventBus?.GameRestart.RemoveListener(OnGameRestarted);
            _gameManager.EventBus?.TutorialFinished.RemoveListener(OnTutorialFinished);
            if (LevelTimer != null)
                LevelTimer.OnTimerComplete -= HandleLevelTimerFinished;
            _haveListeners = false;
        }
    }

    private void PlayerInit(GameManager gm)
    {
        gm.ClearPlayerData(gm.EventBus, _playerSpeed);
        gm.InitPlayerStats(100, 200, 0.5f);
    }

    private void OnDisable()
    {
        RemoveListeners();
        if (_spawner != null)
        {
            _spawner.Dispose();
        }
    }

    private void OnValidate()
    {
        if (_spawner == null)
            _spawner = transform.gameObject.GetComponent<SpawnedObjects>();
    }

    private void FixedUpdate()
    {
        if (LevelTimer != null && LevelTimer.IsRunning && LevelTimer.GetRemainingTime() > 0)
            LevelTimer.Update();
    }

    private void OnGetScore(ulong score)
    {
        //update score
        //SoundFXManager.Instance.PlaySoundFXClip(_collectedCart, transform, 1f);
        SoundFXManager.Instance.PlayRandomSoundFXClip(_collectedSoundClip, transform, 1f);


        if (_dropZoneController == null)
            _dropZoneController = GetComponentInChildren<DropZoneController>();
        if (_dropZoneController != null)
            score = _dropZoneController.Count;
        _score = int.Parse(score.ToString());
        if (_goalCondition > 0 && _score >= _goalCondition)
        {
            _gameManager?.EventBus?.TriggerGameLevelGoalComplete();
            _goalCondition = 0;
            _score = 0;
        }
    }
}

