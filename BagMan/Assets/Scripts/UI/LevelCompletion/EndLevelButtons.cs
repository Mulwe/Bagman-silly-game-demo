using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelButtons : MonoBehaviour
{
    [SerializeField] private Button _btnExit;
    [SerializeField] private Button _btnRestart;

    private EventBus _bus;
    public event Action OnExitClicked;
    public event Action OnRestartCliked;


    public void Initialize(EventBus bus)
    {
        Debug.Log($"{this.gameObject.name}: Init");
        _bus = bus;
        if (_bus == null)
            Debug.LogError("EventBus is not init");

        AddListeners();
    }

    public void OnRestartClick()
    {
        if (_btnRestart != null)
        {
            OnRestart Clicked?.Invoke();
            //_bus?.TriggerRestartGame();
            //save score
        }
    }



    public void OnExitClick()
    {
        if (_btnExit != null)
        {
            Debug.Log("OnExitClick");
            OnExitClicked?.Invoke();
            _bus?.TriggerExitGame();
        }
    }

    private void Awake()
    {
        var obj = this.gameObject;
        if (obj)
        {
            if (obj.layer == LayerMask.NameToLayer("UI"))
            {
                Button[] buttons = obj.GetComponentsInChildren<Button>(true);
                if (buttons != null && buttons.Length > 0)
                {
                    _btnExit = buttons.FirstOrDefault(b => b.name == "ButtonExit");
                    _btnRestart = buttons.FirstOrDefault(b => b.name == "ButtonRestart");
                }
            }
        }
    }

    private void AddListeners()
    {
        // _bus?.GameRunTime.AddListener(OnTimePauseChanged);
    }

    private void RemoveListeners()
    {
        //_bus?.GameRunTime.RemoveListener(OnTimePauseChanged);
    }

    private void OnDisable()
    {
        if (_bus != null)
        {
            RemoveListeners();
        }
    }
}
