using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile.ViewModel
{
   public class VmMsgInfo
    {
        private string _verInfo = "";


        private string _updateInfo = "";
        /// <summary>
        /// 版本信息
        /// </summary>
        public string VerInfo { get => _verInfo; set => _verInfo = value; }
        /// <summary>
        /// 更新内容
        /// </summary>
        public string UpdateInfo { get => _updateInfo; set => _updateInfo = value; }
    }
}
