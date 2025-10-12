using UnityEngine;
using Photon.Pun;

public class FlipSync : MonoBehaviourPun, IPunObservable
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log($"[FlipSync] Sending flipX: {spriteRenderer.flipX}");
            stream.SendNext(spriteRenderer.flipX);
        }
        else
        {
            bool received = (bool)stream.ReceiveNext();
            spriteRenderer.flipX = received;
            Debug.Log($"[FlipSync] Received flipX: {received}");
        }
    }
}
