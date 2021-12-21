using System;
using System.Collections.Generic;
using Sitecore.Diagnostics;
using Sitecore.Publishing.Pipelines.PublishItem;

namespace SitecoreHistory.Histories
{
    public class ItemPublished : PublishItemProcessor
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

            Log.Info(item.Paths.FullPath, this);

            if (!ItemChangeService.IsValidToSaveChange(item))
            {
                return;
            }

            var itemChange = new ItemChange()
            {
                Date = DateTime.Now,
                Editor = context.User.Name,
                ID = item.ID.Guid.ToString("N"),
                Path = item.Paths.FullPath,
                Language = item.Language.Name,
                ServerName = Environment.MachineName,
                Fields = new List<FieldChangeDetail>()
                {
                    new FieldChangeDetail()
                    {
                        Name = "Publish", NewValue = item.Version.Number.ToString()
                    }
                }
            };

            ItemChangeService.SaveChange(itemChange);
        }
    }
}