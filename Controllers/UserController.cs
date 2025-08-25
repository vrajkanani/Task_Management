using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    public class UserController : Controller
    {
        #region Uri
        private readonly Uri baseAddress = new Uri("http://localhost:5017/api");
        private readonly HttpClient _Client;

        public UserController()
        {
            _Client = new HttpClient { BaseAddress = baseAddress };
        }
        #endregion

        #region UserList
        [HttpGet]
        [Route("/User")]
        public IActionResult UserList()
        {
            AddAuthorizationHeader();
            List<UserModel> userList = new List<UserModel>();
            HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/User").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;

                userList = JsonConvert.DeserializeObject<List<UserModel>>(data);
            }
            return View("UserList", userList);
        }
        #endregion

        #region UserAddEdit
        [HttpGet]
        [Route("/User/UserAddEdit")]
        public IActionResult UserAddEdit(int? UserID)
        {
            AddAuthorizationHeader();
            UserModel model = new UserModel();

            if (UserID.HasValue && UserID > 0)
            {
                HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/User/{UserID}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    model = JsonConvert.DeserializeObject<UserModel>(data);
                }
            }

            return View("UserAddEdit", model);
        }
        #endregion

        #region UserSave
        [HttpPost]
        [Route("/User/UserSave")]
        public IActionResult UserSave(UserModel model)
        {
            if (ModelState.IsValid)
            {
                AddAuthorizationHeader();
                HttpResponseMessage response;
                if (model.UserID == null)
                {
                    // Add operation
                    response = _Client.PostAsJsonAsync($"{_Client.BaseAddress}/User", model).Result;
                }
                else
                {
                    // Edit operation
                    response = _Client.PutAsJsonAsync($"{_Client.BaseAddress}/User/{model.UserID}", model).Result;
                }

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("UserList");
                }
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["ErrorMassage"] = $"Failed to save the User: {errorMessage}";

            }

            // Return to the form if validation or API call fails
            return View("UserAddEdit", model);
        }
        #endregion

        #region UserDelete
        [HttpPost]
        public IActionResult UserDelete(int UserID)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = _Client.DeleteAsync($"{_Client.BaseAddress}/User/{UserID}").Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("UserList");
            }
            else
            {
                TempData["ErrorMassage"] = "Error deleting the User.";
                return RedirectToAction("UserList");
            }
        }
        #endregion

        #region UserLogin
        [HttpGet]
        [Route("/User/UserLogin")]
        public IActionResult UserLogin()
        {
            return View("UserLogin");
        }

        [HttpPost]
        [Route("/User/UserLogin")]
        public IActionResult UserLogin(UserLoginModel loginModel, bool remember)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _Client.PostAsJsonAsync($"{_Client.BaseAddress}/User/Login", loginModel).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    var responseData = JsonConvert.DeserializeObject<UserLoginResponseModel>(data);

                    if (responseData != null)
                    {
                        var token = responseData.Token;
                        var user = responseData.User;

                        // Store token in session
                        HttpContext.Session.SetString("JWTToken", token);
                        HttpContext.Session.SetString("UserID", user.UserID.ToString());
                        HttpContext.Session.SetString("UserName", user.UserName);
                        HttpContext.Session.SetString("Role", user.Role.ToString());

                        // Store token in cookie if "Remember Me" is checked
                        if (remember)
                        {
                            var cookieOptions = new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = DateTime.UtcNow.AddDays(30) // Set expiration for 30 days
                            };
                            Response.Cookies.Append("JWTToken", token, cookieOptions);
                            Response.Cookies.Append("UserID", user.UserID.ToString(), cookieOptions);
                            Response.Cookies.Append("UserName", user.UserName, cookieOptions);
                            Response.Cookies.Append("Role", user.Role.ToString(), cookieOptions);
                        }

                        //return user.Role ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Home");
                        if (user.Role)
                        {
                            return RedirectToAction("UserList", "User");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                }

                TempData["ErrorMessage"] = "Invalid login credentials.";
            }

            return View("UserLogin", loginModel);
        }
        #endregion

        #region UserRegister
        [HttpGet]
        [Route("/User/UserRegister")]
        public IActionResult UserRegister()
        {
            AddAuthorizationHeader();
            return View("UserRegister");
        }

        [HttpPost]
        [Route("/User/UserRegister")]
        public IActionResult UserRegister(UserRegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                AddAuthorizationHeader();

                HttpResponseMessage response = _Client.PostAsJsonAsync($"{_Client.BaseAddress}/User/register", registerModel).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMassage"] = "Registration successful! Please login.";
                    return RedirectToAction("UserLogin");
                }
                TempData["ErrorMassage"] = "Failed to register the user. Please try again.";
            }

            return View("UserRegister", registerModel);
        }
        #endregion

        #region UserLogout
        [Route("/User/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("UserLogin", "User");
        }
        #endregion

        #region AddAuthorizationHeader
        private void AddAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        #endregion
    }
}
