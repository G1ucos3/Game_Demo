using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchRoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TMP_InputField Input_RoomId;
    [SerializeField] private Button Btn_Join;
    void Start()
    {
        Btn_Join.onClick.AddListener(HandleClickedBtn_Join);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleClickedBtn_Join()
    {

    }
}
