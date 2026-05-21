using System;
using System.Collections.Generic;
using WindBot.Game.AI;
using WindBot.Game.AI.Decks;
namespace WindBot.Game
{
    public static class BotList
    {
        public record BotItem(
            string Name,
            string Deck,
            string Dialog,
            Func<GameAI, Duel, Executor> Factory
        );

        public static List<BotItem> List = new List<BotItem>()
        {
            new BotItem("Albaz", "AI_Albaz", (ai, duel) => new AlbazExecutor(ai, duel)),
            new BotItem("Altergeist", "AI_Altergeist", (ai, duel) => new AltergeistExecutor(ai, duel)),
            new BotItem("Apophis", "AI_Apophis", (ai, duel) => new ApophisExecutor(ai, duel)),
            new BotItem("BE2025", "AI_BE2025", (ai, duel) => new BE2025Executor(ai, duel)),
            new BotItem("Blackwing", "AI_Blackwing", (ai, duel) => new BlackwingExecutor(ai, duel)),
            new BotItem("Blue-Eyes", "AI_BlueEyes", (ai, duel) => new BlueEyesExecutor(ai, duel)),
            new BotItem("BlueEyesMaxDragon", "AI_BlueEyesMaxDragon", (ai, duel) => new BlueEyesMaxDragonExecutor(ai, duel)),
            new BotItem("Brave", "AI_Brave", (ai, duel) => new BraveExecutor(ai, duel)),
            new BotItem("Burn", "AI_Burn", (ai, duel) => new BurnExecutor(ai, duel)),
            new BotItem("ChainBurn", "AI_ChainBurn", (ai, duel) => new ChainBurnExecutor(ai, duel)),
            new BotItem("CyberDragon", "AI_CyberDragon", (ai, duel) => new CyberDragonExecutor(ai, duel)),
            new BotItem("DarkMagician", "AI_DarkMagician", (ai, duel) => new DarkMagicianExecutor(ai, duel)),
            new BotItem("Dogmatika", "AI_Dogmatika", (ai, duel) => new DogmatikaExecutor(ai, duel)),
            new BotItem("Dragun", "AI_Dragun", (ai, duel) => new DragunExecutor(ai, duel)),
            new BotItem("Dragunity", "AI_Dragunity", (ai, duel) => new DragunityExecutor(ai, duel)),
            new BotItem("Evilswarm", "AI_Evilswarm", (ai, duel) => new EvilswarmExecutor(ai, duel)),
            new BotItem("Exosister", "AI_Exosister", (ai, duel) => new ExosisterExecutor(ai, duel)),
            new BotItem("FamiliarPossessed", "AI_FamiliarPossessed", (ai, duel) => new FamiliarPossessedExecutor(ai, duel)),
            new BotItem("Frog", "AI_Frog", (ai, duel) => new FrogExecutor(ai, duel)),
            new BotItem("Gravekeeper", "AI_Gravekeeper", (ai, duel) => new GravekeeperExecutor(ai, duel)),
            new BotItem("Graydle", "AI_Graydle", (ai, duel) => new GraydleExecutor(ai, duel)),
            new BotItem("GrenMajuThunderBoarder", "AI_GrenMajuThunderBoarder", (ai, duel) => new GrenMajuThunderBoarderExecutor(ai, duel)),
            new BotItem("Horus", "AI_Horus", (ai, duel) => new HorusExecutor(ai, duel)),
            new BotItem("Kashtira", "AI_Kashtira", (ai, duel) => new KashtiraExecutor(ai, duel)),
            new BotItem("Labrynth", "AI_Labrynth", (ai, duel) => new LabrynthExecutor(ai, duel)),
            new BotItem("Level VIII", "AI_Level8", (ai, duel) => new Level8Executor(ai, duel)),
            new BotItem("Lightsworn", "AI_Lightsworn", (ai, duel) => new LightswornExecutor(ai, duel)),
            new BotItem("LightswornShaddoldinosour", "AI_LightswornShaddoldinosour", (ai, duel) => new LightswornShaddoldinosour(ai, duel)),
            new BotItem("Maliss", "AI_Maliss", (ai, duel) => new MalissExecutor(ai, duel)),
            new BotItem("MalissOCG", "AI_MalissOCG", (ai, duel) => new MalissOCGExecutor(ai, duel)),
            new BotItem("MathMech", "AI_Mathmech", (ai, duel) => new MathmechExecutor(ai, duel)),
            new BotItem("MokeyMokey", "AI_MokeyMokey", (ai, duel) => new MokeyMokeyExecutor(ai, duel)),
            new BotItem("MokeyMokeyKing", "AI_MokeyMokeyKing", (ai, duel) => new MokeyMokeyKingExecutor(ai, duel)),
            new BotItem("Neko", "AI_Neko", (ai, duel) => new NekoExecutor(ai, duel)),
            new BotItem("Nekroz", "AI_Nekroz", (ai, duel) => new NekrozExecutor(ai, duel)),
            new BotItem("OldSchool", "AI_OldSchool", (ai, duel) => new OldSchoolExecutor(ai, duel)),
            new BotItem("Orcust", "AI_Orcust", (ai, duel) => new OrcustExecutor(ai, duel)),
            new BotItem("Phantasm", "AI_Phantasm", (ai, duel) => new PhantasmExecutor(ai, duel)),
            new BotItem("PureWinds", "AI_PureWinds", (ai, duel) => new PureWindsExecutor(ai, duel)),
            new BotItem("Qliphort", "AI_Qliphort", (ai, duel) => new QliphortExecutor(ai, duel)),
            new BotItem("Rainbow", "AI_Rainbow", (ai, duel) => new RainbowExecutor(ai, duel)),
            new BotItem("Rank V", "AI_Rank5", (ai, duel) => new Rank5Executor(ai, duel)),
            new BotItem("Ryzeal", "AI_Ryzeal", (ai, duel) => new RyzealExecutor(ai, duel)),
            new BotItem("Salamangreat", "AI_Salamangreat", (ai, duel) => new SalamangreatExecutor(ai, duel)),
            new BotItem("SkyStriker", "AI_SkyStriker", (ai, duel) => new SkyStrikerExecutor(ai, duel)),
            new BotItem("ST1732", "AI_ST1732", (ai, duel) => new ST1732Executor(ai, duel)),
            new BotItem("SuperheavySamurai", "AI_SuperheavySamurai", (ai, duel) => new SuperheavySamuraiExecutor(ai, duel)),
            new BotItem("Swordsoul", "AI_Swordsoul", (ai, duel) => new SwordsoulExecutor(ai, duel)),
            new BotItem("Tearlaments", "AI_Tearlaments", (ai, duel) => new TearlamentsExecutor(ai, duel)),
            new BotItem("Test", "AI_Test", (ai, duel) => new DoEverythingExecutor(ai, duel)),
            new BotItem("Lucky", "AI_Test", (ai, duel) => new LuckyExecutor(ai, duel)),
            new BotItem("ThunderDragon", "AI_ThunderDragon", (ai, duel) => new ThunderDragonExecutor(ai, duel)),
            new BotItem("TimeThief", "AI_Timethief", (ai, duel) => new TimeThiefExecutor(ai, duel)),
            new BotItem("Toadally Awesome", "AI_ToadallyAwesome", (ai, duel) => new ToadallyAwesomeExecutor(ai, duel)),
            new BotItem("Trickstar", "AI_Trickstar", (ai, duel) => new TrickstarExecutor(ai, duel)),
            new BotItem("Witchcraft", "AI_Witchcraft", (ai, duel) => new WitchcraftExecutor(ai, duel)),
            new BotItem("Yosenju", "AI_Yosenju", (ai, duel) => new YosenjuExecutor(ai, duel)),
            new BotItem("Yubel", "AI_Yubel", (ai, duel) => new YubelExecutor(ai, duel)),
            new BotItem("Zefra", "AI_Zefra", (ai, duel) => new ZefraExecutor(ai, duel)),
            new BotItem("Zexal Weapons", "AI_ZexalWeapons", (ai, duel) => new ZexalWeaponsExecutor(ai, duel)),
            new BotItem("Zoodiac", "AI_Zoodiac", (ai, duel) => new ZoodiacExecutor(ai, duel))
        };
    }
}