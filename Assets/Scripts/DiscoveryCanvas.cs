using System.Collections;
using System.Collections.Generic;
using FishNet.Discovery;
using UnityEngine;
using System.Net;
using FishNet;
using GameKit.Dependencies.Utilities;

public class DiscoveryCanvas : MonoBehaviour
{
    [SerializeField] CanvasOyuncu canvasOyuncu;
    [SerializeField] NetworkDiscovery discovery;
    [SerializeField] GameObject buton;
    [SerializeField] Transform sunucuListe;
    List<string> ipAdresleri = new List<string>();
    void Start()
    {
        discovery.ServerFoundCallback += SunucuBulundu;
    }

    // Update is called once per frame
    void SunucuBulundu(IPEndPoint endpoint)
    {
        foreach(string adres in ipAdresleri)
        {
            if (adres == endpoint.Address.ToString()){return;}
        }
        
        ipAdresleri.Add(endpoint.Address.ToString());
        GameObject x = Instantiate(buton);
        x.transform.SetParent(sunucuListe);
        //x.transform.position = Vector3.zero;
        //x.transform.localPosition = Vector3.zero;
        x.transform.SetScale(new Vector3(1,1,1));

        SunucuButon y = x.GetComponent<SunucuButon>();
        y.sunucu_Adi.text = endpoint.Address.ToString();
        var temp = endpoint;
        y.katil_buton.onClick.AddListener(() => SunucuKatil(temp));
    }

    void SunucuKatil(IPEndPoint a)
    {
        if( StaticVariables.singleton.AdHazir()){return;}
        discovery.StopSearchingOrAdvertising();
        InstanceFinder.ClientManager.StartConnection(a.Address.ToString());
        canvasOyuncu.OyunModu();
    }
}
