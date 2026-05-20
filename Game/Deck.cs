using System;
using System.Collections.Generic;
using YGOSharp.OCGWrapper;

namespace WindBot.Game
{
    public class Deck
    {
        public IList<NamedCard> Cards { get; private set; }
        public IList<NamedCard> ExtraCards { get; private set; }
        public IList<NamedCard> SideCards { get; private set; }

        public Deck()
        {
            Cards = new List<NamedCard>();
            ExtraCards = new List<NamedCard>();
            SideCards = new List<NamedCard>();
        }

        private void AddNewCard(int cardId, bool sideDeck)
        {
            NamedCard newCard = NamedCard.Get(cardId);
            if (newCard == null)
                return;

            if (!sideDeck)
                AddCard(newCard);
            else
                SideCards.Add(newCard);
        }

        private void AddCard(NamedCard card)
        {
            if (card.IsExtraCard())
                ExtraCards.Add(card);
            else
                Cards.Add(card);
        }

        public static Deck Load(string name)
        {
            try
            {
                Deck deck = new Deck();
                int[][] deckData = DeckData.AiDecks.GetValueOrDefault(name);
                if (deckData != null && deckData.Length >= 2)  {
                    foreach (int id in deckData[0])
                        deck.AddNewCard(id, false);
                    foreach (int id in deckData[1])
                        deck.AddNewCard(id, true);
                }

                if (deck.Cards.Count > 60)
                    return null;
                if (deck.ExtraCards.Count > 15)
                    return null;
                if (deck.SideCards.Count > 15)
                    return null;

                return deck;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}