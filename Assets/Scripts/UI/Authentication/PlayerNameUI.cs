using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Button _saveChangesButton;

    private async void OnEnable()
    {
        UpdateSaveChangesState();
        var playerName = await GetPlayerName();
        UpdatePlayerTextName(playerName);
    }

    public void UpdateSaveChangesState()
    {
        _saveChangesButton.interactable = UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn;
    }

    private void UpdatePlayerTextName(string playerName)
    {
        _playerNameText.text = $"Player Name: {playerName}";
    }

    public async void SaveChangesClick()
    {
        var playerName = await UpdatePlayerName(_playerNameInputField.text);
        UpdatePlayerTextName(playerName);
    }

    public async Task<string> UpdatePlayerName(string desiredPlayerName)
    {
        var playerName = await AuthenticationService.Instance.UpdatePlayerNameAsync(desiredPlayerName);
        Debug.Log($"PlayerName: {playerName}");
        return playerName;
    }

    public async static Task<string> GetPlayerName()
    {
        var playerName = AuthenticationService.Instance.PlayerName;

        if (string.IsNullOrEmpty(playerName))
            playerName = await AuthenticationService.Instance.GetPlayerNameAsync();

        Debug.Log($"PlayerName: {playerName}");
        return playerName;
    }
}
