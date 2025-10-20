using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isAttack = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Press space to attack
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && !isAttack)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttack = true;
        animator.SetBool("IsAttack", true);
        // here you can also play attack sound or spawn hitbox
    }

    // Animation Event at the END of Attack animation calls this
    public void EndAttack()
    {
        isAttack = false;
        animator.SetBool("IsAttack", false);
    }
}
