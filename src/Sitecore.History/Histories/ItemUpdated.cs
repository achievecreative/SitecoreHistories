using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;

namespace SitecoreHistory.Histories
{
    public class ItemUpdated
    {
        public ItemUpdated(string ignoreFields)
        {
            if (!string.IsNullOrEmpty(ignoreFields))
            {
                IgnoreFields = ignoreFields.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] IgnoreFields { get; private set; } = new[] { "__Updated", "__Revision" };

        public void OnItemSaving(object sender, EventArgs args)
        {
            try
            {
                SaveHistory(args);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void SaveHistory(EventArgs args)
        {
            var newItem = Event.ExtractParameter(args, 0) as Item;
            if (newItem == null)
            {
                return;
            }

            if (!ItemChangeService.IsValidToSaveChange(newItem))
            {
                return;
            }

            var origalItem = newItem.Database.GetItem(newItem.ID);

            var itemChange = GetChagnes(origalItem, newItem);

            ItemChangeService.SaveChange(itemChange);
        }

        private ItemChange GetChagnes(Item orignalItem, Item newItem)
        {
            var itemChange = new ItemChange()
            {
                ID = orignalItem.ID.Guid.ToString("N"),
                Path = orignalItem.Paths.FullPath,
                Date = DateTime.Now,
                Language = newItem.Language.Name,
                ServerName = Environment.MachineName,
            };

            if (HttpContext.Current != null)
            {
                itemChange.UserAgent = HttpContext.Current.Request.UserAgent;
                itemChange.Editor = HttpContext.Current.User.Identity.Name;
            }

            orignalItem.Fields.ReadAll();
            newItem.Fields.ReadAll();

            var changes = new List<FieldChangeDetail>();

            //Check version
            if (orignalItem.Version.Number != newItem.Version.Number)
            {
                changes.Add(new FieldChangeDetail()
                {
                    Name = "Version",
                    OldValue = orignalItem.Version.Number.ToString(),
                    NewValue = newItem.Version.Number.ToString()
                });
            }

            if (orignalItem.TemplateID != newItem.TemplateID)
            {
                changes.Add(new FieldChangeDetail()
                {
                    Name = "Template",
                    OldValue = orignalItem.TemplateID.Guid.ToString("N"),
                    NewValue = newItem.TemplateID.Guid.ToString("N")
                });
            }

            foreach (Field orignalField in orignalItem.Fields)
            {
                if (IgnoreFields.Contains(orignalField.Name))
                {
                    continue;
                }

                var newField = newItem.Fields[orignalField.Name];
                if (newField == null)
                {
                    continue;
                }

                if (!orignalField.Value.Equals(newField.Value, StringComparison.Ordinal))
                {
                    changes.Add(new FieldChangeDetail()
                    {
                        Name = orignalField.Name,
                        OldValue = orignalField.Value,
                        NewValue = newField.Value
                    });
                }
            }

            itemChange.Fields = changes;

            return itemChange;
        }
    }
}
