using System.Globalization;
using Quiniela.BusinessLogic;

if (args.Length != 4)
{
    WriteUsage();
    return 1;
}

if (!TryParseScore(args[0], out var actualTeamA)
    || !TryParseScore(args[1], out var actualTeamB)
    || !TryParseScore(args[2], out var predictedTeamA)
    || !TryParseScore(args[3], out var predictedTeamB))
{
    Console.Error.WriteLine("Los cuatro marcadores deben ser enteros no negativos.");
    WriteUsage();
    return 1;
}

var scorer = new QuinielaScorer();
var points = scorer.CalculatePoints(
    actualTeamA,
    actualTeamB,
    predictedTeamA,
    predictedTeamB);

Console.WriteLine($"Puntos: {points}");
return 0;

bool TryParseScore(string value, out int score) =>
    int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out score);

void WriteUsage() =>
    Console.Error.WriteLine(
        "Uso: Quiniela.AppConsole <marcador-real-A> <marcador-real-B> "
        + "<pronostico-A> <pronostico-B>");
