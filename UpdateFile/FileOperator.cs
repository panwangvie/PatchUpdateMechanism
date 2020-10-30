using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile
{
    public class FileOperator
    {
        private string _fileName;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get => _fileName; set => _fileName = value; }


        private OperatorTypeEnum operatorType = 0;
        /// <summary>
        /// 文件更新模式
        /// </summary>
        public OperatorTypeEnum OperatorType { get => operatorType; set => operatorType = value; }
    }

    
}
