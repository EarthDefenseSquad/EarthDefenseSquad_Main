using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageNetworkManager : MonoBehaviour
{
    public GameObject panel_1970,panel_1980,panel_1990,panel_2000,panel_2010,panel_2020;
    public GameObject[] panel_1970_stages,panel_1980_stages,panel_1990_stages,panel_2000_stages,panel_2010_stages,panel_2020_stages; // 0: 1스테이지, 1: 2스테이지, ...
    // panel_1980과 panel_1990... 등도 동일하게 추가.

    void Start()
    {
        int flag = StageSelectNetworkManager.flag;
        int year = flag / 10;
        int stageIndex = flag % 10 - 1; // 1스테이지면 0, 2스테이지면 1 ...

        if (year == 1970)
        {
            for (int i = 0; i < panel_1970_stages.Length; i++)
                panel_1970_stages[i].SetActive(false);
            panel_1970.SetActive(true);
            if (stageIndex >= 0 && stageIndex < panel_1970_stages.Length)
                panel_1970_stages[stageIndex].SetActive(true);
        }
        if (year == 1980)
        {
            for (int i = 0; i < panel_1980_stages.Length; i++)
                panel_1980_stages[i].SetActive(false);
            panel_1980.SetActive(true);
            if (stageIndex >= 0 && stageIndex < panel_1980_stages.Length)
                panel_1980_stages[stageIndex].SetActive(true);
        }

        if (year == 1990)
        {
            for (int i = 0; i < panel_1990_stages.Length; i++)
                panel_1990_stages[i].SetActive(false);
            panel_1990.SetActive(true);
            if (stageIndex >= 0 && stageIndex < panel_1990_stages.Length)
                panel_1990_stages[stageIndex].SetActive(true);
        }

        if (year == 2000)
        {
            for (int i = 0; i < panel_2000_stages.Length; i++)
                panel_2000_stages[i].SetActive(false);
            panel_2000.SetActive(true);
            if (stageIndex >= 0 && stageIndex < panel_2000_stages.Length)
                panel_2000_stages[stageIndex].SetActive(true);
        }

        if (year == 2010)
        {
            for (int i = 0; i < panel_2010_stages.Length; i++)
                panel_2010_stages[i].SetActive(false);
            panel_2010.SetActive(true);
            if (stageIndex >= 0 && stageIndex < panel_2010_stages.Length)
                panel_2010_stages[stageIndex].SetActive(true);
        }
        
        if(year == 2020)
        {
            for(int i = 0; i < panel_2020_stages.Length; i++)
                panel_2020_stages[i].SetActive(false);
            panel_2020.SetActive(true);
            if(stageIndex >= 0 && stageIndex < panel_2020_stages.Length)
                panel_2020_stages[stageIndex].SetActive(true);
        }
        
    }
}
