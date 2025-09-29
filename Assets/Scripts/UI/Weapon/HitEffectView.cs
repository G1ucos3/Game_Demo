using UnityEngine;

public class HitEffectView : MonoBehaviour
{
    float frameRate = 0.1f;

    private Sprite[] frames;

    [SerializeField] private SpriteRenderer sr;

    private int currentFrame;
    private float timer;

    public void Play(float frameRate, Sprite[] frames)
    {
        this.frameRate = frameRate;
        this.frames = frames;
        currentFrame = 0;
        timer = 0f;
    }

    void Update()
    {
        if (frames == null || frames.Length == 0)
        {
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
