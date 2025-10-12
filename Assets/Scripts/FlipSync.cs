using UnityEngine;
using Photon.Pun;

public class FlipSync : MonoBehaviourPun, IPunObservable
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        Debug.Log($"[FlipSync] Started. IsMine: {photonView.IsMine}");
    }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 flipX를 네트워크로 보냄
            stream.SendNext(spriteRenderer.flipX);

            Debug.Log($"[FlipSync] Sending flipX: {spriteRenderer.flipX}");
        }
        else
        {
            bool received = (bool)stream.ReceiveNext();
            spriteRenderer.flipX = received;
            Debug.Log($"[FlipSync] Received flipX: {received}");
        }
    }
}