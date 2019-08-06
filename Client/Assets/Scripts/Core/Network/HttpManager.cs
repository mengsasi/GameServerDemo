using LitJson;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace Core.Network {

    public class HttpManager : Singleton<HttpManager> {

        public enum HttpResponseStatus {
            NetworkError,
            RequestNotFound,
            InternalError,
            OK
        }

        private void ErrorProcess( UnityWebRequest request, Action<HttpResponseStatus, JsonData> callback ) {
            if( request.isNetworkError ) {
                callback( HttpResponseStatus.NetworkError, null );
            }
            else if( request.isHttpError ) {
                if( request.responseCode == 404 ) {
                    callback( HttpResponseStatus.RequestNotFound, null );
                }
                else {
                    callback( HttpResponseStatus.InternalError, null );
                }
            }
            else {
                callback( HttpResponseStatus.InternalError, null );
            }
        }

        public void GetJson( string url, Action<HttpResponseStatus, JsonData> callback ) {
            StartCoroutine( DoGetJson( url, callback ) );
        }

        private IEnumerator DoGetJson( string url, Action<HttpResponseStatus, JsonData> callback ) {
            using( UnityWebRequest request = UnityWebRequest.Get( url ) ) {
                yield return request.SendWebRequest();
                if( request.isNetworkError || request.isHttpError ) {
                    ErrorProcess( request, callback );
                }
                else {
                    string text = request.downloadHandler.text;
                    try {
                        var response = JsonMapper.ToObject( text );
                        callback( HttpResponseStatus.OK, response );
                    }
                    catch( Exception ) {
                        ErrorProcess( request, callback );
                    }
                }
            }
        }

        public void PostJson( string url, JsonData body, Action<HttpResponseStatus, JsonData> callback ) {
            StartCoroutine( DoPostJson( url, body, callback ) );
        }

        private IEnumerator DoPostJson( string url, JsonData body, Action<HttpResponseStatus, JsonData> callback ) {
            var json = body.ToString();
            UnityWebRequest request = new UnityWebRequest( url ) {
                method = "POST",
                uploadHandler = new UploadHandlerRaw( System.Text.Encoding.UTF8.GetBytes( json ) ),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader( "Content-Type", "application/json" );
            yield return request.SendWebRequest();
            if( request.isNetworkError || request.isHttpError ) {
                ErrorProcess( request, callback );
            }
            else {
                string text = request.downloadHandler.text;
                try {
                    var response = JsonMapper.ToObject( text );
                    callback( HttpResponseStatus.OK, response );
                }
                catch( Exception ) {
                    ErrorProcess( request, callback );
                }
            }
        }

    }

}
