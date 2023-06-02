using Newtonsoft.Json;

namespace MVCProject.Utility; 

public static class ApiCallManager {
    
    private static readonly HttpClient Client = new HttpClient();
    private static bool _initialized = false;
    private const string API_URL = "http://localhost:5281/api";
    
    public static async Task<HttpResponseMessage> Get(string url, string key, string userId) {
        if (!_initialized) {
            Client.DefaultRequestHeaders.Add("X-API-Key", key);
            Client.DefaultRequestHeaders.Add("X-API-User", userId);
            _initialized = true;
        }
        
        Console.WriteLine("GET: " + API_URL + url);
        
        var response = await Client.GetAsync(API_URL + url);
        return response;
    }
    
    public static async Task<HttpResponseMessage> Post(string url, string key, string userId, object? requestData = null) {
        if (!_initialized) {
            Client.DefaultRequestHeaders.Add("X-API-Key", key);
            Client.DefaultRequestHeaders.Add("X-API-User", userId);
            _initialized = true;
        }
        
        var response = await Client.PostAsJsonAsync(API_URL + url, requestData);
        return response;
    }
    
    public static async Task<HttpResponseMessage> Delete(string url, string key, string userId) {
        if (!_initialized) {
            Client.DefaultRequestHeaders.Add("X-API-Key", key);
            Client.DefaultRequestHeaders.Add("X-API-User", userId);
            _initialized = true;
        }
        
        var response = await Client.DeleteAsync(API_URL + url);
        return response;
    }
    
    public static async Task<HttpResponseMessage> GetHeaderless(string url) {
        var response = await Client.GetAsync(API_URL + url);
        return response;
    }
    
    public static async Task<HttpResponseMessage> PostHeaderless(string url, object? requestData = null) {
        var response = await Client.PostAsJsonAsync(API_URL + url, requestData);
        return response;
    }
}