using Assets.Scripts.Domain;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class HitView : NetworkBehaviour
{
    private float timeExisted = 0.5f;
    private WeaponController weaponController;
    private float force = 15f;

    [Networked]
    public int WeaponIndex { get; set; }

    [Networked]
    private TickTimer LifeTimer { get; set; }

    public void Binding(WeaponController weaponController)
    {
        this.weaponController = weaponController;
    }   
    public void SetWeaponIndex(int index)
    {
        WeaponIndex = index;
        LifeTimer = TickTimer.CreateFromSeconds(Runner, timeExisted);
    }

    public override void Spawned()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = TempData.weaponObjectsInUse[WeaponIndex].hitSprite;
        PolygonCollider2D polygon = this.gameObject.AddComponent<PolygonCollider2D>();
        polygon.isTrigger = true;
        if (Object.HasStateAuthority)
        {
            //Debug.Log("Bay");
            //Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            //rigidbody.AddForce(transform.right * force, ForceMode2D.Impulse);
            NetworkRigidbody2D nrb = GetComponent<NetworkRigidbody2D>();
            nrb.Rigidbody.AddForce(transform.right * force, ForceMode2D.Impulse);

        }
    }

    public override void FixedUpdateNetwork()
    {
        if (LifeTimer.Expired(Runner))
        {
            // CHỈ Host/Server mới được Despawn
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object); // LUÔN dùng Runner.Despawn
            }
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Object.HasStateAuthority)
        {
            if (other.CompareTag("Enemy"))
            {
                // Xử lý sát thương (Nếu cần WeaponController)
                Vector2 hitPoint = other.ClosestPoint(transform.position);
                weaponController.OnHitEnemy(WeaponIndex ,hitPoint);

                Runner.Despawn(Object); // Host hủy vật thể
            }
        }
    }
}
