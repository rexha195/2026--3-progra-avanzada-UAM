using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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

    [Fact]
    public void EvaluarIntento_LongitudInvalidaDelIntento_LanzaArgumentException()
    {
        // Arrange
        var servicio = new MastermindService();

        // Act and Assert
        Assert.Throws<ArgumentException>(() => servicio.EvaluarIntento("RGBY", "RGB"));
    }

    [Theory]
    [InlineData("XGBY", "RGBY")]
    [InlineData("RGBY", "XGBY")]
    public void EvaluarIntento_ColorFueraDeLaConfiguracion_LanzaArgumentException(
        string patronSecreto,
        string intentoJugador)
    {
        // Arrange
        var servicio = new MastermindService(coloresValidos: ['R', 'G', 'B', 'Y']);

        // Act and Assert
        Assert.Throws<ArgumentException>(() => servicio.EvaluarIntento(patronSecreto, intentoJugador));
    }

    [Fact]
    public void EvaluarIntento_CheatCodeValido_DecrementaUsosYRevelaPista()
    {
        // Arrange
        using var cheatFile = new TemporaryCheatCodeFile("hint-code", usosDisponibles: 2);
        var servicio = new MastermindService(
            rutaCheatCodes: cheatFile.Path,
            seleccionarPosicion: _ => 1);
        var salidaOriginal = Console.Out;
        using var salida = new StringWriter();
        Console.SetOut(salida);

        try
        {
            // Act
            var resultado = servicio.EvaluarIntento("RGBY", "RBBY", "hint-code");

            // Assert
            Assert.Equal(3, resultado.bienPosicionados);
            Assert.Equal(0, resultado.colorCorrecto);
            Assert.Contains("Pista: posición 2, color G.", salida.ToString());
            Assert.Equal(1, cheatFile.UsosDisponibles);
        }
        finally
        {
            Console.SetOut(salidaOriginal);
        }
    }

    [Fact]
    public void EvaluarIntento_CheatCodeRepetidoEnLaMismaSesion_EsRechazadoSinOtroDescuento()
    {
        // Arrange
        using var cheatFile = new TemporaryCheatCodeFile("once-code", usosDisponibles: 2);
        var servicio = new MastermindService(
            rutaCheatCodes: cheatFile.Path,
            seleccionarPosicion: _ => 0);
        var salidaOriginal = Console.Out;
        using var salida = new StringWriter();
        Console.SetOut(salida);

        try
        {
            // Act
            servicio.EvaluarIntento("RGBY", "RBBY", "once-code");
            var exception = Assert.Throws<InvalidOperationException>(
                () => servicio.EvaluarIntento("RGBY", "RBBY", "once-code"));

            // Assert
            Assert.Contains("ya fue utilizado", exception.Message);
            Assert.Equal(1, cheatFile.UsosDisponibles);
        }
        finally
        {
            Console.SetOut(salidaOriginal);
        }
    }

    [Fact]
    public void EvaluarIntento_CheatCodeSeleccionaPosicionCorrecta_ImprimeSinSuerte()
    {
        // Arrange
        using var cheatFile = new TemporaryCheatCodeFile("luck-code", usosDisponibles: 1);
        var servicio = new MastermindService(
            rutaCheatCodes: cheatFile.Path,
            seleccionarPosicion: _ => 0);
        var salidaOriginal = Console.Out;
        using var salida = new StringWriter();
        Console.SetOut(salida);

        try
        {
            // Act
            var resultado = servicio.EvaluarIntento("RGBY", "RBYG", "luck-code");

            // Assert
            Assert.Equal(1, resultado.bienPosicionados);
            Assert.Equal(3, resultado.colorCorrecto);
            Assert.Contains("sin suerte", salida.ToString());
            Assert.Equal(0, cheatFile.UsosDisponibles);
        }
        finally
        {
            Console.SetOut(salidaOriginal);
        }
    }

    private sealed class TemporaryCheatCodeFile : IDisposable
    {
        private readonly string _directory = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            $"mastermind-tests-{Guid.NewGuid():N}");

        public TemporaryCheatCodeFile(string cheatCode, int usosDisponibles)
        {
            Directory.CreateDirectory(_directory);
            Path = System.IO.Path.Combine(_directory, "cheatcodes.json");
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cheatCode)));
            File.WriteAllText(
                Path,
                JsonSerializer.Serialize(new[] { new { hash, usosDisponibles } }));
        }

        public string Path { get; }

        public int UsosDisponibles
        {
            get
            {
                using var json = JsonDocument.Parse(File.ReadAllText(Path));
                return json.RootElement[0].GetProperty("usosDisponibles").GetInt32();
            }
        }

        public void Dispose()
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
