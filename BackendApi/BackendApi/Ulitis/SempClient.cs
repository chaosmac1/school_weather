using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BackendApi.Ulitis {
    public class SempClient {
        private readonly uint _packSize;
        private readonly Socket _socket;

        public SempClient(Socket socket, uint packSize) {
            _socket = socket;
            _packSize = packSize;
        }

        public static Task FactoryStart(Socket socket, uint packSize, Action<SempClient> action) {
            var sempClient = new SempClient(socket, packSize);
            return Task.Run(() => action(sempClient));
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
            if (_socket is null) throw new Exception("Socket is null");
            var buffer = new byte[_packSize];
            try {
                size = _socket.Receive(buffer);
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
            var sendBytes = new byte[value.Length];
            Encoding.UTF8.GetBytes(value.ToCharArray(), sendBytes);

            if (!Send(sendBytes)) {
#if DEBUG
                throw new Exception("Error by SendBytes");
#else
                return false;
#endif
            }

            return true;
        }

        public bool Send(ReadOnlySpan<byte> toClient) {
            if (_socket is null) throw new Exception("Socket is null");
            try {
                _socket.Send(toClient);
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

        public bool Active() => _socket.Connected;

        public void Close() {
            try {
                _socket.Close();
            }
            catch (Exception) {
                // ignored
            }
        }
    }
}