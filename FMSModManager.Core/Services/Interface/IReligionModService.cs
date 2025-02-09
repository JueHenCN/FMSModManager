using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services.Interface
{
    public interface IReligionModService
    {
        /// <summary>
        /// 获取所有MOD
        /// </summary>
        /// <returns></returns>
        List<string> GetAvailableReligionMods();


        /// <summary>
        /// 获取指定MOD的数据
        /// </summary>
        ReligionModModel? GetReligionMod(string modName, bool isRefresh = false);


        /// <summary>
        /// 创建新的MOD
        /// </summary>
        /// <param name="modName"></param>
        /// <returns></returns>
        ReligionModModel CreateReligionMod(string modName);


        /// <summary>
        /// 更新MOD
        /// </summary>
        /// <param name="modName"></param>
        bool UpdateReligionMod(string modName, ReligionModModel? religionMod);


        /// <summary>
        /// 删除指定MOD
        /// </summary>
        bool DeleteReligionMod(string modName);


        /// <summary>
        /// 保存宗教集MOD
        /// </summary>
        /// <param name="modName"></param>
        bool SaveReligionMod(string modName);

    }
}
