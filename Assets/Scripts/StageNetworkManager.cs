using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;



public class StageNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject panel_1970, panel_1980, panel_1990, panel_2000, panel_2010, panel_2020;
    public GameObject[] panel_1970_stages, panel_1980_stages, panel_1990_stages, panel_2000_stages, panel_2010_stages, panel_2020_stages; // 0: 1스테이지, 1: 2스테이지, ...
    // panel_1980과 panel_1990... 등도 동일하게 추가.
    int selectedStageIndex = -1;
    int selectedYearIndex = -1;
    void Start()
    {

        ReadCustomProperties();    

    }

     void ReadCustomProperties()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SelectedStageIndex"))
            selectedStageIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedStageIndex"];

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SelectedYearIndex"))
            selectedYearIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedYearIndex"];

        if (selectedStageIndex == -1 || selectedYearIndex == -1)
        {
            Debug.LogWarning("커스텀 프로퍼티가 아직 준비되지 않았거나 값이 없습니다.");
            return;
        }

        // 정상적으로 값이 있을 때 UI 동기화 RPC 호출
        photonView.RPC("SyncStagePanel", RpcTarget.AllBuffered, selectedStageIndex, selectedYearIndex);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer &&
            (changedProps.ContainsKey("SelectedStageIndex") || changedProps.ContainsKey("SelectedYearIndex")))
        {
            Debug.Log("커스텀 프로퍼티 변경 알림 받음");

            // 프로퍼티 다시 읽고 UI 갱신
            ReadCustomProperties();
        }
    }

    [PunRPC]
    void SyncStagePanel(int selectedStageIndex, int selectedYearIndex)
    {
        int year = selectedYearIndex;
        int stageIndex = selectedStageIndex%5;
        Debug.Log("StageNetworkManager: 선택된 스테이지 인덱스: " + selectedStageIndex);
        Debug.Log("StageNetworkManager: 선택된 연도 인덱스: " + selectedYearIndex);

         // 모든 패널과 스테이지를 끔
        panel_1970.SetActive(false);
        panel_1980.SetActive(false);
        panel_1990.SetActive(false);
        panel_2000.SetActive(false);
        panel_2010.SetActive(false);
        panel_2020.SetActive(false);

        foreach (var s in panel_1970_stages) s.SetActive(false);
        foreach (var s in panel_1980_stages) s.SetActive(false);
        foreach (var s in panel_1990_stages) s.SetActive(false);
        foreach (var s in panel_2000_stages) s.SetActive(false);
        foreach (var s in panel_2010_stages) s.SetActive(false);
        foreach (var s in panel_2020_stages) s.SetActive(false);

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
