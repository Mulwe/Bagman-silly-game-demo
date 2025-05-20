using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Zone : MonoBehaviour, IInitializable
{
    [Header("BoxCollider2D from SpawnZone")]
    [SerializeField] private BoxCollider2D _spawnZone;
    private BoxCollider2D _dropZone;

    private List<Vector2> _spawn;
    private List<Vector2> _drop;

    private void OnValidate()
    {
        _spawnZone ??= GetComponent<BoxCollider2D>();
    }

    //Main Init
    public void Initialize()
    {
        Transform child = _spawnZone.transform.GetChild(0);
        _dropZone = child.GetComponent<BoxCollider2D>();
        _spawn = new List<Vector2>();
        _drop = new List<Vector2>();
        InitializeSpawnVertices(_spawn, _drop);
    }

    public void SpawnColliderIsActive(bool status)
    {
        if (_spawnZone != null)
        {
            _spawnZone.enabled = status;
        }
    }

    public List<Vector2> GetSpawnZone()
    {
        if (_spawn == null)
            return null;
        return _spawn;
    }

    public List<Vector2> GetDropZone()
    {
        if (_drop == null)
            return null;
        return _drop;
    }

    public (float minX, float maxX) GetX(List<Vector2> zone)
    {
        if (zone == null)
        {
            Debug.LogError("Zone list is null or empty!");
            return (0f, 0f);
        }
        float minX = zone.Min(point => point.x);
        float maxX = zone.Max(point => point.x); ;
        return (minX, maxX);
    }

    public (float minY, float maxY) GetY(List<Vector2> zone)
    {
        if (zone == null)
        {
            Debug.LogError("Zone list is null or empty!");
            return (0f, 0f);
        }
        float minY = zone.Min(point => point.y);
        float maxY = zone.Max(point => point.y); ;
        return (minY, maxY);
    }

    //инициализаци€ двух списков с вершиныами колайдеров
    private void InitializeSpawnVertices(List<Vector2> spawn, List<Vector2> drop)
    {
        RecordColliderVertices(_spawnZone, spawn);
        RecordColliderVertices(_dropZone, drop);
        if (drop == null || spawn == null)
        {
            Debug.Log("Drop/Spawn zone are not init");
        }
    }

    //запись в список все вершины одного колайдера
    private void RecordColliderVertices(BoxCollider2D _zone, List<Vector2> vertices)
    {
        if (_zone.transform.position != Vector3.zero)
        {
            Bounds zone = _zone.bounds;
            vertices.Add(new Vector2(zone.min.x, zone.max.y));
            vertices.Add(new Vector2(zone.max.x, zone.max.y));
            vertices.Add(new Vector2(zone.min.x, zone.min.y));
            vertices.Add(new Vector2(zone.max.x, zone.min.y));
        }
        else Debug.Log($"Error: Spawn zone not found");
    }

    //ѕроверка вхождени€ точки в зону коллайдера
    //Ray Casting algorithm
    public bool IsPointInRectangle(List<Vector2> vertices, Vector2 point)
    {
        bool inside = false;
        int n = vertices.Count;
        Vector2 p = point;
        List<Vector2> v = vertices;
        for (int i = 0, j = n - 1; i < vertices.Count; j = ++i)
        {
            if (((v[i].y > p.y) != (v[j].y > p.y)) &&
                (p.x < (v[j].x - v[i].x) * (p.y - v[i].y) / (v[j].y - v[i].y) + v[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private void OnDestroy()
    {
        _spawn?.Clear();
        _drop?.Clear();
    }
}
