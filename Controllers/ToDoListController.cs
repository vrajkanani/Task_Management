using Microsoft.AspNetCore.Mvc;
using Task_Management.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Task_Management.Controllers
{
    [CheckAccess]
    public class ToDoListController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5017/api");
        #region confi
        private readonly HttpClient _httpClient;
        public ToDoListController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        #endregion

        #region tasklist
        [HttpGet]
        public IActionResult TaskList()
        {
            AddAuthorizationHeader();
            List<ToDoListModel> taskList = new List<ToDoListModel>();
			//int? userId = CommonVariable.UserID();
			//HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/ToDoList/User/{userId}").Result;

			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/ToDoList").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                taskList = JsonConvert.DeserializeObject<List<ToDoListModel>>(data);

                // Fetch SubTaskCount for each task
                foreach (var task in taskList)
                {
                    HttpResponseMessage subTaskResponse = _httpClient.GetAsync($"{_httpClient.BaseAddress}/SubTask/Count/{task.Id}").Result;
                    if (subTaskResponse.IsSuccessStatusCode)
                    {
                        string countData = subTaskResponse.Content.ReadAsStringAsync().Result;
                        task.SubTaskCount = JsonConvert.DeserializeObject<int>(countData);
                    }
                }
            }

            return View(taskList);
        }
        #endregion

        #region delete
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/ToDoList/{Id}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "task deleted successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to delete the task.";
            }
            return RedirectToAction("TaskList");
        }
        #endregion

        #region AddEditTask
        [HttpGet]
        public IActionResult AddEditTask(int? id)
        {
            AddAuthorizationHeader();
            if (id.HasValue && id > 0)
            {
                // Fetch task details by ID
                HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/ToDoList/{id.Value}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    ToDoListModel model = JsonConvert.DeserializeObject<ToDoListModel>(data);
                    return View("AddEditTask", model); // Pass the model to the view for editing
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to fetch task details.";
                    return RedirectToAction("TaskList");
                }
            }

            // Return an empty model for adding a new task
            return View("AddEditTask", new ToDoListModel());
        }

        #endregion

        #region TaskSave
        [HttpPost]
        public IActionResult SaveTask(ToDoListModel model)
        {
            if (ModelState.IsValid)
            {
                AddAuthorizationHeader();
                // Ensure UpdatedAt is set
                model.UpdatedAt = DateTime.Now;

                var jsonData = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (model.Id == 0)
                {
                    response = _httpClient.PostAsync($"{_httpClient.BaseAddress}/ToDoList", content).Result;
                }
                else
                {
                    response = _httpClient.PutAsync($"{_httpClient.BaseAddress}/ToDoList/{model.Id}", content).Result;
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Task saved successfully.";
                    return RedirectToAction("TaskList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to save task.";
                }
            }

            // Log ModelState errors if invalid
            foreach (var state in ModelState)
            {
                Console.WriteLine($"Key: {state.Key}, Error: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            return View("AddEditTask", model);
        }

        #endregion

        #region movetask
        [HttpPost]
        public IActionResult Complete(int id)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = _httpClient.PostAsync($"{_httpClient.BaseAddress}/ToDoList/MoveToCompleted/{id}", null).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Task marked as completed successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to mark the task as completed.";
            }
            return RedirectToAction("TaskList");
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
