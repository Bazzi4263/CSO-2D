using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class AIRifle : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public NavMeshAgent agent;
    public GameObject gunHole;
    public AudioSource audioSource;
    public AudioClip fireClip;

    public ParticleSystem muzzleFlashParticle;

    float checkDistance;
    public GameObject target;

    bool isReload = false;
    bool isFire = false;
    float currentAmmo = Rifle.s_currentAmmo;
    float ammo = Rifle.s_currentAmmo;
    float fireRate = Rifle.s_fireRate + 0.05f;
    float reloadTime = Rifle.s_reloadTime;

    public float offset;

    private Vector3 t_pos;
    private Vector3 t_flipPos;

    private void Start()
    {
        StartCoroutine(CheckZombies());
        t_pos = gunHole.transform.localPosition;
        t_flipPos = new Vector3(gunHole.transform.localPosition.x, -gunHole.transform.localPosition.y, 0);
        checkDistance = Random.Range(17, 24);
    }

    private void Update()
    {
        Shot();
        TryReload();
        
        if (target != null)
        {
            GunRotation();
            GunLayer();
            GunFlip();
        }
    }

    IEnumerator CheckZombies()
    {
        while (PhotonNetwork.IsMasterClient)
        {
            List<Transform> zombies = new List<Transform>();
            List<GameObject> zombiesObj = new List<GameObject>();
            GameObject closestTarget = null;
            float closestTargetDistance = float.MaxValue;

            RaycastHit2D[] hitInfoes = Physics2D.CircleCastAll(transform.position, checkDistance, Vector2.zero);

            foreach (RaycastHit2D hitInfo in hitInfoes)
            {
                if (hitInfo.transform.tag == "Zombie" || hitInfo.transform.tag == "ZombieAI")
                {
                    Transform t = hitInfo.transform;
                    t.transform.position = new Vector3(t.position.x, t.position.y, 0);
                    zombies.Add(t);
                    zombiesObj.Add(hitInfo.transform.gameObject);
                }
            }

            Transform[] targets = zombies.ToArray();
            GameObject[] targetsObj = zombiesObj.ToArray();

            for (int i = 0; i < targets.Length; i++)
            {
                NavMeshPath path = new NavMeshPath();

                if (targetsObj[i].GetComponent<NavMeshAgent>() != null)
                {
                    Vector3 temp = transform.position;
                    temp.z = 0;

                    if (targetsObj[i].GetComponent<NavMeshAgent>().CalculatePath(temp, path))
                    {
                        float distance = Vector2.SqrMagnitude(transform.position - path.corners[0]);

                        for (int j = 1; j < path.corners.Length; j++)
                        {
                            distance += Vector2.SqrMagnitude(path.corners[j - 1] - path.corners[j]);
                        }

                        if (distance < closestTargetDistance)
                        {
                            closestTargetDistance = distance;
                            closestTarget = targets[i].gameObject;
                        }
                    }
                }
                else
                {
                    Vector3 temp = transform.position;
                    temp.z = 0;

                    if (NavMesh.CalculatePath(targets[i].transform.position, temp, agent.areaMask, path))
                    {
                        float distance = Vector2.SqrMagnitude(transform.position - path.corners[0]);

                        for (int j = 1; j < path.corners.Length; j++)
                        {
                            distance += Vector2.SqrMagnitude(path.corners[j - 1] - path.corners[j]);
                        }

                        if (distance < closestTargetDistance)
                        {
                            closestTargetDistance = distance;
                            closestTarget = targets[i].gameObject;
                        }
                    }
                }            
            }

            if (closestTarget != null)
            {
                target = closestTarget;
            }

            if (target != null)
            {
                if (Vector2.Distance(transform.position, target.transform.position) > checkDistance)
                {
                    target = null;
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private void Shot()
    {
        if (PhotonNetwork.IsMasterClient && target != null)
        {
            if (currentAmmo > 0 && !isReload && !isFire)
            {
                isFire = true;
                StartCoroutine(Fire());
            }
        }
    }

    IEnumerator Fire()
    {
        PV.RPC("FireSound", RpcTarget.All);
        AddBullet();
        --currentAmmo;
        yield return new WaitForSeconds(fireRate + Random.Range(-0.1f, 0.1f));
        isFire = false;
    }

    private void AddBullet()
    {
        GameObject bullet = PhotonNetwork.Instantiate("BulletAI", gunHole.transform.position, Quaternion.identity);
        bullet.transform.parent = this.transform;
    }

    private void TryReload()
    {
        if (currentAmmo == 0 && !isReload && PhotonNetwork.IsMasterClient)
        {
            StopCoroutine(Reload());
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReload = true;
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = ammo;
        isReload = false;

    }

    [PunRPC]
    public virtual void FireSound()
    {
        muzzleFlashParticle.Play();
        audioSource.PlayOneShot(fireClip);
    }

    public void GunRotation()
    {
        if (PhotonNetwork.IsMasterClient && target != null)
        {
            Vector3 difference = target.transform.position - transform.position;

            difference.Normalize();

            float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            // Atan2 : 각도 구하는 함수, Rad2Deg : atan2는 라디안이기때문에 이 함수로 라디안을 각도로 변환.
            transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);

        }
    }

    public void GunLayer()
    {
        if (target.transform.position.y < this.transform.position.y)
            this.GetComponent<SpriteRenderer>().sortingOrder = 1;
        else
            this.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }

    public void GunFlip()
    {
        if (transform.rotation.eulerAngles.z > 100 && transform.rotation.eulerAngles.z < 260)
        {
            this.GetComponent<SpriteRenderer>().flipY = true;
            this.transform.localPosition = new Vector3(-0.3f, -0.2f, 0);
            gunHole.transform.localPosition = t_flipPos;
        }
        else if (transform.rotation.eulerAngles.z > 280 && transform.rotation.eulerAngles.z < 360)
        {
            this.GetComponent<SpriteRenderer>().flipY = false;
            this.transform.localPosition = new Vector3(0.3f, -0.2f, 0);
            gunHole.transform.localPosition = t_pos;
        }
        else if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 80)
        {
            this.GetComponent<SpriteRenderer>().flipY = false;
            this.transform.localPosition = new Vector3(0.3f, -0.2f, 0);
            gunHole.transform.localPosition = t_pos;
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

    [PunRPC]
    public void GunFlipRPC(int _gunID)
    {
        GameObject _gun = PhotonView.Find(_gunID).gameObject;
        _gun.GetComponent<SpriteRenderer>().flipY = true;
        _gun.transform.localPosition = new Vector3(-0.3f, -0.2f, 0);
        gunHole.transform.localPosition = new Vector3(0.24f, -0.1f, 0);
    }

    [PunRPC]
    public void ReverseGunFlipRPC(int _gunID)
    {
        GameObject _gun = PhotonView.Find(_gunID).gameObject;
        _gun.GetComponent<SpriteRenderer>().flipY = false;
        _gun.transform.localPosition = new Vector3(0.3f, -0.2f, 0);
        gunHole.transform.localPosition = new Vector3(0.24f, 0.1f, 0);
    }
}
