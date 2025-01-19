namespace SharpMUD.Shared.Models {
    public class Player {
        public string Name { get; set; }
        public Room CurrentRoom { get; set; }

        public Player(string name, Room startingRoom) {
            Name = name;
            CurrentRoom = startingRoom;
        }
    }
}