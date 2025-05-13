using UnityEngine;

// Gameplay Scene ссылки на объекты и подготовка сцены
public class Gameplay : MonoBehaviour
{
    [Header("PlayerController:")]
    [SerializeField] private PlayerController _control;
    [Header("Spawner Ч SpawnObjects class:")]
    [SerializeField] private SpawnedObjects _spawner;

    private ButtonHandler _buttonHandler;
    private CharacterStats _playerStats;


    private bool _initialized = false;

    public void Run()
    {
        if (_initialized == true)
        {
            // запуск только после инициализации
            StartGameplay();
        }
        else
            Debug.Log("Gameplay components are not init. GamePlay not started");
    }

    public void Initialize(ButtonHandler buttonHandler, PlayerController playerController, CharacterStats PlayerStats)
    {
        _buttonHandler = buttonHandler;
        _control = playerController;
        if (_buttonHandler == null || _control == null)
        {
            Debug.LogError("Gameplay: References Init error");
        }
        else
        {
            // send something that you need not gm
            this._playerStats = PlayerStats;
            InitSpawner();
        }
    }


    private void InitSpawner()
    {
        _spawner.Initialize();
        if (_spawner.GetList() != null)
        {
            _initialized = true;
        }
        else
            Debug.Log("_spawner returned null list");
    }


    private void StartGameplay()
    {

        Debug.Log($"Check Player stats:" +
            $"\n Stamina: {_playerStats.Stamina}" +
            $"\n Health: {_playerStats.Health}");
        //Update Character Stats
        //_playerStats.TryUseStamina();

        //show the task

        //start timers

        //conditions to win 
        _playerStats.TakeDamage(10);

    }

    private void OnValidate()
    {
        if (_spawner == null)
            _spawner = transform.gameObject.GetComponent<SpawnedObjects>();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }






}

