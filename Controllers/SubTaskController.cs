using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    public class SubTaskController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5017/api");

        #region confi
        private readonly HttpClient _httpClient;
        public SubTaskController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        #endregion

        #region subtasklist
        //[HttpGet]
        //public async Task<IActionResult> SubTaskList()
        //{
        //    AddAuthorizationHeader();
        //    List<SubTaskModel> subtask = new List<SubTaskModel>();
        //    HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/SubTask");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string data = await response.Content.ReadAsStringAsync();
        //        subtask = JsonConvert.DeserializeObject<List<SubTaskModel>>(data);
        //    }
        //    return View(subtask);
        //}


        [HttpGet]
        [Route("SubTask/GetByMainTaskId/{Id?}")]
        public IActionResult SubTaskList(int? Id)
        {
            AddAuthorizationHeader();
            List<SubTaskModel> subtask = new List<SubTaskModel>();
			//int? userId = CommonVariable.UserID();
			if (Id != null)
            {
                HttpResponseMessage response2 = _httpClient.GetAsync($"{_httpClient.BaseAddress}/SubTask/GetByMainTaskId/{Id}").Result;
                if (response2.IsSuccessStatusCode)
                {
                    string data = response2.Content.ReadAsStringAsync().Result;
                    subtask = JsonConvert.DeserializeObject<List<SubTaskModel>>(data);
                }
                return View(subtask);
            }
            else
            {
				//HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/SubTask/User/{userId}").Result;

				HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/SubTask").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    subtask = JsonConvert.DeserializeObject<List<SubTaskModel>>(data);
                }
                return View(subtask);
            }
        }
        #endregion

        #region delete
        [HttpGet]
        public async Task<IActionResult> DeleteSubTask(int subTaskId, int mainTaskId)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/SubTask/{subTaskId}/mainTask/{mainTaskId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Subtask deleted successfully.";
                return RedirectToAction("SubTaskList");
            }
            else
            {
                TempData["Message"] = "Failed to delete subtask.";
                return RedirectToAction("SubTaskList");
            }
        }
        #endregion

        #region AddEditSubTask
        [HttpGet]
        public async Task<IActionResult> AddEditSubTask(int? id)
        {
            AddAuthorizationHeader();
            await LoadTaskList(); // Load task list asynchronously
            if (id.HasValue && id > 0)
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/SubTask/{id.Value}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    SubTaskModel model = JsonConvert.DeserializeObject<SubTaskModel>(data);
                    return View("AddEditSubTask", model); // Pass the model to the view for editing
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to fetch subtask details.";
                    return RedirectToAction("SubTaskList");
                }
            }

            // Return an empty model for adding a new task
            return View("AddEditSubTask", new SubTaskModel());
        }
        #endregion

        #region SubTaskSave
        [HttpPost]
        public async Task<IActionResult> SaveSubTask(SubTaskModel model)
        {
            if (ModelState.IsValid)
            {
                AddAuthorizationHeader();
                model.UpdatedAt = DateTime.Now;

                var jsonData = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (model.SubTaskId == 0)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/SubTask", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/SubTask/{model.SubTaskId}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "SubTask saved successfully.";
                    return RedirectToAction("SubTaskList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to save Subtask.";
                }
            }

            // Log ModelState errors if invalid
            foreach (var state in ModelState)
            {
                Console.WriteLine($"Key: {state.Key}, Error: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            return View("AddEditSubTask", model);
        }
        #endregion

        #region movetask
        [HttpPost]
        public async Task<IActionResult> CompleteSubTask(int id)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/SubTask/MoveToCompletedSubTask/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "SubTask marked as completed successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to mark the Subtask as completed.";
            }
            return RedirectToAction("SubTaskList");
        }
        #endregion

        #region maintaskdropdown
        private async Task LoadTaskList()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/ToDoList");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var tasks = JsonConvert.DeserializeObject<List<MainTaskDropDownModel>>(data);
                ViewBag.TaskList = tasks;
            }
        }
        #endregion

        #region SubTaskCount
        [HttpGet]
        public async Task<IActionResult> GetSubTaskCount(int taskId)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/SubTask/Count/{taskId}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                int count = JsonConvert.DeserializeObject<int>(data);
                return Json(new { count });
            }

            return Json(new { count = 0 });
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

