using System.Collections.Generic;
using System.Linq;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Data.Repository;
using BaseCamp_Web_API.Tests.Fixtures.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Controllers
{
    /// <summary>
    /// Contains unit tests for SneakersController.
    /// </summary>
    public class SneakersControllerTests : IClassFixture<SneakersControllerTestsFixture>
    {
        private readonly SneakersControllerTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="SneakersControllerTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public SneakersControllerTests(SneakersControllerTestsFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetSneakerCollection();
        }

        /// <summary>
        /// Tests controller response when creating a new sneaker.
        /// </summary>
        [Fact]
        public async void SneakersController_CreatingNewSneaker_OkObjectResultWithCreatedSneaker()
        {
            // Arrange
            var createSneakerRequest = new CreateSneakerRequest
            {
                ModelName = "SoMe MoDeL",
                VendorId = 1,
                SeasonId = 1,
                GenderId = 1,
                Price = 199.99M
            };

            // Act
            var actionResult = await _fixture.Controller.CreateAsync(createSneakerRequest);
            var sneaker = _fixture.Mapper.Map<Sneaker>(createSneakerRequest);
            SneakerRepository.PutFieldsToUppercase(sneaker);
            sneaker.Id = 6;

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(sneaker);
        }

        /// <summary>
        /// Tests controller response when fetching all of the existing sneakers.
        /// </summary>
        [Fact]
        public async void SneakersController_FetchingAllSneakers_OkObjectResultWithAllExistingSneakers()
        {
            // Arrange
            var existingSneakers = _fixture.Sneakers;

            // Act
            var actionResult = await _fixture.Controller.GetAllAsync(new PaginationFilter());

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(existingSneakers);
        }

        /// <summary>
        /// Tests controller response when fetching a sneaker by invalid ID.
        /// </summary>
        /// <param name="id">ID of a sneaker to get.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1000)]
        public async void SneakersController_FetchingSneakerByID_OkObjectResultWithEmptyIEnumerable(int id)
        {
            // Arrange
            var existingSneaker = _fixture.GetSneakerById(id);

            // Act
            var actionResult = await _fixture.Controller.GetByIdAsync(id);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Tests controller response when fetching a sneaker by its valid ID.
        /// </summary>
        [Fact]
        public async void SneakersController_FetchingSneakerByID_OkObjectResultWithIEnumerableWithSneaker()
        {
            // Arrange
            var existingId = _fixture.GetAllSneakers().ToList()[0].Id;
            var existingSneaker = _fixture.GetSneakerById(existingId);

            // Act
            var actionResult = await _fixture.Controller.GetByIdAsync(existingId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(existingSneaker);
        }

        /// <summary>
        /// Tests controller response when updating a sneaker by invalid ID.
        /// </summary>
        [Fact]
        public async void SneakersController_UpdatingSneakerByID_NotFoundResult()
        {
            // Arrange
            var nonExistingId = 1000;
            var updateSneakerRequest = new UpdateSneakerRequest
            {
                ModelName = "New Model Name",
                Price = 999.99M,
                GenderId = 1,
                SeasonId = 1,
                VendorId = 1
            };

            // Act
            var actionResult = await _fixture.Controller.UpdateAsync(nonExistingId, updateSneakerRequest);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Tests controller response when updating a sneaker by its valid ID.
        /// </summary>
        [Fact]
        public async void SneakersController_UpdatingSneakerByID_OkObjectResultWithUpdatedSneaker()
        {
            // Arrange
            var existingId = _fixture.Sneakers.ToList()[0].Id;
            var updateSneakerRequest = new UpdateSneakerRequest
            {
                ModelName = "New Model Name",
                Price = 999.99M,
                GenderId = 1,
                SeasonId = 1,
                VendorId = 1
            };

            var sneakerWithUpdateData = _fixture.Mapper.Map<Sneaker>(updateSneakerRequest);
            sneakerWithUpdateData.Id = existingId;
            ((List<Sneaker>)_fixture.Sneakers)[existingId - 1] = sneakerWithUpdateData;

            // Act
            var actionResult = await _fixture.Controller.UpdateAsync(existingId, updateSneakerRequest);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new List<Sneaker> { sneakerWithUpdateData });
        }

        /// <summary>
        /// Tests controller response when deleting a sneaker by invalid ID.
        /// </summary>
        [Fact]
        public async void SneakersController_DeletingSneakerByID_NotFoundResult()
        {
            // Arrange
            var nonExistingId = 1000;

            ((List<Sneaker>)_fixture.Sneakers).Remove(_fixture.Sneakers.SingleOrDefault(s => s.Id == nonExistingId));

            // Act
            var actionResult = await _fixture.Controller.DeleteAsync(nonExistingId);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
            _fixture.Sneakers.Count().Should().Be(5);
        }

        /// <summary>
        /// Tests controller response when deleting a sneaker by its ID.
        /// </summary>
        [Fact]
        public async void SneakersController_DeletingSneakerByID_NoContentResult()
        {
            // Arrange
            var existingId = _fixture.Sneakers.ToList()[0].Id;

            ((List<Sneaker>)_fixture.Sneakers).Remove(_fixture.Sneakers.SingleOrDefault(s => s.Id == existingId));

            // Act
            var actionResult = await _fixture.Controller.DeleteAsync(existingId);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
            _fixture.Sneakers.Where(s => s.Id == existingId).Should().BeEmpty();
        }
    }
}