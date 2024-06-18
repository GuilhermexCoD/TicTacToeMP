using System.Threading.Tasks;
using TMPro;
using Unity.Services.Core;
using UnityEngine;

public class AuthenticatorManagerUI : MonoBehaviour
{
    [SerializeField] private SignInUI _signIn;
    [SerializeField] private SignUpUI _signUp;

    [SerializeField] private GameObject _signInUpContainer;

    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private TextMeshProUGUI _messageText;

    private async void Awake()
    {
        _loadingUI.SetActive(true);
        _signInUpContainer.SetActive(false);
        _messageText.text = "Initializing services...";

        await UnityServices.InitializeAsync();

        await Task.Delay(2000);

        _loadingUI.SetActive(false);
        _signInUpContainer.SetActive(true);

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _signUp.OnSignUpStarted += OnSignUpStarted;
        _signUp.OnSignUpCompleted += OnSignUpCompleted;
        _signUp.OnSignUpFailed += OnSignUpFailed;
    }

    private void OnSignUpStarted()
    {
        _loadingUI.SetActive(true);
        _messageText.text = "Signing Up user...";
    }
    private async void OnSignUpCompleted()
    {
        _loadingUI.SetActive(true);
        _messageText.text = "Sign Up successfully!";
        await Task.Delay(2000);
        _loadingUI.SetActive(false);
        _signUp.gameObject.SetActive(false);
    }

    private async void OnSignUpFailed(string failedMessage)
    {
        _loadingUI.SetActive(true);
        _messageText.text = $"Sign Up Error: {failedMessage}";
        await Task.Delay(4000);
        _loadingUI.SetActive(false);
    }

    private void UnsubscribeEvents()
    {
        _signUp.OnSignUpStarted -= OnSignUpStarted;
        _signUp.OnSignUpCompleted -= OnSignUpCompleted;
        _signUp.OnSignUpFailed -= OnSignUpFailed;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
