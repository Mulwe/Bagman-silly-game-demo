using UnityEngine;


//точка входа сцены
public class GameplayEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneRootBinder _sceneRootBinder;

    private GameManager _gm;

    private UI_Reference _uiReference;
    private EndLevelStatsPopup _endLevelPopUp;
    private Handlers _buttonHandlers;
    private PlayerController _playerController;
    private UI_StatsTracker _UI_StatsTracker;
    private Gameplay _gameplay;
    private EventBus _bus;
    private PlayerEventHandler _playerEventHandler;



    public void Inject(UI_Reference uiReference, UI_StatsTracker ui_StatsTracker,
                        EndLevelStatsPopup uiEndLevel, Handlers buttonHandlers,
                        PlayerController playerController, Gameplay gameplay,
                        PlayerEventHandler playerEventHandler, EventBus bus)
    {
        this._bus = bus;
        this._gameplay = gameplay;
        this._uiReference = uiReference;
        this._endLevelPopUp = uiEndLevel;
        this._buttonHandlers = buttonHandlers;
        this._playerController = playerController;
        this._UI_StatsTracker = ui_StatsTracker;
        this._playerEventHandler = playerEventHandler;
        _gm = new GameManager(_uiReference, buttonHandlers, _UI_StatsTracker,
            _playerController, _bus, _gameplay, _playerEventHandler, uiEndLevel);
    }

    public void Run()
    {
        _gm.EventBus.GameExit.AddListener(OnCloseApplication);
        //Debug.Log("Gameplay Scene loaded: Initializing components");
        // Регистрация компонентов в контейнере
        RegisterComponents();
        // Внедрение зависимостей
        InjectDependencies();
        // Инициализация компонентов в правильном порядке
        InitializeComponents();
        //запуск UI,включение кнопок, Управления игрока, спаун объектов
        RunComponents();
        //RunGamplay .запуск игры условия и игровые механики

        //выход etc..

    }

    void OnCloseApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private T GetIfNotInit<T>() where T : Component
    {
        TryGetComponent<T>(out var component);
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    private void Awake()
    {
        // присваивание со сцены если объект уже инициализирован. ищем локально а не по всей сцене
        _sceneRootBinder = GetIfNotInit<SceneRootBinder>();
    }

    private void RegisterComponents()
    {
        // Регистрация основных компонентов сцены
        _sceneRootBinder.Register<UI_Reference>(_gm.UI_Reference);
        _sceneRootBinder.Register<ButtonHandler>(_gm.BtnHandlerMenu);
        _sceneRootBinder.Register<EndLevelButtons>(_gm.BtnHandlerEndLevel);
        _sceneRootBinder.Register<EndLevelStatsPopup>(_gm.EndLevelPopUp);
        _sceneRootBinder.Register<PlayerController>(_gm.PlayerController);
        _sceneRootBinder.Register<UI_StatsTracker>(_gm.UI_StatsTracker);
        _sceneRootBinder.Register<PlayerEventHandler>(_gm.PlayerEventHandler);
        _sceneRootBinder.Register<Gameplay>(_gm.GamePlay);

        // Можно также зарегистрировать сервисы и другие компоненты
        _sceneRootBinder.Register<GameplayEntryPoint>(this);
    }

    private void InjectDependencies()
    {
        // Внедрение зависимостей в компоненты
        _sceneRootBinder.InjectDependencies(_gm.UI_Reference);
        _sceneRootBinder.InjectDependencies(_gm.EndLevelPopUp);
        _sceneRootBinder.InjectDependencies(_gm.BtnHandlerMenu);
        _sceneRootBinder.InjectDependencies(_gm.BtnHandlerEndLevel);
        _sceneRootBinder.InjectDependencies(_gm.PlayerController);
        _sceneRootBinder.InjectDependencies(_gm.UI_StatsTracker);
        _sceneRootBinder.InjectDependencies(_gm.PlayerEventHandler);
        _sceneRootBinder.InjectDependencies(_gm.GamePlay);
    }

    private void InitializeComponents()
    {
        // Инициализация компонентов в правильном порядке
        _uiReference.Initialize(_bus, _uiReference);
        _buttonHandlers.GetButtonHandler().Initialize(_bus);
        _buttonHandlers.GetEndLevelHandler().Initialize(_bus);
        _endLevelPopUp.Initialize(_bus);
        _playerController.Initialize(_bus);
        _UI_StatsTracker.Initialize(_gm);
        _gameplay.Initialize(_gm);
        _playerEventHandler.Initialize(_gm);
    }

    private void RunComponents()
    {
        // Запуск компонентов в правильном порядке
        //_uiroot
        _playerController?.Run();
        _gameplay?.Run();
        _playerEventHandler?.Run();

    }





    public void OnDisable()
    {
        if (_gm != null)
            _gm.EventBus.GameExit.RemoveListener(OnCloseApplication);
    }
}

