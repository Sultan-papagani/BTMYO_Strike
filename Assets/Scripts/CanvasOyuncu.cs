using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using JetBrains.Annotations;
using GameKit.Dependencies.Utilities;
using FishNet.Discovery;

public class CanvasOyuncu : MonoBehaviour
{
    [SerializeField] GameObject OldunuzYazisi;
    [SerializeField] GameObject MacKazanildiYaziPrefab;
    [SerializeField] GameObject killFeedPrefab;
    [SerializeField] Transform killFeedObj;
    [SerializeField] TextMeshProUGUI hata_yazisi;
    [SerializeField] TMP_InputField ad_giris;
    [SerializeField] GameObject OyuncuListePanel;
    [SerializeField] GameObject A_TAKIM_BUTONU, B_TAKIM_BUTONU;
    [SerializeField] TextMeshProUGUI A_TAKIM, B_TAKIM;
    [SerializeField] Button MaciBaslatTusu;
    [SerializeField] GameObject OyunPanel;
    [SerializeField] GameObject LobiPanel;
    [SerializeField] Transform RaycastEsyaYaziBlogu;
    [SerializeField] TextMeshProUGUI esyaYazisi;
    [SerializeField] TextMeshProUGUI esyaMermiSayisi;
    [SerializeField] List<Sprite> fotolar;
    [SerializeField] List<string> isimler;
    Dictionary<string, Sprite> esyalar = new Dictionary<string, Sprite>();
    [SerializeField] List<Image> slotlar = new List<Image>();
    [SerializeField] List<GameObject> slotlar_secimler = new List<GameObject>();
    [SerializeField] Sprite EsyaYokfotosu;
    [SerializeField] Slider canbari;
    [SerializeField] TextMeshProUGUI mermi_sayisi;
    public Oyuncu owner_oyuncu = null;
    int takim_a = 0, takim_b = 0;

