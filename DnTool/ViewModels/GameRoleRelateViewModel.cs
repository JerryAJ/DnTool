﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
namespace DnTool.ViewModels
{
    public class GameRoleRelateViewModel:FlyoutBaseViewModel
    {
        public GameRoleRelateViewModel()
        {
            this.Header = "未关联角色";
            this.Position = Position.Right;
        }
    }
}
