using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using ExitGames.Client.Photon; 

public class StageSelectNetworkManager : MonoBehaviourPun
{
    
    public GameObject panel_year, panel_1970, panel_1980, panel_1990, panel_2000, panel_2010, panel_2020;
    //각 연도들이 나와있는 패널과, 각 연도의 1,2,3,4,5스테이지가 나와있는 패널

    public Button yearButton_1970, yearButton_1980, yearButton_1990, yearButton_2000, yearButton_2010, yearButton_2020;
    //연도들이 나와있는 패널(panel_year)에서 연도 선택 버튼 
     
    public Button stageButton_1970_1,stageButton_1970_2,stageButton_1970_3, stageButton_1970_4, stageButton_1970_5;
    //1970년도의 각 스테이지.

    public Button stageButton_1980_1, stageButton_1980_2, stageButton_1980_3, stageButton_1980_4, stageButton_1980_5;
    //1980년도의 각 스테이지.

    public Button stageButton_1990_1, stageButton_1990_2, stageButton_1990_3, stageButton_1990_4, stageButton_1990_5;
    //1990년도의 각 스테이지.

    public Button stageButton_2000_1, stageButton_2000_2, stageButton_2000_3, stageButton_2000_4, stageButton_2000_5;
    //2000년도의 각 스테이지.

    public Button stageButton_2010_1, stageButton_2010_2, stageButton_2010_3, stageButton_2010_4, stageButton_2010_5;
    //2010년도의 각 스테이지.

    public Button stageButton_2020_1, stageButton_2020_2, stageButton_2020_3, stageButton_2020_4, stageButton_2020_5;
    //2020년도의 각 스테이지.


    public Button backButton_Year, backButton_1970,backButton_1980,backButton_1990,backButton_2000,backButton_2010,backButton_2020;
    //백버튼

    public static int[] stage1970Flag = new int[] { 19701, 19702, 19703, 19704, 19705 };

    public static int[] stage1980Flag = new int[] { 19801, 19802, 19803, 19804, 19805 };

    public static int[] stage1990Flag = new int[] { 19901, 19902, 19903, 19904, 19905 };

    public static int[] stage2000Flag = new int[] { 20001, 20002, 20003, 20004, 20005, };

    public static int[] stage2010Flag = new int[] { 20101, 20102, 20103, 20104, 20105 };

    public static int[] stage2020Flag = new int[] { 20201, 20202, 20203, 20204, 20205 };
    public static int flag;
    void Awake()
    {
        Debug.Log("스테이지셀렉트네트워크 시작");
        if (yearButton_1970 != null)
            yearButton_1970.onClick.AddListener(() => OnYearButtonClicked(1970));
        if (yearButton_1980 != null)
            yearButton_1980.onClick.AddListener(() => OnYearButtonClicked(1980));
        if (yearButton_1990 != null)
            yearButton_1990.onClick.AddListener(() => OnYearButtonClicked(1990));
        if (yearButton_2000 != null)
            yearButton_2000.onClick.AddListener(() => OnYearButtonClicked(2000));
        if (yearButton_2010 != null)
            yearButton_2010.onClick.AddListener(() => OnYearButtonClicked(2010));
        if (yearButton_2020 != null)
            yearButton_2020.onClick.AddListener(() => OnYearButtonClicked(2020));


        // 마스터 클라이언트만 스테이지 버튼 클릭 가능하도록 설정
        bool isMaster = PhotonNetwork.IsMasterClient;
        SetButtonsInteractable(isMaster);

        if (isMaster)
        {
            // 각 스테이지 버튼에 클릭 리스너 등록
            if (backButton_Year != null)
                backButton_Year.onClick.AddListener(() => OnBackButtonClicked());
            if (stageButton_1970_1 != null)
                stageButton_1970_1.onClick.AddListener(() => OnStageButtonClicked(1970,0));
                Debug.Log("1970 1번 스테이지 버튼 리스너 등록됨");
            if (stageButton_1970_2 != null)
                stageButton_1970_2.onClick.AddListener(() => OnStageButtonClicked(1970, 1));
            if (stageButton_1970_3 != null)
                stageButton_1970_3.onClick.AddListener(() => OnStageButtonClicked(1970,2));
            if (stageButton_1970_4 != null)
                stageButton_1970_4.onClick.AddListener(() => OnStageButtonClicked(1970,3));
            if (stageButton_1970_5 != null)
                stageButton_1970_5.onClick.AddListener(() => OnStageButtonClicked(1970,4));
                
            if (stageButton_1980_1 != null)
                stageButton_1980_1.onClick.AddListener(() => OnStageButtonClicked(1980, 0));
                Debug.Log("1980 1번 스테이지 버튼 리스너 등록됨");
            if (stageButton_1980_2 != null)
                stageButton_1980_2.onClick.AddListener(() => OnStageButtonClicked(1980, 1));
            if (stageButton_1980_3 != null)
                stageButton_1980_3.onClick.AddListener(() => OnStageButtonClicked(1980,2));
            if (stageButton_1980_4 != null)
                stageButton_1980_4.onClick.AddListener(() => OnStageButtonClicked(1980,3));
            if (stageButton_1980_5 != null)
                stageButton_1980_5.onClick.AddListener(() => OnStageButtonClicked(1980,4));

            if (backButton_1970 != null)
                backButton_1970.onClick.AddListener(() => OnBackToYearSelect(1970));
            if (backButton_1980 != null)
                backButton_1980.onClick.AddListener(() => OnBackToYearSelect(1980));
            if (backButton_1990 != null)
                backButton_1990.onClick.AddListener(() => OnBackToYearSelect(1990));
            if (backButton_2000 != null)
                backButton_2000.onClick.AddListener(() => OnBackToYearSelect(2000));
            if (backButton_2010 != null)
                backButton_2010.onClick.AddListener(() => OnBackToYearSelect(2010));
            if (backButton_2020 != null)
                backButton_2020.onClick.AddListener(() => OnBackToYearSelect(2020));
      
        }

    }


