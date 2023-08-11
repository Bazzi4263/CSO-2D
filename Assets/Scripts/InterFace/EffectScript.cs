using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectScript : MonoBehaviourPun
{
    public static EffectScript instance = null;

    public GameObject obj_DamageIndicator;
    public GameObject bloodParticle;
    public GameObject deathParticle;
    public GameObject explosionParticle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [PunRPC]
    public void DamageIndicator(bool _isCritical, int _damage,Vector3 _pos)
    {
        _pos += new Vector3(UnityEngine.Random.Range(-0.2f, 1), 1f, 0);

        GameObject obj_text = Instantiate(obj_DamageIndicator, _pos, Quaternion.identity);

        Text damageText = obj_text.GetComponentInChildren<Text>();

        if (_isCritical)
        {
            damageText.color = new Color(1, 0, 0, 1);
        }

        damageText.text = _damage.ToString();

        Destroy(obj_text, 0.5f);
    }

    [PunRPC]
    public void BloodEffect(Vector3 _pos)
    {
        GameObject particle = Instantiate(bloodParticle , _pos, Quaternion.identity);
        Destroy(particle, 1f);
    }

    [PunRPC]
    public void DeathEffect(Vector3 _pos)
    {
        GameObject particle = Instantiate(deathParticle, _pos, Quaternion.identity);
        GameManager.instance.audioSource.clip = GameManager.instance.zombieDeathClip;
        GameManager.instance.audioSource.Play();
        Destroy(particle, 3f);
    }

    [PunRPC]
    public void ExplosionEffect(Vector3 _pos)
    {
        GameObject particle = Instantiate(explosionParticle, _pos, Quaternion.identity);
        GameManager.instance.audioSource2.PlayOneShot(GameManager.instance.explosionClip);
        Destroy(particle, 3f);
    }
}
