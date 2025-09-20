using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
public class EnemyMove : MonoBehaviourPun
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulecollider;
    GameManager gameManager;


    public int nextMove;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        gameManager = GetComponent<GameManager>();

        Invoke("Think",2);
    }

    void FixedUpdate()
    {
        if (gameManager.gameClear) photonView.RPC("ClearAfterMove", RpcTarget.All);
        // 이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);


        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null){
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
        nextMove = Random.Range(-1, 2);
        
        //Animation
        anim.SetInteger("WalkSpeed", nextMove);
        
        //Flip Sprite
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;


        //재귀함수
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);

    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        //Platform 낭떠러지
        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //Red

        spriteRenderer.flipY = true; //Flip Sprite

        capsulecollider.enabled = false; //Disable Collider

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse); //Jump force

        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false); //Disable GameObject
    }
}
