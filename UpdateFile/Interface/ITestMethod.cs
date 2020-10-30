using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile.Interface
{
    /// <summary>
    /// 检测更新的方式
    /// </summary>
    public interface ITestMethod
    {
 
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="methodPath"></param>
        /// <param name="methodDll"></param>
        /// <param name="t">参数</param>
        void Update(FtpInfo t);
    }
}
