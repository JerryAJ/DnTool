﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnTool.Utilities;
using Utilities.Dm;
using System.Diagnostics;
using DnTool.Models;
using Utilities.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using DnTool.GameTask;
using Utilities.Log;
using MahApps.Metro.Controls.Dialogs;
namespace DnTool.ViewModels
{
    public class BuyViewModel:NotifyPropertyChanged
    {
        ViewModelLocator Locator=new ViewModelLocator();

        public RelayCommand ShuaHuoshanCommand { get; set; }
        public RelayCommand DetectCommand { get; set; }
        public RelayCommand OpenCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand<MallThing> BuyCommand { get; set; }
        private void InitThings()
        {
            this.Things.Add(new MallThing() { ID = 1, Name = "物品保护魔法药", Description = "强化+9至+12保护",CanUseLB=true,Value=100000});
            this.Things.Add(new MallThing() { ID = 2, Name = "钻石潘多拉之心", Description = "随机获得各种物品", CanUseLB = true, Value = 40000 });
            this.Things.Add(new MallThing() { ID = 3, Name = "钻石潘多拉火种", Description = "打开钻石潘多拉之心", CanUseLB = true, Value = 1 });
            this.Things.Add(new MallThing() { ID = 4, Name = "阿尔杰塔的礼物", Description = "随机获得各种物品", CanUseLB = false, Value = 328000 });
            this.Things.Add(new MallThing() { ID = 5, Name = "10000龙币商品券", Description = "10000龙币", CanUseLB = true, Value = 100000 });
            this.Things.Add(new MallThing() { ID = 6, Name = "柏林的感谢口袋", Description = "最高级阿尔泰丶最高级钻石丶生命的精髓各100个", CanUseLB = true, Value = 80000 });
            this.Things.Add(new MallThing() { ID = 7, Name = "龙裔特别口袋", Description = "70龙玉,70纹章，女神的叹息", CanUseLB = true, Value = 30000 });
            this.Things.Add(new MallThing() { ID = 8, Name = "装有富饶护符的箱子", Description = "额外金币加成", CanUseLB = false, Value = 60000 });
        }
        
        public BuyViewModel()
        {
            InitThings();
            this.BuyCommand = new RelayCommand<MallThing>(
                (t)=>this.Buy(t),
                (t)=>t!=null&&SoftContext.Role!=null);

            this.StopCommand = new RelayCommand(() =>
            {
                SoftContext.TaskEngine.Stop();
            });
            this.OpenCommand = new RelayCommand(() =>
            {
                try
                {
                    Process[] all = Process.GetProcessesByName("DragonNest");
                    if (all != null)
                    {
                        if (all.Length == 0)
                        {
                            return;
                        }
                        foreach (Process process in all)
                        {
                            var handles = Win32Processes.GetHandles(process, "Mutant", "\\BaseNamedObjects\\MutexDragonNest");
                            if (handles.Count == 0)
                            {
                                continue;
                            }
                            foreach (var handle in handles)
                            {
                                IntPtr ipHandle = IntPtr.Zero;
                                if (!MutexCloseHelper.DuplicateHandle(Process.GetProcessById(handle.ProcessID).Handle,
                                    handle.Handle, MutexCloseHelper.GetCurrentProcess(), out ipHandle, 0, false, MutexCloseHelper.DUPLICATE_CLOSE_SOURCE))
                                {
                                    // richTextBox1.AppendText("DuplicateHandle() failed, error =" + Marshal.GetLastWin32Error() + Environment.NewLine);
                                }
                                else
                                {
                                    MutexCloseHelper.CloseHandle(ipHandle);
                                    Debug.WriteLine("进程[" + handle.ProcessID + "]的互斥体句柄关闭成功");
                                }
                            }
                        }
                    }
                    else
                    {
                        // richTextBox1.AppendText("没有找到运行的程序" + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });

          
        }
        private async void Buy(MallThing thing)
        {
            TaskContext context = new TaskContext(SoftContext.Role);
        
            /// 任务设置，可用属性为：.Thing .Num .UseLB
            context.Settings.Thing = thing;
            context.Settings.Num = this._number;
            context.Settings.UseLB = this._useLB;
            
            TaskBase task = new BuyThingsTask(context);
            task.Name = "购买商城物品";
            int width = context.Role.Window.Width;
            int height = context.Role.Window.Height;
            if (width != 1152 || height != 864)
            {
                await SoftContext.MainWindow.ShowMessageAsync("购买失败", "请将游戏分辨率设为1152*864！");
                return;
            }
            if(this._number<=0)
            {
                await SoftContext.MainWindow.ShowMessageAsync("购买失败", "请检查物品数量！");
                return;
            }
            SoftContext.TaskEngine.Start(task);
        }

   
        private List<MallThing> _things=new List<MallThing>();

        public List<MallThing> Things
        {
            get { return _things; }
            set
            {
                _things = value;
                this.OnPropertyChanged("Things");
            }
        }

        private int _number;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        private bool _useLB;

        public bool UseLB
        {
            get { return _useLB; }
            set
            {
                base.SetProperty(ref _useLB, value, () => this.UseLB);
            }
        }
        
    }
}
