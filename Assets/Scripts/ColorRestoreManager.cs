using System.Collections;
using UnityEngine;

public class ColorRestoreManager : MonoBehaviour
{
    public static ColorRestoreManager Instance { get; private set; }

    [Header("스테이지 Goal 오브젝트 (Color Restore용)")]
    public GameObject goalObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CheckRestoreStatus()
    {
        GameObject[] restoreAreas = GameObject.FindGameObjectsWithTag("RestoreArea");
        int restoredCount = 0;

        foreach (GameObject obj in restoreAreas)
        {
            var sr = obj.GetComponent<SpriteRenderer>();
            var tilemap = obj.GetComponent<UnityEngine.Tilemaps.Tilemap>();

            if (sr != null && sr.color == Color.white) restoredCount++;
            else if (tilemap != null && tilemap.color == Color.white) restoredCount++;
        }

        if (restoredCount == restoreAreas.Length && restoreAreas.Length > 0)
        {
            Debug.Log("✅ 모든 RestoreArea 복원 완료 → Goal 등장");
            StartCoroutine(ShowGoalEffect());
        }
    }

    private IEnumerator ShowGoalEffect()
    {
        if (goalObject == null)
        {
            Debug.LogWarning("⚠️ Goal 오브젝트가 연결되어 있지 않습니다.");
            yield break;
        }

        goalObject.SetActive(true);

        SpriteRenderer sr = goalObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float blinkInterval = 0.2f;
            for (int i = 0; i < 4; i++)
            {
                sr.enabled = false;
                yield return new WaitForSeconds(blinkInterval);
                sr.enabled = true;
                yield return new WaitForSeconds(blinkInterval);
            }
        }
    }
}