    string[] yasak_kelimeler = {"abaza","abazan","ag","a\u011fz\u0131na s\u0131\u00e7ay\u0131m","ahmak","allah","allahs\u0131z","am","amar\u0131m","ambiti","am biti","amc\u0131\u011f\u0131","amc\u0131\u011f\u0131n","amc\u0131\u011f\u0131n\u0131","amc\u0131\u011f\u0131n\u0131z\u0131","amc\u0131k","amc\u0131k ho\u015faf\u0131","amc\u0131klama","amc\u0131kland\u0131","amcik","amck","amckl","amcklama","amcklaryla","amckta","amcktan","amcuk","am\u0131k","am\u0131na","am\u0131nako","am\u0131na koy","am\u0131na koyar\u0131m","am\u0131na koyay\u0131m","am\u0131nakoyim","am\u0131na koyyim","am\u0131na s","am\u0131na sikem","am\u0131na sokam","am\u0131n feryad\u0131","am\u0131n\u0131","am\u0131n\u0131 s","am\u0131n oglu","am\u0131no\u011flu","am\u0131n o\u011flu","am\u0131s\u0131na","am\u0131s\u0131n\u0131","amina","amina g","amina k","aminako","aminakoyarim","amina koyarim","amina koyay\u0131m","amina koyayim","aminakoyim","aminda","amindan","amindayken","amini","aminiyarraaniskiim","aminoglu","amin oglu","amiyum","amk","amkafa","amk \u00e7ocu\u011fu","amlarnzn","aml\u0131","amm","ammak","ammna","amn","amna","amnda","amndaki","amngtn","amnn","amona","amq","ams\u0131z","amsiz","amsz","amteri","amugaa","amu\u011fa","amuna","ana","anaaann","anal","analarn","anam","anamla","anan","anana","anandan","anan\u0131","anan\u0131","anan\u0131n","anan\u0131n am","anan\u0131n am\u0131","anan\u0131n d\u00f6l\u00fc","anan\u0131nki","anan\u0131sikerim","anan\u0131 sikerim","anan\u0131sikeyim","anan\u0131 sikeyim","anan\u0131z\u0131n","anan\u0131z\u0131n am","anani","ananin","ananisikerim","anani sikerim","ananisikeyim","anani sikeyim","anann","ananz","anas","anas\u0131n\u0131","anas\u0131n\u0131n am","anas\u0131 orospu","anasi","anasinin","anay","anayin","angut","anneni","annenin","annesiz","anuna","aptal","aq","a.q","a.q.","aq.","ass","atkafas\u0131","atm\u0131k","att\u0131rd\u0131\u011f\u0131m","attrrm","auzlu","avrat","ayklarmalrmsikerim","azd\u0131m","azd\u0131r","azd\u0131r\u0131c\u0131","babaannesi ka\u015far","baban\u0131","baban\u0131n","babani","babas\u0131 pezevenk","baca\u011f\u0131na s\u0131\u00e7ay\u0131m","bac\u0131na","bac\u0131n\u0131","bac\u0131n\u0131n","bacini","bacn","bacndan","bacy","bastard","basur","beyinsiz","b\u0131z\u0131r","bitch","biting","bok","boka","bokbok","bok\u00e7a","bokhu","bokkkumu","boklar","boktan","boku","bokubokuna","bokum","bombok","boner","bosalmak","bo\u015falmak","cenabet","cibiliyetsiz","cibilliyetini","cibilliyetsiz","cif","cikar","cim","\u00e7\u00fck","dalaks\u0131z","dallama","daltassak","dalyarak","dalyarrak","dangalak","dassagi","diktim","dildo","dingil","dingilini","dinsiz","dkerim","domal","domalan","domald\u0131","domald\u0131n","domal\u0131k","domal\u0131yor","domalmak","domalm\u0131\u015f","domals\u0131n","domalt","domaltarak","domalt\u0131p","domalt\u0131r","domalt\u0131r\u0131m","domaltip","domaltmak","d\u00f6l\u00fc","d\u00f6nek","d\u00fcd\u00fck","eben","ebeni","ebenin","ebeninki","ebleh","ecdad\u0131n\u0131","ecdadini","embesil","emi","fahise","fahi\u015fe","feri\u015ftah","ferre","fuck","fucker","fuckin","fucking","gavad","gavat","geber","geberik","gebermek","gebermi\u015f","gebertir","ger\u0131zekal\u0131","gerizekal\u0131","gerizekali","gerzek","giberim","giberler","gibis","gibi\u015f","gibmek","gibtiler","goddamn","godo\u015f","godumun","gotelek","gotlalesi","gotlu","gotten","gotundeki","gotunden","gotune","gotunu","gotveren","goyiim","goyum","goyuyim","goyyim","g\u00f6t","g\u00f6t deli\u011fi","g\u00f6telek","g\u00f6t herif","g\u00f6tlalesi","g\u00f6tlek","g\u00f6to\u011flan\u0131","g\u00f6t o\u011flan\u0131","g\u00f6to\u015f","g\u00f6tten","g\u00f6t\u00fc","g\u00f6t\u00fcn","g\u00f6t\u00fcne","g\u00f6t\u00fcnekoyim","g\u00f6t\u00fcne koyim","g\u00f6t\u00fcn\u00fc","g\u00f6tveren","g\u00f6t veren","g\u00f6t verir","gtelek","gtn","gtnde","gtnden","gtne","gtten","gtveren","hasiktir","hassikome","hassiktir","has siktir","hassittir","haysiyetsiz","hayvan herif","ho\u015faf\u0131","h\u00f6d\u00fck","hsktr","huur","\u0131bnel\u0131k","ibina","ibine","ibinenin","ibne","ibnedir","ibneleri","ibnelik","ibnelri","ibneni","ibnenin","ibnerator","ibnesi","idiot","idiyot","imansz","ipne","iserim","i\u015ferim","ito\u011flu it","kafam girsin","kafas\u0131z","kafasiz","kahpe","kahpenin","kahpenin feryad\u0131","kaka","kaltak","kanc\u0131k","kancik","kappe","karhane","ka\u015far","kavat","kavatn","kaypak","kayyum","kerane","kerhane","kerhanelerde","kevase","keva\u015fe","kevvase","koca g\u00f6t","kodu\u011fmun","kodu\u011fmunun","kodumun","kodumunun","koduumun","koyarm","koyay\u0131m","koyiim","koyiiym","koyim","koyum","koyyim","krar","kukudaym","laciye boyad\u0131m","lavuk","libo\u015f","madafaka","mal","malafat","malak","manyak","mcik","meme","memelerini","mezveleli","minaamc\u0131k","mincikliyim","mna","monakkoluyum","motherfucker","mudik","oc","ocuu","ocuun","O\u00c7","o\u00e7","o. \u00e7ocu\u011fu","o\u011flan","o\u011flanc\u0131","o\u011flu it","orosbucocuu","orospu","orospucocugu","orospu cocugu","orospu \u00e7oc","orospu\u00e7ocu\u011fu","orospu \u00e7ocu\u011fu","orospu \u00e7ocu\u011fudur","orospu \u00e7ocuklar\u0131","orospudur","orospular","orospunun","orospunun evlad\u0131","orospuydu","orospuyuz","orostoban","orostopol","orrospu","oruspu","oruspu\u00e7ocu\u011fu","oruspu \u00e7ocu\u011fu","osbir","ossurduum","ossurmak","ossuruk","osur","osurduu","osuruk","osururum","otuzbir","\u00f6k\u00fcz","\u00f6\u015fex","patlak zar","penis","pezevek","pezeven","pezeveng","pezevengi","pezevengin evlad\u0131","pezevenk","pezo","pic","pici","picler","pi\u00e7","pi\u00e7in o\u011flu","pi\u00e7 kurusu","pi\u00e7ler","pipi","pipi\u015f","pisliktir","porno","pussy","pu\u015ft","pu\u015fttur","rahminde","revizyonist","s1kerim","s1kerm","s1krm","sakso","saksofon","salaak","salak","saxo","sekis","serefsiz","sevgi koyar\u0131m","sevi\u015felim","sexs","s\u0131\u00e7ar\u0131m","s\u0131\u00e7t\u0131\u011f\u0131m","s\u0131ecem","sicarsin","sie","sik","sikdi","sikdi\u011fim","sike","sikecem","sikem","siken","sikenin","siker","sikerim","sikerler","sikersin","sikertir","sikertmek","sikesen","sikesicenin","sikey","sikeydim","sikeyim","sikeym","siki","sikicem","sikici","sikien","sikienler","sikiiim","sikiiimmm","sikiim","sikiir","sikiirken","sikik","sikil","sikildiini","sikilesice","sikilmi","sikilmie","sikilmis","sikilmi\u015f","sikilsin","sikim","sikimde","sikimden","sikime","sikimi","sikimiin","sikimin","sikimle","sikimsonik","sikimtrak","sikin","sikinde","sikinden","sikine","sikini","sikip","sikis","sikisek","sikisen","sikish","sikismis","siki\u015f","siki\u015fen","siki\u015fme","sikitiin","sikiyim","sikiym","sikiyorum","sikkim","sikko","sikleri","sikleriii","sikli","sikm","sikmek","sikmem","sikmiler","sikmisligim","siksem","sikseydin","sikseyidin","siksin","siksinbaya","siksinler","siksiz","siksok","siksz","sikt","sikti","siktigimin","siktigiminin","sikti\u011fim","sikti\u011fimin","sikti\u011fiminin","siktii","siktiim","siktiimin","siktiiminin","siktiler","siktim","siktim","siktimin","siktiminin","siktir","siktir et","siktirgit","siktir git","siktirir","siktiririm","siktiriyor","siktir lan","siktirolgit","siktir ol git","sittimin","sittir","skcem","skecem","skem","sker","skerim","skerm","skeyim","skiim","skik","skim","skime","skmek","sksin","sksn","sksz","sktiimin","sktrr","skyim","slaleni","sokam","sokar\u0131m","sokarim","sokarm","sokarmkoduumun","sokay\u0131m","sokaym","sokiim","soktu\u011fumunun","sokuk","sokum","soku\u015f","sokuyum","soxum","sulaleni","s\u00fclaleni","s\u00fclalenizi","s\u00fcrt\u00fck","\u015ferefsiz","\u015f\u0131ll\u0131k","taaklarn","taaklarna","tarrakimin","tasak","tassak","ta\u015fak","ta\u015f\u015fak","tipini s.k","tipinizi s.keyim","tiyniyat","toplarm","topsun","toto\u015f","vajina","vajinan\u0131","veled","veledizina","veled i zina","verdiimin","weled","weledizina","whore","xikeyim","yaaraaa","yalama","yalar\u0131m","yalarun","yaraaam","yarak","yaraks\u0131z","yaraktr","yaram","yaraminbasi","yaramn","yararmorospunun","yarra","yarraaaa","yarraak","yarraam","yarraam\u0131","yarragi","yarragimi","yarragina","yarragindan","yarragm","yarra\u011f","yarra\u011f\u0131m","yarra\u011f\u0131m\u0131","yarraimin","yarrak","yarram","yarramin","yarraminba\u015f\u0131","yarramn","yarran","yarrana","yarrrak","yavak","yav\u015f","yav\u015fak","yav\u015fakt\u0131r","yavu\u015fak","y\u0131l\u0131\u015f\u0131k","yilisik","yogurtlayam","yo\u011furtlayam","yrrak","z\u0131kk\u0131m\u0131m","zibidi","zigsin","zikeyim","zikiiim","zikiim","zikik","zikim","ziksiiin","ziksiin","zulliyetini","zviyetini"};

