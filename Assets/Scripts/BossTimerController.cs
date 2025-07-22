using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BossTimerController : MonoBehaviourPun
{
    [Header("타이머 설정")]
    [SerializeField] private float timeLimit = 300f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime;
    private bool isTimerRunning = false;

    public void StartBossTimer()
    {
        currentTime = timeLimit;
        isTimerRunning = true;
    }

    void Update()
    {
        if (!isTimerRunning) return;

        if (PhotonNetwork.IsMasterClient)
        {
            currentTime -= Time.deltaTime;
            photonView.RPC("SyncTimer", RpcTarget.Others, currentTime);
        }

        UpdateTimerUI();

        if (currentTime <= 0f)
        {
            isTimerRunning = false;
            TimeOut();
        }
    }

    [PunRPC]
    void SyncTimer(float syncedTime)
    {
        currentTime = syncedTime;
    }

    void UpdateTimerUI()
    {
        timerText.text = Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 10f)
        {
            timerText.color = Color.red;
        }
    }

    void TimeOut()
    {
        Debug.Log("시간 초과! 보스에게 패배");
        SceneManager.LoadScene("RetryScene");
    }
}
