using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PatchUpdate.ftp
{
    public delegate void MessageInvoker(string szMessage);
    public delegate void FileTranProgress(int percent);

    public enum TransferType { Binary, ASCII };

    //
    // 摘要:
    //     表示可与 FTP 请求一起使用的 FTP 协议方法的类型。 此类不能被继承。
    public static class Ftp
    {
        //
        // 摘要:
        //     表示要用于从 FTP 服务器下载文件的 FTP RETR 协议方法。
        public const string DownloadFile = "RETR";
        //
        // 摘要:
        //     表示获取 FTP 服务器上的文件的简短列表的 FTP NLIST 协议方法。
        public const string ListDirectory = "NLST";
        //
        // 摘要:
        //     表示将文件上载到 FTP 服务器的 FTP STOR 协议方法。
        public const string UploadFile = "STOR";
        //
        // 摘要:
        //     表示要用于删除 FTP 服务器上的文件的 FTP DELE 协议方法。
        public const string DeleteFile = "DELE";
        //
        // 摘要:
        //     表示要用于将文件追加到 FTP 服务器上的现有文件的 FTP APPE 协议方法。
        public const string AppendFile = "APPE";
        //
        // 摘要:
        //     表示要用于检索 FTP 服务器上的文件大小的 FTP SIZE 协议方法。
        public const string GetFileSize = "SIZE";
        //
        // 摘要:
        //     表示将具有唯一名称的文件上载到 FTP 服务器的 FTP STOU 协议方法。
        public const string UploadFileWithUniqueName = "STOU";
        //
        // 摘要:
        //     表示在 FTP 服务器上创建目录的 FTP MKD 协议方法。
        public const string MakeDirectory = "MKD";
        //
        // 摘要:
        //     表示移除目录的 FTP RMD 协议方法。
        public const string RemoveDirectory = "RMD";
        //
        // 摘要:
        //     表示获取 FTP 服务器上的文件的详细列表的 FTP LIST 协议方法。
        public const string ListDirectoryDetails = "LIST";
        //
        // 摘要:
        //     表示要用于从 FTP 服务器上的文件检索日期时间戳的 FTP MDTM 协议方法。
        public const string GetDateTimestamp = "MDTM";
        //
        // 摘要:
        //     表示打印当前工作目录的名称的 FTP PWD 协议方法。
        public const string PrintWorkingDirectory = "PWD";
        //
        // 摘要:
        //     表示重命名目录的 FTP RENAME 协议方法。
        public const string Rename = "RENAME";
    }   
    /// <summary>
    /// FTP命令
    /// </summary>
    class FtpCommand
    {
        public const string USER = "USER ";
        public const string PASS = "PASS ";
        public const string CWD = "CWD ";
        public const string NLST = "NLST ";
        public const string PASV = "PASV";
        public const string STOR = "STOR ";
        public const string RETR = "RETR ";
        public const string TYPEI = "TYPE I";
        public const string TYPEA = "TYPE A";
        public const string SIZE = "SIZE ";
        public const string DELE = "DELE ";
        public const string MKD = "MKD ";
        public const string RMD = "RMD ";
        public const string RNFR = "RNFR ";
        public const string RNTO = "RNTO ";
        public const string QUIT = "QUIT "; 
    }

    public class CSharpFtpClient : IDisposable
    {
        private string _host;
        /// <summary>
        /// 服务器Ip地址
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        private string _remotePath;
        /// <summary>
        /// 远程目录，默认为跟目录
        /// </summary>
        public string RemotePath
        {
            get { return _remotePath; }
            set
            {
                _remotePath = value;
            }
        }

        private string _userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _passWord;
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }

        private int _port = 21;
        /// <summary>
        /// 断开，默认为21
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private bool _isSsl = false;
        /// <summary>
        /// 是否SSL加密
        /// </summary>
        public bool IsSsl
        {
            get { return _isSsl; }
            set { _isSsl = value; }
        }

        /// <summary>
        /// 编码方式
        /// </summary>
        private Encoding _tranEncoding = Encoding.UTF8;
        /// <summary>
        /// 编码方式
        /// </summary>
        public Encoding TranEncoding
        {
            get { return _tranEncoding; }
            set { _tranEncoding = value; }
        }

        private TransferType _transferType = TransferType.Binary;
        /// <summary>
        /// 传输模式
        /// </summary>
        public TransferType EnTransferType
        {
            get { return _transferType; }
            set { _transferType = value; }
        }

        private bool _isConnect = false;
        /// <summary>
        /// 当前是否处在连接状态
        /// </summary>
        public bool IsConnect
        {
            get { return _isConnect; }
        }

        /// <summary>
        /// 操作信息是事件
        /// </summary>
        public event MessageInvoker MessageEvent;
        /// <summary>
        /// 文件上传下载的进度事件
        /// </summary>
        public event FileTranProgress OnFileTranProgress;

        /// <summary>
        /// 接收和发送数据的缓冲区
        /// </summary>
        private static int BUFFERLENGTH = 1024;

        private Byte[] _buffer = new Byte[BUFFERLENGTH];

        private TcpClient _tcpClient;
        private NetworkStream _tcpStream;

        /// <summary>
        /// 所有的应答信息
        /// </summary>
        private string _respone;

        private bool _isDisposed = false;

        public CSharpFtpClient()
        {
            _userName = "uploadusr";
            _passWord = "h3ckey";
        }

        public CSharpFtpClient(string host)
        {
            _host = host;
            _userName = "uploadusr";
            _passWord = "h3ckey";
        }

        public CSharpFtpClient(string host, string userName, string passwd)
        {
            _host = host;
            _userName = userName;
            _passWord = passwd;
        }

        ~CSharpFtpClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="respone">返回连接信息</param>
        /// <returns></returns>
        public bool Connect()
        {
            string respone = "";
            try
            {
                //1.连接ftp服务器
                if (_tcpClient == null)
                {
                    _tcpClient = new TcpClient();
                }

                if (_tcpClient.Connected == false)
                {
                    _tcpClient.Connect(_host, _port);
                }

                _tcpStream = _tcpClient.GetStream();
                respone = ReadRespone();
                if (respone != "220")
                {
                    respone = respone + " 连接服务器失败";
                    SendMessage(respone);
                    _isConnect = false;
                    return false;
                }
                else
                {
                    respone = respone + " 成功连接到服务器";
                    SendMessage(respone);
                }


                //2.身份验证
                respone = SendCommand(FtpCommand.USER + _userName);
                if (!(respone == "331" || respone == "230"))
                {
                    DisConnect();//关闭连接
                    respone = respone + " 验证用户名失败";
                    SendMessage(respone);
                    _isConnect = false;
                    return false;
                }
                else
                {
                    respone = respone + " 用户名验证成功，需要密码验证";
                    SendMessage(respone);
                }

                respone = SendCommand(FtpCommand.PASS + _passWord);
                if (!(respone == "202" || respone == "230"))
                {
                    DisConnect();//关闭连接
                    respone = respone + " 验证密码失败";
                    SendMessage(respone);
                    _isConnect = false;
                    return false;
                }
                else
                {
                    respone = respone + " 登入成功";
                    SendMessage(respone);
                }

                _isConnect = true;
                return true;
            }
            catch (Exception err)
            {
                respone = "连接失败," + err.Message;
                SendMessage(respone);
                return false;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="respone">返回连接信息</param>
        /// <returns></returns>
        public bool Connect(string host, string userName, string passwd)
        {
            string respone = "";
            try
            {
                _host = host;
                _userName = userName;
                _passWord = passwd;

                //1.连接ftp服务器
                if (_tcpClient == null)
                {
                    _tcpClient = new TcpClient();
                }

                if (_tcpClient.Connected == false)
                {
                    _tcpClient.Connect(_host, _port);
                }

                _tcpStream = _tcpClient.GetStream();
                respone = ReadRespone();
                if (respone != "220")
                {
                    respone = respone + " 连接服务器失败";
                    SendMessage(respone);
                    _isConnect = false;
                    return false;
                }
                else
                {
                    respone = respone + " 成功连接到服务器";
                    SendMessage(respone);
                }


                //2.身份验证
                respone = SendCommand(FtpCommand.USER + _userName);
                if (!(respone == "331" || respone == "230"))
                {
                    DisConnect();//关闭连接
                    respone = respone + " 验证用户名失败";
                    SendMessage(respone);
                    _isConnect = false;
                    return false;
                }
                else
                {
                    respone = respone + " 用户名验证成功，需要密码验证";
                    SendMessage(respone);
                }

                respone = SendCommand(FtpCommand.PASS + _passWord);
                if (!(respone == "202" || respone == "230"))
                {
                    DisConnect();//关闭连接
                    respone = respone + " 验证密码失败";
                    SendMessage(respone);
                    _isConnect = false;
                    return false;
                }
                else
                {
                    respone = respone + " 登入成功";
                    SendMessage(respone);
                }

                _isConnect = true;
                return true;
            }
            catch (Exception err)
            {
                respone = "连接失败," + err.Message;
                SendMessage(respone);
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="respone"></param>
        /// <returns></returns>
        public bool DisConnect()
        {
            bool result = false;
            try
            {
                if (null != _tcpClient && _tcpClient.Connected)
                {
                    LogOut();

                    if (null != _tcpStream)
                    {
                        _tcpStream.Dispose();
                        _tcpStream = null;
                    }

                    _tcpClient.Client.Close();
                    _tcpClient.Close();
                    _tcpClient = null;

                    result = true;
                }
                else
                {
                    result = true;
                }

                SendMessage("断开服务器连接");
                return result;
            }
            catch (Exception err)
            {
                SendMessage(err.Message);
                return false;
            }
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(string message)
        {
            if (MessageEvent != null)
            {
                MessageEvent(message);
            }
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string SendCommand(string command)
        {
            byte[] byCommand = _tranEncoding.GetBytes(command + "\r\n");

            //写入命令
            _tcpStream.Write(byCommand, 0, byCommand.Length);
            return ReadRespone();
        }

        /// <summary>
        /// 读取应答信息
        /// </summary>
        /// <param name="respone"></param>
        /// <returns></returns>
        private string ReadRespone()
        {
            string respone = "";
            int contentLength = 0;

            //读取返回信息
            while (true)
            {
                contentLength = _tcpStream.Read(_buffer, 0, BUFFERLENGTH);
                respone = respone + _tranEncoding.GetString(_buffer, 0, contentLength);
                if (contentLength < BUFFERLENGTH)
                {
                    break;
                }
            }

            _respone = respone;

            if (respone.Length > 3)
            {
                respone = respone.Substring(0, 3);
            }
            return respone;
        }

        /// </summary>
        /// 设置传输模式
        /// </summary>
        /// <param name="ttType">传输模式</param>
        public void SetTransferType(TransferType type)
        {
            string respone = "";

            if (type == TransferType.Binary)
            {
                respone = SendCommand(FtpCommand.TYPEI);//binary类型传输
            }
            else
            {
                respone = SendCommand(FtpCommand.TYPEA);//ASCII类型传输
            }
            if (respone != "200")
            {
                SendMessage(respone + " 设置传输模式失败");
            }
            else
            {
                _transferType = type;
            }
        }

        /// <summary>
        /// 建立进行数据连接的socket
        /// </summary>
        /// <returns>数据连接socket</returns>
        private Socket CreateDataSocket()
        {
            try
            {
                string respone = SendCommand(FtpCommand.PASV);
                if (respone != "227")
                {
                    SendMessage(respone + " 建立pasv模式失败");
                    return null;
                }

                int index1 = _respone.IndexOf('(');
                int index2 = _respone.IndexOf(')');

                string szIpAddress = _respone.Substring(index1 + 1, index2 - index1 - 1);
                string[] aszParts = szIpAddress.Split(',');

                string ipAddress = aszParts[0] + "." + aszParts[1] + "." +
                aszParts[2] + "." + aszParts[3];

                int port = int.Parse(aszParts[4]);
                port = (port << 8) + int.Parse(aszParts[5]);

                Socket PasvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

                PasvSocket.Connect(endpoint);

                return PasvSocket;
            }
            catch (Exception err)
            {
                return null;
            }
        }
        /// <summary>
        /// 改变目录
        /// </summary>
        /// <param name="strDirName">新的工作目录名</param>
        public bool ChangeDir(string dirName)
        {
            bool result = false;
            if (!_isConnect)
            {
                Connect();
            }

            string respone = SendCommand(FtpCommand.CWD + dirName);
            if (respone != "250")
            {
                respone = respone + " 改变目录失败";
                result = false;
            }
            else
            {
                respone = respone + " 改变目录成功";
                this._remotePath = dirName;
                result = true;
            }
            SendMessage(respone);
            return result;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <returns>文件大小</returns>
        private long GetFileSize(string fileName)
        {
            if (!_isConnect)
            {
                Connect();
            }

            string respone = SendCommand(FtpCommand.SIZE + fileName);
            long size = 0;
            if (respone == "213")
            {
                size = Int64.Parse(_respone.Substring(4));
            }
            else
            {
                SendMessage(respone + " 获取文件大小失败");
            }
            return size;
        }

        /// <summary>
        /// 获取指定目录的文件列表
        /// 先设置RemotePath属性
        /// </summary>
        /// <returns></returns>
        public string[] ListDirectory(string dirName)
        {
            if (false == ChangeDir(dirName))
            {
                return null;
            }

            //建立一个数据传送socket
            Socket dataSocket = CreateDataSocket();
            if (null == dataSocket)
            {
                return null;
            }

            string respone = SendCommand(FtpCommand.NLST + dirName);

            if (!(respone == "150" || respone == "125" || respone == "226"))
            {
                respone = respone + " 获取文件列表失败";
                SendMessage(respone);
                return null;
            }
            else
            {
                //读取所有的文件列表
                int contentLength = 0;
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    contentLength = dataSocket.Receive(_buffer, BUFFERLENGTH, 0);
                    sb.Append(_tranEncoding.GetString(_buffer, 0, contentLength));
                    if (contentLength <= 0)
                    {
                        respone = ReadRespone();

                        if (respone == "226" || respone == "250" || string.IsNullOrEmpty(respone))
                        {
                            break;
                        }
                    }
                }

                //关闭连接
                dataSocket.Close();

                string[] aszFileList = sb.ToString().Split('\n');
                List<string> lFileList = new List<string>();

                foreach (string szFile in aszFileList)
                {
                    string tmp = szFile.TrimEnd('\r');
                    if (tmp.Length > dirName.Length)
                    {
                        tmp = tmp.Remove(0, dirName.Length);
                    }
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        lFileList.Add(tmp.Trim('/'));
                    }
                }
                return lFileList.ToArray();
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="dirctory">保存的目录</param>
        /// <param name="fileName">要下载的文件名</param>
        public bool DownLoadFile(string dirctory, string fileName)
        {
            if (!_isConnect)
            {
                Connect();
            }
            //先获取文件大小
            long lFileSize = GetFileSize(fileName);
            SetTransferType(TransferType.Binary);

            Socket socketData = CreateDataSocket();
            string respone = SendCommand(FtpCommand.RETR + fileName);
            if (!(respone == "125" || respone == "150" || respone == "226" || respone == "250"))
            {
                SendMessage(respone + " 下载文件失败");
                return false;
            }

            FileStream fs = new FileStream(dirctory.TrimEnd('\\') + "\\" + Path.GetFileName(fileName), FileMode.Create);

            int length = 0;
            long lRecevie = 0;
            int percent = 0;

            while (true)
            {
                length = socketData.Receive(_buffer, BUFFERLENGTH, 0);
                lRecevie += length;

                fs.Write(_buffer, 0, length);

                if (null != OnFileTranProgress)
                {
                    int tmp = (int)(lRecevie * 100 / lFileSize);

                    if (percent != tmp)
                    {
                        OnFileTranProgress(tmp);
                    }

                    percent = tmp;
                }

                if (lRecevie >= lFileSize)
                {
                    break;
                }
            }

            fs.Close();

            if (socketData.Connected)
            {
                socketData.Close();
            }

            while (true)
            {
                respone = ReadRespone();
                if ((respone == "226" || respone == "250" || string.IsNullOrEmpty(respone)))
                {
                    break;
                }
            }
            SendMessage(respone + " 下载文件(" + fileName + ")成功");
            return true;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool UploadFile(string filePath)
        {
            try
            {
                if (!_isConnect)
                {
                    Connect();
                }
                filePath = "E://WindowsFormsApp1.zip";

                Socket socketData = CreateDataSocket();
                string respone = SendCommand(FtpCommand.STOR + Path.GetFileName(filePath));
                if (!(respone == "125" || respone == "150"))
                {
                    SendMessage(respone + "上传文件失败");
                    return false;
                }

                FileStream fs = new FileStream(filePath, FileMode.Open);
                int length = 0;
                int send = 0;

                long hasSend = 0;
                long wholeLength = fs.Length;
                int percent = 0;

                while (true)
                {
                    length = fs.Read(_buffer, 0, BUFFERLENGTH);
                    send = socketData.Send(_buffer, length, 0);

                    //如果发送不成功，线程睡眠50ms再发一次
                    if (send < length)
                    {
                        System.Threading.Thread.Sleep(50);
                        send = socketData.Send(_buffer, length, 0);
                    }

                    hasSend += send;

                    if (null != OnFileTranProgress)
                    {
                        int tmp = (int)(hasSend * 100 / wholeLength);

                        if (tmp != percent)
                        {
                            OnFileTranProgress(tmp);
                        }

                        percent = tmp;
                    }

                    if (length < BUFFERLENGTH)
                    {
                        break;
                    }
                }

                fs.Close();

                if (socketData.Connected)
                {
                    socketData.Close();
                }

                while (true)
                {
                    respone = ReadRespone();
                    if ((respone == "226" || respone == "250" || string.IsNullOrEmpty(respone)))
                    {
                        break;
                    }
                }
                SendMessage(respone + "上传文件(" + Path.GetFileName(filePath) + ")成功");
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool UploadFile(string filePath, string fileName)
        {
            try
            {
                if (!_isConnect)
                {
                    Connect();
                }

                Socket socketData = CreateDataSocket();

                fileName = "SEngine.exe.config";
                string respone = SendCommand(FtpCommand.STOR + fileName);
                if (!(respone == "125" || respone == "150"))
                {
                    SendMessage(respone + "上传文件失败");
                    return false;
                }

                FileStream fs = new FileStream(filePath, FileMode.Open);
                int length = 0;
                int send = 0;

                long hasSend = 0;
                long wholeLength = fs.Length;
                int percent = 0;

                while (true)
                {
                    length = fs.Read(_buffer, 0, BUFFERLENGTH);
                    send = socketData.Send(_buffer, length, 0);

                    //如果发送不成功，线程睡眠50ms再发一次
                    if (send < length)
                    {
                        System.Threading.Thread.Sleep(50);
                        send = socketData.Send(_buffer, length, 0);
                    }

                    hasSend += send;

                    if (null != OnFileTranProgress)
                    {
                        int tmp = (int)(hasSend * 100 / wholeLength);

                        if (tmp != percent)
                        {
                            OnFileTranProgress(tmp);
                        }

                        percent = tmp;
                    }

                    if (length < BUFFERLENGTH)
                    {
                        break;
                    }
                }

                fs.Close();

                if (socketData.Connected)
                {
                    socketData.Close();
                }

                while (true)
                {
                    respone = ReadRespone();
                    if ((respone == "226" || respone == "250" || string.IsNullOrEmpty(respone)))
                    {
                        break;
                    }
                }
                SendMessage(respone + "上传文件(" + Path.GetFileName(filePath) + ")成功");
                fs.Dispose();
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool DeleteFile(string fileName)
        {
            if (!_isConnect)
            {
                Connect();
            }

            string respone = SendCommand(FtpCommand.DELE + fileName);

            if (respone == "250")
            {
                SendMessage(respone + " 删除文件(" + fileName + ")成功");
                return true;
            }
            else
            {
                SendMessage(respone + " 删除文件(" + fileName + ")失败");
                return false;
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public bool CreateDirectory(string directory)
        {
            if (!_isConnect)
            {
                Connect();
            }

            string respone = SendCommand(FtpCommand.MKD + directory);

            if (respone == "257")
            {
                SendMessage(respone + " 创建目录(" + directory + ")成功");
                return true;
            }
            else
            {
                SendMessage(respone + " 创建目录(" + directory + ")失败");
                return false;
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public bool DeleteDirectory(string directory)
        {
            if (!_isConnect)
            {
                Connect();
            }

            string respone = SendCommand(FtpCommand.RMD + directory);

            if (respone == "250")
            {
                SendMessage(respone + " 删除目录(" + directory + ")成功");
                return true;
            }
            else
            {
                SendMessage(respone + " 删除目录(" + directory + ")失败");
                return false;
            }
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="szCurrentName"></param>
        /// <param name="rename"></param>
        /// <returns></returns>
        public bool ReName(string currentFileName, string rename)
        {
            if (!_isConnect)
            {
                Connect();
            }

            string szRespone = SendCommand(FtpCommand.RNFR + currentFileName);

            if (szRespone == "350")
            {
                SendMessage(szRespone + " 请输入新文件名");
            }
            else
            {
                SendMessage(szRespone + " 重件名(" + rename + ")失败");
                return false;
            }

            szRespone = SendCommand(FtpCommand.RNTO + rename);

            if (szRespone == "250")
            {
                SendMessage(szRespone + " 重命名(" + rename + ")成功");
                return true;
            }
            else
            {
                SendMessage(szRespone + " 重件名(" + rename + ")失败");
                return false;
            }
        }
        /// <summary>
        /// 注销
        /// </summary>
        public void LogOut()
        {
            if (!_isConnect)
            {
                return;
            }

            string szRespone = SendCommand(FtpCommand.QUIT);

            if (szRespone == "221")
            {
                SendMessage(szRespone + " 注销成功");
            }
            else
            {
                SendMessage(szRespone + " 注销失败");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                DisConnect();
                _isDisposed = true;
            }
        }

        public void Close()
        {
            Dispose();
        }
    }
}
