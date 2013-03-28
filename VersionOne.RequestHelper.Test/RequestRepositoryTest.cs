using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VersionOne.SDK.APIClient;
using VersionOne.SDK.ObjectModel;

namespace VersionOne.RequestHelper.Test
{
    [TestClass]
    public class RequestRepositoryTest
    {
        [TestMethod]
        public void Set_category_for_request()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // And a new request repository with that connection
            var requestRepository = new RequestRepository(cx);
            // And a new request category repository with that connection
            var requestCategoryRepository = new RequestCategoryRepository(cx);
            // And a connection to an instance of VersionOne at https://www14.v1host.com/v1sdktesting/ with user credentials for admin
            var instance = new V1Instance("https://www14.v1host.com/v1sdktesting/", "admin", "admin");
            // And a new schedule with 7 day length and no gap
            var schedule = instance.Create.Schedule(Name("Schedule"), Duration.Parse(@"7 Days"), Duration.Parse(@"0 Days"));
            // And a new parent project under the root with that schedule
            var rootProject = instance.Get.ProjectByID(AssetID.FromToken("Scope:0"));
            var requestProject = instance.Create.Project(Name("Request Project"), rootProject, DateTime.Now, schedule);
            // And a new request under that project
            var newRequest = requestProject.CreateRequest(Name("Request Item"));
            // And the OidToken for that request
            var newRequestOid = newRequest.ID.Token;
            // When I set the category for that request
            var categories = requestCategoryRepository.RetrieveRequestCategories();
            requestRepository.SetRequestCategory(newRequestOid, categories["Feature"]);
            // Then I can get "Feature" as the type through the Object Model
            Assert.AreEqual("Feature", newRequest.Type.ToString());
        }

        [TestMethod]
        public void Do_not_make_unnecessary_server_calls_for_setting_category_on_multiple_requests()
        {
            // Given a connection to a VersionOne instance defined in the app.config
            var cx = new EnvironmentContext();
            // And a new request repository with that connection
            var requestRepository = new RequestRepository(cx);
            // And a new request category repository with that connection
            var requestCategoryRepository = new RequestCategoryRepository(cx);
            // And a connection to an instance of VersionOne at https://www14.v1host.com/v1sdktesting/ with user credentials for admin
            var instance = new V1Instance("https://www14.v1host.com/v1sdktesting/", "admin", "admin");
            // And a new schedule with 7 day length and no gap
            var schedule = instance.Create.Schedule(Name("Schedule"), Duration.Parse(@"7 Days"), Duration.Parse(@"0 Days"));
            // And a new parent project under the root with that schedule
            var rootProject = instance.Get.ProjectByID(AssetID.FromToken("Scope:0"));
            var requestProject = instance.Create.Project(Name("Request Project"), rootProject, DateTime.Now, schedule);
            // And 2 new requests under that project
            var newRequest1 = requestProject.CreateRequest(Name("Request Item 1"));
            var newRequest2 = requestProject.CreateRequest(Name("Request Item 2"));
            // When I set the category for both requests
            var categories = requestCategoryRepository.RetrieveRequestCategories();
            requestRepository.SetRequestCategory(newRequest1.ID.Token, categories["Feature"]);
            requestRepository.SetRequestCategory(newRequest2.ID.Token, categories["Feature"]);
            // Then don't make unnecessary calls to the server
        }

        private static string Name(string prefix)
        {
            return (prefix + " Request Helper " + DateTime.Now);
        }

    }
}
