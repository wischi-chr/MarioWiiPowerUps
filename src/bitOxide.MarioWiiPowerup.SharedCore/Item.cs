namespace bitOxide.MarioWiiPowerup.Core
{
    public class Item
    {
        public static Item IceFlower = new Item(nameof(IceFlower), false);
        public static Item FireFlower = new Item(nameof(FireFlower), false);
        public static Item Fly = new Item(nameof(Fly), false);
        public static Item Mushroom = new Item(nameof(Mushroom), false);
        public static Item Mini = new Item(nameof(Mini), false);
        public static Item Star = new Item(nameof(Star), false);
        public static Item Penguin = new Item(nameof(Penguin), false);
        public static Item Bowser = new Item(nameof(Bowser), true);
        public static Item MiniBowser = new Item(nameof(MiniBowser), true);

        public string Name { get; }
        public bool IsBad { get; }

        private Item(string Name, bool IsBad)
        {
            this.Name = Name;
            this.IsBad = IsBad;
        }

        public static bool operator ==(Item a, Item b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Name == b.Name;
        }

        public static bool operator !=(Item a, Item b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            Item b = obj as Item;
            return this == b;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
