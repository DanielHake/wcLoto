using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using wcLoto.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using StackExchange.Redis;
using Newtonsoft.Json;
using helpers.Redis;
using helpers.IDbManager;

namespace wcLoto.Controllers
{
    public class UserController : Controller
    {
        IDatabase userCachingDB;
        IDbManager DbManager;
        IDatabase routingCacheDB;
        public UserController(IRedisConnectionFactory caching)
        {
            DbManager = new genericDbManager();
            userCachingDB = caching.Connection().GetDatabase();
            routingCacheDB = caching.Connection().GetDatabase();
            // user
            routingCacheDB.StringSet("getUser","http://localhost:5001/api/login/getUser");
            routingCacheDB.StringSet("updateUser","http://localhost:5001/api/login/updateUser");
            routingCacheDB.StringSet("createUser","http://localhost:5001/api/login/createUser");
            routingCacheDB.StringSet("deleteUser","http://localhost:5001/api/login/deleteUser");
            // bet
            routingCacheDB.StringSet("getBet","http://localhost:5001/api/bet/getBet");
            routingCacheDB.StringSet("createBet", "http://localhost:5001/api/bet/createBet");
            routingCacheDB.StringSet("updateBet", "http://localhost:5001/api/bet/updateBet");
            routingCacheDB.StringSet("deleteBet", "http://localhost:5001/api/bet/deleteBet");
            // team
            routingCacheDB.StringSet("getTeam","http://localhost:5001/api/team/getTeam");
            routingCacheDB.StringSet("createTeam", "http://localhost:5001/api/team/createTeam");
            routingCacheDB.StringSet("updateTeam", "http://localhost:5001/api/team/updateTeam");
            routingCacheDB.StringSet("deleteTeam", "http://localhost:5001/api/team/deleteTeam");
            // game
            routingCacheDB.StringSet("getGame","http://localhost:5001/api/game/getGame");
            routingCacheDB.StringSet("createGame", "http://localhost:5001/api/game/createGame");
            routingCacheDB.StringSet("updateGame", "http://localhost:5001/api/game/updateGame");
            routingCacheDB.StringSet("deleteGame", "http://localhost:5001/api/game/deleteGame");
            // player
            routingCacheDB.StringSet("getPlayer","http://localhost:5001/api/player/getPlayer");
            routingCacheDB.StringSet("createPlayer", "http://localhost:5001/api/player/createPlayer");
            routingCacheDB.StringSet("updatePlayer", "http://localhost:5001/api/player/updatePlayer");
            routingCacheDB.StringSet("deletePlayer", "http://localhost:5001/api/player/deletePlayer");
            // table
            routingCacheDB.StringSet("getTable","http://localhost:5001/api/table/getTable");
            routingCacheDB.StringSet("createTable", "http://localhost:5001/api/table/createTable");
            routingCacheDB.StringSet("updateTable", "http://localhost:5001/api/table/updateTable");
            routingCacheDB.StringSet("deleteTable", "http://localhost:5001/api/table/deleteTable");
            
            
            // redis example
            // string value = "abcdefg";
            // db.StringSet("mykey", value);
            // string value1 = db.StringGet("mykey");
            // Console.WriteLine(value1); // writes: "abcdefg"

        }
        
// ---------------------------------------Database manager func's ------------------------------------
        
        // get object from db
        public async Task<dynamic> getFromDb(string type,string route,string id)
        {
            HttpResponseMessage response;
            response = await DbManager.get(route,id);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("got obj from db");
                HttpContent content = response.Content;
                string json = await content.ReadAsStringAsync();
                if(json.Contains("_id"))
                {
                    switch(type)
                    {
                        case "user": 
                            User user = (User) JsonConvert.DeserializeObject<User>(json);    
                            return user;   
                        case "bet": 
                            Bet bet = (Bet) JsonConvert.DeserializeObject<Bet>(json);       
                            return bet;
                        case "team": 
                            Team team = (Team) JsonConvert.DeserializeObject<Team>(json);       
                            return team;
                        case "table": 
                            Table table = (Table) JsonConvert.DeserializeObject<Table>(json);       
                            return table;
                        case "player": 
                            Player player = (Player) JsonConvert.DeserializeObject<Player>(json);       
                            return player;
                        case "game": 
                            Game game = (Game) JsonConvert.DeserializeObject<Game>(json);       
                            return game;
                        default:
                            break;
                    }
                }
            }
            Console.WriteLine("db not got obj");
            return null;
        }

