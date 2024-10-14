using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameManagerNetwork : NetworkBehaviour 
{
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientStarted += Singleton_OnClientStarted;
        NetworkManager.Singleton.OnClientStopped += Singleton_OnClientStopped;
    }

    private void Singleton_OnClientStopped(bool clientId)
    {
        Debug.Log($"Client Stopped: {clientId}");
    }

    private void Singleton_OnClientStarted()
    {
        Debug.Log($"Client Started: {NetworkManager.LocalClientId}");

    }

    private void Singleton_OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"Client Disconnect: {clientId}");
    }

    private void Singleton_OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Client Connected: {clientId}");
    }

    [ContextMenu(nameof(CheckClients))]
    public void CheckClients()
    {
        Debug.Log($"Check Connected Client: {NetworkManager.Singleton.ConnectedClientsList.Count}");

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log($"Client: {client.ClientId}");
        } 
    }
}
