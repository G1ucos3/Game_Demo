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

    private NpcController npcController;

    private void Awake()
    {
        npcController = FindFirstObjectByType<NpcController>();

        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
        spriteWeaponRenderer = weapon.GetComponent<SpriteRenderer>();

        currentWeapons[0] = new CurrentWeapon(1, "Kiếm lửa thường",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758528470/0d04c180-3daa-4c62-8901-6bf5d0533964.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757481121/explose_fnf7rz.png", true);

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
            coroutines.Add(StartCoroutine(LoadSpriteHit(currentWeapons[i].hitUrl, weaponObjects[i])));
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
        if (currentWeapons[slot] == null)
        {
            return;
        }

        WeaponObject.Instance = weaponObjects[slot];

        for (int i = 0; i < currentWeapons.Length; i++)
        {
            weaponList[i].color = new Color(weaponList[i].color.r, weaponList[i].color.g, weaponList[i].color.b, 0.5f);
            weaponBG[i].SetActive(false);
        }
        weaponList[slot].color = new Color(weaponList[slot].color.r, weaponList[slot].color.g, weaponList[slot].color.b, 1f);
        weaponBG[slot].SetActive(true);


        //Render Weapon
        spriteWeaponRenderer.sprite = WeaponObject.Instance.weaponSprite;

        float desiredHeight = WeaponObject.Instance.isMelee ? 0.25f : 0.5f;
        float spriteHeight = spriteWeaponRenderer.sprite.bounds.size.y;
        float scale = desiredHeight / spriteHeight;
        spriteWeaponRenderer.transform.localScale = new Vector3(-scale, scale, 1);

        //Render RangeHit
        SpriteRenderer spriteHit = hitPrefab.GetComponent<SpriteRenderer>();
        spriteHit.sprite = WeaponObject.Instance.hitSprite;
        //spriteHeight = spriteHit.bounds.size.y;
        //scale = desiredHeight / spriteHeight;
        spriteHit.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
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
                                                 new Vector2(1.1f, 0.5f));

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
                                                 new Vector2(0.5f, 0.5f));

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
        int index = numWeapons;

        if (numWeapons == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                if (currentWeapons[i].id == WeaponObject.Instance.id)
                {
                    index = i;
                }
            }
        }
        else
        {
            numWeapons++;
        }
        currentWeapons[index] = Gemini.Instance.CurrentWeapon;

        Debug.Log("Current weapon: " + currentWeapons[index].name);
        Debug.Log("index: " + index);
        weaponObjects[index] = new WeaponObject();
        weaponObjects[index].id = currentWeapons[index].id;
        weaponObjects[index].isMelee = currentWeapons[index].isMelee;
        StartCoroutine(LoadSpriteWeapon(currentWeapons[index].imgeUrl, weaponObjects[index], weaponList[index], false));
        StartCoroutine(LoadSpriteHit(currentWeapons[index].hitUrl, weaponObjects[index]));
        StartCoroutine(LoadSpriteEffect(currentWeapons[index].effectUrl, weaponObjects[index]));

    }

    private void PlayerGetWeapon()
    {
        weaponList[numWeapons - 1].sprite = weaponObjects[numWeapons - 1].weaponSprite;
    }
}
