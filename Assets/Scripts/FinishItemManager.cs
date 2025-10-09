using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
public class FinishItemManager : MonoBehaviourPunCallbacks
{
    GameManager gameManager;
    public int finishItemCount = 0;

    // PlayerPrefs 저장 키 이름 (로컬 저장용 키)
    public const string FinishItemKey = "FinishItemCount";

    public static FinishItemManager Instance;
    private List<string> collectedItemIDs = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 꼭 유지하고 싶다면
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    void Start()
    {

        Debug.Log("현재 FinishItemManager 개수: " + FindObjectsOfType<FinishItemManager>().Length);

        //LoadFinishItemCount();
        //UpdateFinishItemUI();
    }

    // public void RegisterCollectedItem(string itemID)
    // {
    //     if (!collectedItemIDs.Contains(itemID))
    //     {
    //         collectedItemIDs.Add(itemID);
    //         SyncCollectedItems();
    //     }
    // }

    void SyncCollectedItems()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "FinishItemCollectedIDs", collectedItemIDs.ToArray() }
            };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
    /// <summary>
    /// FinishItemCount를 불러와서 둘의 싱크를 맞춰줌.
    /// </summary>
    /// 
    /*public void LoadFinishItemCount()
    {
        int localValue = PlayerPrefs.GetInt(FinishItemKey, 0); //일단 값 불러옴
        photonView.RPC("RPC_CompareFinishItemCount", RpcTarget.All, localValue); //클라이언트가 마스터에게 값 전송
    }

    [PunRPC]
    public void RPC_CompareFinishItemCount(int clientValue)
    {
        int myValue = PlayerPrefs.GetInt(FinishItemKey, 0);

        // 규칙: 더 큰 값을 기준으로 맞추기
        int syncValue = Mathf.Max(myValue, clientValue);

        finishItemCount = syncValue;
        PlayerPrefs.SetInt(FinishItemKey, finishItemCount); //이제 두 명의 값이 같아짐.
    }


    /// <summary>
    /// 아이템 수치를 초기화하는 함수
    /// </summary>
    public void ResetFinishItemData()
    {
        // PlayerPrefs에서 해당 키 제거
        PlayerPrefs.DeleteKey(FinishItemKey);

        // 메모리 상의 수치도 0으로 초기화
        finishItemCount = 0;

        // UI 반영
        //UpdateFinishItemUI();
    }

    public void UpdateFinishItemUI()
    {
        photonView.RPC("RPC_UpdateFinishItemUI", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void RPC_UpdateFinishItemUI()
    {
        // 텍스트 컴포넌트가 정상 연결되어 있으면 숫자를 표시함
        if (finishItemText != null)
            finishItemText.text = $"{finishItemCount}";
    }
*/
    public void AddFinishItem()
    {
        /*// 수치 1 증가
        finishItemCount++;
        // PlayerPrefs에 저장 (로컬 디스크에 저장됨)
        PlayerPrefs.SetInt(FinishItemKey, finishItemCount);
        PlayerPrefs.Save(); // 강제로 저장

        LoadFinishItemCount();
        // UI 업데이트
        //UpdateFinishItemUI();*/
        // 현재 값 불러오기
        int currentCount = 0;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("FinishItemCount"))
        {
            currentCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["FinishItemCount"];
        }

        // 새로운 값
        int newCount = currentCount + 1;


        ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable { { "FinishItemCount", newCount } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);

        Debug.Log("AddFinishItem의 AddFinishItem함수의 값 : " + newCount);
    }

    public void RegisterCollectedItem(string itemID)
    {
        // 기존 리스트 불러오기
        string[] currentIDs = new string[0];
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("FinishItemCollectedIDs"))
        {
            currentIDs = (string[])PhotonNetwork.CurrentRoom.CustomProperties["FinishItemCollectedIDs"];
        }

        List<string> idList = new List<string>(currentIDs);

        // 중복 방지
        if (idList.Contains(itemID)) return;

        idList.Add(itemID);

        // 다시 커스텀 속성에 저장
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
    {
        { "FinishItemCollectedIDs", idList.ToArray() }
    };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }




}
