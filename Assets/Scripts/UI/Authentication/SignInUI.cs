using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class SignInUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private TMP_InputField _passwordField;

    public event Action OnSignInStarted;
    public event Action OnSignInCompleted;
    public event Action<string> OnSignInFailed;
    public void SignInClick()
    {
        SignInInternal(_usernameField.text, _passwordField.text);
    }

    private async void SignInInternal(string username, string password)
    {
        OnSignInStarted?.Invoke();

        Debug.Log($"Username: {username} | Password: {password}");
        
        if (username.Length < 3)
        {
            OnSignInFailed?.Invoke("Username is less than 3 characters!");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            OnSignInCompleted?.Invoke();
        }
        catch (AuthenticationException ex)
        {
            OnSignInFailed?.Invoke(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            OnSignInFailed?.Invoke(ex.Message);
        }
    }
}
