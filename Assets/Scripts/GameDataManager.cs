using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public GameObject Start_Panel;
    public GameObject StageSelectPanel;

    public StageSelectUI stageSelectUI;

    public int selectedCharacterIndex1P = -1;
    public int selectedCharacterIndex2P = -1;
    public int selectedYear = -1;
    public int selectedStageIndex = -1;

    public bool[] stageUnlocked;

    public bool story = false; 
   
    public string[] characterNames = { "MZ세대", "MZ세대 2", "X세대", "X세대 2" };

    public TextMeshProUGUI selected1PText;
    public TextMeshProUGUI selected2PText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        stageUnlocked = new bool[10];
        stageUnlocked[0] = true;
    }

    private void Start()
    {
        UpdateCharacterUI();
        LoadDataFromPlayFab();
    }

     
    // 데이터 저장 - PlayFab 사용
    public void SaveDataToPlayFab()
    {
        string storySeenValue = story ? "1" : "0";
        var data = new Dictionary<string, string>
        {
            { "StorySeen", storySeenValue  },
            { "SelectedCharacter1P", selectedCharacterIndex1P.ToString() },
            { "SelectedCharacter2P", selectedCharacterIndex2P.ToString() },
            { "SelectedYear", selectedYear.ToString() },
            { "SelectedStage", selectedStageIndex.ToString() },
            { "StageUnlocked", JsonUtility.ToJson(new Serialization<bool>(stageUnlocked)) }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = data },
            result => Debug.Log("게임 데이터 저장 완료 (PlayFab)"),
            error => Debug.LogError("게임 데이터 저장 실패: " + error.GenerateErrorReport()));
    }

    // 데이터 불러오기 - PlayFab 사용
    public void LoadDataFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                var data = result.Data;
                story = data.ContainsKey("StorySeen") ? data["StorySeen"].Value == "1" : false;
                selectedCharacterIndex1P = data.ContainsKey("SelectedCharacter1P") ? int.Parse(data["SelectedCharacter1P"].Value) : -1;
                selectedCharacterIndex2P = data.ContainsKey("SelectedCharacter2P") ? int.Parse(data["SelectedCharacter2P"].Value) : -1;
                selectedYear = data.ContainsKey("SelectedYear") ? int.Parse(data["SelectedYear"].Value) : -1;
                selectedStageIndex = data.ContainsKey("SelectedStage") ? int.Parse(data["SelectedStage"].Value) : -1;

                if (data.ContainsKey("StageUnlocked"))
                {
                    Serialization<bool> temp = JsonUtility.FromJson<Serialization<bool>>(data["StageUnlocked"].Value);
                    stageUnlocked = temp.ToArray();
                }
                else
                {
                    stageUnlocked = new bool[10];
                    stageUnlocked[0] = true;
                }

                Debug.Log("게임 데이터 불러오기 완료 (PlayFab)");
                UpdateCharacterUI();
                // 스토리 여부에 따라 분기
                if (!story)
                {
                    // 스토리 씬 보여주기
                    UnityEngine.SceneManagement.SceneManager.LoadScene("StoryScene");
            }
                else
                {
                    // 바로 다음 씬으로
                    UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
                }
            },
            error => Debug.LogError("게임 데이터 불러오기 실패: " + error.GenerateErrorReport()));
    }

    // 데이터 초기화 - PlayFab 사용
    public void ResetDataPlayFab()
    {
        selectedCharacterIndex1P = -1;
        selectedCharacterIndex2P = -1;
        selectedYear = -1;
        selectedStageIndex = -1;

        for (int i = 0; i < stageUnlocked.Length; i++)
        {
            stageUnlocked[i] = false;
        }
        stageUnlocked[0] = true;

        var data = new Dictionary<string, string>
        {
            { "SelectedCharacter1P", "-1" },
            { "SelectedCharacter2P", "-1" },
            { "SelectedYear", "-1" },
            { "SelectedStage", "-1" },
            { "StageUnlocked", JsonUtility.ToJson(new Serialization<bool>(stageUnlocked)) }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = data },
            result => Debug.Log("게임 데이터 초기화 완료 (PlayFab)"),
            error => Debug.LogError("게임 데이터 초기화 실패: " + error.GenerateErrorReport()));
    }

    // 스테이지 잠금 초기화 - PlayFab 사용
    public void ResetStageUnlockPlayFab()
    {
        for (int i = 0; i < stageUnlocked.Length; i++)
        {
            stageUnlocked[i] = false;
        }
        stageUnlocked[0] = true;

        var data = new Dictionary<string, string>
        {
            { "StageUnlocked", JsonUtility.ToJson(new Serialization<bool>(stageUnlocked)) }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = data },
            result => Debug.Log("모든 스테이지 잠금 초기화 완료 (PlayFab)"),
            error => Debug.LogError("모든 스테이지 잠금 초기화 실패: " + error.GenerateErrorReport()));
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

// 배열 직렬화/역직렬화 유틸리티
[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    private List<T> target;
    public List<T> ToList() { return target; }

    public T[] ToArray() { return target != null ? target.ToArray() : null; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }

    public Serialization(T[] target)
    {
        this.target = new List<T>(target);
    }
}

