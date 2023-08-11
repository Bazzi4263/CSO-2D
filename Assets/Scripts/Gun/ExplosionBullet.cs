using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExplosionBullet : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public float bulletSpeed;
    public float offset;
    public AudioSource audioSource;

    Quaternion rotation;

    Gun gun;
    Vector3 mousePos;

    bool isFire = false;

    private void Start()
    {
        if (PV.IsMine)
        {
            gun = this.transform.parent.gameObject.GetComponent<Gun>();

            Vector3 sp = transform.position;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePos.x += 0.6f;
            //mousePos.y -= 0.6f;

            Vector3 dir = (mousePos - sp).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - offset;

            float spread = Random.Range(-gun.spread, gun.spread);

            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle + spread));

            PV.RPC("RotationRPC", RpcTarget.AllBuffered, bulletRotation);
        }

        transform.rotation = rotation;
        gameObject.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
        gameObject.transform.parent = null;

        StartCoroutine(BulletSize());
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (Vector2.Distance(mousePos, transform.position) < 0.5)
            {
                if (!isFire)
                {
                    EffectScript.instance.GetComponent<PhotonView>().RPC("ExplosionEffect", RpcTarget.All, transform.position);

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);
                    foreach (Collider2D hit in colliders)
                    {
                        if (hit.tag == "Zombie")
                        {
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;

                            hit.GetComponent<ZombieScript>().PV.RPC("Hit", RpcTarget.All, Barret.explosionDamageMin, Barret.explosionDamageMax, Barret.explosionCriticalDamage, Barret.explosionStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                        else if (hit.tag == "ZombieAI")
                        {
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;

                            hit.GetComponent<ZombieAI>().PV.RPC("Hit", RpcTarget.All, Barret.explosionDamageMin, Barret.explosionDamageMax, Barret.explosionCriticalDamage, Barret.explosionStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                    }
                    PV.RPC("DisapeearBullet", RpcTarget.All);
                    PV.RPC("Destroy", RpcTarget.All, 4f);
                    isFire = true;
                }
            }
        }
    }

    [PunRPC]
    void RotationRPC(Quaternion Rotation)
    {
        rotation = Rotation;
    }

    IEnumerator BulletSize()
    {
        for (int i = 0; i < 50; i++)
        {
            transform.localScale += new Vector3(0, 0.08f, 0);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PV.IsMine)
        {
            if (collision.tag != "Bullet" && collision.tag != "Human")
            {
                if (!isFire)
                {
                    EffectScript.instance.GetComponent<PhotonView>().RPC("ExplosionEffect", RpcTarget.All, transform.position);

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);
                    foreach (Collider2D hit in colliders)
                    {
                        if (hit.tag == "Zombie")
                        {
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;
 
                            hit.GetComponent<ZombieScript>().PV.RPC("Hit", RpcTarget.All, Barret.explosionDamageMin, Barret.explosionDamageMax, Barret.explosionCriticalDamage, Barret.explosionStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                        else if (hit.tag == "ZombieAI")
                        {
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;

                            hit.GetComponent<ZombieAI>().PV.RPC("Hit", RpcTarget.All, Barret.explosionDamageMin, Barret.explosionDamageMax, Barret.explosionCriticalDamage, Barret.explosionStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                    }

                    if (collision.tag == "Zombie")
                    {
                        if (collision.gameObject == null)
                        {
                            return;
                        }

                        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                        Vector2 explosionPos = transform.position;
                        Vector3 dir = (explosionPos - rb.position).normalized;

                        collision.GetComponent<ZombieScript>().PV.RPC("Hit", RpcTarget.All, gun.gunDamageMin, gun.gunDamageMax, gun.criticalDamage, gun.Stiffness, -dir.x, -dir.y, gun.KnockBack);
                        PV.RPC("DisapeearBullet", RpcTarget.All);
                        PV.RPC("Destroy", RpcTarget.All, 4f);
                    }
                    else if (collision.tag == "ZombieAI")
                    {
                        if (collision.gameObject == null)
                        {
                            return;
                        }

                        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                        Vector2 explosionPos = transform.position;
                        Vector3 dir = (explosionPos - rb.position).normalized;

                        collision.GetComponent<ZombieAI>().PV.RPC("Hit", RpcTarget.All, gun.gunDamageMin, gun.gunDamageMax, gun.criticalDamage, gun.Stiffness, -dir.x, -dir.y, gun.KnockBack);
                        PV.RPC("DisapeearBullet", RpcTarget.All);
                        PV.RPC("Destroy", RpcTarget.All, 4f);
                    }
                    else
                    {
                        PV.RPC("DisapeearBullet", RpcTarget.All);
                        PV.RPC("Destroy", RpcTarget.All, 4f);
                    }

                    isFire = true;
                }
            }
        }
    }

    [PunRPC]
    IEnumerator KnockBack(float _knockBack, Vector3 dir, Vector3 _pos, int _id)
    {
        GameObject zombie = PhotonView.Find(_id).gameObject;
        Rigidbody2D rb = zombie.GetComponent<Rigidbody2D>();

        zombie.GetComponent<ZombieScript>().isHit = true;
        rb.velocity = new Vector2(-dir.x * _knockBack, -dir.y * _knockBack);
        yield return new WaitForSeconds(0.1f);
        rb.velocity = new Vector2(0, 0);
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    [PunRPC]
    void Destroy(float _time) => Destroy(gameObject, _time);

    [PunRPC]
    void DisapeearBullet()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }
}
