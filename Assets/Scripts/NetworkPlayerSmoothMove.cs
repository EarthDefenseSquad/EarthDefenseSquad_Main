using Photon.Pun; // Photon 네트워크 관련 네임스페이스
using UnityEngine; // Unity 엔진 네임스페이스

// Photon 네트워크에서 동작하는 플레이어 위치 보간 스크립트
public class NetworkPlayerSmoothMove : MonoBehaviourPun, IPunObservable
{
    Vector3 networkPosition; // 네트워크로 받은 상대방의 위치를 저장하는 변수
    float lerpSpeed = 10f;   // 위치를 보간할 때 사용할 속도 (값이 클수록 더 빠르게 따라감)

    void Update()
    {
        // 내 오브젝트가 아니면(상대방 오브젝트라면)
        if (!photonView.IsMine)
        {
            // 현재 위치에서 네트워크로 받은 위치로 부드럽게 이동(Lerp)
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * lerpSpeed);
        }
    }

    // Photon의 동기화 콜백 함수 (위치 정보를 주고받는 역할)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 내 오브젝트라면(위치 정보를 보내는 쪽)
        {
            stream.SendNext(transform.position); // 내 현재 위치를 네트워크로 보냄
        }
        else // 상대방 오브젝트라면(위치 정보를 받는 쪽)
        {
            networkPosition = (Vector3)stream.ReceiveNext(); // 네트워크로 받은 위치를 변수에 저장
        }
    }
}
