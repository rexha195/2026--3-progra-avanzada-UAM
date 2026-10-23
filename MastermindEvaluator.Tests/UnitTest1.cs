namespace MastermindEvaluator.Tests;

public class MastermindServiceTests
{
    [Theory]
    [InlineData("RGBY", "RGBY", 4, 0)]
    [InlineData("RGBY", "RBYG", 1, 3)]
    [InlineData("RGBY", "WWWW", 0, 0)]
    [InlineData("RRBY", "RGGG", 1, 0)]
    [InlineData("RRBY", "BRRR", 1, 2)]
    public void EvaluarIntento_DevuelveLosColoresBienContados(
        string patronSecreto,
        string intentoJugador,
        int bienPosicionadosEsperados,
        int colorCorrectoEsperado)
    {
        var resultado = MastermindEvaluator.Program.EvaluarIntento(patronSecreto, intentoJugador);

        Assert.Equal(bienPosicionadosEsperados, resultado.bienPosicionados);
        Assert.Equal(colorCorrectoEsperado, resultado.colorCorrecto);
    }
}
