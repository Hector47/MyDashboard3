using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using BlazorApp.Shared;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace BlazorApp.Api
{
    public class BookmarkEntity : TableEntity
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        public string Icon { get; set; }

        public int Order { get; set; }
    }
    public static class BookmarkFunction
    {

        //https://github.com/staticwebdev/blazor-starter


        [FunctionName("Bookmark")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req
            , [Table("Bookmark", Connection = "connstring")] CloudTable table,
            ILogger log)
        {
            var claim = StaticWebAppsAuth.Parse(req);
            var NameIdentifier = claim.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
            var listBookmark = new List<Bookmark>();
            TableQuery<BookmarkEntity> rangeQuery = new TableQuery<BookmarkEntity>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                        NameIdentifier.Value));


    //        TableQuery<BookmarkEntity> rangeQuery = new TableQuery<BookmarkEntity>().Where(
    //TableQuery.CombineFilters(
    //    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
    //        NameIdentifier.Value),
    //    TableOperators.And,
    //    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan,
    //        "t")));

            // Execute the query and loop through the results
            foreach (BookmarkEntity entity in
                await table.ExecuteQuerySegmentedAsync(rangeQuery, null))
            {
                listBookmark.Add(new Bookmark() { Title = entity.Title, Icon = entity.Icon, Order = entity.Order, Uri = entity.Uri });
                
            }



            return new OkObjectResult(listBookmark);
        }
        
    }
}
