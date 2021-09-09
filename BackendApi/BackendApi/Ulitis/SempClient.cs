using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackendApi.Ulitis {
    public class SempClient {
        private Socket Socket;
        private uint PackSize;
        
        public SempClient(Socket socket, uint packSize) {
            Socket = socket;
            PackSize = packSize;
        }

        public static Task FactoryStart(Socket socket, uint packSize, Action<SempClient> action) {
            var sempClient = new SempClient(socket, packSize);
            return Task.Run((() => action(sempClient)));
        }
        
        
        public bool ReceiveString(out string? fromClient) {
            if (!Receive(out var span, out var size)) {
                fromClient = null;
                return false;
            }

            try {
                fromClient = Encoding.UTF8.GetString(span);
                return true;
            }
#if DEBUG
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
#else
            catch (Exception) {
                fromClient = null;
                return false;
#endif
            }
        }
        
        public bool Receive(out ReadOnlySpan<byte> fromClientSpan, out int size) {
            if (Socket is null) throw new Exception("Socket is null");
            var buffer = new byte[this.PackSize];
            try {
                size = Socket.Receive(buffer);
                if (size == -1) {
                    fromClientSpan = Span<byte>.Empty;
                    return false;
                }
                
                fromClientSpan = new Span<byte>(buffer, 0, size);
                return true;
            }
#if DEBUG
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
#else
            catch (Exception) {
                fromClientSpan = Span<byte>.Empty; 
                size = -1;
                return false;
#endif
            }
        }

        public bool SendString(string value) {
            Span<byte> sendBytes = Span<byte>.Empty;
            Encoding.UTF8.GetBytes((ReadOnlySpan<char>)new Span<char>(value.ToCharArray()), sendBytes);

            if (!Send((ReadOnlySpan<byte>)sendBytes)) {
#if DEBUG
                throw new Exception("Error by SendBytes");
#else
                return false;
#endif
            }
            
            return true;
        }
        
        public bool Send(ReadOnlySpan<byte> toClient) {
            if (Socket is null) throw new Exception("Socket is null");
            try {
                Socket.Send(toClient);
                return true;
            }
#if DEBUG
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
#else
            catch (Exception) { 
                return false;
#endif
            }
            
        }

        public bool Active() => Socket.Connected;

        public void Close() {
            try { Socket.Close(); }
            catch (Exception) {
                // ignored
            }
        }
    }
}