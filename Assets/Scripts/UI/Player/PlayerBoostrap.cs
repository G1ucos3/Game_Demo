using Assets.Scripts.Domain;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoostrap : NetworkBehaviour
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerView view;
    [SerializeField] private WeaponView weaponView;


    private PlayerController playerController;
    private WeaponController weaponController;
    private PlayerUICanvas playerUICanvas;
    private WeaponInit weaponInit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerUICanvas = FindFirstObjectByType<PlayerUICanvas>();
        weaponInit = FindFirstObjectByType<WeaponInit>();
    }

    void Start()
    {
        
        playerController = new PlayerController();
        weaponController = new WeaponController();

        view.Bind(playerController, weaponController);
        weaponView.Bind(weaponController);
        if (HasInputAuthority)
        {
            playerUICanvas.Bind(playerController, weaponInit);
        }

        input.OnMoveInput += playerController.HandleMove;
        input.OnDashPressed += playerController.HandleDash;
        input.OnRotate += playerController.HandleCharacterRotation;
        input.OnChangeWeapon += playerController.HandleChangeWeapon;
        input.OnAttack += weaponController.Attack;
    }
}
