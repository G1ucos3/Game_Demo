using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MatchRoom : MonoBehaviour
{
    [SerializeField] private TMP_InputField Input_RoomName;
    [SerializeField] private Button Btn_Join;
    [SerializeField] private NetworkRunner RunnerPrefab;

    private NetworkRunner runner;

    private void Start()
    {
        Btn_Join.onClick.AddListener(OnJoinClicked);
    }

    private async void OnJoinClicked()
    {
        string roomName = Input_RoomName.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("Room name cannot be empty!");
            return;
        }

        runner = Instantiate(RunnerPrefab);
        runner.ProvideInput = true;

        if (runner.GetComponent<NetworkSceneManagerDefault>() == null)
            runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        // Start shared session (Scene Matching vẫn ở đây)
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
        await Task.Delay(200);
        if (result.Ok)
        {
            Debug.Log("Joined/Created room: " + roomName);

            // Nếu host, load Scene1 cho tất cả client
            if (runner.IsServer)
            {
                Debug.Log("You are the host. Loading Scene1...");
                 // Scene1 phải nằm trong Build Settings
            }
            else
            {
                Debug.Log("You are a client. Waiting for the host to load the scene...");
            }
            await runner.LoadScene("Scene1");
        }
        else
        {
            Debug.LogError($"Failed to join/create room: {result.ShutdownReason}");
        }
    }
}