    private void Start() 
    {
        RaycastEsyaYaziBlogu.gameObject.SetActive(false);
        for (int i=0; i<fotolar.Count; i++){
            esyalar.Add(isimler[i], fotolar[i]);
        }
        Sifirla();
    }

    public void RaycastEsyaBulundu(string isim)
    {
        RaycastEsyaYaziBlogu.gameObject.SetActive(true);
        esyaYazisi.text = isim;
    }

    public void RaycastEsyaBirakildi()
    {
        RaycastEsyaYaziBlogu.gameObject.SetActive(false);
        esyaYazisi.text = "";
    }

    public void changeState(List<string> ObjeAdlari)
    {
        int index = 0;
        EnvaterKismiSifirla();
        foreach(string name in ObjeAdlari)
        {
            if(esyalar.TryGetValue(name, out Sprite retrievedSprite)){
                slotlar[index].sprite = retrievedSprite;
            }else{
                print("eshang pajang hongya hong tong tonge shinjong ditajang she Mao Zedonge.Khali da women de ji pange djengi pang shentang ja suo ju ren. Hiyahi zu joh jaja zu joah djengi pang shentang ja suo ju ren.");
            }
            index++;
        }
    }

    public void SecimIndex(int slot)
    {
        if (slot < 0){Sifirla_Secim(); return;}
        Sifirla_Secim();
        slotlar_secimler[slot].SetActive(true);
    }

