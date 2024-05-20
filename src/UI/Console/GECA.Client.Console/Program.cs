
using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Abstractions.IRepositories;
using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.ConcreteCommands;
using GECA.Client.Console.Infrastructure.Implementations.Interfaces;
using GECA.Client.Console.Infrastructure.Implementations.Repositories;
using GECA.Client.Console.Infrastructure.Implementations.Services;
using GECA.Client.Console.Shared;
using Serilog;
using Serilog.Formatting.Json;



#region SerilogConfig

string logFileName = "caterpillar_control_log.txt";

string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFileName);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Async(s => s.File(new JsonFormatter(), "caterpillar_control_log.txt", rollingInterval: RollingInterval.Day))
    .CreateLogger();

#endregion

ICaterpillarRepository caterpillarRepository = new InMemoryCaterpillarRepository();
ISpiceRepository spiceRepository = new InMemorySpiceRepository();
IUnitOfWork unitOfWork = new UnitOfWork(caterpillarRepository, spiceRepository);
ICaterpillarService caterpillarService = new CaterpillarService(unitOfWork);
IMapService mapService = new MapService(unitOfWork);
IServiceManager serviceManager = new ServiceManager(caterpillarService, mapService);


#region GenerateAndPrintMap

CaterpillarSimulation caterpillarSimulation = new CaterpillarSimulation(serviceManager);


// Generate the map asynchronously
char[,] asyncMap = await caterpillarSimulation.GenerateMapAsync(new GenerateMapRequest
{
    Size = 30,
    ObstacleCount = 5,
    BoosterCount = 3,
    SpiceCount = 2
});


CaterpillarSimulation simulation = new CaterpillarSimulation(asyncMap, serviceManager);

int size = await serviceManager.MapService.GetMapSize(asyncMap);

// Print the map to the console
simulation.PrintMap(asyncMap, size);

#endregion

#region CaterpillarMovementSimulation

int moveCount = 0;
string direction;

// Movement simulation
while (true)
{
    Console.WriteLine("Enter direction (UP, DOWN, LEFT, RIGHT) or Q to quit:");
    bool validInput = false;

    do
    {
        direction = Console.ReadLine()!.ToUpper();

        if (direction == "Q")
            break;

        validInput = direction.Equals("UP", StringComparison.OrdinalIgnoreCase) || direction.Equals("U", StringComparison.OrdinalIgnoreCase) || direction.Equals("DOWN", StringComparison.OrdinalIgnoreCase) || direction.Equals("D", StringComparison.OrdinalIgnoreCase) || direction.Equals("LEFT", StringComparison.OrdinalIgnoreCase) || direction.Equals("L", StringComparison.OrdinalIgnoreCase) || direction.Equals("RIGHT", StringComparison.OrdinalIgnoreCase) || direction.Equals("R", StringComparison.OrdinalIgnoreCase);

        if (!validInput)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter a correct value.");
            Console.ResetColor();
        }
    } while (!validInput);

    Console.WriteLine("Enter number of steps:");
    int steps;
    if (!int.TryParse(Console.ReadLine(), out steps))
    {
        Console.WriteLine("Invalid input for steps. Please enter a valid integer.");
        continue;
    }

    AppConstants.Direction = direction;
    AppConstants.Steps = steps;

    await simulation.MoveCaterpillar_(new MoveCaterpillarRequest
    {
        Direction = direction,
        Steps = steps,
    });

    Console.Clear();
    simulation.PrintMap(asyncMap, size);
    simulation.DisplayRadar(asyncMap, AppConstants.CurrentCaterpillarRow, AppConstants.CurrentCaterpillarColumn, AppConstants.RadarRange);

    moveCount++;
    if (moveCount >= 100)
    {
        Console.WriteLine("Reached maximum number of moves (100). Simulation ending.");
        await Task.Delay(1000);
        return;
    }

}

#endregion

