namespace SharpMUD.Shared.Models {
    public class NPC {
        public string Name { get; set; }
        public string Description { get; set; }
        public Room CurrentRoom { get; set; }

        public NPC(string name, string description, Room startingRoom) {
            Name = name;
            Description = description;
            CurrentRoom = startingRoom;
        }
    }
}
