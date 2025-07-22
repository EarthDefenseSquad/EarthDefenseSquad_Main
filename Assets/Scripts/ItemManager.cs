using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public GameManager gameManager;
    public PlayerMove player;
    private Coroutine revealCoroutine;


    // private void Awake()
    // {
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject); // 중복 방지
    //         return;
    //     }

    //     Instance = this;
    //     DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
    // }

    private void Start()
    {
        InitializeItems();
        InitializePlayer();

        // 시작 시 HiddenPlatform 레이어 오브젝트 숨김
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("HiddenPlatform"))
            {
                var renderer = obj.GetComponent<Renderer>();
                var collider = obj.GetComponent<Collider2D>();

                if (renderer != null) renderer.enabled = false;
                if (collider != null) collider.enabled = false;
            }
        }
    }

    private void InitializeItems()
    {
        Debug.Log("✅ Items Initialized.");
    }

    private void InitializePlayer()
    {
        if (player != null && player.playerType == PlayerMove.PlayerType.Player1)
            player.maxSpeed = 5f;

        if (player != null)
            player.EnableInvincibility(false);

        Debug.Log("✅ Player Initialized.");
    }

    public void UseItem(ItemType itemType)
    {
        Debug.Log($"🧪 UseItem 호출됨: {itemType}");
        if (this == null) return;

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
                if (revealCoroutine != null)
                {
                    StopCoroutine(revealCoroutine);
                    Debug.Log("🔁 기존 RevealPlatform 효과 중단");
                }
                revealCoroutine = StartCoroutine(ActivateRevealPlatform());
                break;
            case ItemType.ColorRestore:
                StartCoroutine(ActivateColorRestore());
                break;

            default:
                Debug.LogWarning("❓ Unknown item type");
                break;
        }
    }

    private IEnumerator ActivateBufferingEffect()
    {
        if (player == null) yield break;

        Debug.Log("🐢 Buffering Effect Activated!");
        player.maxSpeed /= 2;
        yield return new WaitForSeconds(5f);
        player.maxSpeed *= 2;
        Debug.Log("⏩ Buffering Effect Ended");
    }

    private IEnumerator ActivateInvincibilityEffect()
    {
        if (player == null) yield break;

        Debug.Log("🛡️ Invincibility Activated!");
        player.EnableInvincibility(true);
        yield return new WaitForSeconds(5f);
        player.EnableInvincibility(false);
        Debug.Log("💥 Invincibility Ended");
    }

    private IEnumerator ActivateDoubleJumpEffect()
    {
        if (player == null) yield break;

        Debug.Log("🪂 Double Jump Activated!");
        player.EnableDoubleJump(5f);
        yield return null;
    }

    private void GrantAccessPass()
    {
        if (player == null) return;

        Debug.Log("🗝️ Access Pass Granted!");
        player.hasAccessPass = true;
    }

    private IEnumerator ActivateRevealPlatform()
    {
        Debug.Log("🔍 RevealPlatform 아이템 사용됨: 숨겨진 플랫폼 활성화 시도");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> hiddenPlatforms = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("HiddenPlatform"))
                hiddenPlatforms.Add(obj);
        }

        if (hiddenPlatforms.Count == 0)
        {
            Debug.LogWarning("❌ HiddenPlatform 레이어 오브젝트 없음");
        }
        else
        {
            Debug.Log($"✅ {hiddenPlatforms.Count}개 플랫폼 활성화");

            foreach (GameObject platform in hiddenPlatforms)
            {
                var renderer = platform.GetComponent<Renderer>();
                var collider = platform.GetComponent<Collider2D>();

                if (renderer != null) renderer.enabled = true;
                if (collider != null) collider.enabled = true;
            }

            yield return new WaitForSeconds(10f);

            foreach (GameObject platform in hiddenPlatforms)
            {
                var renderer = platform.GetComponent<Renderer>();
                var collider = platform.GetComponent<Collider2D>();

                if (renderer != null) renderer.enabled = false;
                if (collider != null) collider.enabled = false;
            }

            Debug.Log("🎬 플랫폼 다시 숨겨짐");
            revealCoroutine = null;
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
