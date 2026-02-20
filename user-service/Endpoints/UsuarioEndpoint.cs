using Application.DTOs;
using Application.Services;

namespace user_service.Endpoints
{
    public static class UsuarioEndpoint
    {
        public static void MapUsuarioEndpoint(this WebApplication app)
        {
            var usuarioMapGroup = app.MapGroup("/");

            usuarioMapGroup.MapPost("/", CriaUsuario);
            usuarioMapGroup.MapPost("/login", Login);
            usuarioMapGroup.MapPatch("/reativar/id", ReactivateUser).RequireAuthorization();
        }

        public static async Task<IResult> CriaUsuario(UsuarioDTO usuarioDTO, UsuarioService usuarioService)
        {
            await usuarioService.CriaUsuario(usuarioDTO);
            return TypedResults.Created();
        }

        public static async Task<IResult> Login(LoginDTO loginDTO, UsuarioService usuarioService)
        {
            LoggedDTO loggedDTO = await usuarioService.Login(loginDTO);
            return TypedResults.Ok(loggedDTO);
        }

        public static async Task<IResult> ReactivateUser(int id, UsuarioService usuarioService)
        {
            await usuarioService.ReactivateUserById(id);
            return TypedResults.NoContent();
        }


    }
}
