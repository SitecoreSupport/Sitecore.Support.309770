
namespace Sitecore.Support.ListManagement
{
  using Sitecore.Analytics.Data;
  using Sitecore.Analytics.DataAccess;
  using Sitecore.Analytics.Model.Entities;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using System.Runtime.InteropServices;

  public static class ContactExtensions
  {
    
    public static bool AddTagToContact(this ContactContext contact, ContactRepositoryBase contactRepository, string tagName, string tagValue, bool isMerge = false, string suffixDelimiter = "", string suffix = "")
    {
      Assert.ArgumentNotNull(contact, "contact");
      Assert.ArgumentNotNull(contactRepository, "contactRepository");
      Assert.ArgumentNotNull(tagName, "tagName");
      Assert.ArgumentNotNull(tagValue, "tagValue");
      Assert.ArgumentNotNull(suffixDelimiter, "suffixDelimiter");
      Assert.ArgumentNotNull(suffix, "suffix");
      bool flag = false;
      Sitecore.Analytics.Tracking.Contact contact2 = null;
      if (isMerge)
      {
        contact2 = contactRepository.LoadContactReadOnly(contact.ContactId);
        if (contact2 != null)
        {
          contactRepository.MergeContacts(contact, contact2);
        }
      }
      if (contact.Tags.GetAll(tagName).All<string>(t => t != tagValue))
      {
        flag = true;
        ITag tag = contact.Tags.Find(tagName);
        if (tag != null)
        {
          ITagValue local1 = tag.Values.Create();
          local1.Value = tagValue;
          local1.DateTime = DateTime.UtcNow;
        }
        else
        {
          contact.Tags.Add(tagName, tagValue);
        }
      }
      if (flag)
      {
        #region ---------------- Changes Start ------------------------
       // if (contact2 != null)
        //{
          //ID id;
          //if (ID.TryParse(tagValue, out id) && (AddContactToList(contactRepository, ID.Parse(contact.ContactId), id.Guid) != 0))
          //{
          //  flag = false;
          //}
          //return flag;

       // }
        contactRepository.SaveContact(contact, new ContactSaveOptions(false, null, null));

        #endregion ---------------- Changes End ------------------------
      }
      return flag;
    }
    
  }
}
