using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Sitecore.Configuration;
using Sitecore.sitecore.admin;
using SitecoreHistory.Models;

namespace SitecoreHistory.sitecore.admin
{
    public partial class HistoryViewer : AdminPage
    {
        private int PageIndex { get; set; }

        private int PageSize { get; set; } = 100;

        private string ConnectionString
        {
            get { return Settings.GetSetting("Item.History.ConnectionString", "mongodb://localhost/"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckSecurity(true);

            if (!Page.IsPostBack)
            {
                Bind();
            }
        }

        private void Bind()
        {
            txtKeywords.Text = Request["Id"];
            var date = Request["date"];
            if (!string.IsNullOrEmpty(date))
            {
                dpDate.SelectedIndex = dpDate.Items.IndexOf(dpDate.Items.FindByValue(date));
            }

            if (dpDate.SelectedIndex == -1)
            {
                dpDate.SelectedIndex = 0;
            }

            if (int.TryParse(Request["pageIndex"], out int _pageIndex))
            {
                PageIndex = _pageIndex;
            }

            if (PageIndex < 0)
            {
                PageIndex = 0;
            }

            doSearch();
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

        private void doSearch()
        {
            Guid itemId;
            Guid.TryParse(txtKeywords.Text, out itemId);

            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(Constants.DatabaseName);
            var collection = database.GetCollection<SavedItemChange>(Constants.CollectionName);

            var query = collection.AsQueryable<SavedItemChange>();
            if (itemId != Guid.Empty)
            {
                query = query.Where(x => x.ItemId == itemId.ToString("N"));
            }
            else if (!string.IsNullOrEmpty(txtKeywords.Text))
            {
                var path = txtKeywords.Text.Trim();
                query = query.Where(x => x.Path.StartsWith(path));
            }

            if (int.TryParse(dpDate.SelectedValue, out int _date))
            {
                var lastDate = DateTime.Now.AddDays(-_date);
                query = query.Where(x => x.Date > lastDate);
            }

            IMongoQueryable<SavedItemChange> results = query.OrderByDescending(x => x.Date);
            var total = results.Count();

            var totalPages = total / PageSize + (total % PageSize > 0 ? 1 : 0);
            if (PageIndex > totalPages)
            {
                PageIndex = 0;
            }

            var skipCount = PageIndex * PageSize;

            results = results.Skip(skipCount).Take(PageSize);

            rptHistories.DataSource = results.ToList().GroupBy(x => x.ItemId);
            rptHistories.DataBind();

            if (totalPages > 1)
            {
                rptPagination.DataSource = Enumerable.Range(0, totalPages);
                rptPagination.DataBind();
            }
        }

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            PageIndex = 0;

            doSearch();
        }

        protected string Keyword
        {
            get { return txtKeywords.Text; }
        }

        protected string Date
        {
            get
            {
                return dpDate.SelectedValue;
            }
        }
    }
}