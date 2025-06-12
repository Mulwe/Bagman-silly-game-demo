using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


//������ ����� �����
//��������� ����� ��������� ����� ���� ��� �� �� ����
//Loading screen ��� �������� UIRootView �� ����������� � �������� � ��������
//����� ����� ��������� ����� �� ����� ������ �����


public class GameEntryPoint
{
    private static GameEntryPoint _instance;
    private Coroutines _coroutines;
    private UIRootView _uiRoot;
    private EventBus _bus;



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AutoStartGame()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _instance = new GameEntryPoint();
        _instance.RunGame();
    }

    private GameEntryPoint()
    {
        //��������� ��������� ����� �������� �� GameEntryPoint
        _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
        Object.DontDestroyOnLoad(_coroutines.gameObject);

        //�������� �������� ������� ����� �������
        //����� ������ Ui root. �� ����� Resourses �� ������� 
        var prefabUIRoot = Resources.Load<UIRootView>("Prefabs/UI/UIRoot");
        _uiRoot = Object.FindFirstObjectByType<UIRootView>();
        _uiRoot = Object.Instantiate(prefabUIRoot);
        Object.DontDestroyOnLoad(_uiRoot.gameObject);

        //��������� ������� �� ������������ � �.�
    }

    public void RunGame()
    {
        //��������� ������ ����� �� ���������
#if UNITY_EDITOR
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == Scene.GAMEPLAY)
        {
            _coroutines.StartCoroutine(LoadAndStartGameplay());
            return;
        }
        if (sceneName != Scene.BOOT)
        {
            return;
        }
#endif
        // ������ ����� ����. � ������ ������ ����� ��������
        _coroutines.StartCoroutine(LoadAndStartGameplay());
    }

    //Entry point
    private void SceneEntryPoint()
    {
        var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();

        //(true) ��� ������ ������� ������� ������� 
        var uiReference = _uiRoot.GetComponentInChildren<UI_Reference>(true);
        var uiHUD = _uiRoot.GetComponentInChildren<UI_StatsTracker>(true);

        Handlers handlers = new Handlers(_uiRoot);

        handlers.GetButtonHandler();
        handlers.GetEndLevelHandler();
        //������ ��� � DontDestroyOnLoad ������� ������� ������� ������� ����� ����� Object
        var playerController = Object.FindFirstObjectByType<PlayerController>();
        var gameplay = Object.FindFirstObjectByType<Gameplay>();
        var playerEventHandler = Object.FindFirstObjectByType<PlayerEventHandler>();

        _bus = new EventBus();

        // �������� �����������. Null reference ���� �� ��������� �����������.
        sceneEntryPoint.Inject(uiReference, handlers, uiHUD,
            playerController, playerEventHandler, gameplay, _bus);



        _uiRoot.HideLoadingScreen();
        //_uiRoot.ShowTipsScreen();
        //_uiRoot.FadingTipsScreen(1);
        _uiRoot.ShowPlayerHud();
        _uiRoot.ShowLevelCompletedScreen();
        sceneEntryPoint.Run();
    }

    private IEnumerator LoadAndStartGameplay()
    {
        _uiRoot.ShowLoadingScreen();
        //��������� ������ ����� -> ��������� �����. ��� ����� ���������
        yield return LoadScene(Scene.BOOT);
        yield return LoadScene(Scene.GAMEPLAY);
        //����� �������� ������ ����� ��������� ���� ��� ���� ������. ����� ������� ����� ��������
        yield return new WaitForSeconds(1f);

        SceneEntryPoint();
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }



}
