using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace YGOSharp.OCGWrapper
{
    public static class NamedCardsManager
    {
        private static IDictionary<int, NamedCard> _cards = new Dictionary<int, NamedCard>();

        public static bool IsInitialized { get { return _cards.Count > 0; } }

        public static void Init(IEnumerable<CardRecord> records)
        {
            if (records == null)
                throw new ArgumentNullException("records");
            Dictionary<int, NamedCard> cards = new Dictionary<int, NamedCard>();
            foreach (CardRecord record in records)
            {
                if (record == null || record.Code == 0 || record.Code > Int32.MaxValue
                    || record.Alias > Int32.MaxValue || record.Level > 255
                    || record.LScale > 255 || record.RScale > 255
                    || record.Setcode == null || record.Setcode.Length != 16
                    || record.Hint == null || record.Hint.Length != 16)
                    throw new ArgumentException("Invalid ygopro_cdb_reader::Card record.", "records");
                NamedCard card = new NamedCard(record);
                cards[card.Id] = card;
            }
            if (cards.Count == 0)
                throw new ArgumentException("The card collection must not be empty.", "records");
            // Publish only after the entire batch has been validated and copied.
            _cards = cards;
        }

        public static void InitJson(byte[] utf8Json)
        {
            Init(JsonSerializer.Deserialize(utf8Json, CardRecordJsonContext.Default.CardRecordArray));
        }

        internal static NamedCard GetCard(int id)
        {
            NamedCard card;
            return _cards.TryGetValue(id, out card) ? card : null;
        }

        public static IList<NamedCard> GetAllCards()
        {
            return _cards.Values.ToList();
        }
    }
}
