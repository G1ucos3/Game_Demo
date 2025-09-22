using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDirection : MonoBehaviour
{
    private Animator animator;
    private Camera maincamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        maincamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Lấy vị trí con trỏ chuột trong không gian thế giới
        Vector2 mousePos =Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = maincamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));

        // Tính vector hướng từ nhân vật đến con trỏ chuột
        Vector3 direction = (mouseWorldPos - transform.position).normalized;

        // Xác định hướng chính (trên, dưới, trái, phải)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Chia góc thành 4 vùng để xác định trạng thái
        if (angle >= 45 &&  angle < 135)
        {
            // Hướng lên
            animator.SetFloat("DirectionX", 0);
            animator.SetFloat("DirectionY", 1);
        }
        else if (angle >= 135 || angle < -135)
        {
            // Hướng trái
            animator.SetFloat("DirectionX", -1);
            animator.SetFloat("DirectionY", 0);
        }
        else if (angle >= -135 && angle < -45)
        {
            // Hướng xuống
            animator.SetFloat("DirectionX", 0);
            animator.SetFloat("DirectionY", -1);
        }
        else
        {
            //Hướng phải
            animator.SetFloat("DirectionX", 1);
            animator.SetFloat("DirectionY", 0);
        }
    }
}
