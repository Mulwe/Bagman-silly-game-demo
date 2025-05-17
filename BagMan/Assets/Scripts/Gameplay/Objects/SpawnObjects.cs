
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
                {
                    objs.Add(result);
                }
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

    public bool ReSpawnObject(GameObject objOnScene, List<Vector2> playerViewArea)
    {
        bool spawned = false;

        if (_spawnZone == null && objOnScene && playerViewArea.Count > 0)
        {
            List<Vector2> spawnArea = _spawnZone.GetSpawnZone();
            List<Vector2> bannedArea = playerViewArea;
            int maxAttemps = 50;
            _spawnZone.SpawnColliderIsActive(false);
            Vector2 spot = Vector2.zero;
            for (int i = 0; i < maxAttemps; i++)
            {
                spot.x = Random.Range(_spawnZone.GetX(spawnArea).minX, _spawnZone.GetX(spawnArea).maxX);
                spot.y = Random.Range(_spawnZone.GetY(spawnArea).minY, _spawnZone.GetY(spawnArea).maxY);
                if (!_spawnZone.IsPointInRectangle(bannedArea, spot))
                {

                    if (IsObstacleFree(objOnScene, spot))
                    {

                        // тут есть у объекта rigidbody
                        Rigidbody rb = objOnScene.GetComponent<Rigidbody>();
                        rb.position = spot;
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        spawned = true;
                    }
                }
            }
            _spawnZone.SpawnColliderIsActive(true);
        }

        return spawned;
    }

    bool IsObstacleFree(GameObject obj, Vector2 targetPosition)
    {
        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("Collider2D is missing!");
            return false;
        }
        Vector2 size = collider.bounds.size;
        //тригирит на любые коллайдеры, даже отключенные
        Collider2D hit = Physics2D.OverlapBox(targetPosition, size, 0f);
        return hit == null;
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

    //очиста 
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
        Debug.LogWarning($"{name} был уничтожен. Stack trace:\n{System.Environment.StackTrace}");
        DestroyAllObjects();
    }
}
