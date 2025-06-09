using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(Button))]
public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private Button _btnResume;
    [SerializeField] private Button _btnExit;
    [SerializeField] private Button _btnRestart;

    private EventBus _bus;
    public event Action OnExitClicked;
    public event Action OnResumeClicked;
    public event Action OnRestartCliked;

    private bool _timeStopped = false;

    public void Initialize(EventBus bus)
    {
        _bus = bus;
        if (_bus == null)
            Debug.LogError("EventBus is not init");
        _timeStopped = false;
        AddListeners();
    }

    public void OnRestartClick()
    {
        if (_btnRestart != null)
        {

            //_bus?.TriggerRestartGame();
            //save score
        }
    }

    public void OnResumeClick()
    {
        if (_btnResume != null)
        {
            OnResumeClicked?.Invoke();
            _bus?.TriggerResumeGame();
        }
    }

    public void OnExitClick()
    {
        if (_btnExit != null)
        {
            OnExitClicked?.Invoke();
            _bus?.TriggerResumeGame();
            _bus?.TriggerExitGame();
        }
    }

    private void OnTimePauseChanged(bool state)
    {
        if (state)
            _timeStopped = state;
        else
            _timeStopped = state;
    }

    private void Awake()
    {
        var obj = this.gameObject;
        if (obj)
        {
            if (obj.CompareTag("Menu"))
            {
                Button[] buttons = obj.GetComponentsInChildren<Button>(true);
                _btnResume = buttons.FirstOrDefault(b => b.name == "ButtonResume");
                _btnExit = buttons.FirstOrDefault(b => b.name == "ButtonExit");
                _btnRestart = buttons.FirstOrDefault(b => b.name == "ButtonRestart");
            }
        }
    }

    private void AddListeners()
    {
        _bus?.GameRunTime.AddListener(OnTimePauseChanged);
    }

    private void RemoveListeners()
    {
        _bus?.GameRunTime.RemoveListener(OnTimePauseChanged);
    }


    private void OnDisable()
    {
        if (_bus != null)
        {
            RemoveListeners();
        }
    }
}
