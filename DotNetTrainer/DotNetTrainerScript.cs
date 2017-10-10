using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DotNetTrainer
{
    public class DotNetTrainerScript : Script
    {
        static Logger logger = new Logger();

        List<Menu> menus = new List<Menu>();
        Menu currentMenu = null;
        Menu nearbyCars = new Menu() { DrawType = DrawTypeEnum.ScreenList };

        public DotNetTrainerScript()
        {
            this.Tick += onTick;
            this.KeyUp += onKeyUp;
            this.KeyDown += onKeyDown;

            userInvincible = true;
            carInvincible = true;
            wrapInSpawned = true;

            InitMenu();
        }

        void InitMenu()
        {
            #region player
            /*
         { "TELEPORT", NULL, NULL},
         { "FIX PLAYER", NULL, NULL},
         { "ADD CASH", NULL, NULL},
         { "NEVER WANTED", &featurePlayerNeverWanted, NULL},
          */
            var wantedDown = new PointAction()
            {
                Identity = "player_wanted_down",
            };

            var wantedUp = new PointAction()
            {
                Identity = "player_wanted_up",
            };

            var player = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "player",
                Items = new MenuItem[]
                {
                    new MenuItem(){ Identity="main_player_wanted_up", Title="WANTED UP", PointTo=wantedUp },
                    new MenuItem(){ Identity="main_player_wanted_down", Title="WANTED DOWN", PointTo=wantedDown },
                    new MenuItem()
                    {
                        GetTitle = ()=>{ return "INVINCIBLE  ["+ ( userInvincible?"ON":"OFF") +"]";},
                        PointTo = new PointAction()
                        {
                            Action = () =>
                            {
                                 userInvincible = !userInvincible;
                            }
                        }
                    },
                }.ToList()
            };

            #endregion

            #region vehicle

            var fixVehicle = new PointAction()
            {
                Action = () =>
                {
                    var playerPed = Function.Call<int>(Hash.PLAYER_PED_ID);
                    bool bPlayerExists = Function.Call<bool>(Hash.DOES_ENTITY_EXIST, playerPed);

                    if (Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, playerPed, 0))
                    {
                        Function.Call(Hash.SET_VEHICLE_FIXED, Function.Call<int>(Hash.GET_VEHICLE_PED_IS_USING, playerPed));
                    }
                    else
                    {
                        ShowStatusText("player isn't in a vehicle");
                    }
                }
            };

            var spawner = new Menu()
            {
                DrawType = DrawTypeEnum.ScreenList,
                Identity = "vehicle_spawner",
            };

            spawner.Items.Add(new SpawnCarMenuItem("ZENTORNO"));
            spawner.Items.Add(new SpawnCarMenuItem("PANTO", "微型汽車"));
            spawner.Items.Add(new SpawnCarMenuItem("HUNTLEY", "肯特利S"));
            spawner.Items.Add(new SpawnCarMenuItem("EXEMPLAR"));
            spawner.Items.Add(new SpawnCarMenuItem("FQ2"));
            spawner.Items.Add(new SpawnCarMenuItem("BUFFALO2"));
            spawner.Items.Add(new SpawnCarMenuItem("BLAZER3"));
            spawner.Items.Add(new SpawnCarMenuItem("POLICE2"));
            spawner.Items.Add(new SpawnCarMenuItem("FBI2"));
            spawner.Items.Add(new SpawnCarMenuItem("GRESLEY"));
            spawner.Items.Add(new SpawnCarMenuItem("HOTKNIFE"));
            spawner.Items.Add(new SpawnCarMenuItem("BANSHEE"));
            spawner.Items.Add(new SpawnCarMenuItem("INSURGENT2"));
            spawner.Items.Add(new SpawnCarMenuItem("BUFFALO"));
            spawner.Items.Add(new SpawnCarMenuItem("BULLET"));
            spawner.Items.Add(new SpawnCarMenuItem("COQUETTE"));
            spawner.Items.Add(new SpawnCarMenuItem("MESA"));
            spawner.Items.Add(new SpawnCarMenuItem("TAXI"));
            spawner.Items.Add(new SpawnCarMenuItem("SUPERD"));
            spawner.Items.Add(new SpawnCarMenuItem("POLICEB"));
            spawner.Items.Add(new SpawnCarMenuItem("SANCHEZ2"));
            spawner.Items.Add(new SpawnCarMenuItem("DOUBLE"));
            spawner.Items.Add(new SpawnCarMenuItem("STRETCH", "加长轿車"));
            spawner.Items.Add(new SpawnCarMenuItem("RHINO"));
            spawner.Items.Add(new SpawnCarMenuItem("MESA3"));
            spawner.Items.Add(new SpawnCarMenuItem("DUNE2"));
            spawner.Items.Add(new SpawnCarMenuItem("SANDKING2"));
            spawner.Items.Add(new SpawnCarMenuItem("DUBSTA3"));
            spawner.Items.Add(new SpawnCarMenuItem("MONSTER"));
            spawner.Items.Add(new SpawnCarMenuItem("BUFFALO3"));
            spawner.Items.Add(new SpawnCarMenuItem("AIRTUG"));
            spawner.Items.Add(new SpawnCarMenuItem("BFINJECTION"));
            spawner.Items.Add(new SpawnCarMenuItem("RIPLEY", "機場牽引車"));
            spawner.Items.Add(new SpawnCarMenuItem("BUS"));
            spawner.Items.Add(new SpawnCarMenuItem("COACH"));
            spawner.Items.Add(new SpawnCarMenuItem("AMBULANCE"));
            spawner.Items.Add(new SpawnCarMenuItem("FIRETRUK"));
            spawner.Items.Add(new SpawnCarMenuItem("BARRACKS"));
            spawner.Items.Add(new SpawnCarMenuItem("DUMP"));
            spawner.Items.Add(new SpawnCarMenuItem("FLATBED"));
            spawner.Items.Add(new SpawnCarMenuItem("HAULER"));
            spawner.Items.Add(new SpawnCarMenuItem("JOURNEY"));
            spawner.Items.Add(new SpawnCarMenuItem("TOWTRUCK"));
            spawner.Items.Add(new SpawnCarMenuItem("BUZZARD"));
            spawner.Items.Add(new SpawnCarMenuItem("SAVAGE"));
            spawner.Items.Add(new SpawnCarMenuItem("CARGOBOB3"));
            spawner.Items.Add(new SpawnCarMenuItem("SKYLIFT"));
            spawner.Items.Add(new SpawnCarMenuItem("MILJET"));
            spawner.Items.Add(new SpawnCarMenuItem("JET"));
            spawner.Items.Add(new SpawnCarMenuItem("LUXOR"));
            spawner.Items.Add(new SpawnCarMenuItem("LAZER"));
            spawner.Items.Add(new SpawnCarMenuItem("HYDRA"));
            spawner.Items.Add(new SpawnCarMenuItem("DODO"));
            spawner.Items.Add(new SpawnCarMenuItem("STUNT"));
            spawner.Items.Add(new SpawnCarMenuItem("VESTRA"));
            spawner.Items.Add(new SpawnCarMenuItem("BLIMP"));
            spawner.Items.Add(new SpawnCarMenuItem("SCORCHER"));
            spawner.Items.Add(new SpawnCarMenuItem("BULLDOZER"));
            spawner.Items.Add(new SpawnCarMenuItem("CUTTER"));
            spawner.Items.Add(new SpawnCarMenuItem("TRACTOR"));
            spawner.Items.Add(new SpawnCarMenuItem("TRACTOR2"));
            spawner.Items.Add(new SpawnCarMenuItem("BOATTRAILER"));
            spawner.Items.Add(new SpawnCarMenuItem("ARMYTANKER"));
            spawner.Items.Add(new SpawnCarMenuItem("SUNTRAP"));
            spawner.Items.Add(new SpawnCarMenuItem("SQUALO"));
            spawner.Items.Add(new SpawnCarMenuItem("DINGHY2"));
            spawner.Items.Add(new SpawnCarMenuItem("JETMAX"));
            spawner.Items.Add(new SpawnCarMenuItem("SEASHARK2"));
            spawner.Items.Add(new SpawnCarMenuItem("SUBMERSIBLE"));
            spawner.Items.Add(new SpawnCarMenuItem("RIPLEY"));
            spawner.Items.Add(new SpawnCarMenuItem("ZTYPE"));


            //foreach (var v in Enum.GetValues(typeof(VehicleHash)))
            //{
            //    var hash = (VehicleHash)Convert.ToUInt32(v);
            //    spawner.Items.Add(new MenuItem() { Title = hash.ToString(), PointTo = new VehicleSpawnAction(hash) });
            //}

            var spawnerEntry = new MenuItem() { Identity = "main_vehicle_spawner", Title = "CAR SPAWNER", PointTo = spawner };
            spawner.FromItem = spawnerEntry;

            var nearby = new MenuItem()
            {
                Title = "SHOW NEARBY CARS",
                PointTo = new PointAction()
                {
                    Action = () =>
                    {
                        var cars = World.GetNearbyVehicles(Game.Player.Character.Position, 50).Select(i => new KeyValuePair<string, int>(i.FriendlyName + "," + i.DisplayName, i.Model.Hash)).Distinct();
                        nearbyCars.Items = cars.Select(i => new MenuItem() { Title = i.Key.Split(',')[0], PointTo = new PointAction() { Action = () => { logger.Log(i.Key + "," + i.Value); DotNetTrainerScript.SpawnCar((uint)i.Value); } } }).ToList();
                        nearbyCars.Items.First().Selected = true;
                        currentMenu = nearbyCars;
                    }
                }
            };

            nearbyCars.FromItem = nearby;

            var vehicle = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "vehicle",
                Items = new MenuItem[]
                {
                    spawnerEntry,
                    new MenuItem(){ Identity="main_vehicle_fix", Title="FIX", PointTo=fixVehicle },
                    new MenuItem()
                    {
                        GetTitle = ()=>{ return "INVINCIBLE  ["+ (carInvincible?"ON":"OFF") +"]";},
                        PointTo = new PointAction()
                        {
                            Action = () =>
                            {
                                //todo 设置正在坐的车
                                carInvincible = !carInvincible;
                            }
                        }
                    },
                    new MenuItem()
                    {
                        GetTitle = ()=>{ return "WRAP IN SPAWNED  ["+ (wrapInSpawned?"ON":"OFF") +"]";},
                        PointTo = new PointAction()
                        {
                            Action = () => { wrapInSpawned = !wrapInSpawned; }
                        }
                    },
                    nearby,
                }.ToList()
            };
            #endregion

            #region time

            var timeForward = new PointAction()
            {
                Identity = "time_forward",
                Action = () =>
                {
                    int h = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                    h = (h == 23) ? 0 : h + 1;
                    int m = Function.Call<int>(Hash.GET_CLOCK_MINUTES);
                    Function.Call<int>(Hash.SET_CLOCK_TIME, h, m, 0);
                    ShowStatusText(h + ":" + m);
                }
            };

            var timeBackward = new PointAction()
            {
                Identity = "time_backward",
                Action = () =>
                {
                    int h = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                    h = (h == 0) ? 23 : h - 1;
                    int m = Function.Call<int>(Hash.GET_CLOCK_MINUTES);
                    Function.Call<int>(Hash.SET_CLOCK_TIME, h, m, 0);
                    ShowStatusText(h + ":" + m);
                }
            };

            var time = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "time",
                Items = new MenuItem[]
                {
                    new MenuItem(){ Identity="main_time_forward", Title="FORWARD", PointTo=timeForward },
                    new MenuItem(){ Identity="main_time_backward", Title="BACKWARD", PointTo=timeBackward },
                }.ToList()
            };
            #endregion

            #region main

            var main_player = new MenuItem() { Identity = "main_player", Title = "PLAYER", PointTo = player };
            var main_vehicle = new MenuItem() { Identity = "main_vehicle", Title = "VEHICLE", PointTo = vehicle };
            var main_time = new MenuItem() { Identity = "main_time", Title = "TIME", PointTo = time };

            var main = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "main",
                Items = new MenuItem[]
                {
                    main_player,
                    main_vehicle,
                    main_time,
                }.ToList()
            };
            #endregion

            vehicle.FromItem = main_vehicle;
            player.FromItem = main_player;

            AddMenu(main);
        }

        void AddMenu(Menu menu)
        {
            menus.Add(menu);
            var children = menu.Items.Where(i => i.PointTo != null && i.PointTo.IsMenu).ToList();
            foreach (var i in children)
            {
                AddMenu(i.PointTo as Menu);
            }
        }

        private void onTick(object sender, EventArgs e)//每几毫秒调用
        {
            Draw();

            Game.Player.Character.IsInvincible = userInvincible;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:
                    if (currentMenu == null)
                    {
                        currentMenu = menus.Where(i => i.Identity == "main").First();
                        for (int i = 0; i < currentMenu.Items.Count; i++)
                            currentMenu.Items[i].Selected = i == 0;
                    }
                    else
                        currentMenu = null;
                    break;
                case Keys.Back:
                    currentMenu = currentMenu.FromItem?.Menu;
                    break;
                case Keys.J:
                case Keys.NumPad4://left
                    if (currentMenu.DrawType == DrawTypeEnum.ScreenList)
                    {
                        var newIdx = 0;
                        var idx = currentMenu.Items.FindIndex(i => i.Selected) + 1;
                        if (idx % listCountPerLine == 1)
                        {
                            newIdx = idx + listCountPerLine - 1;
                            if (newIdx > currentMenu.Items.Count)
                            {
                                newIdx = currentMenu.Items.Count;
                            }
                        }
                        else
                        {
                            newIdx = idx - 1;
                        }

                        newIdx--;

                        for (int i = 0; i < currentMenu.Items.Count; i++)
                        {
                            currentMenu.Items[i].Selected = i == newIdx;
                        }
                    }
                    break;
                case Keys.L:
                case Keys.NumPad6://right
                    if (currentMenu.DrawType == DrawTypeEnum.ScreenList)
                    {
                        var newIdx = 0;
                        var idx = currentMenu.Items.FindIndex(i => i.Selected) + 1;
                        if (idx % listCountPerLine == 0 || idx == currentMenu.Items.Count)
                        {
                            var line = idx / listCountPerLine;
                            if (idx % listCountPerLine == 0)
                                line--;

                            newIdx = line * listCountPerLine + 1;
                        }
                        else
                        {
                            newIdx = idx + 1;
                        }

                        newIdx--;
                        for (int i = 0; i < currentMenu.Items.Count; i++)
                        {
                            currentMenu.Items[i].Selected = i == newIdx;
                        }
                    }
                    break;
                case Keys.I:
                case Keys.NumPad8://up
                    {
                        if (currentMenu.DrawType == DrawTypeEnum.NormalMenu)
                        {
                            var idx = currentMenu.Items.FindIndex(i => i.Selected) - 1;
                            if (idx < 0)
                                idx = currentMenu.Items.Count - 1;

                            for (int i = 0; i < currentMenu.Items.Count; i++)
                                currentMenu.Items[i].Selected = i == idx;
                        }
                        else if (currentMenu.DrawType == DrawTypeEnum.ScreenList)
                        {
                            var idx = currentMenu.Items.FindIndex(i => i.Selected) + 1;
                            if (idx <= listCountPerLine)
                                return;

                            var newIdx = idx - listCountPerLine;

                            newIdx--;
                            for (int i = 0; i < currentMenu.Items.Count; i++)
                                currentMenu.Items[i].Selected = i == newIdx;
                        }
                    }
                    break;
                case Keys.K:
                case Keys.NumPad2://down
                    {
                        if (currentMenu.DrawType == DrawTypeEnum.NormalMenu)
                        {
                            var idx = currentMenu.Items.FindIndex(i => i.Selected) + 1;
                            if (idx > currentMenu.Items.Count - 1)
                                idx = 0;

                            for (int i = 0; i < currentMenu.Items.Count; i++)
                                currentMenu.Items[i].Selected = i == idx;
                        }
                        else if (currentMenu.DrawType == DrawTypeEnum.ScreenList)
                        {
                            var allLine = currentMenu.Items.Count / listCountPerLine;
                            if (currentMenu.Items.Count % listCountPerLine != 0)
                                allLine++;

                            var idx = currentMenu.Items.FindIndex(i => i.Selected) + 1;
                            var inLine = idx / listCountPerLine;
                            if (idx % listCountPerLine != 0)
                                inLine++;
                            if (inLine == allLine)
                                return;

                            var newIdx = idx + listCountPerLine;
                            if (newIdx > currentMenu.Items.Count)
                                newIdx = currentMenu.Items.Count;

                            newIdx--;
                            for (int i = 0; i < currentMenu.Items.Count; i++)
                                currentMenu.Items[i].Selected = i == newIdx;
                        }
                    }
                    break;
                case Keys.Enter:
                    if (currentMenu != null)
                    {
                        var item = currentMenu.Items.Where(i => i.Selected).First();
                        if (item.PointTo.IsMenu)
                        {
                            var menu = item.PointTo as Menu;
                            for (int i = 0; i < menu.Items.Count; i++)
                                menu.Items[i].Selected = i == 0;

                            menu.FromItem = item;
                            currentMenu = menu;
                        }
                        else
                        {
                            var action = item.PointTo as PointAction;
                            action.Action();
                            if (action.CloseAllMenuAfterAction)
                                currentMenu = null;
                        }
                    }
                    break;
            }
        }

        void ShowStatusText(string text)
        {
            statusText = text;
            statusTextShowTime = 0;
        }

        void Draw()
        {
            if (!string.IsNullOrEmpty(statusText))
            {
                DrawStatusText(statusText);
                if (++statusTextShowTime >= statusTextShowTimeMax)
                {
                    statusText = "";
                    statusTextShowTime = 0;
                }
            }

            if (currentMenu == null)
                return;

            if (currentMenu.DrawType == DrawTypeEnum.NormalMenu)
            {
                string title = "DotNetTrainer";
                if (currentMenu.FromItem != null)
                    title = currentMenu.FromItem.Title;

                draw_menu_line(title, normalMenuWidth, normalMenuHeight, 0, 0, 5, false, true);
                int idx = 0;
                foreach (var i in currentMenu.Items)
                {
                    idx++;
                    draw_menu_line(i.GetTitle == null ? i.Title : i.GetTitle(), normalMenuWidth, normalMenuHeight, idx * normalMenuHeight, 0, 9, i.Selected, false);
                }
            }
            else if (currentMenu.DrawType == DrawTypeEnum.ScreenList)
            {
                var selected = currentMenu.Items.FindIndex(i => i.Selected) + 1;
                var skipLine = selected / listCountPerLine;
                if (selected % listCountPerLine == 0)
                    skipLine--;

                var idxInLine = selected % listCountPerLine;

                int idx = 0;
                foreach (var i in currentMenu.Items.Skip(listCountPerLine * skipLine).Take(listCountPerLine))
                {
                    draw_menu_line(i.GetTitle == null ? i.Title : i.GetTitle(), listItemWidth, listItemHeight, 200, 50 + idx * 120, 55 + idx * 120, i.Selected, false);
                    idx++;
                }
            }
        }

        static bool userInvincible = false;
        static bool carInvincible = false;
        static bool wrapInSpawned = false;

        string statusText = "";
        int statusTextShowTime = 0;
        int statusTextShowTimeMax = 80;

        const int normalMenuWidth = 250;
        const int normalMenuHeight = 36;

        const int listCountPerLine = 10;
        const int listItemWidth = 110;
        const int listItemHeight = 32;

        void draw_menu_line(string caption, int lineWidth, int lineHeight, int lineTop, int lineLeft, int textLeft, bool active, bool title, bool rescaleText = true)
        {
            var textColor = Color.FromArgb(255, 255, 255, 255);
            var rectColor = Color.FromArgb(255, 70, 95, 95);

            var activeTextColor = Color.FromArgb(255, 0, 0, 0);
            var activeRectColor = Color.FromArgb(255, 218, 242, 216);

            var titleRectColor = Color.FromArgb(255, 0, 0, 0);
            var titleTextColor = textColor;

            float text_scale = 0.35f;
            int font = 0;

            if (active && rescaleText)
            {
                text_scale = 0.40f;
            }

            if (title && rescaleText)
            {
                text_scale = 0.40f;
                font = 1;
            }

            new UIText(caption, new Point(textLeft, lineTop + 4), text_scale, title ? titleTextColor : (active ? activeTextColor : textColor), (GTA.Font)font, false).Draw();
            new UIRectangle(new Point(lineLeft, lineTop), new Size(lineWidth, lineHeight), title ? titleRectColor : (active ? activeRectColor : rectColor)).Draw();
        }

        void DrawStatusText(string text)
        {
            new UIText(text, new Point(520, 52), 0.35f, Color.FromArgb(255, 255, 255, 255), 0, false).Draw();
            var width = 20 + text.Length * 9;
            if (width < 100)
                width = 100;
            new UIRectangle(new Point(500, 50), new Size(width, 30), Color.FromArgb(255, 70, 95, 95)).Draw();
        }

        public static void SpawnCar(uint hash)
        {
            Vehicle vehicle = World.CreateVehicle((VehicleHash)hash, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 8, Game.Player.Character.Heading + 90);
            vehicle.CanTiresBurst = false;
            vehicle.CustomPrimaryColor = Color.White;
            vehicle.CustomSecondaryColor = Color.Black;
            vehicle.PlaceOnGround();
            vehicle.IsInvincible = carInvincible;
            vehicle.NumberPlate = "HELLO";

            if (wrapInSpawned)
            {
                Function.Call(Hash.SET_ENTITY_HEADING, vehicle.Handle, Game.Player.Character.Heading);
                Function.Call(Hash.SET_PED_INTO_VEHICLE, Game.Player.Character.Handle, vehicle.Handle, -1);
            }
        }
    }
}