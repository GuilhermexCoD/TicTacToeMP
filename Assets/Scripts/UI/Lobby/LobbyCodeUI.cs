using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCodeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _codeText;
    [SerializeField] private Button _copyButton;

    private Lobby _lobby;

    void Start()
    {
        _copyButton.onClick.AddListener(CopyLobbyCode);

        UpdateUI();

        if (LobbyManager.Instance.TryGetLobby(out var lobby))
        {
            SetLobby(lobby);
        }
        else
        {
            LobbyManager.Instance.OnJoinLobbySuccess += OnJoinLobbySuccess;
        }
    }

    private void OnJoinLobbySuccess(Lobby lobby)
    {
        SetLobby(lobby);
    }

    private void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateCodeText();
        UpdateCodeButtonStatus();
    }

    private void UpdateCodeText()
    {
        if (_lobby == null)
            _codeText.text = $"Lobby Code: {string.Empty}";
        else
            _codeText.text = $"Lobby Code: {_lobby.LobbyCode}";
    }

    private void UpdateCodeButtonStatus()
    {
        _copyButton.interactable = _lobby != null;
    }

    public void CopyLobbyCode()
    {
        if (_lobby == null)
            return;

        GUIUtility.systemCopyBuffer = _lobby.LobbyCode;
    }

    private void OnDestroy()
    {
        _copyButton.onClick.RemoveListener(CopyLobbyCode);

        LobbyManager.Instance.OnJoinLobbySuccess -= OnJoinLobbySuccess;
    }
}
