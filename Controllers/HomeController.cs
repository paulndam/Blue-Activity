using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Exam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exam.Controllers {
    public class HomeController : Controller {

        private MyContext _context { get; set; }

        public HomeController (MyContext context) {
            _context = context;
        }

        public User GetUser () {
            return _context.Users.FirstOrDefault (user => user.UserId == HttpContext.Session.GetInt32 ("UserId"));
        }

        [HttpGet ("")]
        public IActionResult Home () {
            return View ();
        }

        [HttpPost ("Register")]

        public IActionResult Register (User user) {

            if (ModelState.IsValid) {
                if (_context.Users.FirstOrDefault (u => u.Email == user.Email) != null) {
                    ModelState.AddModelError ("Email", "email already exist, please login or use different email");
                    return View ("Home");
                } else {
                    PasswordHasher<User> hasher = new PasswordHasher<User> ();
                    user.Password = hasher.HashPassword (user, user.Password);

                    // string hash = hasher.HashPassword (user, user.Password);
                    // user.Password = hash;

                    _context.Users.Add (user);
                    _context.SaveChanges ();
                    HttpContext.Session.SetInt32 ("UserId", user.UserId);

                    return Redirect ($"/Dashboard");
                }
            }
            return View ("Home");
        }

        [HttpPost ("LogIn")]

        public IActionResult LogIn (LoginUser userlogin) {

            //if modelstate is valid, we wann get the login faninfo and then compare it to the info in our db to see if it matches

            if (ModelState.IsValid) {
                var userx = _context.Users.FirstOrDefault (user => user.Email == userlogin.LoginEmail);

                //if there's no match the we aske them to register
                if (userx == null) {
                    ModelState.AddModelError ("Email", "invalid email or password, please register");
                    return View ("Home");
                }

                // now checking if the passowrd does match
                //and if so we will hash the password so that they login and be directed to their page but we do so by comparing their passowrd they have registered in our db and the if okay we then hash it

                var hashthepassword = new PasswordHasher<LoginUser> ();
                var result = hashthepassword.VerifyHashedPassword (userlogin, userx.Password, userlogin.LoginPassword);

                if (result == 0) {
                    ModelState.AddModelError ("Password", "invalid password");
                    return View ("Home");
                }
                //now setting session to store the login info
                HttpContext.Session.SetInt32 ("UserId", userx.UserId);
                // now redirect them to their page
                return Redirect ($"/Dashboard");

            }
            return View ("Home");
        }

        [HttpGet ("Dashboard")]

        public IActionResult Userdash (int userId) {

            User userx = _context.Users
                .Include (u => u.ActivityOfTheUser)
                .FirstOrDefault (u => u.UserId == userId);

            ViewBag.User = userx;

            User current = GetUser ();
            if (current == null) {
                return Redirect ("/Home");
            }
            ViewBag.User = current;

            List<Center> AllActivity = _context.Centers
                .Include (activiplanner => activiplanner.MyCenter)
                .Include (otheruser => otheruser.OtherUsersComingToCenter)
                .ThenInclude (u => u.User)
                .Where (activitime => activitime.Time >= DateTime.Now && DateTime.Now <= activitime.ActivityTime)
                //.OrderByDescending (a => a.ActivityTime)
                .ToList ();

            return View (AllActivity);
        }

        [HttpGet ("AddCenter")]

        public IActionResult AddCenter () {

            return View ();
        }

        [HttpPost ("AddingNewActivity")]

        public IActionResult AddingGame (Center Newcenter) {

            User current = GetUser ();
            if (current == null) {
                return Redirect ("/Home");
            }
            ViewBag.User = current;

            if (ModelState.IsValid) {

                Newcenter.UserId = current.UserId;
                _context.Centers.Add (Newcenter);
                _context.SaveChanges ();

                return RedirectToAction ("Userdash");
            } else {
                return View ("AddCenter");
            }

        }

        [HttpGet ("activity/{CenterId}")]

        public IActionResult activityInfo (int centerId) {

            User current = GetUser ();
            if (current == null) {
                return Redirect ("/Home");
            }
            ViewBag.User = current;

            Center centerx = _context.Centers
                .Include (c => c.OtherUsersComingToCenter)
                .FirstOrDefault (c => c.CenterId == centerId);

            ViewBag.Center = centerx;

            Center Ongoingactivity = _context.Centers
                .Include (c => c.OtherUsersComingToCenter)
                .ThenInclude (u => u.User)
                .Include (activiplaner => activiplaner.MyCenter)
                .FirstOrDefault (c => c.CenterId == centerId);

            return View (Ongoingactivity);
        }

        [HttpGet ("RemoveCenter/{CenterId}")]

        public IActionResult DeleteGame (int CenterId) {

            List<Center> AllCenters = _context.Centers.ToList ();
            Center removecenterx = _context.Centers.FirstOrDefault (c => c.CenterId == CenterId);

            _context.Remove (removecenterx);
            _context.SaveChanges ();

            return Redirect ("/Dashboard");
        }

        [HttpGet ("UpdateCenter/{CenterId}")]

        public IActionResult updategame (int centerId) {
            Center centerx = _context.Centers.FirstOrDefault (c => c.CenterId == centerId);
            return View ("updatecenter", centerx);
        }

        [HttpPost ("UpdatingCenter/{CenterId}")]

        public IActionResult updatingthecenter (int centerId, Center x) {

            if (ModelState.IsValid) {

                Center centerx = _context.Centers.FirstOrDefault (c => c.CenterId == centerId);

                centerx.Title = x.Title;
                centerx.Description = x.Description;
                centerx.ActivityTime = x.ActivityTime;
                centerx.Time = x.Time;
                centerx.Duration = x.Duration;
                centerx.CreatedAt = DateTime.Now;

                _context.SaveChanges ();

                return Redirect ("/Dashboard");

            } else {
                x.CenterId = centerId;
                return View ("updategame", x);
            }

        }

        [HttpGet ("AddCenterToExercise/{CenterId}")]

        public IActionResult addingcentertoexerciseorjoiningthecenter (int centerId) {

            User current = GetUser ();
            if (current == null) {
                return Redirect ("/Home");
            }

            //now linking my manay to many join whereby creating a new variable and giving it to my third table or assigning it.
            //then calling that variable and assigning our user or fan id to the current one we stored in session.
            //now we do the adding of many to many join

            OtherUser joiningotheruseractivitycenter = new OtherUser ();
            joiningotheruseractivitycenter.UserId = current.UserId;
            joiningotheruseractivitycenter.CenterId = centerId;
            _context.OtherUsers.Add (joiningotheruseractivitycenter);
            _context.SaveChanges ();
            ViewBag.Fan = current;

            return Redirect ("/Dashboard");
        }

        [HttpGet ("LeaveAddCenterToExercise/{CenterId}")]

        public IActionResult leavetheaddedcenter (int centerId) {

            //this line codes below is stating that.
            //first we call our store session function 
            //now we do our query thru our third table and look for the fanid or user and if matches with the gameid then we can now remove it

            User current = GetUser ();
            if (current == null) {
                return Redirect ("/Home");
            }

            OtherUser leavingthecenter = _context.OtherUsers
                .FirstOrDefault (o => o.UserId == current.UserId && o.CenterId == centerId);
            _context.OtherUsers.Remove (leavingthecenter);
            _context.SaveChanges ();

            return Redirect ("/Dashboard");
        }

        [HttpGet ("logout")]
        public IActionResult logout () {
            HttpContext.Session.Clear ();
            return View ("Home");
        }

    }
}