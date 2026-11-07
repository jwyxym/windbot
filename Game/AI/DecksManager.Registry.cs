// Maintained by hand. Chinese names and bot order follow the ygopro3 bot list.
using WindBot.Game.AI.Decks;

namespace WindBot.Game.AI
{
    public static partial class DecksManager
    {
        private static void RegisterDecks()
        {
            /* P2 */
            RegisterDeck("Lucky", new DeckInstance("P2-自选卡组", "AI_Test", (ai, duel) => new LuckyExecutor(ai, duel), "Test", "gugugu.zh-CN"));

            /* 悠悠 / 悠悠王 */
            RegisterDeck("MokeyMokey", new DeckInstance("悠悠", "AI_MokeyMokey", (ai, duel) => new MokeyMokeyExecutor(ai, duel), "Easy", "mokey.zh-CN"));
            RegisterDeck("MokeyMokeyKing", new DeckInstance("悠悠王", "AI_MokeyMokeyKing", (ai, duel) => new MokeyMokeyKingExecutor(ai, duel), "Easy", "mokey.zh-CN"));

            /* 谜之剑士LV4 */
            RegisterDeck("Dragunity", new DeckInstance("谜之剑士LV4-龙骑星爆", "AI_Dragunity", (ai, duel) => new DragunityExecutor(ai, duel), "Normal", "swordsman.zh-CN"));
            RegisterDeck("Level VIII", new DeckInstance("谜之剑士LV4-L8", "AI_Level8", (ai, duel) => new Level8Executor(ai, duel), "Normal", "swordsman.zh-CN"));
            RegisterDeck("Rank V", new DeckInstance("谜之剑士LV4-R5", "AI_Rank5", (ai, duel) => new Rank5Executor(ai, duel), "Normal", "swordsman.zh-CN"));
            RegisterDeck("Zexal Weapons", new DeckInstance("谜之剑士LV4-异热同心武器", "AI_ZexalWeapons", (ai, duel) => new ZexalWeaponsExecutor(ai, duel), "Normal", "swordsman.zh-CN"));
            RegisterDeck("HeroBeat1103", new DeckInstance("谜之剑士LV4-1103英雄", "AI_HeroBeat1103", (ai, duel) => new HeroBeat1103Executor(ai, duel), "Normal", "swordsman.zh-CN"));
            RegisterDeck("Blackwing", new DeckInstance("谜之剑士LV4-黑羽", "AI_Blackwing", (ai, duel) => new BlackwingExecutor(ai, duel), "NotFinished", "default"));
            RegisterDeck("CyberDragon", new DeckInstance("谜之剑士LV4-电子龙", "AI_CyberDragon", (ai, duel) => new CyberDragonExecutor(ai, duel), "NotFinished", "default"));
            RegisterDeck("Evilswarm", new DeckInstance("谜之剑士LV4-入魔", "AI_Evilswarm", (ai, duel) => new EvilswarmExecutor(ai, duel), "NotFinished", "default"));
            RegisterDeck("Graydle", new DeckInstance("谜之剑士LV4-灰篮", "AI_Graydle", (ai, duel) => new GraydleExecutor(ai, duel), "NotFinished", "default"));
            RegisterDeck("MathMech", new DeckInstance("谜之剑士LV4-斩机", "AI_Mathmech", (ai, duel) => new MathmechExecutor(ai, duel), "Normal", "default"));
            RegisterDeck("Nekroz", new DeckInstance("谜之剑士LV4-影灵衣", "AI_Nekroz", (ai, duel) => new NekrozExecutor(ai, duel), "NotFinished", "default"));
            RegisterDeck("Albaz", new DeckInstance("谜之剑士LV4-烙印", "AI_Albaz", (ai, duel) => new AlbazExecutor(ai, duel), "Normal", "ecclesia.zh-CN"));

            /* 琪露诺 */
            RegisterDeck("Rainbow", new DeckInstance("琪露诺-彩虹", "AI_Rainbow", (ai, duel) => new RainbowExecutor(ai, duel), "Normal", "cirno.zh-CN"));
            RegisterDeck("Toadally Awesome", new DeckInstance("琪露诺-饼蛙", "AI_ToadallyAwesome", (ai, duel) => new ToadallyAwesomeExecutor(ai, duel), "Normal", "cirno.zh-CN"));

            /* 复制植物 */
            RegisterDeck("Blue-Eyes", new DeckInstance("复制植物-青眼", "AI_BlueEyes", (ai, duel) => new BlueEyesExecutor(ai, duel), "Normal", "copy.zh-CN"));
            RegisterDeck("Zoodiac", new DeckInstance("复制植物-十二兽", "AI_Zoodiac", (ai, duel) => new ZoodiacExecutor(ai, duel), "Normal", "copy.zh-CN"));
            RegisterDeck("Rank8", new DeckInstance("复制植物-R8", "AI_Rank8", (ai, duel) => new Rank8Executor(ai, duel), "Normal", "copy.zh-CN"));

            /* 尼亚 */
            RegisterDeck("Yosenju", new DeckInstance("尼亚-妖仙兽", "AI_Yosenju", (ai, duel) => new YosenjuExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Qliphort", new DeckInstance("尼亚-机壳", "AI_Qliphort", (ai, duel) => new QliphortExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Trickstar", new DeckInstance("尼亚-淘气仙星", "AI_Trickstar", (ai, duel) => new TrickstarExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Altergeist", new DeckInstance("尼亚-幻变骚灵", "AI_Altergeist", (ai, duel) => new AltergeistExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Labrynth", new DeckInstance("尼亚-拉比林斯迷宫", "AI_Labrynth", (ai, duel) => new LabrynthExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Yubel", new DeckInstance("尼亚-于贝尔", "AI_Yubel", (ai, duel) => new YubelExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Apophis", new DeckInstance("尼亚-王家的神殿", "AI_Apophis", (ai, duel) => new ApophisExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Archfiend", new DeckInstance("尼亚-恶魔", "AI_Archfiend", (ai, duel) => new ArchfiendExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Enneacraft", new DeckInstance("尼亚-纠罪巧", "AI_Enneacraft", (ai, duel) => new EnneacraftExecutor(ai, duel), "Normal", "near.zh-CN"));
            RegisterDeck("Pumpking", new DeckInstance("尼亚-南瓜王", "AI_Pumpking", (ai, duel) => new PumpkingExecutor(ai, duel), "Normal", "near.zh-CN"));

            /* 复制梁龙 */
            RegisterDeck("SkyStriker", new DeckInstance("复制梁龙-闪刀姬", "AI_SkyStriker", (ai, duel) => new SkyStrikerExecutor(ai, duel), "Normal", "anothercopy.zh-CN"));
            RegisterDeck("Orcust", new DeckInstance("复制梁龙-自奏圣乐", "AI_Orcust", (ai, duel) => new OrcustExecutor(ai, duel), "Normal", "anothercopy.zh-CN"));
            RegisterDeck("Salamangreat", new DeckInstance("复制梁龙-转生炎兽", "AI_Salamangreat", (ai, duel) => new SalamangreatExecutor(ai, duel), "Normal", "anothercopy.zh-CN"));
            RegisterDeck("Ryzeal", new DeckInstance("复制梁龙-雷火沸动", "AI_Ryzeal", (ai, duel) => new RyzealExecutor(ai, duel), "Normal", "anothercopy.zh-CN"));
            RegisterDeck("Maliss", new DeckInstance("复制梁龙-码丽丝", "AI_Maliss", (ai, duel) => new MalissExecutor(ai, duel), "Normal", "anothercopy.zh-CN"));
            RegisterDeck("BE2025", new DeckInstance("复制梁龙-青眼", "AI_BE2025", (ai, duel) => new BE2025Executor(ai, duel), "Normal", "anothercopy.zh-CN"));
            RegisterDeck("SacredBeast", new DeckInstance("复制梁龙-三幻魔", "AI_SacredBeast", (ai, duel) => new SacredBeastExecutor(ai, duel), "Normal", "anothercopy.zh-CN"));
            RegisterDeck("Elfnote", new DeckInstance("复制梁龙-耀圣", "AI_Elfnote", (ai, duel) => new ElfnoteExecutor(ai, duel), "Normal", "anothercopy.zh-CN"));

            /* VI-1911 */
            RegisterDeck("FamiliarPossessed", new DeckInstance("VI-1911-凭依装着", "AI_FamiliarPossessed", (ai, duel) => new FamiliarPossessedExecutor(ai, duel), "Normal", "VI-1911.zh-CN"));
            RegisterDeck("TimeThief", new DeckInstance("VI-1911-时劫者", "AI_Timethief", (ai, duel) => new TimeThiefExecutor(ai, duel), "Normal", "VI-1911.zh-CN"));

            /* 永远之魂 */
            RegisterDeck("Burn", new DeckInstance("永远之魂-削血", "AI_Burn", (ai, duel) => new BurnExecutor(ai, duel), "Easy", "soul.zh-CN"));
            RegisterDeck("Frog", new DeckInstance("永远之魂-青蛙", "AI_Frog", (ai, duel) => new FrogExecutor(ai, duel), "Easy", "soul.zh-CN"));
            RegisterDeck("Horus", new DeckInstance("永远之魂-荷鲁斯", "AI_Horus", (ai, duel) => new HorusExecutor(ai, duel), "Easy", "soul.zh-CN"));
            RegisterDeck("Chaos408", new DeckInstance("永远之魂-408混沌", "AI_Chaos408", (ai, duel) => new Chaos408Executor(ai, duel), "Normal", "soul.zh-CN"));
            RegisterDeck("Monarch506", new DeckInstance("永远之魂-506风帝", "AI_Monarch506", (ai, duel) => new Monarch506Executor(ai, duel), "Normal", "soul.zh-CN"));
            RegisterDeck("Gravekeeper", new DeckInstance("永远之魂-守墓", "AI_Gravekeeper", (ai, duel) => new GravekeeperExecutor(ai, duel), "NotFinished", "default"));

            /* 比特机灵 */
            RegisterDeck("PureWinds", new DeckInstance("比特机灵-微风", "AI_PureWinds", (ai, duel) => new PureWindsExecutor(ai, duel), "Normal", "zh-CN"));

            /* 试作型机器人1732 */
            RegisterDeck("ST1732", new DeckInstance("试作型机器人1732", "AI_ST1732", (ai, duel) => new ST1732Executor(ai, duel), "Normal", "zh-CN"));

            /* 奇異果 / 奇魔果 / MAX龍果 / 幻煌果 */
            RegisterDeck("LightswornShaddoldinosour", new DeckInstance("奇異果-光道影依恐龙", "AI_LightswornShaddoldinosour", (ai, duel) => new LightswornShaddoldinosour(ai, duel), "Normal", "kiwi.zh-TW"));
            RegisterDeck("Lightsworn", new DeckInstance("奇異果-光道", "AI_Lightsworn", (ai, duel) => new LightswornExecutor(ai, duel), "Normal", "copy.zh-CN"));
            RegisterDeck("DarkMagician", new DeckInstance("奇魔果-黑魔术师", "AI_DarkMagician", (ai, duel) => new DarkMagicianExecutor(ai, duel), "Normal", "kiwi.zh-TW"));
            RegisterDeck("BlueEyesMaxDragon", new DeckInstance("MAX龍果-青眼混沌极龙", "AI_BlueEyesMaxDragon", (ai, duel) => new BlueEyesMaxDragonExecutor(ai, duel), "Normal", "kiwi.zh-TW"));
            RegisterDeck("Phantasm", new DeckInstance("幻煌果-幻煌龙", "AI_Phantasm", (ai, duel) => new PhantasmExecutor(ai, duel), "Normal", "kiwi.zh-TW"));

            /* 燃血鬥士 / 報社鬥士 */
            RegisterDeck("ChainBurn", new DeckInstance("燃血鬥士-连锁烧", "AI_ChainBurn", (ai, duel) => new ChainBurnExecutor(ai, duel), "Normal", "kiwi.zh-TW"));
            RegisterDeck("GrenMajuThunderBoarder", new DeckInstance("報社鬥士-红莲雷滑", "AI_GrenMajuThunderBoarder", (ai, duel) => new GrenMajuThunderBoarderExecutor(ai, duel), "Normal", "kiwi.zh-TW"));

            /* 我太帅了 */
            RegisterDeck("Dragun", new DeckInstance("我太帅了-真红眼龙骑士", "AI_Dragun", (ai, duel) => new DragunExecutor(ai, duel), "Normal", "smart.zh-CN"));
            RegisterDeck("Brave", new DeckInstance("我太帅了-水百凤凰勇者", "AI_Brave", (ai, duel) => new BraveExecutor(ai, duel), "Normal", "smart.zh-CN"));

            /* 玻璃女巫 */
            RegisterDeck("Witchcraft", new DeckInstance("玻璃女巫-魔女术", "AI_Witchcraft", (ai, duel) => new WitchcraftExecutor(ai, duel), "Normal", "verre.zh-CN"));
            RegisterDeck("Exosister", new DeckInstance("玻璃女巫-救祓少女", "AI_Exosister", (ai, duel) => new ExosisterExecutor(ai, duel), "Normal", "verre.zh-CN"));

            /* 艾克莉西娅 */
            RegisterDeck("Swordsoul", new DeckInstance("艾克莉西娅-相剑", "AI_Swordsoul", (ai, duel) => new SwordsoulExecutor(ai, duel), "Normal", "ecclesia.zh-CN"));
            RegisterDeck("Dogmatika", new DeckInstance("艾克莉西娅-教导", "AI_Dogmatika", (ai, duel) => new DogmatikaExecutor(ai, duel), "Normal", "ecclesia.zh-CN"));

            /* 神数不神 */
            RegisterDeck("Zefra", new DeckInstance("神数不神-神数", "AI_Zefra", (ai, duel) => new ZefraExecutor(ai, duel), "Normal", "Zefra.zh-CN"));
            RegisterDeck("Kashtira", new DeckInstance("神数不神-刹帝利", "AI_Kashtira", (ai, duel) => new KashtiraExecutor(ai, duel), "Normal", "Zefra.zh-CN"));
            RegisterDeck("Tearlaments", new DeckInstance("神数不神-珠泪哀歌", "AI_Tearlaments", (ai, duel) => new TearlamentsExecutor(ai, duel), "Normal", "Zefra.zh-CN"));
            RegisterDeck("ThunderDragon", new DeckInstance("[狂野]神数不神-雷龙", "AI_ThunderDragon", (ai, duel) => new ThunderDragonExecutor(ai, duel), "Normal", "Zefra.zh-CN"));

            /* 今晚有宵夜吗 */
            RegisterDeck("SuperheavySamurai", new DeckInstance("今晚有宵夜吗-超重武者", "AI_SuperheavySamurai", (ai, duel) => new SuperheavySamuraiExecutor(ai, duel), "Normal", "superheavysamurai.zh-CN"));
            RegisterDeck("MalissOCG", new DeckInstance("今晚有宵夜吗-码丽丝", "AI_MalissOCG", (ai, duel) => new MalissOCGExecutor(ai, duel), "Normal", "Xiaoye.zh-CN"));
            RegisterDeck("Neko", new DeckInstance("今晚有宵夜吗-味美喵", "AI_Neko", (ai, duel) => new NekoExecutor(ai, duel), "Normal", "Xiaoye.zh-CN"));

            /* 偷摸灵兽幽灵 */
            RegisterDeck("RadiantTyphoon", new DeckInstance("偷摸灵兽幽灵-炽焰", "AI_RadiantTyphoon", (ai, duel) => new RadiantTyphoonExecutor(ai, duel), "Normal", "soyorin.zh-TW"));
            RegisterDeck("ChaosRitual", new DeckInstance("偷摸灵兽幽灵-混沌仪式", "AI_ChaosRitual", (ai, duel) => new ChaosRitualExecutor(ai, duel), "Normal", "soyorin.zh-TW"));

            /* Additional master bots */
            RegisterDeck("Test", new DeckInstance("测试机器人", "AI_Test", (ai, duel) => new DoEverythingExecutor(ai, duel), "Test", "default"));
            RegisterDeck("OldSchool", new DeckInstance("怀旧卡组", "AI_OldSchool", (ai, duel) => new OldSchoolExecutor(ai, duel), "Easy", "default"));
            RegisterDeck("Voiceless", new DeckInstance("肃声", "AI_Voiceless", (ai, duel) => new VoicelessExecutor(ai, duel), "NotFinished", "default"));
        }
    }
}
