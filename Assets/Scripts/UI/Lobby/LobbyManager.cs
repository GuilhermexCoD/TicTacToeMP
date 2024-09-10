using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Services.Authentication;
using System;

public class LobbyManager : MonoBehaviour
{
    public static event Action<Lobby> OnJoinLobbySuccess;

    public static event Action<string, string> OnRemovePlayerSuccess;
    public static event Action<string, string, string> OnRemovePlayerFail;

    [SerializeField] private TMP_InputField _lobbyNameInputField;
    [SerializeField] private TMP_InputField _joinByCodeInputField;
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;
    [SerializeField] private Button _copyLobbyCodeButton;
    [SerializeField] private LobbyListUI _lobbyListUI;

    [SerializeField] private LobbyViewUI _lobbyView;


    private Lobby _lobby;

    private void Start()
    {
        UpdateCopyButtonInteractivity();

        OnJoinLobbySuccess += Instance_OnJoinLobbySuccess;
    }

    private void Instance_OnJoinLobbySuccess(Lobby lobby)
    {
        OpenLobbyView(lobby);
    }

    private async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = isPrivate;

        _lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        OnJoinLobbySuccess.Invoke(_lobby);
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
            OnJoinLobbySuccess?.Invoke(lobby);
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
            OnJoinLobbySuccess.Invoke(lobby);
            Debug.Log($"Success join by Id: {id} {lobby.Name}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoinClick()
    {
        await QuickJoinAsync();
    }

    public async Task QuickJoinAsync()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            options.Filter = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.EQ,
                    value: "1")
            };

            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            OnJoinLobbySuccess.Invoke(lobby);
            Debug.Log($"Success quick join: {lobby.Name}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public static async Task RemovePlayerFromLobby(string lobbyId, string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            OnRemovePlayerSuccess.Invoke(lobbyId, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnRemovePlayerFail.Invoke(lobbyId, playerId, e.Message);
        }
    }

    public static async Task LeaveLobby(string lobbyId)
    {
        //Ensure you sign-in before calling Authentication Instance
        //See IAuthenticationService interface
        string playerId = AuthenticationService.Instance.PlayerId;

        await RemovePlayerFromLobby(lobbyId, playerId);
    }

    public void OpenLobbyView(Lobby lobby)
    {
        _lobbyView.SetLobby(lobby);
    }
}
