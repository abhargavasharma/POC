using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCTestAPP.Repository;
using MVCTestAPP.Models;

namespace MVCTestAPP.Controllers
{
    public class BhargavController : Controller
    {
        private IUserRepository userRepository;
        public BhargavController()
        {
            this.userRepository = new UserRepository(new MVCEntities());
        } 

        // GET: Bhargav
        public ActionResult Index()
        {
            var userList = from user in userRepository.GetUsers() select user;
            var users = new List<MVCTestAPP.Models.User>();
            if (userList.Any())
            {
                foreach (var user in userList)
                {
                    users.Add(new MVCTestAPP.Models.User()
                    {
                        UserId = user.UserId,
                        Address = user.Address,
                        Company = user.Company,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Designation = user.Designation,
                        EMail = user.EMail,
                        PhoneNo = user.PhoneNo
                    });
                }
            }
            return View(users);
        }

        [HttpGet]
         public ActionResult Test()
        {
            return View();
        }
    }
}