using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2, Vector2, float> OnMoveInput;
    public event Action<Vector2, Vector2, Vector2?> OnDashPressed;
    public event Action<Vector2, Vector2> OnRotate;
    public event Action<int> OnChangeWeapon;
    public event Action<Vector2, Vector2> OnAttack;

    private InputActions inputActions;
    private Vector2 moveInput;

    void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        // Enable action map "Player" 
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnCancelMove;
        inputActions.Player.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        OnMoveInput?.Invoke(transform.position, moveInput, Time.fixedDeltaTime);
        OnRotate?.Invoke(GetMousePos(), transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            OnChangeWeapon?.Invoke(0);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            OnChangeWeapon?.Invoke(1);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            OnChangeWeapon?.Invoke(2);
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnAttack?.Invoke(GetMousePos(), transform.position);
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnCancelMove(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        OnDashPressed?.Invoke(transform.position, moveInput, GetMousePos());
    }

    private Vector2 GetMousePos()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        return (Vector2)mouseWorldPos;
    }
}
