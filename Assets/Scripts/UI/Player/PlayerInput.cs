using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public struct PlayerNetworkInputMove : INetworkInput
{
    public Vector2 MousePosition;
    public Vector2 MoveDir;
    public Vector2 CurrentPosition;
}

public struct PlayerNetworkInputChangeWeapon : INetworkInput
{
    public int WeaponIndex;
}

public class PlayerInput : NetworkBehaviour, INetworkRunnerCallbacks
{
    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2, Vector2, Vector2?> OnDashPressed;
    public event Action<Vector2, Vector2> OnRotate;
    public event Action<int> OnChangeWeapon;
    public event Action<int, Vector2, Vector2> OnAttack;

    private InputActions inputActions;
    private Vector2 moveInput;

    private int currentWeaponIndex;

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
        inputActions.Player.ChangeWeapon.performed += ChangeWeapon;
        inputActions.Player.Attack.performed += OnAttackEvent;
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

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Called on the local owner to submit input to the runner
        if (!HasInputAuthority) return;

        input.Set(new PlayerNetworkInputMove
        {
            MoveDir = moveInput,
            MousePosition = GetMousePos(),
            CurrentPosition = transform.position
        });
    }


    // PlayerInput.cs - FixedUpdateNetwork() - MỚI
    public override void FixedUpdateNetwork()
    {
        // Cả Host và Client có quyền điều khiển đều chạy logic bên trong
        if (GetInput(out PlayerNetworkInputMove input))
        {
            // 1. Logic di chuyển được chạy cho cả hai
            OnMoveInput?.Invoke(input.MoveDir);
            OnRotate?.Invoke(input.MousePosition, input.CurrentPosition);
        }
    }

    // Update is called once per frame
    public override void Render()
    {
        if (HasInputAuthority == false)
        {
            return;
        }

    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnAttackEvent(InputAction.CallbackContext ctx)
    {
        OnAttack?.Invoke(currentWeaponIndex, GetMousePos(), transform.position);
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

    private void ChangeWeapon(InputAction.CallbackContext ctx)
    {
        if (HasInputAuthority == false)
        {
            return;
        }
        var key = ctx.control.name; // ví dụ "1", "2", "3"

        switch (key)
        {
            case "1":
                currentWeaponIndex = 0;
                OnChangeWeapon?.Invoke(0);
                break;
            case "2":
                currentWeaponIndex = 1;
                OnChangeWeapon?.Invoke(1);
                break;
            case "3":
                currentWeaponIndex = 2;
                OnChangeWeapon?.Invoke(2);
                break;
        }
    }

    private Vector2 GetMousePos()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        return (Vector2)mouseWorldPos;
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