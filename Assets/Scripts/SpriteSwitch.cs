using UnityEngine;
using UnityEngine.UI;
using TMPro; // ← TextMeshPro용 네임스페이스

public class SpriteSwitch : MonoBehaviour
{
    [System.Serializable]
    public class SpriteSet
    {
        public Image targetImage;             // 이미지 오브젝트
        public TextMeshProUGUI characterName; // 캐릭터 이름 텍스트 (제목)
        public TextMeshProUGUI characterInfo; // 캐릭터 설명 텍스트 (본문)
        public Button leftButton;
        public Button rightButton;

        public Sprite[] sprites;              // 스프라이트 배열
        public string[] names;                // 캐릭터 이름 배열
        public string[] infos;                // 캐릭터 설명 배열
        

        [HideInInspector]
        public int currentIndex = 0;

        public void Init()
        {
            if (sprites.Length == 0 || targetImage == null ||
                names.Length != sprites.Length || infos.Length != sprites.Length)
                return;

            ApplyCurrent();

            if (leftButton != null)
                leftButton.onClick.AddListener(SwitchLeft);
            if (rightButton != null)
                rightButton.onClick.AddListener(SwitchRight);
        }

        private void SwitchRight()
        {
            currentIndex = (currentIndex + 1) % sprites.Length;
            ApplyCurrent();
        }

        private void SwitchLeft()
        {
            currentIndex = (currentIndex - 1 + sprites.Length) % sprites.Length;
            ApplyCurrent();
        }

        private void ApplyCurrent()
        {
            targetImage.sprite = sprites[currentIndex];
            characterName.text = names[currentIndex];
            characterInfo.text = infos[currentIndex];
        }
    }

    [Header("첫 번째 UI 세트")]
    public SpriteSet set1;

    [Header("두 번째 UI 세트")]
    public SpriteSet set2;

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

        set1.Init();
        set2.Init();
    }
}
