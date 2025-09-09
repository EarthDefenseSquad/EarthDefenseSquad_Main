using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpriteSwitchNetworkManager : MonoBehaviourPun
{
    [System.Serializable]
    public class SpriteSet
    {
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
        private Color originalLeftColor;
        private Color originalRightColor;
        public System.Action<SpriteSet> onConfirmToggle;

        public void Init()
        {
            if (sprites.Length == 0 || targetImage == null ||
                names.Length != sprites.Length || infos.Length != sprites.Length)
                return;
            ApplyCurrent();
            if (leftButton != null)
            {
                leftButton.onClick.AddListener(SwitchLeft);
                originalLeftColor = leftButton.image.color; // 🔸 저장
            }
            if (rightButton != null)
            {
                rightButton.onClick.AddListener(SwitchRight);
                originalRightColor = rightButton.image.color; // 🔸 저장
            }
            if (confirmButton != null)
                confirmButton.onClick.AddListener(ToggleConfirm);
        }

        private void SwitchRight()
        {
            if (!isConfirmed)
            {
                currentIndex = (currentIndex + 1) % sprites.Length;
                ApplyCurrent();
            }
        }

        private void SwitchLeft()
        {
            if (!isConfirmed)
            {
                currentIndex = (currentIndex - 1 + sprites.Length) % sprites.Length;
                ApplyCurrent();
            }
        }

        public void ApplyCurrent()
        {
            targetImage.sprite = sprites[currentIndex];
            characterName.text = names[currentIndex];
            characterInfo.text = infos[currentIndex];
        }

        private void ToggleConfirm()
        {
            // 네트워크 RPC 호출로 동기화
            onConfirmToggle?.Invoke(this);
        }

        public void SetConfirmed(bool confirm)
        {
            isConfirmed = confirm;
            leftButton.interactable = !isConfirmed;
            rightButton.interactable = !isConfirmed;
            SetDimmed(targetImage, isConfirmed);
            SetDimmed(backgroundPanel, isConfirmed);
            SetDimmed(characterName, isConfirmed);
            SetDimmed(characterInfo, isConfirmed);
            // 스프라이트 전환 버튼 색상 명도 조절 or 원복
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

        private void SetDimmed(Graphic graphic, bool dim)
        {
            if (graphic == null) return;
            if (dim)
                graphic.color = new Color(0.5f, 0.5f, 0.5f, graphic.color.a);
            else
                graphic.color = new Color(1f, 1f, 1f, graphic.color.a);
        }

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

    void Start()
    {
        set1.infos = new string[] {
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
        };
        set2.infos = new string[] {
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "Z세대 캐릭터\n아이템 사용 불가\n이동속도, 점프력 높음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
            "X세대 캐릭터\n아이템 사용 가능\n이동속도, 점프력 낮음",
        };

        set1.onConfirmToggle += OnConfirmToggle_Network;
        set2.onConfirmToggle += OnConfirmToggle_Network;

        set1.Init();
        set2.Init();

        set1.SetConfirmed(false);
        set2.SetConfirmed(false);

        if (warningPanel != null)
            warningPanel.SetActive(false);

        if (startSceneButton != null)
        {
            startSceneButton.onClick.AddListener(() =>
            {
                CharacterSelect_Panel.SetActive(false);
                Start_Panel.SetActive(true);
            });
        }
    }

    private void OnConfirmToggle_Network(SpriteSet toggledSet)
    {
        int setNumber = (toggledSet == set1) ? 1 : 2;
        int currentIndex = toggledSet.currentIndex;
        bool nextState = !toggledSet.isConfirmed;

        // RPC 호출: 다른 클라이언트에 선택 정보 전송
        photonView.RPC(nameof(RPC_OnConfirmToggle), RpcTarget.OthersBuffered, setNumber, currentIndex, nextState);

        // 로컬 클라이언트는 즉시 처리
        HandleSelection(toggledSet, nextState);
    }

    [PunRPC]
    private void RPC_OnConfirmToggle(int setNumber, int currentIndex, bool isConfirmed)
    {
        SpriteSet targetSet = (setNumber == 1) ? set1 : set2;
        targetSet.currentIndex = currentIndex;
        targetSet.SetConfirmed(isConfirmed);
        targetSet.ApplyCurrent();

        HandleSelection(targetSet, isConfirmed);
    }

    private void HandleSelection(SpriteSet toggledSet, bool isConfirmed)
    {
        SpriteSet otherSet = (toggledSet == set1) ? set2 : set1;

        int group1 = toggledSet.GetCharacterGroup();
        int group2 = otherSet.GetCharacterGroup();

        if (otherSet.isConfirmed && isConfirmed && group1 == group2)
        {
            ShowWarning("서로 다른 세대를 선택하세요.");
            return;
        }
        toggledSet.SetConfirmed(isConfirmed);
        if (warningPanel != null)
            warningPanel.SetActive(false);
        if (set1.isConfirmed && set2.isConfirmed)
        {
            SceneManager.LoadScene("WaitingScene");
        }
    }

    private void ShowWarning(string message)
    {
        if (warningText != null)
            warningText.text = message;
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), 1f);
        }
    }

    private void HideWarning()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
}
