using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private InputActions inputActions;
    private Camera maincamera;
    private Vector2 moveInput;
    private float speed = 5f;
    private Rigidbody2D rb;

    private bool canDash = true;
    private bool isDashing;
    private float dashingDistance = 3f;
    private float dashingTime = 0.2f;
    private float dashingCoolDown = 1f;
    private TrailRenderer trail;

    private bool canControl;

    [SerializeField] private Image imageSkillCooldown;

    [SerializeField] private Animator animator;

    void Awake()
    {
        canControl = true;
        inputActions = new InputActions();
        maincamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
        imageSkillCooldown.fillAmount = 1.0f;
    }

    void OnEnable()
    {

        // Enable action map "Player"
        inputActions.Player.Enable();
        inputActions.Player.Dash.performed += OnDash;
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnCancelMove;
    }

    void OnDisable()
    {
        // Disable action map để tránh rò rỉ bộ nhớ
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        //if dashing, player can't move
        if (!isDashing && canControl)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }
    }

    private void OnCancelMove(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    public void OnDash(InputAction.CallbackContext ctx) 
    {
        if(canDash && canControl)
        {
            imageSkillCooldown.fillAmount = 0.0f;
            StartCoroutine(Dash());
            //Debug.Log("Cahoi");
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        trail.emitting = true;

        Vector2 dashDirection;
        if (moveInput == Vector2.zero)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = maincamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
            dashDirection = ((Vector2)mouseWorldPos - rb.position).normalized;
        }
        else
        {
            dashDirection = moveInput.normalized;
        }

        // Điểm bắt đầu và điểm kết thúc
        Vector2 start = rb.position;
        Vector2 target = start + dashDirection * dashingDistance;

        float elapsed = 0f;
        while (elapsed < dashingTime)
        {
            // Di chuyển mượt từ start -> target
            rb.MovePosition(Vector2.Lerp(start, target, elapsed / dashingTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(target); // đảm bảo kết thúc đúng vị trí
        trail.emitting = false;

        isDashing = false;
        elapsed = 0f;
        while (elapsed < 1.0f)
        {
            elapsed += Time.deltaTime;
            imageSkillCooldown.fillAmount = elapsed / dashingCoolDown;
            yield return null;
        }

        imageSkillCooldown.fillAmount = 1.0f;
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

    public void setCanControl(bool canControl)
    {
        this.canControl = canControl;
    }
}