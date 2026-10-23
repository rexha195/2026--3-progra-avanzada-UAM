namespace MastermindEvaluator;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello, World!");
    }

    public static (int bienPosicionados, int colorCorrecto) EvaluarIntento(
        string patronSecreto,
        string intentoJugador)
    {
        var secretoContado = new bool[patronSecreto.Length];
        var intentoContado = new bool[intentoJugador.Length];
        var bienPosicionados = 0;
        var colorCorrecto = 0;

        var posicionesComunes = Math.Min(patronSecreto.Length, intentoJugador.Length);
        for (var i = 0; i < posicionesComunes; i++)
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
                    secretoContado[i] = true;
                    intentoContado[j] = true;
                    break;
                }
            }
        }

        return (bienPosicionados, colorCorrecto);
    }
}
