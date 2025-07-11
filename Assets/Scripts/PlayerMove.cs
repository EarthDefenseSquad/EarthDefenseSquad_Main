using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
<<<<<<< Updated upstream
=======
using Photon.Pun;
using UnityEngine.UI;
using JetBrains.Annotations;
>>>>>>> Stashed changes

public class PlayerMove : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }
    public PlayerType playerType;

    public float maxSpeed;
    public float jumpForce;
    public bool hasAccessPass = false;

    private bool isInvincible = false;
    private bool doubleJumpActive = false;
    private bool doubleJumpUsed = false;
    private bool colorRestoreMode = false;
    private HashSet<GameObject> restoredObjects = new HashSet<GameObject>();



    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    BoxCollider2D boxCollider;
    CapsuleCollider2D capsulecollider;
    public ItemManager itemManager;
    AudioSource audioSource;

    public AudioClip audioJump, audioAttack, audioDamaged, audioItem, audioDie, audioFinish;

<<<<<<< Updated upstream
    void Awake()
=======

    void Start()
>>>>>>> Stashed changes
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
<<<<<<< Updated upstream

=======
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

        if (PhotonNetwork.InRoom)
            roleID = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % 2;
        //roleID가 actorNumber-1이 짝수이면 나머지0, 홀수이면 나머지1 
>>>>>>> Stashed changes
        if (playerType == PlayerType.Player2)
        {
            jumpForce *= 1.3f;
            maxSpeed *= 1.3f;
        }

        Debug.Log($"[PlayerMove] {playerType} - Speed: {maxSpeed}, Jump: {jumpForce}");

    }



    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!anim.GetBool("isJump"))
            {
                rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetBool("isJump", true);
                doubleJumpUsed = false;
                PlaySound("Jump");
            }
            else if (doubleJumpActive && !doubleJumpUsed)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0);
                rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                doubleJumpUsed = true;
                PlaySound("Jump");
            }
        }

<<<<<<< Updated upstream
        if (Input.GetButtonUp("Horizontal"))
=======

        // Jump
        if (jumpPressed && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);

        }

        // Stop Speed
        if (h == 0)
        {
>>>>>>> Stashed changes
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

        if (Input.GetButtonDown("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        anim.SetBool("isWalk", Mathf.Abs(rigid.velocity.x) >= 0.3f);

        if (GameManager.Instance.colorRestoreMode)
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
                GameManager.Instance.colorRestoreMode = false;

                if (GameManager.Instance.goalObject != null)
                {
                    StartCoroutine(GameManager.Instance.GoalAppearEffect()); // 연출 호출
                }
            }


        }



    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed)
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        if (rigid.velocity.y < 0)
        {
<<<<<<< Updated upstream
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null && rayHit.distance < 0.6f)
                anim.SetBool("isJump", false);
        }
=======
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform", "HiddenPlatform"));
            if (rayHit.collider != null && rayHit.distance < 0.65f)
                anim.SetBool("isJumping", false);
        }
        if (rigid.velocity.x > maxSpeed)      // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))    // Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        // Lnading Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        }
>>>>>>> Stashed changes
    }

    public void EnableInvincibility(bool status)
    {
        isInvincible = status;
        spriteRenderer.color = status ? new Color(1, 1, 1, 0.5f) : Color.white;
    }
<<<<<<< Updated upstream
=======


