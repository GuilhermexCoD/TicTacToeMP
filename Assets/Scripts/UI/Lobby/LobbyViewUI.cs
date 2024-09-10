using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyViewUI : MonoBehaviour
{
    [Header("Lobby View Properties")]
    [SerializeField] private TextMeshProUGUI _lobbyInfoText;
    [SerializeField] private TextMeshProUGUI _playerOneText;
    [SerializeField] private TextMeshProUGUI _playerTwoText;

    private Lobby _lobby;
    public void Start()
    {
        LobbyManager.OnJoinLobbySuccess += OnJoinLobbySuccess;

        LobbyManager.OnRemovePlayerSuccess += OnRemovePlayerSuccess;
        LobbyManager.OnRemovePlayerFail += OnRemovePlayerFail;
    }

    public void SetLobby(Lobby lobby)
    {
        if (lobby == null)
            return;

        gameObject.SetActive(true);

        _lobby = lobby;
        _lobbyInfoText.text = $"Name: {_lobby.Name} | Lobby Id: {_lobby.Id}";

        UpdatePlayerNames();
    }

    private void UpdatePlayerNames()
    {
        if (_lobby.Players.Count >= 1)
        {
            var playerOne = _lobby.Players[0];
            if (playerOne != null)
                _playerOneText.text = playerOne.Id;
        }

        if (_lobby.Players.Count >= 2)
        {
            var playerTwo = _lobby.Players[1];
            if (playerTwo != null)
                _playerTwoText.text = playerTwo.Id;
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
