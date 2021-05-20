namespace EEngine.EEngine
{
    public class Pair<Item1, Item2>
    {
        public Item1 I1 { get; set; }
        public Item2 I2 { get; set; }

        public Pair(Item1 Item1, Item2 Item2)
        {
            this.I1 = Item1;
            this.I2 = Item2;
        }
    }
}
