using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class SignInUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private TMP_InputField _passwordField;

    public event Action OnStarted;
    public event Action OnCompleted;
    public event Action<string> OnFailed;
    public void SignInClick()
    {
        SignInInternal(_usernameField.text, _passwordField.text);
    }

    private async void SignInInternal(string username, string password)
    {
        OnStarted?.Invoke();
        Debug.Log($"Username: {username} | Password: {password}");
        if (username.Length < 3)
        {
            OnFailed?.Invoke("Username less than 3 characters");
            return;
        }
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            OnCompleted?.Invoke();
            
        }
        catch (AuthenticationException ex)
        {
            OnFailed?.Invoke(ex.Message);
         
        }
        catch (RequestFailedException ex)
        {
            OnFailed?.Invoke(ex.Message);

        }
    }
}
