using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine.UI;
using FishNet.Discovery;

public class NetworkCanvas : MonoBehaviour
{

    // Fish networkingdeki hud canvası hangi mal yazdı bilmiyorum ama bu şekilde kopyalayarak tekardan yapmak zorunda kaldım

    [SerializeField] CanvasOyuncu canvasOyuncu;
    [SerializeField] NetworkDiscovery discovery;
    private NetworkManager _networkManager;
    private LocalConnectionState _clientState = LocalConnectionState.Stopped;
    private LocalConnectionState _serverState = LocalConnectionState.Stopped;

    void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkManager == null)
        {
            Debug.LogError("NetworkManager not found, HUD will not function.");
            return;
        }
        else
        {
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        discovery.SearchForServers();
    }

    void Update()
    {
        
    }

    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        _clientState = obj.ConnectionState;
    }


    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        _serverState = obj.ConnectionState;
    }

    public void OnClick_Server()
    {
        if (_networkManager == null)
            return;

        if (_serverState != LocalConnectionState.Stopped)
            _networkManager.ServerManager.StopConnection(true);
        else
            _networkManager.ServerManager.StartConnection();
            discovery.AdvertiseServer();

        StaticVariables.singleton.actual_host = true;
    }

    public void OnClick_Host()
    {
        if( StaticVariables.singleton.AdHazir()){return;}


        if (_networkManager == null)
            return;

        if (_serverState != LocalConnectionState.Stopped)
            _networkManager.ServerManager.StopConnection(true);
        else
            _networkManager.ServerManager.StartConnection();
            discovery.AdvertiseServer();

        if (_networkManager == null)
            return;

        if (_clientState != LocalConnectionState.Stopped)
            _networkManager.ClientManager.StopConnection();
        else
            _networkManager.ClientManager.StartConnection();

        canvasOyuncu.OyunModu();
        StaticVariables.singleton.actual_host = true;

    }

    public void OnClickRefresh()
    {
        discovery.SearchForServers();
    }

    private void OnDestroy()
    {
        if (_networkManager == null)
            return;

        _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
        _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
    }
}
