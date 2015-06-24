﻿using System;
using DnTool.Utilities;
using Utilities.Dm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GalaSoft.MvvmLight.CommandWpf;
namespace DnTool.ViewModels
{
    public class MainViewModel:NotifyPropertyChanged
    {

       //  private int roleBaseAddress = 0x1221740;//游戏内存基址
       // private int moneyBaseAddress = 0x16D1E50;//背包金钱基址
        private string processName = "DragonNest";//游戏进程名字
        public static int Hwnd = 0;
        private DmPlugin dm = new DmPlugin();
        public RelayCommand ExitLoginCommand { get; set; }
        public RelayCommand ShowLoginCommand { get; set; }
        ViewModelLocator Locator = new ViewModelLocator();
        public MainViewModel()
        {

            this.ExitLoginCommand = new RelayCommand(()=>
            {
                Locator.Login.IsOpen = true;
                SoftContext.IsLogin = false;
            }
            ,()=>SoftContext.IsLogin);

            //IRole role = new Role(0);
            //var v1 = ((Role)role).MallVolume;
            //var v2 = ((Role)role).BagMoney;
            //var v3 = ((Role)role).MallLB;
            //var v4 = ((Role)role).RepertoryMoney;
            //this.SaveListCommand = new RelayCommand(() =>
            //{
            //    foreach (var item in this.InfoList)
            //    {
            //      //  FileOperateHelper.WriteFile("",string.Format("{0}#{1}#{2}#{3}",item.Name,item.CurrentPoint.X,item.CurrentPoint.Y,item.CurrentPoint.Z));
            //    }
            //});
 
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
           
    
            this.LoadedCommand = new RelayCommand(() =>
            {
               

              //
               // WinIo.Initialize();
               
            });


            Memory64Helper m = new Memory64Helper();

            //List<int> moduleAddrs = new List<int>();
            //string hwndString=dm.EnumWindowByProcess("DragonNest.exe","","DRAGONNEST",2);
            //List<int> hwndList=dm.GetHwnds(hwndString);
            //foreach (var hwnd in hwndList)
            //{
            //    int base_addr=dm.GetModuleBaseAddr(hwnd, "DragonNest.exe");
            //    if (base_addr > 0)
            //        moduleAddrs.Add(base_addr);
            //}



            this.ClosedCommand = new RelayCommand(() =>
            {
                TeleportViewModel.timer.Stop();
            });
          

        }
      
     
        
        #region 方法



        private bool IsAlive(int hwnd)
        {
            return dm.GetWindowState(hwnd, 0) == 1;
        }

      

        //读取制定内存中的值
        public Int64 ReadMemoryValue(int baseAdd)
        {
            return MemoryHelper.ReadMemoryValue(baseAdd, processName);
        }

        //将值写入指定内存中
        public void WriteMemory(int baseAdd, int value)
        {
            MemoryHelper.WriteMemoryValue(baseAdd, processName, value);
        }
        #endregion

        #region 命令
        public RelayCommand OpenCommand { get; set; }
        public RelayCommand UnBindCommand { get; set; }
        public RelayCommand AddNewPointCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        public RelayCommand ClosedCommand { get; set; }
        public RelayCommand SavePointCommand { get; set; }
        public RelayCommand MoveCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand ModifyCommand { get; set; }
        public RelayCommand ImportListCommand { get; set; }
        public RelayCommand SaveListCommand { get; set; }
        public RelayCommand Redo { get; set; }
        public RelayCommand Undo { get; set; }
        public RelayCommand ClearCommand { get; set; }
        #endregion

        #region 数据
        private ObservableCollection<FlyoutBaseViewModel> _flyouts=new ObservableCollection<FlyoutBaseViewModel>();

        public ObservableCollection<FlyoutBaseViewModel> Flyouts
        {
            get { return _flyouts; }
        }
        
        

       
    
        

         private int _currentHwnd;

         public int CurrentHwnd
         {
             get { return _currentHwnd; }
             set
             {
                if (_currentHwnd != value)
                {
                    _currentHwnd = value;
                   
                }
             }
         }
         

        #endregion
    }
}
