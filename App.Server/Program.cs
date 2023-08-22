using App.Business.Concrete;
using App.Entities.Concrete;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace App.Server
{
    internal class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 27001;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        static void Main(string[] args)
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadLine();
            CloseAllSockets();

            GetAllServicesAsText();
        }

        private static void CloseAllSockets()
        {
            throw new NotImplementedException();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server . . .");
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IPAddress.Loopback.ToString()), PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket;
            try
            {
                socket = serverSocket.EndAccept(ar);
            }
            catch (Exception)
            {
                return;
            }

            SendServiceResponseToClient(socket);
            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, socket);
            Console.WriteLine("Client connected, waiting for request . . . ");
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket current=(Socket)ar.AsyncState;
            int received;
            try
            {
                received = current.EndReceive(ar);
            }
            catch (Exception)
            {
                Console.WriteLine("Client Forcefully disconnected");
                current.Close();
                clientSockets.Remove(current);
                return;
            }
            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string msg = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text");
            if (msg.ToLower()=="exit")
            {
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");
            }
            else if(msg != String.Empty)
            {
                try
                {
                    var result = msg.Split(new[] { ' ' }, 2) ;
                    if (result.Length == 2)
                    {
                        var jsonPart = result[1];
                        var subResult = result[0].Split('\\');
                        var ClassName = subResult[0];
                        var Methodname = subResult[1];
                        var MyType = Assembly.GetAssembly(typeof(ProductService)).GetTypes()
                            .FirstOrDefault(t => t.Name.Contains(ClassName));
                        var MyEntitytype = Assembly.GetAssembly(typeof(Product)).GetTypes()
                            .FirstOrDefault(a => a.FullName.Contains(ClassName));
                        var obj = JsonConvert.DeserializeObject(jsonPart, MyEntitytype);

                        var methods = MyType.GetMethods();
                        MethodInfo MyMethod = methods.FirstOrDefault(m => m.Name.Contains(Methodname));
                        var myInstance = Activator.CreateInstance(MyType);
                        MyMethod.Invoke(myInstance, new object[] { obj });
                        byte[] data =Encoding.ASCII.GetBytes("POST Operation Succes");
                        current.Send(data);
                    }
                    else
                    {
                        result = msg.Split('\\');
                        var ClassName = result[0];
                        var MethodName = result[1];

                        var MyType = Assembly.GetAssembly(typeof(ProductService)).GetTypes()
                            .FirstOrDefault(t => t.Name.Contains(ClassName));
                        if(MyType != null)
                        {
                            var methods = MyType.GetMethods();
                            MethodInfo MyMethod = methods.FirstOrDefault(m => m.Name.Contains(MethodName));

                            object MyInstance = Activator.CreateInstance(MyType);

                            var paramid = -1;
                            var jsonString = string.Empty;
                            object objResponse=null;
                            if (result.Length == 3)
                            {
                                paramid = int.Parse(result[2]);
                                objResponse = MyMethod.Invoke(MyInstance,new object[] { paramid });
                            }
                            else
                            {
                                objResponse = MyMethod.Invoke(MyInstance,null);
                            }
                            jsonString=JsonConvert.SerializeObject(objResponse);
                            byte[]data=Encoding.ASCII.GetBytes(jsonString);
                            current.Send(data);
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            }
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, current);
        }

        private static void SendServiceResponseToClient(Socket client)
        {
            var result = GetAllServicesAsText();
            byte[] data = Encoding.ASCII.GetBytes(result);
            client.Send(data);
        }

        private static string GetAllServicesAsText()
        {
            var myTypes = Assembly.GetAssembly(typeof(ProductService)).GetTypes()
                .Where(t => t.Name.EndsWith("Service") && !t.Name.StartsWith("I"));

            var sb = new StringBuilder();
            foreach (var type in myTypes)
            {
                var className = type.Name.Remove(type.Name.Length - 7, 7);
                var methods = type.GetMethods().Reverse().Skip(4);

                foreach (var m in methods)
                {
                    string responseText = $@"{className}\{m.Name}";
                    var parameters = m.GetParameters();
                    foreach (var param in parameters)
                    {
                        if (param.ParameterType != typeof(string) && param.ParameterType.IsClass)
                        {
                            responseText += $@"\{param.Name}[json]";
                        }
                        else
                        {
                            responseText += $@"\{param.Name}";
                        }
                    }
                    sb.AppendLine(responseText);
                }
            }
            var result = sb.ToString();
            return result;
        }
    }
}
