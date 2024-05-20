using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.BaseCommands;
using GECA.Client.Console.Shared;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.ConcreteCommands
{
    public class MoveCommand : BaseMovementCommand1
    {
        public MoveCommand(CaterpillarSimulation simulation, IServiceManager ServiceManager) : base(simulation, ServiceManager)
        {
        }

        public override void Execute()
        {
            try
            {
                MoveCaterpillarRequest moveRequest = new()
                {
                    CurrentRow = previousRow,
                    CurrentColumn = previousColumn,
                    Direction = AppConstants.Direction,
                    Steps = AppConstants.Steps
                };

                var response = serviceManager.CaterpillarService.MoveCaterpillar(map, moveRequest).Result;

                if (response.Successful)
                {
                    CaterpillarSimulation.caterpillarRow = response.NewCatapillarRow;
                    CaterpillarSimulation.caterpillarColumn = response.NewCatapillarColumn;

                    // Update map representation
                    map[previousRow, previousColumn] = '.';
                    map[CaterpillarSimulation.caterpillarRow, CaterpillarSimulation.caterpillarColumn] = 'C';

                    HandleEventType(response);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void HandleEventType(MoveCaterpillarResponse response)
        {
            switch (response.EventType)
            {
                case EventType.Moved:
                    // Normal movement
                    break;
                case EventType.Obstacle:
                    simulation.Caterpillar.Segments.Clear();
                    simulation.Caterpillar.Segments.Add(new Segment(SegmentType.Head));
                    simulation.Caterpillar.Segments.Add(new Segment(SegmentType.Tail));
                    break;
                case EventType.Booster:
                    var growShrinkResponse = serviceManager.CaterpillarService.GrowShrinkCaterpillar(new GrowShrinkCaterpillarRequest
                    {
                        Caterpillar = new CaterpillarDto { Caterpillar = simulation.Caterpillar },
                        Grow = AppConstants.GrowOrShrink
                    }).Result;
                    break;
                case EventType.Spice:
                    serviceManager.CaterpillarService.CollectAndStoreSpice(response.NewCatapillarRow, response.NewCatapillarColumn).Wait();
                    break;
                default:
                    break;
            }
        }
    }

}
