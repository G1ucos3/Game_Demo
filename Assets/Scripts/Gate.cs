using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] string scenename;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            StartCoroutine(GameManager.Instance.ChangeScene(scenename, 1f));
        }
    }
}
