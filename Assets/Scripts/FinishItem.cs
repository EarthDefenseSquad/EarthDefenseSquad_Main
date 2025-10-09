using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class FinishItem : MonoBehaviourPun
{
    public string itemID;  // 고유 ID: 예) "Stage1_Finish"

    private bool collected = false;

    public Button[] stageButtons;
    void Start()
    {
        //int localState = PlayerPrefs.GetInt(itemID, 0);
    

        // 투명화 처리
        // if (localState == 1)
        // {
        //     collected = true;
        //     SetCollectedVisual();
        // }
        
        StageButtonData data = GetComponent<StageButtonData>();
        string key = data != null ? data.requiredFinishID : itemID;

        bool alreadyCollected = false;

        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties != null)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out object value))
            {
                alreadyCollected = (int)value == 1;
            }
        }

        if (alreadyCollected)
        {
            collected = true;
            SetCollectedVisual();
        }

        // GameManager에 동기화 요청 (자기 값 보내기)
        //GameManager.Instance?.RequestItemSync(itemID, localState);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || collected || !other.GetComponent<PhotonView>().IsMine) return;


        // 로컬 저장
        PlayerPrefs.SetInt(itemID, 1);
        PlayerPrefs.Save();


        //네트워크 저장
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props[itemID] = 1; // "Stage1_Finish" → 1
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);


        // 시각 효과
        collected = true;
        SetCollectedVisual();

        // FinishItem 카운트 증가
        FinishItemManager.Instance?.AddFinishItem();

        // 동기화 전파
        //GameManager.Instance?.SendItemCollected(itemID);
    }

    void SetCollectedVisual()
    {
        // SpriteRenderer를 투명하게
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.3f;
            sr.color = c;
        }
    }
}
