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

    private long _count = 0;
    public long Count => _count;
    private CartBehaviorOptions _behavior;
    public bool IsRespawning => _behavior.IsRespawning;


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
        // Debug.Log("OK");
        AddListeners();
    }


    public void AddListeners()
    {
        _gm?.EventBus?.StartTask.AddListener(ShowMessage);
    }

    public void RemoveListeners()
    {
        _gm?.EventBus?.StartTask.RemoveListener(ShowMessage);
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

    void ShowMessage()
    {
        Debug.Log(" first time");

        StartCoroutine(WaitParameters());
    }


    IEnumerator WaitParameters()
    {
        while (_gm == null || _gm.EventBus == null || _gm.GamePlay == null
            || _gm.GamePlay.LevelTimer == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        _gm.EventBus?.TriggerTimer(_gm.GamePlay?.LevelTimer);
        Debug.Log("Success");
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