public class CaterpillarSimulation
{
    public readonly char[,] map;
    public static int caterpillarRow;
    public static int caterpillarColumn;
    private List<CollectedSpice> collectedSpices; // Collection to store encountered spices
    private bool caterpillarDestroyed; // Flag to indicate if caterpillar is destroyed
    private bool isHorizontalMirroring; // Flag to indicate type of map mirroring

    private readonly IServiceManager serviceManager;
    private readonly ICaterpillarService caterpillarService;
    private readonly IMapService mapService;
    private readonly string logFilePath;
    public Caterpillar Caterpillar;
    private Stack<ICommand> commandHistory = new();
    private Stack<ICommand1> commandHistoryGeneric = new();
    private Stack<ICommand2> moveCommandHistory = new();

    public event Action ObstacleEncountered;
    public event Action BoosterEncountered;
    public event Action SpiceCollected;
    public event Action BoundaryCrossed;
    public event Action BoundaryHit;
    public event Action<string> GrowShrinkDecisionRequested;

    public CaterpillarSimulation(char[,] asyncMap, IServiceManager ServiceManager)
    {
        serviceManager = ServiceManager;
        Caterpillar = new Caterpillar();

        map = asyncMap;
        collectedSpices = new List<CollectedSpice>();
        caterpillarDestroyed = false;
        isHorizontalMirroring = false;
        logFilePath = "caterpillar_control_log.txt";

        PlaceCaterpillar(map);
    }

    public CaterpillarSimulation(IServiceManager ServiceManager)
    {
        serviceManager = ServiceManager;
    }

