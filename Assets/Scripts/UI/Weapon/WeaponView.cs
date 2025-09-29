using Assets.Scripts.Domain;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private GameObject HitPrefab;
    [SerializeField] private Transform hitPos;
    [SerializeField] private GameObject hitEffectPrefab;

    private WeaponController weaponController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Bind(WeaponController weaponController)
    {
        this.weaponController = weaponController;
        this.weaponController.MeleeAttackEvent += MeleeAttack;
        this.weaponController.RangeAttackEvent += RangeAttack;
        this.weaponController.OnAttackEvent += OnHitEnemy;
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

    private void MeleeAttack(float currentAngle, float startAngle, float endAngle, float timeAttack, float speed)
    {
        StartCoroutine(OnMeleeAttack(currentAngle, startAngle, endAngle, speed));
        StartCoroutine(CooldownAttack(timeAttack));
    }

    private IEnumerator OnMeleeAttack(float currentAngle, float startAngle, float endAngle, float speed)
    {
        float step = (endAngle - startAngle) / speed;
        float angle = startAngle;
        float time = 0;
        Quaternion rotation;
        PolygonCollider2D polygon = gameObject.AddComponent<PolygonCollider2D>();
        while (time < speed)
        {
            angle += step * Time.deltaTime;
            rotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = rotation;
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(polygon);
        rotation = Quaternion.Euler(0, 0, currentAngle);
        transform.rotation = rotation;
    }

    private void RangeAttack(float angle, float force, float timeAttackRange, float speedRange, Sprite hitSprite)
    {
        GameObject hitTmp = Instantiate(HitPrefab, hitPos.position, Quaternion.identity);
        HitView hitView = hitTmp.GetComponent<HitView>();
        hitView.Binding(weaponController);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        hitTmp.transform.rotation = rotation;
        SpriteRenderer spriteHit = hitTmp.GetComponent<SpriteRenderer>();
        spriteHit.sprite = hitSprite;
        spriteHit.transform.localScale = new Vector3(-0.1f, 0.1f, 1);
        PolygonCollider2D polygon = hitTmp.AddComponent<PolygonCollider2D>();
        polygon.isTrigger = true;
        Rigidbody2D rigidbody = hitTmp.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(transform.right * force, ForceMode2D.Impulse);
        StartCoroutine(CooldownAttack(timeAttackRange));
    }

    private void OnHitEnemy(Vector2 position, float frameRate, Sprite[] effect)
    {
        if (effect == null || effect.Length == 0)
        {
            Debug.LogWarning("WeaponView.OnHitEnemy: effectSprites is null or empty, skip effect.");
            return;
        }
        if (hitEffectPrefab == null)
        {
            Debug.LogError("WeaponView.OnHitEnemy: hitEffectPrefab is null!");
            return;
        }
        var hitEffectTmp = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        if (hitEffectTmp == null)
        {
            Debug.LogError("WeaponView.OnHitEnemy: Instantiated hitEffectPrefab is null!");
            return;
        }
        HitEffectView hitEffectView = hitEffectTmp.GetComponent<HitEffectView>();
        if (hitEffectView == null)
        {
            Debug.LogError("WeaponView.OnHitEnemy: HitEffectView component is missing on hitEffectPrefab!");
            return;
        }
        hitEffectView.Play(frameRate, effect);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            ContactPoint2D cp = col.GetContact(0);
            Vector2 hitPos = cp.point;      // vị trí va chạm (world)
            weaponController.OnHitEnemy(hitPos);
        }
    }
}
