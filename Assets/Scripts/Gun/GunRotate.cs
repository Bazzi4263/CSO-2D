using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GunRotate : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public float offset;
    public GameObject gunHole;

    private Vector3 t_pos;
    private Vector3 t_flipPos;

    void Start()
    {
        t_pos = gunHole.transform.localPosition;
        t_flipPos = new Vector3(gunHole.transform.localPosition.x, -gunHole.transform.localPosition.y, 0);

    }
    
    void Update()
    {
        GunRotation();
        GunLayer();
        GunFlip();
    }

    public void GunRotation()
    {
        if (PV.IsMine)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            difference.Normalize();

            float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            // Atan2 : 각도 구하는 함수, Rad2Deg : atan2는 라디안이기때문에 이 함수로 라디안을 각도로 변환.
            transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);

        }
    }

    public void GunLayer()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < this.transform.position.y)
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
