using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Task_Management.Models;

namespace TODOAPI.Controllers
{
    public class CompleteSubTaskController : Controller
    {
        #region confi
        private readonly HttpClient _httpClient;
        private readonly Uri _baseAddress = new Uri("http://localhost:5017/api");

        public CompleteSubTaskController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseAddress;
        }
        #endregion

        #region Getall
        [HttpGet]
        public IActionResult CompleteSubTaskList()
        {
            AddAuthorizationHeader();
            List<CompleteSubTaskModel> completeSubTasks = new List<CompleteSubTaskModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/CompleteSubTask").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                completeSubTasks = JsonConvert.DeserializeObject<List<CompleteSubTaskModel>>(data);
            }

            return View(completeSubTasks);
        }
        #endregion

        #region clear
        [HttpPost]
        public IActionResult ClearAll()
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/CompleteSubTask/ClearAll").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "All complete subtasks cleared successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to clear the complete subtasks.";
            }

            return RedirectToAction("CompleteSubTaskList");
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
