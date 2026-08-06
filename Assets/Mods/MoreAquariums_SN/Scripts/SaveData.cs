using System;
using System.Collections.Generic;
using Nautilus.Json;
using Nautilus.Json.Attributes;

namespace DaftAppleGames.MoreAquariums
{
    [FileName("MoreAquariums")] internal class SaveData : SaveDataCache
    {
        public DateTime SaveDateTime = DateTime.Now;
        public HashSet<AquariumSaver.AquariumDetails> AquariumDetailsHashSet { get; set; }
    }
}
