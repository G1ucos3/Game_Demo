using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionLauncher : MonoBehaviour
{
    private NetworkRunner _runner;

    async void Start()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        DontDestroyOnLoad(gameObject);

        var sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        // Tạo phòng mà không load scene
        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,       // Host = server + client
            SessionName = "RoomABC",
            SceneManager = sceneManager      // Fusion quản lý scene
        });

        if (result.Ok)
            Debug.Log("✅ Tạo phòng thành công!");
        else
            Debug.LogError($"❌ Lỗi tạo phòng: {result.ShutdownReason}");
    }
}
