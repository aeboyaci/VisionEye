using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.ComponentModel;
using System;
using TMPro;
using UnityEngine.UI;

public class VoiceChat : MonoBehaviour
{
    VivoxUnity.Client client;
    private Uri server = new Uri("https://unity.vivox.com/appconfig/18967-vr-68023-udash");
    private string issuer = "18967-vr-68023-udash";
    private string domain = "mtu1xp.vivox.com";
    private string tokenKey = "YNW9si8vWnaJEFisu6tRT3eoNHjgxvMr";
    private TimeSpan timeSpan = new TimeSpan(90);
    private AsyncCallback loginCallback;

    private ILoginSession loginSession;
    private IChannelSession channelSession;

    void Start()
    {
        loginCallback = new AsyncCallback(LoginResult);

        client = new VivoxUnity.Client();
        client.Uninitialize();
        client.Initialize();
        DontDestroyOnLoad(this);

        Login();
    }

    public void Bind_Login_Callback_Listeners(bool bind, ILoginSession loginSesh)
    {
        if (bind)
        {
            loginSesh.PropertyChanged += Login_Status;
        }
        else
        {
            loginSesh.PropertyChanged -= Login_Status;
        }
    }

    public void Bind_Channel_Callback_Listeners(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.PropertyChanged += On_Channel_Status_Changed;
        }
        else
        {
            channelSesh.PropertyChanged -= On_Channel_Status_Changed;
        }
    }

    public void Login()
    {
        AccountId accountId = new AccountId(issuer, State.PlayerId, domain);
        loginSession = client.GetLoginSession(accountId);
        Bind_Login_Callback_Listeners(true, loginSession);
        loginSession.BeginLogin(server, loginSession.GetLoginToken(tokenKey, timeSpan), loginCallback);
    }

    public void LoginResult(IAsyncResult asyncResult)
    {
        try
        {
            loginSession.EndLogin(asyncResult);
        }
        catch (Exception e)
        {
            Bind_Login_Callback_Listeners(false, loginSession);
            Debug.Log(e.Message);
        }
    }

    public void Login_Status(object sender, PropertyChangedEventArgs loginArgs)
    {
        var source = (ILoginSession)sender;
        switch (source.State)
        {
            case LoginState.LoggingIn:
                Debug.Log("Logging In");
                break;

            case LoginState.LoggedIn:
                Debug.Log("Logged In");
                JoinChannel(true, true, true, ChannelType.NonPositional);
                break;
        }
    }

    public void JoinChannel(bool isAudio, bool isText, bool switchTransmission, ChannelType channelType)
    {
        ChannelId channelId = new ChannelId(issuer, State.ActiveTeamId, domain, channelType);
        channelSession = loginSession.GetChannelSession(channelId);
        Bind_Channel_Callback_Listeners(true, channelSession);
        channelSession.BeginConnect(isAudio, isText, switchTransmission, channelSession.GetConnectToken(tokenKey, timeSpan), ar =>
        {
            try
            {
                channelSession.EndConnect(ar);
            }
            catch (Exception e)
            {
                Bind_Channel_Callback_Listeners(false, channelSession);
                Debug.Log(e.Message);
            }
        });
    }

    public void Leave_Channel(IChannelSession channelToDisconnect)
    {
        channelToDisconnect.Disconnect();
        loginSession.DeleteChannelSession(new ChannelId(issuer, State.ActiveTeamId, domain));
    }

    public void On_Channel_Status_Changed(object sender, PropertyChangedEventArgs channelArgs)
    {
        IChannelSession source = (IChannelSession)sender;
        switch (source.ChannelState)
        {
            case ConnectionState.Connecting:
                Debug.Log("Connecting");
                break;

            case ConnectionState.Connected:
                Debug.Log("Connected");
                break;

            case ConnectionState.Disconnecting:
                Debug.Log("Disconnecting");
                break;

            case ConnectionState.Disconnected:
                Debug.Log("Disconnected");
                break;
        }
    }
}
