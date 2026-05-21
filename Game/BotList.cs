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
            string AIName,
            string Dialog,
            Func<GameAI, Duel, Executor> Factory
        );

        public static List<BotItem> List = new List<BotItem>()
        {
            /* P2 */
            new BotItem("P2", "AI_Test", "Lucky", "gugugu.zh-CN", (ai, duel) => new LuckyExecutor(ai, duel)),

            /* 悠悠 / 悠悠王 */
            new BotItem("悠悠", "AI_MokeyMokey", "MokeyMokey", "mokey.zh-CN", (ai, duel) => new MokeyMokeyExecutor(ai, duel)),
            new BotItem("悠悠王", "AI_MokeyMokeyKing", "MokeyMokeyKing", "mokey.zh-CN", (ai, duel) => new MokeyMokeyKingExecutor(ai, duel)),

            /* 谜之剑士LV4 */
            new BotItem("谜之剑士LV4", "AI_Dragunity", "Dragunity", "swordsman.zh-CN", (ai, duel) => new DragunityExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Level8", "Level VIII", "swordsman.zh-CN", (ai, duel) => new Level8Executor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Rank5", "Rank V", "swordsman.zh-CN", (ai, duel) => new Rank5Executor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_ZexalWeapons", "Zexal Weapons", "swordsman.zh-CN", (ai, duel) => new ZexalWeaponsExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Blackwing", "Blackwing", "swordsman.zh-CN", (ai, duel) => new BlackwingExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_CyberDragon", "CyberDragon", "swordsman.zh-CN", (ai, duel) => new CyberDragonExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Evilswarm", "Evilswarm", "swordsman.zh-CN", (ai, duel) => new EvilswarmExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Graydle", "Graydle", "swordsman.zh-CN", (ai, duel) => new GraydleExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Mathmech", "MathMech", "swordsman.zh-CN", (ai, duel) => new MathmechExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Nekroz", "Nekroz", "swordsman.zh-CN", (ai, duel) => new NekrozExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_OldSchool", "OldSchool", "swordsman.zh-CN", (ai, duel) => new OldSchoolExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Test", "Test", "swordsman.zh-CN", (ai, duel) => new DoEverythingExecutor(ai, duel)),
            new BotItem("谜之剑士LV4", "AI_Albaz", "Albaz", "ecclesia.zh-CN", (ai, duel) => new AlbazExecutor(ai, duel)),

            /* 琪露诺 */
            new BotItem("琪露诺", "AI_Rainbow", "Rainbow", "cirno.zh-CN", (ai, duel) => new RainbowExecutor(ai, duel)),
            new BotItem("琪露诺", "AI_ToadallyAwesome", "Toadally Awesome", "cirno.zh-CN", (ai, duel) => new ToadallyAwesomeExecutor(ai, duel)),

            /* 复制植物 */
            new BotItem("复制植物", "AI_BlueEyes", "Blue-Eyes", "copy.zh-CN", (ai, duel) => new BlueEyesExecutor(ai, duel)),
            new BotItem("复制植物", "AI_Zoodiac", "Zoodiac", "copy.zh-CN", (ai, duel) => new ZoodiacExecutor(ai, duel)),

            /* 尼亚 */
            new BotItem("尼亚", "AI_Yosenju", "Yosenju", "near.zh-CN", (ai, duel) => new YosenjuExecutor(ai, duel)),
            new BotItem("尼亚", "AI_Qliphort", "Qliphort", "near.zh-CN", (ai, duel) => new QliphortExecutor(ai, duel)),
            new BotItem("尼亚", "AI_Trickstar", "Trickstar", "near.zh-CN", (ai, duel) => new TrickstarExecutor(ai, duel)),
            new BotItem("尼亚", "AI_Altergeist", "Altergeist", "near.zh-CN", (ai, duel) => new AltergeistExecutor(ai, duel)),
            new BotItem("尼亚", "AI_Labrynth", "Labrynth", "near.zh-CN", (ai, duel) => new LabrynthExecutor(ai, duel)),
            new BotItem("尼亚", "AI_Yubel", "Yubel", "near.zh-CN", (ai, duel) => new YubelExecutor(ai, duel)),
            new BotItem("尼亚", "AI_Apophis", "Apophis", "near.zh-CN", (ai, duel) => new ApophisExecutor(ai, duel)),

            /* 复制梁龙 */
            new BotItem("复制梁龙", "AI_SkyStriker", "SkyStriker", "anothercopy.zh-CN", (ai, duel) => new SkyStrikerExecutor(ai, duel)),
            new BotItem("复制梁龙", "AI_Orcust", "Orcust", "anothercopy.zh-CN", (ai, duel) => new OrcustExecutor(ai, duel)),
            new BotItem("复制梁龙", "AI_Salamangreat", "Salamangreat", "anothercopy.zh-CN", (ai, duel) => new SalamangreatExecutor(ai, duel)),
            new BotItem("复制梁龙", "AI_Ryzeal", "Ryzeal", "anothercopy.zh-CN", (ai, duel) => new RyzealExecutor(ai, duel)),
            new BotItem("复制梁龙", "AI_Maliss", "Maliss", "anothercopy.zh-CN", (ai, duel) => new MalissExecutor(ai, duel)),
            new BotItem("复制梁龙", "AI_BE2025", "BE2025", "anothercopy.zh-CN", (ai, duel) => new BE2025Executor(ai, duel)),

            /* VI-1911 */
            new BotItem("VI-1911", "AI_FamiliarPossessed", "FamiliarPossessed", "VI-1911.zh-CN", (ai, duel) => new FamiliarPossessedExecutor(ai, duel)),
            new BotItem("VI-1911", "AI_Timethief", "TimeThief", "VI-1911.zh-CN", (ai, duel) => new TimeThiefExecutor(ai, duel)),

            /* 永远之魂 */
            new BotItem("永远之魂", "AI_Burn", "Burn", "soul.zh-CN", (ai, duel) => new BurnExecutor(ai, duel)),
            new BotItem("永远之魂", "AI_Frog", "Frog", "soul.zh-CN", (ai, duel) => new FrogExecutor(ai, duel)),
            new BotItem("永远之魂", "AI_Horus", "Horus", "soul.zh-CN", (ai, duel) => new HorusExecutor(ai, duel)),
            new BotItem("永远之魂", "AI_Gravekeeper", "Gravekeeper", "soul.zh-CN", (ai, duel) => new GravekeeperExecutor(ai, duel)),

            /* 比特机灵 */
            new BotItem("比特机灵", "AI_PureWinds", "PureWinds", "zh-CN", (ai, duel) => new PureWindsExecutor(ai, duel)),

            /* 试作型机器人1732 */
            new BotItem("试作型机器人1732", "AI_ST1732", "ST1732", "zh-CN", (ai, duel) => new ST1732Executor(ai, duel)),

            /* 奇異果 / 奇魔果 / MAX龍果 / 幻煌果 */
            new BotItem("奇異果", "AI_LightswornShaddoldinosour", "LightswornShaddoldinosour", "kiwi.zh-TW", (ai, duel) => new LightswornShaddoldinosour(ai, duel)),
            new BotItem("奇異果", "AI_Lightsworn", "Lightsworn", "kiwi.zh-TW", (ai, duel) => new LightswornExecutor(ai, duel)),
            new BotItem("奇魔果", "AI_DarkMagician", "DarkMagician", "kiwi.zh-TW", (ai, duel) => new DarkMagicianExecutor(ai, duel)),
            new BotItem("MAX龍果", "AI_BlueEyesMaxDragon", "BlueEyesMaxDragon", "kiwi.zh-TW", (ai, duel) => new BlueEyesMaxDragonExecutor(ai, duel)),
            new BotItem("幻煌果", "AI_Phantasm", "Phantasm", "kiwi.zh-TW", (ai, duel) => new PhantasmExecutor(ai, duel)),

            /* 燃血鬥士 / 報社鬥士 */
            new BotItem("燃血鬥士", "AI_ChainBurn", "ChainBurn", "kiwi.zh-TW", (ai, duel) => new ChainBurnExecutor(ai, duel)),
            new BotItem("報社鬥士", "AI_GrenMajuThunderBoarder", "GrenMajuThunderBoarder", "kiwi.zh-TW", (ai, duel) => new GrenMajuThunderBoarderExecutor(ai, duel)),

            /* 我太帅了 */
            new BotItem("我太帅了", "AI_Dragun", "Dragun", "smart.zh-CN", (ai, duel) => new DragunExecutor(ai, duel)),
            new BotItem("我太帅了", "AI_Brave", "Brave", "smart.zh-CN", (ai, duel) => new BraveExecutor(ai, duel)),

            /* 玻璃女巫 */
            new BotItem("玻璃女巫", "AI_Witchcraft", "Witchcraft", "verre.zh-CN", (ai, duel) => new WitchcraftExecutor(ai, duel)),
            new BotItem("玻璃女巫", "AI_Exosister", "Exosister", "verre.zh-CN", (ai, duel) => new ExosisterExecutor(ai, duel)),

            /* 艾克莉西娅 */
            new BotItem("艾克莉西娅", "AI_Swordsoul", "Swordsoul", "ecclesia.zh-CN", (ai, duel) => new SwordsoulExecutor(ai, duel)),
            new BotItem("艾克莉西娅", "AI_Dogmatika", "Dogmatika", "ecclesia.zh-CN", (ai, duel) => new DogmatikaExecutor(ai, duel)),

            /* 神数不神 */
            new BotItem("神数不神", "AI_Zefra", "Zefra", "Zefra.zh-CN", (ai, duel) => new ZefraExecutor(ai, duel)),
            new BotItem("神数不神", "AI_Kashtira", "Kashtira", "Zefra.zh-CN", (ai, duel) => new KashtiraExecutor(ai, duel)),
            new BotItem("神数不神", "AI_Tearlaments", "Tearlaments", "Zefra.zh-CN", (ai, duel) => new TearlamentsExecutor(ai, duel)),
            new BotItem("[狂野]神数不神", "AI_ThunderDragon", "ThunderDragon", "Zefra.zh-CN", (ai, duel) => new ThunderDragonExecutor(ai, duel)),

            /* 今晚有宵夜吗 */
            new BotItem("今晚有宵夜吗", "AI_SuperheavySamurai", "SuperheavySamurai", "superheavysamurai.zh-CN", (ai, duel) => new SuperheavySamuraiExecutor(ai, duel)),
            new BotItem("今晚有宵夜吗", "AI_MalissOCG", "MalissOCG", "Xiaoye.zh-CN", (ai, duel) => new MalissOCGExecutor(ai, duel)),
            new BotItem("今晚有宵夜吗", "AI_Neko", "Neko", "Xiaoye.zh-CN", (ai, duel) => new NekoExecutor(ai, duel)),
        };
    }
}