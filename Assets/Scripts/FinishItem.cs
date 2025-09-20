using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // Photon Hashtable

public class FinishItem : MonoBehaviourPunCallbacks
{
    public string itemID;  // 고유 ID: 예) "Stage1_Finish"
    private bool collected = false;

    void Start()
    {
        // 룸 프로퍼티에 이미 기록되어 있으면 투명 처리
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(itemID))
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties[itemID] == 1)
            {
                collected = true;
                SetCollectedVisual();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!collected)
        {
            collected = true;
            SetCollectedVisual();

            // ✅ 룸 프로퍼티 업데이트
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props[itemID] = 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            // GameManager에 FinishItem 증가 반영
            GameManager gm = FindObjectOfType<GameManager>();
            gm.AddFinishItem();
        }
    }

    void SetCollectedVisual()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.3f;
            sr.color = c;
        }
    }

    // 룸 프로퍼티 변경 시 동기화
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(itemID))
        {
            if ((int)propertiesThatChanged[itemID] == 1)
            {
                collected = true;
                SetCollectedVisual();
            }
        }
    }
}
