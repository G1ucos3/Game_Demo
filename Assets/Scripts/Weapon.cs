using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using static System.Net.WebRequestMethods;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject hitPrefab;

    [SerializeField]
    private Transform hitPos;

    [SerializeField]
    private GameObject hitEffectPrefab;

    private float TimeHit = 0.2f;
    private float hitForce = 20f;
    private float timeHit;
    private float totalAngleMelee = -1;
    private bool inAnimation = false;

    private bool isLeft;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inAnimation)
        {
            RotateWeapon();
        }

        if (WeaponObject.Instance != null)
        {
            if (WeaponObject.Instance.isMelee)
            {
                timeHit -= Time.deltaTime;
                if (timeHit < 0 && totalAngleMelee == -1 && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    inAnimation = true;
                    totalAngleMelee = 0;
                    Quaternion rotation;
                    if (isLeft)
                    {
                        rotation = Quaternion.Euler(0, 0, GetAngleMouseAndWeapon() - 75);

                    }
                    else
                    {
                        rotation = Quaternion.Euler(0, 0, GetAngleMouseAndWeapon() + 75);
                    }
                    
                    transform.rotation = rotation;
                    PolygonCollider2D polygon = gameObject.AddComponent<PolygonCollider2D>();
                    
                }
                else if (inAnimation)
                {
                    MeleeHit();
                }
            }
            else
            {
                timeHit -= Time.deltaTime;
                if (timeHit < 0 && WeaponObject.Instance != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    RangeHit();
                }
            }
        }
    }

    private void RotateWeapon()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, GetAngleMouseAndWeapon());
        transform.rotation = rotation;

        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y < 0 ? transform.localScale.y : -transform.localScale.y, 0);
            isLeft = true;
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y > 0 ? transform.localScale.y : transform.localScale.y * -1, 0);
            isLeft = false;
        }
    }



    private void RangeHit()
    {
        if (CurrentWeapon.Instance == null)
        {
            return;
        }

        GameObject hitTmp = Instantiate(hitPrefab, hitPos.position, Quaternion.identity);

        Quaternion rotation = Quaternion.Euler(0, 0, GetAngleMouseAndWeapon());
        hitTmp.transform.rotation = rotation;

        Rigidbody2D rigidbody = hitTmp.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(transform.right * hitForce, ForceMode2D.Impulse);
        timeHit = TimeHit;
    }

    private void MeleeHit()
    {
        if (totalAngleMelee <= 150)
        {
            float angle = Time.deltaTime * 150 / 0.3f;
            Quaternion rotation;
            if (isLeft)
            {
                rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle);
            }
            else
            {
                rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z - angle);
            }

            transform.rotation = rotation;
            totalAngleMelee += angle;
        }
        else
        {
            totalAngleMelee = -1;
            inAnimation = false;
            timeHit = 0.5f;
            PolygonCollider2D collider2D = GetComponent<PolygonCollider2D>();
            Destroy(collider2D);
        }
    }

    private float GetAngleMouseAndWeapon()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane)
        );

        Vector2 lookDir = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        return angle;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            ContactPoint2D cp = col.GetContact(0);
            Vector2 hitPos = cp.point;      // vị trí va chạm (world)

            Instantiate(hitEffectPrefab, hitPos, Quaternion.identity);
        }
    }
}
