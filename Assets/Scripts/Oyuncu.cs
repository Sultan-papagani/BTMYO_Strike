using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using FishNet.Utility.Template;
using System;
using GameKit.Dependencies.Utilities;
using FishNet.Object.Synchronizing;
using UnityEditor.Rendering;
using System.Data.Common;
using FishNet.Component.Animating;

public class Oyuncu : NetworkBehaviour
{

    [SerializeField] Material takim_a_rengi, takim_b_rengi;
    [SerializeField] NetworkAnimator animator;
    [SerializeField] SkinnedMeshRenderer skin;
    [SerializeField] CharacterController OyuncuKontrol;
    [SerializeField] public Transform KameraPozisyonu;
    [SerializeField] Transform YerPozisyonu;
    [SerializeField] public Transform EsyaTutmaPozisyonu;
    [SerializeField] public Transform EsyaDondurmeAksisi;
    [SerializeField] Envater envater;
    [SerializeField] float oyuncuHiz = 7;
    [SerializeField] float kameraHassasiyet = 5;
    [SerializeField] float ziplamaGucu = 2;
    [SerializeField] float yerCekimiGucu = 9.81f;
    [SerializeField] GameObject lineObject;
    Vector3 Hareket;
    public float Kamera_Y_Aksis;
    float YerCekimi;
    public Transform OyuncuKamera;
    public static event Action<Oyuncu> OyuncuSahipGeldi;
    CanvasOyuncu OyuncuCanvas;
    EtkilesimliObje RaycastObjesi = null;
    EtkilesimliObje ElimizdekiObje = null;
    int can = 100;
    public Takim takim = Takim.SECMEDI; 
    bool OyunBasladi = false; // hemde hayattamı booleanı
    public bool spectator = false;
    TakimPozisyonlayici takimPozisyonlayici;
    public Transform silah_raycast_noktasi;

    public string oyuncuADI = "";

    //float time = 0;

    void Start()
    {
        takimPozisyonlayici = FindAnyObjectByType<TakimPozisyonlayici>();
    }