        // create new doc in db
        public async Task<HttpResponseMessage> createOnDb(string route, string jsonObj)
        {
            HttpResponseMessage response;
            response = await DbManager.create(route,jsonObj);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("db create obj");
                return response;
            }
            else Console.WriteLine("db not created obj");
            return response;
        }

        // update doc in db
        public async Task<int> updateDb(string route,string jsonObj)
        {
            HttpResponseMessage response;
            response = await DbManager.update(route,jsonObj);
            if (response.IsSuccessStatusCode)
            {
                HttpContent content = response.Content;
                string json = await content.ReadAsStringAsync();
                if (json.Contains("_id"))
                {
                    Console.WriteLine(json);
                    Console.WriteLine("updated db");
                    return 1;
                }
                else Console.WriteLine(json);
            }
            else Console.WriteLine("db not updated");
            return -1;
        }
        
        // delete an object from db
        public async Task<int> deleteFromDb(string route,string jsonObj)
        {
            HttpResponseMessage response;
            response = await DbManager.delete(route,jsonObj);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("db delete obj");
                return 1;
            }
            else Console.WriteLine("db not delete obj");
            return -1;
        }

// -------------------------------------------------------------------------------------------------------

        // example of getting some bets - returning one them.
        public async Task<dynamic> getExampleBet()
        {
            Bet golden = null;
            Bet wcWinner = null;
            Bet game = null;
            dynamic d = await getFromDb("bet", routingCacheDB.StringGet("getBet"),"golden");
            if (d != null)
            {
                golden = (Bet) d;
            }
            else Console.WriteLine("failed to get bet");
            dynamic d1 = await getFromDb("bet", routingCacheDB.StringGet("getBet"),"wcWinner");
            if (d != null)
            {
                wcWinner = (Bet) d1;
            }
            else Console.WriteLine("failed to get bet");
            dynamic d2 = await getFromDb("bet", routingCacheDB.StringGet("getBet"),"b_russia_uruguay");
            if (d != null)
            {
                game = (Bet) d2;
            }
            else Console.WriteLine("failed to get bet");
            return golden;
        }


        // update user db
        public async Task<User> updateUser(User user) {
            string jsonObj = JsonConvert.SerializeObject(user);
            dynamic d3 = await updateDb(routingCacheDB.StringGet("updateUser"),jsonObj); 
            if (d3 < 0)
            {
                Console.WriteLine("update user failed");
                return user;
            }
            Console.WriteLine("update user worked");
            
            return user; 
        } 
        // update bet db
        public async Task<Bet> updateBet(Bet bet) {
            string jsonObj = JsonConvert.SerializeObject(bet);
            dynamic d3 = await updateDb(routingCacheDB.StringGet("updateBet"),jsonObj);
            Bet b = null;
            if (d3 < 0)
            {
                Console.WriteLine("update bet failed");
                return bet;
            }
            Console.WriteLine("update bet worked");
            return b; 
        } 

        // update table in db
        public async Task<Table> updateTable(Table table) {
            string jsonObj = JsonConvert.SerializeObject(table);
            dynamic d3 = await updateDb(routingCacheDB.StringGet("updateTable"),jsonObj);
            if (d3 < 0)
            {
                Console.WriteLine("update bet failed");
                return table;
            }
            Console.WriteLine("update bet worked");
            return null; 
        } 

        public async Task<IActionResult> updateBetNoteFromProfile(string userId, string betId, string note)
        {
            User user = null;
            Bet bet = null;
            if (userCachingDB.KeyExists(userId))
            {
                Console.WriteLine("got user from cache");
                Console.WriteLine(userCachingDB.StringGet(userId));
                user = (User) JsonConvert.DeserializeObject<User>(userCachingDB.StringGet(userId));
            }
            else 
            {
                dynamic dUser = await getFromDb("user",userCachingDB.StringGet("getUser"),userId);
                if (dUser != null)
                {
                    Console.WriteLine("got user from db");
                    user = dUser;
                }
                else 
                    Console.WriteLine("couldnt get user from db to update its bet");         
            }
            bet = user.bets.Find(b=>b._id==betId);
            if (bet != null)
            {
                int index = user.bets.IndexOf(bet);
                bet.note = note;
                user.bets[index].note = note;
            }
            

            await updateUser(user);
            await updateBet(bet);

            return View("Profile", user);
            
        }
        public async Task<IActionResult> updateBetFromProfile(string userId,string betId,string teamAScore,string teamBScore)
        {
            User user = null;
            Bet bet = null;
            if (userCachingDB.KeyExists(userId))
            {
                Console.WriteLine("got user from cache");
                Console.WriteLine(userCachingDB.StringGet(userId));
                user = (User) JsonConvert.DeserializeObject<User>(userCachingDB.StringGet(userId));
            }
            else 
            {
                dynamic dUser = await getFromDb("user",userCachingDB.StringGet("getUser"),userId);
                if (dUser != null)
                {
                    Console.WriteLine("got user from db");
                    user = dUser;
                }
                else 
                    Console.WriteLine("couldnt get user from db to update its bet");         
            }
        
            bet = user.bets.Find(b=>b._id==betId);
            if (bet != null)
            {
                int index = user.bets.IndexOf(bet);
                int scoreA = Int32.Parse(teamAScore.ToString());
                int scoreB = Int32.Parse(teamBScore.ToString());
                bet.scoreTeamA = scoreA;
                bet.scoreTeamB = scoreB;
                user.bets[index].scoreTeamA = scoreA;
                user.bets[index].scoreTeamB = scoreB;
            }
            

            await updateUser(user);
            await updateBet(bet);

            return View("Profile", user);
        }
        public int updateUserCache(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            if(userCachingDB.StringSet(user._id,json))
            return 1;
            else return -1;
        }
        // User profile page
        public async Task<IActionResult> Profile(User u)
        {

            User user = u;
            Bet golden = null;
            Bet wcWinner = null;
            Bet game = null;
            dynamic d = await getFromDb("bet", routingCacheDB.StringGet("getBet"),"golden");
            if (d != null)
            {
                golden = (Bet) d;
                user.bets.Add(golden);
            }
            dynamic d1 = await getFromDb("bet", routingCacheDB.StringGet("getBet"),"wcWinner");
            if (d != null)
            {
                wcWinner = (Bet) d1;
                user.bets.Add(wcWinner);
            }
            dynamic d2 = await getFromDb("bet", routingCacheDB.StringGet("getBet"),"b_russia_uruguay");
            if (d != null)
            {
                game = (Bet) d2;
                user.bets.Add(game);
            }
            await updateUser(user);
            if (updateUserCache(user)<0)
                Console.WriteLine("cache didn't updated");

            return View(user);
        }
 
        public IActionResult Records()
        {

            return View();
        }
        public IActionResult Games()
        {

            return View();
        }

        public async Task<IActionResult> Tables(User user)
        {
            // ViewData["Message"] = "User Registration Page";
            User u =null;
            string json = userCachingDB.StringGet(user._id);
            if (json.Contains("_id"))
            {
                u = (User) JsonConvert.DeserializeObject<User>(json);
            }
            else u = new User();
            Table table = null;
            dynamic d = await getFromDb("table", userCachingDB.StringGet("getTable"), "overall");
            if (d != null)
            {
                table = (Table) d;
                u.tables.Add(table);
                table.users.Add(u);
                table.users.OrderBy(a => a.totalScore).ToList();
            }
            await updateTable(table);
            await updateUser(u);
            updateUserCache(u);

            return View(u);
        }
        public IActionResult Table()
        {

            return View();
        }
        public IActionResult QA()
        {

            return View();
        }
        public IActionResult About()
        {

            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
