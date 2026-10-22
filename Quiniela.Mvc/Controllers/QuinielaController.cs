using Microsoft.AspNetCore.Mvc;
using Quiniela.BusinessLogic;
using Quiniela.Mvc.Models;

namespace Quiniela.Mvc.Controllers;

public class QuinielaController(QuinielaScorer scorer) : Controller
{
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult WebUnitTesting() => View(new QuinielaPredictionViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CalculatePoints(QuinielaPredictionViewModel prediction)
    {
        if (!ModelState.IsValid)
        {
            return View("WebUnitTesting", prediction);
        }

        prediction.TotalPoints = scorer.CalculatePoints(
            prediction.ActualTeamAScore!.Value,
            prediction.ActualTeamBScore!.Value,
            prediction.PredictedTeamAScore!.Value,
            prediction.PredictedTeamBScore!.Value);

        return View("WebUnitTesting", prediction);
    }
}
