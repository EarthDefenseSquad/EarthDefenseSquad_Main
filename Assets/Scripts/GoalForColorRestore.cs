using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GoalForColorRestore : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void BlinkGoal()
    {
        StartCoroutine(BlinkEffect());
    }

    IEnumerator BlinkEffect()
    {
        float interval = 0.2f;
        for (int i = 0; i < 5; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(interval);
            sr.enabled = true;
            yield return new WaitForSeconds(interval);
        }
        sr.enabled = true;
    }
}
