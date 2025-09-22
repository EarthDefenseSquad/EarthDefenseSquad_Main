using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;


public class FinishItemManager : MonoBehaviourPun
{
    GameManager gameManager;
    public int finishItemCount = 0;
    public Text finishItemText;
    // PlayerPrefs 저장 키 이름 (로컬 저장용 키)
    public  const string FinishItemKey = "FinishItemCount";

    public static FinishItemManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        LoadFinishItemCount();
        UpdateFinishItemUI();
    }
    /// <summary>
    /// FinishItemCount를 불러와서 둘의 싱크를 맞춰줌.
    /// </summary>
    /// 
    public void LoadFinishItemCount()
    {
        int localValue = PlayerPrefs.GetInt(FinishItemKey, 0); //일단 값 불러옴
        photonView.RPC("RPC_CompareFinishItemCount", RpcTarget.MasterClient, localValue); //클라이언트가 마스터에게 값 전송
    }

    [PunRPC]
    public void RPC_CompareFinishItemCount(int clientValue, PhotonMessageInfo info) 
    {
        int masterValue = PlayerPrefs.GetInt(FinishItemKey, 0); 

        // 규칙: 더 큰 값을 기준으로 맞추기
        int syncValue = Mathf.Max(masterValue, clientValue);

        // 두 쪽에 모두 적용
        photonView.RPC("RPC_SyncFinishItemCount", RpcTarget.All, syncValue); //비교해서 나온 큰 값을 모두에게 전송.
    }

    [PunRPC]
    public void RPC_SyncFinishItemCount(int syncValue)
    {
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
        UpdateFinishItemUI();
    }

    public void UpdateFinishItemUI()
    {
        photonView.RPC("RPC_UpdateFinishItemUI", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void RPC_UpdateFinishItemUI() {
        // 텍스트 컴포넌트가 정상 연결되어 있으면 숫자를 표시함
        if (finishItemText != null)
            finishItemText.text = $"{finishItemCount}";
    }

    public void AddFinishItem()
    {
        // 수치 1 증가
        finishItemCount++;

        // PlayerPrefs에 저장 (로컬 디스크에 저장됨)
        PlayerPrefs.SetInt(FinishItemKey, finishItemCount);
        PlayerPrefs.Save(); // 강제로 저장

        LoadFinishItemCount();
        // UI 업데이트
        UpdateFinishItemUI();

    }
    

}
