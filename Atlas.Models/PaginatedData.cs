﻿using System;
using System.Collections.Generic;

namespace Atlas.Models
{
    public class PaginatedData<T> where T : class
    {
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public string Search { get; set; }

        public IList<T> Items { get; set; } = new List<T>();

        public PaginatedData()
        {
            
        }

        public PaginatedData(IList<T> items, int totalRecords, int pageSize, string search = null)
        {
            Items = items;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(TotalRecords / (decimal)pageSize);
            Search = search;
        }
    }
}