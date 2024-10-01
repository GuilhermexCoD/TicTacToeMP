using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using System.Collections.Generic;
using Unity.Services.Authentication;
using System;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;

public class LobbyManager : MonoBehaviour
{
    private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";
    private const string CONNECTION_TYPE = "dtls";

    public static LobbyManager Instance;

    public event Action<Lobby> OnJoinLobbySuccess;

    public event Action<string, string> OnRemovePlayerSuccess;
    public event Action<string, string, string> OnRemovePlayerFail;

    [SerializeField] private TMP_InputField _lobbyNameInputField;
    [SerializeField] private TMP_InputField _joinByCodeInputField;
    [SerializeField] private LobbyListUI _lobbyListUI;

    [SerializeField] private GameObject _createLobbyView;
    [SerializeField] private LobbyViewUI _lobbyView;

    private Lobby _lobby;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);
    }

    private void Start()
    {
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

        var allocation = await AllocateRelay(maxPlayers);

        var relayJoinCode = await GetRelayJoinCode(allocation.AllocationId);

        await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                {RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
            }
        });

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetRelayServerData(new RelayServerData(allocation, CONNECTION_TYPE));

        StartHost();

        OnJoinLobbySuccess.Invoke(_lobby);
    }

    public async void CreateLobbyClick()
    {
        var lobbyName = _lobbyNameInputField.text;
        await CreateLobby(lobbyName, 2, false);
        _createLobbyView.SetActive(false);
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

            await NetworkJoinLobby(lobby);

            OnJoinLobbySuccess?.Invoke(lobby);
            Debug.Log($"Success join by Code: {lobbyCode} {lobby.Name}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
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

            await NetworkJoinLobby(lobby);

            Instance.OnJoinLobbySuccess.Invoke(lobby);
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

            await NetworkJoinLobby(lobby);

            OnJoinLobbySuccess.Invoke(lobby);
            Debug.Log($"Success quick join: {lobby.Name}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private static async Task NetworkJoinLobby(Lobby lobby)
    {
        var relayJoinCode = lobby.Data[RELAY_JOIN_CODE_KEY].Value;

        JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, CONNECTION_TYPE));

        StartClient();
    }

    public static async Task RemovePlayerFromLobby(string lobbyId, string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            Instance.OnRemovePlayerSuccess.Invoke(lobbyId, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            Instance.OnRemovePlayerFail.Invoke(lobbyId, playerId, e.Message);
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

    public bool TryGetLobby(out Lobby lobby)
    {
        lobby = _lobby;

        return lobby != null;
    }

    public static void StartClient()
    {
        //TODO: Ao conectar em um lobby devemos esconder a tela anterior e atualizar a informaçao no Host
        //TODO: Implementar callbacks
        //NetworkManager.Singleton.OnClientDisconnectCallback += Client_OnClientDisconnectCallback;
        //NetworkManager.Singleton.OnClientConnectedCallback += Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    public static void StartHost()
    {
        //TODO: Implementar callbacks
        //NetworkManager.Singleton.OnClientConnectedCallback += Server_ClientConnectedCallback;
        //NetworkManager.Singleton.OnClientDisconnectCallback += Server_OnClientDisconnectCallback;
        //NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private static async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }

        return default;
    }

    private async Task<string> GetRelayJoinCode(Guid allocationId)
    {
        try
        {
            return await RelayService.Instance.GetJoinCodeAsync(allocationId);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }

        return default;
    }

    private async Task<Allocation> AllocateRelay(int maxPlayersAmount)
    {
        try
        {
            // Removemos 1 pois o host já esta alocado
            return await RelayService.Instance.CreateAllocationAsync(maxPlayersAmount - 1);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);

            return default;
        }
    }

    public async void SubscribeLobbyEvents(string lobbyId)
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;
        callbacks.KickedFromLobby += OnKickedFromLobby;
        callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
        try
        {
            var lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(lobbyId, callbacks);
        }
        catch (LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{lobbyId}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                default: throw;
            }
        }
    }

    private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
    {
        Debug.Log($"Lobby State Changed: {state}");
    }

    private void OnKickedFromLobby()
    {
        Debug.Log("You were kicked from lobby");
    }

    private void OnLobbyChanged(ILobbyChanges changes)
    {
        Debug.Log($"Lobby info changed: {changes.Name.Value}");

    }
}