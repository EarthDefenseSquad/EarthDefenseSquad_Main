using UnityEngine;
using Photon.Pun;

public class FlipSync : MonoBehaviourPun, IPunObservable
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"[FlipSync] SpriteRenderer not found on: {gameObject.name}");
        }
        else
        {
            Debug.Log($"[FlipSync] Found SpriteRenderer on: {gameObject.name}");
        }
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
