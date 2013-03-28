using VersionOne.SDK.APIClient;

namespace VersionOne.RequestHelper
{
    public class RequestRepository
    {
        private readonly EnvironmentContext _cx;
        private readonly IAssetType _requestType;
        private readonly IAttributeDefinition _categoryAttribute;

        public RequestRepository(EnvironmentContext context)
        {
            _cx = context;
            _requestType = _cx.MetaModel.GetAssetType("Request");
            _categoryAttribute = _requestType.GetAttributeDefinition("Category");
        }

        public void SetRequestCategory(string requestOidToken, string categoryOidToken)
        {
            var request = new Asset(_cx.Services.GetOid(requestOidToken));
            request.SetAttributeValue(_categoryAttribute, _cx.Services.GetOid(categoryOidToken));
            _cx.Services.Save(request);
        }
    }
}
