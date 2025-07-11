using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ItemManager : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerMove player;

    private void Start()
    {
        InitializeItems();
        InitializePlayer(); GameObject[] hiddenPlatforms = GameObject.FindGameObjectsWithTag("HiddenPlatform");
        foreach (GameObject platform in hiddenPlatforms)
        {
            var renderer = platform.GetComponent<TilemapRenderer>();
            var collider = platform.GetComponent<TilemapCollider2D>();

            if (renderer != null) renderer.enabled = false;
            if (collider != null) collider.enabled = false;
        }
    }

    private void InitializeItems()
    {
        Debug.Log("Items Initialized.");
    }

    private void InitializePlayer()
    {
        if (player.playerType == PlayerMove.PlayerType.Player1)
            player.maxSpeed = 5f; // Player1만 기본값 고정
        player.EnableInvincibility(false);
        Debug.Log("Player Initialized.");
    }

    public void UseItem(ItemType itemType)
    {
        // if (player.playerType == PlayerMove.PlayerType.Player2 && itemType == ItemType.AccessPass)
        // {
        //     Debug.Log("Player 2 cannot use AccessPass.");
        //     return;
        // }

        switch (itemType)
        {
            case ItemType.BufferingIcon:
                StartCoroutine(ActivateBufferingEffect());
                break;
            case ItemType.Invincibility:
                StartCoroutine(ActivateInvincibilityEffect());
                break;
            case ItemType.DoubleJump:
                StartCoroutine(ActivateDoubleJumpEffect());
                break;
            case ItemType.AccessPass:
                GrantAccessPass();
                break;
            case ItemType.RevealPlatform:
                StartCoroutine(ActivateRevealPlatform());
                break;
            case ItemType.ColorRestore:
                StartCoroutine(ActivateColorRestore());
                break;
            default:
                Debug.Log("Unknown item type");
                break;
        }
    }

    private IEnumerator ActivateBufferingEffect()
    {
        Debug.Log("Buffering Effect Activated!");
        player.maxSpeed /= 2;
        yield return new WaitForSeconds(5f);
        player.maxSpeed *= 2;
        Debug.Log("Buffering Effect Ended");
    }

    private IEnumerator ActivateInvincibilityEffect()
    {
        Debug.Log("Invincibility Activated!");
        player.EnableInvincibility(true);
        yield return new WaitForSeconds(5f);
        player.EnableInvincibility(false);
        Debug.Log("Invincibility Ended");
    }

    private IEnumerator ActivateDoubleJumpEffect()
    {
        Debug.Log("Double Jump Activated!");
        player.EnableDoubleJump(5f);
        yield return null;
    }

    private void GrantAccessPass()
    {
        Debug.Log("Access Pass Granted!");
        player.hasAccessPass = true;
    }

    private IEnumerator ActivateRevealPlatform()
    {
        Debug.Log("🔍 RevealPlatform 아이템 사용됨: 숨겨진 플랫폼 활성화 시도");

        GameObject[] hiddenPlatforms = GameObject.FindGameObjectsWithTag("HiddenPlatform");

        if (hiddenPlatforms.Length == 0)
        {
            Debug.LogWarning("❌ 숨겨진 플랫폼을 찾을 수 없습니다! 태그 또는 활성화 상태 확인 필요");
        }
        else
        {
            Debug.Log($"✅ 숨겨진 플랫폼 {hiddenPlatforms.Length}개 발견됨");

            foreach (GameObject platform in hiddenPlatforms)
            {
                var renderer = platform.GetComponent<TilemapRenderer>();
                var collider = platform.GetComponent<TilemapCollider2D>();

                if (renderer != null)
                {
                    renderer.enabled = true;
                    Debug.Log($"🟢 {platform.name} - TilemapRenderer 활성화됨");
                }

                if (collider != null)
                {
                    collider.enabled = true;
                    Debug.Log($"🟢 {platform.name} - TilemapCollider2D 활성화됨");
                }
            }

            yield return new WaitForSeconds(5f);

            foreach (GameObject platform in hiddenPlatforms)
            {
                var renderer = platform.GetComponent<TilemapRenderer>();
                var collider = platform.GetComponent<TilemapCollider2D>();

                if (renderer != null)
                {
                    renderer.enabled = false;
                    Debug.Log($"🔴 {platform.name} - TilemapRenderer 비활성화됨");
                }

                if (collider != null)
                {
                    collider.enabled = false;
                    Debug.Log($"🔴 {platform.name} - TilemapCollider2D 비활성화됨");
                }
            }

            Debug.Log("🎬 숨겨진 플랫폼 다시 숨겨짐 완료");
        }
    }
    private IEnumerator ActivateColorRestore()
    {
        Debug.Log("Color Restore Started");
        player.EnableColorRestore(true);

        // Goal 오브젝트 숨기기
        if (gameManager.goalObject != null)
        {
            gameManager.goalObject.SetActive(false);
            Debug.Log("🚫 Goal 비활성화");
        }

        yield return null;
    }

}
