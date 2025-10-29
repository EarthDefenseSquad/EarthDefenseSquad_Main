using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject Start_Panel;
    public GameObject StageSelectPanel;

    [Header("References")]
    public StageSelectUI stageSelectUI;

    [Header("Selected Info")]
    public int selectedCharacterIndex1P = -1;
    public int selectedCharacterIndex2P = -1;
    public int selectedYear = -1;
    public int selectedStageIndex = -1;

    [Header("Stage Data")]
    public bool[] stageUnlocked;

    [Header("Story Progress")]
    public bool story = false; 

    [Header("Character Names")]
    public string[] characterNames = { "MZ세대", "MZ세대 2", "X세대", "X세대 2" };

    [Header("Character UI")]
    public TextMeshProUGUI selected1PText;
    public TextMeshProUGUI selected2PText;

    private void Awake()
    {
        // 싱글톤 유지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 초기 스테이지 잠금 설정
        stageUnlocked = new bool[10];
        stageUnlocked[0] = true; // 첫 스테이지만 해금
    }

    private void Start()
    {
        UpdateCharacterUI();

         UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    // 데이터 초기화
    public void ResetData()
    {
        selectedCharacterIndex1P = -1;
        selectedCharacterIndex2P = -1;
        selectedYear = -1;
        selectedStageIndex = -1;

        for (int i = 0; i < stageUnlocked.Length; i++)
            stageUnlocked[i] = false;

        stageUnlocked[0] = true;
        story = false;

        Debug.Log("데이터 초기화 완료 (로컬 변수만)");
    }

    // 캐릭터 선택 관련
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
