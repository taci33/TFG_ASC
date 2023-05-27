using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Curratelo.Pages
{
    public class perfilesModel : PageModel
    {
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
                                int id = info.id =  reader.GetInt32(0);
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
                               
                            }
                            connection.Close();
                            


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
            List<Comentario> comentarios = new List<Comentario>();

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
        public IActionResult OnPostCrearPerfil(string nombre, string profesion, string experiencia, string ubicacion, string descripcion, string email/*, decimal puntuacion*/)
        {
            try
            {
                // Obtener los datos del formulario
                //string nombre = Request.Form["nombre"];
                //string profesion = Request.Form["profesion"];
                //string experiencia = Request.Form["experiencia"];
                //string ubicacion = Request.Form["ubicacion"];
                //string descripcion = Request.Form["descripcion"];
                //string email = Request.Form["email"];
                //decimal puntuacion = decimal.Parse(Request.Form["puntuacion"]);

                // Establecer la cadena de conexión a tu base de datos SQL Server
                string connectionString = "Server=ALBERTO-PC\\SQLEXPRESS;Database=proyecto;Trusted_Connection=True;MultipleActiveResultSets=true";

                // Establecer la consulta SQL para insertar el nuevo perfil
                string query = "INSERT INTO Perfiles (Nombre, Profesion, Experiencia, Ubicacion, Descripcion, Email, Puntuacion) " +
                               "VALUES (@Nombre, @Profesion, @Experiencia, @Ubicacion, @Descripcion, @Email, @Puntuacion)";

                // Crear una instancia de SqlConnection con la cadena de conexión
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Abrir la conexión a la base de datos
                    connection.Open();

                    // Crear una instancia de SqlCommand con la consulta SQL y la conexión
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Establecer los parámetros de la consulta
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Profesion", profesion);
                        command.Parameters.AddWithValue("@Experiencia", experiencia);
                        command.Parameters.AddWithValue("@Ubicacion", ubicacion);
                        command.Parameters.AddWithValue("@Descripcion", descripcion);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Puntuacion", 0);

                        // Ejecutar la consulta
                        command.ExecuteNonQuery();
                    }
                }

                // Redirigir al usuario nuevamente a la página de perfiles
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
