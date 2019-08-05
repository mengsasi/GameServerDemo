using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Core.Network {

    public delegate void SocketStateChangedCallback( NetworkTcpClient sender, bool connected );

    public class NetworkTcpClient {

        private Socket socket;
        public SocketStateChangedCallback StateChanged;

        private const int BUFFER_SIZE = 1024;
        private byte[] readBuffer = new byte[BUFFER_SIZE];

        private Queue<byte[]> recvQueue = new Queue<byte[]>();

        public NetworkTcpClient() {
            readBuffer = new byte[BUFFER_SIZE];
        }

        public void ConnectDomain( string host, int port ) {
            Dns.BeginGetHostEntry( host, result => {
                try {
                    IPHostEntry entry = Dns.EndGetHostEntry( result );
                    var addresses = entry.AddressList;
                    var address = addresses[UnityEngine.Random.Range( 0, addresses.Length - 1 )];
                    Do_Connect( address, port );
                }
                catch( Exception ) {
                    StateChanged( this, false );
                }
            }, null );
        }

        public void Connnect( string ip, int port ) {
            Do_Connect( IPAddress.Parse( ip ), port );
        }

        public void Close() {
            if( socket != null && socket.Connected ) {
                try {
                    socket.Shutdown( SocketShutdown.Both );
                    socket.Close();
                    socket = null;
                }
                catch {
                    socket = null;
                }
            }
        }

        public byte[] Dispatch() {
            lock( recvQueue ) {
                if( recvQueue.Count == 0 )
                    return null;

                return recvQueue.Dequeue();
            }
        }

        public void Destroy() {
            StateChanged = null;
            Close();
        }

        public void Send( byte[] data, int length ) {
            try {
                int byteSent = socket.Send( data, 0, length, SocketFlags.None );
                Debug.Assert( byteSent == length );
            }
            catch( Exception ) {
                StateChanged( this, false );
            }
        }

        private void Do_Connect( IPAddress address, int port ) {
            var endPoint = new IPEndPoint( address, port );
            socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.BeginConnect( endPoint, result => {
                try {
                    socket.EndConnect( result );
                    StateChanged( this, true );
                    Begin_Recv();
                }
                catch( Exception ) {

                }
            }, null );
        }

        private void Begin_Recv() {
            if( socket.Connected ) {
                socket.BeginReceive( readBuffer, 0,
                readBuffer.Length, SocketFlags.None, result => {
                    try {
                        int length = socket.EndReceive( result );
                        if( length <= 0 ) {
                            StateChanged( this, false );
                            return;
                        }

                        var buffer = readBuffer;
                        int typeLength = buffer[0];
                        int dataLength = buffer[1];

                        if( dataLength > 0 ) {
                            int blength = buffer.Length;
                            byte[] data = new byte[blength];

                            for( int i = 0; i < blength; i++ ) {
                                data[i] = buffer[i];
                            }

                            lock( recvQueue ) {
                                recvQueue.Enqueue( data );
                            }
                        }
                        readBuffer = new byte[BUFFER_SIZE];
                        Begin_Recv();
                    }
                    catch( Exception ) {
                        Socket ts = (Socket)result.AsyncState;
                        if( ts != socket ) {
                            return;
                        }
                        StateChanged( this, false );
                    }
                }, socket );
            }
        }
    }
}