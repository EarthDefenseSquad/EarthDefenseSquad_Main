using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRestoreManager : MonoBehaviour
{
    [Header("이 스테이지에서 사용할 Goal 오브젝트 (직접 할당)")]
    [SerializeField] private GameObject goalObject;

    private GameManager gameManager;

    private void Start()
    {
        // GameManager 찾기
        gameManager = FindObjectOfType<GameManager>();

        if (goalObject == null)
        {
            Debug.LogWarning("[ColorRestoreManager] goalObject가 비어있습니다.");
            return;
        }

        if (gameManager != null)
        {
            gameManager.goalObject = goalObject;
            Debug.Log($"[ColorRestoreManager] GameManager에 Goal 오브젝트 등록 완료: {goalObject.name}");
        }
        else
        {
            Debug.LogWarning("[ColorRestoreManager] GameManager를 찾을 수 없습니다.");
        }
    }
}

