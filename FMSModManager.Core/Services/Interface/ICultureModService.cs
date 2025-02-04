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
        /// 获取所有MOD
        /// </summary>
        /// <returns></returns>
        List<string> GetAvailableCultureMods();

        /// <summary>
        /// 获取指定MOD的数据
        /// </summary>
        CultureModModel? GetCultureMod(string modName, bool isRefresh = false);

        /// <summary>
        /// 删除指定MOD
        /// </summary>
        bool DeleteCultureMod(string modName);
        
        /// <summary>
        /// 创建新的MOD
        /// </summary>
        /// <param name="modName"></param>
        /// <returns></returns>

        CultureModModel CreateCultureMod(string modName);

        /// <summary>
        /// 保存文化集MOD
        /// </summary>
        /// <param name="modName"></param>
        void SaveCultureMod(string modName);

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
