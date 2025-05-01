using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    
    public float maxSpeed;
    public float jumpPower;


    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsulecollider;
    AudioSource audioSource;


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
                if (rayHit.distance < 0.5f) {
                    anim.SetBool("isJumping", false);
                }

            }
        }
    }

    

    }