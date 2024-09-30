using Microsoft.Extensions.Logging;
using FluentAssertions;
using FakeItEasy;
using NdfcAPIsMongoDB.Controllers;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository.HistoryService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;
using NdfcAPIsMongoDB.Common;

namespace NdfcAPIsMongoDB.Tests.UnitTest
{
    public class ClubHistoryControllerTests
    {
        [Fact]
        public async Task GetClubHistory_ShouldReturnOkWithClubHistory()
        {
            // Arrange
            var historyRepository = A.Fake<IHistoryRepositorycs>();
            var cache = A.Fake<IMemoryCache>();
            var logger = A.Fake<ILogger<BaseController>>();

            var expectedHistory = new ClubHistory { Id = "1", Name = "Test Club" };
            A.CallTo(() => historyRepository.GetClubHistoryAsync()).Returns(expectedHistory);

            var controller = new ClubHistoryController(historyRepository, cache, logger);

            // Act
            var result = await controller.GetClubHistory();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHistory = Assert.IsType<ApiResponse<ClubHistory>>(okObjectResult.Value);

            returnedHistory.Data.Should().BeEquivalentTo(expectedHistory);
            A.CallTo(() => historyRepository.GetClubHistoryAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateClubHistory_ShouldReturnOkResult()
        {
            // Arrange
            var historyRepository = A.Fake<IHistoryRepositorycs>();
            var cache = A.Fake<IMemoryCache>();
            var logger = A.Fake<ILogger<BaseController>>();

            var clubHistoryToUpdate = new ClubHistory { Id = "1", Name = "Updated Club" };
            A.CallTo(() => historyRepository.UpdateClubHistoryAsync(A<ClubHistory>._)).DoesNothing();

            var controller = new ClubHistoryController(historyRepository, cache, logger);

            // Act
            var result = await controller.UpdateClubHistory(clubHistoryToUpdate);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okObjectResult.Value);

            apiResponse.Message.Should().Be("Cập nhật thành công");
            A.CallTo(() => historyRepository.UpdateClubHistoryAsync(A<ClubHistory>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DeleteClubHistory_ShouldReturnOkResult()
        {
            // Arrange
            var historyRepository = A.Fake<IHistoryRepositorycs>();
            var cache = A.Fake<IMemoryCache>();
            var logger = A.Fake<ILogger<BaseController>>();

            A.CallTo(() => historyRepository.DeleteClubHistoryAsync()).DoesNothing();

            var controller = new ClubHistoryController(historyRepository, cache, logger);

            // Act
            var result = await controller.DeleteClubHistory();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okObjectResult.Value);

            apiResponse.Message.Should().Be("Xóa thành công");
            A.CallTo(() => historyRepository.DeleteClubHistoryAsync()).MustHaveHappenedOnceExactly();
        }
    }
}

