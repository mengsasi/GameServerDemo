using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T instance;

    private static object _lock = new object();

    public static T Instance {
        get {
            if( applicationIsQuitting ) {
                Debug.LogWarning( "application is quit" );
                return null;
            }

            lock( _lock ) {
                if( instance == null ) {
                    instance = (T)FindObjectOfType( typeof( T ) );

                    if( instance == null ) {
                        GameObject singletion = new GameObject();
                        instance = singletion.AddComponent<T>();
                        singletion.name = "(singletion)" + typeof( T ).ToString();

                        if( !Application.isPlaying ) {
                            instance.hideFlags = HideFlags.HideAndDontSave;
                        }
                        else {
                            DontDestroyOnLoad( singletion );
                        }
                    }
                }
                return instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy() {
        applicationIsQuitting = true;
    }

}
