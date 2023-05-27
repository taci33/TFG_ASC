using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Curratelo.Pages.perfilesModel;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Curratelo.Pages
{
    public class ComentariosModel : PageModel
    {
        public List<Comentario> comentarios = new List<Comentario>();
        public List<infoperfil> listaperfiles = new List<infoperfil>();
        public void OnGet()
        {

            try
            {
                // ...

                // Establecer la cadena de conexión a tu base de datos SQL Server
                string connectionString = "Server=ALBERTO-PC\\SQLEXPRESS;Database=proyecto;Trusted_Connection=True;MultipleActiveResultSets=true";

                // Establecer la consulta SQL para recuperar los datos de los perfiles
                string query = "SELECT * FROM Perfiles";

                // Crear una instancia de SqlConnection con la cadena de conexión
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Abrir la conexión a la base de datos
                    connection.Open();

                    // Crear una instancia de SqlCommand con la consulta SQL y la conexión
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Ejecutar la consulta y obtener los resultados en un SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Crear un StringBuilder para construir el HTML de los perfiles
                            StringBuilder htmlBuilder = new StringBuilder();

                            // Iterar sobre los resultados y generar los perfiles
                            while (reader.Read())
                            {
                                // Obtener los valores de cada columna del resultado
                                infoperfil info = new infoperfil();
                                int id = info.id = reader.GetInt32(0);
                                string nombre = info.nombre = "" + reader.GetString(1);
                                string profesion = info.profesion = "" + reader.GetString(2);
                                string experiencia = info.experiencia = "" + reader.GetString(3);
                                string ubicacion = info.ubicacion = "" + reader.GetString(4);
                                string descripcion = info.descripcion = "" + reader.GetString(5);
                                string email = info.email = "" + reader.GetString(6);
                                string puntuacion = info.puntuacion = "" + reader.GetDecimal(7);
                                List<Comentario> comentarios = ObtenerComentariosPerfil(info.id);
                                info.Comentarios = comentarios;

                                listaperfiles.Add(info);
                                // Generar el HTML del perfil utilizando interpolación de cadenas
                                string profileHtml = $@"
                    <div class=""profile"">
                        <img src=""/img/{nombre}.png"" alt=""Profile Picture"">
                        <h2>{nombre}</h2>
                        <p>Profesión: {profesion}</p>
                        <p>Experiencia: {experiencia} años</p>
                        <p>Ubicación: {ubicacion}</p>
                        <p>Descripción: {descripcion}</p>
                        <p>Email: {email}</p>
                        <div>
                            <p class=""additional-section"">Puntuación: {puntuacion}/10</p>
                            <button type=""submit"" class=""comentarios"">Ver reseñas</button>
                        </div>
                    </div>
                ";

                                // Agregar el perfil generado al StringBuilder
                                htmlBuilder.Append(profileHtml);
                            }
                            connection.Close();
                            string profilesHtml = htmlBuilder.ToString();

                            // Asignar el HTML generado al modelo
                            ViewData["ProfilesHtml"] = profilesHtml;

                            // Devolver la vista con el layout y el modelo

                            //return Content(profilesHtml, "text/html");


                        }
                    }

                    // Cerrar la conexión a la base de datos

                }

                // Obtener el HTML completo de los perfiles


                // Usar el HTML generado en tu página web para mostrar los perfiles dinámicamente
                // (por ejemplo, asignándolo a una etiqueta div en tu página)

            }
            catch (Exception ex)
            {

            }


        }
        private List<Comentario> ObtenerComentariosPerfil(int perfilId)
        {
            

            // Establecer la cadena de conexión a tu base de datos SQL Server
            string connectionString = "Server=ALBERTO-PC\\SQLEXPRESS;Database=proyecto;Trusted_Connection=True;MultipleActiveResultSets=true";

            // Establecer la consulta SQL para recuperar los comentarios del perfil
            string query = "SELECT * FROM Comentarios WHERE TrabajadorId = @TrabajadorId";

            // Crear una instancia de SqlConnection con la cadena de conexión
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Abrir la conexión a la base de datos
                connection.Open();

                // Crear una instancia de SqlCommand con la consulta SQL y la conexión
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Establecer los parámetros de la consulta
                    command.Parameters.AddWithValue("@TrabajadorId", perfilId);

                    // Ejecutar la consulta y obtener los resultados en un SqlDataReader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Iterar sobre los resultados y crear los objetos de Comentario
                        while (reader.Read())
                        {
                            Comentario comentario = new Comentario();
                            comentario.Id = reader.GetInt32(0);
                            comentario.TrabajadorId = reader.GetInt32(1);
                            comentario.Autor = reader.GetString(2);
                            comentario.Puntuacion = reader.GetDecimal(3);
                            comentario.ComentarioTexto = reader.GetString(4);

                            comentarios.Add(comentario);
                        }
                    }
                }
            }

            return comentarios;
        }
        public IActionResult OnPostCrearComentario(int trabajadorId, string autor, decimal puntuacion, string comentario)
        {
            try
            {
                // Establecer la cadena de conexión a tu base de datos SQL Server
                string connectionString = "Server=ALBERTO-PC\\SQLEXPRESS;Database=proyecto;Trusted_Connection=True;MultipleActiveResultSets=true";

                // Establecer la consulta SQL para insertar el nuevo comentario
                string query = "INSERT INTO Comentarios (TrabajadorId, Autor, Puntuacion, Comentario) " +
                               "VALUES (@TrabajadorId, @Autor, @Puntuacion, @Comentario)";

                // Crear una instancia de SqlConnection con la cadena de conexión
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Abrir la conexión a la base de datos
                    connection.Open();

                    // Crear una instancia de SqlCommand con la consulta SQL y la conexión
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Establecer los parámetros de la consulta
                        command.Parameters.AddWithValue("@TrabajadorId", trabajadorId);
                        command.Parameters.AddWithValue("@Autor", autor);
                        command.Parameters.AddWithValue("@Puntuacion", puntuacion);
                        command.Parameters.AddWithValue("@Comentario", comentario);

                        // Ejecutar la consulta
                        command.ExecuteNonQuery();
                    }
                }

                // Redirige al usuario nuevamente a la página de perfiles
                return RedirectToPage("/Perfiles");
            }
            catch (Exception ex)
            {
                // Manejo de errores (puedes personalizarlo según tus necesidades)
                return Page();
            }
        }

        public class infoperfil
        {
            public int id;
            public string nombre;
            public string profesion;
            public string experiencia;
            public string ubicacion;
            public string descripcion;
            public string email;
            public string puntuacion;

            public List<Comentario> Comentarios { get; set; }
        }
        public class Comentario
        {
            public int Id { get; set; }
            public int TrabajadorId { get; set; }
            public string Autor { get; set; }
            public decimal Puntuacion { get; set; }
            public string ComentarioTexto { get; set; }
        }
    }
}