    public async Task<char[,]> GenerateMapAsync(GenerateMapRequest generateMapRequest)
    {
        try
        {
            var serviceResponse = await serviceManager.MapService.GenerateMapAsync(generateMapRequest);
            return await Task.FromResult(serviceResponse);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<PlaceCaterpillarResponse> PlaceCaterpillar(char[,] map)
    {

        var serviceResponse = await serviceManager.MapService.PlaceCaterpillar(map);
        caterpillarRow = serviceResponse.Row;
        caterpillarColumn = serviceResponse.Column;

        Caterpillar.CurrentRow = caterpillarRow;
        Caterpillar.CurrentColumn = caterpillarColumn;

        return serviceResponse;

    }

    public void PrintMap(char[,] map, int size)
    {
        try
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Check if the cell contains caterpillar, an obstacle, booster, or spice
                    if (i == caterpillarRow && j == caterpillarColumn)
                    {
                        if (caterpillarDestroyed == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue; // Set color for the caterpillar
                        }
                    }
                    else if (map[i, j] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Set color for obstacles
                    }
                    else if (map[i, j] == 'B')
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Set color for boosters
                    }
                    else if (map[i, j] == 'S')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow; // Set color for spices
                    }

                    // Print the cell content
                    Console.Write(map[i, j] + " ");

                    Console.ResetColor();
                }
                Console.WriteLine(); // Move to the next line after printing a row
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public async Task<MoveCaterpillarResponse> MoveCaterpillar(MoveCaterpillarRequest moveCaterpillarRequest)
    {
        moveCaterpillarRequest.CurrentRow = caterpillarRow;
        moveCaterpillarRequest.CurrentColumn = caterpillarColumn;

        var serviceResponse = await serviceManager.CaterpillarService.MoveCaterpillar(map, moveCaterpillarRequest);

        Log.Information("Caterpillar Movement: " +
            "{DateTime}: {Direction}: {Steps}: " +
            "{PreviousRow}: {PreviousCol}: " +
            "{CurrentRow}: {CurrentCol}:",
            DateTime.Now, moveCaterpillarRequest.Direction, moveCaterpillarRequest.Steps,
            caterpillarRow, caterpillarColumn,
            serviceResponse.NewCatapillarRow, serviceResponse.NewCatapillarColumn
            );

        caterpillarRow = serviceResponse.NewCatapillarRow;
        caterpillarColumn = serviceResponse.NewCatapillarRow;
        AppConstants.CurrentCaterpillarRow = serviceResponse.NewCatapillarRow;
        AppConstants.CurrentCaterpillarColumn = serviceResponse.NewCatapillarColumn;

        switch (serviceResponse.EventType)
        {
            case EventType.Moved:
                
                break;

            case EventType.Obstacle:
                caterpillarDestroyed = true;
                AppConstants.CurrentCaterpillarRow = moveCaterpillarRequest.CurrentRow;
                AppConstants.CurrentCaterpillarColumn = moveCaterpillarRequest.CurrentColumn;
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("May Day! We hit an Obstacle! ");
                Console.ResetColor();
                await serviceManager.CaterpillarService.DestroyCaterpillar(map, moveCaterpillarRequest.CurrentRow, moveCaterpillarRequest.CurrentColumn);
                break;

            case EventType.Booster:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("We encountered Booster! ");
                Console.WriteLine();

                Console.WriteLine("Do you wish to Grow or Shrink the Caterpillar? \n" +
                    "(For Grow, enter G or Grow || For Shrink, enter S Shrink");
                Console.ResetColor();

                string input;
                bool validInput = false;

                do
                {
                    input = Console.ReadLine()?.ToUpper();
                    validInput = input == "GROW" || input == "G" || input == "SHRINK" || input == "S";

                    if (!validInput)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input. Please enter 'G' or 'Grow' to grow, or 'S' or 'Shrink' to shrink.");
                        Console.ResetColor();
                    }
                } while (!validInput);

                bool grow = input.Equals("GROW", StringComparison.OrdinalIgnoreCase) || input.Equals("G", StringComparison.OrdinalIgnoreCase);
                bool shrink = input.Equals("SHRINK", StringComparison.OrdinalIgnoreCase) ||input.Equals("S", StringComparison.OrdinalIgnoreCase);

                if (grow || shrink)
                {
                    AppConstants.GrowOrShrink = grow ? true : false;
                    var growShrinkResponse = await serviceManager.CaterpillarService.GrowShrinkCaterpillar(new GrowShrinkCaterpillarRequest
                    {
                        Caterpillar = new CaterpillarDto() { Caterpillar = new() },
                        Grow = AppConstants.GrowOrShrink
                    });

                    if (growShrinkResponse.Successful)
                    {
                        if (growShrinkResponse.CaterpillarGrown)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Caterpillar grown by {growShrinkResponse.CurrentSegments - growShrinkResponse.InitialSegments} sement(s)");
                            Console.ResetColor();
                        }
                        else if (growShrinkResponse.CaterpillarShrunk)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Caterpillar shrunk by {growShrinkResponse.InitialSegments - growShrinkResponse.CurrentSegments} sement(s)");
                            Console.ResetColor();
                        }

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Initial Segment Count: {growShrinkResponse.InitialSegments}");
                        Console.WriteLine($"Current Segment Count: {growShrinkResponse.CurrentSegments}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("Operation failed. Caterpillar is already at maximum/minimum size.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }

                map[serviceResponse.NewCatapillarRow, serviceResponse.NewCatapillarColumn] = '.';
                map[serviceResponse.NewCatapillarRow, serviceResponse.NewCatapillarColumn] = 'C';

                break;

            case EventType.Spice:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Spice! No Sauce ");
                var spiceCount = await serviceManager.CaterpillarService.CollectAndStoreSpice(serviceResponse.NewCatapillarRow, serviceResponse.NewCatapillarColumn);
                if (spiceCount > 0)
                {
                    Console.WriteLine("Spice Saved! ");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something Went Wrong! I Could Not Save The Spice!! ");
                    Console.ResetColor();
                }

                map[serviceResponse.NewCatapillarRow, serviceResponse.NewCatapillarColumn] = '.';
                map[serviceResponse.NewCatapillarRow, serviceResponse.NewCatapillarColumn] = 'C';

                break;

            case EventType.HorizontalCrossBoundary:
            case EventType.VerticalCrossBoundary:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("-----------------------------------------------------------------------.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Crossing Boundaries!!");

                isHorizontalMirroring = serviceResponse.EventType == EventType.HorizontalCrossBoundary ? true : false;

                ReplicateMapRequest replicateMapRequest = new()
                {
                    Map = map,
                    CaterpillarRow = serviceResponse.NewCatapillarRow,
                    CaterpillarColumn = serviceResponse.NewCatapillarColumn,
                    IsHorizontalMirroring = isHorizontalMirroring,
                };

                var replicateMapResponse = await serviceManager.MapService.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(replicateMapRequest);

                if (isHorizontalMirroring)
                {
                    // Horizontal mirroring
                    map[replicateMapResponse.NewCaterpillarRow, map.GetLength(0) - 1 - replicateMapResponse.NewCaterpillarColumn] = 'C'; // Place caterpillar at mirrored position
                }
                else
                {
                    // Vertical mirroring
                    map[map.GetLength(0) - 1 - replicateMapResponse.NewCaterpillarRow, replicateMapResponse.NewCaterpillarColumn] = 'C'; // Place caterpillar at mirrored position
                }

                break;

            case EventType.HitMapBoundary:
                Console.WriteLine("Cannot move. Reached map boundary.");
                map[caterpillarRow, caterpillarColumn] = 'C';
                break;

            default:
                break;
        }
        return serviceResponse;

    }

    public async Task<MoveCaterpillarResponse> MoveCaterpillar_(MoveCaterpillarRequest moveCaterpillarRequest)
    {
        moveCaterpillarRequest.CurrentRow = caterpillarRow;
        moveCaterpillarRequest.CurrentColumn = caterpillarColumn;

        // Create and execute the move command
        var moveCommand = new CaterpillarMoveCommand(Caterpillar, map, moveCaterpillarRequest, serviceManager.CaterpillarService, serviceManager.MapService);
        var serviceResponse = await ExecuteMoveCommand(moveCommand);
        LogCommand(moveCommand);
       
        // Update caterpillar position
        caterpillarRow = serviceResponse.NewCatapillarRow;
        caterpillarColumn = serviceResponse.NewCatapillarColumn;
        AppConstants.CurrentCaterpillarRow = serviceResponse.NewCatapillarRow;
        AppConstants.CurrentCaterpillarColumn = serviceResponse.NewCatapillarColumn;

        // Handle event-specific logic
        switch (serviceResponse.EventType)
        {
            case EventType.Moved:
                // No additional handling required
                break;
            case EventType.Obstacle:
                caterpillarDestroyed = true;
                AppConstants.CurrentCaterpillarRow = moveCaterpillarRequest.CurrentRow;
                AppConstants.CurrentCaterpillarColumn = moveCaterpillarRequest.CurrentColumn;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("May Day! We hit an Obstacle!");
                Console.ResetColor();
                break;
            case EventType.Booster:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("We encountered Booster!");
                Console.WriteLine("Do you wish to Grow or Shrink the Caterpillar? (For Grow, enter G or Grow || For Shrink, enter S Shrink)");
                Console.ResetColor();

                string input;
                bool validInput;
                do
                {
                    input = Console.ReadLine()?.ToUpper();
                    validInput = input == "GROW" || input == "G" || input == "SHRINK" || input == "S";

                    if (!validInput)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input. Please enter 'G' or 'Grow' to grow, or 'S' or 'Shrink' to shrink.");
                        Console.ResetColor();
                    }
                } while (!validInput);

                var growShrinkResponse = await serviceManager.CaterpillarService.GrowShrinkCaterpillar(new GrowShrinkCaterpillarRequest
                {
                    Caterpillar = new CaterpillarDto() { Caterpillar = Caterpillar },
                    Grow = input == "GROW" || input == "G"
                });

                if (growShrinkResponse.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (growShrinkResponse.CaterpillarGrown)
                    {
                        Console.WriteLine($"Caterpillar grown by {growShrinkResponse.CurrentSegments - growShrinkResponse.InitialSegments} segment(s)");
                    }
                    else if (growShrinkResponse.CaterpillarShrunk)
                    {
                        Console.WriteLine($"Caterpillar shrunk by {growShrinkResponse.InitialSegments - growShrinkResponse.CurrentSegments} segment(s)");
                    }
                    Console.WriteLine($"Initial Segment Count: {growShrinkResponse.InitialSegments}");
                    Console.WriteLine($"Current Segment Count: {growShrinkResponse.CurrentSegments}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Operation failed. Caterpillar is already at maximum/minimum size.");
                }
                break;
            case EventType.Spice:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Spice! No Sauce");
                var spiceCount = await serviceManager.CaterpillarService.CollectAndStoreSpice(serviceResponse.NewCatapillarRow, serviceResponse.NewCatapillarColumn);
                if (spiceCount > 0)
                {
                    Console.WriteLine("Spice Saved!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something Went Wrong! I Could Not Save The Spice!!");
                }
                Console.ResetColor();
                break;
            case EventType.HorizontalCrossBoundary:
            case EventType.VerticalCrossBoundary:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("-----------------------------------------------------------------------.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Crossing Boundaries!!");
                Console.ResetColor();
                break;
            case EventType.HitMapBoundary:
                Console.WriteLine("Cannot move. Reached map boundary.");
                break;
        }

        return serviceResponse;
    }

    public async Task MoveCaterpillarAsync(ICommand command, MoveCaterpillarRequest moveCaterpillarRequest)
    {
        moveCaterpillarRequest.CurrentRow = caterpillarRow;
        moveCaterpillarRequest.CurrentColumn = caterpillarColumn;

        switch (moveCaterpillarRequest.Direction.ToUpper())
        {
            case "U":
            case "UP":
                command = new MoveUpCommand(this, serviceManager);
                break;
            default:
                break;
        }

        ExecuteCommand(command);
       
    }

    public void Move(IServiceManager serviceManager)
    {
        var command = new MoveCommand(this, serviceManager);
        ExecuteCommandGeneric(command);
    }

    public void DisplayRadar(char[,] map, int caterpillarRow, int caterpillarColumn, int radarRange)
    {
        Console.WriteLine("Radar Display:");

        int mapSize = map.GetLength(0);
        int halfRange = radarRange / 2;

        // Determine the boundaries for scanning
        int startRow = Math.Max(0, caterpillarRow - halfRange);
        int endRow = Math.Min(mapSize - 1, caterpillarRow + halfRange);
        int startCol = Math.Max(0, caterpillarColumn - halfRange);
        int endCol = Math.Min(mapSize - 1, caterpillarColumn + halfRange);


        // Scan the surrounding area and display the contents of each cell
        for (int i = startRow; i <= endRow; i++)
        {
            for (int j = startCol; j <= endCol; j++)
            {
                if (i == caterpillarRow && j == caterpillarColumn)
                {
                    if (!caterpillarDestroyed == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("C "); // Display the caterpillar
                        Console.ResetColor();
                    }
                    
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(map[i, j] + "."); // Display the content of the cell
                    Console.ResetColor();
                }
            }
            Console.WriteLine(); // Move to the next row
        }
    }


    public void ExecuteCommand(ICommand command)
    {
        command.ExecuteAsync(this);
        commandHistory.Push(command);
    }

    public async Task<MoveCaterpillarResponse> ExecuteMoveCommand(ICommand2 command)
    {
        var serviceResponse =  await command.ExecuteAsync();
        moveCommandHistory.Push(command);
        return serviceResponse;
    }

    public void ExecuteCommandGeneric(ICommand1 command)
    {
        command.Execute();
        commandHistoryGeneric.Push(command);
    }

    public void UndoLastCommand()
    {
        if (commandHistory.Count > 0)
        {
            var lastCommand = commandHistory.Pop();
            lastCommand.UndoAsync(this);
        }
    }

    private void LogCommand(ICommand2 command)
    {
        command.LogCommandDetails(); 
    }


}





