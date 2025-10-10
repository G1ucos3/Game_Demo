using Assets.Scripts.Domain;
using Fusion;
using Fusion.Addons.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rigidbodyPlayer;
    [SerializeField] private NetworkRigidbody2D networkRigidbody2D;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteWeaponRenderer;
    [SerializeField] private Transform hitPos;

    private PlayerController playerController;
    private WeaponController weaponController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trail.emitting = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Bind(PlayerController playerController, WeaponController weaponController)
    {
        this.playerController = playerController;
        this.playerController.OnMoving += HandleMoving;
        this.playerController.OnDashStart += HandleDashStart;
        this.playerController.OnDashing += HandleDashing;
        this.playerController.OnDashEnd += HandleDashEnd;
        this.playerController.OnRotate += HandleCharacterRotation;
        this.playerController.OnRotate += HandleWeaponRotation;
        this.playerController.OnUseWeapon += UseWeapon;

        this.weaponController = weaponController;
    }

    private void HandleMoving(Vector2 movingPosition, float moveSpeed)
    {
        animator.SetFloat("Speed", moveSpeed);
        rigidbodyPlayer.MovePosition(movingPosition);
        //transform.position = movingPosition;
    }

    private void HandleCharacterRotation(float angle)
    {
        // Chia góc thành 4 vùng để xác định trạng thái
        if (angle >= 45 && angle < 135)
        {
            // Hướng lên
            animator.SetFloat("DirectionX", 0);
            animator.SetFloat("DirectionY", 1);
        }
        else if (angle >= 135 || angle < -135)
        {
            // Hướng trái
            animator.SetFloat("DirectionX", -1);
            animator.SetFloat("DirectionY", 0);
        }
        else if (angle >= -135 && angle < -45)
        {
            // Hướng xuống
            animator.SetFloat("DirectionX", 0);
            animator.SetFloat("DirectionY", -1);
        }
        else
        {
            //Hướng phải
            animator.SetFloat("DirectionX", 1);
            animator.SetFloat("DirectionY", 0);
        }
    }

    private void HandleWeaponRotation(float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        spriteWeaponRenderer.transform.rotation = rotation;

        if (spriteWeaponRenderer.transform.eulerAngles.z >= -90 && spriteWeaponRenderer.transform.eulerAngles.z < 90)
        {
            spriteWeaponRenderer.transform.localScale = new Vector3(spriteWeaponRenderer.transform.localScale.x, spriteWeaponRenderer.transform.localScale.y > 0 ? spriteWeaponRenderer.transform.localScale.y : spriteWeaponRenderer.transform.localScale.y * -1, 0);
        }
        else
        {
            spriteWeaponRenderer.transform.localScale = new Vector3(spriteWeaponRenderer.transform.localScale.x, spriteWeaponRenderer.transform.localScale.y < 0 ? spriteWeaponRenderer.transform.localScale.y : -spriteWeaponRenderer.transform.localScale.y, 0);
        }
    }

    private void HandleDashing(Vector2 targetPosition, float dashTime)
    {
        StartCoroutine(LerpToTarget(targetPosition, dashTime));
    }

    private IEnumerator LerpToTarget(Vector2 target, float dashtime)
    {
        Vector2 start = rigidbodyPlayer.position;
        float elapsed = 0f;

        while (elapsed < dashtime)
        {
            rigidbodyPlayer.MovePosition(Vector2.Lerp(start, target, elapsed / dashtime));
            elapsed += Runner.DeltaTime;
            yield return null;
        }

        rigidbodyPlayer.MovePosition(target);
        playerController.EndDash();
    }

    private void HandleDashEnd()
    {
        trail.emitting = false;
    }

    private void HandleDashStart()
    {
        trail.emitting = true;
    }
    private void UseWeapon(WeaponObject weapon, int index)
    {
        //Render Weapon
        spriteWeaponRenderer.sprite = weapon.weaponSprite;
        // Lưu lại world position ban đầu của child
        Vector3 worldPos = hitPos.position;

        float desiredHeight = weapon.isMelee ? 0.25f : 0.5f;
        float spriteHeight = spriteWeaponRenderer.sprite.bounds.size.y;
        float scale = desiredHeight / spriteHeight;
        spriteWeaponRenderer.transform.localScale = new Vector3(-scale, scale, 1);

        // Cập nhật lại localPosition của child để world position không đổi
        hitPos.localPosition = spriteWeaponRenderer.transform.InverseTransformPoint(worldPos);
    }
}
