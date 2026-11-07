namespace YGOSharp.OCGWrapper
{
    internal static class CardsManager
    {
        internal static Card GetCard(int id)
        {
            return NamedCardsManager.GetCard(id);
        }
    }
}
