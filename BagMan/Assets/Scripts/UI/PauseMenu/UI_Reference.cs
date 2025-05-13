using UnityEngine;

public class UI_Reference : MonoBehaviour
{
    [Header("GameObject of UIMenuPause")]
    [SerializeField] private GameObject _reference;

    private UI_MenuPause _menuPause;
    private EventBus _eventBus;

    public void Initialize(EventBus eventBus, UI_Reference ui)
    {
        //Debug.Log("UI_Reference initialized");
        _eventBus = eventBus;
        _menuPause = new UI_MenuPause(ui);
        _eventBus.UI_Menu.AddListener(OnPauseMenuToggle);
    }

    private void OnPauseMenuToggle(bool show)
    {
        // Показываем или скрываем меню паузы
        if (_menuPause != null)
        {
            _menuPause.ShowUI(show);
        }
        else
            Debug.LogError("Menu ref is null");
    }

    private void OnDestroy()
    {
        // Отписываемся при уничтожении
        _eventBus.UI_Menu.RemoveListener(OnPauseMenuToggle);
    }

    private void OnValidate()
    {
        if (_reference == null)
        {
            _reference = gameObject;
            // Debug.Log($"{nameof(_reference)} was automatically set.");
        }
        if (!_reference.tag.Equals("Menu"))
            Debug.LogError("UIMenuPause reference:  Wrong tag!\nName of Component:" + _reference.name);
    }

    private void Awake()
    {
        if (_reference == null)
        {
            _reference = gameObject;
        }

    }
}
