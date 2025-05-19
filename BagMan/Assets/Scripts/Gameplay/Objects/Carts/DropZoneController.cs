using UnityEngine;

public class DropZoneController : MonoBehaviour
{
    BoxCollider2D DropZoneCollider;


    private Gameplay _gameplay;
    private GameManager _gm;
    private SpawnedObjects _spawner;

    private long _count = 0;
    public long Count => _count;

    private void Start()
    {
        DropZoneCollider = GetComponent<BoxCollider2D>();
        if (DropZoneCollider != null)
            DropZoneCollider.enabled = true;
        else
            Debug.Log("DropZoneCollider is not found");

        _gameplay = transform.parent.parent.GetComponent<Gameplay>();
        if (_gameplay == null)
            Debug.Log("Gameplay null");
        _spawner = _gameplay?.GetComponent<SpawnedObjects>();
        if (_spawner == null)
            Debug.Log("_spawner null");
        _gm = _gameplay.GameManager;
    }



    //проверка зоны на объекты
    //
    // отстегнуть, засчитать поинт, 
    // телепортировать или уничтожить объект

    private void TryTeleportCart(Collider2D collision)
    {
        ImprovedCartAttachment obj = collision.gameObject.GetComponent<ImprovedCartAttachment>();
        if (obj != null)
        {
            if (obj.IsAttached() != null)
            {
                obj.DetachChain();
            }

            if (obj.IsAttached() == null && _spawner != null)
            {
                if (_spawner.RespawnObject(obj.gameObject))
                {
                    _count += 1;

                    if (_gm == null)
                        _gm = _gameplay.GameManager;
                    if (_gm != null && _gm.EventBus != null)
                        _gm.EventBus.TriggerPlayerCountUpdateUI(_count);
                }
            }
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
}