    // oyuncunun sahipliği bu objeye geçtiğinde çalışır
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        // bu objenin sahibi biziz;
        if (base.IsOwner){
            OyuncuSahipGeldi?.Invoke(this);

            Camera.main.GetComponent<Animator>().enabled = false;

            // ana kamerayı karakterin kafasına yerleştir
            Camera.main.transform.SetParent(KameraPozisyonu);
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            OyuncuKamera = Camera.main.transform;

            // oyuncu canvas'i getir
            OyuncuCanvas = FindAnyObjectByType<CanvasOyuncu>();

            OyuncuCanvas.owner_oyuncu = this;

            takimPozisyonlayici = FindAnyObjectByType<TakimPozisyonlayici>();

            if (StaticVariables.singleton.actual_host)
            {
                OyuncuCanvas.OwnerMaciActi();
            }


            // Burda "peak" oyun tasarım yöntemlerini görüyorsunuz
            envater.OyuncuCanvas = OyuncuCanvas;
            envater.oyuncu = this; 
            oyuncuADI = StaticVariables.singleton.OYUNCU_ADI;

        }else
        {

        }
    }

    [ObserversRpc]
    public void KarakteriEtkinlestir(bool state)
    {
        OyunBasladi = state;
    }

    public void OwnerOyunuBaslat()
    {
        takimPozisyonlayici.TakimlariBaslangicaDiz();
    }

    public void TakimSecVeAdAyarla(Takim a, string AD)
    {
        takim = a;
        TakimSec_SERVER_RPC(a, AD);
    }

    [ServerRpc]
    public void TakimSec_SERVER_RPC(Takim a, string AD)
    {
        TakimSec_CLIENT_RPC(a, AD);
    }

    [ObserversRpc]
    public void TakimSec_CLIENT_RPC(Takim a, string AD)
    {
        takim = a;
        oyuncuADI = AD;
        // UI da takım şeysilerini arttır
        if (base.IsOwner){OyuncuCanvas.setA_B_TakimSayisi(a);}
        else{FindObjectOfType<CanvasOyuncu>().setA_B_TakimSayisi(a);}
    }

    [ObserversRpc]
    public void SyncTakimNumaralari_veAdlari(int ta, int tb, string AD)
    {
        oyuncuADI = AD;
        if (base.IsOwner){OyuncuCanvas.setA_B_TakimSayisi(ta,tb);}
        else{FindObjectOfType<CanvasOyuncu>().setA_B_TakimSayisi(ta,tb);}
    }

    // her kare(FPS) çalışan fonksiyon
    void Update()
    {
        // bu oyuncuyu kontrol etmiyorsak geri dön.
        if (!base.IsOwner){return;}
        if (!OyunBasladi){return;}

        // ölmüşüz freecam takıl.
        if(spectator)
        {
            oyuncuVeKameraDondur(Input.GetAxis("Mouse X") * kameraHassasiyet, Input.GetAxis("Mouse Y") * kameraHassasiyet);
            Hareket = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
            transform.position += transform.TransformDirection(Hareket * Time.deltaTime * oyuncuHiz);
            if (Input.GetKey(KeyCode.Space))
            {
                // uç
                transform.Translate(Vector3.up * Time.deltaTime * oyuncuHiz * 5f);
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // alçal
                transform.Translate(Vector3.down * Time.deltaTime * oyuncuHiz * 5f);
            }
            return;
        }

        // w-a-s-d kontrolünü vektör'e kaydet
        Hareket = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));

        // animasyonlar
        Animasyon(Hareket == Vector3.zero);

        // mouse ile oyuncuyu döndür
        oyuncuVeKameraDondur(Input.GetAxis("Mouse X") * kameraHassasiyet, Input.GetAxis("Mouse Y") * kameraHassasiyet);
    
        // yer cekimi uygula ve mümkün ise zıpla
        YerCekimiUygula(Input.GetKeyDown(KeyCode.Space));

        // oyuncuyu Delta Zamana göre hareket ettir
        OyuncuKontrol.Move(transform.TransformDirection(Hareket * Time.deltaTime * oyuncuHiz));

        // raycast ile yerden eşya üzerine gelince ekranda adının falan çıkması 
        RaycastEsyaKontrolEt();

        // E ye basarsan, Envaterde boş yer varsa, Ve alabileceğin eşya var ise al
        if (Input.GetKeyDown(KeyCode.E)){
            if (RaycastObjesi != null && !envater.EnvaterDoluMu()){

                //almaya çalıştığımız mermi
                ObjeTipi tip = RaycastObjesi.ObjeTipiEnum();
                if (tip == ObjeTipi.Mermi)
                {
                    // elimizde silah varsa mermi olarak ekle yoksa.. birşey yapma
                    if (ElimizdekiObje != null){
                        if (ElimizdekiObje.ObjeTipiEnum() == ObjeTipi.Silah)
                        {
                            ElimizdekiObje.MiktarEkle(RaycastObjesi.GetAmount());
                            OyuncuCanvas.MermiSayisi(ElimizdekiObje.GetAmount());
                            // yerdeki mermiyi kullandık, bunu servere ilet.
                            EsyaYokEt_SERVER_RPC(RaycastObjesi.GetGameObject());
                        }
                    }
                }
                else if (tip == ObjeTipi.SaglikKiti){
                    // adamımıza CAN ekle sonra objeyi yok et
                    can += RaycastObjesi.GetAmount();
                    OyuncuCanvas.CanBari(can);
                    EsyaYokEt_SERVER_RPC(RaycastObjesi.GetGameObject());
                }
                else{
                    // elimize aldığımız obje herhangi birşey
                    RaycastObjesi.KullanEventRegister(this);
                    EsyayiEnvatereEkle();
                    if (ElimizdekiObje != null){
                        OyuncuCanvas.MermiSayisi(ElimizdekiObje.GetAmount());
                    }
                }
            }
        }

        // Elinde eşya varsa ve ateş tuşuna basarsan eşyayı kullan
        //if (Input.GetMouseButtonDown(0)){
        if (Input.GetMouseButton(0)){
            if (ElimizdekiObje != null){
                ElimizdekiObje.Kullan();
                // elimizdeki biterse bug olur (duvar silincek envaterden)
                OyuncuCanvas.MermiSayisi(ElimizdekiObje.GetAmount());
            }
        }

        // sağ tıklayınca silah zoom yap
        if (Input.GetMouseButtonDown(1)){
            if (ElimizdekiObje != null){
                ElimizdekiObje.Sagtikla();
            }
        }

        // hassasiyet arttır-azalt
        if(Input.GetKeyDown(KeyCode.O))
        {
            kameraHassasiyet -= 1;
            kameraHassasiyet = Mathf.Clamp(kameraHassasiyet, 1, 20);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            kameraHassasiyet += 1;
            kameraHassasiyet = Mathf.Clamp(kameraHassasiyet, 1, 20);
        }

        // hassasiyet sıfırla
        if(Input.GetKeyDown(KeyCode.L))
        {
            kameraHassasiyet = 5;
        }

        // Mouse tekerini döndürerek eşya değiştir
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0){
            envater.MouseTekerlegiCallback(scroll);
            if (ElimizdekiObje != null){OyuncuCanvas.MermiSayisi(ElimizdekiObje.GetAmount());}
            else{OyuncuCanvas.MermiSayisi(0);}
        }

        // Elimizdeki eşyayı Q ile yere at.
        if (Input.GetKeyDown(KeyCode.Q)){
            if (ElimizdekiObje != null){
                ElimizdekiObje.KullanEventUnRegister();
                EsyayiYereAt();
                if (ElimizdekiObje != null){OyuncuCanvas.MermiSayisi(ElimizdekiObje.GetAmount());}
                else{OyuncuCanvas.MermiSayisi(0);}
            }
        }
        
    }

    [ServerRpc]
    public void Animasyon(bool r)
    {
        if (r)
        {
            animator.ResetTrigger("run"); animator.SetTrigger("stand");
        }else
        {
            animator.ResetTrigger("stand"); animator.SetTrigger("run");
        }
    }

    [ObserversRpc]
    public void SetPos(Vector3 pos, Takim takim = Takim.SECMEDI)
    {
        if (takim != Takim.SECMEDI)
        {
            // oyuncu rengini ayarla
            if (takim == Takim.A){skin.material = takim_a_rengi;}
            else{skin.material = takim_b_rengi;}
        }
        OyuncuKontrol.enabled = false;
        transform.position = pos;
        OyuncuKontrol.enabled = true;
    }


    [ServerRpc]
    public void EsyaYokEt_SERVER_RPC(GameObject obj)
    {
        EsyaYokEt_CLIENT_RPC(obj);
    }

    [ObserversRpc]
    public void EsyaYokEt_CLIENT_RPC(GameObject obj)
    {
        obj.GetComponent<EtkilesimliObje>().SetMeshAndCollider(false);
    }

    public void EsyayiYereAt()
    {
        // Önce Eşyayı koycak zemin bul
        RaycastHit hit;
        Debug.DrawRay(OyuncuKamera.position, OyuncuKamera.forward * 100f, Color.red, 50f);
        if (!Physics.Raycast(OyuncuKamera.position, OyuncuKamera.forward, out hit, 20f)){return;} // eşyayı atacak yer yok.. şimdilik fail durumu yok, geri dön

        // eşyayı yere yerleştir, Parent'i çıkar
        ElimizdekiObje.UnParentTransform();
        ElimizdekiObje.SetPos(hit.point);
        ElimizdekiObje.GetTransform().rotation = Quaternion.identity; // yeni
        ElimizdekiObje.ToggleCollider(true);

        // sunucuya ilet
        EsyayiYereAt_SERVER_RPC(ElimizdekiObje.GetGameObject(), hit.point);

        // seçili eşyayı envaterden çıkar
        int new_index = envater.EsyaSil();

        // yeni eşyayı kuşan
        EsyayiKusan(new_index);
    }

    [ServerRpc]
    public void EsyayiYereAt_SERVER_RPC(GameObject obj, Vector3 pos)
    {
        EsyayiYereAt_CLIENT_RPC(obj, pos);
    }

    [ObserversRpc]
    public void EsyayiYereAt_CLIENT_RPC(GameObject obj, Vector3 pos)
    {
        // yere atılan nesneyi görünür yap ve yerini ayarla
        obj.GetComponent<EtkilesimliObje>().SetMeshAndCollider(true);
        obj.transform.SetParent(null);
        obj.GetComponent<Collider>().enabled = true;
        obj.transform.position = pos;
        obj.transform.rotation = Quaternion.identity; // yeni

        // elimizdeki atmış olduğumuz nesneyi bırak
        // 1 SATIRLIK BUG FIX 
        ElimizdekiObje = null;
    }

    // Raycast ile bakmakta olduğumuz objeyi envatere ekle (bug free %100)
    public void EsyayiEnvatereEkle()
    {
        GameObject obje = RaycastObjesi.GetGameObject();

        // esyayi görünmez yap
        RaycastObjesi.SetMeshAndCollider(false);

        // eşyayı elimizin oraya koy
        obje.transform.SetParent(EsyaTutmaPozisyonu.transform);
        obje.transform.localPosition = Vector3.zero;
        obje.transform.localRotation = Quaternion.identity;

        // collider'ı kapat
        RaycastObjesi.ToggleCollider(false);

        // Eşyayı envatere eklediğimizi server'e ilet (diğer pclerde eşya görünmez olucak ve kişinin koluna yerleşicek)
        EsyayiEnvatereEkle_SERVER_RPC(obje);

        // envatere ekle
        int yeni_index = envater.EsyaEkle(obje);
        if (yeni_index != -1)
        {
            // hiç eşya yoktu ve ilk eşyayı ekledik. o yüzden default onu kuşanmamız lazım.
            EsyayiKusan(yeni_index);
        } 
    }

    // envaterimizdeki herhangi eşyayi index ile elimize al
    public void EsyayiKusan(int index)
    {        
        GameObject obje = envater.EsyaAl(index);
        if (obje != null){

            // önceki eşya varsa bırak
            if (ElimizdekiObje != null)
            {
                ElimizdekiObje.SetMeshAndCollider(false);
            }

            // yenisini kuşan
            ElimizdekiObje = obje.GetComponent<EtkilesimliObje>();
            ElimizdekiObje.SetMeshAndCollider(true);
            ElimizdekiObje.ToggleCollider(false);     // item elimizdeyken collider'ı olmamalı

            EsyayiKusan_SERVER_RPC(obje);
        }else{
            // kuşanmaya çalıştığımız eşya yok ise ve envater boş ise, elimizdeki obje şeysini null yap
            if (envater.secili_esya == -1)
            {
                ElimizdekiObje = null;
            }
        }
    }

    [ServerRpc]
    public void EsyayiKusan_SERVER_RPC(GameObject obje)
    {
        EsyayiKusan_CLIENT_RPC(obje);
    }

    [ObserversRpc]
    public void EsyayiKusan_CLIENT_RPC(GameObject obje)
    {
        // önceki eşya varsa bırak
        if (ElimizdekiObje != null)
        {
            ElimizdekiObje.SetMeshAndCollider(false);
        }
        // yenisini kuşan
        ElimizdekiObje = obje.GetComponent<EtkilesimliObje>();
        ElimizdekiObje.SetMeshAndCollider(true);
        ElimizdekiObje.ToggleCollider(false); // obje eldeyken collider yok
    }

    [ServerRpc]
    public void EsyayiEnvatereEkle_SERVER_RPC(GameObject obje)
    {
        EsyayiEnvatereEkle_CLIENT_RPC(obje);
    }

    [ObserversRpc]
    public void EsyayiEnvatereEkle_CLIENT_RPC(GameObject obje)
    {
        // eşyayi görünmez yap.
        obje.GetComponent<EtkilesimliObje>().SetMeshAndCollider(false);

        // eşyayı elimizin oraya koy
        obje.transform.SetParent(EsyaTutmaPozisyonu.transform);
        obje.transform.localPosition = Vector3.zero;
        obje.transform.localRotation = Quaternion.identity;

        // collider'ı kapat
        obje.GetComponent<Collider>().enabled = false;
    }

    public void RaycastEsyaKontrolEt()
    {
        RaycastHit hit;
        Debug.DrawRay(OyuncuKamera.position, OyuncuKamera.forward * 100f, Color.red);
        if (Physics.Raycast(OyuncuKamera.position, OyuncuKamera.forward, out hit, 20f)){
            if (hit.transform.TryGetComponent<EtkilesimliObje>(out EtkilesimliObje obje))
            {
                if (!obje.Equals(RaycastObjesi)){
                    RaycastObjesi = obje;
                    OyuncuCanvas.RaycastEsyaBulundu(obje.ObjeIsmi());
                }
            }
            else if(RaycastObjesi != null)
            {
                RaycastObjesi = null;
                OyuncuCanvas.RaycastEsyaBirakildi();
            }
        }
        else if (RaycastObjesi != null)
        {
            RaycastObjesi = null;
            OyuncuCanvas.RaycastEsyaBirakildi();
        }
    }


    // mouse sağ-sol yaparak karekteri döndür
    // mouse youkarı-aşağı ile kamerayı döndür
    public void oyuncuVeKameraDondur(float x, float y)
    {
        transform.Rotate(Vector3.up * x);

        Kamera_Y_Aksis += y;
        Kamera_Y_Aksis = Mathf.Clamp(Kamera_Y_Aksis, -90, 90);
        OyuncuKamera.localRotation = Quaternion.Euler(-Kamera_Y_Aksis, 0f, 0f);
        //OyuncuKamera.localRotation = Quaternion.Lerp(OyuncuKamera.localRotation, Quaternion.Euler(-Kamera_Y_Aksis, 0f, 0f), time * kameraHassasiyet);
        //time = time + Time.deltaTime;

        EsyaDondurmeAksisi.localRotation = Quaternion.Euler(-Kamera_Y_Aksis, 0f, 0f);
    }


    // yer çekimi (veya gök itimi)
    public void YerCekimiUygula(bool zipla)
    {
        bool yerde = KarakterYerdeMi();
        if (yerde) { YerCekimi = 0f; }

        if (zipla && yerde){
            YerCekimi += Mathf.Sqrt(ziplamaGucu * 3.0f * yerCekimiGucu);
        }

        YerCekimi -= yerCekimiGucu * Time.deltaTime;
        OyuncuKontrol.Move(transform.TransformDirection(Vector3.up * YerCekimi * Time.deltaTime));
    }

    // unity bunun için niye default bir çözüm vermiyo 
    // isGround çok kötü 
    public bool KarakterYerdeMi()
    {
        return Physics.Raycast(YerPozisyonu.position, Vector3.down, 0.1f);
    }

    // ateş et
    public void AtesEt(int hasar)
    {
        AtesEt_SERVER_RPC(OyuncuKamera.position, OyuncuKamera.TransformDirection(Vector3.forward), hasar);
    }

    [ServerRpc]
    public void AtesEt_SERVER_RPC(Vector3 pos, Vector3 rotation, int hasar)
    {
        // önce herkese ateş etmenin grafik kısmını göster
        TrailCiz();

        // raycast
        RaycastHit hit;
        Debug.DrawRay(pos, rotation * 100f, Color.yellow, 20f);
        if (Physics.Raycast(pos, rotation, out hit, 100f)){
            if (hit.transform.TryGetComponent<Oyuncu>(out Oyuncu oyuncu)){

                // vurduğumuz adam aynı takımda olabilir
                // ama csgoda vurabiliyoz o yüzden eklemedim.

                // kendimizi vurmuş olabiliriz
                if(oyuncu.oyuncuADI == oyuncuADI){return;} 

                // vurulduğunu ilet.
                oyuncu.can -= hasar;
                if (oyuncu.can <= 0)
                {
                    // ölen kişiye öldüğünü göster
                    oyuncu.BaskasiTarafindanOldurulduk(oyuncuADI);
                    oyuncu.spectator = true;

                    // vuran kişiye adamı öldürdüğünü söyle
                    BiriniOldurdun(oyuncu.oyuncuADI);

                    // maç bitti mi kontrol et
                    takimPozisyonlayici.MacBittiMi();
                    
                }else{
                    // kişiye vurulduğunu göster
                    oyuncu.BaskasiTarafindanVurulduk(oyuncu.can, oyuncuADI);
                }

            }
        }
    }

    [ObserversRpc]
    public void BuMacBitti(Vector3 spawnPoint, Takim kazanan_takim)
    {
        // görünür ol, ve yerine ışınlan.
        skin.enabled = true;
        OyuncuKontrol.enabled = false;
        transform.position = spawnPoint;
        OyuncuKontrol.enabled = true;
        spectator = false;
        can = 100;

        // oyundaki owner hariç diğerleride silahları ellerinden çıkarmalı.
        for (int i=0; i<EsyaTutmaPozisyonu.childCount; i++)
        {
            print("silindi");
            EsyaTutmaPozisyonu.GetChild(i).GetComponent<EtkilesimliObje>().SetPosSpawn();
        }

        ElimizdekiObje = null;

        // ekranda kazanan takımı göster
        if (base.IsOwner)
        {
            // envaterimizi temizle
            /*foreach(GameObject x in envater.envater)
            {
                x.GetComponent<EtkilesimliObje>().SetPosSpawn();
            }*/

            Camera.main.fieldOfView = 60;

            envater.Temizle();
            ElimizdekiObje = null;

            OyuncuCanvas.EnvaterKismiSifirla();
            OyuncuCanvas.CanBari(can);
            OyuncuCanvas.MacKazanildiYaziSpawnla(kazanan_takim);

            SilahPosSifirla_SERVER_RPC();
        }
    }

    [ServerRpc]
    public void SilahPosSifirla_SERVER_RPC()
    {
        SilahPosSifirla_CLIENT_RPC();
    }

    [ObserversRpc]
    public void SilahPosSifirla_CLIENT_RPC()
    {
        for (int i=0; i<EsyaTutmaPozisyonu.childCount; i++)
        {
            EsyaTutmaPozisyonu.GetChild(i).GetComponent<EtkilesimliObje>().SetPosSpawn();
        }
    }

    [ObserversRpc]
    public void TekSilahinYeriniSifirla_CLIENT_RPC(GameObject obj)
    {
        obj.GetComponent<EtkilesimliObje>().SetPosSpawn();
    }

    [ObserversRpc]
    public void BiriniOldurdun(string OldurdugunKisi)
    {
        // Birini öldürdük, kill feed'e ekle
        if (!base.IsOwner){return;}
        OyuncuCanvas.killYaziSpawnla(oyuncuADI, OldurdugunKisi);
    }

    // observer rpc owner içinde çağırılıyo btw aynı şeyi iki kere yapma ha
    [ObserversRpc]
    public void TrailCiz()
    {
        if(base.IsOwner){Instantiate(lineObject, silah_raycast_noktasi.position, EsyaTutmaPozisyonu.rotation);}
        else{Instantiate(lineObject, EsyaTutmaPozisyonu.position, EsyaTutmaPozisyonu.rotation);}
    }

    [ObserversRpc]
    public void BaskasiTarafindanVurulduk(int yeni_can, string ad)
    {
        // biri bizi vurdu
        if (!base.IsOwner){return;}

        // ekrandaki can barını göster
        OyuncuCanvas.CanBari(yeni_can);
    }

    [ObserversRpc]
    public void BaskasiTarafindanOldurulduk(string ad)
    {
        // biri bizi öldürdü
        // ölme yazisi (ownere ait sadece)
        if (base.IsOwner){OyuncuCanvas.killYaziSpawnla(ad, oyuncuADI);OyuncuCanvas.CanBari(0);OyuncuCanvas.OldunuzYazisiSpawnla();}
        
        // karakteri gizli (herkeste)
        spectator = true;
        OyuncuKontrol.enabled = false;
        skin.enabled = false;
    }
    
}
