using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class FinishItem : MonoBehaviourPun
{
    public string itemID;  // 고유 ID: 예) "Stage1_Finish"

    private bool collected = false;

    void Start()
    {
        // ✅ 방의 커스텀 속성에서 이미 먹은 아이템인지 확인
        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("FinishItemCollectedIDs"))
        {
            string[] collectedIDs = (string[])PhotonNetwork.CurrentRoom.CustomProperties["FinishItemCollectedIDs"];
            if (System.Array.Exists(collectedIDs, id => id == itemID))
            {
                collected = true;
                SetCollectedVisual();
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || collected || !other.GetComponent<PhotonView>().IsMine) return;


        // ✅ FinishItemManager에게 아이템 수집 등록 요청
        FinishItemManager.Instance?.RegisterCollectedItem(itemID);



        // 시각 효과
        collected = true;
        SetCollectedVisual();

        // FinishItem 카운트 증가
        FinishItemManager.Instance?.AddFinishItem();

        // ✅ 모든 클라이언트에 동기화
        photonView.RPC("RPC_RemoteCollect", RpcTarget.OthersBuffered, itemID);

        // 동기화 전파
        //GameManager.Instance?.SendItemCollected(itemID);
    }

    [PunRPC]
    void RPC_RemoteCollect(string id)
    {
        if (id != itemID || collected) return;

        collected = true;
        SetCollectedVisual();
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
