using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WinApp
{
    public class BaseViewModel : Document
    {
        public virtual void Dispose() 
        { 
        }
    }
    public abstract class AsyncViewModel : BaseViewModel
    {
        public abstract bool IsReady { get; }
        public abstract void Execute();
        public abstract object GetDataContext();
    }
}
