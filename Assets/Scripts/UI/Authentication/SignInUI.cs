using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

public class SignInUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private TMP_InputField _passwordField;

    public void SignInClick()
    {
        SignInInternal(_usernameField.text, _passwordField.text);
    }

    private void SignInInternal(string username, string password)
    {
        Debug.Log($"Username: {username} | Password: {password}");
        if (username.Length < 3)
            return;

        AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
    }
}
