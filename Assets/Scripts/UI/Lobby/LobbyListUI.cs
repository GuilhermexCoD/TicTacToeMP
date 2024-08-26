using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private LobbyItemUI _lobbyItemPrefab;
    [SerializeField] private Transform _listContainer;

    public void CreateLobbyList(List<Lobby> lobbies)
    {
        foreach (var lobby in lobbies)
        {
            var lobbyItem = Instantiate(_lobbyItemPrefab, _listContainer);
            lobbyItem.SetLobby(lobby);
        }
    }
}
