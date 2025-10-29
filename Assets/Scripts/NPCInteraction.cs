using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NPCInteraction : MonoBehaviourPun
{
    public float interactRange = 2f;        // 플레이어와의 상호작용 거리
    public string goodEndingScene = "GoodEnding";
    public string badEndingScene = "BadEnding";
    private bool isPlayerNearby = false;

    private int collected;
    private GameObject player;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            // int collected = PlayerPrefs.GetInt("FinishItemCount", 0);  // 저장된 finish 개수 불러오기
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("FinishItemCount"))
            {
                collected = (int)PhotonNetwork.CurrentRoom.CustomProperties["FinishItemCount"];
            }
            //int required = FindObjectOfType<GameManager>().totalStages;
            int required = 3;
            if (collected >= required)
            {
                Debug.Log("Good Ending으로 이동");
                //SceneManager.LoadScene(goodEndingScene);
                photonView.RPC("GoTotheEndingScene", RpcTarget.All, "GoodEnding");
            }
            else
            {
                Debug.Log("Bad Ending으로 이동");
                //SceneManager.LoadScene(badEndingScene);
                photonView.RPC("GoTotheEndingScene", RpcTarget.All, "BadEnding");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
        }
    }

    [PunRPC]
    void GoTotheEndingScene(string scenename)
    {
        PhotonNetwork.LoadLevel(scenename);
    }
}
