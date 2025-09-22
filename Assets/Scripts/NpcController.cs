using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class NpcController : MonoBehaviour
{
    [SerializeField]
    Image dialog;

    [SerializeField]
    TMP_InputField inputPrompt;

    [SerializeField]
    Button submit;

    [SerializeField]
    Button cancel;


    private bool allowInteract = false;

    Player playerScript;

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
            Debug.Log("Đang giữ phím E");
            if (allowInteract)
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
            Debug.Log("Player entered NPC trigger area.");
            // Add your interaction logic here
            allowInteract = true;
            dialog.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited NPC trigger area.");

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
        EndInteract();
    }

    private void EndInteract()
    {
        dialog.gameObject.SetActive(false);
        inputPrompt.gameObject.SetActive(false);
        inputPrompt.text = "";
        submit.gameObject.SetActive(false);
        cancel.gameObject.SetActive(false);
        allowInteract = false;
    }
}
