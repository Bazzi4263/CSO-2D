using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ZombieAIHand : MonoBehaviourPunCallbacks, IPunObservable
{
    public float weaponReach;
    public float attackDelayA;
    public float attackDelayB;
    public float offset;
    public PhotonView PV;

    public bool isAttack = false;
    private bool isStartAttack = false;

    public float lerpSpeed = 10;
    public float lagSpeed = 100;

    Vector3 Rot;
    Vector3 MovedRot;
    Vector3 CRot;
    float lag;

    public AudioSource audioSource;

    public AudioClip attackClip1;
    public AudioClip attackClip2;
    public AudioClip attackHitClip;

    bool changeAttackSound = false;

    public ZombieAI zombieAI;

    private void Start()
    {
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(0.4f);
        isStartAttack = true;
    }

    private void Update()
    {
        if (isStartAttack)
        {
            Attack();
        }
        HandLayer();
    }

    void LateUpdate()
    {
        if (zombieAI.target != null)
            HandRotation();
    }

    private void Attack()
    {
        if (PhotonNetwork.IsMasterClient && zombieAI.target != null)
        {
            if (Vector3.Distance(zombieAI.target.transform.position, transform.position) < 5 && !isAttack)
            {
                PV.RPC("AttackCoroutine", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        AttackSound();
        transform.localPosition = Vector3.Lerp(transform.localPosition, transform.up * weaponReach, Time.deltaTime * 5);
        yield return new WaitForSeconds(attackDelayA);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 10);
        yield return new WaitForSeconds(attackDelayB);
        isAttack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.tag == "Human" && /*collision.GetComponent<PhotonView>().IsMine &&*/ isAttack)
            {
                PV.RPC("AttackHitSound", RpcTarget.All);
                var t = collision.GetComponent<HumanScript>();
                if (t == null)
                {
                    collision.GetComponent<HumanAI>().PV.RPC("InfectToZombie", RpcTarget.All, false);
                }
                else
                {
                    t.PV.RPC("InfectToZombie", RpcTarget.All, false);
                }
                return;
            }
        }
    }

    public void HandRotation()
    {
        if (!isAttack)
        {
            Vector3 TmpPos = transform.rotation.eulerAngles;

            if (!PhotonNetwork.IsMasterClient)
            {
                transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, CRot + MovedRot * lag, Time.deltaTime * lerpSpeed));
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 difference = zombieAI.target.transform.position - transform.position;
                difference.Normalize();

                float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                // Atan2 : 각도 구하는 함수, Rad2Deg : atan2는 라디안이기때문에 이 함수로 라디안을 각도로 변환.
                transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
            }

            PV.RPC("SetHand", RpcTarget.All);
            Rot = transform.rotation.eulerAngles;
            MovedRot = Rot - TmpPos;
        }
    }

    private void AttackSound()
    {
        if (changeAttackSound)
        {
            audioSource.PlayOneShot(attackClip1);
            changeAttackSound = !changeAttackSound;
        }
        else
        {
            audioSource.PlayOneShot(attackClip2);
            changeAttackSound = !changeAttackSound;
        }

    }

    [PunRPC]
    private void AttackHitSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(attackHitClip);
    }

    [PunRPC]
    private void SetHand()
    {
        transform.localPosition = transform.up * 0.3f;
    }

    public void HandLayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < this.transform.position.y)
                PV.RPC("LookFrontSynchronize", RpcTarget.All, this.gameObject.GetPhotonView().ViewID);
            else
                PV.RPC("LookBehindSynchronize", RpcTarget.All, this.gameObject.GetPhotonView().ViewID);
        }
    }

    [PunRPC]
    public void LookFrontSynchronize(int _gunID)
    {
        GameObject _gun = PhotonView.Find(_gunID).gameObject;
        _gun.GetComponent<SpriteRenderer>().sortingOrder = 1;

    }

    [PunRPC]
    public void LookBehindSynchronize(int _gunID)
    {
        GameObject _gun = PhotonView.Find(_gunID).gameObject;
        _gun.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Rot);
            stream.Serialize(ref MovedRot);
        }
        else
        {
            stream.Serialize(ref CRot);
            stream.Serialize(ref MovedRot);

            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp)) * lagSpeed;

        }
    }
}
