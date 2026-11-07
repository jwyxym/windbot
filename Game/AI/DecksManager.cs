using System;
using System.Collections.Generic;

namespace WindBot.Game.AI
{
    public static partial class DecksManager
    {
        private class DeckInstance
        {
            public string Name { get; private set; }
            public string Deck { get; private set; }
            public Func<GameAI, Duel, Executor> Factory { get; private set; }
            public string Level { get; private set; }
            public string Dialog { get; private set; }

            public DeckInstance(string name, string deck, Func<GameAI, Duel, Executor> factory, string level, string dialog)
            {
                Name = name;
                Deck = deck;
                Factory = factory;
                Level = level;
                Dialog = dialog;
            }
        }

        private static Dictionary<string, DeckInstance> _decks;
        private static List<KeyValuePair<string, DeckInstance>> _list;
        private static Random _rand;

        public static void Init()
        {
            _decks = new Dictionary<string, DeckInstance>();
            _list = new List<KeyValuePair<string, DeckInstance>>();
            _rand = Random.Shared;

            RegisterDecks();

            Logger.WriteLine("Decks initialized, " + _decks.Count + " found.");
        }

        private static void RegisterDeck(string name, DeckInstance deck)
        {
            _decks.Add(name, deck);
            _list.Add(new KeyValuePair<string, DeckInstance>(name, deck));
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
                    infos = _list[_rand.Next(_list.Count)].Value;
                }
                while (infos.Level != "Normal");
            }

            Executor executor = infos.Factory(ai, duel);
            executor.Deck = infos.Deck;
            if (String.IsNullOrEmpty(ai.Game.Username))
                ai.Game.Username = infos.Name;
            if (ai.Game.Dialog == null)
                ai.Game.Dialog = infos.Dialog;
            return executor;
        }

        public static bool HasDeck(string name)
        {
            return _decks.ContainsKey(name);
        }

        internal static string[][] GetDecks()
        {
            List<string[]> decks = new List<string[]>();
            foreach (KeyValuePair<string, DeckInstance> entry in _list)
                decks.Add(new[] { entry.Value.Name, entry.Key, entry.Value.Deck, entry.Value.Dialog, entry.Value.Level });
            return decks.ToArray();
        }
    }
}
