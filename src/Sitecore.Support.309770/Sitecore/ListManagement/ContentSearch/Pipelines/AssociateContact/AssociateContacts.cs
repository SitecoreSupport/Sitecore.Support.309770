namespace Sitecore.Support.ListManagement.ContentSearch.Pipelines.AssociateContact
{
  using Sitecore.Analytics.Data;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Diagnostics;
  using Sitecore.ListManagement.ContentSearch.Model;
  using Sitecore.ListManagement.ContentSearch.Pipelines;
  using Sitecore.ListManagement.Data;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class AssociateContacts : Sitecore.ListManagement.ContentSearch.Pipelines.AssociateContact.AssociateContacts
  {
    public override void Process(AssociateContactsArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");
      Assert.IsTrue(arguments.ContactList.GetType() == typeof(ContactList), "Operation is not supported for segmented lists.");
      if (arguments.Contacts.Any<ContactData>() || (arguments.Contact != null))
      {
        int num = 0;
        ContactContext contact = arguments.Contact as ContactContext;
        if ((contact != null) && contact.AddTagToContact(this.ContactRepository, "ContactLists", arguments.ContactList.Id, true, "", ""))
        {
          num++;
        }
        ProcessContactsResult<ContactData> result = this.ContactProcessingProvider.ProcessContacts(arguments.Contacts, "ContactLists", arguments.ContactList.Id, "ListManagementUpdateContactTags_");
        string operationId = result.OperationId;
        int num2 = result.ProcessedContactsCount + num;
        arguments.IncrementRecipientsCount += num2 - result.PreviouslyAssociatedContactsCount;
        arguments.Contacts = result.Contacts;
        if (!string.IsNullOrEmpty(operationId))
        {
          arguments.AssociatedAssociationsCount = num2;
          if (arguments.BulkOperationsId == null)
          {
            arguments.BulkOperationsId = new List<string>();
          }
          arguments.BulkOperationsId.Add(operationId);
        }
      }
    }
  }
}
