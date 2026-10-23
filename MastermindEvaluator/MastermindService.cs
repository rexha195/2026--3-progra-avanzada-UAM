using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MastermindEvaluator;

public sealed class MastermindService
{
    private static readonly char[] ColoresPredeterminados = ['Y', 'C', 'B', 'P', 'W', 'K', 'R', 'G'];
    private static readonly object BloqueoArchivoCheatCodes = new();
    private static readonly JsonSerializerOptions OpcionesJson = new() { WriteIndented = true };

    private readonly HashSet<char> _coloresValidos;
    private readonly HashSet<string> _cheatCodesUsados = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _rutaCheatCodes;
    private readonly object _bloqueoSesion = new();
    private readonly Func<int, int> _seleccionarPosicion;

    public MastermindService(
        IEnumerable<char>? coloresValidos = null,
        string? rutaCheatCodes = null,
        Func<int, int>? seleccionarPosicion = null)
    {
        _coloresValidos = new HashSet<char>(coloresValidos ?? ColoresPredeterminados);
        if (_coloresValidos.Count == 0)
        {
            throw new ArgumentException("Debe configurarse al menos un color válido.", nameof(coloresValidos));
        }

        _rutaCheatCodes = rutaCheatCodes ?? Path.Combine(AppContext.BaseDirectory, "cheatcodes.json");
        _seleccionarPosicion = seleccionarPosicion ?? Random.Shared.Next;
    }

    public (int bienPosicionados, int colorCorrecto) EvaluarIntento(
        string patronSecreto,
        string intentoJugador,
        string? cheatCode = null)
    {
        ValidarPatron(patronSecreto, nameof(patronSecreto));
        ValidarPatron(intentoJugador, nameof(intentoJugador));

        if (patronSecreto.Length != intentoJugador.Length)
        {
            throw new ArgumentException("El patrón secreto y el intento deben tener la misma longitud.");
        }

        var resultado = ContarCoincidencias(patronSecreto, intentoJugador);

        lock (_bloqueoSesion)
        {
            if (cheatCode is not null)
            {
                AplicarCheatCode(cheatCode, patronSecreto, intentoJugador);
            }

            var fechaHora = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            Console.WriteLine(
                $"En la fecha {fechaHora} se evaluó el intento {intentoJugador} contra el patrón {patronSecreto}. " +
                $"Resultado: {resultado.bienPosicionados} bien posicionados – {resultado.colorCorrecto} con color correcto.");
        }

        return resultado;
    }

    private void ValidarPatron(string patron, string nombreParametro)
    {
        ArgumentNullException.ThrowIfNull(patron, nombreParametro);

        if (patron.Length is not (4 or 6))
        {
            throw new ArgumentException("El patrón debe tener una longitud de 4 o 6 caracteres.", nombreParametro);
        }

        foreach (var color in patron)
        {
            if (!_coloresValidos.Contains(color))
            {
                throw new ArgumentException($"El color '{color}' no está configurado como válido.", nombreParametro);
            }
        }
    }

    private static (int bienPosicionados, int colorCorrecto) ContarCoincidencias(
        string patronSecreto,
        string intentoJugador)
    {
        var secretoContado = new bool[patronSecreto.Length];
        var intentoContado = new bool[intentoJugador.Length];
        var bienPosicionados = 0;
        var colorCorrecto = 0;

        for (var i = 0; i < patronSecreto.Length; i++)
        {
            if (patronSecreto[i] == intentoJugador[i])
            {
                bienPosicionados++;
                secretoContado[i] = true;
                intentoContado[i] = true;
            }
        }

        for (var i = 0; i < patronSecreto.Length; i++)
        {
            if (secretoContado[i])
            {
                continue;
            }

            for (var j = 0; j < intentoJugador.Length; j++)
            {
                if (!intentoContado[j] && patronSecreto[i] == intentoJugador[j])
                {
                    colorCorrecto++;
                    intentoContado[j] = true;
                    break;
                }
            }
        }

        return (bienPosicionados, colorCorrecto);
    }

    private void AplicarCheatCode(string cheatCode, string patronSecreto, string intentoJugador)
    {
        var codigoHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cheatCode)));

        if (_cheatCodesUsados.Contains(codigoHash))
        {
            throw new InvalidOperationException("Este cheat code ya fue utilizado en esta sesión de juego.");
        }

        lock (BloqueoArchivoCheatCodes)
        {
            var contenido = File.ReadAllText(_rutaCheatCodes);
            var codigos = JsonSerializer.Deserialize<List<CheatCodeEntry>>(contenido)
                ?? throw new InvalidDataException("El archivo cheatcodes.json no contiene una lista válida.");

            var codigo = codigos.FirstOrDefault(
                entry => string.Equals(entry.Hash, codigoHash, StringComparison.OrdinalIgnoreCase));

            if (codigo is null)
            {
                throw new ArgumentException("El cheat code no es válido.", nameof(cheatCode));
            }

            if (codigo.UsosDisponibles <= 0)
            {
                throw new InvalidOperationException("El cheat code no tiene usos disponibles.");
            }

            codigo.UsosDisponibles--;
            PersistirCodigos(codigos);
            _cheatCodesUsados.Add(codigoHash);
        }

        var posicion = _seleccionarPosicion(patronSecreto.Length);
        if (posicion < 0 || posicion >= patronSecreto.Length)
        {
            throw new InvalidOperationException("El selector aleatorio devolvió una posición no válida.");
        }

        if (patronSecreto[posicion] == intentoJugador[posicion])
        {
            Console.WriteLine("sin suerte");
        }
        else
        {
            Console.WriteLine($"Pista: posición {posicion + 1}, color {patronSecreto[posicion]}.");
        }
    }

    private void PersistirCodigos(List<CheatCodeEntry> codigos)
    {
        var rutaCompleta = Path.GetFullPath(_rutaCheatCodes);
        var directorio = Path.GetDirectoryName(rutaCompleta)
            ?? throw new InvalidOperationException("No se pudo determinar el directorio de cheatcodes.json.");
        var rutaTemporal = Path.Combine(directorio, $".cheatcodes.{Guid.NewGuid():N}.tmp");

        try
        {
            File.WriteAllText(
                rutaTemporal,
                JsonSerializer.Serialize(codigos, OpcionesJson),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            File.Move(rutaTemporal, rutaCompleta, overwrite: true);
        }
        finally
        {
            if (File.Exists(rutaTemporal))
            {
                File.Delete(rutaTemporal);
            }
        }
    }

    private sealed class CheatCodeEntry
    {
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        [JsonPropertyName("usosDisponibles")]
        public int UsosDisponibles { get; set; }
    }
}
