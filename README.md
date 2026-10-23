Repositorio para los archivos del curso del tercer cuatrimestre de UAM.

## Projects

- `Quiniela.BusinessLogic` contains the reusable `QuinielaScorer` class.
- `Quiniela.AppConsole` parses the command-line arguments, uses the business
  logic project, and prints the points.
- `Quiniela.Mvc` provides a web interface for entering actual and predicted
  scores and viewing the calculated points.
- `Quiniela.BusinessLogic.Tests` contains xUnit tests for the scoring rules
  and invalid score values.

Run the console app with four non-negative integer arguments in this order:
the real score for team A, the real score for team B, the prediction for team
A, and the prediction for team B.

```powershell
dotnet run --project .\Quiniela.AppConsole -- 2 1 2 0
```

The scorer awards 5 points for predicting both scores exactly, 2 points for
predicting the outcome (team A wins, team B wins, or a draw), and 1 extra point
if the outcome and either team's score are correct. An incorrect outcome earns
0 points.

Run the unit tests with:

```powershell
dotnet test .\Quiniela.BusinessLogic.Tests
```

Run the MVC app with:

```powershell
dotnet run --project .\Quiniela.Mvc
```

## Mastermind evaluator

`MastermindService` evaluates 4- or 6-color attempts using `Y`, `C`, `B`,
`P`, `W`, `K`, `R`, and `G` by default. A game should use its own service
instance; each instance tracks which cheat-code hashes have already been used
in that game.

Cheat codes are stored in `MastermindEvaluator/cheatcodes.json` as an array
containing a lowercase or uppercase SHA-256 hash and an available-use count:

```json
[
  {
    "hash": "<SHA-256 hash of the UTF-8 cheat code>",
    "usosDisponibles": 1
  }
]
```

The hash is calculated with SHA-256 over the cheat code's UTF-8 bytes. The
project copies this file beside the application at build time. Successful use
decrements `usosDisponibles` and persists the updated JSON.
