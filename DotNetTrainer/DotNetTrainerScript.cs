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

        /// <summary>
        /// 所有菜单，包括子菜单
        /// </summary>
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
            weaponNoReload = true;

            InitMenu();
        }

        void InitMenu()
        {
            #region player

            var place = new Menu() { DrawType = DrawTypeEnum.NormalMenu, };
            place.Items.Add(
                new MenuItem()
                {
                    Title = "Marker",
                    PointTo = new PointAction()
                    {
                        Action = () => { TeleportToMarker(); }
                    }
                }
            );
            place.Items.Add(new TeleportMenuItem("MICHAEL'S HOUSE", -852.4f, 160.0f, 65.6f));
            place.Items.Add(new TeleportMenuItem("FRANKLIN'S HOUSE", 7.9f, 548.1f, 175.5f));
            place.Items.Add(new TeleportMenuItem("TREVOR'S TRAILER", 1985.7f, 3812.2f, 32.2f));
            place.Items.Add(new TeleportMenuItem("STRIPCLUB", 127.4f, -1307.7f, 29.2f));
            place.Items.Add(new TeleportMenuItem("FERRIS WHEEL", -1670.7f, -1125.0f, 13.0f));
            place.Items.Add(new TeleportMenuItem("MILITARY BASE", -2047.4f, 3132.1f, 32.8f));
            place.Items.Add(new TeleportMenuItem("CHILLIAD", 425.4f, 5614.3f, 766.5f));
            place.Items.Add(new TeleportMenuItem("警察局", 807.2786f, -1277.551f, 25.96281f));

            var teleport = new MenuItem()
            {
                Title = "TELEPORT",
                PointTo = place
            };

            place.FromItem = teleport;

            var player = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "player",
                Items = new MenuItem[]
                {
                    teleport,
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
                    new MenuItem()
                    {
                        Title = "GET PLAYER POSITION",
                        PointTo = new PointAction()
                        {
                            Action = () =>
                            {
                                var p=Game.Player.Character.Position;
                                ShowStatusText(p.ToString());
                                logger.Log(p.X+"f,"+p.Y+"f,"+p.Z+"f");
                            }
                        }
                    }
                }.ToList()
            };

            #endregion

            #region vehicle

            var fixVehicle = new PointAction()
            {
                Action = () =>
                {
                    if (Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Game.Player.Character.Handle, 0))
                    {
                        Function.Call(Hash.SET_VEHICLE_FIXED, Function.Call<int>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character.Handle));
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

            //至尊慧眼,COGCABRI,57346                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
            //愛國者,PATRIOT,-808457413
            //愛快,ALPHA,57346
            //公路霸者,DOMINATO,80636076
            //貨機,CARGOPL,368211810
            //碼頭裝卸車,HANDLER,82434
            //FIB 公務車, FBI,154882
            //泰坦號,TITAN,73218

            spawner.Items.Add(new SpawnCarMenuItem("ZENTORNO"));
            spawner.Items.Add(new SpawnCarMenuItem("BANSHEE", "女妖"));
            spawner.Items.Add(new SpawnCarMenuItem("INSURGENT2", "叛亂份子"));
            spawner.Items.Add(new SpawnCarMenuItem("RHINO", "犀式坦克"));
            spawner.Items.Add(new SpawnCarMenuItem("BLAZER3", "四輪摩托"));
            spawner.Items.Add(new SpawnCarMenuItem("STRETCH", "加長轎車"));
            spawner.Items.Add(new SpawnCarMenuItem("HUNTLEY", "肯特利S"));
            spawner.Items.Add(new SpawnCarMenuItem("FQ2"));
            spawner.Items.Add(new SpawnCarMenuItem("TAXI"));
            spawner.Items.Add(new SpawnCarMenuItem("POLICE2"));

            spawner.Items.Add(new SpawnCarMenuItem("FBI2"));
            spawner.Items.Add(new SpawnCarMenuItem("ISSI2", "天威"));
            spawner.Items.Add(new SpawnCarMenuItem("PANTO", "微型汽車"));
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


            var spawnerEntry = new MenuItem() { Identity = "main_vehicle_spawner", Title = "CAR SPAWNER", PointTo = spawner };
            spawner.FromItem = spawnerEntry;

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
                }.ToList()
            };
            #endregion

            #region weapon


            var weapon = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "weapon",
                Items = new MenuItem[]
                {
                    new MenuItem()
                    {
                        Title ="GET ALL WEAPON",
                        PointTo = new PointAction()
                        {
                            Action = () =>
                            {
                                foreach(uint v in Enum.GetValues(typeof(WeaponHash)))
                                {
                                    Game.Player.Character.Weapons.Give((WeaponHash)v, 1000, false, true);
                                }
                            }
                        }
                    },
                    new MenuItem()
                    {
                        Title ="REMOVE ALL WEAPON",
                        PointTo = new PointAction()
                        {
                            Action = () =>
                            {
                                Game.Player.Character.Weapons.RemoveAll();
                            }
                        }
                    },
                    new MenuItem()
                    {
                        GetTitle = ()=>{ return "NO RELOAD  ["+ (weaponNoReload?"ON":"OFF") +"]";},
                        PointTo = new PointAction()
                        {
                            Action = () =>
                            {
                                weaponNoReload = !weaponNoReload;
                            }
                        }
                    },
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
            var main_weapon = new MenuItem() { Identity = "main_weapon", Title = "WEAPON", PointTo = weapon };
            var main_time = new MenuItem() { Identity = "main_time", Title = "TIME", PointTo = time };

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

            var main = new Menu()
            {
                DrawType = DrawTypeEnum.NormalMenu,
                Identity = "main",
                Items = new MenuItem[]
                {
                    main_player,
                    main_vehicle,
                    main_weapon,
                    main_time,
                    nearby,
                }.ToList()
            };
            #endregion

            vehicle.FromItem = main_vehicle;
            player.FromItem = main_player;
            time.FromItem = main_time;
            weapon.FromItem = main_weapon;

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

        void SetWeaponNoReload()
        {
            if (PlayerExists() && weaponNoReload)
            {
                var playerPed = Game.Player.Character.Handle;
                var o = new OutputArgument();
                if (Function.Call<bool>(Hash.GET_CURRENT_PED_WEAPON, playerPed, o, 1))
                {
                    var cur = o.GetResult<uint>();
                    if (Function.Call<bool>(Hash.IS_WEAPON_VALID, cur))
                    {
                        o = new OutputArgument();
                        if (Function.Call<bool>(Hash.GET_MAX_AMMO, playerPed, cur, o))
                        {
                            int maxAmmo = o.GetResult<int>();
                            Function.Call(Hash.SET_PED_AMMO, playerPed, cur, maxAmmo);

                            maxAmmo = Function.Call<int>(Hash.GET_MAX_AMMO_IN_CLIP, playerPed, cur, 1);
                            if (maxAmmo > 0)
                                Function.Call(Hash.SET_AMMO_IN_CLIP, playerPed, cur, maxAmmo);
                        }
                    }
                }
            }
        }

        private void onTick(object sender, EventArgs e)//每几毫秒调用
        {
            Draw();

            Game.Player.Character.IsInvincible = userInvincible;

            SetWeaponNoReload();

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
                case Keys.F3:
                    TeleportToMarker();
                    break;
                case Keys.F6:
                    SpawnCar("ZENTORNO");
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
                case Keys.D9://try amazing rocket
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

        static void ShowStatusText(string text)
        {
            statusText = text;
            statusTextShowTime = 0;
        }

        /// <summary>
        /// 画出需要显示的内容
        /// </summary>
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

                DrawMenuLine(title, normalMenuWidth, normalMenuHeight, 0, 0, 5, false, true);
                int idx = 0;
                foreach (var i in currentMenu.Items)
                {
                    idx++;
                    DrawMenuLine(i.GetTitle == null ? i.Title : i.GetTitle(), normalMenuWidth, normalMenuHeight, idx * normalMenuHeight, 0, 9, i.Selected, false);
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
                    DrawMenuLine(i.GetTitle == null ? i.Title : i.GetTitle(), listItemWidth, listItemHeight, 200, 50 + idx * 120, 55 + idx * 120, i.Selected, false);
                    idx++;
                }
            }
        }

        static uint lastVehicleHash = 0;

        static bool userInvincible = false;
        static bool carInvincible = false;
        static bool wrapInSpawned = false;
        static bool weaponNoReload = false;
        //static bool seatbelt = false;

        static string statusText = "";
        static int statusTextShowTime = 0;
        static int statusTextShowTimeMax = 80;

        const int normalMenuWidth = 250;
        const int normalMenuHeight = 30;

        const int listCountPerLine = 10;
        const int listItemWidth = 110;
        const int listItemHeight = 28;

        /// <summary>
        /// 根据计算好的位置画一个菜单项
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="lineWidth"></param>
        /// <param name="lineHeight"></param>
        /// <param name="lineTop"></param>
        /// <param name="lineLeft"></param>
        /// <param name="textLeft"></param>
        /// <param name="active"></param>
        /// <param name="title"></param>
        /// <param name="rescaleText"></param>
        void DrawMenuLine(string caption, int lineWidth, int lineHeight, int lineTop, int lineLeft, int textLeft, bool active, bool title, bool rescaleText = true)
        {
            var textColor = Color.FromArgb(255, 255, 255, 255);
            var rectColor = Color.FromArgb(255, 70, 95, 95);

            var activeTextColor = Color.FromArgb(255, 0, 0, 0);
            var activeRectColor = Color.FromArgb(255, 218, 242, 216);

            var titleRectColor = Color.FromArgb(255, 0, 0, 0);
            var titleTextColor = textColor;

            float text_scale = 0.30f;
            int font = 0;

            if (active && rescaleText)
            {
                text_scale = 0.35f;
            }

            if (title && rescaleText)
            {
                text_scale = 0.35f;
                font = 1;
            }

            new UIText(caption, new Point(textLeft, lineTop + 3), text_scale, title ? titleTextColor : (active ? activeTextColor : textColor), (GTA.Font)font, false).Draw();
            new UIRectangle(new Point(lineLeft, lineTop), new Size(lineWidth, lineHeight), title ? titleRectColor : (active ? activeRectColor : rectColor)).Draw();
        }

        void DrawStatusText(string text)
        {
            //new UIText(text, new Point(520, 52), 0.35f, Color.FromArgb(255, 255, 255, 255), 0, false).Draw();
            //var width = 20 + text.Length * 9;
            //if (width < 100)
            //    width = 100;
            //new UIRectangle(new Point(500, 50), new Size(width, 30), Color.FromArgb(255, 70, 95, 95)).Draw();

            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_SCALE, 0.55, 0.55);
            Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255);
            Function.Call(Hash.SET_TEXT_WRAP, 0.0, 1.0);
            Function.Call(Hash.SET_TEXT_CENTRE, 1);
            Function.Call(Hash.SET_TEXT_DROPSHADOW, 0, 0, 0, 0, 0);
            Function.Call(Hash.SET_TEXT_EDGE, 1, 0, 0, 0, 205);

            Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);

            Function.Call(Hash._DRAW_TEXT, 0.5, 0.5);
        }

        bool PlayerExists()
        {
            return Function.Call<bool>(Hash.DOES_ENTITY_EXIST, Game.Player.Character.Handle);
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

        static void TeleportToMarker()
        {
            int e = Game.Player.Character.Handle;
            if (Game.Player.Character.IsInVehicle())
                e = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character.Handle);

            bool blipFound = false;
            Vector3 coords = new Vector3();

            int blipIterator = Function.Call<int>(Hash._GET_BLIP_INFO_ID_ITERATOR);
            for (int i = Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, blipIterator); Function.Call<bool>(Hash.DOES_BLIP_EXIST, i); i = Function.Call<int>(Hash.GET_NEXT_BLIP_INFO_ID, blipIterator))
            {
                if (Function.Call<int>(Hash.GET_BLIP_INFO_ID_TYPE, i) == 4)
                {
                    coords = Function.Call<Vector3>(Hash.GET_BLIP_INFO_ID_COORD, i);
                    blipFound = true;
                    break;
                }
            }

            if (blipFound)
            {
                // load needed map region and check height levels for ground existence
                bool groundFound = false;
                float[] groundCheckHeight =
                    {
                        100.0f, 150.0f, 50.0f, 0.0f, 200.0f, 250.0f, 300.0f, 350.0f, 400.0f,
                        450.0f, 500.0f, 550.0f, 600.0f, 650.0f, 700.0f, 750.0f, 800.0f
                    };

                for (int i = 0; i < groundCheckHeight.Length; i++)
                {
                    Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, e, coords.X, coords.Y, groundCheckHeight[i], 0, 0, 1);
                    Wait(100);
                    var z = new OutputArgument();
                    if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, coords.X, coords.Y, groundCheckHeight[i], z))
                    {
                        groundFound = true;
                        coords.Z = z.GetResult<float>() + 3.0f;
                        break;
                    }
                }

                // if ground not found then set Z in air and give player a parachute
                if (!groundFound)
                {
                    coords.Z = 1000.0f;
                    Function.Call(Hash.GIVE_DELAYED_WEAPON_TO_PED, Game.Player.Character.Handle, 0xFBAB5776, 1, 0);
                }

                TeleportPlayer(coords.X, coords.Y, coords.Z);
            }
            else
            {
                ShowStatusText("Map marker isn't set");
            }
        }

        public static void TeleportPlayer(float x, float y, float z)
        {
            int e = Game.Player.Character.Handle;
            if (Game.Player.Character.IsInVehicle())
                e = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character.Handle);

            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, e, x, y, z, false, false, true);
        }
    }
}