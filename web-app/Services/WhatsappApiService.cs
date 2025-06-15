using System;
using System.Text;
using System.Text.Json;

namespace web_app.Services
{
    public class WhatsappApiService
    {
        private readonly HttpClient _httpClient;

        public WhatsappApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private class QrCodeResponse
        {
            public string? qr { get; set; }
        }

        public async Task<string?> GetQRAccessBase64ImageAsync()
        {
            try
            {
                string apiUrl = "http://localhost:3000/qr";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    // Deserialize JSON: { "qr": "base64..." }
                    var qrResponse = JsonSerializer.Deserialize<QrCodeResponse>(json);
                    return qrResponse?.qr;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SendMessageAsync(string number, string message)
        {
            try
            {
                var payload = new
                {
                    number = number,
                    message = message
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:3000/send", content);

                return response.IsSuccessStatusCode;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
