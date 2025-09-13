using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using UnityEngine.SceneManagement;


public class GoodEnding : MonoBehaviour
{
    public DialogManager DialogManager;

    public GameObject EndButton;
    public GameObject[] Example;

    private void Awake()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/speed:-10/ 디바이드\n\n/speed:0.15/끝났다고 생각했나.\n이해? 웃기지 마. 인간은 결국 반복할 뿐이다.", "GoodEnding"));

        dialogTexts.Add(new DialogData("/emote:2/", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:2//speed:-10/요원 Z\n\n/speed:0.05/박사님! /speed:0.1/전송 준비됐습니다!", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:2//speed:-10/요원 X\n\n/speed:0.1/지금 아니면 기회 없습니다. 보내세요.", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:2//speed:0.1/\n\n전송 개시. 모든 세대 데이터를 통합 중/speed:0.5/...", "GoodEnding"));

        dialogTexts.Add(new DialogData("/emote:3/", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:3//speed:-10/디바이드\n\n/speed:0.1/이렇게 끝나다니...! 내 계획이!", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:3//speed:-10/박사\n\n/speed:0.1/완전 소거 확인.\n드디어 디바이드가 사라졌다.", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:3//speed:-10/요원 Z\n\n/speed:0.1/진짜 끝난 거예요... 맞죠?", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:3//speed:-10/요원 X\n\n/speed:0.1/...그래. 이건 확실해.", "GoodEnding"));

        dialogTexts.Add(new DialogData("/emote:4//speed:0.1/", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:4//speed:0.1/우리는 완벽하진 않지만,\n다시 연결될 수 있다.", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:4//speed:0.1/화합은 이상이 아니다. 선택이다.", "GoodEnding"));
        dialogTexts.Add(new DialogData("/emote:4//speed:0.2/[Happy Ending - 떠오르는 희망]", "GoodEnding", () => ShowEndButton()));

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
