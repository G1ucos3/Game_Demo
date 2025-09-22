using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AnimationTest : MonoBehaviour
{
    float frameRate = 0.1f;

    private Sprite[] frames;

    private SpriteRenderer sr;

    private int currentFrame;
    private float timer;

    private void Start()
    {
        frames = WeaponObject.Instance.effectSprites;
        sr = GetComponent<SpriteRenderer>();
        currentFrame = 0;
        timer = 0f;
    }

    void Update()
    {
        if (frames == null || frames.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        // Nếu đã chạy hết frame thì không cập nhật nữa
        if (currentFrame >= frames.Length - 1)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame++;
            sr.sprite = frames[currentFrame];
        }
    }
}
