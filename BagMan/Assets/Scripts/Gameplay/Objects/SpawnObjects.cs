using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnedObjects : MonoBehaviour
{
    [Header("Prefab to spawn:")]
    [SerializeField] private GameObject _objPrefab;
    [SerializeField] private int _amount = 5;

    [Header("The zone of spawn:")]
    [SerializeField] private Zone _spawnZone;
    public List<GameObject> listObjects;

    private bool _isSpawned = false;

    private PoolManager _poolManager;
    private Coroutine _coroutine;

    public PoolManager PoolManager => _poolManager;
    public bool IsInit => _isSpawned;


    public List<GameObject> GetList()
    {
        return this.listObjects;
    }

    public void ChangeAmount(int newAmount)
    {
        this._amount = newAmount;
    }

    public void Run()
    {
        if (_isSpawned)
            Debug.Log("Objects are spawned");
    }

    public void Initialize()
    {
        if (_spawnZone != null)
        {
            _spawnZone.Initialize();
            _poolManager = new PoolManager(listObjects);
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = StartCoroutine(PoolCleanupCoroutine(_poolManager));
            _isSpawned = SpawnInZone(_spawnZone);
        }
    }

    public void Initialize(int cartAmount)
    {
        ChangeAmount(cartAmount);
        if (_spawnZone != null)
        {
            _spawnZone.Initialize();
            _poolManager = new PoolManager(listObjects);
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = StartCoroutine(PoolCleanupCoroutine(_poolManager));
            _isSpawned = SpawnInZone(_spawnZone);
        }
    }

    public void Dispose()
    {
        if (_poolManager != null)
            Clear();
        if (listObjects != null)
        {
            if (listObjects.Count > 0)
            {
                foreach (GameObject obj in listObjects)
                {
                    UnityEngine.Object.Destroy(obj);
                }
                listObjects.Clear();
                listObjects = null;
                _isSpawned = false;
            }
        }
    }


    public bool SpawnInZone(Zone s)
    {
        //выключу колайдер прямо перед проверкой.  
        //Изза функции Physics2D.OverlapBox тригирится на любые колайдеры - видимые и невидимые. 

        if (_objPrefab != null)
        {
            listObjects = Spawner(_objPrefab, s, _amount);
            return listObjects != null;
        }
        Debug.Log("Prefab not found");
        return false;
    }

    private IEnumerator PoolCleanupCoroutine(PoolManager p)
    {
        //Debug.Log("Pool is working");
        if (p == null) yield break;

        while (p != null)
        {
            p.ClearPool();
            yield return new WaitForSeconds(5f);
        }
    }

    private List<GameObject> Spawner(GameObject prefab, Zone s, int count)
    {
        List<GameObject> objsList = null;
        if (prefab && count > 0)
        {
            objsList = new List<GameObject>();
            s.SpawnColliderIsActive(false);
            for (int i = 0; i < count; i++)
            {
                GameObject result = CheckAndSpawn(prefab, s);
                if (result)
                {
                    objsList.Add(result);
                }
            }
            s.SpawnColliderIsActive(true);
            return objsList;
        }
        else
        {
            Debug.Log("Spawned 0 objects. Reference do not exists");
            return (objsList);
        }
    }


    private GameObject SpawnCart(GameObject objRef, Vector3 spawnPoint)
    {
        Vector3 point = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
        GameObject newObj = Instantiate(objRef, point, Quaternion.identity, transform);
        newObj.SetActive(true);
        return newObj;
    }

    GameObject CheckAndSpawn(GameObject prefab, Zone s)
    {
        List<Vector2> spawnArea = s.GetSpawnZone();
        List<Vector2> bannedArea = new(_spawnZone.GetSpawnZone());

        ShiftBannedZone(bannedArea);


        int maxAttemps = 100;

        Vector2 spot = Vector2.zero;
        GameObject obj = SpawnCart(prefab, new Vector3(0, 0, -100));
        for (int i = 0; i < maxAttemps; i++)
        {
            spot.x = Random.Range(s.GetX(spawnArea).minX, s.GetX(spawnArea).maxX);
            spot.y = Random.Range(s.GetY(spawnArea).minY, s.GetY(spawnArea).maxY);
            if (!s.IsPointInRectangle(bannedArea, spot))
            {
                if (IsObstacleFree(obj, spot))
                {
                    obj.transform.position = spot;
                    return obj;
                }
            }
        }
        if (obj != null)
            UnityEngine.Object.Destroy(obj);
        return null;
    }

    bool IsObstacleFree(GameObject obj, Vector2 targetPosition)
    {
        if (obj == null) return false;

        if (obj.TryGetComponent<Collider2D>(out var prefabCollider))
        {
            Vector2 size = GetColliderSize(prefabCollider);
            Collider2D[] hits = Physics2D.OverlapBoxAll(targetPosition, size, 0f);
            foreach (Collider2D hit in hits)
            {
                if (hit == null || !hit.enabled || !hit.gameObject.activeInHierarchy) continue;
                if (hit.isTrigger || hit.CompareTag("Player")) return false;
            }
            return true;
        }
        else return false;
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

    private void ShiftBannedZone(List<Vector2> bannedArea)
    {
        float offset = 0.5f;
        Vector2 center = Vector2.zero;
        foreach (var v in bannedArea)
            center += v;
        center /= bannedArea.Count;
        for (int i = 0; i < bannedArea.Count; i++)
        {
            Vector2 direction = (bannedArea[i] - center).normalized;
            bannedArea[i] += direction * offset;
        }
    }

    private bool SpawnObjectsOnScene(GameObject obj)
    {
        List<Vector2> spawnArea = _spawnZone.GetSpawnZone();
        List<Vector2> bannedArea = new(_spawnZone.GetSpawnZone());

        ShiftBannedZone(bannedArea);
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
                    //AddedToPool
                    StartCoroutine(HideAndMove(obj, spot, true));
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
        yield return StartCoroutine(HideAndMove(obj, new Vector3(-99, -99, -99), false));
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


    private IEnumerator HideAndMove(GameObject obj, Vector3 spot, bool isRespawn)
    {
        SpriteRenderer spriteRenderer = null;
        Rigidbody2D rb = null;
        if (obj != null)
            spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;
        spriteRenderer.enabled = false;
        yield return null;  //для обновления bounds

        if (obj != null)
            rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = spot;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0f;
        }
        else
            obj.transform.position = spot;
        while (isRespawn && IsVisibleToCamera2D(obj))
        {
            yield return new WaitForSeconds(1.0f);
        }

        if (!isRespawn && _poolManager != null)
        {
            //Debug.Log("Помечен для удаления");
            _poolManager.AddToDelete(obj);
        }
    }


    //учитывает только размер спрайта и или колайдера
    bool IsVisibleToCamera2D(GameObject obj)
    {
        Camera camera = Camera.main;
        if (camera == null || obj == null) return false;

        Bounds bounds;

        Collider2D collider = null;
        if (obj != null && obj.TryGetComponent<Collider2D>(out var result))
            collider = result;

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

    private void Start()
    {
        if (_spawnZone == null)
        {
            Debug.Log("SpawnObject: Spawnzone reference is null.");
            return;
        }
    }

    private void Clear()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _poolManager?.DeleteMainQueueAfterDespose(true);
        _poolManager?.Dispose();
    }

    private void OnDestroy()
    {

        Clear();
    }
}
