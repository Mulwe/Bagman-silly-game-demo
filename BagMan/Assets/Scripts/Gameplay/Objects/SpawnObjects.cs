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
        GameObject newObj = Instantiate(objRef, point, Quaternion.identity);
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
        for (int i = 0; i < maxAttemps; i++)
        {
            spot.x = Random.Range(s.GetX(spawnArea).minX, s.GetX(spawnArea).maxX);
            spot.y = Random.Range(s.GetY(spawnArea).minY, s.GetY(spawnArea).maxY);
            if (!s.IsPointInRectangle(bannedArea, spot))
            {
                if (IsObstacleFree(prefab, spot))
                {
                    obj = SpawnCart(prefab, spot);
                    return obj;
                }
            }
        }
        return null;
    }

    bool IsObstacleFree(GameObject obj, Vector2 targetPosition)
    {
        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider == null) return false;

        Vector2 size = collider.bounds.size;
        //тригирит на любые коллайдеры, даже отключенные.нужно отключать колайдеры перед проверкой
        Collider2D hit = Physics2D.OverlapBox(targetPosition, size, 0f);

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Vector2 spriteSize = sr.size;
            Collider2D spriteHit = Physics2D.OverlapBox(targetPosition, size, 0f);
            return (hit == null && spriteHit == null);
        }
        return hit == null;
    }

    // SpawnObject behind player's view. inside spawned area
    public bool RespawnObject(GameObject objOnScene)
    {
        if (_spawnZone == null || objOnScene == null) return false;

        List<Vector2> spawnArea = _spawnZone.GetSpawnZone();
        List<Vector2> bannedArea = _spawnZone.GetDropZone();

        int maxAttemps = 100;

        _spawnZone.SpawnColliderIsActive(false);
        for (int i = 0; i < maxAttemps; i++)
        {
            Vector2 spot;
            //GetPlayerViewArea(out CameraViewArea);
            spot.x = Random.Range(_spawnZone.GetX(spawnArea).minX, _spawnZone.GetX(spawnArea).maxX);
            spot.y = Random.Range(_spawnZone.GetY(spawnArea).minY, _spawnZone.GetY(spawnArea).maxY);

            if (!_spawnZone.IsPointInRectangle(bannedArea, spot) && !WouldBeVisibleAtPosition(objOnScene, spot))
            {
                if (IsObstacleFree(objOnScene, spot))
                {
                    StartCoroutine(HideAndMove(objOnScene, spot));
                    _spawnZone.SpawnColliderIsActive(true);
                    return true;
                }
            }
        }
        _spawnZone.SpawnColliderIsActive(true);
        return false;
    }

    bool WouldBeVisibleAtPosition(GameObject obj, Vector2 position)
    {
        Vector3 originalPosition = obj.transform.position;
        obj.transform.position = new Vector3(position.x, position.y, originalPosition.z);

        bool wouldBeVisible = IsVisibleToCamera2D(obj);
        obj.transform.position = originalPosition;
        return wouldBeVisible;
    }



    //пр€чу объект от игрока пока не отвернетс€
    IEnumerator HideAndMove(GameObject obj, Vector2 spot)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        spriteRenderer.enabled = false;
        yield return null;//дл€ обновлени€ bounds

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
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
        spriteRenderer.enabled = true;
    }

    //учитывает только размер спрайта и или колайдера
    bool IsVisibleToCamera2D(GameObject obj)
    {
        Camera camera = Camera.main;
        if (camera == null) return false;

        Bounds bounds;

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.enabled)
            bounds = spriteRenderer.bounds;
        else
        {
            Collider2D collider = obj.GetComponent<Collider2D>();
            if (collider != null)
                bounds = collider.bounds;
            else
            {
                Vector3 position = obj.transform.position;
                bounds = new Bounds(position, Vector3.one);
            }
        }

        Vector3 min = camera.WorldToViewportPoint(bounds.min);
        Vector3 max = camera.WorldToViewportPoint(bounds.max);

        return (min.z > 0 && max.z > 0 &&
                min.x < 1 && max.x > 0 &&
                min.y < 1 && max.y > 0);
    }


    //PlayerView box 
    private void GetPlayerViewArea(out List<Vector2> playerViewArea)
    {
        Camera camera = Camera.main;

        float camHeight = 2f * camera.orthographicSize;
        float camWidth = camHeight * camera.aspect;

        float minX = camera.transform.position.x - camWidth / 2f;
        float maxX = camera.transform.position.x + camWidth / 2f;
        float minY = camera.transform.position.y - camHeight / 2f;
        float maxY = camera.transform.position.y + camHeight / 2f;
        playerViewArea = GetPlayerViewRectange(minX, maxX, minY, maxY);
        //size = new Vector2(maxX - minX, maxY - minY);
    }

    private List<Vector2> GetPlayerViewRectange(float minX, float maxX, float minY, float maxY)
    {
        List<Vector2> playerViewArea = new List<Vector2>();
        playerViewArea.Add(new Vector2(minX, maxY));
        playerViewArea.Add(new Vector2(maxX, maxY));
        playerViewArea.Add(new Vector2(minX, minY));
        playerViewArea.Add(new Vector2(maxX, minY));
        return playerViewArea;
    }





    //удаление объекта со сцены и списка
    // при манипул€ции со списокм нельз€ исользовать foreach
    private void Destroy(GameObject obj)
    {
        if (_listObjects == null || obj == null || _listObjects.Count == 0)
            return;
        _listObjects.RemoveAll(item =>
        item != null && item.GetInstanceID() == obj.GetInstanceID());

        UnityEngine.Object.Destroy(obj);
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
