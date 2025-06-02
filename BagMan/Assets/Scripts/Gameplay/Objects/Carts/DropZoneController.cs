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


    private void Start()
    {
        DropZoneCollider = GetComponent<BoxCollider2D>();
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


    public void AddListeners()
    {
        _gm?.EventBus?.StartTask.AddListener(OnCartCountedInScore);
    }

    public void RemoveListeners()
    {
        _gm?.EventBus?.StartTask.RemoveListener(OnCartCountedInScore);
    }

    private void TryTeleportCart(Collider2D collision)
    {
        ImprovedCartAttachment obj = collision.gameObject.GetComponent<ImprovedCartAttachment>();
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
                if (_gm != null && _gm.EventBus != null)
                {
                    _gm.EventBus.TriggerPlayerCountUpdateUI(_count);
                    if (_count == 1)
                        _gm.EventBus.TriggerStartTask();
                }
            }
        }
    }

    void OnCartCountedInScore()
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
        if (collision != null)
        {
            if (collision.CompareTag("cart"))
            {
                TryTeleportCart(collision);
            }
        }
        else
            Debug.Log("Collision is null");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Collider2D col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
            Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
}