    public void EnvaterKismiSifirla()
    {
        foreach(Image image in slotlar){
            image.sprite = EsyaYokfotosu;
        }
    }

    public void Sifirla()
    {
        foreach(Image image in slotlar){
            image.sprite = EsyaYokfotosu;
        }

        Sifirla_Secim();
        LobiModu();
        A_TAKIM_BUTONU.SetActive(true);
        B_TAKIM_BUTONU.SetActive(true);
        takim_a = takim_b = 0;

        MaciBaslatTusu.gameObject.SetActive(false);

        canbari.value = 100;
        mermi_sayisi.text = "";
    }

    public void Sifirla_Secim()
    {
        foreach(GameObject image in slotlar_secimler){
            image.SetActive(false);
        }
    }

    public void CanBari(int can)
    {
        canbari.value = can;
    }

    public void MermiSayisi(int amount)
    {
        mermi_sayisi.text = amount.ToString();
    }

    public void OyunModu()
    {
        OyunPanel.SetActive(true);
        LobiPanel.SetActive(false);
        OyuncuListePanel.SetActive(true);
    }

    public void LobiModu()
    {
        OyunPanel.SetActive(false);
        LobiPanel.SetActive(true);
        OyuncuListePanel.SetActive(false);
    }

    public void KullaniciAdiniOnaylaTusu()
    {
        hata_yazisi.text = "";
        string ad = ad_giris.text;

        ad = ad.Trim();
        ad = ad.Replace(" ", "");
        ad = ad.ToLower();

        if (ad == "" || ad==" ")
        {
            hata_yazisi.text = "soldaki boşluğa birşeyler yazman lazim";
            return;
        }

        foreach (string filtre in yasak_kelimeler)
        {
            if (ad.Contains(filtre))
            {
                hata_yazisi.text = "küfür yazma **** ******.";
                return;
            }
        }

        // adı kaydet
        StaticVariables.singleton.OYUNCU_ADI = ad_giris.text;
    } 

