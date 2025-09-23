using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    //[SerializeField]
    //private Button showBtn;

    public Action alreadyWeapon;
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject Player;

    private SpriteRenderer spriteWeaponRenderer;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform hitPos;
    [SerializeField] private List<Image> weaponList;
    [SerializeField] List<GameObject> weaponBG;
    [SerializeField] Animator animator;

    [SerializeField]
    private GameObject weapon;

    [SerializeField]
    private GameObject hitPrefab;

    int columns = 3; // số cột frame trong sprite sheet
    int rows = 3;    // số hàng frame
    private Sprite[] frames;

    int numWeapons = 1;

    // Data vừa load từ database về
    private CurrentWeapon[] currentWeapons = new CurrentWeapon[3];

    // Data sau khi load sprite về
    private WeaponObject[] weaponObjects = new WeaponObject[3];

    private WeaponObject newWeaponGen;

    private NpcController npcController;

    public Action getWeapon;

    private void Awake()
    {
        

        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
        npcController = FindFirstObjectByType<NpcController>();
        spriteWeaponRenderer = weapon.GetComponent<SpriteRenderer>();

        currentWeapons[0] = new CurrentWeapon(39, "Cung Hỏa thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758538354/9f94361c-301a-4070-98ee-3c237de0cefb.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758538507/download_5_bai5s3.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false);

        for (int i = 0; i < weaponObjects.Length; i++)
        {
            weaponObjects[i] = new WeaponObject();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        Gemini.Instance.alreadyChoseWeapon += HandleAlreadyChoseWeapon;
        npcController.playerGetWeapon += PlayerGetWeapon;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player.transform.localPosition = spawnPoint.position;
        Player.SetActive(false);
        weaponObjects = new WeaponObject[currentWeapons.Length];
        //showBtn.onClick.AddListener(HandleShowBtnClicked);

        StartCoroutine(LoadAllSpritesAndCallWeapon());
    }

    private IEnumerator LoadAllSpritesAndCallWeapon()
    {
        // Tạo danh sách để lưu trữ các Coroutine
        List<Coroutine> coroutines = new List<Coroutine>();

        // Khởi tạo weaponObjects và bắt đầu các Coroutine
        for (int i = 0; i < 1; i++)
        {
            weaponObjects[i] = new WeaponObject();
            weaponObjects[i].id = currentWeapons[i].id;
            weaponObjects[i].isMelee = currentWeapons[i].isMelee;
            weaponList[i].color = new Color(weaponList[i].color.r, weaponList[i].color.g, weaponList[i].color.b, 0.5f);
            weaponBG[i].SetActive(false);

            // Thêm các Coroutine vào danh sách
            coroutines.Add(StartCoroutine(LoadSpriteWeapon(currentWeapons[i].imgeUrl, weaponObjects[i], weaponList[i], true)));
            if (!weaponObjects[i].isMelee)
            {
                coroutines.Add(StartCoroutine(LoadSpriteHit(currentWeapons[i].hitUrl, weaponObjects[i])));
            }
            coroutines.Add(StartCoroutine(LoadSpriteEffect(currentWeapons[i].effectUrl, weaponObjects[i])));
        }

        //// Chờ tất cả Coroutine hoàn thành
        //foreach (Coroutine coroutine in coroutines)
        //{
        //    yield return coroutine;
        //}

        // Gọi CallWeapon(0) sau khi tất cả Coroutine hoàn thành
        Player.SetActive(true);
        yield break;
    }

    public void CallWeapon(int slot)
    {
        if (weaponObjects[slot] == null)
        {
            return;
        }

        WeaponObject.Instance = weaponObjects[slot];
        Debug.Log("Call weapon: " + WeaponObject.Instance.id);

        for (int i = 0; i < weaponObjects.Length; i++)
        {
            weaponList[i].color = new Color(weaponList[i].color.r, weaponList[i].color.g, weaponList[i].color.b, 0.5f);
            weaponBG[i].SetActive(false);
        }
        weaponList[slot].color = new Color(weaponList[slot].color.r, weaponList[slot].color.g, weaponList[slot].color.b, 1f);
        weaponBG[slot].SetActive(true);


        //Render Weapon
        spriteWeaponRenderer.sprite = WeaponObject.Instance.weaponSprite;
        // Lưu lại world position ban đầu của child
        Vector3 worldPos = hitPos.position;

        float desiredHeight = WeaponObject.Instance.isMelee ? 0.25f : 0.5f;
        float spriteHeight = spriteWeaponRenderer.sprite.bounds.size.y;
        float scale = desiredHeight / spriteHeight;
        spriteWeaponRenderer.transform.localScale = new Vector3(-scale, scale, 1);

        // Cập nhật lại localPosition của child để world position không đổi
        hitPos.localPosition = spriteWeaponRenderer.transform.InverseTransformPoint(worldPos);

        //Render RangeHit
        SpriteRenderer spriteHit = hitPrefab.GetComponent<SpriteRenderer>();
        spriteHit.sprite = WeaponObject.Instance.hitSprite;
        //spriteHeight = spriteHit.bounds.size.y;
        //scale = desiredHeight / spriteHeight;
        spriteHit.transform.localScale = new Vector3(-0.1f, 0.1f, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator LoadSpriteWeapon(string imageUrl, WeaponObject weaponObject, Image wepImg, bool isTaken)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                                 new Vector2(weaponObject.isMelee?1.1f:0.8f, 0.5f));

                Debug.Log("Weapon null: " + weaponObject == null);

                weaponObject.weaponSprite = newSprite;


                if (isTaken)
                {
                    wepImg.sprite = newSprite;
                }
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

            frames = SliceSpriteSheet(tex, columns, rows);

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

    public IEnumerator ChangeScene(string sceneName, float delayTime)
    {
        animator.SetTrigger("End");
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadSceneAsync(sceneName);
        animator.SetTrigger("Start");
    }

    private void HandleAlreadyChoseWeapon()
    {
        CurrentWeapon temp = Gemini.Instance.CurrentWeapon;

        newWeaponGen = new WeaponObject();
        newWeaponGen.id = temp.id;
        newWeaponGen.isMelee = temp.isMelee;
        StartCoroutine(LoadSpriteWeapon(temp.imgeUrl, newWeaponGen, null, false));
        if (!newWeaponGen.isMelee)
        {
            StartCoroutine(LoadSpriteHit(temp.hitUrl, newWeaponGen));
        }
        StartCoroutine(LoadSpriteEffect(temp.effectUrl, newWeaponGen));
        alreadyWeapon?.Invoke();
    }

    private void PlayerGetWeapon()
    {
        int index = numWeapons;
        if (numWeapons == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                if (weaponObjects[i].id == WeaponObject.Instance.id)
                { 
                    index = i;
                    break;
                }
            }
        } 
        else
        {
            numWeapons++;
        }
        weaponObjects[index] = newWeaponGen;
        weaponList[index].sprite = newWeaponGen.weaponSprite;
        newWeaponGen = null;
        CallWeapon(index);
    }
}
