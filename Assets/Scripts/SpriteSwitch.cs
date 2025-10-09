using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

//public class SpriteSwitch : MonoBehaviourPun
public class SpriteSwitch : MonoBehaviourPunCallbacks
{
    private Coroutine initCo;
    
    [System.Serializable]
    public class SpriteSet
    {
        // 중첩 클래스 내부에 부모 참조 추가
        [System.NonSerialized] public SpriteSwitch parent;
        public int ownerPlayerIndex; // 0: set1, 1: set2
        public Image targetImage; // 캐릭터 이미지를 표시할 UI 이미지
        public Image backgroundPanel; // 배경 패널 이미지
        public TextMeshProUGUI characterName; // 캐릭터 이름 텍스트
        public TextMeshProUGUI characterInfo; // 캐릭터 정보 텍스트
        public Button leftButton; // 좌측 전환 버튼
        public Button rightButton; // 우측 전환 버튼
        public Button confirmButton; // 선택 확인 버튼
        public TextMeshProUGUI confirmButtonText; // 확인 버튼 텍스트
        public Image confirmIcon; // 확인 버튼 아이콘 이미지
        public Sprite[] sprites; // 캐릭터 스프라이트들 배열
        public string[] names; // 캐릭터 이름들 배열
        public string[] infos; // 캐릭터 정보 배열
        [HideInInspector] public int currentIndex = 0; // 현재 선택된 인덱스
        [HideInInspector] public bool isConfirmed = false; // 선택 확정 여부
        public Color originalLeftColor; // 좌측 버튼 원래 색상 저장용
        public Color originalRightColor; // 우측 버튼 원래 색상 저장용
        public System.Action<SpriteSet> onConfirmToggle; // 선택 토글시 호출되는 이벤트

        // ---------- 하드락 유틸 ----------
        private void SetButtonHardLock(Button btn, bool interactable)
        {
            if (btn == null) return;
            btn.interactable = interactable;

            var img = btn.GetComponent<Image>();
            if (img != null) img.raycastTarget = interactable;
            foreach (var g in btn.GetComponentsInChildren<Graphic>(true))
                g.raycastTarget = interactable;

            var cg = btn.GetComponent<CanvasGroup>();
            if (cg == null) cg = btn.gameObject.AddComponent<CanvasGroup>();
            cg.interactable = interactable;
            cg.blocksRaycasts = interactable;
        }

