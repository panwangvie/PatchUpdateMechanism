using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile.Interface
{
    /// <summary>
    /// 比较版本号是否一致
    /// </summary>
    public interface IComparisonVersion
    {
        /// <summary>
        /// 是否是同一个版本，检查是否需要更新
        /// </summary>
        /// <returns></returns>
        bool IsSameVersion(ref VersionInfo version);
        /// <summary>
        /// 版本号比较规则
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="oldVersion"></param>
        /// <returns></returns>
        bool VersionRule(string newVersion, string oldVersion);

        /// <summary>
        /// 补丁好比较规则
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="oldVersion"></param>
        /// <returns></returns>
        bool PatchRule(string newVersion, string oldVersion);
    }
}