>>>>>>> Stashed changes

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

    public void EnableColorRestore(bool enable)
    {
        GameManager.Instance.EnableColorRestoreMode(enable); // GameManager 통해 글로벌 설정
    }



    private bool ApproximatelyColor(Color a, Color b, float threshold = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
<<<<<<< Updated upstream

=======
    // Velocity : 리지드 바디의 현재 속도
>>>>>>> Stashed changes

    void OnTriggerEnter2D(Collider2D collision)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.right);

        if (collision.CompareTag("Item"))
        {
            string name = collision.name;

            bool isCoin =
                name.Contains("Bronze") ||
                name.Contains("Sliver") ||
                name.Contains("Gold");

<<<<<<< Updated upstream
            // ✅ 코인일 경우: Player1, Player2 모두 점수 획득
            if (isCoin)
            {
                if (name.Contains("Bronze")) gameManager.stagePoint += 50;
                else if (name.Contains("Sliver")) gameManager.stagePoint += 100;
                else if (name.Contains("Gold")) gameManager.stagePoint += 300;
=======
                if (isCoin)
                {
                    if (name.Contains("Bronze")) GameManager.Instance.stagePoint += 50;
                    else if (name.Contains("Sliver")) GameManager.Instance.stagePoint += 100;
                    else if (name.Contains("Gold")) GameManager.Instance.stagePoint += 300;

                    collision.gameObject.SetActive(false);
                    PlaySound("Item");
                    return;
                }

                if (playerType == PlayerType.Player2)
                {
                    Debug.Log("Player2는 아이템을 사용할 수 없습니다.");
                    return;
                }

                if (name.Contains("Buffering")) itemManager.UseItem(ItemType.BufferingIcon);
                else if (name.Contains("Invincibility")) itemManager.UseItem(ItemType.Invincibility);
                else if (name.Contains("DoubleJump")) itemManager.UseItem(ItemType.DoubleJump);
                else if (name.Contains("AccessPass")) itemManager.UseItem(ItemType.AccessPass);
                else if (name.Contains("RevealPlatform")) itemManager.UseItem(ItemType.RevealPlatform);
                else if (name.Contains("ColorRestore")) itemManager.UseItem(ItemType.ColorRestore);
>>>>>>> Stashed changes

                collision.gameObject.SetActive(false);
                PlaySound("Item");
            }
<<<<<<< Updated upstream

            // ❌ Player2는 아이템 무시 (먹지도 않고 삭제도 안 함)
            if (playerType == PlayerType.Player2)
            {
                Debug.Log("❌ Player2는 아이템을 사용할 수 없습니다. 아이템 무시됨.");
                return;
            }

            // ✅ Player1만 아이템 사용
            if (name.Contains("Buffering")) itemManager.UseItem(ItemType.BufferingIcon);
            else if (name.Contains("Invincibility")) itemManager.UseItem(ItemType.Invincibility);
            else if (name.Contains("DoubleJump")) itemManager.UseItem(ItemType.DoubleJump);
            else if (name.Contains("AccessPass")) itemManager.UseItem(ItemType.AccessPass);
            else if (name.Contains("RevealPlatform")) itemManager.UseItem(ItemType.RevealPlatform);
            else if (name.Contains("ColorRestore")) itemManager.UseItem(ItemType.ColorRestore);

            collision.gameObject.SetActive(false);
            PlaySound("Item");
=======
>>>>>>> Stashed changes
        }
        else if (collision.CompareTag("Finish"))
        {
            //gameManager.AddFinishItem();           // 수치 증가 + 저장 + UI 갱신
            collision.gameObject.SetActive(false); // 아이템 제거

            //gameManager.NextStage();
            PlaySound("Finish");
            PhotonNetwork.LoadLevel("StageSelect");
        }

<<<<<<< Updated upstream
        if (collision.gameObject.tag == "GameStart"){
            SceneManager.LoadScene("StageSelect");
        } // 세대, 스테이지선택씬으로
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible) return;

        if (collision.gameObject.CompareTag("Enemy"))
=======
        if (collision.gameObject.tag == "GameStart")
>>>>>>> Stashed changes
        {
                PhotonNetwork.LoadLevel("StageSelect");
             // 세대, 스테이지선택씬으로
        }
    }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (isInvincible) return;

            if (collision.gameObject.CompareTag("Enemy"))
            {
<<<<<<< Updated upstream
                OnAttack(collision.transform);
                PlaySound("Attack");
            }
            else
                OnDamaged(collision.transform.position);
        }
    }
=======
                if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                {
                    OnAttack(collision.transform);
                    PlaySound("Attack");
                }
                else
                {
                    OnDamaged(collision.transform.position);
                }
            }


        }

        void OnAttack(Transform enemy)
        {
            rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
            GameManager.Instance.stagePoint += 100;
            enemy.GetComponent<EnemyMove>()?.OnDamaged();
        }

        void OnDamaged(Vector2 targetPos)
        {
            GameManager.Instance.HealthDown();
            gameObject.layer = 11;
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
>>>>>>> Stashed changes

            int dir = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dir, 1) * 7, ForceMode2D.Impulse);

            PlaySound("Damaged");
            anim.SetTrigger("doDamaged");
            Invoke("OffDamaged", 3);
        }

        void OffDamaged()
        {
            gameObject.layer = 10;
            spriteRenderer.color = Color.white;
        }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        boxCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        PlaySound("Die");
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
<<<<<<< Updated upstream
=======


>>>>>>> Stashed changes
