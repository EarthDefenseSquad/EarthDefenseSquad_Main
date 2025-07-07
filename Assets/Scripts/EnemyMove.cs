using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
public class EnemyMove : MonoBehaviourPun
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriterenderer;
    public int nextMove;

    public GameManager gameManager;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();

        Invoke("Think", 5);
    }


    void FixedUpdate()
    {

        if (gameManager.gameClear) photonView.RPC("ClearAfterMove", RpcTarget.All);
        // 이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 낭떠러지 체크

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y); // 레이시작위치

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); // 레이 그리기
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // 레이히트

        if (rayHit.collider == null)
        {
            Turn();
        }
    }
    [PunRPC]
    public void ClearAfterMove()
    {
        gameObject.SetActive(false);
        Debug.Log("Enemy 비활성화");
    }
    void Think()
    {
        nextMove = Random.Range(-1, 2); // 왼쪽 -1, 멈춤 0, 오른쪽 1

        // 애니메이션 전환
        anim.SetInteger("WalkSpeed", nextMove);

        // 방향전환
        if (nextMove != 0)
            spriterenderer.flipX = nextMove == 1;

        float nextThinkTime = Random.Range(2f, 5f); // 인보크 시간 랜덤지정
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        // Debug.Log("낭떠러지~");

        nextMove *= -1; // nextMove = nextMove * -1;
        spriterenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }
}
