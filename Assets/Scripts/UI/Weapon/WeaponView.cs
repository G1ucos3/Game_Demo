using Assets.Scripts.Domain;
using Fusion;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponView : NetworkBehaviour
{
    [SerializeField] private GameObject HitPrefab;
    [SerializeField] private Transform hitPos;
    [SerializeField] private GameObject hitEffectPrefab;

    private WeaponController weaponController;
    private int weaponIndex;

    public void Bind(WeaponController weaponController)
    {
        this.weaponController = weaponController;
        this.weaponController.MeleeAttackEvent += MeleeAttack;
        this.weaponController.RangeAttackEvent += RangeAttack;
        this.weaponController.OnAttackEvent += RPC_OnHitEnemy;
    }

    private IEnumerator CooldownAttack(float cooldown)
    {
        float time = 0;
        while (time < cooldown)
        {
            time += Time.deltaTime;
            yield return null;
        }

        weaponController.ResetAttack();
    }

    private void MeleeAttack(int weaponIndex, float currentAngle, float startAngle, float endAngle, float timeAttack, float speed)
    {
        this.weaponIndex = weaponIndex;
        Rpc_MeleeAttack(currentAngle, startAngle, endAngle, speed);
        StartCoroutine(CooldownAttack(timeAttack));
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void Rpc_MeleeAttack(float currentAngle, float startAngle, float endAngle, float speed, RpcInfo info = default)
    {
        PlayerRef sender = info.Source;
        NetworkObject playerNetObject = Runner.GetPlayerObject(sender);

        if (playerNetObject == null)
        {
            Debug.LogWarning($"[RPC Server] GetPlayerObject returned NULL for sender {sender}. The association was never made or was lost.");
            return;
        }

        // Nếu tìm thấy, lấy GameObject
        GameObject playerObject = playerNetObject.gameObject;

        GameObject weaponObject = playerObject.transform.Find("Weapon").gameObject;
        StartCoroutine(OnMeleeAttack(currentAngle, startAngle, endAngle, speed, weaponObject));
    }
    private IEnumerator OnMeleeAttack(float currentAngle, float startAngle, float endAngle, float speed, GameObject weaponObject)
    {
        float step = (endAngle - startAngle) / speed;
        float angle = startAngle;
        float time = 0;
        Quaternion rotation;
        
        //chỉ nên để phía host
        PolygonCollider2D polygon = weaponObject.AddComponent<PolygonCollider2D>();

        while (time < speed)
        {
            angle += step * Runner.DeltaTime;
            rotation = Quaternion.Euler(0, 0, angle);
            weaponObject.transform.rotation = rotation;
            time += Runner.DeltaTime;
            yield return null;
        }
        Destroy(polygon);
        rotation = Quaternion.Euler(0, 0, currentAngle);
        weaponObject.transform.rotation = rotation;
    }

    private void RangeAttack(int weaponIndex, float angle, float force, float timeAttackRange, float speedRange, Sprite hitSprite)
    {
        Rpc_SpawnHit(weaponIndex, angle);
        StartCoroutine(CooldownAttack(timeAttackRange));
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void Rpc_SpawnHit(int index, float angle, RpcInfo info = default)
    {
        PlayerRef sender = info.Source;
        NetworkObject playerNetObject = Runner.GetPlayerObject(sender);

        if (playerNetObject == null)
        {
            Debug.LogWarning($"[RPC Server] GetPlayerObject returned NULL for sender {sender}. The association was never made or was lost.");
            return;
        }

        // Nếu tìm thấy, lấy GameObject
        GameObject playerObject = playerNetObject.gameObject;
        Transform weaponTransform = playerObject.transform.Find("Weapon");
        Transform hitPos = weaponTransform.Find("HitPos");

        Runner.Spawn(
            HitPrefab, 
            hitPos.position, 
            Quaternion.Euler(0, 0, angle), 
            info.Source,
            (runner, newObject) =>
            {
                newObject.GetComponent<HitView>().SetWeaponIndex(index);
                newObject.GetComponent<HitView>().Binding(weaponController);
            });
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_OnHitEnemy(Vector2 position, float frameRate, int weaponIndex)
    {
        var hitEffectTmp = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        HitEffectView hitEffectView = hitEffectTmp.GetComponent<HitEffectView>();
        Sprite[] effect = TempData.weaponObjectsInUse[weaponIndex].effectSprites;
        hitEffectView.Play(frameRate, effect);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (col.gameObject.tag == "Enemy")
        {
            ContactPoint2D cp = col.GetContact(0);
            Vector2 hitPos = cp.point;      // vị trí va chạm (world)
            weaponController.OnHitEnemy(weaponIndex, hitPos);
        }
    }
}
