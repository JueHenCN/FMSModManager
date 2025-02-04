using Steamworks.Ugc;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMSModManager.Core.Models;
using Prism.Events;
using FMSModManager.Core.Events;

namespace FMSModManager.Core.Services
{
    public class SteamworkService
    {
        private readonly uint GAME_APP_ID;
        private readonly IEventAggregator _eventAggregator;

        public bool IsValid => SteamClient.IsValid;

        private string GameFolder { get; set; }

        public List<ModItemModel> SubscriptionModList { get; set; }

        public List<ModItemModel> PublishModList { get; set; }

        public SteamworkService(IEventAggregator eventAggregator, uint appId = 3035500)
        {
            GAME_APP_ID = appId;
            SubscriptionModList = new List<ModItemModel>();
            PublishModList = new List<ModItemModel>();
            _eventAggregator = eventAggregator;
        }

        public string GetGameFolder()
        {
            try
            {
                if (!SteamClient.IsValid) InitSteamworkService();
                GameFolder = SteamApps.AppInstallDir(GAME_APP_ID);

                return GameFolder;
            }
            catch (Exception ex)
            {
                LogService.Error($"Steam游戏目录载入失败", ex);
                return "";
            }

        }

        private void InitSteamworkService()
        {
            try
            {
                if (!SteamClient.IsValid)
                    SteamClient.Init(GAME_APP_ID);
            }
            catch (Exception ex)
            {
                LogService.Error($"SteamApi初始化失败", ex);
                throw;
            }
        }

        public List<string> GetAvailableSubscriptionMods()
        {
            return SubscriptionModList.Select(mod => mod.Title).ToList();
        }

        public List<string> GetAvailablePublishMods()
        {
            return PublishModList.Select(mod => mod.Title).ToList();
        }

        public async Task RefauseWorkShopModList()
        {
            try 
            {
                if (!IsValid) InitSteamworkService();

                var subscribedQuery = Query.Items.WhereUserSubscribed();
                int subscribedPage = 1;
                var subscribedList = new List<ModItemModel>();
                while (true)
                {
                    var pageList = await subscribedQuery.GetPageAsync(subscribedPage);
                    if (!pageList.HasValue || pageList.Value.ResultCount < 1) break;
                    foreach (var item in pageList.Value.Entries)
                    {
                        var modInfo = new ModItemModel(
                            item.Id.Value, item.Title, item.Description, item.Created.ToString(),
                            item.Updated.ToString(), true, item.Tags.ToList());
                        subscribedList.Add(modInfo);
                    }
                    subscribedPage++;
                }

                var publishedQuery = Query.Items.WhereUserPublished();
                int publishedPage = 1;
                var publishedList = new List<ModItemModel>();
                while (true)
                {
                    var pageList = await publishedQuery.GetPageAsync(publishedPage);
                    if (!pageList.HasValue || pageList.Value.ResultCount < 1) break;
                    foreach (var item in pageList.Value.Entries)
                    {
                        var modInfo = new ModItemModel(
                            item.Id.Value, item.Title, item.Description, item.Created.ToString(),
                            item.Updated.ToString(), true, item.Tags.ToList());
                        publishedList.Add(modInfo);
                    }
                    publishedPage++;
                }

                SubscriptionModList = subscribedList;
                PublishModList = publishedList;
                _eventAggregator.GetEvent<SteamworkServiceRefuseEvent>().Publish();
            }
            catch (Exception ex)
            {
                LogService.Error($"读取Mod信息异常", ex);
            }
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
