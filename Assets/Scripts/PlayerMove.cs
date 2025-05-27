using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMove : MonoBehaviour
{
    
    public float maxSpeed;
    public float jumpPower;


    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsulecollider;
    AudioSource audioSource;
    BoxCollider2D boxCollider;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();    
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
           
        }
            
        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2 (rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            //normalized : 벡터 크기를 1로 만든 상태 (단위벡터)
        }

        // Direction Sprite 
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);

    }

    void FixedUpdate()  // 디폴트는 1초에 50번
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        // 방향키를 꾹 눌렀을 때 힘을 계속 더하니까 계속 빨라지겠군 ==> Max speed 를 정하자

        // Velocity : 리지드 바디의 현재 속도
        if (rigid.velocity.x > maxSpeed)      // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))    // Left Max Speed
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        // Lnading Platform
        if(rigid.velocity.y < 0) {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null) {
                //Debug.Log(rayHit.collider.name);
                //Debug.Log(rayHit.distance); // 거리 0.5076 이렇게나옴
                if (rayHit.distance < 0.6f)
                // 플레이어중심(레이시작점)-(레이쏴서맞은)플랫폼 거리가 0.5보다 커서 0.5f로하면 점핑모션 안끝나는 버그가 있었음.
                // 0.6f로 수정하니 해결..
                // 바닥에 비비다보면 점핑모션 제대로 끝났는데 그건 왜된거지 -> 가끔 0.4로 찍히는 곳도 있는데 이래서 끝난듯
                {
                    anim.SetBool("isJumping", false);
                }

            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "GameStart"){
            SceneManager.LoadScene("GameScene");
        }
    }

    

    }