using System;
using System.Collections.Generic;
using System.Globalization;
using VersionOne.SDK.APIClient;

namespace VersionOne.RequestHelper
{
    public class RequestCategoryRepository
    {
        private readonly EnvironmentContext _cx;
        private readonly IAssetType _requestCategoryType;
        private readonly IAttributeDefinition _nameAttribute;
        private readonly IAttributeDefinition _changeAttribute;
        private readonly Query _queryForAllRequestCategories;
        private DateTime? _mostRecentChangeDateTime;
        private IDictionary<string, string> _allRequestCategories;

        public RequestCategoryRepository(EnvironmentContext context)
        {
            _cx = context;
            _requestCategoryType = _cx.MetaModel.GetAssetType("RequestCategory");
            _nameAttribute = _requestCategoryType.GetAttributeDefinition("Name");
            _changeAttribute = _requestCategoryType.GetAttributeDefinition("ChangeDateUTC");
            _queryForAllRequestCategories = BuildQueryForAllRequestCategories();
        }

        public bool IsDirty()
        {
            if (!_mostRecentChangeDateTime.HasValue)
            {
                return true;
            }
            var query = new Query(_requestCategoryType);
            query.Selection.Add(_changeAttribute);
            var term = new FilterTerm(_changeAttribute);
            term.Greater(_mostRecentChangeDateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture));
            query.Filter = term;
            var result = _cx.Services.Retrieve(query);
            return result.TotalAvaliable > 0;
        }

        public void Reload()
        {
            _allRequestCategories = new Dictionary<string, string>();
            var result = _cx.Services.Retrieve(_queryForAllRequestCategories);
            foreach (var asset in result.Assets)
            {
                _allRequestCategories.Add(asset.GetAttribute(_nameAttribute).Value.ToString(), asset.Oid.Token);
                // Remember the most recent change to VersionOne for checking dirty state
                var changeDateTime = DB.DateTime(asset.GetAttribute(_changeAttribute).Value);
                if ((!_mostRecentChangeDateTime.HasValue) || (changeDateTime > _mostRecentChangeDateTime))
                {
                    _mostRecentChangeDateTime = changeDateTime;
                }                
            }
        }

        public Query BuildQueryForAllRequestCategories()
        {
            var query = new Query(_requestCategoryType);
            query.Selection.Add(_nameAttribute);
            query.Selection.Add(_changeAttribute);
            return query;
        }

        /// <summary>
        /// Poll server for new or updated categories. If there is something new, then retreives the whole list.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> RetrieveRequestCategories()
        {
            if (IsDirty())
            {
                Reload();
            }
            return _allRequestCategories;
        }
    }
}