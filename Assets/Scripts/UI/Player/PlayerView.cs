using Assets.Scripts.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbodyPlayer;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Image imageSkillCooldown;
    [SerializeField] private Animator animator;
    [SerializeField] private List<Image> weaponSlots;
    [SerializeField] private List<GameObject> weaponSlotsBG;
    [SerializeField] private SpriteRenderer spriteWeaponRenderer;
    [SerializeField] private Transform hitPos;

    private PlayerController playerController;
    private WeaponLoader weaponLoader;
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

    public void Bind(PlayerController playerController, WeaponLoader weaponLoader, WeaponController weaponController)
    {
        this.playerController = playerController;
        this.playerController.OnMoving += HandleMoving;
        this.playerController.OnDashStart += HandleDashStart;
        this.playerController.OnDashing += HandleDashing;
        this.playerController.OnDashEnd += HandleDashEnd;
        this.playerController.OnDashCooldown += HandleDashCooldown;
        this.playerController.OnRotate += HandleCharacterRotation;
        this.playerController.OnRotate += HandleWeaponRotation;
        this.playerController.OnUseWeapon += UseWeapon;

        this.weaponLoader = weaponLoader;
        this.weaponLoader.OnLoadWeaponSlot += ShowWeaponSLot;

        this.weaponController = weaponController;
    }

    private void HandleMoving(Vector2 movingPosition, float moveSpeed)
    {
        animator.SetFloat("Speed", moveSpeed);
        rigidbodyPlayer.MovePosition(movingPosition);
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

    private void HandleDashCooldown(float dashCooldown)
    {
        StartCoroutine(CooldownUI(dashCooldown));
    }

    private IEnumerator LerpToTarget(Vector2 target, float dashtime)
    {
        Vector2 start = rigidbodyPlayer.position;
        float elapsed = 0f;
        float dashTime = 0.2f; // hoặc lấy từ domain

        while (elapsed < dashTime)
        {
            rigidbodyPlayer.MovePosition(Vector2.Lerp(start, target, elapsed / dashTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rigidbodyPlayer.MovePosition(target);
        playerController.EndDash();
    }

    private IEnumerator CooldownUI(float dashCooldown)
    {
        float elapsed = 0f;

        while (elapsed < dashCooldown)
        {
            elapsed += Time.deltaTime;
            imageSkillCooldown.fillAmount = elapsed / dashCooldown;
            yield return null;
        }

        playerController.ResetDash();
        imageSkillCooldown.fillAmount = 1f;
    }

    private void HandleDashEnd()
    {
        trail.emitting = false;
    }

    private void HandleDashStart()
    {
        trail.emitting = true;
    }

    private void ShowWeaponSLot(Sprite prite, int index)
    {
        weaponSlots[index].color = new Color(weaponSlots[index].color.r, weaponSlots[index].color.g, weaponSlots[index].color.b, 0.5f);
        weaponSlotsBG[index].SetActive(false);
        weaponSlots[index].sprite = prite;
    }

    private void UseWeapon(WeaponObject weapon, int index)
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            weaponSlots[i].color = new Color(weaponSlots[i].color.r, weaponSlots[i].color.g, weaponSlots[i].color.b, 0.5f);
            weaponSlotsBG[i].SetActive(false);
        }
        weaponSlots[index].color = new Color(weaponSlots[index].color.r, weaponSlots[index].color.g, weaponSlots[index].color.b, 1f);
        weaponSlotsBG[index].SetActive(true);

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
