using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.ConcreteCommands;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Helpers;
using Moq;

namespace GECA.Tests.xUnit
{
    public class CaterpillarMoveCommandTests
    {
        private readonly Mock<ICaterpillarService> _mockCaterpillarService;
        private readonly Mock<IMapService> _mockMapService;
        private readonly Caterpillar _caterpillar;
        private readonly char[,] _map;
        private readonly MoveCaterpillarRequest _moveCaterpillarRequest;
        private readonly CaterpillarMoveCommand _command;

        public CaterpillarMoveCommandTests()
        {
            _mockCaterpillarService = new Mock<ICaterpillarService>();
            _mockMapService = new Mock<IMapService>();
            _caterpillar = new Caterpillar
            {
                Segments = new List<Segment>
                {
                    new Segment(SegmentType.Head),
                    new Segment(SegmentType.Tail)
                },
                CurrentRow = 5,
                CurrentColumn = 5
            };
            _map = new char[30, 30];
            _moveCaterpillarRequest = new MoveCaterpillarRequest
            {
                CurrentRow = 15,
                CurrentColumn = 15,
                NewRow = 12,
                NewColumn = 15
            };

            _command = new CaterpillarMoveCommand(_caterpillar, _map, _moveCaterpillarRequest, _mockCaterpillarService.Object, _mockMapService.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldMoveCaterpillar()
        {
            // Arrange
            var moveResponse = new MoveCaterpillarResponse
            {
                EventType = EventType.None,
                NewCatapillarRow = 12,
                NewCatapillarColumn = 15
            };
            _mockCaterpillarService.Setup(service => service.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                                   .ReturnsAsync(moveResponse);

            // Act
            var result = await _command.ExecuteAsync();

            // Assert
            Assert.Equal(12, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal(moveResponse, result);
        }

        [Fact]
        public async Task Undo_ShouldRestorePreviousState()
        {
            // Arrange
            var moveResponse = new MoveCaterpillarResponse
            {
                EventType = EventType.Moved,
                NewCatapillarRow = 6,
                NewCatapillarColumn = 6
            };
            _mockCaterpillarService.Setup(service => service.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                                   .ReturnsAsync(moveResponse);

            await _command.ExecuteAsync(); // Move to save state
            _caterpillar.CurrentRow = 7; // Change state
            _caterpillar.CurrentColumn = 7;
            _caterpillar.Segments.Add(new Segment(SegmentType.Intermediate));

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(5, _caterpillar.CurrentRow);
            Assert.Equal(5, _caterpillar.CurrentColumn);
            Assert.Equal(2, _caterpillar.Segments.Count);
        }

        [Fact]
        public async Task Redo_ShouldReexecuteCommand()
        {
            // Arrange
            var moveResponse = new MoveCaterpillarResponse
            {
                EventType = EventType.Moved,
                NewCatapillarRow = 6,
                NewCatapillarColumn = 6
            };
            _mockCaterpillarService.Setup(service => service.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                                   .ReturnsAsync(moveResponse);
            await _command.ExecuteAsync();
            await _command.Undo();

            // Act
            await _command.Redo();

            // Assert
            Assert.Equal(6, _caterpillar.CurrentRow);
            Assert.Equal(6, _caterpillar.CurrentColumn);
        }
    }
}