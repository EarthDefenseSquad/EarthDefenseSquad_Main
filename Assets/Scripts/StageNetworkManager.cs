using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class StageNetworkManager : MonoBehaviourPun
{
    public GameObject panel_1970, panel_1980, panel_1990, panel_2000, panel_2010, panel_2020;
    public GameObject[] panel_1970_stages, panel_1980_stages, panel_1990_stages, panel_2000_stages, panel_2010_stages, panel_2020_stages; // 0: 1스테이지, 1: 2스테이지, ...
    // panel_1980과 panel_1990... 등도 동일하게 추가.
    
    void Start()
    {

        int flag = PlayerPrefs.GetInt("SelectedStageFlag", 0);
        if(flag == 0)
            Debug.LogError("선택된 스테이지 플래그가 없습니다!");
        else
            Debug.Log("받은 flag: " + flag);
        

        int year = flag / 10;
        int stageIndex = flag % 10 - 1; // 1스테이지면 0, 2스테이지면 1 ...
        Debug.Log("flag = " + flag);
        photonView.RPC("SyncStagePanel", RpcTarget.AllBuffered, flag);

        

    }

    [PunRPC]
    void SyncStagePanel(int flag)
    {
        int year = flag / 10;
        int stageIndex = flag % 10 - 1;

        switch (year)
        {
            case 1970:
                SetStage(panel_1970, panel_1970_stages, stageIndex);
                break;
            case 1980:
                SetStage(panel_1980, panel_1980_stages, stageIndex);
                break;
            case 1990:
                SetStage(panel_1990, panel_1990_stages, stageIndex);
                break;
            case 2000:
                SetStage(panel_2000, panel_2000_stages, stageIndex);
                break;
            case 2010:
                SetStage(panel_2010, panel_2010_stages, stageIndex);
                break;
            case 2020:
                SetStage(panel_2020, panel_2020_stages, stageIndex);
                break;
        }
    }
    void SetStage(GameObject panel, GameObject[] stages, int stageIndex)
    {
        foreach(var stage in stages)
            stage.SetActive(false);
        panel.SetActive(true);
        if(stageIndex >= 0 && stageIndex < stages.Length)
            stages[stageIndex].SetActive(true);
    }
}
