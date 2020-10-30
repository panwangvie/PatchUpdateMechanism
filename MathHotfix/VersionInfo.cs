namespace MathHotfix
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
        public string Date { get; set; }

        //  md5密码
        public string MD5 { get; set; }

        // 客户端端名称
        public string ClientName;

        //对内使用， 补丁更新内容
        public string Reason { get; set; }

        //  对外使用，补丁更新内容
        public string UpdateDesc { get; set; }

        // 更新模式
        public int Model { get; set; }

        // 是否更新客户端安装包,0:否，1:是
        public string UpdateApp { get; set; }

        // 补丁大小
        public string PatchSize { get; set; }

        // 补丁名称
        public string PatchName { get; set; }

        // 更新的服务
        public string Server { get; set; }

        //  用户名
        public string UserName { get; set; }

        // 密码
        public string Passwd { get; set; }
    }
}