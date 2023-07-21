using UnityEngine;

public class TestDriveData {
    [System.Serializable]
    public class Entry {
        public string name;
        public string score;
        public string seconds;
        public string text;
        public string date;
    }

    [System.Serializable]
    public class Leaderboard {
        public Entry[] entry;
    }

    [System.Serializable]
    public class Dreamlo {
        public Leaderboard leaderboard;
    }

    [System.Serializable]
    public class RootObject {
        public Dreamlo dreamlo;
    }
}
