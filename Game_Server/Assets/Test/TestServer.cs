using Server;
using UnityEngine;
using UnityEngine.UI;

public class TestServer : MonoBehaviour {

    public Button BtnStartServer;

    void Awake() {
        BtnStartServer.onClick.AddListener( StartServer );
    }

    private void StartServer() {

        MainServer.Instance.StartServer( "0.0.0.0", 22334 );

    }

}
