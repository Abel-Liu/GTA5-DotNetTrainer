using GTA;
using GTA.Math;
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

            var player = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "player",
                Items = new MenuItem[]
                {
                    new MenuItem()
                    {
                        Title ="WANTED UP",
                        PointTo = new PointAction()
                        {
                            Action=()=> { Game.Player.WantedLevel++; }
                        }
                    },
                    new MenuItem()
                    {
                        Title ="WANTED DOWN",
                        PointTo = new PointAction()
                        {
                            Action=()=> { Game.Player.WantedLevel--; }
                        }
                    },
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
            spawner.Items.Add(new SpawnCarMenuItem("BANSHEE", "女妖"));
            spawner.Items.Add(new SpawnCarMenuItem("RHINO", "犀式坦克"));
            spawner.Items.Add(new SpawnCarMenuItem("PANTO", "微型汽車"));
            spawner.Items.Add(new SpawnCarMenuItem("BLAZER3", "四輪摩托"));
            spawner.Items.Add(new SpawnCarMenuItem("STRETCH", "加長轎車"));
            spawner.Items.Add(new SpawnCarMenuItem("HUNTLEY", "肯特利S"));
            spawner.Items.Add(new SpawnCarMenuItem("FQ2"));
            spawner.Items.Add(new SpawnCarMenuItem("TAXI"));
            spawner.Items.Add(new SpawnCarMenuItem("POLICE2"));
            spawner.Items.Add(new SpawnCarMenuItem("FBI2"));
            spawner.Items.Add(new SpawnCarMenuItem("INSURGENT2", "叛亂份子"));
            spawner.Items.Add(new SpawnCarMenuItem("SUPERD"));
            spawner.Items.Add(new SpawnCarMenuItem("POLICEB", "警用摩托"));
            spawner.Items.Add(new SpawnCarMenuItem("DOUBLE", "摩托"));
            spawner.Items.Add(new SpawnCarMenuItem("DUBSTA3", "迪布達6x6"));
            spawner.Items.Add(new SpawnCarMenuItem("MONSTER", "大腳車"));
            spawner.Items.Add(new SpawnCarMenuItem("AIRTUG", "行李拖車"));
            spawner.Items.Add(new SpawnCarMenuItem("RIPLEY", "機場牽引車"));
            spawner.Items.Add(new SpawnCarMenuItem(516990260, "公共事業卡車"));
            spawner.Items.Add(new SpawnCarMenuItem("BULLDOZER", "推土機"));
            spawner.Items.Add(new SpawnCarMenuItem("CUTTER", "鑽洞機"));
            spawner.Items.Add(new SpawnCarMenuItem("BUS"));
            spawner.Items.Add(new SpawnCarMenuItem("COACH"));
            spawner.Items.Add(new SpawnCarMenuItem("AMBULANCE"));
            spawner.Items.Add(new SpawnCarMenuItem(1938952078, "消防車"));
            spawner.Items.Add(new SpawnCarMenuItem("BARRACKS", "軍用卡車"));
            spawner.Items.Add(new SpawnCarMenuItem("DUMP", "運土車"));
            spawner.Items.Add(new SpawnCarMenuItem("HAULER", "卡車頭"));
            spawner.Items.Add(new SpawnCarMenuItem("TOWTRUCK", "拖吊車"));
            spawner.Items.Add(new SpawnCarMenuItem("BUZZARD", "小直升機"));
            spawner.Items.Add(new SpawnCarMenuItem("SAVAGE", "大直升機"));
            spawner.Items.Add(new SpawnCarMenuItem("CARGOBOB3", "運輸直升機"));
            spawner.Items.Add(new SpawnCarMenuItem("MILJET", "小客機"));
            spawner.Items.Add(new SpawnCarMenuItem("JET", "大客機"));
            spawner.Items.Add(new SpawnCarMenuItem("LUXOR", "商務機"));
            spawner.Items.Add(new SpawnCarMenuItem("LAZER", "戰鬥機"));
            spawner.Items.Add(new SpawnCarMenuItem("HYDRA", "鷂式戰機"));
            spawner.Items.Add(new SpawnCarMenuItem("DODO", "水上飛機"));
            spawner.Items.Add(new SpawnCarMenuItem("STUNT", "特技飛機"));
            spawner.Items.Add(new SpawnCarMenuItem("BLIMP", "汽艇"));
            spawner.Items.Add(new SpawnCarMenuItem("SCORCHER", "自行車"));
            spawner.Items.Add(new SpawnCarMenuItem("TRACTOR2", "農耕機"));
            spawner.Items.Add(new SpawnCarMenuItem("DINGHY2", "救生艇"));
            spawner.Items.Add(new SpawnCarMenuItem("SEASHARK2", "小海鯊"));
            spawner.Items.Add(new SpawnCarMenuItem("SUBMERSIBLE", "潛艇"));

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
                        var cars = World.GetNearbyVehicles(Game.Player.Character.Position, 50)
                            .Select(i => new VehicleInformation() { DisplayName = i.DisplayName, FriendlyName = i.FriendlyName, Hash = i.Model.Hash })
                            .Distinct(new VehicleInformationEqualityComparer());

                        nearbyCars.Items = cars.Select(i => new MenuItem() { Title = i.FriendlyName, PointTo = new PointAction() { Action = () => { LogCarInfo(i.DisplayName, i.FriendlyName, i.Hash); DotNetTrainerScript.SpawnCar((uint)i.Hash); } } }).ToList();
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
                    //new MenuItem()
                    //{
                    //    GetTitle = ()=>{ return "SEATBELT  ["+ (seatbelt?"ON":"OFF") +"]";},
                    //    PointTo = new PointAction()
                    //    {
                    //        Action = () => { seatbelt = !seatbelt; }
                    //    }
                    //},
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

            //seatbelt
            const int PED_FLAG_CAN_FLY_THRU_WINDSCREEN = 32;
            Function.Call(Hash.SET_PED_CONFIG_FLAG, Game.Player.Character.Handle, PED_FLAG_CAN_FLY_THRU_WINDSCREEN, false);
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F6:
                    SpawnCar("ZENTORNO");
                    break;
                case Keys.F7:
                    SpawnCar("LAZER");
                    break;
                case Keys.F8:
                    SpawnCar("HYDRA");
                    break;
                case Keys.F10:
                    if (lastVehicleHash != 0)
                        SpawnCar(lastVehicleHash);
                    break;
                case Keys.O:
                case Keys.NumPad9://spead up
                    {
                        var veh = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character.Handle);
                        float speed = Function.Call<float>(Hash.GET_ENTITY_SPEED, veh);
                        if (speed < 8.0f)
                            speed = 8.0f;
                        speed += speed * 0.5f;

                        Function.Call(Hash.SET_VEHICLE_FORWARD_SPEED, veh, speed);
                    }
                    break;
                case Keys.U:
                case Keys.NumPad3://spead down
                    {
                        var veh = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character.Handle);
                        float speed = Function.Call<float>(Hash.GET_ENTITY_SPEED, veh);

                        if (Function.Call<bool>(Hash.IS_ENTITY_IN_AIR, veh) || speed > 5.0)
                            Function.Call(Hash.SET_VEHICLE_FORWARD_SPEED, veh, 0);
                    }
                    break;
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
                            if (currentMenu.Items.Count <= listCountPerLine)
                                return;

                            var idx = currentMenu.Items.FindIndex(i => i.Selected) + 1;

                            var newIdx = 0;
                            if (idx > listCountPerLine)
                                newIdx = idx - listCountPerLine;
                            else
                            {
                                if (currentMenu.Items.Count % listCountPerLine != 0)
                                {
                                    var lastCount = currentMenu.Items.Count % listCountPerLine;

                                    newIdx = currentMenu.Items.Count / listCountPerLine * listCountPerLine + (lastCount >= idx ? idx : lastCount);
                                }
                                else
                                {
                                    var lastLine = currentMenu.Items.Count / listCountPerLine;
                                    newIdx = --lastLine * listCountPerLine + idx;
                                }
                            }

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
                            var allLineCount = currentMenu.Items.Count / listCountPerLine;
                            if (currentMenu.Items.Count % listCountPerLine != 0)
                                allLineCount++;

                            if (allLineCount == 1)
                                return;

                            var currentIdx = currentMenu.Items.FindIndex(i => i.Selected) + 1;

                            var currentLine = currentIdx / listCountPerLine;
                            if (currentIdx % listCountPerLine != 0)
                                currentLine++;

                            int newIdx = 0;
                            if (currentLine != allLineCount)
                            {
                                newIdx = currentIdx + listCountPerLine;
                                if (newIdx > currentMenu.Items.Count)
                                    newIdx = currentMenu.Items.Count;
                            }
                            else
                            {
                                newIdx = currentIdx % listCountPerLine;
                            }

                            newIdx--;
                            for (int i = 0; i < currentMenu.Items.Count; i++)
                                currentMenu.Items[i].Selected = i == newIdx;
                        }
                    }
                    break;
                case Keys.Enter:
                case Keys.NumPad5:
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
                case Keys.D8://rocket
                case Keys.D9:
                    if (Function.Call<bool>(Hash.IS_PLAYER_CONTROL_ON, Game.Player.Handle) && Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Game.Player.Character.Handle, 0))
                    {
                        var weaponAssetRocket = Function.Call<int>(Hash.GET_HASH_KEY, "WEAPON_VEHICLE_ROCKET");
                        if (!Function.Call<bool>(Hash.HAS_WEAPON_ASSET_LOADED, weaponAssetRocket))
                        {
                            Function.Call(Hash.REQUEST_WEAPON_ASSET, weaponAssetRocket, 31, 0);
                            while (!Function.Call<bool>(Hash.HAS_WEAPON_ASSET_LOADED, weaponAssetRocket))
                            { }
                        }

                        var veh = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character.Handle);

                        var o0 = new OutputArgument();
                        var o1 = new OutputArgument();
                        //Get size
                        Function.Call(Hash.GET_MODEL_DIMENSIONS, Function.Call<int>(Hash.GET_ENTITY_MODEL, veh), o0, o1);

                        var vMin = o0.GetResult<Vector3>();
                        var vMax = o1.GetResult<Vector3>();

                        if (e.KeyCode == Keys.D8)
                        {
                            var fromForwardLeft = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, -(vMax.X + 0.25f), vMax.Y + 1.25f, 0.1);
                            var toForwardLeft = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, -vMax.X, vMax.Y + 100.0f, 0.1f);

                            var fromForwardLeft1 = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, 0, vMax.Y + 1.25f, 0.1);
                            var toForwardLeft1 = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, 0, vMax.Y + 100.0f, 0.1f);

                            var fromForwardRight = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, (vMax.X + 0.25f), vMax.Y + 1.25f, 0.1);
                            var toForwardRight = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, vMax.X, vMax.Y + 100.0f, 0.1f);

                            Function.Call(Hash.SHOOT_SINGLE_BULLET_BETWEEN_COORDS, fromForwardLeft.X, fromForwardLeft.Y, fromForwardLeft.Z, toForwardLeft.X, toForwardLeft.Y, toForwardLeft.Z, 250, 1, weaponAssetRocket, Game.Player.Character.Handle, 1, 0, -1.0);
                            Function.Call(Hash.SHOOT_SINGLE_BULLET_BETWEEN_COORDS, fromForwardLeft1.X, fromForwardLeft1.Y, fromForwardLeft1.Z, toForwardLeft1.X, toForwardLeft1.Y, toForwardLeft1.Z, 250, 1, weaponAssetRocket, Game.Player.Character.Handle, 1, 0, -1.0);
                            Function.Call(Hash.SHOOT_SINGLE_BULLET_BETWEEN_COORDS, fromForwardRight.X, fromForwardRight.Y, fromForwardRight.Z, toForwardRight.X, toForwardRight.Y, toForwardRight.Z, 250, 1, weaponAssetRocket, Game.Player.Character.Handle, 1, 0, -1.0);
                        }
                        else if (e.KeyCode == Keys.D9)
                        {
                            var per = (vMax.X * 2f + 20f) / 10f;
                            for (int i = 1; i <= 10; i++)
                            {
                                var fromForwardLeft = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, 0, vMax.Y + 1.25f, 0.1);
                                var toForwardLeft = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, -vMax.X - 10f + per * i, vMax.Y + 100.0f, 0.1f);

                                Function.Call(Hash.SHOOT_SINGLE_BULLET_BETWEEN_COORDS, fromForwardLeft.X, fromForwardLeft.Y, fromForwardLeft.Z, toForwardLeft.X, toForwardLeft.Y, toForwardLeft.Z, 250, 1, weaponAssetRocket, Game.Player.Character.Handle, 1, 0, -1.0);
                            }

                            var fromBackwardLeft = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, -(vMax.X + 0.25f), vMin.Y + 1.25f, 0.1);
                            var toBackwardLeft = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, -vMax.X, vMin.Y - 100.0f, 0.1f);

                            var fromBackwardRight = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, (vMax.X + 0.25f), vMin.Y + 1.25f, 0.1);
                            var toBackwardRight = Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, veh, vMax.X, vMin.Y - 100.0f, 0.1f);

                            Function.Call(Hash.SHOOT_SINGLE_BULLET_BETWEEN_COORDS, fromBackwardLeft.X, fromBackwardLeft.Y, fromBackwardLeft.Z, toBackwardLeft.X, toBackwardLeft.Y, toBackwardLeft.Z, 250, 1, weaponAssetRocket, Game.Player.Character.Handle, 1, 0, -1.0);
                            Function.Call(Hash.SHOOT_SINGLE_BULLET_BETWEEN_COORDS, fromBackwardRight.X, fromBackwardRight.Y, fromBackwardRight.Z, toBackwardRight.X, toBackwardRight.Y, toBackwardRight.Z, 250, 1, weaponAssetRocket, Game.Player.Character.Handle, 1, 0, -1.0);
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

        static uint lastVehicleHash = 0;

        static bool userInvincible = false;
        static bool carInvincible = false;
        static bool wrapInSpawned = false;
        //static bool seatbelt = false;

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

        public static void SpawnCar(string vehicleName, bool log = false)
        {
            var hash = Enum.Parse(typeof(VehicleHash), vehicleName, true);
            if (hash != null)
                SpawnCar(Convert.ToUInt32(hash), log);
        }

        public static void SpawnCar(uint hash, bool log = false)
        {
            Vehicle vehicle = World.CreateVehicle((VehicleHash)hash, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 8, Game.Player.Character.Heading + 90);
            vehicle.CanTiresBurst = false;
            vehicle.PlaceOnGround();
            vehicle.IsInvincible = carInvincible;
            vehicle.NumberPlate = "HELLO";

            if (wrapInSpawned)
            {
                lastVehicleHash = hash;
                Function.Call(Hash.SET_ENTITY_HEADING, vehicle.Handle, Game.Player.Character.Heading);
                Function.Call(Hash.SET_PED_INTO_VEHICLE, Game.Player.Character.Handle, vehicle.Handle, -1);
                if (log)
                {
                    LogCarInfo(vehicle);
                }
            }
        }

        static void LogCarInfo(Vehicle vehicle)
        {
            LogCarInfo(vehicle.DisplayName, vehicle.FriendlyName, vehicle.Handle);
        }

        static void LogCarInfo(string name, string friendlyName, int hash)
        {
            logger.Log(friendlyName + "," + name + "," + hash);
        }
    }
}