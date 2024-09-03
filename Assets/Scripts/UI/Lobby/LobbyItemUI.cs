using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private Button _joinButton;

    private Lobby _lobby;

    public void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        UpdateLobbyName();
    }

    public void UpdateLobbyName()
    {
        _lobbyNameText.text = _lobby.Name;
    }

    public async void JoinLobbyClickAsync()
    {
        if (_lobby != null)
        {
            await LobbyManager.JoinByLobbyId(_lobby.Id);
        }
    }
}
