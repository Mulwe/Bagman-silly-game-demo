using UnityEngine;

public class DynamicSorting : MonoBehaviour
{
    public int baseSortingOrder = 0;
    public float viewDistance = 3f; // Радиус действия игрока
    private SpriteRenderer sr;
    private Transform player;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseSortingOrder = sr.sortingOrder;
        player = GameObject.FindWithTag("Player").transform;
    }

    public void UpdateSortingRelativeTo(Transform player)
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < viewDistance)
        {
            var IsConnected = transform.GetComponent<HingeJoint2D>();
            if (IsConnected)
            {
                if (transform.position.y > player.position.y)
                    sr.sortingOrder = 101;
                else
                    sr.sortingOrder = 99;
            }
            else
            {
                if (transform.position.y > player.position.y)
                    sr.sortingOrder = 99;
                else
                    sr.sortingOrder = 101;
            }
        }
        else
        {
            sr.sortingOrder = baseSortingOrder;
        }
    }
}