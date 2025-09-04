using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class test : MonoBehaviour
{
    public DialogManager DialogManager;

    public GameObject[] Example;
    

    private void Awake()
    {
        // DialogData dialogData = new DialogData("Helloooooooooo!", "p1");

        // 사용법 헷갈리면 데모씬 확인해볼 것

        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("Helloooooooooo!!", "p1"));
        
        dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is MMMMZZZ.", "p1"));


        DialogManager.Show(dialogTexts);
    }

    private void Show_Example(int index)
    {
        Example[index].SetActive(true);
    }

}
