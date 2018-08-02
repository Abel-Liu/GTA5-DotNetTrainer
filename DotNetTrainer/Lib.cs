using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetTrainer
{
    public class Logger
    {
        public void Log(object message)
        {
            File.AppendAllText("DotNetTrainer.log", DateTime.Now.ToString() + "  " + message + "\r\n");
        }
    }

    public class VehicleInformation
    {
        public string DisplayName { get; set; }
        public string FriendlyName { get; set; }
        public int Hash { get; set; }
    }

    public class VehicleInformationEqualityComparer : IEqualityComparer<VehicleInformation>
    {
        public bool Equals(VehicleInformation x, VehicleInformation y)
        {
            return x.Hash == y.Hash;
        }

        public int GetHashCode(VehicleInformation obj)
        {
            return obj.Hash.GetHashCode();
        }
    }

    public enum DrawTypeEnum
    {
        /// <summary>
        /// 普通菜单
        /// </summary>
        NormalMenu = 0,

        /// <summary>
        /// 铺在屏幕中的横向列表
        /// </summary>
        ScreenList = 1
    }

    public interface IOperate
    {
        bool IsMenu { get; }
    }

    public class BaseEntity
    {
        public string Identity { get; set; }
    }

    /// <summary>
    /// 一个菜单，包含若干菜单项
    /// </summary>
    public class Menu : BaseEntity, IOperate
    {
        /// <summary>
        /// 从某个菜单项进入此菜单
        /// </summary>
        public MenuItem FromItem { get; set; }

        public DrawTypeEnum DrawType { get; set; }

        /// <summary>
        /// IOperate成员，指示当前IOperate是打开一个菜单还是执行某个操作
        /// </summary>
        public bool IsMenu => true;

        private List<MenuItem> items = new List<MenuItem>();

        /// <summary>
        /// 包含的菜单项
        /// </summary>
        public List<MenuItem> Items
        {
            get
            {
                foreach (var i in items)
                    i.Menu = this;
                return items;
            }
            set { items = value; }
        }
    }

    /// <summary>
    /// 菜单中的一项，包含标题、指向的操作等
    /// </summary>
    public class MenuItem : BaseEntity
    {
        /// <summary>
        /// 所属菜单，不用手动赋值
        /// </summary>
        public Menu Menu { get; set; }

        /// <summary>
        /// 标题，若GetTitle为null则显示此标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 需要动态显示的标题
        /// </summary>
        public Func<string> GetTitle { get; set; }

        /// <summary>
        /// 处于选中状态
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// 指向的操作，打开菜单(Menu) 或 执行操作(PointAction)
        /// </summary>
        public IOperate PointTo { get; set; }
    }

    /// <summary>
    /// 菜单项，特用于传送到某处
    /// </summary>
    public class TeleportMenuItem : MenuItem
    {
        public TeleportMenuItem(string placeName, float x, float y, float z)
        {
            Title = placeName;
            PointTo = new PointAction()
            {
                Action = () =>
                {
                    DotNetTrainerScript.TeleportPlayer(x, y, z);
                }
            };
        }
    }

    /// <summary>
    /// 菜单项，特用于获取某车辆
    /// </summary>
    public class SpawnCarMenuItem : MenuItem
    {
        public SpawnCarMenuItem(VehicleHash carHash, string frendlyName)
        {
            Title = frendlyName;
            PointTo = new VehicleSpawnAction(carHash);
        }

        public SpawnCarMenuItem(uint carHash, string frendlyName)
        {
            Title = frendlyName;
            PointTo = new VehicleSpawnAction(carHash);
        }

        public SpawnCarMenuItem(string carName, string frendlyName)
        {
            Title = frendlyName;
            //try
            //{
            //    var hash = Enum.Parse(typeof(VehicleHash), carName, true);
            //    if (hash != null)
            //        PointTo = new VehicleSpawnAction(Convert.ToUInt32(hash));
            //}
            //catch (Exception e)
            //{
            //    PointTo = new VehicleSpawnAction(carName);
            //}

            PointTo = new VehicleSpawnAction(carName);
        }

        public SpawnCarMenuItem(string carName)
            : this(carName, carName)
        { }
    }

    /// <summary>
    /// 菜单项点击后不打开新菜单，而是执行某个操作
    /// </summary>
    public class PointAction : BaseEntity, IOperate
    {
        public bool IsMenu => false;

        public Action Action { get; set; }

        /// <summary>
        /// Default false
        /// </summary>
        public bool CloseAllMenuAfterAction { get; set; } = false;
    }

    /// <summary>
    /// 获取车辆的操作
    /// </summary>
    public class VehicleSpawnAction : PointAction
    {
        public VehicleSpawnAction(VehicleHash vehicleHash)
            : this((uint)vehicleHash)
        { }

        public VehicleSpawnAction(uint vehicleHash)
        {
            CloseAllMenuAfterAction = true;
            Action = () =>
            {
                DotNetTrainerScript.SpawnCar(vehicleHash, true);
            };
        }

        public VehicleSpawnAction(string vehicleName)
        {
            CloseAllMenuAfterAction = true;
            Action = () =>
            {
                DotNetTrainerScript.SpawnCar(vehicleName, true);
            };
        }
    }

}