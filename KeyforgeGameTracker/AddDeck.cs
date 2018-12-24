
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace KeyforgeGameTracker
{
    public static class AddDeck
    {
        private static HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://www.keyforgegame.com") };

        [FunctionName("AddDeck")]
        [return: Table("PlayerDecks")]
        public static async Task<PlayerDeck> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "players/{player}/decks/{deck}")]HttpRequest req,
            string player, string deck, TraceWriter log)
        {
            log.Info("AddDeck function starting.");

            var requestTemplate = "api/decks/?page=1&page_size=10&search={0}&power_level=0,11&chains=0,24&ordering=-date";

            var deckResponse = await httpClient.GetAsync(string.Format(requestTemplate, deck));
            dynamic deckObj = await deckResponse.Content.ReadAsAsync<object>();

            // Response
            //{"count":1,"data":[{"id":"e9e59f6c-dfd5-4bf9-a460-e59accfa4b2a","name":"Xinia, Quarter Composer","expansion":341,"power_level":0,"chains":0,"wins":0,"losses":0,"is_my_deck":false,"cards":["1ad71526-2782-4e56-a7b9-a0579fd63688","9c613507-63b6-447b-9df5-a72a5d62fdf3","22476b3c-d05c-4274-ad8f-ec1efabad116","c68fa80b-08b2-4153-88c1-9e56aac487fe","652c4e38-c4fa-4e30-8f8d-036e95249529","652c4e38-c4fa-4e30-8f8d-036e95249529","29eafbc7-7fc5-4239-b2a7-8bc90df1fc0f","d4f666db-302f-43d0-b0af-bd03071f92ce","d792387f-8392-49b3-ad7c-ccaf7552256f","4064612e-602e-46c1-b8eb-8adda1cfd0d0","e1312fbf-c297-4d9f-b403-2d892271de62","bb0dc3dc-b591-447d-a181-1dc4907e3eaa","d2edea65-7c2f-487f-a6f4-f44a077c4a65","10715fd2-031a-47ca-9119-9b7b2ec1d2c0","be492d70-5c87-441e-8223-79fb2bce85c9","5607fecd-b90e-4e12-84bc-cb36d079117c","ea2a390e-e121-4cbd-96c5-2430cc600e81","750a9323-9c07-4ae7-be5e-79367b4a2a8d","96548d93-b318-40e3-9f5c-3297c8070ebd","2d0d0224-b954-47df-9bed-9161a7742815","f97316b0-75a4-45a4-8735-15e72cc1568c","f97316b0-75a4-45a4-8735-15e72cc1568c","6b113c63-c8e0-4c52-9973-b94263d2bf0d","9ed7d241-1ca3-4a2a-b067-bb44776f7d4b","ce051448-1745-4606-95a0-e44e70401ba1","1ca5f524-5a24-4a58-aacf-8204bdb46a32","2cb1f58c-5979-4d3a-ae86-9dadc6000288","3515de43-9c9a-4ec8-bced-d2d21ff24824","953bbe23-df2b-4459-a3f6-beca7cd49a34","3c5c1881-486c-4911-a3ce-497ef258e8ba","45d564a2-fcc9-4baa-8dc8-8e1a0fe2a37a","5a1ee413-4b39-467f-a0bf-e5935f1edf9b","5a1ee413-4b39-467f-a0bf-e5935f1edf9b","d6aae364-d547-49ec-83dd-be3ffbcb80c6","026fa764-3bf2-40fc-9182-b34f0acfb760","60f095d7-1816-4f14-88ec-04412ebde43b"],"notes":[],"is_my_favorite":false,"casual_wins":0,"casual_losses":0,"_links":{"houses":["Brobnar","Dis","Logos"]}}],"_linked":{"houses":[{"id":"Brobnar","name":"Brobnar","image":"https://cdn.keyforgegame.com/media/houses/Brobnar_RTivg44.png"},{"id":"Dis","name":"Dis","image":"https://cdn.keyforgegame.com/media/houses/Dis_OooSNPO.png"},{"id":"Logos","name":"Logos","image":"https://cdn.keyforgegame.com/media/houses/Logos_2mOY1dH.png"}]}}

            //https://cdn.keyforgegame.com/media/houses/Dis_OooSNPO.png
            //https://cdn.keyforgegame.com/media/houses/Logos_2mOY1dH.png
            //https://cdn.keyforgegame.com/media/houses/Brobnar_RTivg44.png
            //https://cdn.keyforgegame.com/media/houses/Mars_CmAUCXI.png
            //https://cdn.keyforgegame.com/media/houses/Sanctum_lUWPG7x.png
            //https://cdn.keyforgegame.com/media/houses/Shadows_z0n69GG.png
            //https://cdn.keyforgegame.com/media/houses/Untamed_bXh9SJD.png

            var playerDeck = new PlayerDeck(player, deck);

            if (deckObj.count == 1)
            {
                playerDeck.DeckId = deckObj.data[0].id;
                playerDeck.RowKey = deckObj.data[0].name;
                playerDeck.Houses = string.Join(",", deckObj.data[0]._links.houses);
            }

            return playerDeck;
        }
    }
}
