using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DotNetTrainer
{
    public class Logger
    {
        public void Log(string message)
        {
            File.AppendAllText("DotNetTrainer.log", DateTime.Now.ToString() + "  " + message + "\r\n");
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

    public class SpawnCarMenuItem : MenuItem
    {
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
        int hash = 0;

        public VehicleSpawnAction(VehicleHash vehicleHash)
            : this((uint)vehicleHash)
        { }

        public VehicleSpawnAction(uint vehicleHash)
        {
            CloseAllMenuAfterAction = true;
            Action = () =>
            {
                DotNetTrainerScript.SpawnCar(vehicleHash);
            };
        }
    }

}