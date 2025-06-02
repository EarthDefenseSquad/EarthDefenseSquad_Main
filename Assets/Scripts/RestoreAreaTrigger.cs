using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RestoreAreaTrigger : MonoBehaviour
{
    private bool isRestored = false;
    private Tilemap tilemap;

    private float checkRadius = 0.5f; // 플레이어와 얼마나 가까워야 복원되는지

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("❌ RestoreAreaTrigger에 Tilemap이 없습니다!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (isRestored) return;
        if (!ItemManager.Instance || !ItemManager.Instance.IsColorRestoreActive()) return;

        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // 거리 검사
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= checkRadius)
        {
            TryRestore();
        }
    }

    private void TryRestore()
    {
        if (isRestored) return;

        Color dimColor = new Color(0.27f, 0.27f, 0.27f);
        if (ApproximatelyColor(tilemap.color, dimColor))
        {
            tilemap.color = Color.white;
            isRestored = true;
            Debug.Log($"🎨 복원됨: {gameObject.name}");

            // GoalForColorRestore가 자동으로 복원 완료 상태 체크
        }
    }

    private bool ApproximatelyColor(Color a, Color b, float threshold = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
}
