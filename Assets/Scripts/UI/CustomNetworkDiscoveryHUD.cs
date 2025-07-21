using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomNetworkDiscoveryHUD : MonoBehaviour
{
    public NetworkDiscovery networkDiscovery;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();


    public Button btn1;
    public TextMeshProUGUI lbl1;
    public Button btn2;
    public TextMeshProUGUI lbl2;
    public Button btn3;
    public TextMeshProUGUI lbl3;
    public GameObject displayPanel;
    public RectTransform content;
    public TextMeshProUGUI contentTitle;
    public GameObject serverButtonPrefab;


    private void Awake()
    {
        BaseMode();
    }

    private void BaseMode()
    {
        displayPanel.SetActive(true);

        btn1.onClick.RemoveAllListeners();
        btn1.onClick.AddListener(FindServers);
        lbl1.text = "Find Server";
        btn1.gameObject.SetActive(true);

        btn2.onClick.RemoveAllListeners();
        btn2.onClick.AddListener(StartHost);
        lbl2.text = "Start host";
        btn2.gameObject.SetActive(true);

        btn3.onClick.RemoveAllListeners();
        btn3.onClick.AddListener(StartServer);
        lbl3.text = "Start server";
        btn3.gameObject.SetActive(true);
    }

    private void FocusedMode()
    {
        btn1.onClick.RemoveAllListeners();
        btn2.gameObject.SetActive(false);
        btn2.onClick.RemoveAllListeners();
        btn3.gameObject.SetActive(false);
        btn3.onClick.RemoveAllListeners();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        Reset();
    }

    void Reset()
    {
        networkDiscovery = GetComponent<NetworkDiscovery>();

        // Add default event handler if not already present
        if (!Enumerable.Range(0, networkDiscovery.OnServerFound.GetPersistentEventCount())
            .Any(i => networkDiscovery.OnServerFound.GetPersistentMethodName(i) == nameof(OnDiscoveredServer)))
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new UnityEngine.Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif

    public void StartHost()
    {
        FocusedMode();
        displayPanel.SetActive(false);
        btn1.onClick.AddListener(StopHost);
        lbl1.text = "Stop Host";

        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void StopHost() 
    {
        if (!(NetworkServer.active && NetworkClient.isConnected)) { return; }

        BaseMode();
        NetworkManager.singleton.StopHost();
        networkDiscovery.StopDiscovery();
    }

    public void StartServer()
    {
        FocusedMode();
        displayPanel.SetActive(false);
        btn1.onClick.AddListener(StopServer);
        lbl1.text = "Stop Host";

        discoveredServers.Clear();
        NetworkManager.singleton.StartServer();
        networkDiscovery.AdvertiseServer();
    }

    public void StopServer() 
    {
        if (!NetworkServer.active) {  return; }

        BaseMode();
        NetworkManager.singleton.StopServer();
        networkDiscovery.StopDiscovery();
    }


    public void FindServers()
    {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        Debug.Log(discoveredServers.Count);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        Debug.Log($"Discovered Server: {info.serverId} | {info.EndPoint} | {info.uri}");

        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
        SetDiscoveredServers();
    }


    void SetDiscoveredServers()
    {
        foreach(GameObject child in content)
        {
            Destroy(child);
        }
        contentTitle.text = "Discovered Servers";
        foreach (ServerResponse info in discoveredServers.Values) 
        {
            GameObject buttonInstance = Instantiate(serverButtonPrefab, content);
            buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = info.EndPoint.Address.ToString();
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => 
            {
                FocusedMode();
                displayPanel.SetActive(false);
                btn1.onClick.AddListener(StopClient);
                lbl1.text = "Stop Client";
                networkDiscovery.StopDiscovery();
                NetworkManager.singleton.StartClient(info.uri);
            });
        }
    }

    public void StopClient()
    {
        if (NetworkServer.active) { return; }
        if (NetworkClient.isConnected)
        {

            BaseMode();
            NetworkManager.singleton.StopClient();
            networkDiscovery.StopDiscovery();
        }
    }
}
