using Server;
using UnityEngine;
using UnityEngine.UI;

public class TestServer : MonoBehaviour {

    public Button BtnStartServer;

    void Awake() {
        BtnStartServer.onClick.AddListener( StartServer );
    }

    private void StartServer() {
        MainServer.Instance.StartServer( "127.0.0.1", 50001 );
    }

    void Start() {
        MainServer.Instance.StartServer( "127.0.0.1", 50001 );
    }

}
