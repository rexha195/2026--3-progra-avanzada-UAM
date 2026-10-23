namespace Quiniela.BusinessLogic;

public class QuinielaScorer
{
    public int CalculatePoints(
        int actualTeamAScore,
        int actualTeamBScore,
        int predictedTeamAScore,
        int predictedTeamBScore)
    {
        if (actualTeamAScore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(actualTeamAScore));
        }

        if (actualTeamBScore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(actualTeamBScore));
        }

        if (predictedTeamAScore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(predictedTeamAScore));
        }

        if (predictedTeamBScore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(predictedTeamBScore));
        }

        if (actualTeamAScore == predictedTeamAScore
            && actualTeamBScore == predictedTeamBScore)
        {
            return 5;
        }

        var actualResult = actualTeamAScore.CompareTo(actualTeamBScore);
        var predictedResult = predictedTeamAScore.CompareTo(predictedTeamBScore);

        if (actualResult != predictedResult)
        {
            return 0;
        }

        var points = 2;
        if (actualTeamAScore == predictedTeamAScore
            || actualTeamBScore == predictedTeamBScore)
        {
            points++;
        }

        return points;
    }
}
