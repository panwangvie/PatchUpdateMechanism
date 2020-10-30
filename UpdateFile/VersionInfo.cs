using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UpdateFile.Log;

namespace UpdateFile
{
    public class VersionInfo
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        // 应用
        public string App { get; set; }

        // 时间
        public string Date{ get; set; }

        //  md5密码
        public string MD5{ get; set; }

        // 客户端端名称
        public string ClientName;

        //对内使用， 补丁更新内容
        public string Reason{ get; set; }

        //  对外使用，补丁更新内容
        public string UpdateDesc{ get; set; }

        // 更新模式
        public int Model{ get; set; }

        // 是否更新客户端安装包,0:否，1:是
        public string UpdateApp{ get; set; }

        // 补丁大小
        public string PatchSize{ get; set; }

        // 补丁名称
        public string PatchName{ get; set; }

        // 更新的服务
        public string Server{ get; set; }

        //  用户名
        public string UserName{ get; set; }

        // 密码
        public string Passwd{ get; set; }

        #region (以xml格式保存VersionInfo.config配置文件,废弃采用josn格式保存)
        /// <summary>
        /// 设置配置文件
        /// </summary>
        public void SetXmlConfig(string path)
        {
            XmlDocument xd = new XmlDocument();
            if (!File.Exists(path))
            {
                XmlDeclaration xmldec = xd.CreateXmlDeclaration("1.0", "utf-8",null);
                xd.AppendChild(xmldec);
                XmlElement xmlelem = xd.CreateElement("", "configuration", "");
                xd.AppendChild(xmlelem);
            }
            else
            {
                xd.Load(path); 
               
            }

            var root = xd.DocumentElement;

            CreateEle(xd,root, "ClientName", ClientName);
            var a = root.SelectSingleNode("Version");
            CreateEle(xd,root, "UserName", UserName);
            CreateEle(xd,root, "App", App);
            CreateEle(xd,root, "Date", Date);
            CreateEle(xd,root, "MD5", MD5);
            CreateEle(xd,root, "Model", Model+"");
            CreateEle(xd,root, "Passwd", Passwd);
            CreateEle(xd,root, "PatchName", PatchName);
            CreateEle(xd,root, "PatchSize", PatchSize);
            CreateEle(xd,root, "Reason", Reason);
            CreateEle(xd,root, "Server", Server);
            CreateEle(xd,root, "UpdateApp", UpdateApp+"");
            CreateEle(xd,root, "UpdateDesc", UpdateDesc);
            CreateEle(xd,root, "Version", Version);
            //var cmxe = xd.CreateElement("ClientName");
            //cmxe.InnerText = ClientName;
            //root.AppendChild(cmxe);

            //var cmxe2 = xd.SelectSingleNode("UserName");  
            //      xd.CreateElement("ClientName"); cmxe2.InnerText = ClientName ;root.AppendChild(cmxe2);
            //var cmxe3 = xd.CreateElement("App"); cmxe3.InnerText = App ;root.AppendChild(cmxe3);
            //var cmxe4 = xd.CreateElement("Date"); cmxe4.InnerText = Date ;root.AppendChild(cmxe4);
            //var cmxe5 = xd.CreateElement("MD5"); cmxe5.InnerText = MD5 ;root.AppendChild(cmxe5);
            //var cmxe6 = xd.CreateElement("Model"); cmxe6.InnerText = Model + "" ;root.AppendChild(cmxe6);
            //var cmxe7 = xd.CreateElement("Passwd"); cmxe7.InnerText = Passwd ;root.AppendChild(cmxe7);
            //var cmxe8 = xd.CreateElement("PatchName"); cmxe8.InnerText = PatchName ;root.AppendChild(cmxe8);
            //var cmxe9 = xd.CreateElement("PatchSize"); cmxe9.InnerText = PatchSize ;root.AppendChild(cmxe9);
            //var cmxe10 = xd.CreateElement("Reason"); cmxe10.InnerText = Reason ;root.AppendChild(cmxe10);
            //var cmxe11 = xd.CreateElement("Server"); cmxe11.InnerText = Server ;root.AppendChild(cmxe11);
            //var cmxe12 = xd.CreateElement("UpdateApp"); cmxe12.InnerText = UpdateApp + "" ;root.AppendChild(cmxe12);
            //var cmxe13 = xd.CreateElement("UpdateDesc"); cmxe13.InnerText = UpdateDesc ;root.AppendChild(cmxe13);
            //var cmxe14 = xd.CreateElement("Version"); cmxe14.InnerText = Version ;root.AppendChild(cmxe14);

            xd.Save(path);

        }

        /// <summary>
        /// 修改或者创建节点
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void CreateEle(XmlDocument xd, XmlElement root ,string name,string value)
        {
            var cm = root.SelectSingleNode(name);
            if (cm == null)
            {
                cm = xd.CreateElement(name);
            }
            cm.InnerText = value; 
            root.AppendChild(cm);
        }

        /// <summary>
        /// 读取配置文件并生成配置文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VersionInfo GetVersionByXml(string path)
        {
           
            VersionInfo versionInfo = new VersionInfo();
            if (!File.Exists(path))
            {
                return versionInfo;
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            var root = xmlDocument.SelectSingleNode("configuration");
            versionInfo.ClientName=root.SelectSingleNode("ClientName")?.InnerText;
            versionInfo.App=root.SelectSingleNode("App")?.InnerText;
            versionInfo.Date=root.SelectSingleNode("Date")?.InnerText;
            versionInfo.MD5=root.SelectSingleNode("MD5")?.InnerText;
            int model;
            int.TryParse(root.SelectSingleNode("Model")?.InnerText, out model);
            versionInfo.Model = model;

            versionInfo.Passwd=root.SelectSingleNode("Passwd")?.InnerText;
            versionInfo.PatchName=root.SelectSingleNode("PatchName")?.InnerText;
            versionInfo.PatchSize=root.SelectSingleNode("PatchSize")?.InnerText;
            versionInfo.Reason=root.SelectSingleNode("Reason")?.InnerText;
            versionInfo.Server=root.SelectSingleNode("Server")?.InnerText;
            versionInfo.UpdateApp = root.SelectSingleNode("UpdateApp")?.InnerText;
            versionInfo.UpdateDesc=root.SelectSingleNode("UpdateDesc")?.InnerText;;
            versionInfo.Version=root.SelectSingleNode("Version")?.InnerText;
            versionInfo.UserName=root.SelectSingleNode("UserName")?.InnerText;

            return versionInfo;
        }
        #endregion

        #region (以json格式保存VersionInfo.config配置文件)
        /// <summary>
        /// json格式读取配置文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VersionInfo GetVersionByJson(string path)
        {
            VersionInfo versionInfo = new VersionInfo();
            try
            {
                if (!File.Exists(path))
                {
                    Logs.WriteLog($"目录{path}读取无文件");
                    return versionInfo;
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    string text = reader.ReadToEnd();
                    versionInfo = Json.Parse<VersionInfo>(text);

                }
            }catch(Exception ex)
            {

                Logs.WriteLog($"读取配置文件{path}失败； Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);

            }
            return versionInfo;
        }

        /// <summary>
        /// 将配置文件保存到文件
        /// </summary>
        /// <param name="path"></param>
        public void SetJosnConfig(string path)
        {
            try
            {
                string contents = Json.Stringify(this);
                File.WriteAllText(path, contents, Encoding.UTF8);
            }catch(Exception ex)
            {
                Logs.WriteLog($"保存至配置文件{path}失败;Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);
            }
        }

        #endregion
    }
}
