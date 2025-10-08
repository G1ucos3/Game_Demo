using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInit : MonoBehaviour
{

    public Action onLoadComplete;
    public bool isLoaded = false;
    IEnumerator Start()
    {
        List<IEnumerator> coroutineWaiters = new List<IEnumerator>();

        for (int i = 0; i < TempData.currentWeaponsInUse.Length; i++)
        {
            if (TempData.currentWeaponsInUse[i] == null)
            {
                break;
            }

            TempData.weaponObjectsInUse[i] = new WeaponObject();
            TempData.weaponObjectsInUse[i].id = TempData.currentWeaponsInUse[i].id;
            TempData.weaponObjectsInUse[i].isMelee = TempData.currentWeaponsInUse[i].isMelee;

            coroutineWaiters.Add(WeaponLoader.LoadSpriteWeapon(TempData.currentWeaponsInUse[i].imgeUrl, TempData.weaponObjectsInUse[i], i));

            if (!TempData.weaponObjectsInUse[i].isMelee)
            {
                coroutineWaiters.Add(WeaponLoader.LoadSpriteHit(TempData.currentWeaponsInUse[i].hitUrl, TempData.weaponObjectsInUse[i]));
            }
            coroutineWaiters.Add(WeaponLoader.LoadSpriteEffect(TempData.currentWeaponsInUse[i].effectUrl, TempData.weaponObjectsInUse[i]));
        }

        foreach (IEnumerator waiter in coroutineWaiters)
        {
            yield return StartCoroutine(waiter);
        }
        Debug.Log("All weapons loaded");
        onLoadComplete?.Invoke();
        isLoaded = true;
    }
}