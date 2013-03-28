# VersionOne.RequestHelper

Example code for working with Requests using SDK.NET API Client.

## Design

### Setting Request Type
The SDK.NET Object Model treats "type" as a read-only property. The API Client can be used to change the property, which is known as "Category" in the Core API. However, there is no quick bridge to drop into the API Client; hence, RequestRepository is an attempt to bridge the 2 styles of access.

### Caching
The RequestCategoryRepository enables some useful caching of the list of categories. The list should be small enough to fit into memory. The list of categories is usually changing slowly, so the repository allows programmers to decide how aggressively to check for changes.

