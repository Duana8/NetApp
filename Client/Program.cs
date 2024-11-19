using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
	public class Program
	{
		static void Main(string[] args)
		{
			try
			{
				IPEndPoint remoteIP = 
					new IPEndPoint(IPAddress.Parse("127.0.0.1"), 
												   8888);
				Socket socket = 
					new Socket(AddressFamily.InterNetwork, 
								SocketType.Stream, 
								ProtocolType.Tcp);
				socket.Connect(remoteIP);
				Thread thread = 
					new Thread(SendListen);
				thread.Start(socket);
				ReceiveListen(socket);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			Console.ReadLine();
		}

		/// <summary>
		/// Прослушивание
		/// </summary>
		/// <param name="obj"></param>
		static void SendListen(object obj)
		{
			Socket socket = obj as Socket;
			while (true)
			{
				string message = 
					Console.ReadLine();
				byte[] buffer = 
					Encoding.Unicode.GetBytes(message);
				socket.Send(buffer);
			}
		}

		/// <summary>
		/// Чтение
		/// </summary>
		/// <param name="socket"></param>
		static void ReceiveListen(Socket socket)
		{
			while (true)
			{
				int bytes = 0;
				byte[] buffer = new byte[1024];
				string message = "";
				do
				{
					bytes = socket.Receive(buffer, 
								buffer.Length, 0);
					message += 
						Encoding.Unicode.GetString(buffer, 
									0, 
									bytes);
				} while (socket.Available > 0);
				Console.WriteLine(message);
			}
		}
	}
}