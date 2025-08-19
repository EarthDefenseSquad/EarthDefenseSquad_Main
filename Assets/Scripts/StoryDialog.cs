using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class StoryDialog : MonoBehaviour
{
    public DialogManager DialogManager;

    public GameObject[] Example;

    private void Awake()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/speed:0.1/세상은... 원래 평화로웠지.", "Story"));

        dialogTexts.Add(new DialogData("/speed:0.1/아이들은 웃으며 뛰어놀았고, 어른들은 서로를 믿고 살아갔어.\n나이도, 생각도 달랐지만 그 차이는 갈등이 아니었지.. 그땐.\n하지만 누군가 그 균형을 망가뜨렸어.", "Story"));

        dialogTexts.Add(new DialogData("/emote:2//speed:0.1/세상은 눈에 띄게 어두워지기 시작했지.\n사람들은 점점 날카로워졌고, 다른 세대를 향한 시선엔 이해 대신 분노가 담기기 시작했어.\n어디서부터 틀어진 건지, 아무도 말하지 않았고, 아무도 들으려 하지 않았지.\n우리, 지구방위본부를 제외하고 말이야.", "Story"));

        dialogTexts.Add(new DialogData("/emote:2//speed:0.1/어디서부터 틀어진 건지, 아무도 말하지 않았고, 아무도 들으려 하지 않았지.\n우리, 지구방위본부를 제외하고 말이야.", "Story"));

        dialogTexts.Add(new DialogData("/emote:3//speed:0.1/지구방위본부는 끝없이 이유를 찾았어, 사람들이 왜 갑자기 서로를 미워하게 된 건지.\n그 갈등은 단순한 세대 차이에서 비롯된 게 아니었지.\n우리는 결국 한 가지 결론에 도달했다.\n보이지 않던 악의, ‘디바이드’라는 존재를.", "Story"));

        dialogTexts.Add(new DialogData("/emote:3//speed:0.1/눈으로는 볼 수 없었지만, 그 존재는 조용히, 그리고 확실하게 자라나고 있었어.\n우리가 눈치챘을 땐.. 이미 도시를 뒤덮을 만큼 거대해져 있었지.", "Story"));

        dialogTexts.Add(new DialogData("/emote:4//speed:0.1/우리는 급하게 대책을 세워야 했다.\n디바이드가 갈등을 먹고 자란다는 사실을 파악한 순간, 우리의 목표는 단 하나로 좁혀졌지.\n이 갈등을 끝내야 한다. 그렇지 않으면, 세상은 통째로 삼켜질 테니까.", "Story"));

        dialogTexts.Add(new DialogData("/emote:4//speed:0.1/그래서.. 우리는 ‘그 작전’을 시작했다.\nOperation : Gateway.", "Story"));

        dialogTexts.Add(new DialogData("/emote:5//speed:0.1/많은 요원들이 디바이드의 힘에 맞서 싸우기 시작했지.\n작전을 수행하며 갈등을 꿰뚫고 진실에 다가가려 했어.", "Story"));

        dialogTexts.Add(new DialogData("/emote:6//speed:0.1/하지만 디바이드는 이미 너무 깊숙이 퍼져 있었고.. 그들의 노력은 끝내 미완으로 남고 말았지. \n그들이 남긴 데이터는, 지금 너희 손에 맡겨졌다. ", "Story"));

        dialogTexts.Add(new DialogData("/emote:7//speed:0.1/지구방위본부의 마지막 희망.. 요원 X, 그리고 요원 Z.\n너희는 지금까지 요원들이 목숨을 걸고 수집한 데이터를 바탕으로\n내가 완성한 가상 시뮬레이션에 진입해야 한다.", "Story"));

        dialogTexts.Add(new DialogData("/emote:7//speed:0.1/이 장치, G.A.T.E는 ‘Genetic Archive Tactical Emulator’로\n세대간의 기억과 감정을 기반으로 구성된 특수한 시뮬레이션이지.\n이 안에서 너희는 직접 사건의 흐름을 되짚고, 분열의 실마리를 밝혀내야만 해.", "Story"));

        dialogTexts.Add(new DialogData("/emote:7//speed:0.1/두려움은 있어도 좋다. 하지만, 멈춰선 안 된다.\n너희의 용기가 곧 인류의 미래이니.", "Story"));

        dialogTexts.Add(new DialogData("/emote:8//speed:0.1/너희를 돕기 위해 몇가지 아이템을 시뮬레이션 내부에 배치해뒀다.\n이건 단순한 도구가 아니야. 데이터를 추적하는 데 있어 결정적인 ‘이정표’가 되어줄 것이지.\n아이템을 활용하여 너희 선배들의 임무를 완수해라. 사용방식은 요원 X에게 일러뒀다.", "Story"));

        dialogTexts.Add(new DialogData("/emote:9//speed:0.1/그곳에서 모은 기록은.. 우리 모두를 구할 실마리가 된다.\n…모든 건 너희 손에 달렸다.\n행운을 빈다. 요원 X, 요원 Z.", "Story"));

        DialogManager.Show(dialogTexts);

        //dialogTexts.Callbak = () => GoNextScene();
    }

    private void Show_Example(int index)
    {
        Example[index].SetActive(true);
    }
    
    //private void GoNextScene()
    //{
    //    SceneManager.LoadScene("SceneName");
    //}
}
