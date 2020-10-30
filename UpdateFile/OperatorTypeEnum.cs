using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile
{
    /// <summary>
    /// 文件更新的模式
    /// </summary>
    public enum OperatorTypeEnum
    {
        /// <summary>
        /// 覆盖
        /// </summary>
        Cover = 0,

        /// <summary>
        /// 删除
        /// </summary>
        Delete=1,

        /// <summary>
        /// 忽略
        /// </summary>
        Ignore=2
    }
}
