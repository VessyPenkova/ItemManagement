using NUnit.Framework;
using Moq;
using ItemManagementApp.Services;
using ItemManagementLib.Repositories;
using ItemManagementLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace ItemManagement.Tests
{
    [TestFixture]
    public class ItemServiceTests
    {
        // Field to hold the mock repository and the service being tested
        private ItemService _itemService;
        private Mock<IItemRepository> _mockItemRepository;

        [SetUp]
        public void Setup()
        {
            //Creating and mocking instance of IItemRepository
            _mockItemRepository = new Mock<IItemRepository>();

            // Instantiate ItemService with the mocked repository
            _itemService = new ItemService(_mockItemRepository.Object);

        }

        [Test]
        public void AddItem_ShouldCallAddItemOnRepositoryIfNameIsValid()
        {
            //Arrange
            var item = new Item { Id = 1, Name = "Single Item" };
            _mockItemRepository.Setup(x => x.AddItem(It.IsAny<Item>()));

            // Act: Call AddItem on the service
             _itemService.AddItem(item.Name);

            // Assert: Verify that AddItem was called on the repository
            _mockItemRepository.Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once());
        }

        [Test]
        public void AddItem_ShouldThrowErrorIfNameIsInValid()
        {
            //Arrange
            string inValidName = "";
            _mockItemRepository
                .Setup(x => x.AddItem(It.IsAny<Item>()))
                .Throws<ArgumentException>();


            // Act & Assert: Verify that AddItem was called on the repository and throw exception
            Assert.Throws<ArgumentException>(() => _itemService.AddItem(inValidName));
            _mockItemRepository
                .Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once());
        }

        [Test]
        public void GetAllItems_ShouldReturnAllItems()
        {
            //Arrange
            var items = new List<Item> { new Item { Id = 1, Name = "SampleItem" }};
            _mockItemRepository.Setup(x =>x.GetAllItems()).Returns(items);

            //Act
            var result = _itemService.GetAllItems();

            //Assert
            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));
            _mockItemRepository.Verify(x=>x.GetAllItems(), Times.Once());
        }

        [Test]
        public void GetAllItemById_ShouldReturnAllItemById_IfItemExist()
        {
            //Arrange
            var item =  new Item { Id = 1, Name = "Single Item" } ;
            _mockItemRepository.Setup(x => x.GetItemById(1)).Returns(item);

            //Act
            var result = _itemService.GetItemById(item.Id);

            //Assert
            Assert.NotNull(result);
            Assert.That(result.Id, Is.EqualTo(item.Id));
            Assert.That(result.Name, Is.EqualTo(item.Name));
            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
        }

        [Test ]
        public void GetAllItemById_ShouldReturnNull_IfItemDoesNotExist()
        {
            //Arrange          
            _mockItemRepository.Setup(x => x.GetItemById(It.IsAny<int>())).Returns<Item>(null);

            //Act
            var result = _itemService.GetItemById(It.IsAny<int>());

            //Assert
            Assert.IsNull(result);
            Assert.That(result, Is.EqualTo(null));
            //Verifying that method been call only once
            _mockItemRepository.Verify(x => x.GetItemById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void UpdateItem_ShouldCallUpdateItemOnRepository()
        {
            //Arrange
            var item = new Item { Id = 1, Name = "Sample Item" };
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>()));

            //Act
            _itemService.UpdateItem(item.Id, "Sample Item Updated");

            //Asser
            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());
        }

        [Test]
        public void UpdateItem_ShouldNotUpdateItem_IfItemDeasNorExist()
        {
            //Arrange
            var nonExistingId = 1;
            _mockItemRepository.Setup(x => x.GetItemById(nonExistingId)).Returns<Item>(null);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>()));

            //Act
            _itemService.UpdateItem(nonExistingId, "DoesNotMatter");
            //Asser
            _mockItemRepository.Verify(x => x.GetItemById(nonExistingId), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Never());
        }

        [Test]
        public void UpdateItem_ShouldThrowException_IfItemNameISInvalid()
        {
            //Arrange
            var item = new Item { Id = 1, Name = "Sample Item" };      
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>()))
                .Throws<ArgumentException>();

            //Act $ Assert
            Assert.Throws<ArgumentException>(() => _itemService.UpdateItem(item.Id, ""));
            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());
        }

        [Test]
        public void DeleteItem_ShouldCallDeleteItemOnRepository()
        {
            //Arrange
            var item = new Item { Id = 1, Name = "Sample Item" };
            _mockItemRepository.Setup(x => x.DeleteItem(item.Id));

            //Act
            _itemService.DeleteItem(item.Id);

            //Assert
            _mockItemRepository.Verify(x => x.DeleteItem(item.Id), Times.Once());
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv", false)]
        [TestCase("SampleName", true)]
        [TestCase("Sample", true)]
        public void ValidateItemName_WhenNameIsValid_ShouldReturnCorrectAnswer_IfItemNameIsValid(string name, bool isValid)
        {
           
            //Act
            var result = _itemService.ValidateItemName(name);

            //Assert
            Assert.That(result, Is.EqualTo(isValid));

        }
    }
}