using UnityEngine;
using UnityEngine.Tilemaps;

public class RestoreAreaTrigger : MonoBehaviour
{
    private bool isRestored = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isRestored) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        if (!ItemManager.Instance || !ItemManager.Instance.IsColorRestoreActive()) return;

        bool restored = false;

        // 1. SpriteRenderer가 있는 경우
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && ApproximatelyColor(sr.color, new Color(0.27f, 0.27f, 0.27f)))
        {
            sr.color = Color.white;
            restored = true;
        }

        // 2. Tilemap이 있는 경우
        Tilemap tilemap = GetComponent<Tilemap>();
        if (tilemap != null && ApproximatelyColor(tilemap.color, new Color(0.27f, 0.27f, 0.27f)))
        {
            tilemap.color = Color.white;
            restored = true;
        }

        if (restored)
        {
            isRestored = true;
            Debug.Log($"🎨 복원됨: {gameObject.name}");
            ColorRestoreManager.Instance.CheckRestoreStatus();
        }
    }

    private bool ApproximatelyColor(Color a, Color b, float threshold = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
}
