using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using SitecoreHistory.Models;

namespace SitecoreHistory.Services
{
    internal static class ItemChangeService
    {
        static ItemChangeService()
        {
            ConnectionString = Settings.GetSetting("Item.History.ConnectionString", "mongodb://localhost/");
        }

        private static string ConnectionString { get; set; }

        public static void SaveChange(ItemChange itemChange)
        {
            var debugEnabled = HttpContext.Current?.IsDebuggingEnabled;

            Task.Factory.StartNew(() =>
            {
                if ((debugEnabled ?? false))
                {
                    using (var writer = new StringWriter())
                    {
                        JsonSerializer.Create().Serialize(writer, itemChange);
                        Log.Info(writer.ToString(), typeof(ItemChangeService));
                    }
                }

                var client = new MongoClient(ConnectionString);
                var server = client.GetServer();
                var db = server.GetDatabase(Constants.DatabaseName);
                var collection = db.GetCollection(typeof(ItemChange), Constants.CollectionName);

                collection.Insert(typeof(ItemChange), itemChange);
            });
        }

        public static bool IsValidToSaveChange(Item item)
        {
            if (item == null)
            {
                return false;
            }

            var acceptPaths = Settings.GetSetting("Item.History.RootPaths", string.Empty).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (acceptPaths.Any(x => item.Paths.FullPath.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }
    }
}