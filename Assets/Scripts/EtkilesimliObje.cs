using UnityEngine;

public interface EtkilesimliObje
{
    ObjeTipi ObjeTipiEnum();
    void Kullan();
    void Sagtikla();
    void KullanEventRegister(Oyuncu oyuncu);
    void KullanEventUnRegister();
    void MiktarEkle(int miktar);
    string ObjeIsmi();
    GameObject GetGameObject();
    Transform GetTransform();
    void UnParentTransform();
    void SetPos(Vector3 pos);
    void ToggleCollider(bool status);
    void SetMeshAndCollider(bool status);
    int GetAmount();
    void SetPosSpawn();
}

public enum ObjeTipi
{
    Silah,
    ElBombasi,
    SaglikKiti,
    Mermi,
    Duvar
}
