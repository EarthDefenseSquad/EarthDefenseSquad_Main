using UnityEngine;
using UnityEditor;

public class DebugEditorWindow : EditorWindow
{
    [MenuItem("Tools/Debug Editor Window")]
    public static void ShowWindow()
    {
        // EditorWindow 열기
        GetWindow<DebugEditorWindow>("Debug Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("디버그 모드 작업", EditorStyles.boldLabel);

        // 버튼을 눌러서 각 작업을 실행할 수 있습니다.
        if (GUILayout.Button("게임 데이터 초기화"))
        {
            ResetGameData();
        }

        if (GUILayout.Button("모든 스테이지 해금"))
        {
            UnlockAllStages();
        }

        if (GUILayout.Button("모든 스테이지 잠금"))
        {
            LockAllStages();
        }
    }

    private void ResetGameData()
    {
        // GameDataManager 초기화 (만약 초기화되지 않았다면)
        if (GameDataManager.Instance == null)
        {
            GameObject gameDataManagerObj = new GameObject("GameDataManager");
            gameDataManagerObj.AddComponent<GameDataManager>();
            Debug.Log("GameDataManager 인스턴스 초기화 완료");
        }

        // PlayerPrefs 초기화
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("게임 데이터 초기화 완료");

        // 캐릭터, 연도, 스테이지 초기화
        GameDataManager.Instance.selectedCharacterIndex1P = -1;
        GameDataManager.Instance.selectedYear = -1;
        GameDataManager.Instance.selectedStageIndex = -1;

        // 스테이지 잠금 초기화
        GameDataManager.Instance.ResetStageUnlockPlayFab();

        Debug.Log("게임 데이터가 초기화되었습니다.");
    }

    private void UnlockAllStages()
    {
        // 모든 스테이지 해금
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt($"Stage{i + 1}Unlocked", 1);
        }
        PlayerPrefs.Save();

        Debug.Log("모든 스테이지 해금 완료");
    }

    private void LockAllStages()
    {
        // 모든 스테이지 잠금
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt($"Stage{i + 1}Unlocked", 0);
        }
        PlayerPrefs.Save();

        Debug.Log("모든 스테이지 잠금 완료");
    }
}
