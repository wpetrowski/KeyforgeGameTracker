
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace KeyforgeGameTracker
{
    public static class GetPlayerDecks
    {
        [FunctionName("GetPlayerDecks")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "players/{player}/decks")]HttpRequest req,
            [Table("PlayerDecks", "MyPartition")] CloudTable decksTable,
            string player, TraceWriter log)
        {
            log.Info("GetPlayerDecks function starting");
            
            var query = new TableQuery<PlayerDeck>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, player));

            var results = new List<PlayerDeck>();

            TableQuerySegment<PlayerDeck> segment = null;
            while (segment == null || segment.ContinuationToken != null)
            {
                segment = await decksTable.ExecuteQuerySegmentedAsync(query, segment?.ContinuationToken);
                results.AddRange(segment.Results);
            }

            var deckUrlTemplate = "https://www.keyforgegame.com/deck-details/{0}";

            return new OkObjectResult(results.Select(x => new { Player = x.PartitionKey, Deck = x.RowKey, Houses = x.Houses, DeckUrl = string.Format(deckUrlTemplate, x.DeckId) }));
        }
    }
}
