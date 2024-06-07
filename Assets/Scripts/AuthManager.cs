using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthManager : NetworkAuthenticator
{
    readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> _playerNames = new HashSet<string>();

    [SerializeField] StartUI _startUI;

    [Header("Client Username")]
    public string _playerName;

    public struct AuthReqMsg : NetworkMessage
    {
        // ������ ���� ���
        // OAuth ���� �� ��� �� �� �κп� ������ ��ū ���� ������ �߰��ϸ� ��
        public string authUserName;
    }

    public struct AuthResMsg : NetworkMessage
    {
        public byte code;
        public string message;
    }

    #region ServerSide
    [UnityEngine.RuntimeInitializeOnLoadMethod]
    static void ResetStatics()
    {
    }

    public override void OnStartServer()
    {
        // Ŭ��κ��� ���� ��û ó���� ���� �ڵ鷯 ����
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }

    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }

    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
    }

    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        // Ŭ�� ���� ��û �޼��� ���� �� ó��

        Debug.Log($"���� ��û : {msg.authUserName}");

        if (_connectionsPendingDisconnect.Contains(conn)) return;


        // ������, DB, Playfab API ���� ȣ���� ���� Ȯ��
        if (!_playerNames.Contains(msg.authUserName))
        {
            _playerNames.Add(msg.authUserName);

            // ������ ���� ���� Player.OnStartServer �������� ����
            conn.authenticationData = msg.authUserName;

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 100,
                message = "Auth Success"
            };

            conn.Send(authResMsg);
            ServerAccept(conn);
        }
        else
        {
            _connectionsPendingDisconnect.Add(conn);

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 200,
                message = "User Name already in use! Try again!"
            };

            conn.Send(authResMsg);
            conn.isAuthenticated = false;

            StartCoroutine(DelayedDisconnect(conn, 1.0f));
        }
    }

    IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ServerReject(conn);

        yield return null;
        _connectionsPendingDisconnect.Remove(conn);
    }
    #endregion

    public void OnInputValueChanged_SetPlayerName(string username)
    {
        _playerName = username;
        _startUI.SetUIOnAuthValueChanged();
    }
    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AuthResMsg>(OnAuthResponseMessage, false);
    }

    public override void OnStopClient()
    {
        NetworkClient.UnregisterHandler<AuthResMsg>();
    }

    // Ŭ�󿡼� ���� ��û �� �ҷ���
    public override void OnClientAuthenticate()
    {
        NetworkClient.Send(new AuthReqMsg { authUserName = _playerName });
    }

    public void OnAuthResponseMessage(AuthResMsg msg)
    {
        if (msg.code == 100) // ����
        {
            Debug.Log($"Auth Response: {msg.code} {msg.message}");
            ClientAccept();
        }
        else
        {
            Debug.LogError($"Auth Response: {msg.code} {msg.message}");
            NetworkManager.singleton.StopHost();

            _startUI.SetUIOnAuthError(msg.message);
        }
    }
}