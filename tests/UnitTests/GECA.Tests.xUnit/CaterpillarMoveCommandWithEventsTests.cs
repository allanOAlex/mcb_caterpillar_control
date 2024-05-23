using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.ConcreteCommands;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Tests.xUnit
{
    public class CaterpillarMoveCommandWithEventsTests
    {
        private readonly Mock<ICaterpillarService> _caterpillarServiceMock;
        private readonly Mock<IMapService> _mapServiceMock;
        private Caterpillar _caterpillar;
        private char[,] _map;
        private MoveCaterpillarRequest _moveCaterpillarRequest;
        private CaterpillarMoveCommand _command;

        public CaterpillarMoveCommandWithEventsTests()
        {
            _caterpillarServiceMock = new Mock<ICaterpillarService>();
            _mapServiceMock = new Mock<IMapService>();

            _caterpillar = new Caterpillar
            {
                CurrentRow = 15,
                CurrentColumn = 15,
                Segments = new List<Segment> { new Segment(SegmentType.Head), new Segment(SegmentType.Tail) }
            };

            _map = new char[30, 30];
            _map[15, 15] = 'C';

            _moveCaterpillarRequest = new MoveCaterpillarRequest
            {
                CurrentRow = 15,
                CurrentColumn = 15,
                Direction = "UP",
                Steps = 1
            };

            _command = new CaterpillarMoveCommand(_caterpillar, _map, _moveCaterpillarRequest, _caterpillarServiceMock.Object, _mapServiceMock.Object);
        }

        [Fact]
        public async Task Undo_ShouldRevertBooster()
        {
            // Arrange
            _command.SaveCurrentState();
            _map[14, 15] = 'B'; // Caterpillar finds a booster

            var moveResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.Booster,
                NewCatapillarRow = 14,
                NewCatapillarColumn = 15
            };

            _caterpillarServiceMock
                .Setup(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(moveResponse);

            var initialSegments = _caterpillar.Segments.Count;

            var growShrinkCaterpillarRequest = new GrowShrinkCaterpillarRequest
            {
                Caterpillar = new CaterpillarDto { Caterpillar = _caterpillar },
                Grow = true
            };

            var growShrinkResponse = new GrowShrinkCaterpillarResponse
            {
                Successful = true,
                InitialSegments = initialSegments,
                CurrentSegments = initialSegments + 1,
                PreviousCaterpillarSegments = _caterpillar.Segments.ToList(),
                CurrentCaterpillarSegments = _caterpillar.Segments.Append(new Segment(SegmentType.Intermediate)).ToList()
            };

            _caterpillarServiceMock
                .Setup(s => s.GrowShrinkCaterpillar(It.IsAny<GrowShrinkCaterpillarRequest>()))
                .ReturnsAsync(growShrinkResponse);

            _caterpillar.Segments = growShrinkResponse.CurrentCaterpillarSegments;

            await _command.ExecuteAsync();

            _caterpillar.CurrentRow = 14;
            _caterpillar.CurrentColumn = 15;
            _caterpillar.PreviousSegmentCount = initialSegments;
            _caterpillar.CurrentSegmentCount = growShrinkResponse.CurrentSegments;
            _caterpillar.PreviousSegments = growShrinkResponse.PreviousCaterpillarSegments;

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(15, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal(initialSegments, _caterpillar.Segments.Count); // Ensure the segment count is reverted
            Assert.Equal('C', _map[15, 15]);
            Assert.Equal('B', _map[14, 15]);

            _caterpillarServiceMock.Verify(s => s.GrowShrinkCaterpillar(It.IsAny<GrowShrinkCaterpillarRequest>()), Times.Once);
        }

        [Theory]
        [InlineData(EventType.HorizontalCrossBoundary, true)]
        [InlineData(EventType.VerticalCrossBoundary, false)]
        public async Task Undo_ShouldRevertBoundaryCross(EventType eventType, bool isHorizontal)
        {
            // Arrange
            _command.SaveCurrentState();

            // Initial move to set the state
            var initialResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.Moved,
                NewCatapillarRow = 1,
                NewCatapillarColumn = 1
            };

            // Setup the sequence of moves
            _caterpillarServiceMock
                .SetupSequence(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(initialResponse)
                .ReturnsAsync(new MoveCaterpillarResponse
                {
                    Successful = true,
                    EventType = eventType,
                    NewCatapillarRow = eventType == EventType.HorizontalCrossBoundary ? 0 : 1,
                    NewCatapillarColumn = eventType == EventType.VerticalCrossBoundary ? 0 : 1
                });

            // Execute the initial move
            await _command.ExecuteAsync();

            _caterpillar.CurrentRow = eventType == EventType.HorizontalCrossBoundary ? 0 : 1;
            _caterpillar.CurrentColumn = eventType == EventType.VerticalCrossBoundary ? 0 : 1;
            _map[_caterpillar.CurrentRow, _caterpillar.CurrentColumn] = 'C';

            // Mock the map service method to return a valid ReplicateMapResponse
            var replicateMapResponse = new ReplicateMapResponse
            {
                Map = _map,
                NewCaterpillarRow = 1,
                NewCaterpillarColumn = 1
            };

            _mapServiceMock
                .Setup(s => s.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(It.Is<ReplicateMapRequest>(req => req.IsHorizontalMirroring == isHorizontal)))
                .ReturnsAsync(replicateMapResponse);

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(1, _caterpillar.CurrentRow);
            Assert.Equal(1, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[1, 1]);

            _mapServiceMock.Verify(s => s.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(It.Is<ReplicateMapRequest>(req =>
                req.IsHorizontalMirroring == isHorizontal
            )), Times.Once);
        }

        [Fact]
        public async Task Undo_ShouldRevertHitMapBoundary()
        {
            // Arrange

            _moveCaterpillarRequest = new MoveCaterpillarRequest
            {
                CurrentRow = 15,
                CurrentColumn = 15,
                Direction = "UP",
                Steps = 14
            };

            var moveResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.HitMapBoundary,
                NewCatapillarRow = 1,
                NewCatapillarColumn = 14
            };

            _caterpillarServiceMock
            .Setup(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
            .ReturnsAsync(moveResponse);

            _command.SaveCurrentState();
            var response = await _command.ExecuteAsync();

            var boundaryResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.HitMapBoundary,
                NewCatapillarRow = 1,
                NewCatapillarColumn = 15
            };

            _caterpillarServiceMock
                .Setup(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(boundaryResponse);


            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(15, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[15, 15]);
        }

        [Fact]
        public async Task Undo_ShouldRevertMove()
        {
            // Arrange
            _command.SaveCurrentState();
            _map[14, 15] = 'C'; // Caterpillar moved up

            // Define the MoveCaterpillarResponse object
            var moveResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.Moved,
                NewCatapillarRow = 14,
                NewCatapillarColumn = 15
            };

            _caterpillarServiceMock
                .Setup(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(moveResponse); // Mock the move response when executing the command

            await _command.ExecuteAsync(); // Execute the command

            _caterpillar.CurrentRow = 14;
            _caterpillar.CurrentColumn = 15;

            // Act
            await _command.Undo(); // Undo the move

            // Assert
            Assert.NotNull(moveResponse); // Ensure that the move response is not null
            Assert.Equal(15, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[15, 15]);
            Assert.Equal('.', _map[14, 15]);
        }

        [Fact]
        public async Task Undo_ShouldRevertObstacle()
        {
            // Arrange
            _command.SaveCurrentState();
            _map[14, 15] = '#'; // Caterpillar hits an obstacle

            // Setup the response for MoveCaterpillar
            var initialResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.Obstacle, // Specify the EventType as Obstacle
                NewCatapillarRow = 14,
                NewCatapillarColumn = 15
            };

            _caterpillarServiceMock
                .Setup(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(initialResponse);

            // Execute the command
            await _command.ExecuteAsync();

            // Set the caterpillar's current position after the move
            _caterpillar.CurrentRow = 14;
            _caterpillar.CurrentColumn = 15;

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(15, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[15, 15]);
            Assert.Equal('#', _map[14, 15]);

            _caterpillarServiceMock
                .Setup(s => s.UnDestroyCaterpillar(It.IsAny<char[,]>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
        }

        [Fact]
        public async Task Undo_ShouldRevertSpice()
        {
            // Arrange
            _command.SaveCurrentState();
            _map[14, 15] = 'S'; // Caterpillar finds a spice

            // Setup the response for MoveCaterpillar
            var initialResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.Spice,
                NewCatapillarRow = 14,
                NewCatapillarColumn = 15
            };

            _caterpillarServiceMock
                .Setup(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(initialResponse);

            // Execute the command
            var response = await _command.ExecuteAsync();

            // Set the caterpillar's current position after the move
            _caterpillar.CurrentRow = 14;
            _caterpillar.CurrentColumn = 15;

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(15, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[15, 15]);
            Assert.Equal('S', _map[14, 15]);
        }


    }
}
