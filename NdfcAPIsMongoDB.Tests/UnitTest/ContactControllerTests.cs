using Microsoft.Extensions.Logging;
using FluentAssertions;
using FakeItEasy;
using NdfcAPIsMongoDB.Controllers;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.ContactService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;

// Nên viết thế này để thực hiện test

namespace NdfcAPIsMongoDB.Tests.UnitTest
{
    public class ContactControllerTests
    {
        [Fact]
        public async Task CreateContact_ShouldReturnCreatedContact()
        {
            // Arrange
            var contactRepository = A.Fake<IContact>();
            var cache = A.Fake<IMemoryCache>();
            var logger = A.Fake<ILogger<BaseController>>();

            var expectedContactDto = new ContactDTO
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Topic = "Test",
                Detail = "This is a test contact"
            };

            var expectedContact = new Contact
            {
                Name = expectedContactDto.Name,
                Email = expectedContactDto.Email,
                Topic = expectedContactDto.Topic,
                Detail = expectedContactDto.Detail,
            };

            A.CallTo(() => contactRepository.CreateContact(A<Contact>._)).Returns(expectedContact);

            var controller = new ContactController(contactRepository, cache, logger);

            // Act
            var result = await controller.CreateContact(expectedContactDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var createdContact = Assert.IsType<Contact>(okObjectResult.Value);

            createdContact.Name.Should().Be(expectedContactDto.Name);
            createdContact.Email.Should().Be(expectedContactDto.Email);
            createdContact.Topic.Should().Be(expectedContactDto.Topic);
            createdContact.Detail.Should().Be(expectedContactDto.Detail);

            A.CallTo(() => contactRepository.CreateContact(A<Contact>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetContactById_ExistingId_ShouldReturnContact()
        {
            // Arrange
            var contactRepository = A.Fake<IContact>();
            var cache = A.Fake<IMemoryCache>();
            var logger = A.Fake<ILogger<BaseController>>();

            var expectedContact = new Contact
            {
                Id = "1",
                Name = "John Doe",
                Email = "johndoe@example.com",
                Topic = "Test",
                Detail = "This is a test contact"
            };

            A.CallTo(() => contactRepository.GetContactById(A<string>._)).Returns(expectedContact);

            var controller = new ContactController(contactRepository, cache, logger);

            // Act
            var result = await controller.GetContactById("1");

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedContact = Assert.IsType<Contact>(okObjectResult.Value);

            returnedContact.Id.Should().Be(expectedContact.Id);
            returnedContact.Name.Should().Be(expectedContact.Name);
            returnedContact.Email.Should().Be(expectedContact.Email);
            returnedContact.Topic.Should().Be(expectedContact.Topic);
            returnedContact.Detail.Should().Be(expectedContact.Detail);

            A.CallTo(() => contactRepository.GetContactById(A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DeleteContact_ExistingId_ShouldReturnNoContent()
        {
            // Arrange
            var contactRepository = A.Fake<IContact>();
            var cache = A.Fake<IMemoryCache>();
            var logger = A.Fake<ILogger<BaseController>>();

            var existingContact = new Contact
            {
                Id = "1",
                Name = "John Doe",
                Email = "johndoe@example.com",
                Topic = "Test",
                Detail = "This is a test contact"
            };

            A.CallTo(() => contactRepository.GetContactById(A<string>._)).Returns(existingContact);
            A.CallTo(() => contactRepository.DeleteContact(A<string>._)).Returns(true);

            var controller = new ContactController(contactRepository, cache, logger);

            // Act
            var result = await controller.DeleteContact("1");

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);

            A.CallTo(() => contactRepository.GetContactById(A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => contactRepository.DeleteContact(A<string>._)).MustHaveHappenedOnceExactly();
        }
    }
}
