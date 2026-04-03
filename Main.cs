using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Encodings.Web;

string? cnpj = null;
void writeCnpj()
{
    Console.WriteLine("Digite um CNPJ para consultar (Ex: 00000000000000): ");
    cnpj = Console.ReadLine();
}

while (cnpj == null || cnpj.Trim().Length != 14 || !ulong.TryParse(cnpj, out _))
{
    Console.WriteLine("CNPJ inválido. Certifique-se de digitar 14 dígitos numéricos.\n");
    writeCnpj();
}

try
{
    using HttpClient client = new();

    string url = $"https://api.opencnpj.org/{cnpj}";

    var resposta = await client.GetAsync(url);

    if (resposta.IsSuccessStatusCode)
    {
        var bytes = await resposta.Content.ReadAsByteArrayAsync();
        var json = System.Text.Encoding.UTF8.GetString(bytes);

        var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

        string jsonBonito = JsonSerializer.Serialize(
            JsonSerializer.Deserialize<object>(json),
            options
        );

        
       int largura = 20;

        for (int progresso = 0; progresso <= 100; progresso++)
        {
            int preenchido = (progresso * largura) / 100;

            Console.Write("\r[");

            for (int i = 0; i < largura; i++)
            {
                if (i < preenchido)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("█");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("░");
                }
            }

            Console.ResetColor();
            Console.Write($"]");

            await Task.Delay(5);
        }
        Console.ResetColor();
        Console.WriteLine("\n--- DADOS ENCONTRADOS ---");
        Console.WriteLine(jsonBonito);
        Console.WriteLine("\n--- FIM DA CONSULTA ---");
        }
        else
        {
            Console.WriteLine($"Erro na API: {resposta.StatusCode}");
        }
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao conectar na API: {ex.Message}");
}
finally
{
    Console.WriteLine("Pressione qualquer tecla para sair...");
    Console.ReadKey();
}