using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StartUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] internal TMP_InputField Input_NetworkAdress;
    [SerializeField] internal TMP_InputField Input_UserName;

    [SerializeField] internal Button[] Btn_ServerOrClient;
    [SerializeField] internal Button Btn_Start;

    [SerializeField] internal TextMeshProUGUI Text_Error;

    [SerializeField] NetworkManager _netManager;

    [SerializeField] GameObject _StartPanel;
    [SerializeField] GameObject _NamePanel;

    public static StartUI Instance { get; private set; }

    private string _originNetworkAddress;
    private bool isServer;

    private void Awake()
    {
        Instance = this;
        Text_Error.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetDefaultNetworkAddress();
    }

    private void OnEnable()
    {
        Input_UserName.onValueChanged.AddListener(OnValueChanged_ToggleButton);
    }

    private void OnDisable()
    {
        Input_UserName.onValueChanged.RemoveListener(OnValueChanged_ToggleButton);
    }

    private void Update()
    {
        if(_NamePanel.activeSelf)
            CheckNetworkAddressValidOnUpdate();
    }

    private void SetDefaultNetworkAddress()
    {
        // 네트워크 주소 없는 경우, 디폴트 세팅
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }

        // 네트워크 주소 공란으로 변경될 경우를 대비해 기존 네트워크 주소 보관
        _originNetworkAddress = NetworkManager.singleton.networkAddress;
    }

    private void CheckNetworkAddressValidOnUpdate()
    {
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = _originNetworkAddress;
        }

        if (Input_NetworkAdress.text != NetworkManager.singleton.networkAddress)
        {
            Input_NetworkAdress.text = NetworkManager.singleton.networkAddress;
        }
    }

    public void SetUIOnClientDisconnected()
    {
        this.gameObject.SetActive(true);
        Input_UserName.text = string.Empty;
        Input_UserName.ActivateInputField();
    }

    public void SetUIOnAuthValueChanged()
    {
        Text_Error.text = string.Empty;
        Text_Error.gameObject.SetActive(false);
    }

    public void SetUIOnAuthError(string msg)
    {
        Text_Error.text = msg;
        Text_Error.gameObject.SetActive(true);
    }

    public void OnValueChanged_ToggleButton(string userName)
    {
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_Start.interactable = isUserNameValid;
    }

    public void OnClick_ServerOrClient(bool _isServer)
    {
        isServer = _isServer;
        _StartPanel.gameObject.SetActive(false);
        _NamePanel.gameObject.SetActive(true);
        
    }
    public void OnClick_Start()
    {
        if (_netManager == null)
            return;

        if (isServer)
        {
            _netManager.StartHost();
        }
        else
        {
            _netManager.StartClient();
        }

        this.gameObject.SetActive(false);
    }
}
