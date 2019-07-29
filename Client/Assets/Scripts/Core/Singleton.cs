using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T instance;

    public static T Instance {
        get {
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
