using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;



public class NpcController : MonoBehaviour
{
    [SerializeField]
    Image dialog;

    [SerializeField]
    Image waiting;

    [SerializeField]
    Image getweapon;

    [SerializeField]
    TMP_InputField inputPrompt;

    [SerializeField]
    Button submit;

    [SerializeField]
    Button cancel;

    [SerializeField]
    private Sprite[] frames;

    private GameManager managerObj;
    private Coroutine animationCoroutine;
    private bool allowInteract = false;
    private bool isWaitingGetWeapon = false;
    private bool isWaitingGenWeapon = false;

    Player playerScript;

    public Action playerGetWeapon;

    void Awake()
    {
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player"); // tìm theo tên trong Hierarchy
        playerScript = playerObj.GetComponent<Player>();
        submit.onClick.AddListener(HandleSubmitBtnClicked);
        cancel.onClick.AddListener(HandleCancelBtnClicked);
        GameManager.Instance.alreadyWeapon += WattingPlayerGetWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.isPressed && allowInteract)
        {
            if (isWaitingGetWeapon)
            {
                isWaitingGetWeapon = false;
                playerGetWeapon?.Invoke();
                EndInteract();
            }
            else
            {
                dialog.gameObject.SetActive(false);
                inputPrompt.gameObject.SetActive(true);
                cancel.gameObject.SetActive(true);
                playerScript.setCanControl(false);
            }
        }

        if (inputPrompt.gameObject.activeSelf)
        {
            if (inputPrompt.text != "")
            {
                submit.gameObject.SetActive(true);
                if (Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    HandleSubmitBtnClicked();
                }
            }
            else
            {
                submit.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            allowInteract = true;
            // Add your interaction logic here
            if (isWaitingGetWeapon)
            {
                getweapon.gameObject.SetActive(true);
            }
            else if (isWaitingGenWeapon)
            {
                waiting.gameObject.SetActive(true);
            }
            else
            {
                dialog.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EndInteract();
        }
    }

    private void HandleCancelBtnClicked()
    {
        playerScript.setCanControl(true);
        EndInteract();
    }

    private void HandleSubmitBtnClicked()
    {
        playerScript.setCanControl(true);
        cancel.gameObject.SetActive(false);
        submit.gameObject.SetActive(false);
        inputPrompt.gameObject.SetActive(false);
        waiting.gameObject.SetActive(true);
        isWaitingGenWeapon = true;
        Gemini.Instance.ValidateContent(
            inputPrompt.text,
            (result) =>
            {
                Debug.Log($"✅ Weapon ID: {result.weaponID}, Reason: {result.reason}");
            },
            (error) =>
            {
                Debug.LogError($"❌ Lỗi khi gọi Gemini: {error}");
            }
        );
        animationCoroutine = StartCoroutine(PlayAnimation());
    }

    private void EndInteract()
    {
        dialog.gameObject.SetActive(false);
        inputPrompt.gameObject.SetActive(false);
        inputPrompt.text = "";
        submit.gameObject.SetActive(false);
        cancel.gameObject.SetActive(false);
        allowInteract = false;
        getweapon.gameObject.SetActive(false);
        waiting.gameObject.SetActive(false);
    }

    private void WattingPlayerGetWeapon()
    {
        Debug.Log("WattingPlayerGetWeapon");
  
        isWaitingGenWeapon = false;

        if (allowInteract)
        {
            waiting.gameObject.SetActive(false);
            getweapon.gameObject.SetActive(true);
        }

        isWaitingGetWeapon = true;
        StopCoroutine(animationCoroutine);
    }

    private IEnumerator PlayAnimation()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int currentFrame = 0;
        float frameRate = 0.1f; // Thời gian giữa các khung hình
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                timer -= frameRate;
                currentFrame = (currentFrame + 1) % frames.Length; // Vòng lặp qua các khung hình
                sr.sprite = frames[currentFrame];
            }
            yield return null;
        }
    }
}
