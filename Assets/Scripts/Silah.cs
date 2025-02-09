using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using GameKit.Dependencies.Utilities;
using UnityEngine;

public class Silah : MonoBehaviour, EtkilesimliObje
{
    Transform atis_noktasi;
    [SerializeField] int sarjorBoyutu;
    [SerializeField] ObjeTipi _ObjeTipi;
    [SerializeField] string ObjeAdi;
    [SerializeField] float AtisSuresi;
    [SerializeField] float SarjorDegistirmeSuresi;
    [SerializeField] float sekme;
    [SerializeField] int hasar;
    float silahcooldown = 0;
    Collider MeshCollider;
    MeshRenderer Renderer;
    Oyuncu _oyuncu = null;
    int mermi = 0;
    bool sarjorDegistiriliyor = false;

    Vector3 startPos;

    Quaternion startRot;

    bool zoom = false;

    public int GetAmount()
    {
        return mermi;
    }

    public void Kullan()
    {
        if (_oyuncu == null){return;}


        if (mermi <= 0){
            // sarjor degistir
            if (sarjorDegistiriliyor){return;} // üst üste aynı fonksiyonu çağırma.
            sarjorDegistiriliyor = true;
            Invoke(nameof(Yenile), SarjorDegistirmeSuresi);
            return;
        }

        if (silahcooldown >= AtisSuresi)
        {
            silahcooldown = 0f;
            mermi -= 1;
            _oyuncu.Kamera_Y_Aksis -= sekme;
            _oyuncu.AtesEt(hasar);
        }
    }

    public void Yenile()
    {
        mermi += sarjorBoyutu;
        sarjorDegistiriliyor = false;
    }

    public void MiktarEkle(int miktar)
    {
        mermi += miktar;
    }

    public ObjeTipi ObjeTipiEnum()
    {
        return _ObjeTipi;
    }

    public string ObjeIsmi()
    {
        return ObjeAdi;
    }

    public void SetMeshAndCollider(bool status)
    {
        Renderer.enabled = status;
        MeshCollider.enabled = status;
    }

    public GameObject GetGameObject(){
        return gameObject;
    }

    void Start()
    {
        MeshCollider = GetComponent<Collider>();
        Renderer = GetComponent<MeshRenderer>();

        atis_noktasi = transform.GetChild(0);
        startPos = transform.position;
        startRot = transform.rotation;

        mermi = sarjorBoyutu;
    }

    void Update()
    {
        silahcooldown += Time.deltaTime;
    }

    public void ToggleCollider(bool status)
    {
        MeshCollider.enabled = status;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void UnParentTransform()
    {
        transform.SetParent(null);
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void KullanEventRegister(Oyuncu oyuncu)
    {
        _oyuncu = oyuncu;
        _oyuncu.silah_raycast_noktasi = atis_noktasi;
    }

    public void KullanEventUnRegister()
    {
       _oyuncu = null;
    }

    public void SetPosSpawn()
    {
        _oyuncu = null;
        silahcooldown = 0;
        sarjorDegistiriliyor = false;
        mermi = sarjorBoyutu;
        UnParentTransform();
        SetMeshAndCollider(true);
        transform.position = startPos;
        transform.rotation = startRot;
    }

    public void Sagtikla()
    {
        zoom = !zoom;
        if (zoom){Camera.main.fieldOfView = 20;}
        else{Camera.main.fieldOfView = 60;}
    }
}
