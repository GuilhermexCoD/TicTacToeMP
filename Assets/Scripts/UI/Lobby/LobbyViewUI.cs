using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyViewUI : MonoBehaviour
{
    private static string PLAYER_NAME_KEY = "PlayerNameKey";

    [Header("Lobby View Properties")]
    [SerializeField] private TextMeshProUGUI _lobbyInfoText;
    [SerializeField] private TextMeshProUGUI _playerOneText;
    [SerializeField] private TextMeshProUGUI _playerTwoText;

    private Lobby _lobby;
    public void Start()
    {
        LobbyManager.Instance.OnJoinLobbySuccess += OnJoinLobbySuccess;

        LobbyManager.Instance.OnLobbyStateChanged += OnLobbyStateChanged;
        LobbyManager.Instance.OnLobbyInfoChanged += OnLobbyInfoChanged;

        LobbyManager.Instance.OnRemovePlayerSuccess += OnRemovePlayerSuccess;
        LobbyManager.Instance.OnRemovePlayerFail += OnRemovePlayerFail;
    }

    private async void OnLobbyInfoChanged(ILobbyChanges changes)
    {
        Debug.Log("Update Player Name");
        var lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);

        SetLobby(lobby);
    }

    private void OnLobbyStateChanged(LobbyEventConnectionState state)
    {
        Debug.Log("Update Player Name");
        UpdatePlayerNames();
    }

    public async void SetLobby(Lobby lobby)
    {
        if (lobby == null)
            return;

        gameObject.SetActive(true);

        _lobby = lobby;
        _lobbyInfoText.text = $"Name: {_lobby.Name} | Lobby Id: {_lobby.Id}";
        _lobby = await SetLobbyPlayerData(_lobby.Id);
        UpdatePlayerNames();
    }

    public async Task<Lobby> SetLobbyPlayerData(string lobbyId)
    {
        try
        {
            var playerName = await PlayerNameUI.GetPlayerName();
            var options = new UpdatePlayerOptions();

            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    PLAYER_NAME_KEY, new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Public,
                        value: playerName)
                }
            };

            string playerId = AuthenticationService.Instance.PlayerId;

            var lobby = await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, options);

            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        return null;
    }

    private void UpdatePlayerNames()
    {
        if (_lobby.Players.Count >= 1)
        {
            var playerOne = _lobby.Players[0];
            if (playerOne != null && playerOne.Data != null)
                _playerOneText.text = playerOne.Data[PLAYER_NAME_KEY]?.Value;
        }

        if (_lobby.Players.Count >= 2)
        {
            var playerTwo = _lobby.Players[1];
            if (playerTwo != null && playerTwo.Data != null)
                _playerTwoText.text = playerTwo.Data[PLAYER_NAME_KEY]?.Value;
        }
    }

    private void OnJoinLobbySuccess(Lobby lobby)
    {
        SetLobby(lobby);
    }

    private void OnRemovePlayerFail(string lobbyId, string playerId, string errorMessage)
    {
        Debug.Log($"Removed fail LobbyId: {lobbyId} | PlayerId: {playerId} | Error: {errorMessage}");
    }

    private void OnRemovePlayerSuccess(string lobbyId, string playerId)
    {
        Debug.Log($"Removed success LobbyId: {lobbyId} | PlayerId: {playerId}");

    }

    public async void LeaveLobbyClick()
    {
        await LobbyManager.LeaveLobby(_lobby.Id);
    }
}
