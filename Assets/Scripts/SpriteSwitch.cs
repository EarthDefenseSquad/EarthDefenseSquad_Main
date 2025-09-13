using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpriteSwitch : MonoBehaviourPun
{
    [System.Serializable]
    public class SpriteSet
    {
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

        private Color originalLeftColor; // 좌측 버튼 원래 색상 저장용
        private Color originalRightColor; // 우측 버튼 원래 색상 저장용

        public System.Action<SpriteSet> onConfirmToggle; // 선택 토글시 호출되는 이벤트

        // 초기화 함수 - 버튼에 클릭 이벤트 리스너 등록 및 초기 UI 적용
        public void Init()
        {
            // 입력 데이터 유효성 검사
            if (sprites.Length == 0 || targetImage == null ||
                names.Length != sprites.Length || infos.Length != sprites.Length)
                return;

            ApplyCurrent(); // 현재 인덱스에 맞는 UI 내용 적용

            if (leftButton != null)
            {
                leftButton.onClick.AddListener(SwitchLeft); // 좌측 버튼 클릭시 SwitchLeft 호출
                originalLeftColor = leftButton.image.color; // 좌측 버튼 색상 저장
            }
            if (rightButton != null)
            {
                rightButton.onClick.AddListener(SwitchRight); // 우측 버튼 클릭시 SwitchRight 호출
                originalRightColor = rightButton.image.color; // 우측 버튼 색상 저장
            }
            if (confirmButton != null)
                confirmButton.onClick.AddListener(ToggleConfirm); // 확인 버튼 클릭시 ToggleConfirm 호출
        }

        // 우측 버튼 클릭시 선택 스프라이트 인덱스 증가, 적용
        private void SwitchRight()
        {
            if (!isConfirmed)
            {
                currentIndex = (currentIndex + 1) % sprites.Length;
                ApplyCurrent();
            }
        }

        // 좌측 버튼 클릭시 선택 스프라이트 인덱스 감소, 적용
        private void SwitchLeft()
        {
            if (!isConfirmed)
            {
                currentIndex = (currentIndex - 1 + sprites.Length) % sprites.Length;
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

        // 확인 버튼 클릭시 호출, 등록된 onConfirmToggle 이벤트 발생
        private void ToggleConfirm()
        {
            onConfirmToggle?.Invoke(this);
        }

        // 선택 확정 및 해제 처리 - 버튼 상호작용 제한, UI 색상 변경 등
        public void SetConfirmed(bool confirm)
        {
            isConfirmed = confirm;

            // 방향 전환 버튼 비활성화 처리
            leftButton.interactable = !isConfirmed;
            rightButton.interactable = !isConfirmed;

            // UI 텍스트/이미지 색상 어둡게 (확정 시)
            SetDimmed(targetImage, isConfirmed);
            SetDimmed(backgroundPanel, isConfirmed);
            SetDimmed(characterName, isConfirmed);
            SetDimmed(characterInfo, isConfirmed);

            // 버튼 이미지 색상 조절 (확정 시 회색톤)
            if (leftButton.image != null)
                leftButton.image.color = isConfirmed ? new Color(0.5f, 0.5f, 0.5f, originalLeftColor.a) : originalLeftColor;
            if (rightButton.image != null)
                rightButton.image.color = isConfirmed ? new Color(0.5f, 0.5f, 0.5f, originalRightColor.a) : originalRightColor;

            // 확인 버튼 텍스트 변경 (확정 시 "취소", 미확정 시 "준비")
            if (confirmButtonText != null)
                confirmButtonText.text = isConfirmed ? "취소" : "준비";

            // 확인 아이콘 색상 변경 (확정 시 노란색, 미확정 시 흰색)
            if (confirmIcon != null)
            {
                string hex = isConfirmed ? "#FFCE3E" : "#FFFFFF";
                if (ColorUtility.TryParseHtmlString(hex, out var color))
                    confirmIcon.color = color;

            }
        }

        // 그래픽 UI 요소 색상 조절 함수 (어둡게 또는 원상복구)
        private void SetDimmed(Graphic graphic, bool dim)
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
    public SpriteSet set1; // 첫 번째 플레이어 UI 캐릭터 세트

    [Header("두 번째 UI 세트")]
    public SpriteSet set2; // 두 번째 플레이어 UI 캐릭터 세트

    [Header("경고 패널")]
    public GameObject warningPanel; // 경고 메시지 패널
    public TextMeshProUGUI warningText; // 경고 메시지 텍스트

    public Button startSceneButton; // 시작 씬 이동 버튼
    public GameObject CharacterSelect_Panel; // 캐릭터 선택 패널
    public GameObject Start_Panel; // 시작 패널

    void Start()
    {
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

        // UI 선택 확인시 실행될 로컬 콜백 등록
        set1.onConfirmToggle += OnConfirmToggled;
        set2.onConfirmToggle += OnConfirmToggled;

        // 네트워크 동기화용 RPC 호출 처리 콜백 등록
        set1.onConfirmToggle += OnConfirmToggle_Network;
        set2.onConfirmToggle += OnConfirmToggle_Network;

        // UI 초기화 및 선택 해제 상태로 설정
        set1.Init();
        set2.Init();
        set1.SetConfirmed(false);
        set2.SetConfirmed(false);

        // 경고 패널 숨김
        if (warningPanel != null)
            warningPanel.SetActive(false);

        // 씬 이동 버튼 클릭 리스너 등록
        if (startSceneButton != null)
        {
            startSceneButton.onClick.AddListener(() =>
            {
                // 캐릭터 선택 패널 끄고 시작 패널 켬
                CharacterSelect_Panel.SetActive(false);
                Start_Panel.SetActive(true);
            });
        }
    }

    // 캐릭터 선택 확인 토글 시 로컬 UI와 상태 처리 함수
    private void OnConfirmToggled(SpriteSet toggledSet)
    {
        // 다른 캐릭터 세트 참조
        SpriteSet otherSet = (toggledSet == set1) ? set2 : set1;
        int group1 = toggledSet.GetCharacterGroup(); // 선택된 캐릭터 그룹
        int group2 = otherSet.GetCharacterGroup(); // 다른 플레이어 캐릭터 그룹

        // 같은 그룹이면 경고 메시지 출력 후 종료
        if (otherSet.isConfirmed && group1 == group2)
        {
            ShowWarning("서로 다른 세대를 선택하세요.");
            return;
        }

        // 선택 상태 토글
        bool nextState = !toggledSet.isConfirmed;
        toggledSet.SetConfirmed(nextState);

        // 경고 패널 숨기기
        if (warningPanel != null)
            warningPanel.SetActive(false);

        // 두 플레이어 모두 선택 완료 시 씬 전환
        //if (set1.isConfirmed && set2.isConfirmed)
        //{
        //    SceneManager.LoadScene("WaitingScene");
        //}
    }

    // 선택 확인 토글 시 네트워크 RPC 호출 처리 함수
    private void OnConfirmToggle_Network(SpriteSet toggledSet)
    {
        if (toggledSet == null || set1 == null || set2 == null) return;

        int setNumber = (toggledSet == set1) ? 1 : 2; // UI 세트 번호 구분
        int currentIndex = toggledSet.currentIndex; // 선택된 캐릭터 인덱스
        bool nextState = !toggledSet.isConfirmed; // 변경될 선택 상태

        // 다른 클라이언트에게 RPC로 상태 정보 전송 (버퍼 포함)
        photonView.RPC(nameof(RPC_OnConfirmToggle), RpcTarget.OthersBuffered, setNumber, currentIndex, nextState);
    }

    // RPC 함수 - 다른 클라이언트에서 호출되어 UI 상태 동기화 처리
    [PunRPC]
    private void RPC_OnConfirmToggle(int setNumber, int currentIndex, bool isConfirmed)
    {
        SpriteSet targetSet = (setNumber == 1) ? set1 : set2;

        // 캐릭터 인덱스 갱신
        targetSet.currentIndex = currentIndex;

        // 기존 OnConfirmToggled 실행해 UI 변경 및 상태 동작 수행
        OnConfirmToggled(targetSet);
    }

    // 경고 메시지 보여주기 함수
    private void ShowWarning(string message)
    {
        if (warningText != null)
            warningText.text = message;

        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            CancelInvoke(nameof(HideWarning)); // 기존 숨김 예약 취소
            Invoke(nameof(HideWarning), 1f); // 1초 후 경고 패널 숨기기 예약
        }
    }

    // 경고 메시지 숨기기 함수
    private void HideWarning()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
}
