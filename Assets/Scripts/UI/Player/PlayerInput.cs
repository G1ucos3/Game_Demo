using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public struct PlayerNetworkInput : INetworkInput
{
    public Vector2 CurentPosition;
    public Vector2 MoveDir;
    public float Time;
}

public class PlayerInput : NetworkBehaviour, INetworkRunnerCallbacks
{
    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2, Vector2, Vector2?> OnDashPressed;
    public event Action<Vector2, Vector2> OnRotate;
    public event Action<int> OnChangeWeapon;
    public event Action<Vector2, Vector2> OnAttack;

    private InputActions inputActions;
    private Vector2 moveInput;

    void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnCancelMove;
        inputActions.Player.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        if (Runner != null)
        {
            Runner.AddCallbacks(this);
        }
    }

    // PlayerInput.cs - FixedUpdateNetwork() - MỚI
    public override void FixedUpdateNetwork()
    {
        // Cả Host và Client có quyền điều khiển đều chạy logic bên trong
        if (GetInput(out PlayerNetworkInput input))
        {
            // 1. Logic di chuyển được chạy cho cả hai
            OnMoveInput?.Invoke(input.MoveDir);
            OnRotate?.Invoke(GetMousePos(), transform.position);
        }
    }

    // Update is called once per frame
    public override void Render()
    {
        if (HasInputAuthority == false)
        {
            return;
        }
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            OnChangeWeapon?.Invoke(0);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            OnChangeWeapon?.Invoke(1);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            OnChangeWeapon?.Invoke(2);
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnAttack?.Invoke(GetMousePos(), transform.position);
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnCancelMove(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (HasInputAuthority == false)
        {
            return;
        }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        OnDashPressed?.Invoke(transform.position, moveInput, GetMousePos());
    }

    private Vector2 GetMousePos()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        return (Vector2)mouseWorldPos;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Called on the local owner to submit input to the runner
        if (!HasInputAuthority) return;

        input.Set(new PlayerNetworkInput
        {
            MoveDir = moveInput,
        });
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
    // Removed unused INetworkRunnerCallbacks methods to avoid runtime NotImplementedException.

}