
namespace Sitecore.ListManagement
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
    public static int AddContactToList(ContactRepositoryBase contactRepository, ID contactId, Guid listId)
    {
      Assert.ArgumentNotNull(contactRepository, "contactRepository");
      Assert.ArgumentNotNull(contactId, "contactId");
      Assert.ArgumentCondition(listId != Guid.Empty, "listId", "Value cannot be empty Guid.\r\nParameter name: contactId");
      MethodInfo method = contactRepository.GetType().GetMethod("InternalAddContactToList", BindingFlags.NonPublic | BindingFlags.Instance);
      if (method == null)
      {
        return -1;
      }
      object[] parameters = new object[] { contactId, listId };
      return (int)method.Invoke(contactRepository, parameters);
    }

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
        if (contact2 != null)
        {
          ID id;
          if (ID.TryParse(tagValue, out id) && (AddContactToList(contactRepository, ID.Parse(contact.ContactId), id.Guid) != 0))
          {
            flag = false;
          }
          return flag;
        }
        contactRepository.SaveContact(contact, new ContactSaveOptions(false, null, null));
      }
      return flag;
    }

    public static int RemoveContactFromList(ContactRepositoryBase contactRepository, ID contactId, Guid listId)
    {
      Assert.ArgumentNotNull(contactRepository, "contactRepository");
      Assert.ArgumentNotNull(contactId, "contactId");
      Assert.ArgumentCondition(listId != Guid.Empty, "listId", "Value cannot be empty Guid.\r\nParameter name: contactId");
      MethodInfo method = contactRepository.GetType().GetMethod("InternalRemoveContactFromList", BindingFlags.NonPublic | BindingFlags.Instance);
      if (method == null)
      {
        return -1;
      }
      object[] parameters = new object[] { contactId, listId };
      return (int)method.Invoke(contactRepository, parameters);
    }

    public static bool RemoveTagFromContact(this ContactContext contact, ContactRepositoryBase contactRepository, string tagName, string tagValue, bool isMerge = false)
    {
      ID id;
      Assert.ArgumentNotNull(contact, "contact");
      Assert.ArgumentNotNull(contactRepository, "contactRepository");
      Assert.ArgumentNotNull(tagName, "tagName");
      Assert.ArgumentNotNull(tagValue, "tagValue");
      bool flag = false;
      if (isMerge)
      {
        Sitecore.Analytics.Tracking.Contact contact2 = contactRepository.LoadContactReadOnly(contact.ContactId);
        if (contact2 != null)
        {
          contactRepository.MergeContacts(contact, contact2);
        }
      }
      if (contact.Tags.GetAll(tagName).Any<string>(t => t == tagValue))
      {
        ITag tag = contact.Tags.Find(tagName);
        if (tag != null)
        {
          for (int i = 0; i < tag.Values.Count; i++)
          {
            if (tag.Values[i].Value == tagValue)
            {
              tag.Values.Remove(i);
              flag = true;
              i--;
            }
          }
        }
      }
      if ((flag && ID.TryParse(tagValue, out id)) && (RemoveContactFromList(contactRepository, ID.Parse(contact.ContactId), id.Guid) != 0))
      {
        flag = false;
      }
      return flag;
    }
  }
}
