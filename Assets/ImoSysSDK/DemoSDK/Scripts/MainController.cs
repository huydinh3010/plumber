using ImoSysSDK.SocialPlatforms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoginFacebookClick() {
        FacebookLogin fbLogin = new FacebookLogin(onLoginFBFailed, onLoginFBSuccess);
        fbLogin.Login();
    }

    private void onLoginFBFailed(string message) {
        Debug.Log("login failed: " + message);
    }

    private void onLoginFBSuccess() {
        Debug.Log("Login success");
    }
}
