using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private InputActions inputActions;
    private Vector2 moveInput;
    private float speed = 5f;
    private Rigidbody2D rb;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 15f;
    private float dashingTime = 0.2f;
    private float dashingCoolDown = 1f;

    [SerializeField] private Animator animator;
    void Awake()
    {
        inputActions = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Enable action map "Player"
        inputActions.Player.Enable();

        // Đăng ký lắng nghe Move
        OnMove();

        
    }

    void OnDisable()
    {
        // Disable action map để tránh rò rỉ bộ nhớ
        inputActions.Player.Disable();
    }

    private void OnMove()
    {
        if (!isDashing)
        {
            inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        }
    }

    public void OnDash(InputAction.CallbackContext ctx) 
    {
        if(ctx.started && canDash && moveInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }
        Debug.Log("Cahoi");
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        Vector2 dashDirection = moveInput.normalized;
        rb.linearVelocity = dashDirection * dashingPower;
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);

            float moveSpeed = moveInput.magnitude;
            animator.SetFloat("Speed", moveSpeed);
        }
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            GameManager.Instance.CallWeapon(0);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            GameManager.Instance.CallWeapon(1);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            GameManager.Instance.CallWeapon(2);
        }
    }
}