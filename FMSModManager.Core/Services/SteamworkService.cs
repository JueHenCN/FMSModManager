using Steamworks.Ugc;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMSModManager.Core.Models;

namespace FMSModManager.Core.Services
{
    public class SteamworkService
    {
        private uint appId;

        public string GameFolder { get; set; }

        public bool IsValid => SteamClient.IsValid;

        public void Init(uint appId = 3035500)
        {
            try
            {
                this.appId = appId;
                SteamClient.Init(appId);
                GameFolder = SteamApps.AppInstallDir(appId);
            }
            catch (Exception ex)
            {
                // LogService.Error($"Steam初始化失败", ex);
            }
        }


        public (string, bool) GetGameFolder()
        {
            try
            {
                if (!SteamClient.IsValid)
                {
                    // LogService.Error($"SteamApi未正常初始化");
                    return ("SteamApi未正常初始化", false);
                }
                string installDir = SteamApps.AppInstallDir(appId);
                if (string.IsNullOrEmpty(installDir))
                {
                    // LogService.Error($"未寻找到游戏目录");
                    return ("未寻找到游戏目录", false);
                }
                return (installDir, true);
            }
            catch (Exception ex)
            {
                // LogService.Error($"Steam加载游戏失败", ex);
                return ("Steam加载游戏失败", false);
            }
        }

        public async Task<List<ModItemModel>> ReadAllModItem()
        {
            List<ModItemModel> list = new List<ModItemModel>();
            try
            {
                if (!SteamClient.IsValid)
                {
                    return list;
                }
                var query = Query.Items.WhereUserPublished();
                int page = 1;

                while (true)
                {
                    var pageList = await query.GetPageAsync(page);
                    if (pageList.HasValue)
                    {
                        var modValue = pageList.Value;
                        if (modValue.ResultCount < 1)
                        {
                            break;
                        }
                        foreach (var item in modValue.Entries)
                        {
                            var modInfo = new ModItemModel()
                            {
                                ModId = item.Id.Value,
                                Title = item.Title,
                                Description = item.Description,
                                CreateData = item.Created.ToString(),
                                UpdateData = item.Updated.ToString(),
                                Tags = item.Tags.ToList(),
                            };
                            list.Add(modInfo);
                        }
                    }
                    else
                    {
                        break;
                    }
                    page++;
                }
            }
            catch (Exception ex)
            {
                // LogService.Error($"读取Mod信息异常", ex);
            }
            return list;
        }

        public async Task<List<Item>> ReadAllMod()
        {
            List<Item> list = new List<Item>();
            try
            {
                if (!SteamClient.IsValid)
                {
                    return list;
                }
                var query = Query.Items.WhereUserPublished();
                int page = 1;

                while (true)
                {
                    var pageList = await query.GetPageAsync(page);
                    if (pageList.HasValue)
                    {
                        var modValue = pageList.Value;
                        if (modValue.ResultCount < 1)
                        {
                            break;
                        }
                        foreach (var item in modValue.Entries)
                        {
                            list.Add(item);
                        }
                    }
                    else
                    {
                        break;
                    }
                    page++;
                }
            }
            catch (Exception ex)
            {
                //LogService.Error($"读取Mod信息异常", ex);
            }
            return list;
        }

        public async Task<(bool, string)> PublishModUpdate(ModUploadModel modUpload, ProgressModel progress, bool isCreate = false)
        {
            try
            {
                if (!SteamClient.IsValid)
                {
                    return (false, "SteamApi未正常初始化");
                }
                var modInfo = modUpload.ModItem;
                Editor editor = Editor.NewCommunityFile;
                if (!isCreate)
                    editor = new Editor(modInfo.ModId);

                if (modUpload.IsUpdateModFile)
                {
                    editor = editor.WithContent(modUpload.ModFilePath);
                }
                if (modUpload.IsUpdatePreviewImage)
                {
                    editor = editor.WithPreviewFile(modUpload.ModPreviewImagePath);
                }
                if (modUpload.IsUpdateModInfo)
                {
                    foreach (var tag in modInfo.Tags)
                    {
                        editor = editor.WithTag(tag);
                    }
                    editor = editor
                        .WithTitle(modInfo.Title)
                        .WithDescription(modInfo.Description);
                }

                editor = editor.WithPublicVisibility();
                var result = await editor.SubmitAsync(progress);
                return (result.Success, result.Result.ToString());
            }
            catch (Exception e)
            {
                // LogService.Fatal($"推送Mod更新异常", e);
                return (false, $"推送Mod更新异常：{e.Message}");
            }
        }
    }
}
