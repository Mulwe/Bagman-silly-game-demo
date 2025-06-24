using System;
using System.Collections;
using UnityEngine;

public struct CartBehaviorOptions
{
    public bool IsRespawning { get; set; }
}

public class DropZoneController : MonoBehaviour
{
    BoxCollider2D DropZoneCollider;

    private Gameplay _gameplay;
    private GameManager _gm;
    private SpawnedObjects _spawner;
    private ulong _count = 0;
    public ulong Count => _count;
    private CartBehaviorOptions _behavior;
    public bool IsRespawning => _behavior.IsRespawning;
    private Coroutine _response;

    private ShaderController _outline;
    public event Action<bool, float> ToogleOutline;
    public event Action TurnOffOutline;


    public void OnTurnOffOutline()
    {
        TurnOffOutline.Invoke();
    }

    private void Start()
    {
        DropZoneCollider = GetComponent<BoxCollider2D>();
        _outline = GetComponent<ShaderController>();
        if (DropZoneCollider != null)
            DropZoneCollider.enabled = true;
        else
            Debug.Log("DropZoneCollider is not found");

        _gameplay = transform.parent.parent.GetComponent<Gameplay>();
        _spawner = (_gameplay == null) ? null : _gameplay.GetComponent<SpawnedObjects>();
        if (_gameplay == null || _spawner == null) Debug.Log("Gameplay null or _spawner null");

        _behavior = new CartBehaviorOptions
        {
            IsRespawning = false
        };
        StartCoroutine(WaitInit());
    }

    IEnumerator WaitInit()
    {
        while (!_gameplay.IsInitialized)
        {
            yield return new WaitForSeconds(1f);
        }
        _gm = _gameplay.GameManager;
        AddListeners();
    }

    private void AddListeners()
    {
        _gm?.EventBus?.StartTask.AddListener(OnCartCountedInScore);
        _gm?.EventBus?.GameClearScore.AddListener(OnScoreCleared);
        _gm?.EventBus?.OutlineDropzone.AddListener(OnToogleOutline);
    }

    private void OnToogleOutline(bool state, float duration)
    {
        StartOutlinePulse(state, duration);
    }
    public void StartOutlinePulse(bool state, float duration)
    {
        ToogleOutline.Invoke(state, duration);
    }

    private void OnScoreCleared()
    {
        _count = 0;
        _gm.EventBus.TriggerPlayerCountUpdateUI(_count);
    }

    public void RemoveListeners()
    {
        _gm?.EventBus?.StartTask.RemoveListener(OnCartCountedInScore);
        _gm?.EventBus?.GameClearScore.RemoveListener(OnScoreCleared);
        _gm?.EventBus?.OutlineDropzone.AddListener(OnToogleOutline);
    }

    private void TryTeleportCart(Collider2D collision)
    {
        ImprovedCartAttachment obj = null;
        if (collision.gameObject.TryGetComponent<ImprovedCartAttachment>(out var comp))
            obj = comp;
        if (obj != null)
        {
            if (obj.IsAttached() != null)
                obj.DetachChain();
            HandleCountAction(obj);
        }
    }

    private void HandleCountAction(ImprovedCartAttachment cart)
    {
        if (cart.IsAttached() == null && _spawner != null)
        {
            if (_spawner.RespawnObject(cart.gameObject, _behavior.IsRespawning))
            {
                _count += 1;
                UpdateAllViewModels();
            }
        }
    }

    private float UlongToFloat(ulong ulongValue)
    {
        if (ulongValue <= float.MaxValue)
        {
            return ulongValue * 1.0f;
        }
        return float.MaxValue;
    }


    private void UpdateAllViewModels()
    {
        if (_gm != null && _gm.EventBus != null)
        {
            _gm.EventBus.TriggerGameSetScore(_count);
            _gm.EventBus.TriggerPlayerCountUpdateUI(_count);
            if (_count == 1)
            {
                _gm.EventBus.TriggerStartTask();
            }
        }
    }

    private void OnCartCountedInScore()
    {
        //Debug.Log("Timer Triggered by cart");
        StartCoroutine(WaitParametersAndTriggerTimer());
    }

    IEnumerator WaitParametersAndTriggerTimer()
    {
        while (_gm == null || _gm.EventBus == null || _gm.GamePlay == null ||
            _gm.GamePlay.LevelTimer == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        _gm.EventBus?.TriggerTimer(_gm.GamePlay?.LevelTimer);

        // ждем ответ с той стороны
        _response = StartCoroutine(CheckResponse());
        yield return _response;
    }

    IEnumerator CheckResponse()
    {
        //  waiting
        _gm.EventBus.TimerReceived.AddListener(OnTimerResponseReceived);
        float start = Time.time;
        while (Time.time - start < 5f)
        {
            _gm.EventBus?.TriggerTimer(_gm.GamePlay?.LevelTimer);
            yield return new WaitForSeconds(0.1f);
        }
        _gm.EventBus.TimerReceived.RemoveListener(OnTimerResponseReceived);
    }

    void OnTimerResponseReceived()
    {
        if (_response != null)
        {
            StopCoroutine(_response);
            _response = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Cart -> CapsuleCollider2D -> despawn / or respawn
        if (collision != null
            && collision.CompareTag("cart")
            && collision is CapsuleCollider2D caps)
        {
            TryTeleportCart(collision);
        }
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }


}
