using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    public int selectedCharacterIndex1P = -1;
    public int selectedCharacterIndex2P = -1;

    public int selectedYear = -1;
    public int selectedStageIndex = -1;

    public bool[] stageUnlocked;

    // 캐릭터 이름 배열 (인덱스와 매칭)
    public string[] characterNames = { "MZ세대", "MZ세대 2", "X세대", "X세대 2" };

    public TextMeshProUGUI selected1PText;
    public TextMeshProUGUI selected2PText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            stageUnlocked = new bool[10];
            stageUnlocked[0] = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCharacterUI();
    }

    public void OnCharacterButtonClicked1P(int characterIndex)
    {
        selectedCharacterIndex1P = characterIndex;
        string characterName = GetCharacterName(characterIndex);
        Debug.Log($"1P 캐릭터 선택됨: {characterName}");
        UpdateCharacterUI();
    }

    public void OnCharacterButtonClicked2P(int characterIndex)
    {
        selectedCharacterIndex2P = characterIndex;
        string characterName = GetCharacterName(characterIndex);
        Debug.Log($"2P 캐릭터 선택됨: {characterName}");
        UpdateCharacterUI();
    }

    private string GetCharacterName(int index)
    {
        if (index >= 0 && index < characterNames.Length)
            return characterNames[index];
        return "알 수 없음";
    }

    public void UpdateCharacterUI()
    {
        if (selected1PText != null)
        {
            string name1P = selectedCharacterIndex1P >= 0 ? GetCharacterName(selectedCharacterIndex1P) : "미선택";
            selected1PText.text = $"1P 선택: {name1P}";
        }

        if (selected2PText != null)
        {
            string name2P = selectedCharacterIndex2P >= 0 ? GetCharacterName(selectedCharacterIndex2P) : "미선택";
            selected2PText.text = $"2P 선택: {name2P}";
        }
    }
}
