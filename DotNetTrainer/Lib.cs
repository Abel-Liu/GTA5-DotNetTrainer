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
        NormalMenu = 0,
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

    public class Menu : BaseEntity, IOperate
    {
        public MenuItem FromItem { get; set; }

        public DrawTypeEnum DrawType { get; set; }

        public bool IsMenu => true;

        private List<MenuItem> items = new List<MenuItem>();

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

    public class MenuItem : BaseEntity
    {
        /// <summary>
        /// 不用手动赋值
        /// </summary>
        public Menu Menu { get; set; }

        public string Title { get; set; }

        public Func<string> GetTitle { get; set; }

        /// <summary>
        /// 处于选中状态
        /// </summary>
        public bool Selected { get; set; } = false;

        public IOperate PointTo { get; set; }
    }

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

    public class SpawnCarMenuItem : MenuItem
    {
        public SpawnCarMenuItem(uint carHash, string frendlyName)
        {
            Title = frendlyName;
            PointTo = new VehicleSpawnAction(carHash);
        }

        public SpawnCarMenuItem(string carName, string frendlyName)
        {
            try
            {
                var hash = Enum.Parse(typeof(VehicleHash), carName, true);
                if (hash != null)
                {
                    Title = frendlyName;
                    PointTo = new VehicleSpawnAction(Convert.ToUInt32(hash));
                }
            }
            catch { }
        }

        public SpawnCarMenuItem(string carName)
            : this(carName, carName)
        { }
    }

    public class PointAction : BaseEntity, IOperate
    {
        public bool IsMenu => false;

        public Action Action { get; set; }

        /// <summary>
        /// Default false
        /// </summary>
        public bool CloseAllMenuAfterAction { get; set; } = false;
    }

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
    }

}