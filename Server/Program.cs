using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
	public class Program
	{
		/// <summary>
		/// Список пользователей
		/// </summary>
		static List<Socket> sockets = new List<Socket>();
		static void Main(string[] args)
		{
			IPEndPoint localEP = 
				new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
			Socket listenSocket = 
				new Socket(AddressFamily.InterNetwork, 
							SocketType.Stream, 
							ProtocolType.Tcp);

			// привязка сокета к локальному IP & port
			listenSocket.Bind(localEP); 
			listenSocket.Listen(10);

			// создание потока для обработки сообщений
			Thread thread = new Thread(Listen);
			thread.Start(listenSocket);

			Console.WriteLine("Сервер запущен, ожидание " +
				"подключения пользователей...");
			while (true)
			{
				foreach (Socket socket in sockets.ToList())
				{
					if (Connected(socket))
					{
						// проверка на наличие данных для чтения
						if (socket.Available > 0) Receive(socket);
					}
					else
					{
						Console.WriteLine($"Пользователь с IP " +
							$"{socket.RemoteEndPoint} разорвал соединение");
						// отключение сокета от чтения и записи
						socket.Shutdown(SocketShutdown.Both);
						socket.Close();
						sockets.Remove(socket);
					}
				}
			}
		}

		/// <summary>
		/// Подключение
		/// </summary>
		/// <param name="obj"></param>
		static void Listen(object obj)
		{
			Socket socket = obj as Socket;
			while (true)
			{
				Socket handler = socket.Accept();
				sockets.Add(handler);
				Console.WriteLine("Новое подключение: " 
					+ handler.RemoteEndPoint.ToString());
			}
		}

		/// <summary>
		/// Чтение
		/// </summary>
		/// <param name="handler"></param>
		static void Receive(Socket handler)
		{
			string message = $"{handler.RemoteEndPoint} : ";
			int bytes = 0;
			byte[] buffer = new byte[1024];
			do
			{
				bytes = handler.Receive(buffer);
				message += 
					Encoding.Unicode.GetString(buffer, 0, bytes);
			} while (handler.Available > 0);

			Console.WriteLine(message);
			buffer = Encoding.Unicode.GetBytes(message);
			foreach (Socket socket in sockets.ToList())
			{
				if (socket != handler)
				{
					socket.Send(buffer);
				}
			}	
		}

		/// <summary>
		/// Подключения
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		static bool Connected(Socket socket)
		{
			try
			{
				// если сокет не в режиме только чтение, то возврат
				return !(socket.Poll(10, SelectMode.SelectRead) 
						&& socket.Available == 0);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}