using Microsoft.VisualStudio.TestTools.UnitTesting;
using PnP.Core.Model.SharePoint;
using PnP.Core.QueryModel.Query;
using PnP.Core.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnP.Core.Test.QueryModel
{
    [TestClass]
    public class QueryableConsistency
    {
        [TestMethod]
        public void TestQueryListsConsistency()
        {
            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var query = (from l in context.Web.Lists
                             select l)
                            .Load(l => l.Id, l => l.Title, l => l.Description);

                var queryResult = query.ToList();

                Assert.IsNotNull(queryResult);
                Assert.IsTrue(queryResult.Count >= 5);
                Assert.IsTrue(queryResult.Count(l => l.Title == "Site Pages") == 1);
                Assert.IsTrue(context.Web.Lists.Length >= 5);
                Assert.IsTrue(context.Web.Lists.Count(l => l.Title == "Site Pages") == 1);

                var web = context.Web.GetAsync(w => w.Lists);

                Assert.IsTrue(context.Web.Lists.Length >= 5);
                Assert.IsTrue(context.Web.Lists.Count(l => l.Title == "Site Pages") == 1);
            }
        }

        [TestMethod]
        public async Task TestQueryListsAddConsistency()
        {
            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                // Get the whole set of lists via LINQ
                var query = (from l in context.Web.Lists
                             select l)
                            .Load(l => l.Id, l => l.Title, l => l.Description);

                // Save the count of retrieved lists
                var queryResult = query.ToList();
                var listsCount = queryResult.Count;

                // There should be at least 5 lists
                Assert.IsNotNull(queryResult);
                Assert.IsTrue(listsCount >= 5);
                // There should be one and only one list with Title "Site Pages"
                Assert.IsTrue(queryResult.Count(l => l.Title == "Site Pages") == 1);

                // Now add a new list
                string listTitle = "TestQueryListsConsistency";
                var newList = context.Web.Lists.FirstOrDefault(l => l.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (newList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }
                else
                {
                    newList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Double-check that the number of lists is now increased by 1
                var newListsCount = context.Web.Lists.Length;
                Assert.AreEqual(listsCount + 1, newListsCount);

                // Now execute again the LINQ query
                queryResult = query.ToList();
                // and store the new number of list items
                listsCount = queryResult.Count;

                // Check if the number of list items is now increased by 1
                Assert.AreEqual(listsCount, newListsCount);
            }
        }

        [TestMethod]
        public async Task TestQueryListsDeleteConsistency()
        {
            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                // Get the whole set of lists via LINQ
                var query = (from l in context.Web.Lists
                             select l)
                            .Load(l => l.Id, l => l.Title, l => l.Description);

                // Save the count of retrieved lists
                var queryResult = query.ToList();
                var listsCount = queryResult.Count;

                // There should be at least 5 lists
                Assert.IsNotNull(queryResult);
                Assert.IsTrue(listsCount >= 5);
                // There should be one and only one list with Title "Site Pages"
                Assert.IsTrue(queryResult.Count(l => l.Title == "Site Pages") == 1);

                // Now add a new list
                string listTitle = "TestQueryListsDeleteConsistency";
                var newList = context.Web.Lists.FirstOrDefault(l => l.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (newList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }
                else
                {
                    newList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Double-check that the number of lists is now increased by 1
                var newListsCount = context.Web.Lists.Length;
                Assert.AreEqual(listsCount + 1, newListsCount);

                // Now delete the just added list
                await newList.DeleteAsync();

                // And double-check that the number of lists is now decreased by 1
                newListsCount = context.Web.Lists.Length;
                Assert.AreEqual(listsCount, newListsCount);

                // Now execute again the LINQ query
                queryResult = query.ToList();
                // and store the new number of list items
                listsCount = queryResult.Count;

                // Check if the number of list items is now increased by 1
                Assert.AreEqual(listsCount, newListsCount);
            }
        }

        [TestMethod]
        public async Task TestQueryListsUpdateConsistency()
        {
            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                // Get the whole set of lists via LINQ
                var query = (from l in context.Web.Lists
                             select l)
                            .Load(l => l.Id, l => l.Title, l => l.Description);

                // Save the count of retrieved lists
                var queryResult = query.ToList();
                var listsCount = queryResult.Count;

                // There should be at least 5 lists
                Assert.IsNotNull(queryResult);
                Assert.IsTrue(listsCount >= 5);
                // There should be one and only one list with Title "Site Pages"
                Assert.IsTrue(queryResult.Count(l => l.Title == "Site Pages") == 1);

                // Now add a new list
                string listTitle = "TestQueryListsUpdateConsistency";
                var newList = context.Web.Lists.FirstOrDefault(l => l.Title.Equals(listTitle, StringComparison.InvariantCultureIgnoreCase));

                if (newList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }
                else
                {
                    newList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Double-check that the number of lists is now increased by 1
                var newListsCount = context.Web.Lists.Length;
                Assert.AreEqual(listsCount + 1, newListsCount);

                // Now update the just added list
                var newTitle = $"{newList.Title} - Updated";
                newList.Title = newTitle;
                await newList.UpdateAsync();

                // Now double-check that the number of lists is still the same
                newListsCount = context.Web.Lists.Length;
                Assert.AreEqual(listsCount + 1, newListsCount);

                // And that the list title is updated both in memory
                Assert.AreEqual(newTitle, newList.Title);
                // and through a LINQ query
                var updatedList = context.Web.Lists.GetByTitle(newTitle);
                Assert.AreEqual(newTitle, updatedList.Title);

                // Now execute again the LINQ query
                queryResult = query.ToList();
                // and store the new number of list items
                listsCount = queryResult.Count;

                // Check if the number of list items is now increased by 1
                Assert.AreEqual(listsCount, newListsCount);
            }
        }

        [TestMethod]
        public void TestQueryListItemsConsistency()
        {
            var expectedListItemTitle = "Home";

            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var sitePages = context.Web.Lists.GetByTitle("Site Pages");

                // Retrieve a single item via LINQ query
                var query = (from i in sitePages.Items
                             where i.Title == expectedListItemTitle
                             select i)
                             .Load(l => l.Id, l => l.Title);

                var queryResult = query.ToList();

                // Check the result
                Assert.IsNotNull(queryResult);
                Assert.AreEqual(1, queryResult.Count);
                Assert.AreEqual(expectedListItemTitle, queryResult.FirstOrDefault()?.Title);
                var itemId = queryResult.FirstOrDefault()?.Id;

                // Now retrieve the same item via direct request
                if (itemId == null)
                {
                    Assert.Inconclusive("Test data set should be setup to have the initial items available.");
                }

                var listItem = sitePages.Items.GetById(itemId.Value);

                // Check that the item is the expected one
                Assert.IsNotNull(listItem);
                Assert.AreEqual(expectedListItemTitle, listItem.Title);
                Assert.AreEqual(itemId, listItem.Id);
            }
        }

        [TestMethod]
        public async Task TestQueryListItemsAddConsistency()
        {
            var listTitle = "TestQueryListItemsAddConsistency";
            var expectedNewListItemTitle = "New Item";

            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }
                else
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Retrieve all the list items via LINQ query
                var query = (from i in myList.Items
                             select i)
                             .Load(l => l.Id, l => l.Title);

                var queryResult = query.ToList();

                // Check the number of items in the list (should be 0)
                Assert.IsNotNull(queryResult);
                var retrievedItemsCount = queryResult.Count;
                Assert.AreEqual(myList.Items.Length, retrievedItemsCount);

                // Now add a new item
                Dictionary<string, object> values = new Dictionary<string, object>
                {
                    { "Title", expectedNewListItemTitle }
                };
                var listItem = await myList.Items.AddAsync(values);
                listItem = await listItem.GetAsync(); // TODO: Fix this behavior

                // Check that the new item is the expected one
                Assert.IsNotNull(listItem);
                Assert.AreEqual(expectedNewListItemTitle, listItem.Title);

                // Check the number of items in the list (should be incremented by 1)
                Assert.AreEqual(retrievedItemsCount + 1, myList.Items.Length);

                // Make a new query for list items
                queryResult = query.ToList();

                // Check again the number of items
                Assert.IsNotNull(queryResult);
                Assert.AreEqual(myList.Items.Length, queryResult.Count);
            }
        }

        [TestMethod]
        public async Task TestQueryListItemsDeleteConsistency()
        {
            var listTitle = "TestQueryListItemsDeleteConsistency";
            var expectedNewListItemTitle = "New Item";

            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }
                else
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Retrieve all the list items via LINQ query
                var query = (from i in myList.Items
                             select i)
                             .Load(l => l.Id, l => l.Title);

                var queryResult = query.ToList();

                // Check the number of items in the list (should be 0)
                Assert.IsNotNull(queryResult);
                var retrievedItemsCount = queryResult.Count;
                Assert.AreEqual(myList.Items.Length, retrievedItemsCount);

                // Now add a new item
                Dictionary<string, object> values = new Dictionary<string, object>
                {
                    { "Title", expectedNewListItemTitle }
                };
                var listItem = await myList.Items.AddAsync(values);

                // Check that the new item is the expected one
                Assert.IsNotNull(listItem);

                // Check the number of items in the list (should be incremented by 1)
                Assert.AreEqual(retrievedItemsCount + 1, myList.Items.Length);

                // Now delete the list item
                await listItem.DeleteAsync();

                // Check the number of items in the list (should be 0)
                Assert.AreEqual(retrievedItemsCount, myList.Items.Length);

                // Make a new query for list items
                queryResult = query.ToList();

                // Check again the number of items
                Assert.IsNotNull(queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryListItemsUpdateConsistency()
        {
            var listTitle = "TestQueryListItemsUpdateConsistency";
            var expectedNewListItemTitle = "New Item";
            var expectedUpdatedTitle = "Updated";

            // TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var myList = context.Web.Lists.GetByTitle(listTitle);

                if (myList != null)
                {
                    Assert.Inconclusive("Test data set should be setup to not have the list available.");
                }
                else
                {
                    myList = await context.Web.Lists.AddAsync(listTitle, ListTemplateType.GenericList);
                }

                // Retrieve all the list items via LINQ query
                var query = (from i in myList.Items
                             select i)
                             .Load(l => l.Id, l => l.Title);

                var queryResult = query.ToList();

                // Check the number of items in the list (should be 0)
                Assert.IsNotNull(queryResult);
                var retrievedItemsCount = queryResult.Count;
                Assert.AreEqual(myList.Items.Length, retrievedItemsCount);

                // Now add a new item
                Dictionary<string, object> values = new Dictionary<string, object>
                {
                    { "Title", expectedNewListItemTitle }
                };
                var listItem = await myList.Items.AddAsync(values);
                await listItem.GetAsync(); // TODO: Fix this behavior

                // Check that the new item is the expected one
                Assert.IsNotNull(listItem);
                var listItemId = listItem.Id;

                // Check the number of items in the list (should be incremented by 1)
                Assert.AreEqual(retrievedItemsCount + 1, myList.Items.Length);

                // Now delete the list item
                listItem.Values["Title"] = expectedUpdatedTitle;
                await listItem.UpdateAsync();

                // Make a new query for list items
                var updatedItem = myList.Items.GetById(listItemId);

                // Check the updated title in the just retrieved item
                Assert.AreEqual(updatedItem.Title, expectedUpdatedTitle);
            }
        }

        [TestMethod]
        public async Task TestQueryListFollowedByGraphQueryConsistency()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                // Load list via Linq query --> this should now ensure the GraphId property of the web model is populated
                var list = context.Web.Lists.GetByTitle("Site Pages", l => l.Id, l => l.Title, l => l.Description);

                // Get the list items -- will happen via Graph
                await list.GetAsync(p => p.Items);
            }
        }
    }
}