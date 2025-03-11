using System.Text.Json.Serialization;

namespace LouisManager.Api.Models;
public class EntryDto {
    public Guid Id { get; set; }
    public string Day { get; set; } = string.Empty;
    public string Hour { get; set; } = string.Empty;
    public bool Boob { get; set; }
    public int Food { get; set; }
    [JsonPropertyName("artificial_food")]
    public int ArtificialFood { get; set; }
    public int Pee { get; set; }
    public int Poop { get; set; }
    [JsonPropertyName("poop_bag")]
    public int PoopBag { get; set; }
    public int Refeeding { get; set; }
    public bool Salt { get; set; }
    public bool Vitamin { get; set; }
    [JsonPropertyName("bag")]
    public bool ChangeBag { get; set; }
    public string Other { get; set; } = string.Empty;
}