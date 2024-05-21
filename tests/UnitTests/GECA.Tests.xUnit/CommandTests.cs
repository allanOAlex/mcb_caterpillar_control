using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.ConcreteCommands;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Helpers;
using Moq;

namespace GECA.Tests.xUnit
{
    public class CommandTests
    {
        private Caterpillar _caterpillar;
        private char[,] _map;
        private Mock<ICaterpillarService> _caterpillarServiceMock;
        private Mock<IMapService> _mapServiceMock;
        private CommandHistory _commandHistory;

        public CommandTests()
        {
            _caterpillar = new Caterpillar { CurrentRow = 0, CurrentColumn = 0, Segments = new List<Segment> { new Segment(SegmentType.Head) } };
            _map = new char[10, 10];
            _caterpillarServiceMock = new Mock<ICaterpillarService>();
            _mapServiceMock = new Mock<IMapService>();
            _commandHistory = new CommandHistory();
        }

        [Fact]
        public async Task TestExecuteUndoRedo()
        {
            // Arrange
            var moveRequest = new MoveCaterpillarRequest { CurrentRow = 0, CurrentColumn = 0, NewRow = 1, NewColumn = 1 };
            var moveResponse = new MoveCaterpillarResponse { EventType = EventType.None, NewCatapillarRow = 1, NewCatapillarColumn = 1 };
            _caterpillarServiceMock.Setup(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>())).ReturnsAsync(moveResponse);

            var command = new CaterpillarMoveCommand(_caterpillar, _map, moveRequest, _caterpillarServiceMock.Object, _mapServiceMock.Object);

            // Act
            _commandHistory.Execute(command);

            // Assert
            Assert.Equal(1, _caterpillar.CurrentRow);
            Assert.Equal(1, _caterpillar.CurrentColumn);

            // Act
            _commandHistory.Undo();

            // Assert
            Assert.Equal(0, _caterpillar.CurrentRow);
            Assert.Equal(0, _caterpillar.CurrentColumn);

            // Act
            _commandHistory.Redo();

            // Assert
            Assert.Equal(1, _caterpillar.CurrentRow);
            Assert.Equal(1, _caterpillar.CurrentColumn);
        }
    }
}