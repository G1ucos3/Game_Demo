using System;
using UnityEngine;

public class Hit : MonoBehaviour
{
    private float timeExisted = 2f;

    [SerializeField]
    private GameObject hitEffectPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
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

    [Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            //stop hit
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;

            // Lấy vị trí va chạm
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
