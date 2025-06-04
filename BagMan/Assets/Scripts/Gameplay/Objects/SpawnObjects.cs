using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnedObjects : MonoBehaviour, IInitializable
{
    [Header("Prefab to spawn:")]
    [SerializeField] private GameObject _objPrefab;
    [SerializeField] private int _amount = 5;
    [Header("The zone of spawn:")]
    [SerializeField] private Zone _spawnZone;
    public List<GameObject> _listObjects;

    private bool _isSpawned = false;
    public bool IsInit => _isSpawned;

    public List<GameObject> GetList()
    {
        return this._listObjects;
    }
    public void ChangeAmount(int newAmount)
    {
        this._amount = newAmount;
    }

    public void Run()
    {
        if (_isSpawned)
        {
            Debug.Log("Objects are spawned");
        }
    }

    public void Initialize()
    {
        if (_listObjects == null)
            _listObjects = new List<GameObject>();
        if (_spawnZone != null)
        {
            _spawnZone.Initialize();

            _listObjects = SpawnInZone(_spawnZone);

            _isSpawned = _listObjects != null;
        }
    }

    public List<GameObject> SpawnInZone(Zone s)
    {
        List<GameObject> list;
        //выключу колайдер пр€мо перед проверкой.  
        //»зза функции Physics2D.OverlapBox тригиритс€ на любые колайдеры - видимые и невидимые. 
        if (_objPrefab != null)
        {
            s.SpawnColliderIsActive(false);
            list = Spawner(_objPrefab, s, _amount);
            s.SpawnColliderIsActive(true);
            return list;
        }
        else
        {
            Debug.Log("Prefab not found");
            return _listObjects;
        }
    }


    List<GameObject> Spawner(GameObject prefab, Zone s, int count)
    {
        List<GameObject> objs = new List<GameObject>();
        if (prefab && count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                //check spot and spawn
                GameObject result = CheckAndSpawn(prefab, s);
                if (result)
                    objs.Add(result);
            }
            return objs;
        }
        else
        {
            Debug.Log("Spawned 0 objects. Reference do not exists");
            return (objs);
        }
    }

    private GameObject SpawnCart(GameObject objRef, Vector2 spawnPoint)
    {
        Vector3 point = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
        GameObject newObj = Instantiate(objRef, point, Quaternion.identity, transform);
        newObj.SetActive(true);
        return newObj;
    }

    GameObject CheckAndSpawn(GameObject prefab, Zone s)
    {
        List<Vector2> spawnArea = s.GetSpawnZone();
        List<Vector2> bannedArea = s.GetDropZone();
        GameObject obj;
        int maxAttemps = 50;

        Vector2 spot = Vector2.zero;
        _spawnZone.SpawnColliderIsActive(false);
        for (int i = 0; i < maxAttemps; i++)
        {
            spot.x = Random.Range(s.GetX(spawnArea).minX, s.GetX(spawnArea).maxX);
            spot.y = Random.Range(s.GetY(spawnArea).minY, s.GetY(spawnArea).maxY);
            if (!s.IsPointInRectangle(bannedArea, spot))
            {
                if (IsObstacleFree(prefab, spot))
                {
                    obj = SpawnCart(prefab, spot);
                    _spawnZone.SpawnColliderIsActive(true);
                    return obj;
                }
            }
        }
        _spawnZone.SpawnColliderIsActive(true);
        return null;
    }

    bool IsObstacleFree(GameObject obj, Vector2 targetPosition)
    {
        if (obj == null) return false;

        Collider2D prefabCollider = obj.GetComponent<Collider2D>();
        if (prefabCollider == null) return false;

        Vector2 size = GetColliderSize(prefabCollider);
        //тригирит на любые коллайдеры, даже отключенные.нужно отключать колайдеры перед проверкой
        Collider2D[] hits = Physics2D.OverlapBoxAll(targetPosition, size, 0f);
        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            if (!hit.enabled || !hit.gameObject.activeInHierarchy) continue;
            if (hit.isTrigger) return false;
            if (hit.CompareTag("Player")) return false;
        }
        return true;
    }

    Vector2 GetColliderSize(Collider2D collider)
    {
        switch (collider)
        {
            case BoxCollider2D box:
                return box.size;
            case CircleCollider2D circle:
                float diameter = circle.radius * 2f;
                return new Vector2(diameter, diameter);
            case CapsuleCollider2D capsule:
                return capsule.size;
            default:
                return collider.bounds.size;
        }
    }

    // respawn or destroy object
    public bool RespawnObject(GameObject objOnScene, bool UseRespawnLogic)
    {
        if (_spawnZone == null || objOnScene == null) return false;
        if (UseRespawnLogic)
        {
            return SpawnObjectsOnScene(objOnScene);
        }
        else
        {
            StartCoroutine(DeleteCollectedObject(objOnScene));
            return true;
        }
    }


    private bool SpawnObjectsOnScene(GameObject obj)
    {
        List<Vector2> spawnArea = _spawnZone.GetSpawnZone();
        List<Vector2> bannedArea = new(_spawnZone.GetSpawnZone());

        foreach (var point in bannedArea)
            point.Scale();
        int maxAttemps = 100;

        _spawnZone.SpawnColliderIsActive(false);
        for (int i = 0; i < maxAttemps; i++)
        {
            Vector2 spot;
            spot.x = Random.Range(_spawnZone.GetX(spawnArea).minX, _spawnZone.GetX(spawnArea).maxX);
            spot.y = Random.Range(_spawnZone.GetY(spawnArea).minY, _spawnZone.GetY(spawnArea).maxY);

            if (!_spawnZone.IsPointInRectangle(bannedArea, spot) && !WouldBeVisibleAtPosition(obj, spot))
            {
                if (IsObstacleFree(obj, spot))
                {
                    StartCoroutine(HideAndMove(obj, spot));
                    _spawnZone.SpawnColliderIsActive(true);
                    return true;
                }
            }
        }
        _spawnZone.SpawnColliderIsActive(true);
        return false;
    }

    private IEnumerator DeleteCollectedObject(GameObject obj)
    {
        yield return HideAndMove(obj, new Vector3(-99, -99, -99));
        yield return null;
    }

    private bool WouldBeVisibleAtPosition(GameObject obj, Vector2 position)
    {
        Vector3 originalPosition = obj.transform.position;
        obj.transform.position = new Vector3(position.x, position.y, originalPosition.z);

        bool wouldBeVisible = IsVisibleToCamera2D(obj);
        obj.transform.position = originalPosition;
        return wouldBeVisible;
    }


    //пр€чу объект от игрока пока не отвернетс€
    private IEnumerator HideAndMove(GameObject obj, Vector3 spot)
    {

        SpriteRenderer spriteRenderer = obj?.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;
        spriteRenderer.enabled = false;
        yield return null;  //дл€ обновлени€ bounds

        Rigidbody2D rb = obj?.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = spot;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0f;
        }
        else
            obj.transform.position = spot;

        while (IsVisibleToCamera2D(obj))
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        if (obj != null)
            DestroyObject(obj);

    }

    //учитывает только размер спрайта и или колайдера
    bool IsVisibleToCamera2D(GameObject obj)
    {
        Camera camera = Camera.main;
        if (camera == null || obj == null) return false;

        Bounds bounds;

        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider != null)
            bounds = collider.bounds;
        else
        {
            Vector3 position = obj.transform.position;
            bounds = new Bounds(position, Vector3.one);
        }

        Vector3 min = camera.WorldToViewportPoint(bounds.min);
        Vector3 max = camera.WorldToViewportPoint(bounds.max);

        return (min.z > 0 && max.z > 0 &&
                min.x < 1 && max.x > 0 &&
                min.y < 1 && max.y > 0);
    }


    //удаление объекта со сцены и списка
    // при манипул€ции со списокм нельз€ исользовать foreach
    private void DestroyObject(GameObject obj)
    {
        Debug.Log($"{obj.name} Was destroyed");
        if (_listObjects == null || obj == null || _listObjects.Count == 0)
            return;
        _listObjects.RemoveAll(item => item == obj);

        UnityEngine.Object.Destroy(obj);
        obj = null;
        //Debug.Log("Obj deleted");
    }


    private void DestroyAllObjects()
    {
        if (_listObjects == null || _listObjects.Count == 0) return;
        for (int i = 0; i < _listObjects.Count - 1; i++)
        {
            if (_listObjects[i] != null)
                Destroy(_listObjects[i]);
        }
        _listObjects.Clear();
    }

    private void Start()
    {
        _listObjects = new List<GameObject>();
        if (_spawnZone == null)
        {
            Debug.Log("SpawnObject: Spawnzone reference is null.");
            return;
        }
    }

    private void OnDestroy()
    {
        // Debug.LogWarning($"{name} был уничтожен. Stack trace:\n{System.Environment.StackTrace}");
        DestroyAllObjects();
    }
}
