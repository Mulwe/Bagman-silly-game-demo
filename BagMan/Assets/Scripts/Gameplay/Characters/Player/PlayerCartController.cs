using UnityEngine;

public class PlayerCartController : MonoBehaviour
{
    [Header("Cart Attachment")]
    public Transform attachPoint;
    public bool hasCartAttached = false;

    [Header("Cart Control")]
    public KeyCode detachAllKey = KeyCode.R;

    [SerializeField] private int _limitAttachedCarts = 4;
    private int _attachedCarts = 0;
    public int AttachedCarts => _attachedCarts;
    bool _isLogging = false;


    private Animator _animator;

    void Start()
    {
        // Create attachment point if not set
        _attachedCarts = 0;
        InitializeAttachPoint();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detach all carts when pressing the detach key
        if (hasCartAttached && Input.GetKeyDown(detachAllKey))
        {
            DetachAllCarts();
        }
    }

    private void Log(string message)
    {
        if (_isLogging)
            Debug.Log(message);
    }

    private void OnEnable()
    {
        ImprovedCartAttachment.OnCartAttached += HandleCartAttached;
        ImprovedCartAttachment.OnCartDetached += HandleCartDetached;

    }
    private void OnDisable()
    {
        ImprovedCartAttachment.OnCartAttached -= HandleCartAttached;
        ImprovedCartAttachment.OnCartDetached -= HandleCartDetached;
    }


    public bool ReachedLimit()
    {
        if (_attachedCarts >= _limitAttachedCarts)
        {
            Log($"Player has reached the cart limit. [{_attachedCarts}]/[{_limitAttachedCarts}] {this.gameObject}");
            return true;
        }
        return false;
    }

    private void HandleCartDetached()
    {
        if (_attachedCarts < 0)
            _attachedCarts = 0;
        else
            _attachedCarts--;

    }

    private void HandleCartAttached()
    {

        if (_attachedCarts > _limitAttachedCarts)
            _attachedCarts = _limitAttachedCarts;
        else
            _attachedCarts++;
        //Debug.Log($"Player have [{_attachedCarts}] attached carts");
    }

    void InitializeAttachPoint()
    {
        if (attachPoint == null)
        {
            GameObject attachPointObj = new GameObject("CartAttachPoint");
            attachPointObj.transform.SetParent(transform);
            BoxCollider2D collider = GetComponent<BoxCollider2D>();

            if (collider != null)
            {
                // нижняя граница: 
                float bottomY = collider.offset.y - (collider.size.y / 2f);
                attachPointObj.transform.localPosition = new Vector3(0f, bottomY, 0f);
            }
            else
            {
                // Fallback, если нет коллайдера
                attachPointObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            }
            attachPoint = attachPointObj.transform;
        }
    }


    void DetachAllCarts()
    {
        Collider2D[] cartColliders = Physics2D.OverlapCircleAll(attachPoint.position, 0.5f);
        foreach (Collider2D cartCollider in cartColliders)
        {
            ImprovedCartAttachment cart = cartCollider.GetComponent<ImprovedCartAttachment>();
            if (cart != null && cart.enabled)
            {

                cart.DetachChain();
                _attachedCarts = 0;
                break;
            }
        }

        hasCartAttached = false;
        _attachedCarts = 0;
    }


    void OnDrawGizmosSelected()
    {
        if (attachPoint != null)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(attachPoint.position, 0.09f);


            Gizmos.color = new Color(0, 0, 1, 0.3f);
            Gizmos.DrawSphere(attachPoint.position, 0.5f);
        }

    }
}
