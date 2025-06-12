public class Handlers
{

    private ButtonHandler _menu;
    private EndLevelButtons _scoreScreen;

    public Handlers(UIRootView ui)
    {
        if (ui != null)
        {
            InitMenu(ui);
            InitScoreScreen(ui);
        }
    }

    private void InitMenu(UIRootView ui)
    {
        var handlers = ui.GetComponentsInChildren<ButtonHandler>(true);
        if (handlers == null || handlers.Length == 0) return;

        foreach (var handler in handlers)
        {
            if (handler == null) continue;

            if (handler.CompareTag("Menu"))
            {
                handler.TryGetComponent<ButtonHandler>(out var menu);
                if (menu != null)
                    _menu = menu;
            }
        }
    }

    private void InitScoreScreen(UIRootView ui)
    {
        var handlers = ui.GetComponentsInChildren<EndLevelButtons>(true);
        if (handlers == null || handlers.Length == 0) return;

        foreach (var handler in handlers)
        {
            if (handler == null) continue;
            if (handler.CompareTag("UI_EndLevel"))
            {
                handler.TryGetComponent<EndLevelButtons>(out var scoreScreen);
                if (scoreScreen != null)
                    _scoreScreen = scoreScreen;
            }
        }
    }

    public ButtonHandler GetButtonHandler()
    {
        return _menu;
    }

    public EndLevelButtons GetEndLevelHandler()
    {
        return _scoreScreen;
    }


    public void Clear()
    {
        _menu = null;
        _scoreScreen = null;
    }

}
