using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services.Interface
{
    public interface ICultureModService
    {
        /// <summary>
        /// 获取指定MOD的数据
        /// </summary>
        CultureModModel? GetModData(string modName, bool isRefresh = false);

        /// <summary>
        /// 保存州名称变更
        /// </summary>
        void SaveStateNames(string modName);

        /// <summary>
        /// 保存城市名称变更
        /// </summary>
        void SaveCityNames(string modName);

        /// <summary>
        /// 保存政治体制变更
        /// </summary>
        void SavePoliticalSystems(string modName);
    }
}
