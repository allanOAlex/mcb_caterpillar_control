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
    public class CaterpillarMove_Undo_CommandTests
    {
        private readonly Mock<ICaterpillarService> _caterpillarServiceMock;
        private readonly Mock<IMapService> _mapServiceMock;
        private Caterpillar _caterpillar;
        private char[,] _map;
        private MoveCaterpillarRequest _moveCaterpillarRequest;
        private CaterpillarMoveCommand _command;

        public CaterpillarMove_Undo_CommandTests()
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

            var initialSegmentCount = _caterpillar.Segments.Count;

            var growShrinkCaterpillarRequest = new GrowShrinkCaterpillarRequest
            {
                Caterpillar = new CaterpillarDto { Caterpillar = _caterpillar },
                Grow = true
            };

            var growShrinkResponse = new GrowShrinkCaterpillarResponse
            {
                Successful = true,
                InitialSegmentCount = initialSegmentCount,
                CurrentSegmentCount = initialSegmentCount + 1,
                PreviousCaterpillarSegments = _caterpillar.Segments.ToList(),
                CurrentCaterpillarSegments = _caterpillar.Segments.Append(new Segment(SegmentType.Intermediate)).ToList()
            };

            _caterpillarServiceMock
                .Setup(s => s.GrowShrinkCaterpillar(It.IsAny<GrowShrinkCaterpillarRequest>()))
                .ReturnsAsync(growShrinkResponse);

            _caterpillar.Segments = growShrinkResponse.CurrentCaterpillarSegments;

            await _command.ExecuteAsync();

            _caterpillar.PreviousSegmentCount = initialSegmentCount;
            _caterpillar.CurrentSegmentCount = growShrinkResponse.CurrentSegmentCount;
            _caterpillar.PreviousSegments = growShrinkResponse.PreviousCaterpillarSegments;
            _caterpillar.Segments = growShrinkResponse.CurrentCaterpillarSegments;

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(15, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal(initialSegmentCount, _caterpillar.Segments.Count); // Ensure the segment count is reverted
            Assert.Equal('C', _map[15, 15]);
            Assert.Equal('B', _map[14, 15]);

            _caterpillarServiceMock.Verify(s => s.GrowShrinkCaterpillar(It.IsAny<GrowShrinkCaterpillarRequest>()), Times.Once);
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
        public async Task Undo_ShouldRevertHorizontalBoundaryCross()
        {
            // Arrange
            _command.SaveCurrentState();

            _moveCaterpillarRequest = new MoveCaterpillarRequest
            {
                CurrentRow = 15,
                CurrentColumn = 15,
                Direction = "RIGHT",
                Steps = 15
            };

            var initialResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.HorizontalCrossBoundary,
                NewCatapillarRow = 15,
                NewCatapillarColumn = 0
            };

            // Setup the sequence of moves
            _caterpillarServiceMock
                .SetupSequence(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(initialResponse);
                
            // Execute the initial move
            await _command.ExecuteAsync();

            _caterpillar.CurrentRow = 15;
            _caterpillar.CurrentColumn = 0;
            _map[_caterpillar.CurrentRow, _caterpillar.CurrentColumn] = 'C';

            // Mock the map service method to return a valid ReplicateMapResponse
            var replicateMapResponse = new ReplicateMapResponse
            {
                Map = _map,
                NewCaterpillarRow = 15,
                NewCaterpillarColumn = 0
            };

            _mapServiceMock
                .Setup(s => s.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(It.Is<ReplicateMapRequest>(req => req.IsHorizontalMirroring)))
                .ReturnsAsync(replicateMapResponse);

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(_caterpillar.PreviousRow, _caterpillar.CurrentRow);
            Assert.Equal(_caterpillar.PreviousColumn, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[_caterpillar.PreviousRow, _caterpillar.PreviousColumn]);

            _mapServiceMock.Verify(s => s.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(It.Is<ReplicateMapRequest>(req =>
                req.IsHorizontalMirroring
            )), Times.Once);
        }

        [Fact]
        public async Task Undo_ShouldRevertMove()
        {
            // Arrange
            _command.SaveCurrentState();
            _map[14, 15] = 'C'; // Caterpillar moved up

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

            await _command.ExecuteAsync(); 

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

            await _command.ExecuteAsync();

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

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(15, _caterpillar.CurrentRow);
            Assert.Equal(15, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[15, 15]);
            Assert.Equal('S', _map[14, 15]);
        }

        [Fact]
        public async Task Undo_ShouldRevertVerticalBoundaryCross()
        {
            // Arrange
            _command.SaveCurrentState();

            _moveCaterpillarRequest = new MoveCaterpillarRequest
            {
                CurrentRow = 15,
                CurrentColumn = 15,
                Direction = "UP",
                Steps = 15
            };

            // Initial move to set the state
            var initialResponse = new MoveCaterpillarResponse
            {
                Successful = true,
                EventType = EventType.VerticalCrossBoundary,
                NewCatapillarRow = 0,
                NewCatapillarColumn = 15
            };

            // Setup the sequence of moves
            _caterpillarServiceMock
                .SetupSequence(s => s.MoveCaterpillar(It.IsAny<char[,]>(), It.IsAny<MoveCaterpillarRequest>()))
                .ReturnsAsync(initialResponse);
                
            // Execute the initial move
            await _command.ExecuteAsync();

            _caterpillar.CurrentRow = 0;
            _caterpillar.CurrentColumn = 15;
            _map[_caterpillar.CurrentRow, _caterpillar.CurrentColumn] = 'C';

            // Mock the map service method to return a valid ReplicateMapResponse
            var replicateMapResponse = new ReplicateMapResponse
            {
                Map = _map,
                NewCaterpillarRow = 0,
                NewCaterpillarColumn = 15
            };

            _mapServiceMock
                .Setup(s => s.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(It.Is<ReplicateMapRequest>(req => !req.IsHorizontalMirroring)))
                .ReturnsAsync(replicateMapResponse);

            // Act
            await _command.Undo();

            // Assert
            Assert.Equal(_caterpillar.PreviousRow, _caterpillar.CurrentRow);
            Assert.Equal(_caterpillar.PreviousColumn, _caterpillar.CurrentColumn);
            Assert.Equal('C', _map[_caterpillar.PreviousRow, _caterpillar.PreviousColumn]);

            _mapServiceMock.Verify(s => s.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(It.Is<ReplicateMapRequest>(req =>
                !req.IsHorizontalMirroring
            )), Times.Once);
        }



    }
}
