using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.ComponentModel;
using System;
using TMPro;
using UnityEngine.UI;

public class LoginCredentials : MonoBehaviour
{
//    [SerializeField] TMP_InputField tmp_Input_Username;
//    [SerializeField] TMP_InputField tmp_Input_Channelname;


    string tmp_Input_Username = "Test User";
    string tmp_Input_Channelname = "Test Channel";


    VivoxUnity.Client client;
    private Uri server = new Uri("https://unity.vivox.com/appconfig/18967-voice-46356-udash");
    private string issuer = "18967-voice-46356-udash";
    private string domain = "mtu1xp.vivox.com";
    private string tokenKey = "XLhCGxN6GtOTamleJfQSYzpcmAoprxLR";
    private TimeSpan timeSpan = new TimeSpan(90);
    private AsyncCallback loginCallback;

    private ILoginSession loginSession;
    private IChannelSession channelSession;


    [SerializeField] private Button LoginButton;
    [SerializeField] private Button JoinButton;
   


   

    private void Awake()
    {
        client = new Client();
        client.Uninitialize();
        client.Initialize();
        DontDestroyOnLoad(this);

        LoginButton.onClick.AddListener(() =>
        {
            Login_User();

        });

        JoinButton.onClick.AddListener(() =>
        {
            Btn_Join_Channel();
        });

    }
    private void OnApplicationQuit()
    {
        client.Uninitialize();
    }


    // Start is called before the first frame update
    void Start()
    {
        loginCallback = new AsyncCallback(Login_Result);
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


#region Login Methods

    public void Login_User()
    {
        Login(tmp_Input_Username);
    }

    public void Login(string userName)
    {
        AccountId accountId = new AccountId(issuer, userName, domain);
        loginSession = client.GetLoginSession(accountId);
        Bind_Login_Callback_Listeners(true, loginSession);
        loginSession.BeginLogin(server, loginSession.GetLoginToken(tokenKey, timeSpan), loginCallback);


    }

    public void Login_Result(IAsyncResult asyncResult)
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

    public void Logout()
    {
        loginSession.Logout();
        Bind_Login_Callback_Listeners(false, loginSession);
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
                break;
        }

    }
    #endregion


#region Join Channel Methods

    public void JoinChannel(string channelName, bool isAudio, bool isText, bool switchTransmission, ChannelType channelType)
    {
        ChannelId channelId = new ChannelId(issuer, channelName, domain, channelType);
        channelSession = loginSession.GetChannelSession(channelId);
        Bind_Channel_Callback_Listeners(true, channelSession);
        channelSession.BeginConnect(isAudio, isText, switchTransmission, channelSession.GetConnectToken(tokenKey, timeSpan), ar =>
        {

            try
            {
                channelSession.EndConnect(ar);
            } catch (Exception e)
            {
                Bind_Channel_Callback_Listeners(false, channelSession);
                Debug.Log(e.Message);
            }


        });


    }

    public void Btn_Join_Channel()
    {
        JoinChannel(tmp_Input_Channelname, true, true, true, ChannelType.NonPositional);

    }

    public void Leave_Channel(IChannelSession channelToDisconnect, string channelName)
    {
        channelToDisconnect.Disconnect();
        loginSession.DeleteChannelSession(new ChannelId(issuer, channelName, domain));
    }


    public void Btn_Leave_Channel_Clicked()
    {
        Leave_Channel(channelSession, tmp_Input_Channelname);
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
    
#endregion


}
