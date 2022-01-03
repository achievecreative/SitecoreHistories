using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using SitecoreHistory.Models;

namespace SitecoreHistory
{
    public static class ItemExtensions
    {
        public static ItemChange InitialItemHistoryInfo(this Item item)
        {
            if (item == null)
            {
                return null;
            }

            var itemChange = new ItemChange()
            {
                Id = item.ID.Guid.ToString("N"),
                Path = item.Paths.FullPath,
                Date = DateTime.Now,
                Language = item.Language.Name,
                ServerName = Environment.MachineName,
                ItemVersion = item.Version.Number,
                SitecoreVersion = Settings.GetAppSetting("Item.History.SitecoreVersion"),
                Environment = Settings.GetAppSetting("Item.History.Environment"),
                Editor = Sitecore.Context.User?.LocalName
            };

            return itemChange;
        }
    }
}