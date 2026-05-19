using System;
using System.Collections.Generic;
using WindBot.Game.AI.Decks;

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
            Register("Albaz", "AI_Albaz", (ai, duel) => new AlbazExecutor(ai, duel));
            Register("Altergeist", "AI_Altergeist", (ai, duel) => new AltergeistExecutor(ai, duel));
            Register("Apophis", "AI_Apophis", (ai, duel) => new ApophisExecutor(ai, duel));
            Register("BE2025", "AI_BE2025", (ai, duel) => new BE2025Executor(ai, duel));
            Register("Blackwing", "AI_Blackwing", (ai, duel) => new BlackwingExecutor(ai, duel), "NotFinished");
            Register("Blue-Eyes", "AI_BlueEyes", (ai, duel) => new BlueEyesExecutor(ai, duel));
            Register("BlueEyesMaxDragon", "AI_BlueEyesMaxDragon", (ai, duel) => new BlueEyesMaxDragonExecutor(ai, duel));
            Register("Brave", "AI_Brave", (ai, duel) => new BraveExecutor(ai, duel));
            Register("Burn", "AI_Burn", (ai, duel) => new BurnExecutor(ai, duel), "Easy");
            Register("ChainBurn", "AI_ChainBurn", (ai, duel) => new ChainBurnExecutor(ai, duel));
            Register("CyberDragon", "AI_CyberDragon", (ai, duel) => new CyberDragonExecutor(ai, duel), "NotFinished");
            Register("DarkMagician", "AI_DarkMagician", (ai, duel) => new DarkMagicianExecutor(ai, duel));
            Register("Dogmatika", "AI_Dogmatika", (ai, duel) => new DogmatikaExecutor(ai, duel));
            Register("Dragun", "AI_Dragun", (ai, duel) => new DragunExecutor(ai, duel));
            Register("Dragunity", "AI_Dragunity", (ai, duel) => new DragunityExecutor(ai, duel));
            Register("Evilswarm", "AI_Evilswarm", (ai, duel) => new EvilswarmExecutor(ai, duel), "NotFinished");
            Register("Exosister", "AI_Exosister", (ai, duel) => new ExosisterExecutor(ai, duel));
            Register("FamiliarPossessed", "AI_FamiliarPossessed", (ai, duel) => new FamiliarPossessedExecutor(ai, duel));
            Register("Frog", "AI_Frog", (ai, duel) => new FrogExecutor(ai, duel), "Easy");
            Register("Gravekeeper", "AI_Gravekeeper", (ai, duel) => new GravekeeperExecutor(ai, duel), "NotFinished");
            Register("Graydle", "AI_Graydle", (ai, duel) => new GraydleExecutor(ai, duel), "NotFinished");
            Register("GrenMajuThunderBoarder", "AI_GrenMajuThunderBoarder", (ai, duel) => new GrenMajuThunderBoarderExecutor(ai, duel));
            Register("Horus", "AI_Horus", (ai, duel) => new HorusExecutor(ai, duel), "Easy");
            Register("Kashtira", "AI_Kashtira", (ai, duel) => new KashtiraExecutor(ai, duel));
            Register("Labrynth", "AI_Labrynth", (ai, duel) => new LabrynthExecutor(ai, duel));
            Register("Level VIII", "AI_Level8", (ai, duel) => new Level8Executor(ai, duel));
            Register("Lightsworn", "AI_Lightsworn", (ai, duel) => new LightswornExecutor(ai, duel), "NotFinished");
            Register("LightswornShaddoldinosour", "AI_LightswornShaddoldinosour", (ai, duel) => new LightswornShaddoldinosour(ai, duel));
            Register("Maliss", "AI_Maliss", (ai, duel) => new MalissExecutor(ai, duel));
            Register("MalissOCG", "AI_MalissOCG", (ai, duel) => new MalissOCGExecutor(ai, duel));
            Register("MathMech", "AI_Mathmech", (ai, duel) => new MathmechExecutor(ai, duel));
            Register("MokeyMokey", "AI_MokeyMokey", (ai, duel) => new MokeyMokeyExecutor(ai, duel), "Easy");
            Register("MokeyMokeyKing", "AI_MokeyMokeyKing", (ai, duel) => new MokeyMokeyKingExecutor(ai, duel), "Easy");
            Register("Neko", "AI_Neko", (ai, duel) => new NekoExecutor(ai, duel));
            Register("Nekroz", "AI_Nekroz", (ai, duel) => new NekrozExecutor(ai, duel), "NotFinished");
            Register("OldSchool", "AI_OldSchool", (ai, duel) => new OldSchoolExecutor(ai, duel), "Easy");
            Register("Orcust", "AI_Orcust", (ai, duel) => new OrcustExecutor(ai, duel));
            Register("Phantasm", "AI_Phantasm", (ai, duel) => new PhantasmExecutor(ai, duel));
            Register("PureWinds", "AI_PureWinds", (ai, duel) => new PureWindsExecutor(ai, duel));
            Register("Qliphort", "AI_Qliphort", (ai, duel) => new QliphortExecutor(ai, duel));
            Register("Rainbow", "AI_Rainbow", (ai, duel) => new RainbowExecutor(ai, duel));
            Register("Rank V", "AI_Rank5", (ai, duel) => new Rank5Executor(ai, duel));
            Register("Ryzeal", "AI_Ryzeal", (ai, duel) => new RyzealExecutor(ai, duel));
            Register("Salamangreat", "AI_Salamangreat", (ai, duel) => new SalamangreatExecutor(ai, duel));
            Register("SkyStriker", "AI_SkyStriker", (ai, duel) => new SkyStrikerExecutor(ai, duel));
            Register("ST1732", "AI_ST1732", (ai, duel) => new ST1732Executor(ai, duel));
            Register("SuperheavySamurai", "AI_SuperheavySamurai", (ai, duel) => new SuperheavySamuraiExecutor(ai, duel));
            Register("Swordsoul", "AI_Swordsoul", (ai, duel) => new SwordsoulExecutor(ai, duel));
            Register("Tearlaments", "AI_Tearlaments", (ai, duel) => new TearlamentsExecutor(ai, duel));
            Register("Test", "AI_Test", (ai, duel) => new DoEverythingExecutor(ai, duel), "Test");
            Register("Lucky", "AI_Test", (ai, duel) => new LuckyExecutor(ai, duel), "Test");
            Register("ThunderDragon", "AI_ThunderDragon", (ai, duel) => new ThunderDragonExecutor(ai, duel));
            Register("TimeThief", "AI_Timethief", (ai, duel) => new TimeThiefExecutor(ai, duel));
            Register("Toadally Awesome", "AI_ToadallyAwesome", (ai, duel) => new ToadallyAwesomeExecutor(ai, duel));
            Register("Trickstar", "AI_Trickstar", (ai, duel) => new TrickstarExecutor(ai, duel));
            Register("Witchcraft", "AI_Witchcraft", (ai, duel) => new WitchcraftExecutor(ai, duel));
            Register("Yosenju", "AI_Yosenju", (ai, duel) => new YosenjuExecutor(ai, duel));
            Register("Yubel", "AI_Yubel", (ai, duel) => new YubelExecutor(ai, duel));
            Register("Zefra", "AI_Zefra", (ai, duel) => new ZefraExecutor(ai, duel));
            Register("Zexal Weapons", "AI_ZexalWeapons", (ai, duel) => new ZexalWeaponsExecutor(ai, duel));
            Register("Zoodiac", "AI_Zoodiac", (ai, duel) => new ZoodiacExecutor(ai, duel));
        }

        private static void Register(string name, string file, Func<GameAI, Duel, Executor> factory, string level = "Normal")
        {
            _decks.Add(name, new DeckInstance(file, level, factory));
        }
    }
}

