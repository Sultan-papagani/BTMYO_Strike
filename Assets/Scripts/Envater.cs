using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Envater : MonoBehaviour
{
    public Oyuncu oyuncu;
    public CanvasOyuncu OyuncuCanvas; 
    public List<GameObject> envater;
    int max = 3;
    public int secili_esya = -1; // -1 ise eşya yok

    // envaterin dolu olup olmadığı
    public bool EnvaterDoluMu()
    {
        return envater.Count >= max;
    }

    // Enavterdeki eşyaları ekrana çizmek için
    public void updateCanvasItemsState(){
        List<string> isimler = new List<string>();
        foreach(GameObject item in envater){
            isimler.Add(item.GetComponent<EtkilesimliObje>().ObjeIsmi());
        }
        OyuncuCanvas.changeState(isimler);
    }

    // Mouse tekerleğini çevirdikçe envaterden eşya seç
    public void MouseTekerlegiCallback(float scroll)
    {
        if (scroll > 0){
            // yukarı
            if (secili_esya + 1 > envater.Count - 1 || secili_esya + 1 > max){
                if (envater.Count <= 0){
                    secili_esya = -1;
                }else{
                secili_esya = 0;
                }
            }
            else{secili_esya++;}
        }else{
            // aşağı
            if (secili_esya - 1 < 0){
                if (envater.Count <= 0){
                    // envater boş ise
                    secili_esya = -1;
                }
                else{
                secili_esya = envater.Count - 1;
                }
            }
            else{secili_esya--;}
        }

        // secili eşya index değişti, karakterde bu etkiyi göster
        OyuncuCanvas.SecimIndex(secili_esya);
        oyuncu.EsyayiKusan(secili_esya);
    }

    // Envatere eşya ekle
    public int EsyaEkle(GameObject esya)
    {
        int eski_sayi = envater.Count;
        envater.Add(esya);
        if (eski_sayi <= 0){
            secili_esya = 0; // hiç eşya yoktuysa ve yeni eşya eklendiyse seçili eşyayı 0. yap
            updateCanvasItemsState();
            OyuncuCanvas.SecimIndex(secili_esya); // ekranda seçilen eşya indexini değiştir
            return secili_esya;
        }
        updateCanvasItemsState();
        return -1; // secili index değişmedi. 
    }

    // Envaterden eşya sil
    public int EsyaSil(int index = -1)
    {
        if (index == -1){
            index = secili_esya;
        }
        envater.RemoveAt(index);
        secili_esya = envater.Count - 1;
        if (secili_esya < 0){secili_esya = -1;} // aslında bu satıra gerek yok

        updateCanvasItemsState();
        OyuncuCanvas.SecimIndex(secili_esya); // ekranda seçilen eşya indexini değiştir
        return secili_esya;
    }

    // Index ile envaterden eşya al
    public GameObject EsyaAl(int index)
    {
        if (index > max || index < 0 || index > envater.Count - 1){return null;}
        return envater[index];
    }

    // Sıfırla
    public void Temizle()
    {
        envater.Clear();
        secili_esya = -1;
    }

}
