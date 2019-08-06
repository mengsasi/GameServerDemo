using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Core.Network {

    public delegate void SocketStateChangedCallback( NetworkTcpClient sender, bool connected );

    public class NetworkTcpClient {

        private Socket socket;
        public SocketStateChangedCallback StateChanged;

        private ProtoStream recvStream = new ProtoStream();
        private Queue<byte[]> recvQueue = new Queue<byte[]>();

        public NetworkTcpClient() {
            InitStream();
        }

        private void InitStream() {
            byte[] receiveBuffer = new byte[1 << 16];
            recvStream.Write( receiveBuffer, 0, receiveBuffer.Length );
            recvStream.Seek( 0, System.IO.SeekOrigin.Begin );
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

        private static int receivePosition;
        private void Begin_Recv() {
            if( socket.Connected ) {
                socket.BeginReceive( recvStream.Buffer, receivePosition,
                recvStream.Buffer.Length - receivePosition, SocketFlags.None, result => {
                    try {
                        int length = socket.EndReceive( result );
                        if( length <= 0 ) {
                            StateChanged( this, false );
                            return;
                        }

                        receivePosition += length;
                        int i = recvStream.Position;
                        while( receivePosition >= i + 2 ) {
                            int dataLength = recvStream[i] + recvStream[i + 1];

                            int sz = dataLength + 2;
                            if( receivePosition < i + sz ) {
                                break;
                            }

                            recvStream.Seek( 0, SeekOrigin.Current );

                            if( dataLength > 0 ) {
                                byte[] data = new byte[sz];
                                recvStream.Read( data, 0, sz );

                                lock( recvQueue ) {
                                    recvQueue.Enqueue( data );
                                }
                            }

                            i += sz;
                        }

                        if( receivePosition == recvStream.Buffer.Length ) {
                            recvStream.Seek( 0, SeekOrigin.End );
                            recvStream.MoveUp( i, i );
                            receivePosition = recvStream.Position;
                            recvStream.Seek( 0, SeekOrigin.Begin );
                        }

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