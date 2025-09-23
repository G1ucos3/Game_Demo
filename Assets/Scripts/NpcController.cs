using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private GameManager managerObj;

    private bool allowInteract = false;
    private bool isWaitingGetWeapon = false; 

    Player playerScript;

    public Action playerGetWeapon;

    void Awake()
    {
        managerObj = FindFirstObjectByType<GameManager>();
    }

    void OnEnable()
    {
        managerObj.alreadyWeapon += WattingPlayerGetWeapon;
    }

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player"); // tìm theo tên trong Hierarchy
        playerScript = playerObj.GetComponent<Player>();
        submit.onClick.AddListener(HandleSubmitBtnClicked);
        cancel.onClick.AddListener(HandleCancelBtnClicked);

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.isPressed)
        {
            if (isWaitingGetWeapon)
            {
                EndInteract();
                isWaitingGetWeapon = false;
                playerGetWeapon?.Invoke();
            }
            else if (allowInteract)
            {
                dialog.gameObject.SetActive(false);
                inputPrompt.gameObject.SetActive(true);
                cancel.gameObject.SetActive(true);
                allowInteract = false;
                playerScript.setCanControl(false);
            }
            
        }

        if (inputPrompt.gameObject.activeSelf)
        {
            if (inputPrompt.text != "")
            {
                submit.gameObject.SetActive(true);
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
            // Add your interaction logic here
            allowInteract = true;
            dialog.gameObject.SetActive(true);
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

        WattingPlayerGetWeapon();
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
    }

    private void WattingPlayerGetWeapon()
    {
        waiting.gameObject.SetActive(false);
        getweapon.gameObject.SetActive(true);
        isWaitingGetWeapon = true;
    }
}
