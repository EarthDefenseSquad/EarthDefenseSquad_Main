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
    
    
    

}
