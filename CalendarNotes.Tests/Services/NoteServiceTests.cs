using AutoMapper;
using CalendarNotes.BLL.DTOs.Notes;
using CalendarNotes.BLL.Mappings;
using CalendarNotes.BLL.Services;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.Repositories.Interfaces;
using CalendarNotes.DAL.UnitOfWork;
using CalendarNotes.Shared.Exceptions;
using Moq;
using Xunit;

namespace CalendarNotes.Tests.Services
{
    public class NoteServiceTests
    {
        private readonly IMapper _mapper;

        public NoteServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new BLLMappingProfile()));
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task CreateAsync_Should_Save_Note()
        {
            // Arrange
            var repoMock = new Mock<INoteRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.NoteRepository).Returns(repoMock.Object);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new NoteService(unitOfWorkMock.Object, _mapper);
            var request = new CreateNoteRequestDTO { Title = "test", Text = "t", NotificationTime = DateTime.UtcNow };

            // Act
            await service.CreateAsync(request);

            // Assert
            repoMock.Verify(r => r.CreateAsync(It.Is<Note>(n => n.Title == request.Title && n.Text == request.Text)), Times.Once);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_When_Not_Found()
        {
            // Arrange
            var repoMock = new Mock<INoteRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((Note?)null);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.NoteRepository).Returns(repoMock.Object);
            var service = new NoteService(unitOfWorkMock.Object, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.GetByIdAsync(1, false));
        }
    }
}