    public void SetButtonsInteractable(bool interactable)
    {
        backButton_Year.interactable = interactable;
        backButton_1970.interactable = interactable;
        backButton_1980.interactable = interactable;
        backButton_1990.interactable = interactable;
        backButton_2000.interactable = interactable;
        backButton_2010.interactable = interactable;
        backButton_2020.interactable = interactable;

        stageButton_1970_1.interactable = interactable;
        stageButton_1970_2.interactable = interactable;
        stageButton_1970_3.interactable = interactable;
        stageButton_1970_4.interactable = interactable;
        stageButton_1970_5.interactable = interactable;

        stageButton_1980_1.interactable = interactable;
        stageButton_1980_2.interactable = interactable;
        stageButton_1980_3.interactable = interactable;
        stageButton_1980_4.interactable = interactable;
        stageButton_1980_5.interactable = interactable;

        stageButton_1990_1.interactable = interactable;
        stageButton_1990_2.interactable = interactable;
        stageButton_1990_3.interactable = interactable;
        stageButton_1990_4.interactable = interactable;
        stageButton_1990_5.interactable = interactable;

        stageButton_2000_1.interactable = interactable;
        stageButton_2000_2.interactable = interactable;
        stageButton_2000_3.interactable = interactable;
        stageButton_2000_4.interactable = interactable;
        stageButton_2000_5.interactable = interactable;

        stageButton_2010_1.interactable = interactable;
        stageButton_2010_2.interactable = interactable;
        stageButton_2010_3.interactable = interactable;
        stageButton_2010_4.interactable = interactable;
        stageButton_2010_5.interactable = interactable;

        stageButton_2020_1.interactable = interactable;
        stageButton_2020_2.interactable = interactable;
        stageButton_2020_3.interactable = interactable;
        stageButton_2020_4.interactable = interactable;
        stageButton_2020_5.interactable = interactable;
    
    }

