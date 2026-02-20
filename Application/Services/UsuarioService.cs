using Application.DTOs;
using Application.Exceptions;
using Application.Helper;
using AutoMapper;
using Domain.Entity;
using Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class UsuarioService
    {
        IUsuarioRepository _usuarioRepository;
        IMapper _mapper;
        BaseLogger<UsuarioService> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioService(IUsuarioRepository usuarioRepository, IMapper mapper, IConfiguration configuration, BaseLogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task CriaUsuario(UsuarioDTO usuarioDTO)
        {
            _logger.LogInformation($"Adicionando novo Usuario.");

            ValidateUser(usuarioDTO);
            usuarioDTO.Senha = GenerateGuidFromSenha(usuarioDTO.Senha);
            var userToAdd = _mapper.Map<Usuario>(usuarioDTO);
            await _usuarioRepository.Add(userToAdd);

            _logger.LogInformation($"Novo Usuario adicionado.");
        }

        public async Task ReactivateUserById(int id)
        {
            _logger.LogInformation($"Reativando user com Id:{id}");
            Usuario usuario = await _usuarioRepository.GetById(id);

            if (usuario == null)
                throw new NotFoundException("Não existe user com Id: " + id);

            usuario.IsActive = true;
            _logger.LogInformation($"Usuario com Id:{id} reativado");
            await _usuarioRepository.Update(usuario);
        }

        public async Task<LoggedDTO> Login(LoginDTO loginDTO)
        {
            _logger.LogInformation($"Efetuando login para {loginDTO.Email}.");
            loginDTO.Senha = GenerateGuidFromSenha(loginDTO.Senha);
            Usuario usuario = await _usuarioRepository.GetUsuarioByEmailESenha(loginDTO.Email, loginDTO.Senha);
            if (usuario == null)
                throw new NotFoundException("Email ou senha inválidos");
            if (!usuario.IsActive)
                throw new BadDataException("Conta inativa.");

            LoggedDTO loggedDTO = new LoggedDTO() { Email = loginDTO.Email, Token = GenerateJwtToken(usuario) };

            _logger.LogInformation($"Login efetuado para {loginDTO.Email}.");
            return loggedDTO;
        }

        private void ValidateUser(UsuarioDTO usuario)
        {
            string errorMessage = "";
            errorMessage = ValidationHelper.ValidaEmpties<UsuarioDTO>(usuario, errorMessage);

            if (!IsEmailValid(usuario.Email))
                errorMessage += "Email inválido. ";

            if (!IsSenhaValid(usuario.Senha))
                errorMessage += "Senha deve conter no mínimo de 8 caracteres com números, letras e caracteres especiais. ";

            if (!string.IsNullOrEmpty(errorMessage))
                throw new BadDataException(errorMessage.Trim());
        }



        private bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private bool IsSenhaValid(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return false;

            // Pelo menos 8 caracteres, 1 letra, 1 número e 1 caractere especial
            var pattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&^+=_\-])[A-Za-z\d@$!%*#?&^+=_\-]{8,}$";
            return Regex.IsMatch(senha, pattern);
        }



        private string GenerateGuidFromSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));

            var guidBytes = new byte[16];
            Array.Copy(hash, guidBytes, 16);

            var guid = new Guid(guidBytes);
            return guid.ToString();
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"])),
                issuer: _configuration["Jwt:Issuer"],
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
