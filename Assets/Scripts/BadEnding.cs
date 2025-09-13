using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using UnityEngine.SceneManagement;


public class BadEnding : MonoBehaviour
{
    public DialogManager DialogManager;

    public GameObject EndButton;
    public GameObject[] Example;

    private void Awake()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/speed:-10/ 디바이드\n\n/speed:0.15/끝났다고 생각했나.\n이해? 웃기지 마. 인간은 결국 반복할 뿐이다.", "BadEnding"));

        dialogTexts.Add(new DialogData("/emote:2/", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:2//speed:-10/요원 Z\n\n/speed:0.05/박사님! /speed:0.1/전송 준비됐습니다!", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:2//speed:-10/요원 X\n\n/speed:0.1/지금 아니면 기회 없습니다. 보내세요.", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:2//speed:0.1/\n\n전송 개시. 모든 세대 데이터를 통합 중/speed:0.5/...", "BadEnding"));

        dialogTexts.Add(new DialogData("/emote:3/", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:3//speed:-10/박사\n\n/speed:0.1/전송 완료. 이상 없음. 디바이드 제거 완료로 보인다.", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:3//speed:-10/요원 Z\n\n/speed:0.1/됐다. 진짜 끝났다...", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:3//speed:-10/요원 X\n\n/speed:0.1/이제 좀 쉴 수 있겠군.", "BadEnding"));

        dialogTexts.Add(new DialogData("/emote:4//speed:0.3/...", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:4//speed:0.1/남은 조각은 어떻게 될 것인가?", "BadEnding"));
        dialogTexts.Add(new DialogData("/emote:4//speed:0.2/[Bad Ending - 번뜩이는 눈]", "BadEnding", () => ShowEndButton()));

        DialogManager.Show(dialogTexts);

        //dialogTexts.Callbak = () => ShowEndButton();
    }

    private void Show_Example(int index)
    {
        Example[index].SetActive(true);
    }

    private void ShowEndButton()
    {
        // 원하는 UI 오브젝트를 활성화
        Debug.Log("다이얼로그가 모두 끝났습니다.");
        EndButton.SetActive(true); // <- 여기에 버튼 연결
    }
    
    public void GoToStartScene()
    {
        SceneManager.LoadScene("StartScene");  // "MainScene"은 실제 메인 씬 이름으로 바꿔줘
    }
    
    //private void GoNextScene()
    //{
    //    SceneManager.LoadScene("SceneName");
    //}
}
