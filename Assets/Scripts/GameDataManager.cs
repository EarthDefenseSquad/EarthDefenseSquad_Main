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
        LoadData();  // 저장된 데이터 불러오기
    }


    private void OnApplicationQuit()
    {
        SaveData();
    }

    // 데이터 저장
    public void SaveData()
    {
        PlayerPrefs.SetInt("SelectedCharacter1P", selectedCharacterIndex1P);
        PlayerPrefs.SetInt("SelectedCharacter2P", selectedCharacterIndex2P);
        PlayerPrefs.SetInt("SelectedYear", selectedYear);
        PlayerPrefs.SetInt("SelectedStage", selectedStageIndex);
        PlayerPrefs.Save();
        Debug.Log("게임 데이터 저장 완료");
    }

    // 데이터 불러오기
    public void LoadData()
    {
        selectedCharacterIndex1P = PlayerPrefs.GetInt("SelectedCharacter1P", -1);
        selectedCharacterIndex2P = PlayerPrefs.GetInt("SelectedCharacter2P", -1);
        selectedYear = PlayerPrefs.GetInt("SelectedYear", -1);
        selectedStageIndex = PlayerPrefs.GetInt("SelectedStage", -1);
        Debug.Log("게임 데이터 불러오기 완료");
        UpdateCharacterUI();  // UI 갱신
    }

    // 데이터 초기화
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("게임 데이터 초기화 완료");

        selectedCharacterIndex1P = -1;
        selectedCharacterIndex2P = -1;
        selectedYear = -1;
        selectedStageIndex = -1;
        for (int i = 0; i < stageUnlocked.Length; i++)
        {
            stageUnlocked[i] = false; // 모든 스테이지 잠금
        }
    }

    // 스테이지 잠금 초기화
    public void ResetStageUnlock()
    {
        for (int i = 0; i < stageUnlocked.Length; i++)
        {
            stageUnlocked[i] = false;
            PlayerPrefs.SetInt($"Stage{i + 1}Unlocked", 0);
        }

        PlayerPrefs.Save();
        Debug.Log("모든 스테이지 잠금 초기화 완료");
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