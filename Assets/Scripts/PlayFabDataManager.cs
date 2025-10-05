using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;



public class PlayFabDataManager : MonoBehaviour
{

    void Awake()
    {
        string customId = PhotonNetwork.NickName;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request,
            result => {
                Debug.Log("로그인 성공! PlayFabId: " + result.PlayFabId);
            },
            error =>
            {
                Debug.LogError("로그인 실패: " + error.GenerateErrorReport());
            }// 
        );
    }
    // 예시: CustomId로 로그인 (닉네임과 동일하게)
    

   
    public void SaveStageClear(int clearedStage, System.Action<int> onSuccess = null)
    {
        string key = "Stage" + clearedStage; // 예: stage3
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { key, "Clear" }
            }
        };
        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"{key} 저장 완료!");
                onSuccess?.Invoke(clearedStage);
            },
            error => Debug.LogError(error.GenerateErrorReport()));
    }
}
