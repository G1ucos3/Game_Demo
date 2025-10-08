using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponLoader : MonoBehaviour 
{
    public static IEnumerator LoadSpriteWeapon(string imageUrl, WeaponObject weaponObject, int index)
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
            }
            else
            {
                Debug.LogError("Lỗi tải ảnh: " + uwr.error);
            }
        }
    }


    public static IEnumerator LoadSpriteHit(string imageUrl, WeaponObject weaponObject)
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

    public static IEnumerator LoadSpriteEffect(string effectUrl, WeaponObject weaponObject)
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

    private static Sprite[] SliceSpriteSheet(Texture2D tex, int cols, int rows)
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
