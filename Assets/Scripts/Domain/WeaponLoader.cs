using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.Rendering.DebugUI.Table;

public class WeaponLoader : MonoBehaviour
{
    public event Action<Sprite, int> OnLoadWeaponSlot;

    IEnumerator Start()
    {
        List<Coroutine> coroutines = new List<Coroutine>();

        for (int i = 0; i < TempData.currentWeaponsInUse.Length; i++)
        {
            if (TempData.currentWeaponsInUse[i] == null)
            {
                break;
            }

            TempData.weaponObjectsInUse[i] = new WeaponObject();
            TempData.weaponObjectsInUse[i].id = TempData.currentWeaponsInUse[i].id;
            TempData.weaponObjectsInUse[i].isMelee = TempData.currentWeaponsInUse[i].isMelee;

            // Thêm các Coroutine vào danh sách
            coroutines.Add(StartCoroutine(LoadSpriteWeapon(TempData.currentWeaponsInUse[i].imgeUrl, TempData.weaponObjectsInUse[i], i)));
            if (!TempData.weaponObjectsInUse[i].isMelee)
            {
                coroutines.Add(StartCoroutine(LoadSpriteHit(TempData.currentWeaponsInUse[i].hitUrl, TempData.weaponObjectsInUse[i])));
            }
            coroutines.Add(StartCoroutine(LoadSpriteEffect(TempData.currentWeaponsInUse[i].effectUrl, TempData.weaponObjectsInUse[i])));
        }


        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadSpriteWeapon(string imageUrl, WeaponObject weaponObject, int index)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                                 new Vector2(weaponObject.isMelee ? 1.1f : 0.8f, 0.5f));

                Debug.Log("Weapon null: " + weaponObject == null);

                weaponObject.weaponSprite = newSprite;

                OnLoadWeaponSlot?.Invoke(newSprite, index);
            }
            else
            {
                Debug.LogError("Lỗi tải ảnh: " + uwr.error);
            }
        }
    }


    private IEnumerator LoadSpriteHit(string imageUrl, WeaponObject weaponObject)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                                 new Vector2(0.7f, 0.5f));

                weaponObject.hitSprite = newSprite;
            }
            else
            {
                Debug.LogError("Lỗi tải ảnh: " + uwr.error);
            }
        }

    }

    private IEnumerator LoadSpriteEffect(string effectUrl, WeaponObject weaponObject)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(effectUrl))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Load fail: " + uwr.error);
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(uwr);

            Sprite[] frames = SliceSpriteSheet(tex, 3, 3);

            weaponObject.effectSprites = frames;
        }
    }

    private Sprite[] SliceSpriteSheet(Texture2D tex, int cols, int rows)
    {
        int w = tex.width / cols;
        int h = tex.height / rows;
        Sprite[] sprites = new Sprite[cols * rows];

        int index = 0;
        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < cols; x++)
            {
                Rect rect = new Rect(x * w, y * h, w, h);
                sprites[index++] = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), 100f);
            }
        }
        return sprites;
    }
}
