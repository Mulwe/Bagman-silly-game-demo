using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    public PlayerInputActions InputActions { get; private set; }
    [SerializeField] private float _speed = 5f;

    private Vector2 _direction = Vector2.zero;
    private Vector2 _lastState;
    private Animator _animator;
    private Rigidbody2D _rb;

    private PauseManager _PauseManager;
    private EventBus _eventBus;
    private bool _isPaused;



    public void Run()
    {
        //логика после всех инициализаций
    }

    public void Initialize(EventBus eventBus)
    {
        _PauseManager = new PauseManager(eventBus);
        _eventBus = eventBus;
        EnableActions();
        _isPaused = false;

    }

    public void UpdateCurrentPlayerSpeed(float updatedSpeed)
    {
        if (updatedSpeed < 0)
            _speed = 0;
        else if (updatedSpeed > 50)
            _speed = 50;
        else
            _speed = updatedSpeed;
        _eventBus?.TriggerPlayerSpeedUpdateUI();
    }

    public float GetPlayerSpeed()
    {
        return _speed;
    }

    private void Awake()
    {
        InputActions = new PlayerInputActions();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void EnableActions()
    {
        if (InputActions != null)
        {
            InputActions.Player.Move.Enable();
            InputActions.UI.Menu.Enable();
            InputActions.Player.PickUp.Enable();
            InputActions.Player.DropOff.Enable();

            InputActions.UI.Menu.performed += OnEscapePressed;

        }
    }



    private void OnDisable()
    {
        if (InputActions != null)
        {
            InputActions.UI.Menu.performed -= OnEscapePressed;

            InputActions.UI.Menu.Disable();
            InputActions.Player.Move.Disable();
            InputActions.Player.PickUp.Disable();
            InputActions.Player.DropOff.Disable();
        }
    }

    private void OnDestroy()
    {
        _PauseManager?.RemoveListeners();
    }

    private void Update()
    {
        _direction = InputActions.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        ProccessInput();
        Animate();
        _rb.linearVelocity = new Vector2(_direction.x * _speed, _direction.y * _speed);
    }

    private void Animate()
    {
        Vector2 finalPos = _direction;
        if (finalPos.magnitude > 0.0f)
            _animator.SetBool("isMoving", true);
        else
        {
            _animator.SetBool("isMoving", false);
            finalPos = _lastState;
        }
        _animator.SetFloat("moveX", finalPos.x);
        _animator.SetFloat("moveY", finalPos.y);
    }

    private void ProccessInput()
    {
        if (_direction.x != 0 || _direction.y != 0)
        {
            if (_lastState.x == 0 && _lastState.y == 0)
                _lastState = _direction;
            else
                _lastState = _direction;
        }
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        //stop физики,таймера и т.д
        _isPaused = !_PauseManager.isGamePaused();
        _PauseManager.TogglePauseWithUI(_isPaused);
    }

    private void OnShiftHeld(InputAction.CallbackContext context)
    {
        Debug.Log("Shift key is being held down");
        _eventBus?.TriggerPlayerStaminaUpdateUI();
    }

}
