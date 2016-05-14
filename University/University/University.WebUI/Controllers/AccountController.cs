using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using DotNetOpenAuth.AspNet;
//using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using University.WebUI.Filters;
using University.WebUI.Models;
using University.Domain.Abstract;

namespace University.WebUI.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        IDataBaseRepository _data;


        public AccountController(IDataBaseRepository data)
        {
            _data = data;
        }

        //public ViewResult Registration(UserRegistrationModel user)
        //{
        //    if (user == null)
        //    {
        //        return View();
        //    }

        //    return View(user);
        //}

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //public ActionResult Authorization(string returnUrl)
        //{
        //    //So that the user can be referred back to where they were when they click logon
        //    if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
        //        returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

        //    if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
        //    {
        //        TempData["ReturnUrl"] = returnUrl;
        //    }

        //    return View();
        //}


        //[HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            //string ReturnUrl = TempData.Peek("ReturnUrl").ToString();
            //string decodedUrl = "";
            //if (!string.IsNullOrEmpty(ReturnUrl))
            //    decodedUrl = Server.UrlDecode(ReturnUrl);
            
            if (loginModel.UserName == null) { return View(); }
            else if (Membership.ValidateUser(loginModel.UserName, loginModel.Password))
            {
                FormsAuthentication.SetAuthCookie(loginModel.UserName, false);
            }
            else
            {
                TempData["message"] = "Помилка авторизації. Введено некоректні облікові дані.";
                return View();
            }

            //if (Url.IsLocalUrl(decodedUrl))
            //{
            //    return Redirect(decodedUrl);
            //}

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");

        }


        [HttpPost]
        public ActionResult CreateUser(RegisterModel user)
        {
            try
            {
                MembershipUser newUser = Membership.CreateUser(user.UserName, user.Password, user.Email);//Membership.CreateUser(user.Login, user.Password, user.Email, user.Question, user.AnswerQuestion, true, out status);

                //User ecoUser = new User()
                //{
                //    FirstName = user.FirstName,
                //    LastName = user.LastName,
                //    AppUserID = (Guid)newUser.ProviderUserKey,
                //    ActivationCode = Guid.NewGuid()
                //};

                //_data.Users.Add(ecoUser);
                //_data.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            catch (MembershipCreateUserException e)
            {
                TempData["message"] = GetErrorMessage(e.StatusCode);
            }
            catch (HttpException e)
            {
                TempData["message"] = e.Message;
            }
            return RedirectToAction("Registration", new { user });
        }

        public string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Ім'я користувача вже існує. Будь ласка, введіть інше ім'я користувача.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ім'я користувача для цього адресу електронної пошти вже існує. Будь ласка, введіть іншу адресу електронної пошти.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Пароль не є недійсним. Будь ласка, введіть припустиме значення пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Адреса електронної пошти є недійсною. Будь ласка, перевірте значення і повторіть спробу.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Пароль вилучення відповідь є недійсною. Будь ласка, перевірте значення і повторіть спробу.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Пароль вилучення питання є недійсною. Будь ласка, перевірте значення і повторіть спробу.";

                case MembershipCreateStatus.ProviderError:
                    return "Постачальник перевірки автентичності повертає повідомлення про помилку. Будь ласка, перевірте введені дані і спробуйте ще раз. Якщо проблема не усувається, будь ласка, зверніться до системного адміністратора.";

                case MembershipCreateStatus.UserRejected:
                    return "Запит на створення користувача був відмінений. Будь ласка, перевірте введені дані і спробуйте ще раз. Якщо проблема не усувається, будь ласка, зверніться до системного адміністратора.";

                default:
                    return "Виникла невідома помилка. Будь ласка, перевірте введенні дані і спробуйте ще раз. Якщо проблема не усувається, будь ласка, зверніться до системного адміністратора.";
            }
        }

    }
}