    public void TakimPaneliniKapat()
    {
        OyuncuListePanel.SetActive(false);
    }

    public void OwnerMaciActi()
    {
        MaciBaslatTusu.gameObject.SetActive(true);
    }

    public void setA_B_TakimSayisi(Takim a)
    {
        if (a==Takim.A){takim_a++; A_TAKIM.text = takim_a.ToString()+"/20";}
        if (a==Takim.B){takim_b++; B_TAKIM.text = takim_b.ToString()+"/20";}
    }

    public void setA_B_TakimSayisi(int A, int B)
    {
        A_TAKIM.text = A.ToString()+"/20";
        B_TAKIM.text = B.ToString()+"/20";
    }

    public void OwnerMaciBaslatTusuCallback()
    {
        if (owner_oyuncu == null){return;}
        // Sunucu reklamını kapat
        // NetworkDiscovery durdurulamıyor 
        FishNet.InstanceFinder.ServerManager.GetComponent<NetworkDiscovery>().StopSearchingOrAdvertising();
        owner_oyuncu.OwnerOyunuBaslat();
    }

    public void takimKatilA()
    {
        if (owner_oyuncu == null){return;}
        owner_oyuncu.TakimSecVeAdAyarla(Takim.A, StaticVariables.singleton.OYUNCU_ADI);
        TakimButonlariniSil();
    }

    public void takimKatilB()
    {
        if (owner_oyuncu == null){return;}
        owner_oyuncu.TakimSecVeAdAyarla(Takim.B, StaticVariables.singleton.OYUNCU_ADI);
        TakimButonlariniSil();
    }

    public void TakimButonlariniSil()
    {
        A_TAKIM_BUTONU.SetActive(false);
        B_TAKIM_BUTONU.SetActive(false);
    }

    public void killYaziSpawnla(string Olduren, string Olen)
    {
        GameObject x = Instantiate(killFeedPrefab);
        x.transform.SetParent(killFeedObj);
        x.transform.SetScale(Vector3.one);
        x.GetComponent<killFeed>()._start(Olduren, Olen);
    }

    public void MacKazanildiYaziSpawnla(Takim a)
    {
        GameObject x = Instantiate(MacKazanildiYaziPrefab, OyunPanel.transform);
        x.transform.localScale = Vector3.one;

        if (a == Takim.A){
            x.GetComponent<killFeed>()._start("A TAKIMI (T) KAZANDI","", 7f, false);
        }else{
            x.GetComponent<killFeed>()._start("B TAKIMI (CT) KAZANDI","", 7f, false);
        }
    }

    public void OldunuzYazisiSpawnla()
    {
        GameObject x = Instantiate(OldunuzYazisi, OyunPanel.transform);
        x.transform.SetScale(Vector3.one);
    }
}
