using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

public class SpriteSwitch : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class SpriteSet
    {
        [System.NonSerialized] public SpriteSwitch parent;
        public int ownerPlayerIndex; // 0: set1, 1: set2
        public Image targetImage;
        public Image backgroundPanel;
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI characterInfo;
        public Button leftButton;
        public Button rightButton;
        public Button confirmButton;
        public TextMeshProUGUI confirmButtonText;
        public Image confirmIcon;
        public Sprite[] sprites;
        public string[] names;
        public string[] infos;

        [HideInInspector] public int currentIndex = 0;
        [HideInInspector] public bool isConfirmed = false;
        public Color originalLeftColor;
        public Color originalRightColor;

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

        private void SetGraphicDim(Graphic graphic, bool dim)
        {
            if (graphic == null) return;
            graphic.color = dim
                ? new Color(0.5f, 0.5f, 0.5f, graphic.color.a)
                : new Color(1f, 1f, 1f, graphic.color.a);
        }
        // -------------------------------

        public void Init(int myPlayerIndex)
        {
            bool isMine = (ownerPlayerIndex == myPlayerIndex);

            // Confirm 버튼
            if (confirmButton != null)
            {
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

            // 좌측 버튼
            if (leftButton != null)
            {
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

            // 우측 버튼
            if (rightButton != null)
            {
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

        public void ApplyCurrent()
        {
            if (sprites != null && sprites.Length > 0 && targetImage != null)
                targetImage.sprite = sprites[currentIndex];
            if (names != null && names.Length > currentIndex && characterName != null)
                characterName.text = names[currentIndex];
            if (infos != null && infos.Length > currentIndex && characterInfo != null)
                characterInfo.text = infos[currentIndex];
        }

        public void SetConfirmed(bool confirm)
        {
            isConfirmed = confirm;

            // 좌우 버튼은 준비 상태일 때만 활성
            if (leftButton != null) leftButton.interactable = !isConfirmed;
            if (rightButton != null) rightButton.interactable = !isConfirmed;

            SetGraphicDim(targetImage, isConfirmed);
            SetGraphicDim(backgroundPanel, isConfirmed);
            SetGraphicDim(characterName, isConfirmed);
            SetGraphicDim(characterInfo, isConfirmed);

            if (leftButton != null && leftButton.image != null)
                leftButton.image.color = isConfirmed ? new Color(0.5f, 0.5f, 0.5f, leftButton.image.color.a) : originalLeftColor;
            if (rightButton != null && rightButton.image != null)
                rightButton.image.color = isConfirmed ? new Color(0.5f, 0.5f, 0.5f, rightButton.image.color.a) : originalRightColor;

            if (confirmButtonText != null)
                confirmButtonText.text = isConfirmed ? "취소" : "준비";

            if (confirmIcon != null)
            {
                string hex = isConfirmed ? "#FFCE3E" : "#FFFFFF";
                if (ColorUtility.TryParseHtmlString(hex, out var color))
                    confirmIcon.color = color;
            }
        }

        public int GetCharacterGroup() => currentIndex / 2;
    }

    [Header("세트 참조")]
    public SpriteSet set1;
    public SpriteSet set2;

    [Header("경고 UI")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;

    [Header("패널 / 씬")]
    public Button startSceneButton;
    public GameObject CharacterSelect_Panel;
    public GameObject Start_Panel;
    public string nextSceneName = "StoryScene";

    private Coroutine initCo;
    private bool isProcessing = false;

    void Awake()
    {
        set1.parent = this;
        set2.parent = this;

        if (warningPanel != null) warningPanel.SetActive(false);

        if (startSceneButton != null)
        {
            startSceneButton.onClick.RemoveAllListeners();
            startSceneButton.onClick.AddListener(() =>
            {
                CharacterSelect_Panel.SetActive(false);
                Start_Panel.SetActive(true);
            });
        }
    }

    public override void OnJoinedRoom()
    {
        if (initCo != null) StopCoroutine(initCo);
        initCo = StartCoroutine(WaitAndInit());
    }

    private IEnumerator WaitAndInit()
    {
        while (!PhotonNetwork.InRoom || PhotonNetwork.LocalPlayer == null || PhotonNetwork.LocalPlayer.ActorNumber <= 0)
            yield return null;

        int playerIndex = GetLocalPlayerIndex();
        Debug.Log($"[SpriteSwitch] Init OK. InRoom={PhotonNetwork.InRoom}, Actor={PhotonNetwork.LocalPlayer.ActorNumber}, idx={playerIndex}, set1.owner={set1.ownerPlayerIndex}, set2.owner={set2.ownerPlayerIndex}");

        set1.Init(set1.ownerPlayerIndex == playerIndex ? playerIndex : -1);
        set2.Init(set2.ownerPlayerIndex == playerIndex ? playerIndex : -1);

        if (set1.ownerPlayerIndex == playerIndex) set1.SetConfirmed(false);
        if (set2.ownerPlayerIndex == playerIndex) set2.SetConfirmed(false);
    }

    public int GetLocalPlayerIndex()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.ActorNumber > 0)
            return PhotonNetwork.LocalPlayer.ActorNumber - 1;
        return PhotonNetwork.IsMasterClient ? 0 : 1;
    }

    // --- 네트워크 동기화 ---
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

    public void OnConfirmToggledLocalWithNetwork(SpriteSet toggledSet)
    {
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

        if (warningPanel != null) warningPanel.SetActive(false);

        if (isLocalCall)
        {
            OnConfirmToggle_Network(toggledSet);

            // ✅ 내 선택 인덱스를 CustomProperties에 저장
            var props = new ExitGames.Client.Photon.Hashtable();
            props["selectedIndex"] = toggledSet.currentIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        if (set1.isConfirmed && set2.isConfirmed && PhotonNetwork.IsMasterClient)
        {
            CharacterSelectionData.player1SelectedIndex = set1.currentIndex;
            CharacterSelectionData.player2SelectedIndex = set2.currentIndex;

            // 방장이 씬 전환 → AutomaticallySyncScene 덕분에 클라 따라옴
            PhotonNetwork.LoadLevel(nextSceneName);
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

    [PunRPC]
    public void RPC_OnConfirmToggle(int setNumber, int currentIndex, bool isConfirmed)
    {
        var targetSet = (setNumber == 1) ? set1 : set2;
        targetSet.currentIndex = currentIndex;
        targetSet.ApplyCurrent();
        OnConfirmToggled_Internal(targetSet, false);
    }

    [PunRPC] public void RPC_LoadNextScene(string sceneName) => PhotonNetwork.LoadLevel(sceneName);

    public void ShowWarning(string message)
    {
        if (warningText != null) warningText.text = message;
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), 3f);
        }
    }
    public void HideWarning() { if (warningPanel != null) warningPanel.SetActive(false); }
}
