using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyNameInputField;
    [SerializeField] private TMP_InputField _joinByCodeInputField;
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;
    [SerializeField] private Button _copyLobbyCodeButton;
    [SerializeField] private LobbyListUI _lobbyListUI;
    private Lobby _lobby;

    private void Start()
    {
        UpdateCopyButtonInteractivity();
    }

    private async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = isPrivate;

        _lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        UpdateCopyButtonInteractivity();
        _lobbyCodeText.text = $"Lobby Code: {_lobby.LobbyCode}";
    }

    public async void CreateLobbyClick()
    {
        var lobbyName = _lobbyNameInputField.text;
        await CreateLobby(lobbyName, 2, false);
    }

    public void UpdateCopyButtonInteractivity()
    {
        _copyLobbyCodeButton.interactable = _lobby != null;
    }

    public async void JoinByCodeClick()
    {
        string lobbyCode = _joinByCodeInputField.text;
        await JoinByLobbyCode(lobbyCode);
    }

    public async Task JoinByLobbyCode(string lobbyCode)
    {
        try
        {
            Debug.Log($"Joining by Code: {lobbyCode}");
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log($"Success join by Code: {lobbyCode} {lobby.Name}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public void CopyLobbyCode()
    {
        if (_lobby == null)
            return;

        GUIUtility.systemCopyBuffer = _lobby.LobbyCode;
    }

    [ContextMenu(nameof(SearchForLobbiesAsync))]
    public async void SearchForLobbiesAsync()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            _lobbyListUI.CreateLobbyList(lobbies.Results);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public static async Task JoinByLobbyId(string id)
    {
        try
        {
            Debug.Log($"Joining by Id: {id}");
            var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);
            Debug.Log($"Success join by Id: {id} {lobby.Name}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
