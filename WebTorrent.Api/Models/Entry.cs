using System.Text.Json.Serialization;

namespace WebTorrent.Api.Models;

public class Entry : Entity
{
    public string Day { get; private set; } = string.Empty;
    public string Hour { get; private set; } = string.Empty;
    public bool Boob { get; private set; }
    public int Food { get; private set; }
    public int ArtificialFood { get; private set; }
    public int Pee { get; private set; }
    public int Poop { get; private set; }
    public int PoopBag { get; private set; }
    public int Refeeding { get; private set; }
    public bool Salt { get; private set; }
    public bool Vitamin { get; private set; }
    public bool ChangeBag { get; private set; }
    public string Other { get; private set; } = string.Empty;

    //create method to update all fields from entry
    public void UpdateEntry(string day, string hour, bool boob, int food, int artificialFood, int pee, int poop, int poopBag, bool salt, bool vitamin, bool changeBag, int refeeding, string other)
    {
        Day = day;
        Hour = hour;
        Boob = boob;
        Food = food;
        ArtificialFood = artificialFood;
        Pee = pee;
        Poop = poop;
        PoopBag = poopBag;
        Salt = salt;
        Vitamin = vitamin;
        ChangeBag = changeBag;
        Refeeding = refeeding;
        Other = other;
    }

    public static Entry CreateEntry(string day, string hour, bool boob, int food, int artificialFood, int pee, int poop, int poopBag, bool salt, bool vitamin, bool changeBag, int refeeding, string other)
    {
        return new Entry {
            Day = day,
            Hour = hour,
            Boob = boob,
            Food = food,
            ArtificialFood = artificialFood,
            Pee = pee,
            Poop = poop,
            PoopBag = poopBag,
            Salt = salt,
            Vitamin = vitamin,
            ChangeBag = changeBag,
            Refeeding = refeeding,
            Other = other
        };
    }

    public EntryDto ToDto()
    {
        return new EntryDto
        {
            Id = Id,
            Day = Day,
            Hour = Hour,
            Boob = Boob,
            Food = Food,
            ArtificialFood = ArtificialFood,
            Pee = Pee,
            Poop = Poop,
            PoopBag = PoopBag,
            Salt = Salt,
            Vitamin = Vitamin,
            ChangeBag = ChangeBag,
            Refeeding = Refeeding,
            Other = Other
        };
    }
}