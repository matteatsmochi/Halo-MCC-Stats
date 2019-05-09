using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Models;
using System.Collections;
public class Bot : MonoBehaviour
{
    //Twitch
    Client client;

    API api;
    
    
    void Start()
    {
        Application.runInBackground = true;
        client = new Client();
        //ConnectToTwitchChat("matteatsmochi");

        client.OnConnected += onConnected;
        client.OnMessageReceived += globalChatMessageRecieved;
        client.OnMessageSent += globalChatMessageSent;
        client.OnDisconnected += onDisconnected;

        api = GetComponent<API>();

    }

    void Update()
    {

    }


    #region "Twitch"

    public void ConnectToTwitchChat(string channel)
    {
        
        //ConnectionCredentials credentials = new ConnectionCredentials("mccstatsbot", "09fyeabvbbuuaxtw2uke8gc1t2ncqi");
        ConnectionCredentials credentials = new ConnectionCredentials("halomccbot", "0t4ail3b33usestuyxhdahdv2k4l8b");
        client.Initialize(credentials, channel.ToLower().Replace(" ",""));

        if (!client.IsConnected)
        {
        client.Connect();
        }
        Debug.Log("<< Connecting >>");
        
    }

    void DisconnectFromTwitchChat()
    {
        client.LeaveChannel(client.JoinedChannels[0]);
        if (client.IsConnected)
        {
            client.Disconnect();
        }
        Debug.Log("<< Disconnecting . . . >>");
    }

    void onConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
    {
        
        Debug.Log("<< Connected >>");
        StartCoroutine(ConnectionMessage());
    }

    void onDisconnected(object sender, TwitchLib.Client.Events.OnDisconnectedArgs e)

    {
        Debug.Log("<< Disconnected from chat server >>");
        
    }

    void globalChatMessageRecieved(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        //Debug.Log(e.ChatMessage.Username + ": " + e.ChatMessage.Message);

        if (e.ChatMessage.Message.StartsWith("!KD"))
        {
            //TwitchMessage("Session K/D: " + api.sessionKD.ToString("0.##"));
        }

    }

    void globalChatMessageSent(object sender, TwitchLib.Client.Events.OnMessageSentArgs e)
    {
        Debug.Log(e.SentMessage.DisplayName + ": " + e.SentMessage.Message);
    }

    public void TwitchMessage(string msg)
    {
        client.SendMessage(client.JoinedChannels[0], msg);

        Debug.Log(msg);
    }

    IEnumerator ConnectionMessage()
    {
        yield return new WaitForSeconds(3);
        TwitchMessage("MrDestructoid Hi! I'm a bot that finds your Halo MCC stats and tracks them.");
    }


    #endregion
}
