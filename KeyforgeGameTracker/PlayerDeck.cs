using Microsoft.WindowsAzure.Storage.Table;

namespace KeyforgeGameTracker
{
    public class PlayerDeck : TableEntity
    {
        public PlayerDeck(string playerName, string deckName)
        {
            PartitionKey = playerName;
            RowKey = deckName;
        }

        public PlayerDeck() { }
    }
}
