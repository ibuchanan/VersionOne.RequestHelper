using Microsoft.VisualStudio.TestTools.UnitTesting;
using VersionOne.SDK.APIClient;

namespace VersionOne.RequestHelper.Test
{
    [TestClass]
    public class RequestCategoryRepositoryTest
    {

        [TestMethod]
        public void New_repository_is_dirty()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // When I create a new repository with that connection
            var repository = new RequestCategoryRepository(cx);
            // Then it is initially dirty
            Assert.IsTrue(repository.IsDirty());
        }
        
        [TestMethod]
        public void Query_for_request_categories_is_scoped_to_RequestCategory()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // And a new repository with that connection
            var repository = new RequestCategoryRepository(cx);
            // When I build the query for request categories
            var query = repository.BuildQueryForAllRequestCategories();
            // Then the asset type is request categories
            Assert.AreEqual("RequestCategory", query.AssetType.Token);
        }

        [TestMethod]
        public void Query_for_request_categories_selects_name()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // And a new repository with that connection
            var repository = new RequestCategoryRepository(cx);
            // And a reference to the request category asset type
            var assetType = cx.MetaModel.GetAssetType("RequestCategory");
            // And a reference to the name attribute
            var nameAttribute = assetType.GetAttributeDefinition("Name");
            // When I build the query for request categories
            var query = repository.BuildQueryForAllRequestCategories();
            // Then the query selects the name attribute
            Assert.IsTrue(query.Selection.Contains(nameAttribute));
        }

        [TestMethod]
        public void Query_for_request_categories_selects_change_date()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // And a new repository with that connection
            var repository = new RequestCategoryRepository(cx);
            // And a reference to the request category asset type
            var assetType = cx.MetaModel.GetAssetType("RequestCategory");
            // And a reference to the change date attribute
            var changeDateAttr = assetType.GetAttributeDefinition("ChangeDateUTC");
            // When I build the query for request categories
            var query = repository.BuildQueryForAllRequestCategories();
            // Then the query selects the change date attribute
            Assert.IsTrue(query.Selection.Contains(changeDateAttr));
        }

        [TestMethod]
        public void Reload_is_clean()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // And a new repository with that connection
            var repository = new RequestCategoryRepository(cx);
            // When I reload the repository
            repository.Reload();
            // Then the repository is not dirty
            Assert.IsFalse(repository.IsDirty());
        }

        [TestMethod]
        public void Get_request_categories()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // And a new repository with that connection
            var repository = new RequestCategoryRepository(cx);
            // And that instance has a request category named "Feature"
            var requestType = cx.MetaModel.GetAssetType("RequestCategory");
            var nameAttribute = requestType.GetAttributeDefinition("Name");
            var query = new Query(requestType);
            var term = new FilterTerm(nameAttribute);
            term.Equal("Feature");
            query.Filter = term;
            var result = cx.Services.Retrieve(query);
            if (result.TotalAvaliable == 0)
            {
                var newRequestCategory = cx.Services.New(requestType, null);
                newRequestCategory.SetAttributeValue(nameAttribute, "Feature");
                cx.Services.Save(newRequestCategory);
            }
            // When I retrieve the request categories
            var requestCategories = repository.RetrieveRequestCategories();
            // Then my local cache of request categories has "Feature"
            Assert.IsTrue(requestCategories.ContainsKey("Feature"));
        }

    }
}