        // 초기화 함수 - 버튼에 클릭 이벤트 리스너 등록 및 초기 UI 적용
        // “내 세트만 조작 가능” 로직 추가 - 10.08 수정
        public void Init(int myPlayerIndex)
        {
            bool isMine = (ownerPlayerIndex == myPlayerIndex);

            if (ownerPlayerIndex != myPlayerIndex) return;
            if (sprites.Length == 0 || targetImage == null || names.Length != sprites.Length || infos.Length != sprites.Length) return;

            if (leftButton != null)
            {
                // 주석 = 기존 코드 전체 - 10.08 수정
                // leftButton.onClick.RemoveAllListeners();
                // leftButton.onClick.AddListener(() => { SwitchLeft(); parent?.OnSpriteSetIndexChanged(this); });
                // originalLeftColor = leftButton.image.color;

                if (isMine)
                {
                    leftButton.onClick.RemoveAllListeners();
                    SetButtonHardLock(leftButton, true);
                    leftButton.onClick.AddListener(() =>
                    {
                        // 🔐 로직 가드
                        if (parent.GetLocalPlayerIndex() != ownerPlayerIndex) return;

                        if (!isConfirmed && sprites != null && sprites.Length > 0)
                        {
                            currentIndex = (currentIndex - 1 + sprites.Length) % sprites.Length;
                            ApplyCurrent();
                            parent?.OnSpriteSetIndexChanged(this);
                        }
                    });
                    if (leftButton.image != null) originalLeftColor = leftButton.image.color;
                }
                else
                {
                    SetButtonHardLock(leftButton, false);
                }

            }

            if (rightButton != null)
            {
                // 주석 = 기존 코드 전체 - 10.08 수정
                // rightButton.onClick.RemoveAllListeners();
                // rightButton.onClick.AddListener(() => { SwitchRight(); parent?.OnSpriteSetIndexChanged(this); });
                // originalRightColor = rightButton.image.color;

                if (isMine)
                {
                    rightButton.onClick.RemoveAllListeners();
                    SetButtonHardLock(rightButton, true);
                    rightButton.onClick.AddListener(() =>
                    {
                        // 🔐 로직 가드
                        if (parent.GetLocalPlayerIndex() != ownerPlayerIndex) return;

                        if (!isConfirmed && sprites != null && sprites.Length > 0)
                        {
                            currentIndex = (currentIndex + 1) % sprites.Length;
                            ApplyCurrent();
                            parent?.OnSpriteSetIndexChanged(this);
                        }
                    });
                    if (rightButton.image != null) originalRightColor = rightButton.image.color;
                }
                else
                {
                    SetButtonHardLock(rightButton, false);
                }
            }

            if (confirmButton != null)
            {
                // 주석 = 기존 코드 전체 - 10.08 수정
                // confirmButton.onClick.RemoveAllListeners();
                // confirmButton.onClick.AddListener(() => { parent?.OnConfirmToggledLocalWithNetwork(this); });

                if (isMine)
                {
                    confirmButton.onClick.RemoveAllListeners();
                    SetButtonHardLock(confirmButton, true);
                    confirmButton.onClick.AddListener(() =>
                    {
                        if (parent.GetLocalPlayerIndex() != ownerPlayerIndex) return; // 🔐 로직 가드
                        parent?.OnConfirmToggledLocalWithNetwork(this);
                    });
                }
                else
                {
                    SetButtonHardLock(confirmButton, false);
                }


            }

            if (!isMine) return; // 상대 세트는 여기서 끝

            // 내 세트만 UI 데이터 적용
            if (sprites == null || targetImage == null || sprites.Length == 0 ||
                names == null || infos == null ||
                names.Length != sprites.Length || infos.Length != sprites.Length)
            {
                Debug.LogWarning("[SpriteSet.Init] 데이터 부족");
                return;
            }


            ApplyCurrent();
        }

        // 좌우 전환 함수
        public void SwitchLeft()
        {
            if (!isConfirmed)
            {
                currentIndex = (currentIndex - 1 + sprites.Length) % sprites.Length;
                Debug.Log($"SwitchLeft currentIndex={currentIndex}");
                ApplyCurrent();
            }
        }
        public void SwitchRight()
        {
            if (!isConfirmed)
            {
                currentIndex = (currentIndex + 1) % sprites.Length;
                Debug.Log($"SwitchRight currentIndex={currentIndex}");
                ApplyCurrent();
            }
        }

        // 현재 선택된 인덱스에 맞게 UI 이미지, 이름, 정보 업데이트
        public void ApplyCurrent()
        {
            targetImage.sprite = sprites[currentIndex];
            characterName.text = names[currentIndex];
            characterInfo.text = infos[currentIndex];
        }

        // 선택 확정 및 해제 처리 - 버튼 상호작용 제한, UI 색상 변경 등
        public void SetConfirmed(bool confirm)
        {
            isConfirmed = confirm;
            leftButton.interactable = !isConfirmed;
            rightButton.interactable = !isConfirmed;
            SetDimmed(targetImage, isConfirmed);
            SetDimmed(backgroundPanel, isConfirmed);
            SetDimmed(characterName, isConfirmed);
            SetDimmed(characterInfo, isConfirmed);

            if (leftButton.image != null)
                leftButton.image.color = isConfirmed ? new Color(0.5f, 0.5f, 0.5f, originalLeftColor.a) : originalLeftColor;
            if (rightButton.image != null)
                rightButton.image.color = isConfirmed ? new Color(0.5f, 0.5f, 0.5f, originalRightColor.a) : originalRightColor;

            if (confirmButtonText != null)
                confirmButtonText.text = isConfirmed ? "취소" : "준비";

            if (confirmIcon != null)
            {
                string hex = isConfirmed ? "#FFCE3E" : "#FFFFFF";
                if (ColorUtility.TryParseHtmlString(hex, out var color))
                    confirmIcon.color = color;
            }
        }

