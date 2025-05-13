using UnityEngine;

public class AttachableObject : MonoBehaviour
{
    public float attachDistance = 1.5f;
    public LayerMask playerLayer;
    public LayerMask cartLayer;

    public Transform frontPoint;
    public Transform backPoint;

    private bool isAttached = false;
    private HingeJoint2D joint;
    private Rigidbody2D rb;

    [SerializeField] private bool hasSomethingBehind = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (frontPoint == null)
        {
            GameObject front = new GameObject("FrontPoint");
            front.transform.SetParent(transform);
            front.transform.localPosition = new Vector3(0, 0.5f, 0);
            frontPoint = front.transform;
        }

        if (backPoint == null)
        {
            GameObject back = new GameObject("BackPoint");
            back.transform.SetParent(transform);
            back.transform.localPosition = new Vector3(0, -0.5f, 0);
            backPoint = back.transform;
        }
    }

    void Update()
    {
        if (isAttached && Input.GetKeyDown(KeyCode.Q))
        {
            Detach();
            return;
        }

        if (!isAttached && Input.GetKeyDown(KeyCode.E))
        {
            TryAttach();
        }
    }

    void TryAttach()
    {
        // Сначала проверяем игрока
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attachDistance, playerLayer);
        if (playerCollider != null)
        {
            AttachToPlayer(playerCollider.gameObject);
            return;
        }

        // Затем проверяем тележки
        Collider2D[] cartColliders = Physics2D.OverlapCircleAll(transform.position, attachDistance, cartLayer);
        foreach (Collider2D cartCollider in cartColliders)
        {
            // Пропускаем себя
            if (cartCollider.gameObject == gameObject) continue;

            AttachableObject otherCart = cartCollider.GetComponent<AttachableObject>();
            if (otherCart != null && otherCart.isAttached && !otherCart.hasSomethingBehind)
            {
                AttachToCart(cartCollider.gameObject);
                return;
            }
        }
    }

    void AttachToPlayer(GameObject player)
    {
        // Создаем шарнир
        joint = gameObject.AddComponent<HingeJoint2D>();
        joint.connectedBody = player.GetComponent<Rigidbody2D>();

        // Настраиваем точки соединения
        joint.anchor = frontPoint.localPosition;
        joint.connectedAnchor = new Vector2(0, -0.5f); // Задняя часть игрока

        // Стабилизируем соединение
        joint.useLimits = true;
        JointAngleLimits2D limits = new JointAngleLimits2D();
        limits.min = -5;
        limits.max = 5;
        joint.limits = limits;

        // Обновляем состояние
        isAttached = true;

        Debug.Log(gameObject.name + " прикреплен к игроку");
    }

    void AttachToCart(GameObject cart)
    {
        AttachableObject otherCart = cart.GetComponent<AttachableObject>();

        // Создаем шарнир
        joint = gameObject.AddComponent<HingeJoint2D>();
        joint.connectedBody = cart.GetComponent<Rigidbody2D>();

        // Настраиваем точки соединения
        joint.anchor = frontPoint.localPosition;
        joint.connectedAnchor = otherCart.backPoint.localPosition;

        // Стабилизируем соединение
        joint.useLimits = true;
        JointAngleLimits2D limits = new JointAngleLimits2D();
        limits.min = -5;
        limits.max = 5;
        joint.limits = limits;

        // Обновляем состояние
        isAttached = true;
        otherCart.hasSomethingBehind = true;

        // Позиционируем тележку
        PositionBehindCart(cart);

        Debug.Log(gameObject.name + " прикреплен к тележке " + cart.name);
    }

    void PositionBehindCart(GameObject cart)
    {
        // Получаем компоненты
        AttachableObject otherCart = cart.GetComponent<AttachableObject>();

        // Устанавливаем позицию и поворот
        transform.position = otherCart.backPoint.position;
        transform.rotation = cart.transform.rotation;

        // Смещаем слегка назад для предотвращения конфликтов
        transform.position -= transform.up * 0.1f;

        // Обнуляем скорости для предотвращения рывков
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    void Detach()
    {
        if (joint != null)
        {
            // Если мы прикреплены к тележке, обновляем её флаг
            GameObject connectedObject = joint.connectedBody.gameObject;
            AttachableObject connectedCart = connectedObject.GetComponent<AttachableObject>();
            if (connectedCart != null)
            {
                connectedCart.hasSomethingBehind = false;
            }

            // Уничтожаем шарнир
            Destroy(joint);
            joint = null;
        }

        // Обновляем состояние
        isAttached = false;

        Debug.Log(gameObject.name + " отсоединен");
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attachDistance);


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
}