using Quiniela.BusinessLogic;

namespace Quiniela.BusinessLogic.Tests;

public class QuinielaScorerTests
{
    private readonly QuinielaScorer _scorer = new();

    [Theory]
    [InlineData(2, 1, 2, 1, 5)]
    [InlineData(3, 1, 2, 0, 2)]
    [InlineData(3, 1, 3, 0, 3)]
    [InlineData(3, 1, 2, 1, 3)]
    [InlineData(1, 3, 0, 2, 2)]
    [InlineData(1, 3, 0, 3, 3)]
    [InlineData(2, 2, 1, 1, 2)]
    [InlineData(3, 1, 0, 2, 0)]
    [InlineData(3, 1, 2, 2, 0)]
    public void CalculatePoints_ReturnsExpectedPoints(
        int actualTeamAScore,
        int actualTeamBScore,
        int predictedTeamAScore,
        int predictedTeamBScore,
        int expectedPoints)
    {
        var points = _scorer.CalculatePoints(
            actualTeamAScore,
            actualTeamBScore,
            predictedTeamAScore,
            predictedTeamBScore);

        Assert.Equal(expectedPoints, points);
    }

    [Theory]
    [InlineData(-1, 0, 0, 0, "actualTeamAScore")]
    [InlineData(0, -1, 0, 0, "actualTeamBScore")]
    [InlineData(0, 0, -1, 0, "predictedTeamAScore")]
    [InlineData(0, 0, 0, -1, "predictedTeamBScore")]
    public void CalculatePoints_RejectsNegativeScores(
        int actualTeamAScore,
        int actualTeamBScore,
        int predictedTeamAScore,
        int predictedTeamBScore,
        string expectedParameterName)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            _scorer.CalculatePoints(
                actualTeamAScore,
                actualTeamBScore,
                predictedTeamAScore,
                predictedTeamBScore));

        Assert.Equal(expectedParameterName, exception.ParamName);
    }
}
