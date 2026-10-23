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
        return new MastermindService().EvaluarIntento(patronSecreto, intentoJugador);
    }
}
