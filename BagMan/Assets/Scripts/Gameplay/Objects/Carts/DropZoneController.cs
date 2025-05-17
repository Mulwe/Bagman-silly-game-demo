using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneController : MonoBehaviour
{
    BoxCollider2D DropZoneCollider;
    private GameObject _gameObjectInZone;


    private Gameplay _gameplay;
    private GameManager _gm;
    private SpawnedObjects _spawner;

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
        if (_gameplay && _spawner)
        {
            Debug.Log($"_spawner not null {_spawner.name}");
        }
    }

    //корутина уничтожается если выбрасывается исключение
    IEnumerator InitWithDelay()
    {
        yield return new WaitForSeconds(1f);
    }


    //spawn behind player's view
    private void RespwanObject(GameObject objectToRespawn)
    {
        List<Vector2> playerViewArea = GetPlayerViewArea();
        if (_spawner == null)
            Debug.Log($"Spawner null");
        if (_spawner != null)
        {
            _spawner.ReSpawnObject(objectToRespawn, playerViewArea);
        }
    }

    private List<Vector2> GetPlayerViewArea()
    {
        Camera camera = Camera.main;
        List<Vector2> playerViewArea = new List<Vector2>();

        float camHeight = 2f * camera.orthographicSize;
        float camWidth = camHeight * camera.aspect;

        float minX = camera.transform.position.x - camWidth / 2f;
        float maxX = camera.transform.position.x + camWidth / 2f;
        float minY = camera.transform.position.y - camHeight / 2f;
        float maxY = camera.transform.position.y + camHeight / 2f;
        playerViewArea.Add(new Vector2(minX, maxY));
        playerViewArea.Add(new Vector2(maxX, maxY));
        playerViewArea.Add(new Vector2(minX, minY));
        playerViewArea.Add(new Vector2(maxX, minY));
        return playerViewArea;
    }

    //проверка зоны на объекты
    //
    // отстегнуть, засчитать поинт, 
    // телепортировать или уничтожить объект

    private void CartCheck(Collider2D collision)
    {
        ImprovedCartAttachment obj = collision.gameObject.GetComponent<ImprovedCartAttachment>();
        if (obj != null)
        {
            // Debug.Log($"Object {obj.name} Attached to {attachedObject?.name}");
            if (obj.IsAttached() != null)
            {
                obj.DetachChain();
            }

            if (obj.IsAttached() == null)
            {

                RespwanObject(obj.gameObject);
                Debug.Log("Add point");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("cart"))
            {
                CartCheck(collision);

                //Add point + check null

                //пристегнутость тележки
                //отстегнуть последнюю, засчитать поинт,
                //телепорт объекта за пределы экрана
            }
        }

        else
            Debug.Log("Collision is null");
    }



    private void OnDisable()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Collider2D col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);

        }
    }
}
