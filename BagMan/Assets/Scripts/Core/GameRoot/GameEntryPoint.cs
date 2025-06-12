using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


//патерн точка входа
//запускает сцены проверяет сцена игры или не из игры
//Loading screen как заглушка UIRootView не выгружается и работает в корутине
//точка входа запускает сцену со своей точкой входа


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
        //позволяет запускать любые корутины из GameEntryPoint
        _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
        Object.DontDestroyOnLoad(_coroutines.gameObject);

        //комплекс объектов поэтому нужно создать
        //найти префаб Ui root. от папки Resourses до префаба 
        var prefabUIRoot = Resources.Load<UIRootView>("Prefabs/UI/UIRoot");
        _uiRoot = Object.FindFirstObjectByType<UIRootView>();
        _uiRoot = Object.Instantiate(prefabUIRoot);
        Object.DontDestroyOnLoad(_uiRoot.gameObject);

        //настройки графики от пользователя и т.д
    }

    public void RunGame()
    {
        //вычленяем нужную сцену из редактора
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
        // запуск самой игры. с нужной сценой через корутины
        _coroutines.StartCoroutine(LoadAndStartGameplay());
    }

    //Entry point
    private void SceneEntryPoint()
    {
        var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();

        //(true) для поиска включая скрытые объекты 
        var uiReference = _uiRoot.GetComponentInChildren<UI_Reference>(true);
        var uiHUD = _uiRoot.GetComponentInChildren<UI_StatsTracker>(true);

        Handlers handlers = new Handlers(_uiRoot);

        handlers.GetButtonHandler();
        handlers.GetEndLevelHandler();
        //данный код в DontDestroyOnLoad поэтому достаем объекты главной сцены через Object
        var playerController = Object.FindFirstObjectByType<PlayerController>();
        var gameplay = Object.FindFirstObjectByType<Gameplay>();
        var playerEventHandler = Object.FindFirstObjectByType<PlayerEventHandler>();

        _bus = new EventBus();

        // Внедряем зависимости. Null reference если не прокинуть зависимости.
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
        //загрузили пустую сцену -> загружаем новую. Это более безопасно
        yield return LoadScene(Scene.BOOT);
        yield return LoadScene(Scene.GAMEPLAY);
        //чтобы создался объект нужно подождать кадр или пару секунд. Чтобы увидеть экран загрузки
        yield return new WaitForSeconds(1f);

        SceneEntryPoint();
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }



}
