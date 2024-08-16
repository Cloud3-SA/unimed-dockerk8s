using Bogus;
using MySqlConnector;
using Fluid;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseStaticFiles();

Dictionary<string, string> db = new Dictionary<string, string>();
foreach (string e in new string [] {"DB_HOST", "DB_USER", "DB_PASS", "DB_DATABASE"}) {
  db.Add(e, Environment.GetEnvironmentVariable(e));
  if (db[e] == null)
    { throw new Exception($"A variável de ambiente {e} não pode ser nula!"); }
}

app.MapGet("/", (HttpContext ctx) => {

  var connection = new MySqlConnection($"Server={db["DB_HOST"]};User={db["DB_USER"]};Password={db["DB_PASS"]};Database={db["DB_DATABASE"]}");
  //var connection = new MySqlConnection(builder.Configuration.GetConnectionString("Default");
  connection.Open();

  var cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS usuarios (id INT AUTO_INCREMENT PRIMARY KEY, nome VARCHAR(255), email VARCHAR(255))", connection);
  cmd.ExecuteNonQuery();

  var person = new Bogus.Person();
  var insert = new MySqlCommand("INSERT INTO usuarios (nome, email) VALUES (@n, @e)", connection);
  insert.Parameters.AddWithValue("n", $"{person.FirstName} {person.LastName}");
  insert.Parameters.AddWithValue("e", person.Email);
  insert.ExecuteNonQuery();

  var select = new MySqlCommand("SELECT * FROM usuarios", connection);
  var reader = select.ExecuteReader();
  List<Dictionary<string,string>> usuarios = new List<Dictionary<string,string>>();
  while (reader.Read()) {
      Dictionary<string, string> usuario = new Dictionary<string, string>();
      usuario["id"] = reader.GetInt32(0).ToString();
      usuario["nome"] = reader.GetString(1);
      usuario["email"] = reader.GetString(2);
      usuarios.Add(usuario);
  }
  connection.Close();

  string html = File.ReadAllText("Views/index.html");

  var parser = new FluidParser();
  var template = parser.Parse(html);
  var context = new TemplateContext(new { usuarios = usuarios });
  var result = template.Render(context);
  ctx.Response.Headers.ContentType = new Microsoft.Extensions.Primitives.StringValues("text/html; charset=UTF-8");
  return result;
});

app.Run();
