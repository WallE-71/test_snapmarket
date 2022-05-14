using System;

namespace SnapMarket.ViewModels
{
    public class ResultViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }  
    
    public class ResultViewModel<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
