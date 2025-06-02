using TMPro;
using UnityEngine;

public class EndLevelStatsPopup : MonoBehaviour
{
    private EventBus _eventbus;
    private TextMeshProUGUI _data;
    private bool _isInit = false;

    public bool IsInit => _isInit;


    public void Initialize(EventBus eventBus)
    {
        if (eventBus != null)
        {
            _eventbus = eventBus;
            AddListeners(_eventbus);

            _isInit = true;
        }
    }

    private void AddListeners(EventBus eventBus)
    {
        eventBus.GameCountScore.AddListener(OnScoreCount);
        eventBus.GameLevelComplete.AddListener(OnLevelCompleted);
    }

    private void RemoveListeners(EventBus eventBus)
    {
        eventBus.GameCountScore.RemoveListener(OnScoreCount);
        eventBus.GameLevelComplete.RemoveListener(OnLevelCompleted);
    }

    private void HidePopup()
    {
        this.gameObject.SetActive(false);
    }

    private void ShowPopup()
    {
        this.gameObject.SetActive(true);
    }

    private void OnScoreCount(ulong score)
    {
        if (_data != null)
        {
            _data.text = score.ToString();
        }
    }

    private void OnLevelCompleted()
    {
        ShowPopup();
        Debug.Log($"Count is {_data.text}");
    }

    private void Awake()
    {
        Transform child = transform.Find("Data");
        if (child != null)
        {
            child.TryGetComponent<TextMeshProUGUI>(out _data);
        }
        HidePopup();
    }

    private void OnDisable()
    {
        if (_eventbus != null)
            RemoveListeners(_eventbus);
    }
}
