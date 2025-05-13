using System.Collections.Generic;
using UnityEngine;

//send Player position for sorting sprites
public class SortingOrderController : MonoBehaviour
{
    public List<DynamicSorting> trackedObjects;

    void Update()
    {
        foreach (var obj in trackedObjects)
        {
            obj.UpdateSortingRelativeTo(transform);
        }
    }
}
