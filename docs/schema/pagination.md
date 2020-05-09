# Pagination

Pagination schema defines how a pagination response will be returned by the api supporting pagination.

### Links

| Link | Details |
| ---- | ---- |
| self | Link to current page |
| next | Link to next page of resources. This link is only rendered if there is next page available |
| previous | Link to previous page of resources. This link is only rendered if there is a previous page available |


### Properties

| Property | Details |
| -------- | ------- |
| pageSize | Size of page used for pagination |
| pageCount | Total count of pages of resources present |
| currentPageIndex | Index of page rendered by the index. Index of page is based on 1 and would be index of first page or resources |
| totalCount | Total number of resource present |
| data | Array of resources |
