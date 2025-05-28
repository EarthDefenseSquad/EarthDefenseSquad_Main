using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

    public bool colorRestoreMode = false;
    public GameObject goalObject; // Goal 오브젝트 연결


    // int FirstPositionX;
    // int FirstPositionY;

    // void Start()
    // {
    //     FirstPositionX = Player.main.transform.position.x;
    //     FirstPositionY = Player.main.transform.position.y;
    // }
    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposion();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            Time.timeScale = 0; // Pause the game
            Debug.Log("모든 스테이지를 클리어했습니다.");
            Text btnText = RestartButton.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            RestartButton.SetActive(true);
        }

        totalPoint += stagePoint;

        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 0)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.2f);
        }
        else
        {
            UIhealth[0].color = new Color(1, 0, 0, 0.2f);

            player.OnDie();

            Debug.Log("플레이어가 죽었습니다.");

            RestartButton.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            if (health > 1)
            {
                PlayerReposion();
            }


            HealthDown();
        }
    }

    void PlayerReposion()
    {
        player.transform.position = new Vector3(0, 0, -1); // Reset player position
        player.VelocityZero();
    }


    public void RestartGame()
    {
        Time.timeScale = 1; // Resume the game
        SceneManager.LoadScene(0);
    }

    public void EnableColorRestoreMode(bool enable)
    {
        colorRestoreMode = enable;
    }

    public IEnumerator GoalAppearEffect()
    {
        if (goalObject == null)
        {
            Debug.LogWarning("⚠ Goal 오브젝트가 비어있습니다.");
            yield break;
        }

        SpriteRenderer sr = goalObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("⚠ Goal 오브젝트에 SpriteRenderer가 없습니다.");
            yield break;
        }

        goalObject.SetActive(true); // 활성화는 하지만...
        float blinkInterval = 0.2f;
        int blinkCount = 5;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            sr.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 최종적으로 보이도록 유지
        sr.enabled = true;
        Debug.Log("🎯 Goal 깜빡임 연출 완료");
    }


}
