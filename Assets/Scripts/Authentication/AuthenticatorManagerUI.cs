using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;

public class AuthenticatorManagerUI : MonoBehaviour
{
    public void SignIn(string username, string password)
    {

    }

    public void SignUp(string username, string password, string confirmPassword)
    {
        if (username.Length < 3)
            return;

        AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
    }
}
