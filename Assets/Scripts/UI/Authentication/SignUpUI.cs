using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class SignUpUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TMP_InputField _confirmPasswordField;

    public event Action OnSignUpStarted;
    public event Action OnSignUpCompleted;
    public event Action<string> OnSignUpFailed;

    public void SignUpClick()
    {
        SignUpInternal(_usernameField.text, _passwordField.text, _confirmPasswordField.text);
    }

    private async void SignUpInternal(string username, string password, string confirmPassword)
    {
        OnSignUpStarted?.Invoke();

        Debug.Log($"Username: {username} | Password: {password} | Confirm Password: {confirmPassword}");

        if (username.Length < 3)
            return;

        if (password != confirmPassword)
            return;

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            OnSignUpCompleted?.Invoke();
        }
        catch (AuthenticationException ex)
        {
            OnSignUpFailed?.Invoke(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            OnSignUpFailed?.Invoke(ex.Message);
        }
    }
}
