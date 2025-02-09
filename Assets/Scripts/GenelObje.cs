using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GenelObje : MonoBehaviour, EtkilesimliObje
{

    [SerializeField] ObjeTipi _tip;
    [SerializeField] string Ad;
    [SerializeField] int amount;
    Collider MeshCollider;
    MeshRenderer Renderer;

    public int GetAmount()
    {
        return amount;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Transform GetTransform()
    {
        return gameObject.transform;
    }

    public void Kullan()
    {
        
    }

    public void KullanEventRegister(Oyuncu oyuncu)
    {
        
    }

    public void KullanEventUnRegister()
    {
        
    }

    public void MiktarEkle(int miktar)
    {
        
    }

    public string ObjeIsmi()
    {
        return Ad;
    }

    public ObjeTipi ObjeTipiEnum()
    {
        return _tip;
    }

    public void Sagtikla()
    {
        
    }

    public void SetMeshAndCollider(bool status)
    {
        Renderer.enabled = status;
        MeshCollider.enabled = status;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetPosSpawn()
    {

    }

    public void ToggleCollider(bool status)
    {
        MeshCollider.enabled = status;
    }

    public void UnParentTransform()
    {
        transform.SetParent(null);
    }

    void Start()
    {
        MeshCollider = GetComponent<Collider>();
        Renderer = GetComponent<MeshRenderer>();
    }

}
