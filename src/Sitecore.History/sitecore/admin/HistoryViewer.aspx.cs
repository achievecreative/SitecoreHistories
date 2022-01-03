using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Sitecore.Configuration;
using Sitecore.sitecore.admin;
using SitecoreHistory.Models;

namespace SitecoreHistory.sitecore.admin
{
    public partial class HistoryViewer : AdminPage
    {
        private string ConnectionString
        {
            get { return Settings.GetSetting("Item.History.ConnectionString", "mongodb://localhost/"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckSecurity(true);

            Bind();
        }

        private void Bind()
        {
            Guid itemId;
            Guid.TryParse(Request["Id"], out itemId);

            var client = new MongoClient(ConnectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(Constants.DatabaseName);
            var collection = database.GetCollection(Constants.CollectionName);

            var query = collection.AsQueryable<SavedItemChange>();
            if (itemId != Guid.Empty)
            {
                query = query.Where(x => x.Id == itemId.ToString("N"));
            }

            var mongoQuery = ((MongoQueryable<SavedItemChange>)query).GetMongoQuery();

            var results = collection.FindAs<SavedItemChange>(mongoQuery).SetSortOrder(SortBy<SavedItemChange>.Descending(x => x.Date));

            results = results.SetSkip(0).SetLimit(50);

            rptHistories.DataSource = results.GroupBy(x => x.Id);
            rptHistories.DataBind();
        }

        protected string GetPath(object obj)
        {
            var itemChange = obj as IGrouping<string, ItemChange>;
            if (itemChange == null)
            {
                return " - ";
            }

            return itemChange.First().Path;
        }

        protected string GetChange(object obj)
        {
            var fieldChange = obj as FieldChangeDetail;
            if (fieldChange == null)
            {
                return string.Empty;
            }

            if (fieldChange.Name == Constants.FieldChangeName.Publish)
            {
                return $"Publish(Version: {fieldChange.NewValue})";
            }

            return $"<label>{fieldChange.Name}</label> updated";
        }
    }
}