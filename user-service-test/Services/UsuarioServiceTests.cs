using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Domain.Repository;
using Application.Services;
using Application.Helper;
using Application.DTOs;
using Domain.Entity;
using Application.Exceptions;
using Microsoft.Extensions.Logging;
using Infrastructure.Middleware;

namespace user_service_test.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger> _iloggerMock;
        private readonly Mock<ICorrelationIdGenerator> _correlationIdMock;
        private readonly Mock<BaseLogger<UsuarioService>> _loggerMock;

        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<BaseLogger<UsuarioService>>(_iloggerMock, _correlationIdMock);

            _service = new UsuarioService(
                _usuarioRepositoryMock.Object,
                _mapperMock.Object,
                _configurationMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CriaUsuario_DeveAdicionarUsuario_ComSucesso()
        {
            // Arrange
            var dto = new UsuarioDTO
            {
                Nome = "Teste Test",
                Email = "teste@email.com",
                Senha = "Teste123!"
            };

            var usuario = new Usuario();

            _mapperMock
                .Setup(x => x.Map<Usuario>(It.IsAny<UsuarioDTO>()))
                .Returns(usuario);

            // Act
            await _service.CriaUsuario(dto);

            // Assert
            _usuarioRepositoryMock.Verify(
                x => x.Add(It.IsAny<Usuario>()),
                Times.Once);
        }

        [Fact]
        public async Task ReactivateUserById_DeveReativarUsuario_QuandoExistir()
        {
            // Arrange
            var usuario = new Usuario { Id = 1, IsActive = false };

            _usuarioRepositoryMock
                .Setup(x => x.GetById(1))
                .ReturnsAsync(usuario);

            // Act
            await _service.ReactivateUserById(1);

            // Assert
            usuario.IsActive.Should().BeTrue();

            _usuarioRepositoryMock.Verify(
                x => x.Update(usuario),
                Times.Once);
        }

        [Fact]
        public async Task ReactivateUserById_DeveLancarNotFoundException_QuandoNaoExistir()
        {
            // Arrange
            _usuarioRepositoryMock
                .Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync((Usuario)null);

            // Act
            Func<Task> act = async () => await _service.ReactivateUserById(1);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Login_DeveLancarNotFoundException_QuandoUsuarioNaoEncontrado()
        {
            // Arrange
            _usuarioRepositoryMock
                .Setup(x => x.GetUsuarioByEmailESenha(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync((Usuario)null);

            var loginDTO = new LoginDTO
            {
                Email = "errado@email.com",
                Senha = "123"
            };

            // Act
            Func<Task> act = async () => await _service.Login(loginDTO);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

    }
}
