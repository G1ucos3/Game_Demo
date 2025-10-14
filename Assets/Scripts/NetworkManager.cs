using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    
    [SerializeField] private NetworkRunner _runner;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    public static Dictionary<PlayerRef, NetworkObject> _playerRefs = new Dictionary<PlayerRef, NetworkObject>();

    private bool isSelectedRole = false;

    async void StartGame(GameMode mode)
    {
        //_runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            // FIXED: Chuyển đổi buildIndex sang SceneRef
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        if (!isSelectedRole)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
                isSelectedRole = true;
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
                isSelectedRole = true;
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Chỉ chạy trên Server/Host
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3(0, 0, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            if (networkPlayerObject != null)
            {
                _playerRefs.Add(player, networkPlayerObject);

                runner.SetPlayerObject(player, networkPlayerObject);
            }
            else
            {
                // Log 4: Báo lỗi nếu spawn thất bại
                Debug.LogError($"[NetworkManager] Failed to spawn prefab for player: {player}.");
            }
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_playerRefs.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _playerRefs.Remove(player);
        }
    }

    // --- CÁC HÀM CALLBACKS KHÁC ĐƯỢC THÊM VÀO HOẶC SỬA LẠI CHO ĐÚNG SIGNATURE ---

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }

    // FIXED: Sửa lại signature của hàm
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    // FIXED: Sửa lại signature của hàm
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    // FIXED: Thêm hàm mới mà interface yêu cầu
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    // FIXED: Thêm 2 hàm mới mà interface yêu cầu (liên quan đến Area of Interest)
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}