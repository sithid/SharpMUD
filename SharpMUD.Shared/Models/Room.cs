namespace SharpMUD.Shared.Models {
    public class Room {
        public string Name { get; set; }
        public string Description { get; set; }

        private Dictionary<string, Room> _exits;
        public Dictionary<string, Room> Exits { get { return _exits; } }

        public List<Item> _items;
        public List<Item> Items { get { return _items; } }

        public Room(string name, string description) {
            Name = name;
            Description = description;
            _exits = new Dictionary<string, Room>();
            _items = new List<Item>();
        }

        public void AddExit(string direction, Room room) {
            _exits[direction] = room;
        }

        public void AddItem( Item item ) {
            _items.Add(item);
        }
    }
}
