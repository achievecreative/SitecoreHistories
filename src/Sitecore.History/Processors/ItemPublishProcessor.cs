using System;
using System.Collections.Generic;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Publishing.Pipelines.PublishItem;
using SitecoreHistory.Models;
using SitecoreHistory.Services;

namespace SitecoreHistory.Histories
{
    public class ItemPublishProcessor : PublishItemProcessor
    {
        public override void Process(PublishItemContext context)
        {
            try
            {
                SaveHistory(context);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void SaveHistory(PublishItemContext context)
        {
            if (context == null || context.PublishOptions == null || context.PublishOptions.TargetDatabase == null)
            {
                return;
            }

            var item = context.PublishHelper.GetSourceItem(context.ItemId);
            if (item == null)
            {
                return;
            }

#if DEBUG
            Log.Info(item.Paths.FullPath, this);
#endif

            if (!ItemChangeService.IsValidToSaveChange(item))
            {
                return;
            }

            var itemChange = item.InitialItemHistoryInfo();
            if (HttpContext.Current != null)
            {
                itemChange.UserAgent = HttpContext.Current.Request.UserAgent;
            }

            itemChange.Fields = new List<FieldChangeDetail>()
            {
                new FieldChangeDetail()
                {
                    Name = "Publish", NewValue = item.Version.Number.ToString()
                }
            };

            ItemChangeService.SaveChange(itemChange);
        }
    }
}