using System.ComponentModel.DataAnnotations;

namespace Quiniela.Mvc.Models;

public class QuinielaPredictionViewModel
{
    [Required]
    [Display(Name = "Actual score for team A")]
    [Range(0, int.MaxValue)]
    public int? ActualTeamAScore { get; set; }

    [Required]
    [Display(Name = "Actual score for team B")]
    [Range(0, int.MaxValue)]
    public int? ActualTeamBScore { get; set; }

    [Required]
    [Display(Name = "Predicted score for team A")]
    [Range(0, int.MaxValue)]
    public int? PredictedTeamAScore { get; set; }

    [Required]
    [Display(Name = "Predicted score for team B")]
    [Range(0, int.MaxValue)]
    public int? PredictedTeamBScore { get; set; }

    public int? TotalPoints { get; set; }
}
