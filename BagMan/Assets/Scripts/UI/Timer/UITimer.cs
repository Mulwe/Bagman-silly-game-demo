using System.Collections;
using TMPro;
using UnityEngine;
using Action = System.Action;

public class UITimer : MonoBehaviour
{
    [SerializeField] private UI_StatsTracker _ui_StatsTracker;
    private TextMeshProUGUI _textMeshProUGUI;

    private Coroutine _coroutine;
    private bool _isRunning = false;
    private EventBus _eventBus;
    private Timer _timer;

    private bool _trigger30 = false;
    private bool _trigger15 = false;
    private bool _trigger5 = false;

    private bool _isPulsing = false;

    void Start()
    {
        _ui_StatsTracker ??= transform.parent.GetComponentInChildren<UI_StatsTracker>();
        _textMeshProUGUI ??= this.GetComponent<TextMeshProUGUI>();
        if (_textMeshProUGUI != null)
        {
            ChangeAlpha(_textMeshProUGUI, 0.0f);
        }
    }

    private void OnEnable()
    {
        if (_eventBus == null)
        {
            StartCoroutine(WaitInit());
        }
    }

    void AddListeners()
    {
        _eventBus?.Timer.AddListener(ShowTimerUI);
    }

    void RemoveListeners()
    {
        if (_timer != null)
            _timer.OnTimerComplete -= HandleTimerCompletion;
        _eventBus?.Timer.RemoveListener(ShowTimerUI);
    }

    IEnumerator WaitInit()
    {
        while (_eventBus == null)
        {
            if (_ui_StatsTracker != null)
                _eventBus = _ui_StatsTracker.EventBus;
            yield return null;
        }
        AddListeners();
    }

    //C������ �����, ���� ����� ��������������� ��������� ����� � ������ � ������� ������
    private bool IsTimerUIActive()
    {
        if (_textMeshProUGUI != null)
        {
            float alpha = _textMeshProUGUI.color.a;
            return alpha > 0.0f && alpha <= 1.0f;
        }
        return false;
    }

    private void ShowTimerUI(Timer t)
    {
        if (t != null && IsTimerUIActive() == false)
        {
            _eventBus?.Timer.RemoveListener(ShowTimerUI);
            _eventBus?.TriggerTimerReceived();
            _timer = t;
            _timer.OnTimerComplete += HandleTimerCompletion;
            if (_textMeshProUGUI != null)
            {
                var (min, sec) = NormilizeTime(_timer.GetRemainingTime());
                _textMeshProUGUI.text = $"{min:00}:{sec:00}";
                if (!_isRunning)
                {
                    _isRunning = true;
                    _coroutine = StartCoroutine(FadeIn(_textMeshProUGUI, 1.0f));
                }
            }
        }
    }

    //Trigger ������� ����� ����� ����
    void HandleTimerCompletion()
    {
        if (_eventBus != null)
        {
            StopAllCoroutines();
            if (_textMeshProUGUI != null)
                ChangeAlpha(_textMeshProUGUI, 0.0f);
            if (_timer != null)
                _timer.OnTimerComplete -= HandleTimerCompletion;
            _eventBus.Timer.AddListener(ShowTimerUI);
        }
    }

    IEnumerator FadeIn(TextMeshProUGUI target, float seconds)
    {
        if (target != null)
        {
            float time = 0f;
            while (time < seconds)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Clamp01(time / seconds);
                ChangeAlpha(target, alpha);
                yield return null;
            }
            ChangeAlpha(target, 1.0f);
            _coroutine = null;
            _isRunning = false;
        }
    }

    // Quick solution for now due to time constraints.
    // Should be moved to an event-driven approach later.
    void Update()
    {
        if (_textMeshProUGUI != null && _timer != null && _timer.IsRunning && !_timer.IsFinished)
        {
            float remainingTime = _timer.GetRemainingTime();
            var (min, sec) = NormilizeTime(remainingTime);
            _textMeshProUGUI.text = $"{min:00}:{sec:00}";

            if (!_trigger30 && remainingTime <= 30f)
            {
                _trigger30 = true;
                RemindAboutTimer(_textMeshProUGUI, _timer, 5f, Color.yellow);
            }
            if (!_trigger15 && remainingTime <= 15f)
            {
                _trigger15 = true;
                RemindAboutTimer(_textMeshProUGUI, _timer, 5f, Color.red);
            }
            if (!_trigger5 && remainingTime <= 5f)
            {
                _trigger5 = true;
                RemindAboutTimer(_textMeshProUGUI, _timer, 5f, Color.red);
            }
        }

    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private (int minutes, int seconds) NormilizeTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        return (minutes, seconds);
    }

    private void ChangeAlpha(TextMeshProUGUI target, float alpha)
    {
        target.color = new Color(target.color.r, target.color.g, target.color.b, alpha);
    }

    void RemindAboutTimer(TextMeshProUGUI text, Timer timer, float duration, Color blinkColor)
    {
        if (!_isPulsing)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);

            float currentA = 1.0f;
            Color blink = new Color(blinkColor.r, blinkColor.g, blinkColor.b, currentA);
            Color white = new Color(1f, 1f, 1f, currentA);

            StartCoroutine(PropertyPulse(
                text, timer, duration, () =>
                {
                    text.color = Color.Equals(text.color, white) ? blink : white;

                }));
            text.color = white; //forced alpha color
        }
    }

    IEnumerator PropertyPulse(TextMeshProUGUI target, Timer timer, float duration, Action action)
    {
        if (_isPulsing) yield break;

        _isPulsing = true;
        //���� � �������������. ����� ��������� ����� �����
        Color originalColor = new Color(target.color.r, target.color.g, target.color.b, target.color.a);

        float startRemaining = timer.GetRemainingTime();

        while (timer.GetRemainingTime() > 0)
        {
            action?.Invoke();
            if (startRemaining - timer.GetRemainingTime() >= duration)
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }

        target.color = originalColor;
        _isPulsing = false;
    }

}
