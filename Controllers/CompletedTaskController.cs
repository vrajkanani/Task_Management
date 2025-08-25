using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    public class CompletedTaskController : Controller
    {
        #region confi
        Uri baseAddress = new Uri("http://localhost:5017/api");
        private readonly HttpClient _httpClient;
        public CompletedTaskController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        #endregion

        #region getall
        [HttpGet]
        public IActionResult CompletedTaskList()
        {
            AddAuthorizationHeader();
            List<CompletedTaskModel> completedtask = new List<CompletedTaskModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/CompletedTask").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                completedtask = JsonConvert.DeserializeObject<List<CompletedTaskModel>>(data);
            }
            return View(completedtask);
        }
        #endregion

        #region clear
        [HttpPost]
        public IActionResult ClearAll()
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/CompletedTask/ClearAll").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "All Completed Tasks cleared successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to clear the Completed Tasks.";
            }
            return RedirectToAction("CompletedTaskList");
        }
        #endregion

        #region AddAuthorizationHeader
        private void AddAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        #endregion
    }
}
