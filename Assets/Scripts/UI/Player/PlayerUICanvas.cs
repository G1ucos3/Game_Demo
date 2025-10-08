using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUICanvas : MonoBehaviour
{
    [SerializeField] private List<Image> weaponSlots;
    [SerializeField] private List<GameObject> weaponSlotsBG;
    [SerializeField] private Image imageSkillCooldown;

    private PlayerController playerController;
    private WeaponInit weaponInit;

    public void LoadAllSlotWeapon()
    {
        Debug.Log("Loading all slot weapon");
        for (int i = 0; i < TempData.weaponObjectsInUse.Length; i++)
        {
            if (TempData.weaponObjectsInUse[i] == null)
            {
                continue;
            }
            ShowWeaponSLot(TempData.weaponObjectsInUse[i].weaponSprite, i);
        }
    }

    public void Bind(PlayerController playerController, WeaponInit weaponInit)
    {
        this.playerController = playerController;
        this.playerController.OnUseWeapon += UseWeapon;
        this.playerController.OnDashCooldown += HandleDashCooldown;

        this.weaponInit = weaponInit;
        this.weaponInit.onLoadComplete += LoadAllSlotWeapon;
        if (this.weaponInit.isLoaded)
        {
            LoadAllSlotWeapon();
        }
    }

    private void ShowWeaponSLot(Sprite sprite, int index)
    {
        weaponSlots[index].color = new Color(weaponSlots[index].color.r, weaponSlots[index].color.g, weaponSlots[index].color.b, 0.5f);
        weaponSlotsBG[index].SetActive(false);
        weaponSlots[index].sprite = sprite;
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
    }

    private void HandleDashCooldown(float dashCooldown)
    {
        StartCoroutine(CooldownUI(dashCooldown));
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
}
