using Fusion;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController
{
    public event Action<Vector2, float> OnMoving;
    public event Action<Vector2, float> OnDashing;
    public event Action<float> OnRotate;
    public event Action<float> OnDashCooldown;   // Thông báo view update cooldown
    public event Action OnDashStart;
    public event Action OnDashEnd;
    public event Action<WeaponObject, int> OnUseWeapon;

    private bool canDash = true;
    private bool canMove = true;
    private bool isDashing  = false;
    private float dashDistance = 3f;
    private float dashTime = 0.5f;
    private float dashCooldown = 1f;
    private float moveSpeed = 5f;

    public bool CanDash => canDash;
    public bool IsDashing => isDashing;

    private Vector2 position;

    // Domain lắng nghe input qua event
    public void HandleMove(Vector2 dir)
    {
        if (canMove)
        {
            OnMoving?.Invoke(dir, moveSpeed);
        }
    }

    public void HandleCharacterRotation(Vector2 mousePos, Vector2 currentPos)
    {
        if (canMove)
        {
            //Tính góc hướng với con trỏ chuột
            Vector3 direction = (mousePos - currentPos).normalized;

            // Xác định hướng chính (trên, dưới, trái, phải)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            OnRotate?.Invoke(angle);
        }
    }

    public void HandleDash(Vector2 currentPosition, Vector2 moveInput, Vector2? mouseWorldPos = null)
    {
        if (!canDash || isDashing) return;

        Vector2 dashDirection;
        if (moveInput == Vector2.zero)
        {
            if (mouseWorldPos == null)
            {
                return;
            }
            dashDirection = ((Vector2)mouseWorldPos - currentPosition).normalized;
        }
        else
        {
            dashDirection = moveInput.normalized;
        }

        isDashing = true;
        canDash = false;
        canMove = false;
        OnDashStart?.Invoke();

        Vector2 target = currentPosition + dashDirection * dashDistance;

        OnDashing?.Invoke(target, dashTime);

        OnDashCooldown?.Invoke(dashCooldown);
    }

    public void EndDash()
    {
        canMove = true;
        OnDashEnd?.Invoke();
    }

    public void ResetDash()
    {
        canDash = true;
        isDashing = false;
    }

    public void HandleChangeWeapon(int weaponIndex)
    {
        if (TempData.weaponObjectsInUse[weaponIndex] == null)
        {
            return;
        }
        WeaponObject.Instance = TempData.weaponObjectsInUse[weaponIndex];
        OnUseWeapon?.Invoke(WeaponObject.Instance, weaponIndex);
    }
}
