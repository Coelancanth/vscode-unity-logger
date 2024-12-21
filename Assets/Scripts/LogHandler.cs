using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class LogHandler : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private bool isConnected = false;
    private readonly string serverIP = "127.0.0.1";
    private readonly int serverPort = 3000;
    private Thread connectionThread;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        ConnectToServer();
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        DisconnectFromServer();
    }

    private void ConnectToServer()
    {
        connectionThread = new Thread(() =>
        {
            while (!isConnected)
            {
                try
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(serverIP, serverPort);
                    networkStream = tcpClient.GetStream();
                    isConnected = true;
                    Debug.Log("Connected to log server successfully!");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to connect to log server: {e.Message}");
                    Thread.Sleep(5000); // 重试间隔5秒
                }
            }
        });
        connectionThread.Start();
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!isConnected) return;

        try
        {
            // 提取文件名和行号
            string sourceInfo = "";
            if (!string.IsNullOrEmpty(stackTrace))
            {
                var lines = stackTrace.Split('\n');
                if (lines.Length > 0)
                {
                    var firstLine = lines[0];
                    var atIndex = firstLine.LastIndexOf("at ");
                    if (atIndex >= 0)
                    {
                        sourceInfo = firstLine.Substring(atIndex).Trim();
                    }
                }
            }

            // 使用字符串拼接来创建JSON，避免序列化问题
            string jsonLog = $"{{\"timestamp\":\"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\",\"type\":\"{type}\",\"message\":\"{EscapeJsonString(logString)}\",\"source\":\"{EscapeJsonString(sourceInfo)}\"}}";

            byte[] data = Encoding.UTF8.GetBytes(jsonLog + "\n");
            networkStream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            isConnected = false;
            ConnectToServer();
        }
    }

    private string EscapeJsonString(string str)
    {
        if (string.IsNullOrEmpty(str)) return "";
        return str.Replace("\\", "\\\\")
                 .Replace("\"", "\\\"")
                 .Replace("\n", "\\n")
                 .Replace("\r", "\\r")
                 .Replace("\t", "\\t");
    }

    private void DisconnectFromServer()
    {
        isConnected = false;
        if (connectionThread != null && connectionThread.IsAlive)
        {
            connectionThread.Abort();
        }
        
        if (networkStream != null)
        {
            networkStream.Close();
        }
        
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }
} 