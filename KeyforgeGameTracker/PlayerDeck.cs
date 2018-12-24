using Microsoft.WindowsAzure.Storage.Table;
using System;

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

        public string Houses { get; set; }

        public Guid DeckId { get; set; }
    }
}
