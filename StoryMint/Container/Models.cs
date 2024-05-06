using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StoryMint.Container;

public readonly struct Constants
{
    public enum Countries
    {
        [Display(Name = "United States of America")]
        USA,
        Canada,
        France,
        Germany,
        Spain
    }

    public enum Genre
    {
        Action,
        Adventure,
        Comedy,
        Crime,
        Drama,
        Epic,
        Erotica,
        [Display(Name = "Fairy Tale")]
        FairyTale,
        Fantasy,
        [Display(Name = "Historical Fiction")]
        HistoricalFiction,
        Horror,
        Mystery,
        Paranormal,
        [Display(Name = "Political Thriller")]
        PoliticalThriller,
        [Display(Name = "Post-apocalyptic")]
        PostApocalyptic,
        Romance,
        Satire,
        [Display(Name = "Science Fiction")]
        ScienceFiction,
        [Display(Name = "Slice of Life")]
        SliceOfLife,
        Supernatural,
        Suspense,
        Thriller,
        Western,
        [Display(Name = "Young Adult")]
        YoungAdult
    }

    public enum Tone
    {
        Formal,
        Casual,
        Humorous,
        Sarcastic,
        Serious,
        Flirtatious,
        Mysterious
    }
    #region Age Groups

    public static readonly SelectListGroup AgeChildrenGroup = new() { Name = "Children (0-12)" };
    public static readonly SelectListGroup AgeTeensGroup = new() { Name = "Teens (13-19)" };
    public static readonly SelectListGroup AgeAdultsGroup = new() { Name = "Adults (20+)" };
    public static readonly SelectListGroup AgeElderlyGroup = new() { Name = "Elderly (65+)" };

    public static readonly List<SelectListItem> AgeGroups = [
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                    Value = "All ages",
                                                                                                    Text = "All Ages",
                                                                                                    //Group = AgeChildrenGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                    Value = "0-3",
                                                                                                    Text = "Toddlers (0-3)",
                                                                                                    Group = AgeChildrenGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "3-5",
                                                                                                        Text = "Preschoolers (3-5)",
                                                                                                        Group = AgeChildrenGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "5-8",
                                                                                                        Text = "Early Elementary (5-8)",
                                                                                                        Group = AgeChildrenGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "8-12",
                                                                                                        Text = "Late Elementary (8-12)",
                                                                                                        Group = AgeChildrenGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "13-15",
                                                                                                        Text = "Young Teens (13-15)",
                                                                                                        Group = AgeTeensGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "16-19",
                                                                                                        Text = "Older Teens (16-19)",
                                                                                                        Group = AgeTeensGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "20-35",
                                                                                                        Text = "Young Adults (20-35)",
                                                                                                        Group = AgeAdultsGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "36-55",
                                                                                                        Text = "Middle-aged Adults (36-55)",
                                                                                                        Group = AgeAdultsGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "56-64",
                                                                                                        Text = "Older Adults (56-64)",
                                                                                                        Group = AgeAdultsGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "65-74",
                                                                                                        Text = "Young Old (65-74)",
                                                                                                        Group = AgeElderlyGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "75-84",
                                                                                                        Text = "Middle Old (75-84)",
                                                                                                        Group = AgeElderlyGroup
                                                                                                    },
                                                                                                    new SelectListItem
                                                                                                    {
                                                                                                        Value = "85+",
                                                                                                        Text = "Oldest Old (85+)",
                                                                                                        Group = AgeElderlyGroup
                                                                                                    }
                                                                                                ];
    #endregion
}

public record AzureModelConfig(string DeploymentName, string Endpoint, string ApiKey);
public record CreateStory(    
    [MaxLength(1024)]
    string? Description,
    [Display(Name = "Age Group")]
    string AgeGroup,
    Constants.Genre Genre,
    Constants.Tone Tone);

public record StoryCreated(Guid Id, string Title, string Description, string Content, string CoverImage);