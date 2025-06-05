using System.Collections.Generic;

using UnityEngine;


public class PoolManager
{
    private List<GameObject> _mainQueue;
    private List<GameObject> _pool;

    private bool _used = false;
    private bool _cleaned;
    private bool _cleanAll;
    public bool IsCleared => _cleaned;
    public bool IsUsing => _used;

    public PoolManager(List<GameObject> activeObjects)
    {
        _pool = new List<GameObject>();
        _cleaned = true;
        if (activeObjects != null)
            _mainQueue = activeObjects;
        _cleanAll = false;
    }

    /// <summary>
    /// Assigns only the reference, does not delete the objects or the previous list.
    /// </summary>
    /// <param name="newActiveObjects"></param>
    public void ReInitializeMainQueue(List<GameObject> newActiveObjects)
    {
        if (newActiveObjects != null && _mainQueue != newActiveObjects)
        {
            _mainQueue = newActiveObjects;
        }
    }

    public void AddToDelete(GameObject obj)
    {
        if (_pool != null && obj != null)
        {
            if (!_pool.Contains(obj))
                _pool?.Add(obj);
            GameObject tmp = obj;
            if (obj != null)
                _mainQueue?.Remove(obj);
            if (tmp != null)
                UnityEngine.Object.Destroy(tmp);
            _cleaned = false;
        }
    }

    public void DeleteMainQueueAfterDespose(bool checker)
    {
        _cleanAll &= checker;
    }

    public void ClearPool()
    {
        if (!_cleaned)
        {
            ClearList(null);
            _cleaned = true;
            // Debug.Log("Pool successfully cleaned up");
        }
    }

    public void ClearMainQueue()
    {
        ClearList(_mainQueue);
    }

    public void ClearList(List<GameObject> lst)
    {
        lst ??= _pool;

        if (lst == null || lst.Count == 0) return;
        for (int i = 0; i < lst.Count; i++)
        {
            if (lst[i] != null)
            {
                UnityEngine.Object.Destroy(lst[i]);
                lst[i] = null;
            }
        }
        if (lst.Count > 0) lst.Clear();
    }



    public void Dispose()
    {
        if (!_cleaned)
        {
            ClearPool();
            _cleaned = true;
        }

        if (_cleanAll)
        {
            Debug.Log("list of Gameobjects deleted");
            ClearMainQueue();
        }
    }

}
