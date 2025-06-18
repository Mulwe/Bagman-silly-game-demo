using UnityEngine;

public class SortingOrderManager : MonoBehaviour
{
    private GameObject _player;

    public float viewDistance = 3f;
    private SpriteRenderer sr;

    private int baseSortingOrder;
    private SpriteRenderer playerSr;

    private HingeJoint2D _hinge;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseSortingOrder = sr.sortingOrder;

        _player = GameObject.FindWithTag("Player");
        playerSr = _player.GetComponent<SpriteRenderer>();

        _hinge = GetComponent<HingeJoint2D>();
    }

    void Update()
    {
        if (playerSr != null && _player != null)
        {
            float distanceSqr = (transform.position - _player.transform.position).sqrMagnitude;
            if (distanceSqr < viewDistance * viewDistance)
            {
                bool isConnected = _hinge != null && _hinge.enabled;
                if (isConnected)
                {
                    if ((transform.position.y > _player.transform.position.y))
                        sr.sortingOrder = playerSr.sortingOrder + 1;
                    else
                        sr.sortingOrder = playerSr.sortingOrder - 1;
                }
                if ((transform.position.y > _player.transform.position.y))
                    sr.sortingOrder = playerSr.sortingOrder - 1;
                else
                    sr.sortingOrder = playerSr.sortingOrder + 1;
            }
            else
            {
                sr.sortingOrder = baseSortingOrder;
            }
        }
    }
}