    public void OnYearButtonClicked(int year)
    {
        if (year == 1970)
        {
            // 마스터 클라이언트만 패널 활성화 관련 RPC 전송 가능
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_ShowPanel), RpcTarget.All, 1970);
            else
                Debug.LogWarning("버튼은 마스터 클라이언트만 가능합니다.");
        }
        if (year == 1980)
        {
            // 마스터 클라이언트만 패널 활성화 관련 RPC 전송 가능
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_ShowPanel), RpcTarget.All, 1980);
            else
                Debug.LogWarning("버튼은 마스터 클라이언트만 가능합니다.");
        }
        if (year == 1990)
        {
            // 마스터 클라이언트만 패널 활성화 관련 RPC 전송 가능
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_ShowPanel), RpcTarget.All, 1990);
            else
                Debug.LogWarning("버튼은 마스터 클라이언트만 가능합니다.");
        }
        if (year == 2000)
        {
            // 마스터 클라이언트만 패널 활성화 관련 RPC 전송 가능
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_ShowPanel), RpcTarget.All, 2000);
            else
                Debug.LogWarning("버튼은 마스터 클라이언트만 가능합니다.");
        }
        if (year == 2010)
        {
            // 마스터 클라이언트만 패널 활성화 관련 RPC 전송 가능
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_ShowPanel), RpcTarget.All, 2010);
            else
                Debug.LogWarning("버튼은 마스터 클라이언트만 가능합니다.");
        }
        if (year == 2020)
        {
            // 마스터 클라이언트만 패널 활성화 관련 RPC 전송 가능
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_ShowPanel), RpcTarget.All, 2020);
            else
                Debug.LogWarning("버튼은 마스터 클라이언트만 가능합니다.");
        }
    }


    [PunRPC]
    public void RPC_ShowPanel(int year) //백버튼 눌렀을 때 해당 연도 패널 가려지고 year패널 나타나도록.
    {
        
        Debug.Log("연도 패널 보여줌");
        if (year == 1970)
        {
            if (panel_1970 != null)
            {
                panel_year.SetActive(false);
                panel_1970.SetActive(true);
                Hashtable props = new Hashtable
                {
                    { "SelectedYearIndex", 1970 }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }
        if (year == 1980)
        {
            if (panel_1980 != null)
            {
                panel_year.SetActive(false);
                panel_1980.SetActive(true);
                Hashtable props = new Hashtable
                {
                    { "SelectedYearIndex", 1980 }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }
        if (year == 1990)
        {
            if (panel_1990 != null)
            {
                panel_year.SetActive(false);
                panel_1990.SetActive(true);
            }
        }
        if (year == 2000)
        {
            if (panel_2000 != null)
            {
                panel_year.SetActive(false);
                panel_2000.SetActive(true);
            }
        }
        if (year == 2010)
        {
            if (panel_2010 != null)
            {
                panel_year.SetActive(false);
                panel_2010.SetActive(true);
            }
        }
        if (year == 2020)
        {
            if (panel_2020 != null)
            {
                panel_year.SetActive(false);
                panel_2020.SetActive(true);
            }   
        }
    }


    public void OnStageButtonClicked(int year, int stage)
    {
        Debug.Log("스테이지씬 클릭");
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("스테이지 선택은 마스터 클라이언트만 가능합니다.");
            return;
        }

        // 선택된 스테이지에 따라 필요 시 다른 처리를 할 수 있음
        Debug.Log($"마스터가 연도 {year} 선택");

        Debug.Log("RPC 호출 시도: year=" + year + ", stage=" + stage);
        photonView.RPC(nameof(RPC_GoToStageScene), RpcTarget.All, year, stage);
    }

    public void OnBackButtonClicked()
    {
        photonView.RPC(nameof(RPC_BackToWaitingScene), RpcTarget.All);
    }

    public void OnBackToYearSelect(int year)
    {
        photonView.RPC(nameof(RPC_BackToYearSelect), RpcTarget.All, year);
    }

    [PunRPC]
    public void RPC_BackToWaitingScene()
    {
        Debug.Log("웨이팅씬으로 감");
        PhotonNetwork.LoadLevel("WaitingScene");
    }

    [PunRPC]
    public void RPC_GoToStageScene(int year, int stage)
    {
        Debug.Log($"RPC_GoToStageScene 실행! year:{year}, stage:{stage}");
        if (year == 1970)
        {
            flag = stage1970Flag[stage];
            Debug.Log("현재 플래그 " + flag);
        }
        if (year == 1980)
        {
            flag = stage1980Flag[stage];
        }
        if (year == 1990)
        {
            flag = stage1990Flag[stage];
        }
        if (year == 2000)
        {
            flag = stage2000Flag[stage];
        }
        if (year == 2010)
        {
            flag = stage2010Flag[stage];
        }
        if (year == 2020)
        {
            flag = stage2020Flag[stage];
        }

       
    }

   
    [PunRPC]
    public void RPC_BackToYearSelect(int year)
    {
        Debug.Log("뒤로가기");
        if (year == 1970)
        {
            panel_1970.SetActive(false);
            panel_year.SetActive(true);
        }

        if (year == 1980)
        {
            panel_1980.SetActive(false);
            panel_year.SetActive(true);
        }
        if (year == 1990)
        {
            panel_1990.SetActive(false);
            panel_year.SetActive(true);
        }

        if (year == 2000)
        {
            panel_2000.SetActive(false);
            panel_year.SetActive(true);
        }
        if (year == 2010)
        {
            panel_2010.SetActive(false);
            panel_year.SetActive(true);
        }
        if (year == 2020)
        {
            panel_2020.SetActive(false);
            panel_year.SetActive(true);
        }
    }
}

