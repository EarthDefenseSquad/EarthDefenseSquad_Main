using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    // =========================
    //        Constants
    // =========================
    private const string K_SelectedIndex = "selectedIndex";
    private const string K_PlayerPrefs_SelectedStage = "SelectedStageIndex";

    // =========================
    //        Singleton
    // =========================
    //public static GameManager Instance;

    // =========================
    //        Game State
    // =========================
    [Header("Game State")]
    public int totalPoint = 0;
    public int stagePoint = 0;
    public int stageIndex = -1;
    public int health = 3;
    public bool gameClear = false;

    [Header("Stage / Map Roots")]
    public GameObject[] Stages; // 각 스테이지 루트 오브젝트 (Inspector)

    [Header("UI - Core")]
    public Image[] UIhealth;           // 하트(체력) UI
    public Text UIPoint;               // 점수 UI (UnityEngine.UI.Text)
    public Text UIStage;               // STAGE n UI

    [Header("UI - Restart / Stage Select")]
    public GameObject RestartButton;   // 게임오버 시 노출
    public GameObject StageSelectPanel;
    public Button Button_StageBack, Button_Stage1, Button_Stage2;

    [Header("Finish / Goal")]
    public int finishItemCount = 0;    // 수집한 Finish 아이템 개수
    public int totalStages = 3;        // 총 필요 수
    public Text finishItemText;        // "n / total" UI
    public GameObject goalObject;      // 골 오브젝트(숨겨놨다가 등장)

    [Header("Player Spawn")]
    public Transform[] spawnPoints;        // 0=P1, 1=P2
    public string[] playerPrefabNames;     // selectedIndex → 프리팹 이름 매핑

    [Header("Etc Options")]
    public bool colorRestoreMode = false;  // 색 복원 모드
    public bool isInstantDeathMode = false;// 개발용 즉사 모드

    // =========================
    //   Runtime / Internals
    // =========================
    private HashSet<string> restoredObjectNames = new HashSet<string>();
    private int myViewID;

    // 마지막으로 스폰된 (내) 플레이어
    public GameObject playerObj;
    public PlayerMove player;

    

    // =========================
    //        Unity Hooks
    // =========================
    void Awake()
    {
        //Instance = this;

        // Photon: 씬 자동 동기화 사용 (방장만 LoadLevel 호출)
        PhotonNetwork.AutomaticallySyncScene = true;

        // 즉사 모드 → 시작 체력 1
        health = isInstantDeathMode ? 1 : 3;
        UpdateHealthUI();

        // 내 PhotonView 추적(있으면)
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            myViewID = pv.ViewID;
            Debug.Log($"[GameManager] PhotonView ID = {myViewID}");
        }
    }

    void Start()
    {
        // 스테이지 표시/활성
        stageIndex = PlayerPrefs.GetInt(K_PlayerPrefs_SelectedStage, 0);
        for (int i = 0; i < Stages.Length; i++)
            if (Stages[i] != null) Stages[i].SetActive(false);

        if (stageIndex >= 0 && stageIndex < Stages.Length && Stages[stageIndex] != null)
        {
            Stages[stageIndex].SetActive(true);
            if (UIStage != null) UIStage.text = "STAGE " + (stageIndex + 1);
        }

        // 씬 별 스폰
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "WaitingScene")
        {
            StartCoroutine(DelayedSpawn());   // CustomProps 도착 대기 후 스폰
        }
        else if (scene == "StageScene")
        {
            int idx = GetLocalPlayerIndex();
            int sel = GetSelectedIndexForLocal();
            SpawnPlayer(idx, sel);
        }

        // 스테이지 선택 버튼 와이어링
        WireStageSelectButtons();
        //UpdateFinishItemUI();
    }

    void Update()
    {
        // StageScene에서만 점수 UI 갱신(프로젝트 정책에 맞게)
        if (SceneManager.GetActiveScene().name == "StageScene" && UIPoint != null)
        {
            UIPoint.text = totalPoint.ToString();
        }
    }

    // =========================
    //       Spawn Flow
    // =========================
    private IEnumerator DelayedSpawn()
    {
        // 룸/로컬 준비까지 대기
        while (!PhotonNetwork.InRoom || PhotonNetwork.LocalPlayer == null)
            yield return null;

        // 선택값(CustomProperties) 들어올 때까지 잠깐 대기 (최대 2초)
        float t = 0f;
        int selectedIndex = -1;
        while (t < 2f)
        {
            selectedIndex = GetSelectedIndexForLocal();
            if (selectedIndex >= 0) break;
            t += Time.deltaTime;
            yield return null;
        }

        int localIdx = GetLocalPlayerIndex();
        Debug.Log($"[GameManager] DelayedSpawn → localIdx={localIdx}, selectedIndex={selectedIndex}");
        SpawnPlayer(localIdx, selectedIndex);
    }

    /// <summary>
    /// 2인 고정 게임이면 이 방식이 가장 안전.
    /// 방장=0, 클라=1.
    /// </summary>
    private int GetLocalPlayerIndex()
    {
        return PhotonNetwork.IsMasterClient ? 0 : 1;
    }

    /// <summary>
    /// 내 캐릭터 선택값을 우선 CustomProperties에서 읽고,
    /// 없으면 CharacterSelectionData 폴백.
    /// </summary>
    private int GetSelectedIndexForLocal()
    {
        var lp = PhotonNetwork.LocalPlayer;
        if (lp != null && lp.CustomProperties != null && lp.CustomProperties.ContainsKey(K_SelectedIndex))
        {
            object v = lp.CustomProperties[K_SelectedIndex];
            if (v is int iv) return iv;
            if (v != null && int.TryParse(v.ToString(), out var iv2)) return iv2;
        }

        int myIdx = GetLocalPlayerIndex();
        int fb = (myIdx == 0) ? CharacterSelectionData.player1SelectedIndex
                              : CharacterSelectionData.player2SelectedIndex;
        return fb;
    }

    /// <summary>
    /// 기존 시그니처 호환용 (선택값은 내부에서 가져옴)
    /// </summary>
    public void SpawnPlayer(int player_index)
    {
        int selectedIndex = (player_index == 0) ? CharacterSelectionData.player1SelectedIndex
                                                : CharacterSelectionData.player2SelectedIndex;
        SpawnPlayer(player_index, selectedIndex);
    }

    /// <summary>
    /// 실제 스폰 메서드(선택값 명시).
    /// </summary>
    public void SpawnPlayer(int player_index, int selectedIndex)
    {
        // 프리팹 이름 결정
        string prefabName = ResolvePrefabNameBySelectedIndex(selectedIndex, player_index);
        Debug.Log($"Selected prefabName: {prefabName} for player_index: {player_index}");

        // 스폰 위치
        Vector3 spawnPos;
        if (spawnPoints != null && spawnPoints.Length > player_index && spawnPoints[player_index] != null)
            spawnPos = spawnPoints[player_index].position;
        else
        {
            // 안전 폴백 좌표
            Vector3[] defaults = { new Vector3(-1f, -0.5f, 0f), new Vector3(0f, -0.5f, 0f) };
            spawnPos = defaults[Mathf.Clamp(player_index, 0, 1)];
        }

        // 네트워크 생성
        GameObject obj = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
        playerObj = obj;
        player = obj.GetComponent<PlayerMove>();

        // PlayerMove 내부 필드 이름이 프로젝트마다 다르므로, 디버그시 안전하게만 출력
        if (player != null)
        {
            Debug.Log($"[PlayerMove] Player{player_index + 1} 스폰 완료");
        }

        Debug.Log("플레이어 생성 완료");

        // 카메라 유효성(선택)
        if (Camera.main == null)
        {
            Debug.LogWarning("[GameManager] Main Camera가 없습니다. 카메라 세팅을 확인하세요.");
        }
    }

    /// <summary>
    /// 선택 인덱스 → 프리팹 이름 매핑
    /// </summary>
    private string ResolvePrefabNameBySelectedIndex(int selectedIndex, int playerIndex)
    {
        // Inspector로 매핑 관리하는 걸 권장
        if (playerPrefabNames != null &&
            selectedIndex >= 0 &&
            selectedIndex < playerPrefabNames.Length &&
            !string.IsNullOrEmpty(playerPrefabNames[selectedIndex]))
        {
            return playerPrefabNames[selectedIndex];
        }

        // 하드코딩 폴백 (프로젝트 네이밍에 맞게 필요시 수정)
        switch (selectedIndex)
        {
            case 0: return "Player";
            case 1: return "Player Z-2";
            case 2: return "Player X-1";
            case 3: return "Player X-2";
            default:
                return (playerPrefabNames != null && playerPrefabNames.Length > 0)
                    ? playerPrefabNames[0]
                    : "Player";
        }
    }

    // =========================
    //     Finish / Goal
    // =========================
    /*public void AddFinishItem(int add = 1)
    {
        finishItemCount += add;
        if (finishItemCount < 0) finishItemCount = 0;
        UpdateFinishItemUI();

        // 방장이라면 방에 동기화 (원하면 Buffered)
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_SyncFinishItem), RpcTarget.OthersBuffered, finishItemCount);
        }
    }

    [PunRPC]
    private void RPC_SyncFinishItem(int synced)
    {
        finishItemCount = synced;
        UpdateFinishItemUI();
    }

    private void UpdateFinishItemUI()
    {
        if (finishItemText != null)
        {
            finishItemText.text = $"{finishItemCount} / {totalStages}";
        }
    }*/

    /// <summary>
    /// 목표 도달(예: finishItemCount==totalStages)이면 호출
    /// </summary>
    public void OnReachGoal()
    {
        if (finishItemCount >= totalStages)
        {
            gameClear = true;
            Debug.Log("[GameManager] Stage Clear!");
            GoalAppearEffect();
        }
        else
        {
            Debug.Log("[GameManager] 아직 목표 조건이 부족합니다.");
        }
    }

    /// <summary>
    /// PlayerMove에서 호출하던 이름 그대로 유지
    /// 골(문/포탈 등)을 등장시키는 효과
    /// </summary>



    // public void GoalAppearEffect()
    // {
    //     if (goalObject != null)
    //     {
    //         if (!goalObject.activeSelf)
    //             goalObject.SetActive(true);

    //         // 필요하다면 간단한 이펙트/애니메이션 트리거 추가 가능
    //         Debug.Log("[GameManager] GoalAppearEffect executed");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("[GameManager] goalObject가 설정되지 않았습니다.");
    //     }
    // }
    
        public IEnumerator GoalAppearEffect()
    {
        if (goalObject != null)
        {
            if (!goalObject.activeSelf)
                goalObject.SetActive(true);

            // 필요하다면 간단한 이펙트/애니메이션 트리거 추가 가능
            Debug.Log("[GameManager] GoalAppearEffect executed");
        }
        else
        {
            Debug.LogWarning("[GameManager] goalObject가 설정되지 않았습니다.");
        }

        // IEnumerator는 반드시 반환이 필요하므로 종료 처리
        yield break;
    }


    // =========================
    //         Health
    // =========================
    public void HealthDown(int amount = 1)
    {
        if (health <= 0) return;
        health -= amount;
        if (health < 0) health = 0;
        UpdateHealthUI();

        if (health <= 0)
        {
            OnGameOver();
        }
    }

    private void UpdateHealthUI()
    {
        if (UIhealth == null) return;
        for (int i = 0; i < UIhealth.Length; i++)
        {
            bool on = (i < health);
            if (UIhealth[i] != null)
                UIhealth[i].color = on ? Color.white : new Color(1f, 1f, 1f, 0.2f);
        }
    }

    private void OnGameOver()
    {
        Debug.Log("[GameManager] Game Over");
        if (RestartButton != null) RestartButton.SetActive(true);
        // TODO: 필요하면 게임오버 UI/Logic 추가
    }

    public void OnClick_Restart()
    {
        string scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    // =========================
    //     Stage Select UI
    // =========================
    private void WireStageSelectButtons()
    {
        if (Button_StageBack != null)
        {
            Button_StageBack.onClick.RemoveAllListeners();
            Button_StageBack.onClick.AddListener(() =>
            {
                if (StageSelectPanel != null) StageSelectPanel.SetActive(false);
            });
        }

        if (Button_Stage1 != null)
        {
            Button_Stage1.onClick.RemoveAllListeners();
            Button_Stage1.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt(K_PlayerPrefs_SelectedStage, 0);
                SceneManager.LoadScene("StageScene");
            });
        }

        if (Button_Stage2 != null)
        {
            Button_Stage2.onClick.RemoveAllListeners();
            Button_Stage2.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt(K_PlayerPrefs_SelectedStage, 1);
                SceneManager.LoadScene("StageScene");
            });
        }
    }

    // =========================
    //   Color Restore(옵션)
    // =========================

    /// <summary>
    /// PlayerMove에서 호출할 수 있는 외부 API(이 이름 그대로 필요하다고 하셨음)
    /// 모드를 활성화하고, 네트워크 동기화가 필요하면 여기서 추가 RPC 호출
    /// </summary>
    public void EnableColorRestoreMode()
    {
        colorRestoreMode = true;
        Debug.Log("[GameManager] ColorRestoreMode 활성화");
    }

    /// <summary>
    /// PlayerMove에서 호출(이름 그대로). 특정 오브젝트의 색을 복원.
    /// 내부적으로 네트워크 전파도 수행.
    /// </summary>
    public void SyncColorRestoration(string objectName)
    {
        // 로컬 즉시 반영
        RestoreColorLocal(objectName);
        // 다른 클라이언트에도 반영
        photonView.RPC(nameof(RPC_SyncColorRestoration), RpcTarget.OthersBuffered, objectName);
    }

    private void RestoreColorLocal(string objectName)
    {
        var go = GameObject.Find(objectName);
        if (go == null) return;

        var sr = go.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = Color.white;

        // 복원 목록 관리 (선택)
        RegisterRestorable(objectName);
    }

    [PunRPC]
    private void RPC_SyncColorRestoration(string objectName)
    {
        RestoreColorLocal(objectName);
    }

    /// <summary>
    /// 색 복원 대상 등록
    /// </summary>
    public void RegisterRestorable(string objName)
    {
        if (string.IsNullOrEmpty(objName)) return;
        if (!restoredObjectNames.Contains(objName))
            restoredObjectNames.Add(objName);
    }

    /// <summary>
    /// 모드가 켜져 있으면 등록된 오브젝트 색을 복원
    /// </summary>
    public void RestoreColorsIfNeeded()
    {
        if (!colorRestoreMode) return;

        foreach (var name in restoredObjectNames)
        {
            var go = GameObject.Find(name);
            if (go == null) continue;
            var rend = go.GetComponentInChildren<SpriteRenderer>();
            if (rend != null) rend.color = Color.white;
        }
        restoredObjectNames.Clear();
    }

    // =========================
    //     Photon Callbacks
    // =========================
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[GameManager] Player entered: {newPlayer.NickName} ({newPlayer.ActorNumber})");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[GameManager] Player left: {otherPlayer.NickName} ({otherPlayer.ActorNumber})");
    }
}