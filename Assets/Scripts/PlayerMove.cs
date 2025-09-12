using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public enum PlayerType { Player1, Player2 }
    public PlayerType playerType;

    public float maxSpeed;
    public float jumpForce;
    public bool hasAccessPass = false;
    private bool isInvincible = false;
    private bool doubleJumpActive = false;
    private bool doubleJumpUsed = false;
    private bool isJumping = false;

    private bool colorRestoreMode = false;
    private HashSet<GameObject> restoredObjects = new HashSet<GameObject>();

    
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsulecollider;
    public ItemManager itemManager;
    AudioSource audioSource;
    public static int clearedStage = -1;

    public AudioClip audioJump, audioAttack, audioDamaged, audioItem, audioDie, audioFinish;

    

    float h = 0; // 좌우 입력값
    bool jumpPressed = false;

    private bool waitToSelect = false;
    private bool stageToSelect = false;

    private GameManager gameManager;
    public StageSelectUI stageSelectUI;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        stageSelectUI = FindObjectOfType<StageSelectUI>();
    }
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        
        if (photonView.IsMine)
        {
            if (Camera.main != null)
            {
                Camera.main.GetComponent<CameraFollows>().SetTarget(this.transform);
            }
            else
            {
                Debug.LogError("Main Camera가 없습니다!");
            }
        }

        if (playerType == PlayerType.Player2)
        {
            jumpForce *= 1.3f;
            maxSpeed *= 1.3f;
        }
        Debug.Log($"[PlayerMove] {playerType} - Speed: {maxSpeed}, Jump: {jumpForce}");
    }//start 끝

    void Update()
    {
        if (!photonView.IsMine) return; //멀티 기능이므로 자기 자신이 아니면 움직이지 않도록 리턴시킴.

        // y값이 -20보다 작아지면 추락으로 간주
        if (transform.position.y <= -20f)
        {
            if (gameManager != null)
                gameManager.HealthDown();
        }
        
        // 플레이어: 방향키, Space
        h = Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;

        if (gameManager.colorRestoreMode)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("RestoreArea"))
                {
                    GameObject obj = hit.gameObject;

                    if (!restoredObjects.Contains(obj))
                    {
                        bool restored = false;

                        // 1. SpriteRenderer 타입인 경우
                        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            if (ApproximatelyColor(sr.color, new Color(0.27f, 0.27f, 0.27f)))
                            {
                                sr.color = Color.white;
                                restored = true;
                            }
                        }

                        // 2. TilemapRenderer + Tilemap 조합인 경우
                        UnityEngine.Tilemaps.Tilemap tilemap = obj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
                        if (tilemap != null)
                        {
                            if (ApproximatelyColor(tilemap.color, new Color(0.27f, 0.27f, 0.27f)))
                            {
                                tilemap.color = Color.white;
                                restored = true;
                            }
                        }

                        // 3. 복원되었다면 목록에 추가
                        if (restored)
                        {
                            restoredObjects.Add(obj);
                            Debug.Log($"🎨 복원됨: {obj.name}");
                        }
                    }
                }
            }

            GameObject[] restoreAreas = GameObject.FindGameObjectsWithTag("RestoreArea");
            if (restoreAreas.Length == restoredObjects.Count && restoreAreas.Length > 0)
            {
                Debug.Log("✅ 모든 RestoreArea 복원 완료 → Goal 나타남");
                gameManager.colorRestoreMode = false;

                if (gameManager.goalObject != null)
                {
                    StartCoroutine(gameManager.GoalAppearEffect()); // 연출 호출
                }
            }

        }
    
    } //update끝

    void FixedUpdate()
    {

        if (!photonView.IsMine) return;
        if (!ChatManager.isChatInputActive && Input.GetKeyDown(KeyCode.Space))
{           jumpPressed = true;
}
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed)
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        if (rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform", "HiddenPlatform"));
            if (rayHit.collider != null && rayHit.distance < 0.65f)
                anim.SetBool("isJumping", false);
        }
        if (jumpPressed)
        {
            if (!anim.GetBool("isJumping")) // 1단 점프
            {
                rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetBool("isJumping", true);
                doubleJumpUsed = false;
                PlaySound("Jump");

                jumpPressed = false;   // ✅ 여기서 바로 false 처리 → 같은 프레임에서 else if 못 탐
                return;                // ✅ 강제 리턴 → 2단 점프 분기 진입 방지
            }
            else if (doubleJumpActive && !doubleJumpUsed) // 2단 점프
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0);
                rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                doubleJumpUsed = true;
                PlaySound("Jump");
            }
            jumpPressed = false;
        }

        // Stop Speed
        if (h == 0)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            //normalized : 벡터 크기를 1로 만든 상태 (단위벡터)
        }

        // Direction Sprite 
        //if (Input.GetButton("Horizontal"))
        //    spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        if (h != 0)
            spriteRenderer.flipX = h == -1;
        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
            
        
    }

    public void EnableInvincibility(bool status)
    {
        isInvincible = status;
        spriteRenderer.color = status ? new Color(1, 1, 1, 0.5f) : Color.white;
    }
    
    public void EnableDoubleJump(float duration)
    {
        StartCoroutine(ActivateDoubleJump(duration));
    }

    IEnumerator ActivateDoubleJump(float duration)
    {
        doubleJumpActive = true;
        yield return new WaitForSeconds(duration);
        doubleJumpActive = false;
    }

    // ColorRestore가 GameManager를 통해 동작하고, Goal 연출도 GameManager에서 담당
    public void EnableColorRestore(bool enable)
    {
        gameManager.EnableColorRestoreMode(enable); // GameManager 통해 글로벌 설정
    }

    bool ApproximatelyColor(Color a, Color b, float threshold = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
    void OnTriggerEnter2D(Collider2D collision) //충돌인데 trigger체크 되어있는 충돌들
    {
      
        if (collision.CompareTag("Item"))
        {
            string name = collision.name;

            bool isCoin = name.Contains("Bronze") || name.Contains("Sliver") || name.Contains("Gold");

           // if (rayHit.collider != null)
            //{
            
                if (isCoin)
                {
                    if (name.Contains("Bronze")) gameManager.stagePoint += 50;
                    else if (name.Contains("Sliver")) gameManager.stagePoint += 100;
                    else if (name.Contains("Gold")) gameManager.stagePoint += 300;

                    collision.gameObject.SetActive(false);
                    //PlaySound("Item");
                    return;
                }

                if (playerType == PlayerType.Player2)
                {
                    Debug.Log("Player2는 아이템을 사용할 수 없습니다.");
                    return;
                }

                if (name.Contains("Buffering")) ItemManager.Instance.UseItem(ItemType.BufferingIcon, this);
                else if (name.Contains("Invincibility")) ItemManager.Instance.UseItem(ItemType.Invincibility, this);
                else if (name.Contains("DoubleJump")) ItemManager.Instance.UseItem(ItemType.DoubleJump, this);
                else if (name.Contains("AccessPass")) ItemManager.Instance.UseItem(ItemType.AccessPass, this);
                else if (name.Contains("RevealPlatform")) ItemManager.Instance.UseItem(ItemType.RevealPlatform, this);
                else if (name.Contains("ColorRestore")) ItemManager.Instance.UseItem(ItemType.ColorRestore, this);


                collision.gameObject.SetActive(false);
                PlaySound("Item");
            //}

        }

        else if (collision.CompareTag("Finish"))
        {
            //gameManager.AddFinishItem();           // 수치 증가 + 저장 + UI 갱신
            collision.gameObject.SetActive(false); // 아이템 제거
            gameManager.NextStage();
            //PlaySound("Finish");

            string finishId = collision.gameObject.name; // 예: Finish_1970_Stage1

            // 이미 저장된 경우 → 재카운트 방지 + 투명화
            if (PlayerPrefs.HasKey(finishId))
            {
                Debug.Log($"이미 먹은 Finish 아이템: {finishId}");
                // 이미 먹었음을 표시 (예: 반투명)
                SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(1f, 1f, 1f, 0.3f);
                }
                return;
            }

            // 처음 먹은 경우 → 저장하고 클리어 처리
            PlayerPrefs.SetInt(finishId, 1);
            PlayerPrefs.Save();

            Debug.Log($"✅ Finish 아이템 최초 획득: {finishId}");

            // GameManager 처리
            GameManager.Instance.SaveFinishItem();
            if (PhotonNetwork.IsMasterClient)
            {
                if (!stageToSelect)
                {
                    // ✅ 스테이지 클리어 처리
                    GameManager.Instance.OnGameClear();
                    GameManager.Instance.SaveFinishItem();

                    int clearedStage = GameManager.Instance.stageIndex;

                    // ✅ 스테이지 해금 DB 처리
                    photonView.RPC("DBonGameClear", RpcTarget.All, clearedStage);

                    // ✅ 씬 이동
                    photonView.RPC("ReqeustLoadLeveltoStage", RpcTarget.All, "StageSelect");
                }
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision) //충돌
    {
        if (isInvincible) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                //PlaySound("Attack");
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
        else if (collision.gameObject.CompareTag("Machine"))
        {
            Debug.Log("충돌");
            // Doctor 오브젝트 비활성화
            //collision.gameObject.SetActive(false);
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            if (PhotonNetwork.IsMasterClient)
            {
                if (!waitToSelect)
                {
                    photonView.RPC("ReqeustLoadLeveltoStage", RpcTarget.All, "StageSelect");
                }
            }
            else
            {
                    photonView.RPC("ReqeustLoadLeveltoStage", RpcTarget.MasterClient, "StageSelect");
            }
        }
    }


    [PunRPC]
    void ReqeustLoadLeveltoStage(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient && !waitToSelect) //마스터 클라이언트 && 웨이팅씬 나옴.
        {
            clearedStage++;
            Debug.Log("마스터 클라이언트 clearedStage: "+ clearedStage);
            photonView.RPC("SyncClearedStage", RpcTarget.OthersBuffered, clearedStage);
            waitToSelect = true;
            stageToSelect = true;
        }
        PhotonNetwork.LoadLevel(sceneName);
        
    }
    [PunRPC]
    void SyncClearedStage(int updatedClearedStage)
    {
        clearedStage = updatedClearedStage;
        Debug.Log("플레이어 clearedStage : " +clearedStage);
    }
    void OnAttack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        gameManager.stagePoint += 100;
        enemy.GetComponent<EnemyMove>()?.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        gameManager.HealthDown();
        gameObject.layer = 11;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 1) * 7, ForceMode2D.Impulse);

        PlaySound("Damaged");
        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 3);
    }

    void OffDamaged() //데미지 받으면 잠깐 투명해짐 그게 원상복구 되는 함수.
    {
        gameObject.layer = 10;
        spriteRenderer.color = Color.white;
    }
    

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        capsulecollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //PlaySound("Die");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump": audioSource.clip = audioJump; break;
            case "Attack": audioSource.clip = audioAttack; break;
            case "Damaged": audioSource.clip = audioDamaged; break;
            case "Item": audioSource.clip = audioItem; break;
            case "Die": audioSource.clip = audioDie; break;
            case "Finish": audioSource.clip = audioFinish; break;
        }
        audioSource.Play();
    }
}
