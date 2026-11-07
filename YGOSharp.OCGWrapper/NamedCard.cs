using System;
using System.Collections.Generic;

namespace YGOSharp.OCGWrapper
{
    public class NamedCard : Card
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IReadOnlyList<string> Hint { get; private set; }
        public uint Category { get; private set; }

        internal NamedCard(CardRecord record) : base(record)
        {
            Name = record.Name ?? String.Empty;
            Description = record.Description ?? String.Empty;
            Hint = Array.AsReadOnly((string[])record.Hint.Clone());
            Category = record.Category;
        }

        public static new NamedCard Get(int id)
        {
            return NamedCardsManager.GetCard(id);
        }

        /// <summary>
        /// Returns whether a card ID and its alias represent alternate artwork.
        /// Aliases with an ID difference of 20 or more are rule-name aliases, not alternate artwork.
        /// </summary>
        public static bool IsAltartAlias(int cardId, int alias)
        {
            // YGOPro treats this normal-monster printing as a rule-name alias of the ritual monster.
            if (cardId == 5405695)
                return false;
            return cardId > 0 && alias > 0 && Math.Abs(cardId - alias) < 20;
        }
    }
}
