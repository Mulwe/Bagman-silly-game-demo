using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ImprovedCartAttachment : MonoBehaviour
{
    [Header("Debug: ")]
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private PlayerCartController _playerCartController;

    private PlayerInputActions _inputActions;

    [Header("Attachment Settings")]
    public float attachDistance = 1.5f;
    public LayerMask playerLayer;
    public LayerMask cartLayer;

    [Header("Interaction buttons:")]
    public KeyCode AttachButton = KeyCode.E;
    public KeyCode DetachButton = KeyCode.Q;

    //Tip collider 
    CircleCollider2D interactionArea;

    [Header("Connection Points")]
    public Transform frontPoint;
    public Transform backPoint;

    [Header("Joint Settings")]
    public float jointAngleLimit = 5f;
    public float positionOffset = 0.1f;

    [Header("Debug Info")]
    [SerializeField] private bool isAttached = false;
    [SerializeField] private bool isAttachedToPlayer = false;
    [SerializeField] private bool hasSomethingBehind = false;

    public bool Linked => isAttached || isAttachedToPlayer || hasSomethingBehind;


    // References
    private HingeJoint2D joint;
    private Rigidbody2D rb;
    private GameObject connectedObject;

    // static для вызова одного общего события
    public static event Action OnCartAttached;
    public static event Action OnCartDetached;

    private readonly int _maxCarts = 4;


    Vector3 DebugPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeAttachmentPoints();

        DebugPosition = transform.position;

        if (_playerObject == null)
            _playerObject = GameObject.FindWithTag("Player");
        if (_playerObject != null)
            _playerCartController = _playerObject.GetComponent<PlayerCartController>();
        else
            Debug.LogError("Player Cart Controller not found!");

        AddInteractionCollider(attachDistance);
        InitActionControl(_playerObject);
    }

    private void InitActionControl(GameObject _player)
    {
        if (_player != null)
        {
            this._inputActions = _player.GetComponent<PlayerController>().InputActions;
            this.AddListeners();
        }
    }

    private void AddListeners()
    {
        if (this._inputActions != null)
        {
            this._inputActions.Player.DropOff.performed += this.OnDetach;
            this._inputActions.Player.PickUp.performed += this.OnAttach;
        }
    }

    private void RemoveListeners()
    {
        if (this._inputActions != null)
        {
            this._inputActions.Player.DropOff.performed -= this.OnDetach;
            this._inputActions.Player.PickUp.performed -= this.OnAttach;
        }
    }

    private void OnDetach(InputAction.CallbackContext context)
    {
        if (Linked)
        {
            if (isAttached && context.performed)
            {
                Detach();
                return;
            }
        }
    }

    private void OnAttach(InputAction.CallbackContext context)
    {
        if (!isAttached && context.performed)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attachDistance, playerLayer);
            TryAttach(playerCollider);
        }

    }




    private void OnDisable()
    {
        this.RemoveListeners();
    }

    private void FixedUpdate()
    {

        if (Linked && interactionArea != null)
            interactionArea.enabled = false;
        else
            interactionArea.enabled = true;
    }

    //подсказки. скрыто. не реализован 
    private void AddInteractionCollider(float radius)
    {
        if (gameObject != null && gameObject.GetComponent<CircleCollider2D>() == null)
        {
            interactionArea = gameObject.AddComponent<CircleCollider2D>();
            if (interactionArea != null)
            {
                interactionArea.isTrigger = true;
                interactionArea.radius = radius;
                interactionArea.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision is CircleCollider2D)
            {
                if (!Linked && _playerCartController != null)
                {
                    if (_playerCartController.AttachedCarts < _maxCarts)
                        _playerCartController.HandleShowPickUpTip(null);
                    else
                        _playerCartController.HandleShowPickUpTip("Q - to drop");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision is CircleCollider2D)
            {
                if (_playerCartController != null)
                {
                    if (!Linked && _playerCartController.AttachedCarts == _maxCarts)
                    {
                        _playerCartController.HandleHidePickUpTip();
                    }
                }
            }
        }
    }
    private void InitializeAttachmentPoints()
    {
        if (frontPoint == null)     // Create front attachment point if needed
        {
            GameObject front = new GameObject("FrontPoint");
            front.transform.SetParent(transform);
            front.transform.localPosition = new Vector3(0, 0.5f, 0);
            frontPoint = front.transform;
        }

        if (backPoint == null)      // Create back attachment point if needed
        {
            GameObject back = new GameObject("BackPoint");
            back.transform.SetParent(transform);
            back.transform.localPosition = new Vector3(0, -0.5f, 0);
            backPoint = back.transform;
        }
    }

    void TryAttach(Collider2D playerCollider)
    {
        if (_playerCartController != null)
        {
            if (_playerCartController.ReachedLimit())
                return;
        }
        if (isAttached == false) // чтобы скипал все те что уже пристегнуты
        {
            bool CartInPlayerZone = TryAttachCheckPlayer(playerCollider);
            TryAttachCheckCarts(CartInPlayerZone);
        }
    }


    //  First joint from player to cart 
    bool TryAttachCheckPlayer(Collider2D playerCollider)
    {
        if (playerCollider != null)
        {
            PlayerCartController plCrtCtrl = _playerCartController;

            if (plCrtCtrl != null && !plCrtCtrl.hasCartAttached)
            {
                AttachToPlayer(playerCollider.gameObject);
                plCrtCtrl.hasCartAttached = true;
                return false;
            }
            else if (plCrtCtrl == null)
            {
                AttachToPlayer(playerCollider.gameObject);
            }
            else
            {
                //Debug.Log("Player already has a cart attached!");
                //attach this one to the end of line
                return (true);
            }
        }
        return false;
    }

    // this part means the player already has a connected cart.
    // Check for nearby carts; if successful, attach this cart to the last cart in the chain
    void TryAttachCheckCarts(bool CartInPlayerZone)
    {
        // after we found last cart in line,connect the cart in player zone, if possible
        int amount = _playerCartController.AttachedCarts;
        Collider2D[] cartColliders = Physics2D.OverlapCircleAll(transform.position, attachDistance + (amount - 1) * (0.5f), cartLayer);

        foreach (Collider2D cartCollider in cartColliders)
        {
            ImprovedCartAttachment currentCart = cartCollider.GetComponent<ImprovedCartAttachment>();
            if (cartCollider.gameObject == gameObject && currentCart.isAttached)
                continue;
            if (currentCart != null && currentCart.isAttached && !currentCart.hasSomethingBehind)
            {
                if (AttachToCart(cartCollider.gameObject)) { }
                return;
            }
        }
    }

    private void AttachToPlayer(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb == null)
        {
            Debug.LogWarning("Player doesn't have a Rigidbody2D! Adding one...");
            playerRb = player.AddComponent<Rigidbody2D>();
            playerRb.freezeRotation = true;
        }
        // Position behind player FIRST (before creating joint)
        PlayerCartController playerController = player.GetComponent<PlayerCartController>();
        Vector3 attachPosition;
        if (playerController != null && playerController.attachPoint != null)
        {
            // Use player's attach point
            attachPosition = playerController.attachPoint.position;
            // Set our position and rotation to match the player's orientation
            transform.position = attachPosition;
            transform.rotation = player.transform.rotation;
        }
        else
        {
            // Use default positioning
            Vector3 playerPos = player.transform.position;
            attachPosition = playerPos - player.transform.up * 1.0f;
            transform.position = attachPosition;
            transform.rotation = player.transform.rotation;
        }
        // Reset velocities BEFORE creating joint
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
        // Create joint AFTER positioning
        joint = gameObject.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false; // Important: disable auto-configure
        joint.connectedBody = playerRb;
        // Configure joint
        joint.anchor = frontPoint.localPosition;
        if (playerController != null && playerController.attachPoint != null)
        {
            // Convert player's attach point to local space
            Vector2 localAttachPoint = playerRb.transform.InverseTransformPoint(playerController.attachPoint.position);
            joint.connectedAnchor = localAttachPoint;
        }
        else
        {
            // Default attachment to bottom of player if no attach point
            joint.connectedAnchor = new Vector2(0, -0.5f);
        }
        // Stabilize joint
        joint.useLimits = true;
        JointAngleLimits2D limits = new JointAngleLimits2D();
        limits.min = -jointAngleLimit;
        limits.max = jointAngleLimit;
        joint.limits = limits;

        // Update state
        isAttached = true;
        isAttachedToPlayer = true;
        connectedObject = player;
        OnCartAttached?.Invoke();

        //Debug.Log(gameObject.name + " attached to player");
    }

    private bool AttachToCart(GameObject cart)
    {
        ImprovedCartAttachment otherCart = cart.GetComponent<ImprovedCartAttachment>();
        Rigidbody2D otherRb = cart.GetComponent<Rigidbody2D>();
        if (otherRb == null)
        {
            Debug.LogError("Cart doesn't have a Rigidbody2D!");
            return false;
        }
        // проверка где находится backpoint у тележки
        // first position precisely behind the cart
        Vector3 attachPosition = otherCart.backPoint.position;

        // set our position, Rotation to match the cart's orientation
        transform.position = attachPosition;
        transform.rotation = cart.transform.rotation;

        // apply a small offset to prevent physics overlap
        transform.position -= transform.up * positionOffset;

        // reset velocities BEFORE creating joint
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            // ensure the cart is stationary for a moment to avoid physics jitter
            rb.Sleep();
        }
        // small delay to let physics settle
        StartCoroutine(CreateJointDelayed(otherCart, otherRb));

        // update state immediately
        isAttached = true;
        isAttachedToPlayer = false;
        otherCart.hasSomethingBehind = true;
        connectedObject = cart;
        OnCartAttached?.Invoke();
        // Debug.Log(gameObject.name + " attached to cart " + cart.name);
        return true;
    }

    IEnumerator CreateJointDelayed(ImprovedCartAttachment otherCart, Rigidbody2D otherRb)
    {
        // Small delay to let physics settle
        yield return new WaitForFixedUpdate();

        // Create joint AFTER positioning
        if (joint != null)
        {
            Destroy(joint);
        }

        joint = gameObject.AddComponent<HingeJoint2D>();
        //Important: disable auto-configure
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = otherRb;

        //Configure joint
        joint.anchor = frontPoint.localPosition;
        joint.connectedAnchor = otherCart.backPoint.localPosition;

        //add some damping to reduce oscillation
        joint.useMotor = true;
        JointMotor2D motor = new JointMotor2D();
        motor.maxMotorTorque = 10f;  // adjust as needed
        motor.motorSpeed = 0;
        joint.motor = motor;

        //stabilize Joint
        joint.useLimits = true;
        JointAngleLimits2D limits = new JointAngleLimits2D();
        limits.min = -jointAngleLimit;
        limits.max = jointAngleLimit;
        joint.limits = limits;
    }

    void Detach()
    {
        if (connectedObject != null)
        {
            PlayerCartController plCrtController = null;
            connectedObject.TryGetComponent<PlayerCartController>(out var tmp);
            if (tmp != null)
                plCrtController = tmp;
            if (joint != null)
            {
                if (isAttachedToPlayer)
                {
                    // Update player's attached status
                    if (plCrtController != null)
                        plCrtController.hasCartAttached = false;
                }
                else
                {
                    // Update other cart's status
                    ImprovedCartAttachment connectedCart = connectedObject.GetComponent<ImprovedCartAttachment>();
                    if (connectedCart != null)
                        connectedCart.hasSomethingBehind = false;
                }
                Destroy(joint);
                joint = null;
            }
        }
        isAttached = false;
        isAttachedToPlayer = false;
        connectedObject = null;
        OnCartDetached?.Invoke();
    }

    public GameObject IsAttached()
    {
        if (connectedObject != null)
            return connectedObject;
        return null;
    }

    // Recursively detach the chain
    public void DetachChain()
    {
        //detach this cart first
        Detach();
        Collider2D[] cartColliders = Physics2D.OverlapCircleAll(backPoint.position, 0.5f, cartLayer);
        foreach (Collider2D cartCollider in cartColliders)
        {
            if (cartCollider.gameObject == gameObject) continue;
            ImprovedCartAttachment attachedCart = cartCollider.GetComponent<ImprovedCartAttachment>();
            if (attachedCart != null && attachedCart.isAttached)
            {
                attachedCart.DetachChain();
                break;
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attachDistance);

        // Draw attachment points
        if (frontPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(frontPoint.position, 0.05f);
        }

        if (backPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(backPoint.position, 0.05f);
        }
    }

    void OnDrawGizmos()
    {
        bool swap = true;
        // Draw detection radius
        if (DebugPosition != Vector3.zero && Input.GetKeyDown(KeyCode.E))
        {
            if (swap)
                Gizmos.color = Color.red;
            else
                Gizmos.color += Color.green;
            swap = !swap;
            Gizmos.DrawSphere(DebugPosition, 0.05f);
        }
    }
}