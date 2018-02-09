
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using wcLoto.Models;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Net.Http;
using System.Net.Http.Headers;
using helpers.Redis;
using Newtonsoft.Json;
using helpers.IDbManager;



namespace wcLoto.Controllers
{
    public class HomeController : Controller
    {
        IDbManager dbManager;
        
        IDatabase cacheDb;

        // home page
        public HomeController(IRedisConnectionFactory caching)
        {
            dbManager = new genericDbManager();
            cacheDb = caching.Connection().GetDatabase();
            cacheDb = caching.Connection().GetDatabase();
            cacheDb.StringSet("getUser","http://localhost:5001/api/Login/getUser");
            cacheDb.StringSet("createUser","http://localhost:5001/api/Login/CreateUser");
            
        }

        public IActionResult Index()
        {
            return View();
        }

        // gets input from register form
        [HttpPost]
        public async Task<IActionResult> register(String regusername, String regpassword)
        {
            // ViewData.Clear();
            // Console.WriteLine("The regi func got : " + regusername + regpassword);
            string userId = regusername + regpassword;
            
            // is user exists in cache 
            if (cacheDb.KeyExists(userId))
            {
                // ViewBag["msg"] = "user already exists";
                return RedirectToAction("Index");
            }
            else 
            {
                Console.WriteLine("home controller : user isn't in cache");
                // create new user
                User u = new User();
                u._id = userId;
                u.name = regusername;
                u.password = regpassword;
                // add user to cache as json
                string jsonUser = JsonConvert.SerializeObject(u);
                if(cacheDb.StringSet(userId, jsonUser))
                {    
                    // send requset to db to create new user
                    dynamic d = await dbManager.create(cacheDb.StringGet("createUser"),jsonUser); 
                    if (d != null)
                    {
                        using (HttpResponseMessage response = d)
                        {
                            if(response.IsSuccessStatusCode)
                            {
                                // TempData["user"] = u;
                                return RedirectToAction("Profile","User",u);              
                            }
                        }
                    }
                }
                // if user not added to cache
                else 
                {
                    // ViewBag["msg"] = "error creating user";
                    return RedirectToAction("Index");
                }
            }
            // if user exists in cache
            // ViewBag["msg"] = "error creating user";
            return RedirectToAction("Index");
        }


        // deserialize jsonObject to User Object
        
        // gets input from login form
        [HttpPost]
        public async Task<IActionResult> login(String logusername, String logpassword)
        {
            // ViewData.Clear();
            var userId = logusername+logpassword;
            User user = null;

            // if exists in db
            if (cacheDb.KeyExists(userId))
            {
                Console.WriteLine("found in cache");
                user = (User) JsonConvert.DeserializeObject<User>(cacheDb.StringGet(userId));
                return RedirectToAction("Profile", "User", user);
            }
            else Console.WriteLine("not found in cache");
            // check db
            using(dynamic d = await dbManager.get(cacheDb.StringGet("getUser"), userId))
            {
                if (d != null)
                {
                    using (HttpResponseMessage response = (HttpResponseMessage) d)
                    {
                        using (HttpContent resContent = response.Content)
                        {
                            var jsonAns = await response.Content.ReadAsStringAsync();
                            Console.WriteLine("The object returned was : " + jsonAns);
                            // if contains id it's user else error
                            if (jsonAns.Contains("_id"))
                            {
                                cacheDb.StringSet(userId, jsonAns);
                                Console.WriteLine("The object was added to cache");
                                user = (User) JsonConvert.DeserializeObject<User>(jsonAns);
                                if (user == null)
                                {
                                    return RedirectToAction("Index");
                                }
                                else {
                                        return RedirectToAction("Profile", "User", user);
                                    }
                            } 
                            else
                            {
                                // ViewBag.Message = "can't find user in db";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            // ViewData["Message"] = "User Registration Page";

            return View();
        }

        public IActionResult Contact()
        {
            // ViewData["Message"] = "User Login Page";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