        // 그래픽 UI 요소 색상 조절 함수 (어둡게 또는 원상복구)
        public void SetDimmed(Graphic graphic, bool dim)
        {
            if (graphic == null) return;
            graphic.color = dim ? new Color(0.5f, 0.5f, 0.5f, graphic.color.a) : new Color(1f, 1f, 1f, graphic.color.a);
        }

        // 현재 캐릭터의 그룹 구분 (2개씩 묶는 기준)
        public int GetCharacterGroup()
        {
            return currentIndex / 2;
        }
    }

    [Header("첫 번째 UI 세트")]
    public SpriteSet set1;
    [Header("두 번째 UI 세트")]
    public SpriteSet set2;
    [Header("경고 패널")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;
    public Button startSceneButton;
    public GameObject CharacterSelect_Panel;
    public GameObject Start_Panel;
    [Header("다음 씬 이름")]
    public string nextSceneName = "WaitingScene";

    private bool isProcessing = false;

    void Awake()
    {
        // 중첩 클래스에 부모 참조 전달
        set1.parent = this;
        set2.parent = this;

        // 캐릭터 설명 텍스트 초기화 (set1)
        set1.infos = new string[] {
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
        };
        // 캐릭터 설명 텍스트 초기화 (set2)
        set2.infos = new string[] {
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
        };

        // 경고 패널 숨김
        if (warningPanel != null) warningPanel.SetActive(false);

        // 씬 이동 버튼 클릭 리스너 등록
        if (startSceneButton != null)
        {
            startSceneButton.onClick.AddListener(() =>
            {
                CharacterSelect_Panel.SetActive(false);
                Start_Panel.SetActive(true);
            });
        }
    }

    void OnEnable()
    {
        // 기존 초기화 제거
        // Debug.Log("SpriteSwitch OnEnable - UI 초기화");
        // int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        // set1.Init(playerIndex);
        // set1.SetConfirmed(false);
        // set2.Init(playerIndex);
        // set2.SetConfirmed(false);
    }

    public override void OnJoinedRoom()
    {
        if (initCo != null) StopCoroutine(initCo);
        initCo = StartCoroutine(WaitAndInit());
    }

    private IEnumerator WaitAndInit()
    {
        // Photon 완전 입장 대기
        while (!PhotonNetwork.InRoom || PhotonNetwork.LocalPlayer == null || PhotonNetwork.LocalPlayer.ActorNumber <= 0)
            yield return null;

        int playerIndex = GetLocalPlayerIndex();
        Debug.Log($"[SpriteSwitch] Init OK. InRoom={PhotonNetwork.InRoom}, Actor={PhotonNetwork.LocalPlayer.ActorNumber}, idx={playerIndex}, set1.owner={set1.ownerPlayerIndex}, set2.owner={set2.ownerPlayerIndex}");

        // 내 세트만 Init
        set1.Init(set1.ownerPlayerIndex == playerIndex ? playerIndex : -1);
        set2.Init(set2.ownerPlayerIndex == playerIndex ? playerIndex : -1);

        // 기본 상태 세팅
        if (set1.ownerPlayerIndex == playerIndex) set1.SetConfirmed(false);
        if (set2.ownerPlayerIndex == playerIndex) set2.SetConfirmed(false);
    }

    // 캐릭터 인덱스 변경 동기화
    public void OnSpriteSetIndexChanged(SpriteSet changedSet)
    {
        int setNumber = (changedSet == set1) ? 1 : 2;
        photonView.RPC(nameof(RPC_SyncCurrentIndex), RpcTarget.AllBuffered, setNumber, changedSet.currentIndex);
    }

    [PunRPC]
    private void RPC_SyncCurrentIndex(int setNumber, int syncedIndex)
    {
        var targetSet = (setNumber == 1) ? set1 : set2;
        targetSet.currentIndex = syncedIndex;
        targetSet.ApplyCurrent();
    }

    private bool IsSameGenerationConflict(SpriteSet toggledSet)
    {
        SpriteSet otherSet = (toggledSet == set1) ? set2 : set1;
        return otherSet.isConfirmed && toggledSet.GetCharacterGroup() == otherSet.GetCharacterGroup();
    }


    // “같은 세대 충돌 방지” 로직 복원 - 10.08 수정
    public void OnConfirmToggledLocalWithNetwork(SpriteSet toggledSet)
    {
        // 기존 코드 전체 주석처리함
        // if (IsSameGenerationConflict(toggledSet))
        // {
        //     ShowWarning("서로 다른 세대를 선택하세요.");
        //     return;
        // }
        // OnConfirmToggled_Internal(toggledSet, true);

        int myIdx = GetLocalPlayerIndex();
        if (toggledSet.ownerPlayerIndex != myIdx)
        {
            Debug.LogWarning($"[SpriteSwitch] Not your set. owner={toggledSet.ownerPlayerIndex}, me={myIdx}");
            return;
        }

        if (IsSameGenerationConflict(toggledSet))
        {
            ShowWarning("서로 다른 세대를 선택하세요.");
            return;
        }
        OnConfirmToggled_Internal(toggledSet, true);
    }

    private void OnConfirmToggled_Internal(SpriteSet toggledSet, bool isLocalCall)
    {
        if (isProcessing) return;
        isProcessing = true;

        bool nextState = !toggledSet.isConfirmed;
        toggledSet.SetConfirmed(nextState);

        if (warningPanel != null)
            warningPanel.SetActive(false);

        if (isLocalCall)
        {
            OnConfirmToggle_Network(toggledSet);

            var props = new ExitGames.Client.Photon.Hashtable();
            props["selectedIndex"] = toggledSet.currentIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            Debug.Log($"[SpriteSwitch] Saved selectedIndex={toggledSet.currentIndex} for player {PhotonNetwork.LocalPlayer.ActorNumber}");
        }

        if (set1.isConfirmed && set2.isConfirmed && PhotonNetwork.IsMasterClient)
        {
            CharacterSelectionData.player1SelectedIndex = set1.currentIndex;
            CharacterSelectionData.player2SelectedIndex = set2.currentIndex;

            var props = new ExitGames.Client.Photon.Hashtable();
            props["selectedIndex"] = PhotonNetwork.IsMasterClient ? set1.currentIndex : set2.currentIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
            photonView.RPC(nameof(RPC_LoadNextScene), RpcTarget.AllBuffered, nextSceneName);
        }

        isProcessing = false;
    }

    public void OnConfirmToggle_Network(SpriteSet toggledSet)
    {
        int setNumber = (toggledSet == set1) ? 1 : 2;
        int currentIndex = toggledSet.currentIndex;
        bool nextState = !toggledSet.isConfirmed;
        photonView.RPC(nameof(RPC_OnConfirmToggle), RpcTarget.AllBuffered, setNumber, currentIndex, nextState);
    }

    public int GetLocalPlayerIndex()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.ActorNumber > 0)
            return PhotonNetwork.LocalPlayer.ActorNumber - 1;
        return PhotonNetwork.IsMasterClient ? 0 : 1;
    }

    [PunRPC]
    public void RPC_OnConfirmToggle(int setNumber, int currentIndex, bool isConfirmed)
    {
        var targetSet = (setNumber == 1) ? set1 : set2;
        targetSet.currentIndex = currentIndex;
        targetSet.ApplyCurrent();
        OnConfirmToggled_Internal(targetSet, false);
    }

    [PunRPC]
    public void RPC_LoadNextScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public void ShowWarning(string message)
    {
        if (warningText != null)
            warningText.text = message;
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), 3f);
        }
    }

    public void HideWarning()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
}