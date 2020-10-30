using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchUpdate.ftp
{
    /// <summary>
    /// 操作远程文件，包括http文件，以及ftp图片
    /// </summary>
    public class FileOperate
    {
        /// <summary>
        /// 上传http图片
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="serverUrl"></param>
        /// <returns></returns>
        public static bool uploadHttpImg(string localPath, string serverUrl)
        {
            //待继续
            return true;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static bool getImg(string imgUrl, ref Bitmap bitmap)
        {
            if ("http" == imgUrl.Split(':')[0])
            {
                if (getHttpImg(imgUrl, ref bitmap))
                {
                    return true;
                }
            }
            if ("ftp" == imgUrl.Split(':')[0])
            {
                //加载ftp信息
                string ftpUserName = imgUrl.Split(':')[1].Split('/')[2];
                string ftpPasswd = imgUrl.Split(':')[2].Split('@')[0];
                //    imgUrl="ftp://"+imgUrl.Split(
                if (getFtpImg(imgUrl, ftpUserName, ftpPasswd, ref bitmap))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取http图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static bool getHttpImg(string imgUrl, ref Bitmap bitmap)
        {
            try
            {
                //创建连接
                WebRequest webRequest = WebRequest.Create(imgUrl);
                webRequest.Timeout = 3000;
                webRequest.Proxy = null;

                //获取响应
                WebResponse webResponse = webRequest.GetResponse();

                //将读取的http图片转换为流模式
                Stream reader = webResponse.GetResponseStream();

                //将流转换为bit
                bitmap = new Bitmap(reader);

                reader.Close();
                reader.Dispose();
                webResponse.Close();
                webRequest.Abort();

            }
            catch (Exception err)
            {
                JTLog.log().ErrorFormat("获取http图片：{0},失败。失败原因：{1}", imgUrl, err);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取http图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap getHttpImg(string imgUrl)
        {
            Bitmap bitmap = null;
            try
            {
                //创建连接
                WebRequest webRequest = WebRequest.Create(imgUrl);
                webRequest.Timeout = 3000;
                webRequest.Proxy = null;

                //获取响应
                WebResponse webResponse = webRequest.GetResponse();

                //将读取的http图片转换为流模式
                Stream reader = webResponse.GetResponseStream();

                bitmap = new Bitmap(reader);

                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                webResponse.Close();
                webRequest.Abort();

            }
            catch (Exception err)
            {
                JTLog.log().ErrorFormat("获取http图片：{0},失败。失败原因：{1}", imgUrl, err);
                return bitmap;
            }

            return bitmap;
        }

        /// <summary>
        /// 获取http图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static bool getHttpImg(string imgUrl, ref Image Bitmap)
        {
            //Bitmap bitmap = null;
            try
            {
                //创建连接
                WebRequest webRequest = WebRequest.Create(imgUrl);
                webRequest.Proxy = null;
                //获取响应
                WebResponse webResponse = webRequest.GetResponse();
                //将读取的http图片转换为流模式
                Stream reader = webResponse.GetResponseStream();
                //将流转换为bit
                Bitmap = new Bitmap(reader);
                //内存释放
                webRequest.Abort();
                webResponse.Close();
                reader.Close();
                reader.Dispose();
            }
            catch (Exception err)
            {
                JTLog.log().ErrorFormat("获取http图片：{0},失败。失败原因：{1}", imgUrl, err);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取http图片,固定大小
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool getHttpImg(string imgUrl, ref Bitmap bitmap, int imgWidth, int imgHeight)
        {
            //Bitmap bitmap = null;
            try
            {
                //创建连接
                WebRequest webRequest = WebRequest.Create(imgUrl);
                webRequest.Proxy = null;
                //获取响应
                WebResponse webResponse = webRequest.GetResponse();
                //将读取的http图片转换为流模式
                Stream reader = webResponse.GetResponseStream();
                //将流转换为bit            
                bitmap = new Bitmap(reader);
                bitmap = new Bitmap(bitmap, imgWidth, imgHeight);
                //内存释放
                webRequest.Abort();
                webResponse.Close();
                reader.Close();
                reader.Dispose();
            }
            catch (Exception err)
            {
                JTLog.log().ErrorFormat("获取http图片：{0},失败。失败原因：{1}", imgUrl, err);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 下载http图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public static bool downLoadHttpImg(string imgUrl, string localPath)
        {
            Bitmap bitmap = null;
            if (getHttpImg(imgUrl, ref bitmap))
            {
                try
                {
                    bitmap.Save(localPath);
                    bitmap.Dispose();
                }
                catch (Exception err)
                {
                    JTLog.log().ErrorFormat("下载图片:{0},失败。失败原因：{1}", imgUrl, err);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 上传ftp图片
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="ftpPath"></param>
        /// <param name="ftpName"></param>
        /// <param name="userName"></param>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public static bool uploadFtpImg(string localPath, string ftpPath, string ftpName, string userName, string passwd)
        {
            Stream requestStream = null;
            Stream Stream = null;
            string URI = ftpPath + '/' + ftpName;
            WebRequest webRequest = null;
            WebResponse webResponse = null;
            //FileStream fileStream = null;
            try
            {
                FtpWebRequest uploadRequest = GetRequest(URI, userName, passwd);
                //FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(serverUrl);
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
                uploadRequest.UseBinary = true;

                FtpWebRequest Request = null;
                FtpWebResponse res = null;
                //读取本地文件
                if (localPath.TrimStart('\0').Substring(0, 7) == "http://")
                {
                    //创建连接
                    webRequest = WebRequest.Create(localPath);
                    webRequest.Proxy = null;
                    //获取响应
                    webResponse = webRequest.GetResponse();
                    //将读取的http图片转换为流模式
                    Stream = webResponse.GetResponseStream();
                }
                else if (localPath.TrimStart('\0').Substring(0, 6) == "ftp://")
                {
                    string ftpUserName = localPath.Split(':')[1].Split('/')[2];
                    string ftpPasswd = localPath.Split(':')[2].Split('@')[0];
                    Request = GetRequest(localPath, ftpUserName, ftpPasswd);
                    Request.Method = WebRequestMethods.Ftp.DownloadFile;

                    res = (FtpWebResponse)Request.GetResponse();
                    Stream = res.GetResponseStream();
                }
                else
                {
                    byte[] buff = File.ReadAllBytes(localPath);
                    Stream = new MemoryStream(buff);
                }
                uploadRequest.ContentLength = 1024;
                requestStream = uploadRequest.GetRequestStream();
                FtpWebResponse uploadResponse = null;

                byte[] buffer = new byte[2048];
                int bytesRead;
                //将图片上传到FTP
                while (true)
                {
                    bytesRead = Stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    requestStream.Write(buffer, 0, bytesRead);
                }
                requestStream.Close();
                uploadResponse = (FtpWebResponse)uploadRequest.GetResponse();
                uploadRequest.Abort();
                uploadResponse.Close();
                Stream.Close();
                Stream.Dispose();
                //内存释放
                if (Request != null)
                {
                    Request.Abort();
                }
                if (res != null)
                {
                    res.Close();
                }
            }
            catch (Exception err)
            {
                JTLog.log().ErrorFormat("将图片:{0},上传到FTp图片:{1},失败。失败原因：{2}", localPath, URI, err);
                return false;
            }
            finally
            {
                if (null != requestStream)
                {
                    requestStream.Dispose();
                }
                if (null != Stream)
                {
                    Stream.Dispose();
                }
                if (null != webRequest)
                {
                    webRequest.Abort();
                }
                if (null != webResponse)
                {
                    webResponse.Close();
                }


            }
            return true;
        }
        /// <summary>
        /// 获取FTP图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static bool getFtpImg(string imgUrl, string userName, string passwd, ref Bitmap bitmap)
        {
            try
            {

                //创建连接
                //FtpWebRequest frequest = (FtpWebRequest)WebRequest.Create(new Uri(imgUrl));
                //frequest.Method = WebRequestMethods.Ftp.DownloadFile;
                //frequest.UseBinary = true;

                //frequest.Credentials = new NetworkCredential(userName, passwd)

                FtpWebRequest Request = GetRequest(imgUrl, userName, passwd);
                //  FtpWebRequest Request = GetRequest("ftp://uploadusr:h3ckey@10.131.20.37/tollgate/14634864083732165.jpg", userName, passwd);
                Request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse res = (FtpWebResponse)Request.GetResponse();
                Stream reader = res.GetResponseStream();

                bitmap = new Bitmap(reader);
                //内存释放
                Request.Abort();
                res.Close();
                //reader.Close();
                //reader.Dispose();
            }
            catch (Exception err)
            {
                JTLog.log().ErrorFormat("获取ftp图片：{0},失败。失败原因：{1}", imgUrl, err);
                return false;
            }
            return true;
        }

        /// <summary> 
        /// 删除FTP文件 
        /// </summary> 
        /// <param name="filePath"></param> 
        public static bool fileDelete(string ftpPath, string ftpName, string ftpUser, string ftpPasswd)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            StreamReader streamReader = null;

            try
            {
                string uri = ftpPath + ftpName;
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                if (null == ftpWebRequest)
                {
                    return false;
                }
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;

                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                long size = ftpWebResponse.ContentLength;

                ftpResponseStream = ftpWebResponse.GetResponseStream();
                streamReader = new StreamReader(ftpResponseStream);

                string result = String.Empty;
                result = streamReader.ReadToEnd();



                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }

                if (ftpResponseStream != null)
                {
                    ftpResponseStream.Close();
                }

                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
                if (null != ftpWebRequest)
                {
                    ftpWebRequest.Abort();
                }
            }

            return success;
        }

        /// <summary>
        /// 验证用户FTP信息 
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static FtpWebRequest GetRequest(string URI, string username, string password)
        {
            //根据服务器信息FtpWebRequest创建类的对象
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(new Uri(URI));
            //提供身份验证信息
            result.Credentials = new NetworkCredential(username, password);
            //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
            result.KeepAlive = false;
            return result;
        }


        /// <summary> 
        /// 检查FTP文件是否存在 
        /// </summary> 
        /// <param name="ftpPath"></param> 
        /// <param name="ftpName"></param> 
        /// <returns></returns> 
        public static bool fileCheckExist(string ftpPath, string ftpName, string ftpUser, string ftpPasswd)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            WebResponse webResponse = null;
            StreamReader reader = null;

            try
            {
                ftpWebRequest = GetRequest(ftpPath, ftpUser, ftpPasswd);
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));
                ftpWebRequest.KeepAlive = false;
                webResponse = ftpWebRequest.GetResponse();
                reader = new StreamReader(webResponse.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line == ftpName)
                    {
                        success = true;
                        break;
                    }
                    line = reader.ReadLine();
                }
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
            return success;
        }


        /// <summary>
        /// 图片尺寸压缩
        /// </summary>
        /// <param name="sorPath">原路径</param>
        /// <param name="destPath">目标路径</param>
        /// <param name="width">新宽</param>
        /// <param name="height">心高</param>
        public static Bitmap CompressImage(Bitmap bp, float plus = 0.1f)
        {
            int width = 0;
            int height = 0;

            //if (bp.Width * bp.Height < 10000000)
            //{
            //    return bp;
            //}
            //else
            //{
            if (plus < 1)
            {
                width = (int)(bp.Width * plus);
                height = (int)(bp.Height * plus);
            }
            else
            {
                return bp;
            }
            //}

            MemoryStream msSource = new MemoryStream();
            bp.Save(msSource, System.Drawing.Imaging.ImageFormat.Jpeg);

            return CompressImage(msSource, width, height);
        }


        /// <summary>
        /// 图片尺寸压缩
        /// </summary>
        /// <param name="sorPath">原路径</param>
        /// <param name="destPath">目标路径</param>
        /// <param name="width">新宽</param>
        /// <param name="height">心高</param>
        public static Bitmap CompressImage(MemoryStream msSource, int width, int height)
        {
            byte[] buf = new byte[msSource.Length];//导入图片的buffer
            msSource.Position = 0;
            msSource.Read(buf, 0, buf.Length);

            byte[] resBuffer = new byte[msSource.Length];
            uint resLen = (uint)msSource.Length;

            var r = UV.IMOS.SDK.ImosSdkDef.flexibleImageV3(buf, (uint)buf.Length, resBuffer, ref resLen, (uint)width, (uint)height);
            if (r != 0)
            {
                JTComponet.JTLog.log().ErrorFormat("Failed to GetCopressImageByte,ErrorMessage:{0}", r);

                if (msSource != null)
                {
                    msSource.Close();
                    msSource.Dispose();
                }

                return null;
            }
            else
            {
                msSource.Close();
                msSource.Dispose();
            }

            Bitmap bpReulst = null;

            using (MemoryStream msResult = new MemoryStream(resBuffer))
            {
                bpReulst = new Bitmap(msResult);
            }

            return bpReulst;
        }

        public static Bitmap CompressImage(Image img, int width, int height)
        {
            if (img == null)
            {
                return null;
            }

            Bitmap bmpDest = new Bitmap(width, height);

            try
            {
                Graphics g = Graphics.FromImage(bmpDest);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, bmpDest.Size.Width, bmpDest.Size.Height);

                img.Dispose();
                g.Dispose();
            }
            catch (Exception ex)
            {
                JTLog.log().Error("CompressImage(Image img, int width, int height) function exception ", ex);
            }


            return bmpDest;
        }

        public static Bitmap GetImageThumb(Bitmap mg, Size newSize)
        {
            double ratio = 0d;
            double myThumbWidth = 0d;
            double myThumbHeight = 0d;
            int x = 0;
            int y = 0;

            Bitmap bp;

            if ((mg.Width / Convert.ToDouble(newSize.Width)) > (mg.Height /
            Convert.ToDouble(newSize.Height)))
                ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
            else
                ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
            myThumbHeight = Math.Ceiling(mg.Height / ratio);
            myThumbWidth = Math.Ceiling(mg.Width / ratio);

            Size thumbSize = new Size((int)newSize.Width, (int)newSize.Height);
            bp = new Bitmap(newSize.Width, newSize.Height);
            x = (newSize.Width - thumbSize.Width) / 2;
            y = (newSize.Height - thumbSize.Height);

            using (System.Drawing.Graphics g = Graphics.FromImage(bp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
                g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);
                mg.Dispose();
            }

            return bp;
        }

        #region 福州新增

        /// <summary>
        /// 获取xml中指定节点对应数据 nodes的长度和nodesValues等长
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nodes">节点名称/路径</param>
        /// <param name="nodesValues">值</param>
        /// <returns></returns>
        public static bool GetXmlNodesInfo(string filePath, List<string> nodes, List<string> nodesValues)
        {

            if (!File.Exists(filePath))
            {
                JTLog.log().ErrorFormat("当前XML文件不存在,请联系管理员!");
                return false;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(filePath);
            }
            catch (Exception ex)
            {
                JTLog.log().ErrorFormat("\"{0}\" is a illegal xml file.The error's reason is {1};StackInfo:{2}", filePath, ex.ToString(), ex.StackTrace);
                return false;
            }
            try
            {
                for (int i = 0; i < nodes.Count; ++i)
                {
                    XmlNodeList xnl = doc.GetElementsByTagName(nodes[i]);
                    nodesValues.Add(xnl.Count == 0 ? "" : xnl[0].InnerText);
                }
            }
            catch (Exception ex)
            {
                JTLog.log().ErrorFormat("\"{0}\" get nodesInfo Happened Exception,Error Reason is {1};StackInfo:{2}", filePath, ex.ToString(), ex.StackTrace);
                return false;
            }
            return true;
        }



        /// <summary>
        /// 下载录像
        /// </summary>
        /// <param name="_fileName"></param>
        /// <param name="_url"></param>
        /// <returns></returns>
        public static bool DownLoadVideo(string _fileName, string _url)
        {
            try
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                using (FileStream fs = new FileStream(_fileName, FileMode.Create))
                {
                    //打开网络连接
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    long fileSize = response.ContentLength;

                    using (Stream stream = response.GetResponseStream())
                    {
                        //定义一个字节数据
                        byte[] buffer = new byte[1024];

                        int length = stream.Read(buffer, 0, 1024);

                        while (length > 0)
                        {
                            fs.Write(buffer, 0, length);

                            Array.Clear(buffer, 0, buffer.Length);
                            length = stream.Read(buffer, 0, 1024);

                            int percent = (int)(fs.Length * 100 / fileSize);

                            ////显示进度
                            //if (_isQueryProgress && percent != _precent)
                            //{
                            //    OnProgressChanged();
                            //}

                            //_precent = percent;
                        }
                    }

                    response.Close();
                    sw.Stop();
                    JTLog.log().InfoFormat("DownLoadVideo cast times: {0}", sw.Elapsed.TotalMilliseconds);
                    return true;
                }
            }
            catch (Exception err)
            {
                //CSLog.Log.Write.Error(err);
                return false;
            }
            finally
            {
                // RecordDownBase.ManuStopDownLoad(_identityId, "下载完成");
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        public static bool CreateDirecotry(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception ex)
                {
                    //创建文件夹失败
                    return false;
                }
            }
            return true;
        }

        //删除指定文件夹下所有文件
        public void DeleteDirFiles(string path)
        {

        }
        #endregion
    }
}
