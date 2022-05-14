﻿using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnapMarket.ViewModels
{
    //public class BaseViewModel<TEntity> where TEntity : class
    public class BaseViewModel<TKey>
    {
        [JsonPropertyName("Id")]
        public TKey Id { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [Display(Name = "نام"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Name { get; set; }

        [Display(Name = "تکمیل")]
        public bool? IsComplete { get; set; } = false;

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [JsonIgnore]
        public DateTime? UpdateTime { get; set; }

        [Display(Name = "زمان درج/ایجاد")]
        public DateTime? InsertTime { get; set; }

        [Display(Name = "زمان درج/ایجاد")]
        public string PersianInsertTime { get; set; }

        //[JsonPropertyName("data")]
        //public virtual TEntity Data { get; set; }

        //[JsonPropertyName("dataList")]
        //public virtual List<TEntity> Datas { get; set; }
    }
}
