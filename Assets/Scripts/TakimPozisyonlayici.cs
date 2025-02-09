using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Discovery;
using FishNet.Managing;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;


public enum Takim
{
    A,
    B,
    SECMEDI
}

public class TakimPozisyonlayici : NetworkBehaviour
{
    [SerializeField] CanvasOyuncu oyuncuCanvas;
    public List<Transform> Takim_A;
    public List<Transform> Takim_B;

    NetworkObject yeni_oyuncu = null;

    [SerializeField] int A_TAKIM_SAYISI = 0, B_TAKIM_SAYISI = 0;
    [SerializeField] List<Oyuncu> oyuncuListesi = new List<Oyuncu>();
    List<Vector3> silahlar_a_pos = new List<Vector3>();
    List<Vector3> silahlar_b_pos = new List<Vector3>();
    [SerializeField] List<GameObject> silahlar_a = new List<GameObject>();
    [SerializeField] List<GameObject> silahlar_b = new List<GameObject>();


    // SERVER
    public void MacBittiMi()
    {
        int takima = A_TAKIM_SAYISI;
        int takimb = B_TAKIM_SAYISI;

        print("A: "+takima);
        print("B: "+takimb);


        // hayatta olanları say
        foreach(Oyuncu x in oyuncuListesi){
            if (x.takim == Takim.A){
                if (x.spectator){takima--;}
            }else{
                if (x.spectator){takimb--;}
            }
        }

        print("A sayimi: "+takima);
        print("B sayimi: "+takimb);

        int a=0, b=0;
        if (takima <= 0 || takimb <= 0){
            print("MAC BITTI");
            // maç bitti
            Takim kazanan = takima <= 0 ? Takim.A : Takim.B;
            foreach(Oyuncu x in oyuncuListesi)
            {
                // herkesi maçın bitmesinden haberdar et
                Vector3 pos;
                if (x.takim == Takim.A){pos = Takim_A[a].position; a++;}
                else{pos = Takim_B[b].position; b++;}
                x.BuMacBitti(pos, kazanan);
            }
            SILAHLARI_SIFIRLA_SERVER_RPC();
        }else{
            // DAHA KAZANAN YOK
            return;
        }
        return;
    }

    // bütün oyuncular silahları sıfırlıyor.
    [ObserversRpc]
    public void SILAHLARI_SIFIRLA_SERVER_RPC()
    {
        foreach(GameObject sa in silahlar_a)
        {
            sa.GetComponent<EtkilesimliObje>().SetPosSpawn();
        }

        foreach(GameObject sb in silahlar_a)
        {
            sb.GetComponent<EtkilesimliObje>().SetPosSpawn();
        }
    }

    public void Start()
    {
        InstanceFinder.ServerManager.GetComponent<PlayerSpawner>().OnSpawned += OnClientJoin;
    }

    private void OnClientJoin(NetworkObject new_one)
    {
        yeni_oyuncu = new_one;
        // birazcık gecikme ver
        Invoke(nameof(OnClientJoin_delayed), 1f);
    }

    public void OnClientJoin_delayed()
    {
        print("yeni biri geldi");

        // yeni biri oyuna girdi
        GameObject[] oyuncular = GameObject.FindGameObjectsWithTag("Player");
        int takima = 0, takimb = 0;
        foreach(GameObject obj in oyuncular)
        {
            if (obj.TryGetComponent<Oyuncu>(out Oyuncu oyuncu))
            {
                if (oyuncu.takim == Takim.A)
                {
                    takima++;
                }else if (oyuncu.takim == Takim.B)
                {
                    takimb++;
                }
            }
        }

        // yeni gelen kişiye takim kişi sayilarini syncle
        if(yeni_oyuncu.TryGetComponent<Oyuncu>(out Oyuncu o))
        {
            o.SyncTakimNumaralari_veAdlari(takima, takimb, o.oyuncuADI);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakimlariBaslangicaDiz()
    {
        // sunucudaki herkesi optimize(!) bir şekilde bul
        GameObject[] oyuncular = GameObject.FindGameObjectsWithTag("Player");

        // takımını kontrol et, yerleştir ve kontrolü ver
        int takima = 0, takimb = 0;
        foreach(GameObject obj in oyuncular)
        {
            if (obj.TryGetComponent<Oyuncu>(out Oyuncu oyuncu))
            {
                oyuncuListesi.Add(oyuncu);
                oyuncu.KarakteriEtkinlestir(true);

                if (oyuncu.takim == Takim.A && takima < 20)
                {
                    oyuncu.SetPos(Takim_A[takima].position, Takim.A);
                    takima++;
                }else if (oyuncu.takim == Takim.B && takimb < 20)
                {
                    oyuncu.SetPos(Takim_B[takimb].position, Takim.B);
                    takimb++;
                }else
                {
                    return;
                }
            }
        }

        A_TAKIM_SAYISI = takima;
        B_TAKIM_SAYISI = takimb;

        // UI ları başlat
        TakimEkraniniSilVeBaslat();

    }

    [ObserversRpc]
    public void TakimEkraniniSilVeBaslat()
    {
        oyuncuCanvas.TakimPaneliniKapat();

    }
}
