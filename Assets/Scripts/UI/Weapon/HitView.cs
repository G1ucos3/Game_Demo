using Assets.Scripts.Domain;
using UnityEngine;

public class HitView : MonoBehaviour
{
    private float timeExisted = 0.5f;
    private WeaponController weaponController;
    public void Binding(WeaponController weaponController)
    {
        this.weaponController = weaponController;
    }   

    void Update()
    {
        if (timeExisted > 0)
        {
            timeExisted -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            weaponController.OnHitEnemy(hitPoint);
            Destroy(gameObject);
        }
    }
}
