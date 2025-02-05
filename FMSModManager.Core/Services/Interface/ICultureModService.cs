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
        /// 创建新的MOD
        /// </summary>
        /// <param name="modName"></param>
        /// <returns></returns>
        CultureModModel CreateCultureMod(string modName);

        /// <summary>
        /// 更新MOD
        /// </summary>
        /// <param name="modName"></param>
        bool UpdateCultureMod(string modName, CultureModModel? cultureMod);

        /// <summary>
        /// 删除指定MOD
        /// </summary>
        bool DeleteCultureMod(string modName);

        /// <summary>
        /// 保存文化集MOD
        /// </summary>
        /// <param name="modName"></param>
        bool SaveCultureMod(string modName);
    }
}
