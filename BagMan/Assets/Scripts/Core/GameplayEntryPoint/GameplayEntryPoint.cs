using UnityEngine;


//����� ����� �����
public class GameplayEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneRootBinder _sceneRootBinder;

    private GameManager _gm;

    private UI_Reference _uiReference;
    private ButtonHandler _buttonHandler;
    private PlayerController _playerController;
    private UI_StatsTracker _UI_StatsTracker;
    private Gameplay _gameplay;
    private EventBus _bus;
    private PlayerEventHandler _playerEventHandler;



    public void Inject(UI_Reference uiReference, ButtonHandler buttonHandler, UI_StatsTracker ui_StatsTracker,
                      PlayerController playerController, PlayerEventHandler playerEventHandler,
                      Gameplay gameplay, EventBus bus)
    {
        this._bus = bus;
        this._gameplay = gameplay;
        this._uiReference = uiReference;
        this._buttonHandler = buttonHandler;
        this._playerController = playerController;
        this._UI_StatsTracker = ui_StatsTracker;
        this._playerEventHandler = playerEventHandler;
        _gm = new GameManager(_uiReference, _buttonHandler, _UI_StatsTracker,
            _playerController, _bus, _gameplay, _playerEventHandler);
    }

    public void Run()
    {
        _gm.EventBus.GameExit.AddListener(OnCloseApplication);
        //Debug.Log("Gameplay Scene loaded: Initializing components");
        // ����������� ����������� � ����������
        RegisterComponents();
        // ��������� ������������
        InjectDependencies();
        // ������������� ����������� � ���������� �������
        InitializeComponents();
        //������ UI,��������� ������, ���������� ������, ����� ��������
        RunComponents();
        //RunGamplay .������ ���� ������� � ������� ��������

        //����� etc..

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
        var component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    private void Awake()
    {
        // ������������ �� ����� ���� ������ ��� ���������������. ���� �������� � �� �� ���� �����
        _sceneRootBinder = GetIfNotInit<SceneRootBinder>();
    }

    private void RegisterComponents()
    {
        // ����������� �������� ����������� �����
        _sceneRootBinder.Register<UI_Reference>(_gm.UI_Reference);
        _sceneRootBinder.Register<ButtonHandler>(_gm.BtnHandler);
        _sceneRootBinder.Register<PlayerController>(_gm.PlayerController);
        _sceneRootBinder.Register<UI_StatsTracker>(_gm.UI_StatsTracker);
        _sceneRootBinder.Register<PlayerEventHandler>(_gm.PlayerEventHandler);
        _sceneRootBinder.Register<Gameplay>(_gm.GamePlay);

        // ����� ����� ���������������� ������� � ������ ����������
        _sceneRootBinder.Register<GameplayEntryPoint>(this);
    }

    private void InjectDependencies()
    {
        // ��������� ������������ � ����������
        _sceneRootBinder.InjectDependencies(_gm.UI_Reference);
        _sceneRootBinder.InjectDependencies(_gm.BtnHandler);
        _sceneRootBinder.InjectDependencies(_gm.PlayerController);
        _sceneRootBinder.InjectDependencies(_gm.UI_StatsTracker);
        _sceneRootBinder.InjectDependencies(_gm.PlayerEventHandler);
        _sceneRootBinder.InjectDependencies(_gm.GamePlay);
    }

    private void InitializeComponents()
    {
        // ������������� ����������� � ���������� �������
        _uiReference.Initialize(_bus, _uiReference);
        _buttonHandler.Initialize(_bus);
        _playerController.Initialize(_bus);
        _UI_StatsTracker.Initialize(_gm);
        _gameplay.Initialize(_gm);
        _playerEventHandler.Initialize(_gm);
    }

    private void RunComponents()
    {
        // ������ ����������� � ���������� �������
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

