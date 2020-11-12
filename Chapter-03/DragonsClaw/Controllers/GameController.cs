using DragonsClaw.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DragonsClaw.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        [Authorize]
        public ActionResult Index()
        {
            if (!DataLayer.DoesPlayerExist(User.Identity.GetUserName()))
            {
                return RedirectToAction("Create");
            }

            return View(DataLayer.GetPlayer(User.Identity.GetUserName()));
        }

        [Authorize]
        public ActionResult Create()
        {
            if (DataLayer.DoesPlayerExist(User.Identity.GetUserName()))
            {
                return RedirectToAction("Index");
            }

            CreatePlayerModel model = new CreatePlayerModel();

            ExtractClassAndRace(model);
            return View(model);
        }

        private void ExtractClassAndRace(CreatePlayerModel model)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM Races";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<SelectListItem> items = new List<SelectListItem>
                        {
                            new SelectListItem() { Value = "-1", Text = "--------------------" }
                        };

                        while (reader.Read())
                        {
                            SelectListItem item = new SelectListItem
                            {
                                Text = reader.GetString(1),
                                Value = reader.GetInt32(0).ToString()
                            };

                            items.Add(item);
                        }

                        model.RaceList = new SelectList(items, "Value", "Text");

                        reader.Close();
                    }

                    command.CommandText = "SELECT * FROM Classes";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<SelectListItem> items = new List<SelectListItem>
                        {
                            new SelectListItem() { Value = "-1", Text = "--------------------" }
                        };

                        while (reader.Read())
                        {
                            SelectListItem item = new SelectListItem
                            {
                                Text = reader.GetString(1),
                                Value = reader.GetInt32(0).ToString()
                            };

                            items.Add(item);
                        }

                        model.ClassList = new SelectList(items, "Value", "Text");
                    }
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CreatePlayerModel model)
        {
            if (!ModelState.IsValid || model.Race == "-1" || model.Class == "-1" || model.Gender == "-1")
            {
                ExtractClassAndRace(model);
                return View(model);
            }

            DataLayer.CreatePlayer(model, User.Identity.GetUserName());

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Military()
        {
            PlayerViewModel player = DataLayer.GetPlayer(User.Identity.GetUserName());

            TrainingViewModel model = new TrainingViewModel
            {
                Player = player
            };

            DataLayer.GetTroops(player.RaceId, model);

            model.MaxOffensive = Math.Min(player.Cadets / model.OffUnit.CadetCost, player.Credits / model.OffUnit.CreditCost);
            model.MaxDefensive = Math.Min(player.Cadets / model.DefUnit.CadetCost, player.Credits / model.DefUnit.CreditCost);
            model.MaxSpecialty = Math.Min(player.Cadets / model.SpecUnit.CadetCost, player.Credits / model.SpecUnit.CreditCost);
            model.MaxFighters = Math.Min(player.Cadets / 8, player.Credits / 2000);
            model.MaxFighters = Math.Min(model.MaxFighters, player.Ore / model.Fighter.OreCost);
            model.MaxFighters = Math.Min(model.MaxFighters, player.Dilithium / model.Fighter.DilithiumCost);
            model.MaxBombers = Math.Min(player.Cadets / 12, player.Credits / 3000);
            model.MaxBombers = Math.Min(model.MaxBombers, player.Ore / model.Bomber.OreCost);
            model.MaxBombers = Math.Min(model.MaxBombers, player.Dilithium / model.Bomber.DilithiumCost);
            model.MaxSpies = Math.Min(player.Cadets / model.Spy.CadetCost, player.Credits / model.Spy.CreditCost);
            model.MaxRaiders = Math.Min(player.Cadets / model.Raider.CadetCost, player.Credits / model.Raider.CreditCost);
            model.MaxRaiders = Math.Min(model.MaxRaiders, player.Ore / model.Raider.OreCost);
            model.MaxRaiders = Math.Min(model.MaxRaiders, player.Dilithium / model.Raider.DilithiumCost);
            model.MaxCruisers = Math.Min(player.Cadets / 20, player.Credits / 5000);
            model.MaxCruisers = Math.Min(model.MaxCruisers, player.Ore / model.Cruiser.OreCost);
            model.MaxCruisers = Math.Min(model.MaxCruisers, player.Dilithium / model.Cruiser.DilithiumCost);
            model.MaxDestroyers = Math.Min(player.Cadets / 50, player.Credits / 10000);
            model.MaxDestroyers = Math.Min(model.MaxDestroyers, player.Ore / model.Destroyer.OreCost);
            model.MaxDestroyers = Math.Min(model.MaxDestroyers, player.Dilithium / model.Destroyer.DilithiumCost);
            model.MaxDreadnaughts = Math.Min(player.Cadets / 100, player.Credits / 50000);
            model.MaxDreadnaughts = Math.Min(model.MaxDreadnaughts, player.Ore / model.Dreadnaught.OreCost);
            model.MaxDreadnaughts = Math.Min(model.MaxDreadnaughts, player.Dilithium / model.Dreadnaught.DilithiumCost);

            model.InTraining = DataLayer.GetInTraining( player);
            return PartialView("_MilitaryView", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Military([System.Web.Http.FromBody] TrainingViewModel model)
        {
            PlayerViewModel player = DataLayer.GetPlayer( User.Identity.GetUserName());

            TrainingViewModel training = new TrainingViewModel
            {
                Player = player
            };

            DataLayer.GetTroops( player.RaceId, training);

            if (player.PlayerId == 0)
                return RedirectToAction("Create", "Home");

            int offensive = Math.Min(player.Cadets / training.OffUnit.CadetCost, model.OffensiveUnits);
            int defensive = Math.Min(player.Cadets / training.DefUnit.CadetCost, model.DefensiveUnits);
            int specialty = Math.Min(player.Cadets / training.SpecUnit.CadetCost, model.SpecialtyUnits);

            if (player.Credits >= offensive * training.OffUnit.CreditCost)
            {
                player.Credits -= offensive * training.OffUnit.CreditCost;
                player.Cadets -= offensive;
            }
            else
            {
                offensive = player.Credits / training.OffUnit.CreditCost;
                player.Credits -= offensive * training.OffUnit.CreditCost;
                player.Cadets -= offensive;
            }

            if (defensive > player.Cadets / training.DefUnit.CadetCost)
                defensive = player.Cadets / training.DefUnit.CadetCost;

            if (player.Credits >= defensive * training.DefUnit.CreditCost)
            {
                player.Credits -= defensive * training.DefUnit.CreditCost;
                player.Cadets -= defensive;
            }
            else
            {
                defensive = player.Credits / training.DefUnit.CreditCost;
                player.Credits -= defensive * training.DefUnit.CreditCost;
                player.Cadets -= defensive;
            }

            if (specialty > player.Cadets / training.SpecUnit.CadetCost)
                specialty = player.Cadets / training.SpecUnit.CadetCost;

            if (player.Credits >= specialty * training.SpecUnit.CreditCost)
            {
                player.Credits -= specialty * training.SpecUnit.CreditCost;
                player.Cadets -= specialty;
            }
            else
            {
                specialty = player.Credits / training.SpecUnit.CreditCost;
                player.Credits -= specialty * training.SpecUnit.CreditCost;
                player.Cadets -= specialty;
            }

            int fighters = Math.Min(player.Cadets / 8, model.FighterUnits);
            fighters = Math.Min(fighters, player.Ore / training.Fighter.OreCost);
            fighters = Math.Min(fighters, player.Dilithium / training.Fighter.DilithiumCost);

            if (fighters > player.Cadets / 8)
                fighters = player.Cadets / 8;

            if (player.Credits >= fighters * training.Fighter.CreditCost)
            {
                player.Credits -= fighters * training.Fighter.CreditCost;
                player.Cadets -= fighters;
                player.Ore -= fighters * training.Fighter.OreCost;
                player.Dilithium -= fighters * training.Fighter.DilithiumCost;
            }
            else
            {
                fighters = player.Credits / training.Fighter.CreditCost;
                player.Credits -= fighters * training.Fighter.CreditCost;
                player.Cadets -= fighters;
                player.Cadets -= fighters;
                player.Ore -= fighters * training.Fighter.OreCost;
                player.Dilithium -= fighters * training.Fighter.DilithiumCost;
            }

            int bombers = Math.Min(player.Cadets / 12, model.BomberUnits);
            bombers = Math.Min(bombers, player.Ore / training.Bomber.OreCost);
            bombers = Math.Min(bombers, player.Dilithium / training.Bomber.DilithiumCost);

            if (bombers > player.Cadets / 12)
                bombers = player.Cadets / 12;

            if (player.Credits < bombers * training.Bomber.CreditCost)
            {
                bombers = player.Credits / training.Bomber.CreditCost;
            }

            player.Credits -= bombers * training.Bomber.CreditCost;
            player.Cadets -= bombers;
            player.Ore -= bombers * training.Bomber.OreCost;
            player.Dilithium -= bombers * training.Bomber.DilithiumCost;

            int cruisers = Math.Min(player.Cadets / 20, model.CruiserUnits);
            cruisers = Math.Min(cruisers, player.Ore / training.Cruiser.OreCost);
            cruisers = Math.Min(cruisers, player.Dilithium / training.Cruiser.DilithiumCost);

            if (player.Credits < cruisers * training.Cruiser.CreditCost)
            {
                cruisers = player.Credits / training.Cruiser.CreditCost;
            }

            player.Credits -= cruisers * training.Cruiser.CreditCost;
            player.Cadets -= cruisers * training.Cruiser.CadetCost;
            player.Ore -= cruisers * training.Cruiser.OreCost;
            player.Dilithium -= cruisers * training.Cruiser.DilithiumCost;

            int destroyers = Math.Min(player.Cadets / 50, model.DestroyerUnits);
            destroyers = Math.Min(destroyers, player.Ore / training.Destroyer.OreCost);
            destroyers = Math.Min(destroyers, player.Dilithium / training.Destroyer.DilithiumCost);

            if (player.Credits < destroyers * training.Destroyer.CreditCost)
            {
                destroyers = player.Credits / training.Destroyer.CreditCost;
            }

            player.Credits -= destroyers * training.Destroyer.CreditCost;
            player.Cadets -= destroyers * training.Destroyer.CadetCost;
            player.Ore -= destroyers * training.Destroyer.OreCost;
            player.Dilithium -= destroyers * training.Destroyer.DilithiumCost;

            int dreadnaughts = Math.Min(player.Cadets / 100, model.DreadnaughtUnits);
            dreadnaughts = Math.Min(dreadnaughts, player.Ore / training.Dreadnaught.OreCost);
            dreadnaughts = Math.Min(dreadnaughts, player.Dilithium / training.Dreadnaught.DilithiumCost);

            if (player.Credits < dreadnaughts * training.Dreadnaught.CreditCost)
            {
                dreadnaughts = player.Credits / training.Dreadnaught.CreditCost;
            }

            player.Credits -= dreadnaughts * training.Dreadnaught.CreditCost;
            player.Cadets -= dreadnaughts * training.Dreadnaught.CadetCost;
            player.Ore -= dreadnaughts * training.Dreadnaught.OreCost;
            player.Dilithium -= dreadnaughts * training.Dreadnaught.DilithiumCost;

            int spies = Math.Min(player.Cadets / training.Spy.CadetCost, model.SpyUnits);

            if (player.Credits < spies * training.Spy.CreditCost)
            {
                spies = player.Credits / training.Spy.CreditCost;
            }

            player.Credits -= spies * training.Spy.CreditCost;
            player.Cadets -= spies;

            int raiders = Math.Min(player.Cadets, model.RaiderUnits);
            raiders = Math.Min(raiders, player.Ore / training.Raider.OreCost);
            raiders = Math.Min(raiders, player.Dilithium / training.Raider.DilithiumCost);

            if (player.Credits < raiders * training.Raider.CreditCost)
            {
                raiders = player.Credits / training.Raider.CreditCost;
            }

            player.Credits -= raiders * training.Raider.CreditCost;
            player.Cadets -= raiders;
            player.Ore -= raiders * training.Raider.OreCost;
            player.Dilithium -= raiders * training.Raider.DilithiumCost;

            DataLayer.UpdatePlayer( player);

            if (offensive > 0)
                DataLayer.QueueTroops( player.PlayerId, "OffensiveQueue", offensive, 1);

            if (defensive > 0)
                DataLayer.QueueTroops( player.PlayerId, "DefensiveQueue", defensive, 1);

            if (specialty > 0)
                DataLayer.QueueTroops( player.PlayerId, "SpecialtyQueue", specialty, 2);

            if (fighters > 0)
                DataLayer.QueueTroops( player.PlayerId, "FighterQueue", fighters, 10);

            if (bombers > 0)
                DataLayer.QueueTroops( player.PlayerId, "BomberQueue", bombers, 20);

            if (cruisers > 0)
                DataLayer.QueueTroops( player.PlayerId, "CruiserQueue", cruisers, 20);

            if (destroyers > 0)
                DataLayer.QueueTroops( player.PlayerId, "DestroyerQueue", destroyers, 50);

            if (dreadnaughts > 0)
                DataLayer.QueueTroops( player.PlayerId, "DreadnaughtQueue", dreadnaughts, 100);

            if (spies > 0)
                DataLayer.QueueTroops( player.PlayerId, "SpyQueue", spies, 5);

            if (raiders > 0)
                DataLayer.QueueTroops( player.PlayerId, "RaiderQueue", raiders, 5);

            player = DataLayer.GetPlayer( User.Identity.GetUserName());

            training = new TrainingViewModel
            {
                Player = player
            };

            DataLayer.GetTroops( player.RaceId, training);

            training.MaxOffensive = Math.Min(player.Cadets / training.OffUnit.CadetCost, player.Credits / training.OffUnit.CreditCost);
            training.MaxDefensive = Math.Min(player.Cadets / training.DefUnit.CadetCost, player.Credits / training.DefUnit.CreditCost);
            training.MaxSpecialty = Math.Min(player.Cadets / training.SpecUnit.CadetCost, player.Credits / training.SpecUnit.CreditCost);
            training.MaxFighters = Math.Min(player.Cadets / 8, player.Credits / 2000);
            training.MaxFighters = Math.Min(training.MaxFighters, player.Ore / training.Fighter.OreCost);
            training.MaxFighters = Math.Min(training.MaxFighters, player.Dilithium / training.Fighter.DilithiumCost);
            training.MaxBombers = Math.Min(player.Cadets / 12, player.Credits / 3000);
            training.MaxBombers = Math.Min(training.MaxBombers, player.Ore / training.Bomber.OreCost);
            training.MaxBombers = Math.Min(training.MaxBombers, player.Dilithium / training.Bomber.DilithiumCost);
            training.MaxSpies = Math.Min(player.Cadets / training.Spy.CadetCost, player.Credits / training.Spy.CreditCost);
            training.MaxRaiders = Math.Min(player.Cadets / training.Raider.CadetCost, player.Credits / training.Raider.CreditCost);
            training.MaxRaiders = Math.Min(training.MaxRaiders, player.Ore / training.Raider.OreCost);
            training.MaxRaiders = Math.Min(training.MaxRaiders, player.Dilithium / training.Raider.DilithiumCost);
            training.MaxCruisers = Math.Min(player.Cadets / 20, player.Credits / 5000);
            training.MaxCruisers = Math.Min(training.MaxCruisers, player.Ore / training.Cruiser.OreCost);
            training.MaxCruisers = Math.Min(training.MaxCruisers, player.Dilithium / training.Cruiser.DilithiumCost);
            training.MaxDestroyers = Math.Min(player.Cadets / 50, player.Credits / 10000);
            training.MaxDestroyers = Math.Min(training.MaxDestroyers, player.Ore / training.Destroyer.OreCost);
            training.MaxDestroyers = Math.Min(training.MaxDestroyers, player.Dilithium / training.Destroyer.DilithiumCost);
            training.MaxDreadnaughts = Math.Min(player.Cadets / 100, player.Credits / 50000);
            training.MaxDreadnaughts = Math.Min(training.MaxDreadnaughts, player.Ore / training.Dreadnaught.OreCost);
            training.MaxDreadnaughts = Math.Min(training.MaxDreadnaughts, player.Dilithium / training.Dreadnaught.DilithiumCost);

            string message = "You have ordered that";
            bool comma = false;

            if (offensive > 0)
            {
                message += " " + offensive + " " + training.OffensiveName + "s";
                comma = true;
            }

            if (defensive > 0)
            {
                if (comma)
                    message += ',';

                message += " " + defensive + " " + training.DefensiveName + "s";
                comma = true;
            }

            if (specialty > 0)
            {
                if (comma)
                    message += ",";

                message += " " + specialty + " " + training.SpecialtyName + "s";
                comma = true;
            }

            if (fighters > 0)
            {
                if (comma)
                    message += ",";

                message += " " + fighters + " Fighters";
                comma = true;
            }

            if (bombers > 0)
            {
                if (comma)
                    message += ",";

                message += " " + bombers + " Bombers";
                comma = true;
            }

            if (cruisers > 0)
            {
                if (comma)
                    message += ",";

                message += " " + cruisers + " Cruisers";
                comma = true;
            }

            if (destroyers > 0)
            {
                if (comma)
                    message += ",";

                message += " " + destroyers + " Destroyers";
                comma = true;
            }

            if (dreadnaughts > 0)
            {
                if (comma)
                    message += ",";

                message += " " + dreadnaughts + " Dreadnaughts";
                comma = true;
            }

            if (spies > 0)
            {
                if (comma)
                    message += ",";

                message += " " + spies + " Spies";
                comma = true;
            }

            if (raiders > 0)
            {
                if (comma)
                    message += ",";

                message += " " + raiders + " Raiders";
                comma = true;
            }

            message += " be trained. Check the military advisor on when the units might be available.";

            int i = message.LastIndexOf(",");

            if (i >= 0)
                message = message.Substring(0, i) + " and" + message.Substring(i + 1);

            training.InTraining = DataLayer.GetInTraining(player);
            training.Message = message;

            return PartialView("_MilitaryView", training);
        }
    }
}