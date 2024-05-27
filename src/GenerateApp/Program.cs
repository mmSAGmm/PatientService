using GenerateApp;
using PatientService.RequestModels;
using System.Text;
using System.Text.Json;

string BuildJson()
{
    var requestModel = new CreatePatientRequestModel();
    requestModel.BirthDate = DateTime.UtcNow.Add(TimeSpan.FromSeconds(SafeRandom.Random.Next(100000) * -1));
    requestModel.Family = Guid.NewGuid().ToString();
    requestModel.Use = Guid.NewGuid().ToString();
    requestModel.Active = true;
    requestModel.Given = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
    return JsonSerializer.Serialize(requestModel);
}

var chunks = Enumerable.Range(0, 100).Chunk(10);
var client = new HttpClient();
string url = args.Any() ? args[0] : "http://localhost:8080/api/v1/patient";

foreach (var chunk in chunks)
{
    var tasks = chunk
        .Select(x => new StringContent(BuildJson(), Encoding.UTF8, "application/json"))
        .Select(x => client.PostAsync(url, x));
    try
    {
        var results = await Task.WhenAll(tasks);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed with ex: {ex.Message}");
    }
}

Console.WriteLine("Done!!!");
Console.ReadLine();

