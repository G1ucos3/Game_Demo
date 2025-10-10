using Assets.Scripts.Domain;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoostrap : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerView view;
    [SerializeField] private WeaponLoader weaponLoader;
    [SerializeField] private WeaponView weaponView;


    private PlayerController playerController;
    private WeaponController weaponController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
        playerController = new PlayerController();
        weaponController = new WeaponController();

        view.Bind(playerController, weaponLoader, weaponController);
        weaponView.Bind(weaponController);

        input.OnMoveInput += playerController.HandleMove;
        input.OnDashPressed += playerController.HandleDash;
        input.OnRotate += playerController.HandleCharacterRotation;
        input.OnChangeWeapon += playerController.HandleChangeWeapon;
        input.OnAttack += weaponController.Attack;
    }
}
