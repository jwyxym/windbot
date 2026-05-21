using System;
using System.Collections.Generic;
using WindBot.Game;

namespace WindBot.Game.AI
{
    public static class DecksManager
    {
        private class DeckInstance
        {
            public string Deck { get; private set; }
            public string Level { get; private set; }
            public Func<GameAI, Duel, Executor> Factory { get; private set; }

            public DeckInstance(string deck, string level, Func<GameAI, Duel, Executor> factory)
            {
                Deck = deck;
                Level = level;
                Factory = factory;
            }
        }

        private static Dictionary<string, DeckInstance> _decks;
        private static List<DeckInstance> _list;
        private static Random _rand;

        public static void Init()
        {
            _decks = new Dictionary<string, DeckInstance>();
            _rand = new Random();

            RegisterDecks();

            _list = new List<DeckInstance>();
            _list.AddRange(_decks.Values);

            Logger.WriteLine("Decks initialized, " + _decks.Count + " found.");
        }

        public static Executor Instantiate(GameAI ai, Duel duel)
        {
            DeckInstance infos;

            string deck = ai.Game.Deck;

            if (deck != null && _decks.ContainsKey(deck))
                infos = _decks[deck];
            else
            {
                do
                {
                    infos = _list[_rand.Next(_list.Count)];
                }
                while (infos.Level != "Normal");
            }

            Executor executor = infos.Factory(ai, duel);
            executor.Deck = infos.Deck;
            return executor;
        }

        private static void RegisterDecks()
        {
            foreach (BotList.BotItem bot in BotList.List)
                Register(bot);
        }

        private static void Register(BotList.BotItem bot)
        {
            _decks.Add(bot.Name, new DeckInstance(bot.Deck, "Normal", bot.Factory));
        }
    }
}

